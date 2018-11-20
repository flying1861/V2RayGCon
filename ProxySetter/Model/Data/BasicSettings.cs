namespace ProxySetter.Model.Data
{
    public class BasicSettings
    {
        public int sysProxyMode { get; set; }
        public bool isAutoUpdateSysProxy { get; set; }
        public int proxyPort { get; set; }
        public int pacServPort { get; set; }
        public bool isAlwaysStartPacServ { get; set; }
        public int pacMode { get; set; }
        public string customPacFileName { get; set; }
        public bool isUseCustomPac { get; set; }
        public int pacProtocol { get; set; }

        public BasicSettings()
        {
            sysProxyMode = (int)Enum.SystemProxyModes.None;
            pacProtocol = (int)Enum.PacProtocols.HTTP;
            isAutoUpdateSysProxy = false;
            proxyPort = 1080;
            pacServPort = 3000;
            isAlwaysStartPacServ = false;
            pacMode = (int)Enum.OnOff.OFF;
            customPacFileName = string.Empty;
            isUseCustomPac = false;
        }
    }
}
