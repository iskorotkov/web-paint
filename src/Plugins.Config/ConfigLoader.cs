using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Plugins.Config
{
    public class ConfigLoader
    {
        private readonly ILogger<ConfigLoader> _logger;
        private readonly IConfiguration _configuration;

        public ConfigLoader(ILogger<ConfigLoader> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        private readonly IDeserializer _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        public PluginsConfig Load()
        {
            var configPath = _configuration["Plugins:Config"];
            _logger.LogTrace("Config path is %s", configPath);

            var config = string.IsNullOrEmpty(configPath)
                ? new PluginsConfig()
                : ReadFromFile(configPath);
            return config;
        }

        private PluginsConfig ReadFromFile(string path)
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
                config.Plugins ??= new string[0];
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
