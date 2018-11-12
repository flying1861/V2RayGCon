namespace V2RayGCon.Plugin.Apis
{
    class ApiUtils : VgcApis.Models.IUtils
    {
        // static method is evil!

        public string ShowSelectFileDialog(string extension) =>
            Lib.UI.ShowSelectFileDialog(extension);

        public void VisitUrl(string msg, string url) =>
            Lib.UI.VisitUrl(msg, url);

        public bool CopyToClipboard(string content) =>
            Lib.Utils.CopyToClipboard(content);

        public bool Confirm(string content) =>
            Lib.UI.Confirm(content);

        public string RandomHex(int len) =>
            Lib.Utils.RandomHex(len);

        public string GetAppDir() =>
            Lib.Utils.GetAppDir();

        public int Clamp(int value, int min, int max) =>
            Lib.Utils.Clamp(value, min, max);

        public int Str2Int(string content) =>
            Lib.Utils.Str2Int(content);

        public T Clone<T>(T source)
            where T : class =>
            Lib.Utils.Clone<T>(source);

        public T DeserializeObject<T>(string content)
            where T : class =>
            Lib.Utils.DeserializeObject<T>(content);

        public string SerializeObject(object serializeObject) =>
            Lib.Utils.SerializeObject(serializeObject);
    }
}
