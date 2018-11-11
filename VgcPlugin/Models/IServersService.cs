using System;
using System.Collections.ObjectModel;

namespace VgcPlugin.Models
{
    public interface IServersService
    {
        event EventHandler<BoolEvent> OnServerStateChange;
        ReadOnlyCollection<Models.ICoreCtrl> GetTrackableServerList();
    }
}
