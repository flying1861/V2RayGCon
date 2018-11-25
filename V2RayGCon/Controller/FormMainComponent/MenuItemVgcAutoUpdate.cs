using AutoUpdaterDotNET;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Windows.Forms;
using V2RayGCon.Resource.Resx;

namespace V2RayGCon.Controller.FormMainComponent
{
    class MenuItemVgcAutoUpdate : FormMainComponentController
    {
        Service.Setting setting;
        Service.Servers servers;

        public MenuItemVgcAutoUpdate(
            ToolStripMenuItem miCheckVgcUpdate)
        {
            setting = Service.Setting.Instance;
            servers = Service.Servers.Instance;

            InitAutoUpdater();
            miCheckVgcUpdate.Click += CheckForUpdate;
        }

        #region public method
        #endregion

        #region component thing
        public override bool RefreshUI() => false;
        public override void Cleanup()
        {
            AutoUpdater.ParseUpdateInfoEvent -=
                AutoUpdaterOnParseUpdateInfoEvent;
            AutoUpdater.ApplicationExitEvent -=
                AutoUpdater_ApplicationExitEvent;
        }
        #endregion

        #region private method
        void AutoUpdater_ApplicationExitEvent()
        {
            setting.isShutdown = true;
            Application.Exit();
        }

        void InitAutoUpdater()
        {
            AutoUpdater.ReportErrors = true;
            AutoUpdater.ParseUpdateInfoEvent +=
                AutoUpdaterOnParseUpdateInfoEvent;
            AutoUpdater.ApplicationExitEvent +=
                AutoUpdater_ApplicationExitEvent;
        }

        void AutoSetUpdaterProxy()
        {
            if (!setting.isUpdateUseProxy)
            {
                return;
            }

            var port = servers.GetAvailableHttpProxyPort();
            if (port <= 0)
            {
                MessageBox.Show(I18N.NoQualifyProxyServer);
                return;
            }

            var proxy = new WebProxy($"127.0.0.1:{port}", true);
            AutoUpdater.Proxy = proxy;
        }

        void CheckForUpdate(object sender, EventArgs args)
        {
            AutoSetUpdaterProxy();
            AutoUpdater.Start(Properties.Resources.LatestVersionInfoUrl);
        }

        void AutoUpdaterOnParseUpdateInfoEvent(ParseUpdateInfoEventArgs args)
        {
            var updateInfo = JsonConvert
                .DeserializeObject<Model.Data.UpdateInfo>(
                    args.RemoteData);

            // algorithm = "MD5" > Update file Checksum<
            var latestVersion = new Version(updateInfo.version);

            var url = setting.isUpdateToVgcFull ?
                    updateInfo.urlVgcFull :
                    updateInfo.urlVgcLite;

            var md5 = setting.isUpdateToVgcFull ?
                    updateInfo.md5VgcFull :
                    updateInfo.md5VgcLite;

            args.UpdateInfo = new UpdateInfoEventArgs
            {
                CurrentVersion = latestVersion,
                ChangelogURL = Properties.Resources.ChangeLogUrl,
                Mandatory = false,
                DownloadURL = url,
                HashingAlgorithm = "MD5",
                Checksum = md5,
            };
        }
        #endregion
    }
}
