namespace V2RayGCon.Model.Plugin.Apis
{
    class ApiSettingService : VgcPlugin.Models.ISettingService
    {
        Service.Setting setting;
        public ApiSettingService() { }

        public void Run(Service.Setting setting)
        {
            this.setting = setting;
        }

        #region ISettingService
        public void SendLog(string log)
            => setting.SendLog(log);

        public void SavePluginsSetting(string pluginName, string value)
            => setting.SavePluginsSetting(pluginName, value);

        public string GetPluginsSetting(string pluginName)
            => setting.GetPluginsSetting(pluginName);
        #endregion
    }
}
