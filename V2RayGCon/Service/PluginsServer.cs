using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using V2RayGCon.Resource.Resx;

namespace V2RayGCon.Service
{
    class PluginsServer : Model.BaseClass.SingletonService<PluginsServer>
    {
        Setting setting;
        Notifier notifier;
        Model.Plugin.ApiServ apis = new Model.Plugin.ApiServ();

        Dictionary<string, VgcPlugin.IPlugin> plugins =
            new Dictionary<string, VgcPlugin.IPlugin>();

        PluginsServer() { }

        public void Run(
            Setting setting,
            Servers servers,
            Notifier notifier)
        {
            this.setting = setting;
            this.notifier = notifier;

            apis.Run(setting, servers);
            Restart();
        }

        #region properties
        #endregion

        #region public methods
        public void Restart()
        {
            ReloadPlugins();
            StartEnabledPlugins();
            UpdateNotifierMenu();
        }

        public void Cleanup()
        {
            ClearPlugins();
            apis.Cleanup();
        }

        public List<Model.Data.PluginInfoItem> GetterPluginDirInfo()
        {
            var pluginList = SearchForPlugins();
            return GetPluginInfoFrom(pluginList);
        }

        #endregion

        #region private methods
        void UpdateNotifierMenu()
        {
            var enabledList = GetCurEnabledPluginFileNames();

            var children = new List<ToolStripMenuItem>();
            foreach (var fileName in enabledList)
            {
                if (plugins.ContainsKey(fileName))
                {
                    var plugin = plugins[fileName];
                    children.Add(
                        new ToolStripMenuItem(
                            fileName,
                            null,
                            (s, a) => plugin.Show()));
                }
            }

            notifier.UpdatePluginMenu(children.Count > 0 ?
                new ToolStripMenuItem(
                    I18N.Plugins,
                    Properties.Resources.Module_16x,
                    children.ToArray()) :
                null);
        }

        void StartEnabledPlugins()
        {
            var enabledList = GetCurEnabledPluginFileNames();
            StartPlugins(enabledList);
        }

        public Dictionary<string, VgcPlugin.IPlugin> SearchForPlugins()
        {
            var pluginList = new Dictionary<string, VgcPlugin.IPlugin>();

            var dllFileNames = SearchDllFiles();
            if (dllFileNames == null)
            {
                return pluginList;
            }

            foreach (var relativeFilePath in dllFileNames)
            {
                if (!Lib.Utils.IsTrustedPlugin(relativeFilePath))
                {
                    continue;
                }

                var plugin = Model.Plugin.Sandboxer.LoadTrustedPlugin(relativeFilePath);
                if (plugin == null)
                {
                    continue;
                }

                var key = Path.GetFileName(relativeFilePath);
                pluginList[key] = plugin;
            }

            return pluginList;
        }

        void ReloadPlugins()
        {
            ClearPlugins();
            this.plugins = SearchForPlugins();
        }

        void StartPlugins(List<string> fileNames)
        {
            foreach (var fileName in fileNames)
            {
                if (plugins.ContainsKey(fileName))
                {
                    plugins[fileName].Run(apis);
                }
            }
        }

        void CleanupPlugins(List<string> fileNames)
        {
            foreach (var fileName in fileNames)
            {
                if (plugins.ContainsKey(fileName))
                {
                    plugins[fileName].Cleanup();
                }
            }
        }

        void ClearPlugins()
        {
            CleanupPlugins(plugins.Keys.ToList());
            plugins = new Dictionary<string, VgcPlugin.IPlugin>();
        }

        List<string> GetCurEnabledPluginFileNames()
        {
            var list = setting.GetPluginInfoItems();
            return list
                .Where(p => p.isUse)
                .Select(p => p.filename)
                .ToList();
        }

        List<Model.Data.PluginInfoItem> GetPluginInfoFrom(
            Dictionary<string, VgcPlugin.IPlugin> pluginList)
        {
            if (pluginList.Count <= 0)
            {
                return new List<Model.Data.PluginInfoItem>();
            }

            var enabledList = GetCurEnabledPluginFileNames();
            var infos = new List<Model.Data.PluginInfoItem>();
            foreach (var item in pluginList)
            {
                var plugin = item.Value;
                var filename = item.Key;
                var pluginInfo = new Model.Data.PluginInfoItem
                {
                    filename = filename,
                    name = plugin.Name,
                    version = plugin.Version,
                    description = plugin.Description,
                    isUse = enabledList.Contains(filename),
                };
                infos.Add(pluginInfo);
            }
            return infos;
        }

        string[] SearchDllFiles()
        {
            var path = Properties.Resources.PluginsFolderName;
            if (Directory.Exists(path))
            {
                return Directory.GetFiles(path, "*.dll");
            }
            return null;
        }

        #endregion
    }
}
