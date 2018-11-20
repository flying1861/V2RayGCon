using Pacman.Resources.Langs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Pacman.Services
{
    public class Settings
    {
        VgcApis.Models.ISettingService vgcSetting;
        VgcApis.Models.IServersService vgcServers;
        readonly string pluginName = Properties.Resources.Name;
        Models.Data.UserSettings userSettings;
        VgcApis.Models.IUtils vgcUtils;

        public Settings() { }

        public void Run(VgcApis.IApi vgcApi)
        {
            vgcUtils = vgcApi.GetVgcUtils();
            vgcServers = vgcApi.GetVgcServersService();
            vgcSetting = vgcApi.GetVgcSettingService();
            userSettings = LoadUserSettings();
        }

        #region properties

        #endregion

        #region public methods
        public void ImportPackage(
            List<VgcApis.Models.ICoreCtrl> servList)
        {
            vgcServers.PackServersIntoV4Package(servList);
        }

        public ReadOnlyCollection<VgcApis.Models.ICoreCtrl>
            GetAllServersList()
                => vgcServers.GetAllServersList();

        public List<Models.Data.Package> GetPackageList()
        {
            return userSettings.packages;
        }

        public Models.Data.Package GetPackageByIndex(int index)
        {
            var max = userSettings.packages.Count;
            if (max <= 0)
            {
                return new Models.Data.Package();
            }

            index = vgcUtils.Clamp(index, 0, max);
            return userSettings.packages[index];
        }

        public void RemovePackageByName(string name)
        {
            var num = userSettings.packages.RemoveAll(p => p.name == name);
            SaveUserSettings();
            if (num <= 0)
            {
                Libs.UI.MsgBox(I18N.Fail);
            }
        }

        public void SavePackage(Models.Data.Package package)
        {
            if (package == null)
            {
                Libs.UI.MsgBoxAsync(I18N.NullParam);
                return;
            }

            var p = userSettings.packages.FirstOrDefault(s => s.name == package.name);
            if (p == null)
            {
                userSettings.packages.Add(package);
            }
            else
            {
                p.beans = package.beans;
            }

            SaveUserSettings();
            Libs.UI.MsgBoxAsync(I18N.Done);
        }

        public void SaveUserSettings()
        {
            try
            {
                var content = vgcUtils.SerializeObject(userSettings);
                vgcSetting.SavePluginsSetting(pluginName, content);
            }
            catch { }
        }

        public void Cleanup()
        {

        }
        #endregion

        #region private methods
        Models.Data.UserSettings LoadUserSettings()
        {
            var empty = new Models.Data.UserSettings();
            var userSettingString = vgcSetting.GetPluginsSetting(pluginName);
            if (string.IsNullOrEmpty(userSettingString))
            {
                return empty;
            }

            try
            {
                var result = vgcUtils
                    .DeserializeObject<Models.Data.UserSettings>(
                        userSettingString);
                return result ?? empty;
            }
            catch { }

            return empty;
        }
        #endregion
    }
}
