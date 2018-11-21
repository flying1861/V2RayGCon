namespace ProxySetter.Model.Data
{
    public class QueryParams
    {
        public string mime { get; set; } = null;
        public string debug { get; set; } = null;
        public string ip { get; set; } = null;
        public string port { get; set; } = null;
        public string type { get; set; } = null;
        public string proto { get; set; } = null;

        public QueryParams() { }

        public QueryParams(Model.Data.BasicSettings basicSetting)
        {
            port = basicSetting.proxyPort.ToString();
            type =
                basicSetting.sysProxyMode ==
                (int)Model.Data.Enum.PacListModes.WhiteList ?
                "true" : "false";
            proto = basicSetting.pacProtocol ==
                (int)Enum.PacProtocols.SOCKS ?
                "socks" : "http";
        }

        public void ReplaceNullValueWith(QueryParams defaultValues)
        {
            var v = defaultValues;

            mime = mime ?? v.mime;
            debug = debug ?? v.debug;
            ip = ip ?? v.ip;
            port = port ?? v.port;
            type = type ?? v.type;
            proto = proto ?? v.proto;
        }

        public PacUrlParams ToPacUrlParams()
        {
            return new PacUrlParams
            {
                ip = (Lib.Utils.IsIP(ip) ? ip : "127.0.0.1"),
                port = VgcApis.Libs.Utils.Clamp(
                    VgcApis.Libs.Utils.Str2Int(port), 0, 65536),
                isSocks = proto?.ToLower() == "socks",
                isWhiteList = type?.ToLower() != "blacklist",
                isDebug = debug?.ToLower() == "true",
                mime = mime ?? "",
            };
        }
    }
}
