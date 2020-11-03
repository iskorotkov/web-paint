using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Plugins.Base;
using Plugins.Config;

namespace Plugins.Loader
{
    public class PluginLoader
    {
        private readonly ILogger<PluginLoader> _logger;

        public PluginLoader(ILogger<PluginLoader> logger)
        {
            _logger = logger;
        }

        public Dictionary<string, IPlugin> Load(PluginsConfig config)
        {
            var plugins = new Dictionary<string, IPlugin>();

            var folder = AppDomain.CurrentDomain.BaseDirectory;

            var files = config.Mode switch
            {
                PluginsMode.Automatic => Directory.GetFiles(folder, "*.dll"),
                PluginsMode.Manual => Directory.GetFiles(folder, "*.dll")
                    .Where(file =>
                    {
                        var name = Path.GetFileNameWithoutExtension(file);
                        return config.Plugins.Contains(name);
                    })
                    .ToArray(),
                _ => throw new NotImplementedException("Config mode isn't supported.")
            };

            _logger.LogInformation(
                $"Loaded {files.Length} plugins out of {config.Plugins.Length} specified in config file");

            foreach (var file in files)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file);
                    var types = assembly.GetExportedTypes()
                        .Where(t => !t.IsInterface && t.IsAssignableTo(typeof(IPlugin)));

                    foreach (var type in types)
                    {
                        var plugin = Activator.CreateInstance(type);
                        if (plugin is IPlugin p)
                        {
                            plugins.Add(p.Name, p);
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Couldn't load a plugin %s", file);
                }
            }

            return plugins;
        }
    }
}
