using ProxySetter.Resources.Langs;

namespace ProxySetter
{
    public class ProxySetter : VgcApis.IPlugin
    {
        Services.PsLuncher luncher;
        public ProxySetter() { }

        #region private methods


        #endregion

        #region properties
        public string Name => Properties.Resources.Name;
        public string Version => Properties.Resources.Version;
        public string Description => I18N.Description;
        #endregion

        #region public methods
        public void Run(VgcApis.IApi api)
        {
            luncher = new Services.PsLuncher();
            luncher.Run(api);
        }

        public void Show()
        {
            luncher?.Show();
        }

        public void Cleanup()
        {
            luncher?.Cleanup();
        }
        #endregion
    }
}
