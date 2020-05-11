using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using MaeveFramework.Plugins;
using MaeveFramework.Scheduler;

namespace MaeveFramework.Plugins
{
    public class Manager : IDisposable
    {
        public readonly string PluginsDirPath;
        public IList<PluginBase> LoadedPlugins { get; private set; }

        private ILogger _logger;

        public Manager(string pluginsDirPath)
        {
            _logger = LogManager.CreateLogger("MaeveFramework.PluginManager");

            PluginsDirPath = pluginsDirPath;
            if (!Directory.Exists(PluginsDirPath))
            {
                _logger.Warn($"Plugins directory {PluginsDirPath} not existing - creating!");
                Directory.CreateDirectory(PluginsDirPath);
            }

            LoadedPlugins = new List<PluginBase>();

        }

        public FileVersionInfo GetPluginInfo(string path)
        {
            return FileVersionInfo.GetVersionInfo(path);
        }

        public List<string> GetPluginsList()
        {
            List<string> plugins = new List<string>();

            try
            {
                foreach (var pluginPath in Directory.GetDirectories(PluginsDirPath, "*-Plugin", SearchOption.TopDirectoryOnly))
                {
                    plugins.AddRange(Directory.GetFiles(pluginPath, "*-MaevePlugin.dll", SearchOption.TopDirectoryOnly));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error on getting plugins from plugins dir");
            }

            return plugins;
        }

        public void LoadPlugins()
        {
            foreach (string pluginPath in GetPluginsList())
            {
                try
                {
                    Assembly assembly = LoadPlugin(pluginPath);

                    foreach (Type definedType in assembly.DefinedTypes)
                    {
                        if (typeof(PluginBase).IsAssignableFrom(definedType))
                        {
                            if (Activator.CreateInstance(definedType) is PluginBase result)
                            {
                                _logger.Debug($"Plugin loaded: {result.Name} {result.Version}");
                                LoadedPlugins.Add(result);
                            }
                        }
                        else if (typeof(JobBase).IsAssignableFrom(definedType))
                        {
                            if (Activator.CreateInstance(definedType) is JobBase result)
                            {
                                _logger.Debug($"Loading job: {result.Name}");
                                Scheduler.Manager.CreateJob(result);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Failed to complete plugin load from {pluginPath}");
                }
            }
        }

        private Assembly LoadPlugin(string pluginPath)
        {
            AssemblyName pluginName = AssemblyLoadContext.GetAssemblyName(pluginPath);
            _logger.Debug($"Loading plugin: {pluginName.FullName}");

            return AssemblyLoadContext.Default.LoadFromAssemblyPath(pluginPath);
        }

        public void Dispose()
        {
            _logger.Debug("Disposing");

            foreach (PluginBase plugin in LoadedPlugins)
            {
                try
                {
                    plugin.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Exception on disposing plugin {plugin.GetType().FullName}");
                }
            }

            LoadedPlugins?.Clear();
            LoadedPlugins = null;
        }
    }
}
