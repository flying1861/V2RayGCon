namespace ProxySetter.Services
{
    class PsLuncher
    {
        PsSettings setting;
        PacServer pacServer;
        ServerTracker serverTracker;
        VgcApis.Models.IUtils vgcUtils;


        VgcApis.IApi vgcApi;
        Model.Data.ProxyRegKeyValue orgSysProxySetting;
        Views.WinForms.FormMain formMain;

        public PsLuncher() { }

        public void Run(VgcApis.IApi api)
        {
            orgSysProxySetting = Lib.Sys.ProxySetter.GetProxySetting();
            this.vgcApi = api;

            this.vgcUtils = api.GetVgcUtils();
            var vgcSetting = api.GetVgcSettingService();
            var vgcServer = api.GetVgcServersService();

            pacServer = new PacServer();
            setting = new PsSettings();
            serverTracker = new ServerTracker();

            // dependency injection
            setting.Run(vgcUtils, vgcSetting);
            pacServer.Run(vgcUtils, setting);
            serverTracker.Run(vgcUtils, setting, pacServer, vgcServer);

            setting.DebugLog("call Luncher.run");
        }

        public void Show()
        {
            if (formMain != null)
            {
                return;
            }

            formMain = new Views.WinForms.FormMain(
                vgcUtils,
                setting,
                pacServer,
                serverTracker);
            formMain.FormClosed += (s, a) => formMain = null;
        }

        public void Cleanup()
        {
            setting.DebugLog("call Luncher.cleanup");
            setting.isCleaning = true;
            formMain?.Close();
            serverTracker.Cleanup();
            pacServer.Cleanup();
            setting.Cleanup();
            Lib.Sys.ProxySetter.UpdateProxySettingOnDemand(orgSysProxySetting);
        }
        #region properties
        #endregion
    }

}
