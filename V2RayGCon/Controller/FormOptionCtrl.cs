using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using V2RayGCon.Resource.Resx;

namespace V2RayGCon.Controller
{
    class FormOptionCtrl : Model.BaseClass.FormController
    {
        Service.Setting setting;

        public FormOptionCtrl()
        {
            this.setting = Service.Setting.Instance;
        }

        public bool IsOptionsSaved()
        {
            foreach (var component in GetAllComponents())
            {
                var optCtrl = component.Value as OptionComponent.OptionComponentController;
                if (optCtrl.IsOptionsChanged())
                {
                    return false;
                }
            }
            return true;
        }

        public bool SaveAllOptions()
        {
            var changed = false;
            foreach (var kvPair in GetAllComponents())
            {
                var component = kvPair.Value as OptionComponent.OptionComponentController;
                if (component.SaveOptions())
                {
                    changed = true;
                }
            }
            return changed;
        }

        public void BackupOptions()
        {
            if (!IsOptionsSaved())
            {
                MessageBox.Show(I18N.SaveChangeFirst);
                return;
            }

            var serverString = string.Empty;
            foreach (var server in Service.Servers.Instance.GetServerList())
            {
                // insert a space in the front for regex matching
                serverString += " v2ray://"
                    + Lib.Utils.Base64Encode(server.config)
                    + Environment.NewLine;
            }

            var data = new Dictionary<string, string> {
                    { "import", JsonConvert.SerializeObject(setting.GetGlobalImportItems())},
                    { "subscription",JsonConvert.SerializeObject(setting.GetSubscriptionItems()) },
                    { "servers" ,serverString},
                };

            switch (Lib.UI.ShowSaveFileDialog(
                StrConst.ExtText,
                JsonConvert.SerializeObject(data),
                out string filename))
            {
                case Model.Data.Enum.SaveFileErrorCode.Success:
                    MessageBox.Show(I18N.Done);
                    break;
                case Model.Data.Enum.SaveFileErrorCode.Fail:
                    MessageBox.Show(I18N.WriteFileFail);
                    break;
                case Model.Data.Enum.SaveFileErrorCode.Cancel:
                    // do nothing
                    break;
            }
        }

        public void RestoreOptions()
        {
            string backup = Lib.UI.ShowReadFileDialog(StrConst.ExtText, out string filename);

            if (backup == null)
            {
                return;
            }

            if (!Lib.UI.Confirm(I18N.ConfirmAllOptionWillBeReplaced))
            {
                return;
            }

            Dictionary<string, string> options;
            try
            {
                options = JsonConvert.DeserializeObject<Dictionary<string, string>>(backup);
            }
            catch
            {
                MessageBox.Show(I18N.DecodeFail);
                return;
            }

            if (options.ContainsKey("import"))
            {
                GetComponent<Controller.OptionComponent.Import>()
                    .Reload(options["import"]);
            }

            if (options.ContainsKey("subscription"))
            {
                GetComponent<Controller.OptionComponent.Subscription>()
                    .Reload(options["subscription"]);
            }

            if (options.ContainsKey("servers")
                && Lib.UI.Confirm(I18N.ConfirmImportServers))
            {
                Service.Servers.Instance.ImportLinks(options["servers"]);
            }
            else
            {
                MessageBox.Show(I18N.Done);
            }

        }

    }
}
