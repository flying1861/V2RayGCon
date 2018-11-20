using ProxySetter.Resources.Langs;
using System;
using System.Windows.Forms;

namespace ProxySetter.Controllers.VGCPluginComponents
{
    class TabStatus : ComponentCtrl
    {
        Label lbPacUrl, lbPacServerStatus;
        readonly Services.PsSettings setting;
        Services.PacServer pacServer;
        VgcApis.Models.IUtils vgcUtils;

        public TabStatus(
            VgcApis.Models.IUtils vgcUtils,
            Services.PsSettings setting,
            Services.PacServer pacServer,

            Label lbPacServerStatus,
            Label lbPacUrl,
            Button btnRestart,
            Button btnStop,
            Button btnDebug,
            Button btnCopy,
            Button btnClearSysProxy)
        {
            this.vgcUtils = vgcUtils;
            this.setting = setting;
            this.pacServer = pacServer;

            BindControls(lbPacServerStatus, lbPacUrl);
            BindEvents(
                pacServer,
                btnRestart,
                btnStop,
                btnDebug,
                btnCopy,
                btnClearSysProxy);

            OnPacServerStateChangedHandler(null, EventArgs.Empty);

            pacServer.OnPACServerStateChanged += OnPacServerStateChangedHandler;
        }

        private void BindEvents(
            Services.PacServer pacServer,
            Button btnRestart,
            Button btnStop,
            Button btnDebug,
            Button btnCopy,
            Button btnClearSysProxy)
        {
            btnClearSysProxy.Click += (s, a) =>
                Lib.Sys.ProxySetter.ClearSysProxy();

            btnRestart.Click += (s, a) => pacServer.StartPacServer();

            btnStop.Click += (s, a) => pacServer.StopPacServer();

            btnDebug.Click +=
                (s, a) => vgcUtils.VisitUrl(
                    I18N.VisitPacDebugger, GetDebugUrl());

            btnCopy.Click += (s, a) =>
            {
                MessageBox.Show(
                    vgcUtils.CopyToClipboard(this.lbPacUrl.Text) ?
                    I18N.CopySuccess : I18N.CopyFail);
            };
        }

        #region private methods
        string GetDebugUrl()
        {
            return string.Format("{0}?&debug=true", pacServer.GetPacUrl());
        }

        void OnPacServerStateChangedHandler(object sender, EventArgs args)
        {
            lbPacServerStatus?.Invoke((MethodInvoker)delegate
            {
                lbPacServerStatus.Text =
                    pacServer.isRunning ?
                    I18N.PacServerIsOn : I18N.PacServerIsOff;
            });
        }

        private void BindControls(Label lbPacServerStatus, Label lbPacUrl)
        {
            this.lbPacServerStatus = lbPacServerStatus;
            this.lbPacUrl = lbPacUrl;
        }
        #endregion

        #region public method
        public override bool IsOptionsChanged()
        {
            return false;
        }

        public override bool SaveOptions()
        {
            return false;
        }

        public override void Cleanup()
        {
            pacServer.OnPACServerStateChanged -= OnPacServerStateChangedHandler;
        }
        #endregion

    }
}
