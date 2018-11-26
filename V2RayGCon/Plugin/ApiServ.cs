﻿using VgcApis.Models;

namespace V2RayGCon.Plugin
{
    class ApiServ : VgcApis.IServices
    {
        IServersService serversService;
        ISettingService settingService;

        public void Run(
            Service.Setting setting,
            Service.Servers servers)
        {
            this.settingService = setting;
            this.serversService = servers;
        }

        #region IApi interfaces
        public IServersService GetVgcServersService()
            => this.serversService;

        public ISettingService GetVgcSettingService()
            => this.settingService;
        #endregion
    }
}
