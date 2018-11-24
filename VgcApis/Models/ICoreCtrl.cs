using System.Collections.Concurrent;

namespace VgcApis.Models
{
    public interface ICoreCtrl
    {
        ConcurrentQueue<VgcApis.Models.StatsSample> GetSampleData();
        string GetStatus();
        string GetTitle();
        string GetUid();
        string GetConfig();
        string GetName();

        bool IsCoreRunning();
        bool IsSelected();
        bool IsUntrack();

        bool IsSuitableToBeUsedAsSysProxy(
            bool isGlobal,
            out bool isSocks,
            out int port);
    }
}
