using System;
using System.IO;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Plugins.Config
{
    public class ConfigLoader
    {
        private readonly ILogger<ConfigLoader> _logger;

        public ConfigLoader(ILogger<ConfigLoader> logger)
        {
            _logger = logger;
        }

        private readonly IDeserializer _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        public PluginsConfig Load(string path)
        {
            try
            {
                string yaml;
                using (var file = File.Open(path, FileMode.Open))
                {
                    using var reader = new StreamReader(file);
                    yaml = reader.ReadToEnd();
                }

                var config = _deserializer.Deserialize<PluginsConfig>(yaml);
                return config;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Couldn't load config with path %s", path);
                throw;
            }
        }
    }
}
