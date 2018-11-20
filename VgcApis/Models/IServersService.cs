using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VgcApis.Models
{
    public interface IServersService
    {
        event EventHandler<BoolEvent> OnServerStateChange;
        void PackServersIntoV4Package(
            List<VgcApis.Models.ICoreCtrl> servList);

        ReadOnlyCollection<Models.ICoreCtrl> GetTrackableServerList();
        ReadOnlyCollection<Models.ICoreCtrl> GetAllServersList();
    }
}
