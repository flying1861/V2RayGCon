namespace VgcPlugin.Models
{
    public interface IUtils
    {
        string ShowSelectFileDialog(string extension);

        void VisitUrl(string msg, string url);

        bool CopyToClipboard(string content);

        bool Confirm(string content);

        string RandomHex(int len);

        string GetAppDir();

        int Clamp(int value, int min, int max);

        int Str2Int(string content);

        T Clone<T>(T source) where T : class;

        T DeserializeObject<T>(string content) where T : class;

        string SerializeObject(object serializeObject);
    }
}
