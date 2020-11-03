using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Plugins.Loader;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly PluginLoader _pluginLoader;
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(IConfiguration configuration, PluginLoader pluginLoader,
            ILogger<ImagesController> logger)
        {
            _configuration = configuration;
            _pluginLoader = pluginLoader;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var path = _configuration["Images:Default"];
            var image = System.IO.File.Open(path, FileMode.Open);
            return File(image, "image/jpeg");
        }

        [HttpPost]
        public IActionResult Post(PluginDto[] filters)
        {
            var path = _configuration["Images:Default"];
            using var file = System.IO.File.Open(path, FileMode.Open);
            var bitmap = new Bitmap(file);

            var plugins = _pluginLoader.Load();
            foreach (var filter in filters)
            {
                if (!plugins.ContainsKey(filter.Name))
                {
                    _logger.LogError("Tried to use plugin that wasn't loaded");
                    return BadRequest("Tried to use plugin that wasn't loaded");
                }

                plugins[filter.Name].Transform(bitmap);
            }

            using var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Jpeg);
            return File(stream.ToArray(), "image/jpeg");
        }
    }
}
