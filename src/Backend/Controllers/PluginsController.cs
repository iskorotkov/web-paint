using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Plugins.Base;
using Plugins.Config;
using Plugins.Loader;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PluginsController : ControllerBase
    {
        private readonly ILogger<PluginsController> _logger;
        private readonly PluginLoader _pluginLoader;
        private readonly ConfigLoader _configLoader;
        private readonly IConfiguration _configuration;

        public PluginsController(ILogger<PluginsController> logger, PluginLoader pluginLoader,
            ConfigLoader configLoader, IConfiguration configuration)
        {
            _logger = logger;
            _pluginLoader = pluginLoader;
            _configLoader = configLoader;
            _configuration = configuration;
        }

        [HttpGet]
        public Dictionary<string, IPlugin> Get()
        {
            _logger.LogInformation("Plugins requested.");

            var configPath = _configuration["Plugins:Config"];
            _logger.LogTrace("Config path is %s", configPath);

            var config = string.IsNullOrEmpty(configPath)
                ? new PluginsConfig()
                : _configLoader.Load(configPath);

            _logger.LogInformation("Plugins were successfully loaded.");
            return _pluginLoader.Load(config);
        }
    }
}
