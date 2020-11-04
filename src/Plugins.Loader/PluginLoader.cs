using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Plugins.Base;
using Plugins.Config;
using YamlDotNet.Core;

namespace Plugins.Loader
{
    public class PluginLoader
    {
        private readonly ILogger<PluginLoader> _logger;
        private readonly ConfigLoader _configLoader;

        public PluginLoader(ILogger<PluginLoader> logger, ConfigLoader configLoader)
        {
            _logger = logger;
            _configLoader = configLoader;
        }

        public Dictionary<string, IPlugin> Load()
        {
            var plugins = new Dictionary<string, IPlugin>();

            PluginsConfig config;
            try
            {
                config = _configLoader.Load();
            }
            catch (IOException)
            {
                _logger.LogError("Couldn't open specified config file. Using default config instead");
                config = new PluginsConfig();
            }
            catch (YamlException)
            {
                _logger.LogError("Couldn't parse specified config file. Using default config instead");
                config = new PluginsConfig();
            }
            
            var folder = AppDomain.CurrentDomain.BaseDirectory;
            var files = GetPluginFiles(folder, config);

            _logger.LogInformation(
                $"Loaded {files.Length} DLLs out of {config.Plugins.Length} specified in config file");

            foreach (var file in files)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file);
                    var types = assembly.GetExportedTypes()
                        .Where(t => !t.IsInterface && t.IsAssignableTo(typeof(IPlugin)));

                    AddPlugins(types, plugins);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Couldn't load a plugin %s", file);
                }
            }

            _logger.LogInformation(
                $"Loaded {plugins.Count} plugins out of {config.Plugins.Length} specified in config file");

            return plugins;
        }

        private static void AddPlugins(IEnumerable<Type> types, Dictionary<string, IPlugin> plugins)
        {
            foreach (var type in types)
            {
                var plugin = Activator.CreateInstance(type);
                if (plugin is IPlugin p)
                {
                    plugins.Add(p.Name, p);
                }
            }
        }

        private static string[] GetPluginFiles(string folder, PluginsConfig config)
        {
            return config.Mode switch
            {
                PluginsMode.Automatic => Directory.GetFiles(folder, "*.dll"),
                PluginsMode.Manual => Directory.GetFiles(folder, "*.dll")
                    .Where(file =>
                    {
                        var name = Path.GetFileNameWithoutExtension(file);
                        return config.Plugins.Any(plugin =>
                            string.Equals(plugin, name, StringComparison.InvariantCultureIgnoreCase));
                    })
                    .ToArray(),
                _ => throw new NotImplementedException("Config mode isn't supported.")
            };
        }
    }
}
