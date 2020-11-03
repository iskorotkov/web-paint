using System;

namespace Plugins.Config
{
    public class PluginsConfig
    {
        public PluginsMode Mode { get; set; } = PluginsMode.Automatic;
        public string[] Plugins { get; set; } = new string[0];
    }
}
