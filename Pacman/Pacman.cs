using Pacman.Resources.Langs;

namespace Pacman
{
    public class Pacman : VgcApis.IPlugin
    {
        VgcApis.IApi api;
        Services.Settings settings;
        VgcApis.Models.IServersService vgcServers;
        VgcApis.Models.ISettingService vgcSettings;

        Views.WinForms.FormMain formMain = null;

        // form=null;

        #region properties
        public string Name => Properties.Resources.Name;
        public string Version => Properties.Resources.Version;
        public string Description => I18N.Description;
        #endregion

        #region public methods
        public void Run(VgcApis.IApi api)
        {
            this.api = api;
            this.settings = new Services.Settings();
            vgcServers = api.GetVgcServersService();
            vgcSettings = api.GetVgcSettingService();
            settings.Run(api);
        }

        public void Show()
        {
            if (formMain != null)
            {
                formMain.Activate();
                return;
            }

            formMain = new Views.WinForms.FormMain(settings);
            formMain.FormClosed += (s, a) => formMain = null;
            formMain.Show();
        }

        public void Cleanup()
        {
            if (formMain != null)
            {
                formMain.Close();
            }
            settings?.Cleanup();
        }
        #endregion
    }
}
