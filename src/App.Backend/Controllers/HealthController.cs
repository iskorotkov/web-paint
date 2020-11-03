using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace App.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            _logger.LogInformation("Health checked.");
            return "The service is healthy";
        }
    }
}
