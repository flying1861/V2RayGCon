using ProxySetter.Resources.Langs;
using System;
using System.Windows.Forms;

namespace ProxySetter.Views.WinForms
{
    partial class FormMain : Form
    {
        Services.PsSettings setting;
        Services.PacServer pacServer;
        Services.ServerTracker servTracker;
        VgcApis.Models.IUtils vgcUtils;

        Controllers.FormVGCPluginCtrl formVGCPluginCtrl;
        System.Windows.Forms.Timer updateSysProxyInfoTimer = null;

        public FormMain(
            VgcApis.Models.IUtils vgcUtils,
            Services.PsSettings setting,
            Services.PacServer pacServer,
            Services.ServerTracker servTracker)
        {
            this.vgcUtils = vgcUtils;
            this.setting = setting;
            this.pacServer = pacServer;
            this.servTracker = servTracker;

            this.FormClosing += (s, a) =>
            {
                var confirm = true;
                if (!setting.isCleaning && !this.formVGCPluginCtrl.IsOptionsSaved())
                {
                    confirm = vgcUtils.Confirm(I18N.ConfirmCloseWinWithoutSave);
                }

                if (confirm)
                {
                    ReleaseUpdateTimer();
                    formVGCPluginCtrl.Cleanup();
                }
                else
                {
                    a.Cancel = true;
                }
            };

            InitializeComponent();

#if DEBUG
            this.Icon = Properties.Resources.icon_light;
#else
            this.Icon = Properties.Resources.icon_dark;
#endif
            this.Show();
        }

        private void FormPluginMain_Shown(object sender, System.EventArgs e)
        {
            this.Text = string.Format(
                "{0} v{1}",
                Properties.Resources.Name,
                Properties.Resources.Version);

            formVGCPluginCtrl = CreateFormCtrl();

            UpdateSysProxyInfo(null, EventArgs.Empty);
            StartUpdateTimer();
        }

        #region private method
        void UpdateSysProxyInfo(object sender, EventArgs args)
        {
            var proxySetting = Lib.Sys.ProxySetter.GetProxySetting();
            var proxyUrl = proxySetting.autoConfigUrl;
            if (string.IsNullOrEmpty(proxyUrl))
            {
                proxyUrl = proxySetting.proxyEnable ?
                    "http://" + proxySetting.proxyServer :
                    "Direct";
            }

            if (lbBasicProxyLink.Text != proxyUrl)
            {
                lbBasicProxyLink.Text = proxyUrl;
            }
        }

        void ReleaseUpdateTimer()
        {
            if (updateSysProxyInfoTimer != null)
            {
                updateSysProxyInfoTimer.Stop();
                updateSysProxyInfoTimer.Tick -= UpdateSysProxyInfo;
                updateSysProxyInfoTimer.Dispose();
            }
        }

        void StartUpdateTimer()
        {
            updateSysProxyInfoTimer = new Timer
            {
                Interval = 2000,
            };
            updateSysProxyInfoTimer.Tick += UpdateSysProxyInfo;
            updateSysProxyInfoTimer.Start();
        }

        Controllers.FormVGCPluginCtrl CreateFormCtrl()
        {
            var ctrl = new Controllers.FormVGCPluginCtrl();

            ctrl.Plug(new Controllers.VGCPluginComponents.TabStatus(
                vgcUtils,
                setting,
                pacServer,

                lbBasicCurPacServerStatus,
                lbBasicProxyLink,
                btnBasicStartPacServer,
                btnBasicStopPacServer,
                btnBasicDebugPacServer,
                btnBaiscCopyProxyLink,
                btnBasicClearSysProxy));

            ctrl.Plug(new Controllers.VGCPluginComponents.TabBasicSetting(
                vgcUtils,
                setting,
                servTracker,

                cboxBasicPacProtocol,
                cboxBasicSysProxyMode,
                tboxBasicGlobalPort,
                tboxBaiscPacPort,
                cboxBasicPacMode,
                tboxBasicCustomPacPath,
                chkBasicAutoUpdateSysProxy,
                chkBasicPacAlwaysOn,
                chkBasicUseCustomPac,
                btnBasicBrowseCustomPac));

            ctrl.Plug(new Controllers.VGCPluginComponents.TabPacCustomList(
                setting,
                rtboxPacWhiteList,
                rtboxPacBlackList));

            return ctrl;
        }
        #endregion

        #region UI event handler
        private void btnSave_Click(object sender, System.EventArgs e)
        {
            var changed = formVGCPluginCtrl.SaveAllOptions();
            if (changed)
            {
                servTracker.Restart();
            }
            MessageBox.Show(I18N.Done);
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
