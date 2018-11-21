namespace VgcApis
{
    public interface IServices
    {
        Models.ISettingService GetVgcSettingService();
        Models.IServersService GetVgcServersService();
    }
}
