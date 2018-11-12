namespace VgcApis
{
    public interface IApi
    {
        Models.IUtils GetVgcUtils();
        Models.ISettingService GetVgcSettingService();
        Models.IServersService GetVgcServersService();
    }
}
