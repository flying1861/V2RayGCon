using System;
using System.Collections.ObjectModel;

namespace VgcApis.Models
{
    public interface IServersService
    {
        event EventHandler<BoolEvent> OnServerStateChange;
        ReadOnlyCollection<Models.ICoreCtrl> GetTrackableServerList();
    }
}
