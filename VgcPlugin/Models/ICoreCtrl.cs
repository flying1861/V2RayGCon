namespace VgcPlugin.Models
{
    public interface ICoreCtrl
    {
        string GetConfig();

        bool IsCoreRunning();

        bool IsUntrack();

        bool IsSuitableToBeUsedAsSysProxy(
            bool isGlobal,
            out bool isSocks,
            out int port);

        string GetTitle();
    }
}
