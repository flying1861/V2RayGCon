using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using V2RayGCon.Resource.Resx;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormDownloadCore : Form
    {
        #region Sigleton
        static FormDownloadCore _instant;
        public static FormDownloadCore GetForm()
        {
            if (_instant == null || _instant.IsDisposed)
            {
                _instant = new FormDownloadCore();
            }
            return _instant;
        }
        #endregion

        Service.Downloader downloader;
        Service.Setting setting;
        Service.Servers servers;

        FormDownloadCore()
        {
            setting = Service.Setting.Instance;
            servers = Service.Servers.Instance;

            InitializeComponent();
            InitUI();

            this.FormClosed += (s, e) =>
            {
                downloader?.Cleanup();
                setting.LazyGC();
            };

            VgcApis.Libs.UI.AutoSetFormIcon(this);

            this.Show();
        }

        private void FormDownloadCore_Shown(object sender, System.EventArgs e)
        {
            RefreshCurrentCoreVersion();
        }

        #region private method
        int GetAvailableProxyPort()
        {
            var servList = servers.GetServerList()
                .Where(s => s.isServerOn);

            foreach (var serv in servList)
            {
                if (serv.IsSuitableToBeUsedAsSysProxy(
                    true, out bool isSocks, out int port))
                {
                    return port;
                }
            }
            return -1;
        }


        void RefreshCurrentCoreVersion()
        {
            var el = labelCoreVersion;

            Task.Factory.StartNew(() =>
            {
                var core = new Service.Core(setting);
                var version = core.GetCoreVersion();
                var msg = string.IsNullOrEmpty(version) ?
                    I18N.GetCoreVerFail :
                    string.Format(I18N.CurrentCoreVerIs, version);
                try
                {
                    el.Invoke((MethodInvoker)delegate { el.Text = msg; });
                }
                catch { }
            });
        }

        void UpdateProgressBar(int percentage)
        {
            // window may closed before this function is called
            try
            {
                pgBarDownload.Invoke((MethodInvoker)delegate
                {
                    pgBarDownload.Value = Lib.Utils.Clamp(percentage, 0, 101);
                });
            }
            catch { }
        }

        void EnableBtnDownload()
        {
            try
            {
                btnDownload.Invoke((MethodInvoker)delegate
                {
                    btnDownload.Enabled = true;
                });
            }
            catch { }
        }

        void DownloadV2RayCore(int proxyPort)
        {
            downloader = new Service.Downloader(setting);
            downloader.SetArchitecture(cboxArch.SelectedIndex == 1);
            downloader.SetVersion(cboxVer.Text);
            downloader.proxyPort = proxyPort;

            downloader.OnProgress += (s, a) =>
            {
                UpdateProgressBar(a.Data);
            };

            downloader.OnDownloadCompleted += (s, a) =>
            {
                ResetUI(100);
                Task.Factory.StartNew(
                    () => MessageBox.Show(I18N.DownloadCompleted));
            };

            downloader.OnDownloadCancelled += (s, a) =>
            {
                ResetUI(0);
                Task.Factory.StartNew(
                    () => MessageBox.Show(I18N.DownloadCancelled));
            };

            downloader.OnDownloadFail += (s, a) =>
            {
                ResetUI(0);
                Task.Factory.StartNew(
                    () => MessageBox.Show(I18N.TryManualDownload));
            };

            downloader.DownloadV2RayCore();
            UpdateProgressBar(1);
        }

        #endregion

        #region UI
        void ResetUI(int progress)
        {
            UpdateProgressBar(progress);
            downloader = null;
            EnableBtnDownload();
        }

        void InitUI()
        {
            cboxArch.SelectedIndex = 0;
            var verList = Lib.Utils.Str2ListStr(StrConst.VerList);
            Lib.UI.FillComboBox(cboxVer, verList);
            pgBarDownload.Value = 0;
        }

        private void BtnExit_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void BtnRefreshVer_Click(object sender, System.EventArgs e)
        {
            var elRefresh = btnRefreshVer;
            var elCboxVer = cboxVer;

            elRefresh.Enabled = false;

            Task.Factory.StartNew(() =>
            {
                int proxyPort = -1;
                if (chkUseProxy.Checked)
                {
                    proxyPort = GetAvailableProxyPort();
                }
                var versions = Lib.Utils.GetCoreVersions(proxyPort);
                try
                {
                    elRefresh.Invoke((MethodInvoker)delegate
                    {
                        elRefresh.Enabled = true;
                    });

                    elCboxVer.Invoke((MethodInvoker)delegate
                    {
                        if (versions.Count > 0)
                        {
                            Lib.UI.FillComboBox(elCboxVer, versions);
                        }
                        else
                        {
                            MessageBox.Show(I18N.GetVersionListFail);
                        }
                    });
                }
                catch { }
            });
        }

        private void BtnUpdate_Click(object sender, System.EventArgs e)
        {
            if (downloader != null)
            {
                MessageBox.Show(I18N.Downloading);
                return;
            }

            int proxyPort = -1;
            if (chkUseProxy.Checked)
            {
                proxyPort = GetAvailableProxyPort();
                if (proxyPort <= 0)
                {
                    Task.Factory.StartNew(
                        () => MessageBox.Show(
                            I18N.NoQualifyProxyServer));
                }
            }

            btnDownload.Enabled = false;
            DownloadV2RayCore(proxyPort);
        }

        void BtnCancel_Click(object sender, System.EventArgs e)
        {
            if (downloader != null && Lib.UI.Confirm(I18N.CancelDownload))
            {
                downloader?.Cancel();
            }
        }

        private void BtnCheckVersion_Click(object sender, System.EventArgs e)
        {
            RefreshCurrentCoreVersion();
        }

        #endregion
    }
}
