using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using V2RayGCon.Resource.Resx;

namespace V2RayGCon.Controller
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class CoreServerCtrl : VgcPlugin.Models.ICoreCtrl
    {
        [JsonIgnore]
        Service.Cache cache;
        [JsonIgnore]
        Service.Servers servers;
        [JsonIgnore]
        Service.Setting setting;

        #region ICoreCtrl interface
        public string GetConfig() => this.config;
        public bool IsCoreRunning() => this.isServerOn;
        public bool IsUntrack() => this.isUntrack;
        #endregion

        public event EventHandler<Model.Data.StrEvent> OnLog;
        public event EventHandler
            OnPropertyChanged,
            OnRequireStatusBarUpdate,
            OnRequireMenuUpdate,
            OnRequireNotifierUpdate;

        /// <summary>
        /// false: stop true: start
        /// </summary>
        public event EventHandler<Model.Data.BoolEvent> OnRequireKeepTrack;

        public string config; // plain text of config.json
        public bool isAutoRun, isInjectImport, isSelected, isInjectSkipCNSite, isUntrack;
        public string name, summary, inboundIP, mark;
        public int overwriteInboundType, inboundPort, foldingLevel;
        public double index;

        public CoreServerCtrl()
        {
            // new server will displays at the bottom
            index = double.MaxValue;

            isSelected = false;
            isUntrack = false;
            isServerOn = false;
            isAutoRun = false;
            isInjectImport = false;

            foldingLevel = 0;

            mark = string.Empty;
            status = string.Empty;
            name = string.Empty;
            summary = string.Empty;
            config = string.Empty;
            speedTestResult = -1;

            overwriteInboundType = 1;
            inboundIP = "127.0.0.1";
            inboundPort = 1080;
        }

        public void Run(
             Service.Cache cache,
             Service.Setting setting,
             Service.Servers servers)
        {
            this.cache = cache;
            this.servers = servers;
            this.setting = setting;

            server = new Service.Core(setting);
            server.OnLog += OnLogHandler;
            server.OnCoreStatusChanged += OnCoreStateChangedHandler;
        }

        #region non-serialize properties
        [JsonIgnore]
        public long speedTestResult;

        [JsonIgnore]
        Views.WinForms.FormSingleServerLog logForm = null;

        [JsonIgnore]
        public string status;

        [JsonIgnore]
        public bool isServerOn;

        [JsonIgnore]
        public Service.Core server;

        [JsonIgnore]
        ConcurrentQueue<string> _logCache = new ConcurrentQueue<string>();

        [JsonIgnore]
        public string LogCache
        {
            get
            {
                return string.Join(Environment.NewLine, _logCache)
                    + System.Environment.NewLine;
            }
            private set
            {
                // keep 200 lines of log
                if (_logCache.Count > 300)
                {
                    var blackHole = "";
                    for (var i = 0; i < 100; i++)
                    {
                        _logCache.TryDequeue(out blackHole);
                    }
                }
                _logCache.Enqueue(value);
            }
        }
        #endregion

        #region public method
        public void SetIPandPortOnDemand(string ip, int port)
        {
            var changed = false;

            if (ip != this.inboundIP)
            {
                this.inboundIP = ip;
                changed = true;
            }

            if (port != this.inboundPort)
            {
                this.inboundPort = port;
                changed = true;

            }

            if (changed)
            {
                InvokeEventOnPropertyChange();

            }
        }

        public bool IsSuitableToBeUsedAsSysProxy(
            bool isGlobal,
            out bool isSocks,
            out int port)
        {
            isSocks = false;
            port = 0;

            var inboundInfo = GetParsedInboundInfo();
            if (inboundInfo == null)
            {
                SendLog(I18N.GetInboundInfoFail);
                return false;
            }

            var protocol = inboundInfo.Item1;
            port = inboundInfo.Item3;

            if (!IsProtocolMatchProxyRequirment(isGlobal, protocol))
            {
                return false;
            }

            isSocks = protocol == "socks";
            return true;
        }

        public void SetIsSelected(bool selected)
        {
            if (selected == isSelected)
            {
                return;
            }
            this.isSelected = selected;
            InvokeEventOnRequireStatusBarUpdate();
            InvokeEventOnPropertyChange();
        }

        public bool ShowLogForm()
        {
            if (logForm != null)
            {
                return false;
            }
            logForm = new Views.WinForms.FormSingleServerLog(this);

            logForm.FormClosed += (s, a) =>
            {
                logForm.Dispose();
                logForm = null;
            };
            return true;
        }

        public void RunSpeedTest()
        {
            void log(string msg)
            {
                SendLog(msg);
                SetPropertyOnDemand(ref status, msg);
            }

            var port = Lib.Utils.GetFreeTcpPort();
            var config = PrepareSpeedTestConfig(port);

            if (string.IsNullOrEmpty(config))
            {
                log(I18N.DecodeImportFail);
                return;
            }

            var url = StrConst.SpeedTestUrl;
            var text = I18N.Testing;
            log(text);
            SendLog(url);

            var speedTester = new Service.Core(setting);
            speedTester.title = GetTitle();
            speedTester.OnLog += OnLogHandler;
            speedTester.RestartCore(config);

            this.speedTestResult = Lib.Utils.VisitWebPageSpeedTest(url, port);

            text = string.Format("{0}",
                speedTestResult < long.MaxValue ?
                speedTestResult.ToString() + "ms" :
                I18N.Timeout);

            log(text);
            speedTester.StopCore();
            speedTester.OnLog -= OnLogHandler;
        }

        public void SetPropertyOnDemand(ref string property, string value, bool isNeedCoreStopped = false)
        {
            SetPropertyOnDemand<string>(ref property, value, isNeedCoreStopped);
        }

        public void SetPropertyOnDemand(ref int property, int value, bool isNeedCoreStopped = false)
        {
            SetPropertyOnDemand<int>(ref property, value, isNeedCoreStopped);
        }

        public void SetPropertyOnDemand(ref double property, double value, bool isNeedCoreStopped = false)
        {
            SetPropertyOnDemand<double>(ref property, value, isNeedCoreStopped);
        }

        public void SetPropertyOnDemand(ref bool property, bool value, bool isNeedCoreStopped = false)
        {
            SetPropertyOnDemand<bool>(ref property, value, isNeedCoreStopped);
        }

        public void ToggleBoolPropertyOnDemand(ref bool property, bool requireRestart = false)
        {
            property = !property;

            // refresh UI immediately
            InvokeEventOnPropertyChange();

            // time consuming things
            if (requireRestart && isServerOn)
            {
                RestartCoreThen();
            }
        }

        public void ToggleIsInjectImport()
        {
            this.isInjectImport = !this.isInjectImport;

            // refresh UI immediately
            InvokeEventOnPropertyChange();

            // time consuming things
            if (isServerOn)
            {
                RestartCoreThen();
            }

            UpdateSummaryThen(() => InvokeEventOnRequireMenuUpdate());
        }

        public void SetOverwriteInboundType(int type)
        {
            if (this.overwriteInboundType == type)
            {
                return;
            }

            this.overwriteInboundType = Lib.Utils.Clamp(
                type, 0, Model.Data.Table.inboundOverwriteTypesName.Length);

            InvokeEventOnPropertyChange();
            if (isServerOn)
            {
                // time consuming things
                RestartCoreThen();
            }
        }

        public void UpdateSummaryThen(Action lambda = null)
        {
            Task.Factory.StartNew(() =>
            {
                var configString = isInjectImport ?
                    InjectGlobalImport(this.config, false, true) :
                    this.config;
                try
                {
                    UpdateSummary(
                        servers.ParseImport(configString));
                }
                catch
                {
                    UpdateSummary(JObject.Parse(configString));
                }

                // update summary should not clear status
                // this.status = string.Empty;
                InvokeEventOnPropertyChange();
                lambda?.Invoke();
            });
        }

        public void CleanupThen(Action next)
        {
            this.server.StopCoreThen(() =>
            {
                this.server.OnLog -= OnLogHandler;
                this.server.OnCoreStatusChanged -= OnCoreStateChangedHandler;
                Task.Factory.StartNew(() =>
                {
                    next?.Invoke();
                });
            });
        }

        public void ChangeConfig(string config)
        {
            if (this.config == config)
            {
                return;
            }

            this.config = config;
            InvokeEventOnPropertyChange();
            UpdateSummaryThen(() =>
            {
                InvokeEventOnRequireMenuUpdate();
            });

            if (server.isRunning)
            {
                RestartCoreThen();
            }
        }

        public void StopCoreThen(Action next = null)
        {
            Task.Factory.StartNew(() => server.StopCoreThen(
                () =>
                {
                    OnRequireNotifierUpdate?.Invoke(this, EventArgs.Empty);
                    OnRequireKeepTrack?.Invoke(this, new Model.Data.BoolEvent(false));
                    next?.Invoke();
                }));
        }

        public void RestartCoreThen(Action next = null)
        {
            Task.Factory.StartNew(() => RestartCoreWorker(next));
        }

        public void GetterInboundInfoFor(Action<string> next)
        {
            var serverName = this.name;
            Task.Factory.StartNew(() =>
            {
                var inInfo = GetParsedInboundInfo();
                if (inInfo == null)
                {
                    next(string.Format("[{0}]", serverName));
                    return;
                }
                if (string.IsNullOrEmpty(inInfo.Item2))
                {
                    next(string.Format("[{0}] {1}", serverName, inInfo.Item1));
                    return;
                }
                next(string.Format("[{0}] {1}://{2}:{3}",
                    serverName,
                    inInfo.Item1,
                    inInfo.Item2,
                    inInfo.Item3));
            });
        }

        public void SetMark(string mark)
        {
            if (this.mark == mark)
            {
                return;
            }

            this.mark = mark;
            if (!string.IsNullOrEmpty(mark)
                && !(this.servers.GetMarkList().Contains(mark)))
            {
                this.servers.UpdateMarkList();
            }
            InvokeEventOnPropertyChange();
        }

        public void InvokeEventOnPropertyChange()
        {
            // things happen while invoking
            try
            {
                OnPropertyChanged?.Invoke(this, EventArgs.Empty);
            }
            catch { }
        }

        public void ChangeIndex(double index)
        {
            if (Lib.Utils.AreEqual(this.index, index))
            {
                return;
            }

            this.index = index;
            this.server.title = GetTitle();
            InvokeEventOnPropertyChange();
        }

        public bool GetterInfoFor(Func<string[], bool> filter)
        {
            return filter(new string[] {
                // index 0
                name+summary,

                // index 1
                GetInProtocolNameByNumber(overwriteInboundType)
                +inboundIP
                +inboundPort.ToString(),

                // index 2
                this.mark??"",
            });
        }

        public string GetTitle()
        {
            var result = string.Format("{0}.[{1}] {2}",
                (int)this.index,
                this.name,
                this.summary);
            return Lib.Utils.CutStr(result, 60);
        }
        #endregion

        #region private method
        string InjectGlobalImport(string config, bool isIncludeSpeedTest, bool isIncludeActivate)
        {
            JObject import = Lib.Utils.ImportItemList2JObject(
                setting.GetGlobalImportItems(),
                isIncludeSpeedTest,
                isIncludeActivate);

            Lib.Utils.MergeJson(ref import, JObject.Parse(config));
            return import.ToString();
        }

        string PrepareSpeedTestConfig(int port)
        {
            var empty = string.Empty;
            if (port <= 0)
            {
                return empty;
            }

            var config = GetDecodedConfig(true, true, false);

            if (config == null)
            {
                return empty;
            }

            if (!OverwriteInboundSettings(
                ref config,
                (int)Model.Data.Enum.ProxyTypes.HTTP,
                "127.0.0.1",
                port))
            {
                return empty;
            }

            return config.ToString(Formatting.None);
        }

        /// <summary>
        /// return Tuple(protocol, ip, port)
        /// </summary>
        /// <returns></returns>
        Tuple<string, string, int> GetParsedInboundInfo()
        {
            var protocol = GetInProtocolNameByNumber(overwriteInboundType);
            var ip = inboundIP;
            var port = inboundPort;

            if (protocol != "config")
            {
                return new Tuple<string, string, int>(protocol, ip, port);
            }

            var parsedConfig = GetDecodedConfig(true, false, true);
            if (parsedConfig == null)
            {
                return null;
            }

            string prefix = "inbound";
            foreach (var p in new string[] { "inbound", "inbounds.0" })
            {
                prefix = p;
                protocol = Lib.Utils.GetValue<string>(parsedConfig, prefix, "protocol");
                if (!string.IsNullOrEmpty(protocol))
                {
                    break;
                }
            }

            ip = Lib.Utils.GetValue<string>(parsedConfig, prefix, "listen");
            port = Lib.Utils.GetValue<int>(parsedConfig, prefix, "port");
            return new Tuple<string, string, int>(protocol, ip, port);
        }

        static string GetInProtocolNameByNumber(int typeNumber)
        {
            var table = Model.Data.Table.inboundOverwriteTypesName;
            return table[Lib.Utils.Clamp(typeNumber, 0, table.Length)];
        }

        bool IsProtocolMatchProxyRequirment(bool isGlobalProxy, string protocol)
        {
            if (isGlobalProxy && protocol != "http")
            {
                return false;
            }

            if (protocol != "socks" && protocol != "http")
            {
                return false;
            }

            return true;
        }

        void RestartCoreWorker(Action next)
        {
            JObject cfg = GetDecodedConfig(true, false, true);
            if (cfg == null)
            {
                StopCoreThen(next);
                return;
            }

            if (!OverwriteInboundSettings(
                ref cfg,
                overwriteInboundType,
                this.inboundIP,
                this.inboundPort))
            {
                StopCoreThen(next);
                return;
            }

            InjectSkipCNSite(ref cfg);

            server.title = GetTitle();
            server.RestartCoreThen(
                cfg.ToString(),
                () =>
                {
                    OnRequireNotifierUpdate?.Invoke(this, EventArgs.Empty);
                    OnRequireKeepTrack?.Invoke(this, new Model.Data.BoolEvent(true));
                    next?.Invoke();
                },
                Lib.Utils.GetEnvVarsFromConfig(cfg));
        }

        void InjectSkipCNSite(ref JObject config)
        {
            if (!this.isInjectSkipCNSite)
            {
                return;
            }

            // copy from Controller.ConfigerComponent.Quick.cs
            var c = JObject.Parse(@"{}");

            var dict = new Dictionary<string, string> {
                { "dns","dnsCFnGoogle" },
                { "routing","routeCNIP" },
                { "outboundDetour","outDtrFreedom" },
            };

            foreach (var item in dict)
            {
                var tpl = Lib.Utils.CreateJObject(item.Key);
                var value = cache.tpl.LoadExample(item.Value);
                tpl[item.Key] = value;

                if (!Lib.Utils.Contains(config, tpl))
                {
                    c[item.Key] = value;
                }
            }

            Lib.Utils.CombineConfig(ref config, c);
        }

        void SetPropertyOnDemand<T>(
            ref T property,
            T value,
            bool isNeedCoreStopped)
        {
            if (EqualityComparer<T>.Default.Equals(property, value))
            {
                return;
            }

            if (isNeedCoreStopped)
            {
                if (NeedStopCoreFirst())
                {
                    InvokeEventOnPropertyChange(); // refresh ui
                    return;
                }
            }

            property = value;
            InvokeEventOnPropertyChange();
        }

        JObject GetDecodedConfig(bool isUseCache, bool isIncludeSpeedTest, bool isIncludeActivate)
        {
            JObject cfg = null;
            try
            {
                cfg = servers.ParseImport(
                    isInjectImport ?
                    InjectGlobalImport(config, isIncludeSpeedTest, isIncludeActivate) :
                    config);

                cache.core[config] = cfg.ToString(Formatting.None);

            }
            catch { }

            if (cfg == null)
            {
                SendLog(I18N.DecodeImportFail);
                if (isUseCache)
                {
                    try
                    {
                        cfg = JObject.Parse(cache.core[config]);
                    }
                    catch (KeyNotFoundException) { }
                    SendLog(I18N.UsingDecodeCache);
                }
            }

            return cfg;
        }

        bool NeedStopCoreFirst()
        {
            if (!isServerOn)
            {
                return false;
            }

            if (overwriteInboundType != (int)Model.Data.Enum.ProxyTypes.HTTP
                && overwriteInboundType != (int)Model.Data.Enum.ProxyTypes.SOCKS)
            {
                return false;

            }

            Task.Factory.StartNew(() => MessageBox.Show(I18N.StopServerFirst));
            return true;
        }

        bool OverwriteInboundSettings(
            ref JObject config,
            int inboundType,
            string ip,
            int port)
        {
            switch (inboundType)
            {
                case (int)Model.Data.Enum.ProxyTypes.HTTP:
                case (int)Model.Data.Enum.ProxyTypes.SOCKS:
                    break;
                default:
                    return true;
            }

            var protocol = GetInProtocolNameByNumber(inboundType);
            var part = protocol + "In";
            try
            {
                var o = JObject.Parse(@"{}");
                o["protocol"] = protocol;
                o["listen"] = ip;
                o["port"] = port;
                o["settings"] = cache.tpl.LoadTemplate(part);

                if (inboundType == (int)Model.Data.Enum.ProxyTypes.SOCKS)
                {
                    o["settings"]["ip"] = ip;
                }

                // Bug. Stream setting will mess things up.
                // Lib.Utils.MergeJson(ref config, o);
                var hasInbound = Lib.Utils.GetKey(config, "inbound") != null;
                var hasInbounds = Lib.Utils.GetKey(config, "inbounds.0") != null;

                if (hasInbounds && !hasInbound)
                {
                    config["inbounds"][0] = o;
                }
                else
                {
                    config["inbound"] = o;
                }

                var debug = config.ToString(Formatting.Indented);

                return true;
            }
            catch
            {
                SendLog(I18N.CoreCantSetLocalAddr);
            }
            return false;
        }

        void SendLog(string message)
        {
            OnLogHandler(this, new Model.Data.StrEvent(message));
        }

        void OnLogHandler(object sender, Model.Data.StrEvent arg)
        {
            var msg = string.Format("[{0}] {1}", this.name, arg.Data);

            LogCache = msg;
            try
            {
                OnLog?.Invoke(this, new Model.Data.StrEvent(msg));
            }
            catch { }
        }

        void InvokeEventOnRequireStatusBarUpdate()
        {
            OnRequireStatusBarUpdate?.Invoke(this, EventArgs.Empty);
        }

        void InvokeEventOnRequireMenuUpdate()
        {
            OnRequireMenuUpdate?.Invoke(this, EventArgs.Empty);
        }

        void UpdateSummary(JObject config)
        {
            this.name = Lib.Utils.GetAliasFromConfig(config);
            this.summary = Lib.Utils.GetSummaryFromConfig(config);
        }

        void OnCoreStateChangedHandler(object sender, EventArgs args)
        {
            isServerOn = server.isRunning;
            InvokeEventOnPropertyChange();
        }

        #endregion
    }
}
