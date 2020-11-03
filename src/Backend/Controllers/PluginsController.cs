using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Plugins.Base;
using Plugins.Loader;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PluginsController : ControllerBase
    {
        private readonly ILogger<PluginsController> _logger;
        private readonly PluginLoader _pluginLoader;

        public PluginsController(ILogger<PluginsController> logger, PluginLoader pluginLoader)
        {
            _logger = logger;
            _pluginLoader = pluginLoader;
        }

        [HttpGet]
        public IEnumerable<PluginDto> Get()
        {
            _logger.LogInformation("Plugins requested.");

            var plugins = _pluginLoader.Load();
            _logger.LogInformation("Plugins were successfully loaded.");

            var result = new List<PluginDto>();
            foreach (var (name, plugin) in plugins)
            {
                var versionAttributes = plugin
                    .GetType()
                    .GetCustomAttributes<VersionAttribute>()
                    .ToArray();

                var version = versionAttributes.Length switch
                {
                    0 => "unknown",
                    1 => $"v{versionAttributes[0].Major}.{versionAttributes[0].Minor}",
                    _ => "undetermined"
                };

                result.Add(new PluginDto(name, version, plugin.Author));
            }

            return result;
        }
    }
}
