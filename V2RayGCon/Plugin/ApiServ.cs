using VgcApis.Models;

namespace V2RayGCon.Plugin
{
    class ApiServ : VgcApis.IApi
    {
        IServersService serversService;
        ISettingService settingService;
        IUtils apiUtils;

        public void Run(
            Service.Setting setting,
            Service.Servers servers)
        {
            this.apiUtils = new Apis.ApiUtils();
            this.settingService = setting;
            this.serversService = servers;
        }

        #region IApi interfaces
        public IServersService GetVgcServersService()
            => this.serversService;

        public ISettingService GetVgcSettingService()
            => this.settingService;

        public IUtils GetVgcUtils()
            => this.apiUtils;
        #endregion
    }
}
