using VgcPlugin.Models;

namespace V2RayGCon.Model.Plugin
{
    class ApiServ : VgcPlugin.IApi
    {
        public ApiServ()
        {
        }

        public Apis.ApiSettingService settingService;
        public Apis.ApiServersService serversService;
        public Apis.ApiUtils apiUtils;

        public void Run(
            Service.Setting setting,
            Service.Servers servers)
        {
            this.apiUtils = new Apis.ApiUtils();

            this.settingService = new Apis.ApiSettingService();
            this.settingService.Run(setting);

            this.serversService = new Apis.ApiServersService();
            this.serversService.Run(servers);
        }

        public void Cleanup()
        {
            this.serversService.Cleanup();
        }

        #region IApi interface
        public IServersService GetVgcServersService()
            => this.serversService;

        public ISettingService GetVgcSettingService()
            => this.settingService;

        public IUtils GetVgcUtils()
            => this.apiUtils;

        #endregion
    }
}
