namespace V2RayGCon.Model.Data
{
    class UserSettings
    {
        #region public properties
        public int ServerPanelPageSize { get; set; }

        public bool isEnableStat { get; set; } = false;

        public bool isUseV4Format { get; set; }
        public bool CfgShowToolPanel { get; set; }
        public bool isPortable { get; set; }

        public string ImportUrls { get; set; }
        public string DecodeCache { get; set; }
        public string SubscribeUrls { get; set; }

        public string PluginInfoItems { get; set; }
        public string PluginsSetting { get; set; }

        public string Culture { get; set; }
        public string ServerList { get; set; }
        public string PacServerSettings { get; set; }
        public string SysProxySetting { get; set; }
        public string ServerTracker { get; set; }
        public string WinFormPosList { get; set; }
        #endregion

        public UserSettings()
        {
            ServerPanelPageSize = 99;

            isUseV4Format = false;
            CfgShowToolPanel = true;
            isPortable = false;

            ImportUrls = string.Empty;
            DecodeCache = string.Empty;
            SubscribeUrls = string.Empty;

            PluginInfoItems = string.Empty;
            PluginsSetting = string.Empty;

            Culture = string.Empty;
            ServerList = string.Empty;
            PacServerSettings = string.Empty;
            SysProxySetting = string.Empty;
            ServerTracker = string.Empty;
            WinFormPosList = string.Empty;
        }
    }
}
