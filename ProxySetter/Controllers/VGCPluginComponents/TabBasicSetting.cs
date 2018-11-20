using System;
using System.Windows.Forms;

namespace ProxySetter.Controllers.VGCPluginComponents
{
    class TabBasicSetting : ComponentCtrl
    {
        Services.PsSettings setting;
        string oldSetting;
        Model.Data.BasicSettings basicSettings;
        Services.ServerTracker servTracker;
        VgcApis.Models.IUtils vgcUtils;

        ComboBox cboxBasicSysProxyMode, cboxBasicPacMode, cboxBasicPacProtocol;
        TextBox tboxBasicProxyPort, tboxBaiscPacPort, tboxBasicCustomPacPath;
        CheckBox chkBasicAutoUpdateSysProxy, chkBasicPacAlwaysOn, chkBasicUseCustomPac;

        public TabBasicSetting(
            VgcApis.Models.IUtils vgcUtils,
            Services.PsSettings setting,
            Services.ServerTracker servTracker,

            ComboBox cboxBasicPacProtocol,
            ComboBox cboxBasicSysProxyMode,
            TextBox tboxBasicProxyPort,
            TextBox tboxBaiscPacPort,
            ComboBox cboxBasicPacMode,
            TextBox tboxBasicCustomPacPath,
            CheckBox chkBasicAutoUpdateSysProxy,
            CheckBox chkBasicPacAlwaysOn,
            CheckBox chkBasicUseCustomPac,
            Button btnBasicBrowseCustomPac)
        {
            this.vgcUtils = vgcUtils;
            this.setting = setting;
            this.servTracker = servTracker;

            basicSettings = setting.GetBasicSetting();
            oldSetting = vgcUtils.SerializeObject(basicSettings);

            BindControls(
                cboxBasicPacProtocol,
                cboxBasicSysProxyMode,
                tboxBasicProxyPort,
                tboxBaiscPacPort,
                cboxBasicPacMode,
                tboxBasicCustomPacPath,
                chkBasicAutoUpdateSysProxy,
                chkBasicPacAlwaysOn,
                chkBasicUseCustomPac);

            InitControls();

            BindEvents(btnBasicBrowseCustomPac);

            servTracker.OnSysProxyChanged += OnSysProxyChangeHandler;
        }

        #region public method
        void OnSysProxyChangeHandler(object sender, EventArgs args)
        {
            basicSettings = setting.GetBasicSetting();
            oldSetting = vgcUtils.SerializeObject(basicSettings);

            chkBasicAutoUpdateSysProxy?.Invoke((MethodInvoker)delegate
            {
                InitControls();
            });
        }

        public override bool IsOptionsChanged()
        {
            return vgcUtils.SerializeObject(GetterSettings()) != oldSetting;
        }

        public override bool SaveOptions()
        {
            if (!IsOptionsChanged())
            {
                return false;
            }

            var bs = GetterSettings();
            oldSetting = vgcUtils.SerializeObject(bs);
            setting.SaveBasicSetting(bs);
            return true;
        }

        public override void Cleanup()
        {
            servTracker.OnSysProxyChanged -= OnSysProxyChangeHandler;
        }
        #endregion

        #region private methods

        private void BindEvents(Button btnBasicBrowseCustomPac)
        {
            btnBasicBrowseCustomPac.Click += (s, a) =>
            {
                var filename = vgcUtils.ShowSelectFileDialog("JS Files|*.js|All File|*.*");
                if (!string.IsNullOrEmpty(filename))
                {
                    this.tboxBasicCustomPacPath.Text = filename;
                }
            };
        }

        Model.Data.BasicSettings GetterSettings()
        {
            return new Model.Data.BasicSettings
            {
                pacProtocol = Lib.Utils.Clamp(cboxBasicPacProtocol.SelectedIndex, 0, 2),
                sysProxyMode = Lib.Utils.Clamp(cboxBasicSysProxyMode.SelectedIndex, 0, 3),
                proxyPort = Lib.Utils.Str2Int(tboxBasicProxyPort.Text),
                pacServPort = Lib.Utils.Str2Int(tboxBaiscPacPort.Text),
                pacMode = Lib.Utils.Clamp(cboxBasicPacMode.SelectedIndex, 0, 2),
                customPacFileName = tboxBasicCustomPacPath.Text,
                isAutoUpdateSysProxy = chkBasicAutoUpdateSysProxy.Checked,
                isAlwaysStartPacServ = chkBasicPacAlwaysOn.Checked,
                isUseCustomPac = chkBasicUseCustomPac.Checked,
            };
        }

        private void BindControls(
            ComboBox cboxBasicPacProtocol,
            ComboBox cboxBasicSysProxyMode,
            TextBox tboxBasicProxyPort,
            TextBox tboxBaiscPacPort,
            ComboBox cboxBasicPacMode,
            TextBox tboxBasicCustomPacPath,
            CheckBox chkBasicAutoUpdateSysProxy,
            CheckBox chkBasicPacAlwaysOn,
            CheckBox chkBasicUseCustomPac)
        {
            this.cboxBasicPacProtocol = cboxBasicPacProtocol;
            this.cboxBasicSysProxyMode = cboxBasicSysProxyMode;
            this.tboxBasicProxyPort = tboxBasicProxyPort;
            this.tboxBaiscPacPort = tboxBaiscPacPort;
            this.cboxBasicPacMode = cboxBasicPacMode;
            this.tboxBasicCustomPacPath = tboxBasicCustomPacPath;
            this.chkBasicAutoUpdateSysProxy = chkBasicAutoUpdateSysProxy;
            this.chkBasicPacAlwaysOn = chkBasicPacAlwaysOn;
            this.chkBasicUseCustomPac = chkBasicUseCustomPac;
        }

        private void InitControls()
        {
            var s = basicSettings;

            cboxBasicPacProtocol.SelectedIndex = Lib.Utils.Clamp(s.pacProtocol, 0, 2);
            cboxBasicSysProxyMode.SelectedIndex = Lib.Utils.Clamp(s.sysProxyMode, 0, 3);
            tboxBasicProxyPort.Text = s.proxyPort.ToString();
            tboxBaiscPacPort.Text = s.pacServPort.ToString();
            cboxBasicPacMode.SelectedIndex = Lib.Utils.Clamp(s.pacMode, 0, 2);
            tboxBasicCustomPacPath.Text = s.customPacFileName;
            chkBasicAutoUpdateSysProxy.Checked = s.isAutoUpdateSysProxy;
            chkBasicPacAlwaysOn.Checked = s.isAlwaysStartPacServ;
            chkBasicUseCustomPac.Checked = s.isUseCustomPac;
        }
        #endregion
    }
}
