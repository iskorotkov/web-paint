using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Plugins.Base;

namespace Plugins.Loader
{
    public class PluginLoader
    {
        public IEnumerable<IPlugin> Load()
        {
            var plugins = new List<IPlugin>();

            var folder = AppDomain.CurrentDomain.BaseDirectory;
            var files = Directory.GetFiles(folder, "*.dll");

            var tempDomain = AppDomain.CreateDomain("Temp domain");

            foreach (var file in files)
            {
                try
                {
                    var assembly = tempDomain.Load(file);
                    var types = assembly.GetExportedTypes()
                        .Where(t => t.IsAssignableTo(typeof(IPlugin)));

                    foreach (var type in types)
                    {
                        var plugin = Activator.CreateInstance(type);
                        if (plugin is IPlugin p)
                        {
                            plugins.Add(p);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            AppDomain.Unload(tempDomain);

            return plugins;
        }
    }
}
