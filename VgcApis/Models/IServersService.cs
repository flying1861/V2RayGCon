using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VgcApis.Models
{
    public interface IServersService
    {
        event EventHandler<BoolEvent> OnServerStateChange;
        string PackServersIntoV4Package(
            List<VgcApis.Models.ICoreCtrl> servList,
            string orgServerUid,
            string packageName);

        ReadOnlyCollection<Models.ICoreCtrl> GetTrackableServerList();
        ReadOnlyCollection<Models.ICoreCtrl> GetAllServersList();
    }
}
