namespace VgcPlugin.Models
{
    public interface ISettingService
    {
        void SendLog(string log);
        void SavePluginsSetting(string pluginName, string value);
        string GetPluginsSetting(string pluginName);
    }
}
