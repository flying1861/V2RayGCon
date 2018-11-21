using Pacman.Resources.Langs;
using System;
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

        public Settings() { }

        public void Run(VgcApis.IServices vgcApi)
        {
            vgcServers = vgcApi.GetVgcServersService();
            vgcSetting = vgcApi.GetVgcSettingService();
            userSettings = LoadUserSettings();
        }

        #region properties

        #endregion

        #region public methods
        public void Pack(
            List<VgcApis.Models.ICoreCtrl> servList,
            string orgServerUid,
            string packageName,
            Action<string> finish)
        {
            vgcServers.PackServersIntoV4Package(
                servList,
                orgServerUid,
                packageName,
                finish);
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

            index = VgcApis.Libs.Utils.Clamp(index, 0, max);
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

        public bool SavePackage(Models.Data.Package package)
        {
            if (package == null)
            {
                return false;
            }

            var p = userSettings.packages.FirstOrDefault(s => s.name == package.name);
            if (p == null)
            {
                userSettings.packages.Add(package);
            }
            else
            {
                if (!string.IsNullOrEmpty(package.uid))
                {
                    p.uid = package.uid;
                }
                p.beans = package.beans;
            }

            SaveUserSettings();
            return true;
        }

        public void SaveUserSettings()
        {
            try
            {
                var content = VgcApis.Libs.Utils.SerializeObject(userSettings);
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
                var result = VgcApis.Libs.Utils
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
