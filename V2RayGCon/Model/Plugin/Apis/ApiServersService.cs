using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace V2RayGCon.Model.Plugin.Apis
{
    class ApiServersService : VgcPlugin.Models.IServersService
    {
        public event EventHandler<VgcPlugin.Models.BoolEvent>
            OnServerStateChange;

        Service.Servers servers;
        public ApiServersService() { }

        public void Run(Service.Servers servers)
        {
            this.servers = servers;
            this.servers.OnServerStateChange += OnServerStateChangeRelay;
        }

        public void Cleanup()
        {
            this.servers.OnServerStateChange -= OnServerStateChangeRelay;
        }

        #region ApiServersService interface

        public ReadOnlyCollection<VgcPlugin.Models.ICoreCtrl> GetTrackableServerList()
            => this.servers.GetServerList()
                .Where(s => s.isServerOn && !s.isUntrack)
                .Select(s => s as VgcPlugin.Models.ICoreCtrl)
                .ToList()
                .AsReadOnly();

        #endregion

        #region private methods
        void OnServerStateChangeRelay(object sender, Model.Data.BoolEvent args)
        {
            try
            {
                OnServerStateChange?.Invoke(
                    sender,
                    new VgcPlugin.Models.BoolEvent(args.Data));
            }
            catch { }
        }
        #endregion
    }
}
