using System;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using VgcApis;

// https://docs.microsoft.com/en-us/dotnet/framework/misc/how-to-run-partially-trusted-code-in-a-sandbox

namespace V2RayGCon.Plugin
{
    //The Sandboxer class needs to derive from MarshalByRefObject so that we can create it in another   
    // AppDomain and refer to it from the default AppDomain.  
    class Sandboxer : MarshalByRefObject, IPlugin
    {
        IPlugin auxPlugin = null;

        public Sandboxer() { }

        #region IPlugin interface method
        public string Name => auxPlugin.Name;
        public string Version => auxPlugin.Version;
        public string Description => auxPlugin.Description;
        public void Run(IApi api) => auxPlugin.Run(api);
        public void Show() => auxPlugin.Show();
        public void Cleanup() => auxPlugin.Cleanup();

        #endregion

        #region public method


        #endregion


        #region public static method

        /// <summary>
        /// Return null if fail to load dll
        /// </summary>
        /// <param name="pluginFileName">for individual namespaces</param>
        /// <returns></returns>
        public static IPlugin LoadPartiallyTrustedPlugin(
            string pluginFileName)
        {
            // https://docs.microsoft.com/en-us/dotnet/framework/misc/how-to-run-partially-trusted-code-in-a-sandbox

            //Setting the AppDomainSetup. It is very important to set the ApplicationBase to a folder   
            //other than the one in which the sandboxer resides.  
            AppDomainSetup adSetup = new AppDomainSetup();
            adSetup.ApplicationBase = Path.GetFullPath(Properties.Resources.PluginsFolderName);

            //Setting the permissions for the AppDomain. We give the permission to execute and to   
            //read/discover the location where the untrusted code is loaded.  
            PermissionSet permSet = new PermissionSet(PermissionState.None);
            permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

            //We want the sandboxer assembly's strong name, so that we can add it to the full trust list.  
            StrongName fullTrustAssembly = typeof(Plugin.Sandboxer).Assembly.Evidence.GetHostEvidence<StrongName>();

            //Now we have everything we need to create the AppDomain, so let's create it.  
            AppDomain newDomain = AppDomain.CreateDomain("Sandbox" + pluginFileName, null, adSetup, permSet, fullTrustAssembly);

            //Use CreateInstanceFrom to load an instance of the Sandboxer class into the  
            //new AppDomain.   
            ObjectHandle handle = Activator.CreateInstanceFrom(
                newDomain,
                typeof(Sandboxer).Assembly.ManifestModule.FullyQualifiedName,
                typeof(Sandboxer).FullName);

            //Unwrap the new domain instance into a reference in this domain and use it to execute the   
            //untrusted code.  
            var box = handle.Unwrap() as Sandboxer;

            // only host can read file
            var pluginType = LoadPluginTypeFromAssembly(
                    LoadAssemblyFromFile(pluginFileName));

            box.auxPlugin = box.CreatePartiallyTrustedPluginInstance(pluginType);
            return box.auxPlugin == null ? null : box;
        }

        /// <summary>
        /// return null if fail to load
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static IPlugin LoadTrustedPlugin(string fileName)
        {
            // running inside host
            return CreatePluginInstance(
                LoadPluginTypeFromAssembly(
                    LoadAssemblyFromFile(fileName)));
        }
        #endregion

        #region private method
        IPlugin CreatePartiallyTrustedPluginInstance(Type pluginType)
        {
            // running inside sandboxer
            return CreatePluginInstance(pluginType);
        }
        #endregion

        #region static private method
        /// <summary>
        /// return null if fail or param is null
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        static IPlugin CreatePluginInstance(Type pluginType)
        {
            if (pluginType == null)
            {
                return null;
            }

            try
            {
                return (IPlugin)
                    Activator.CreateInstance(pluginType);
            }
            catch
            {
                Lib.Sys.Log.Error("Load plugin fail.");
            }

            return null;
        }

        /// <summary>
        /// return null if fail or param is null
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        static Type LoadPluginTypeFromAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                return null;
            }

            /* debug */
            try
            {
                var types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException es)
            {
                foreach (var e in es.LoaderExceptions)
                {
                    var info = e;
                }
            }
            /* */

            var pluginType = typeof(IPlugin);
            foreach (Type type in assembly.GetTypes())
            {
                if (!type.IsInterface
                    && !type.IsAbstract
                    && type.GetInterface(pluginType.Name) != null)
                {
                    return type;
                }
            }
            return null;
        }

        /// <summary>
        /// return null if fail, return null if param is null
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        static Assembly LoadAssemblyFromFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }
            try
            {
                // https://stackoverflow.com/questions/15238714/net-local-assembly-load-failed-with-cas-policy
                return Assembly.UnsafeLoadFrom(fileName);
            }
            catch { }
            return null;
        }


        #endregion
    }
}
