using System;

namespace DTOs
{
    public class PluginDto
    {
        public PluginDto(string name, string version, string author)
        {
            Name = name;
            Version = version;
            Author = author;
        }

        public string Name { get; set; }
        public string Version { get; set; }
        public string Author { get; set; }
    }
}
