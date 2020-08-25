using Calories.Common;
using Microsoft.AspNetCore.Mvc;

namespace Calories.Order.Controllers
{
    [Route("/")]
    [ApiController]
    public class RootController : ControllerBase
    {
        private readonly EnvironmentSettings _settings;

        public RootController(EnvironmentSettings settings)
        {
            _settings = settings;
        }
        
        [HttpGet("versionOrder", Name = nameof(VersionOrder))]
        public string VersionOrder()
        {
            return "99.5070.1";
        }
        
        [HttpGet("configOrder", Name = nameof(ConfigOrder))]
        [ResponseCache(Duration = 86400)]
        public JsonResult ConfigOrder()
        {
            return new JsonResult(_settings);
        }

        [HttpGet("healthOrder", Name = nameof(HealthOrder))]
        public string HealthOrder()
        {
            return "{\"status\": \"OK\"}";
        }
    }
}