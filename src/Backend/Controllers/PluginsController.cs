using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Plugins.Base;
using Plugins.Loader;
using YamlDotNet.Core;

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
        public ActionResult<IEnumerable<PluginDto>> Get()
        {
            _logger.LogInformation("Plugins requested.");
            var result = new List<PluginDto>();

            Dictionary<string, IPlugin> plugins;
            try
            {
                plugins = _pluginLoader.Load();
                _logger.LogInformation("Plugins were successfully loaded.");
            }
            catch (IOException)
            {
                return BadRequest("Couldn't read config file");
            }
            catch (YamlException)
            {
                return BadRequest("Couldn't parse config file");
            }
            catch (Exception)
            {
                return BadRequest("Something bad happened. Try again");
            }

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
