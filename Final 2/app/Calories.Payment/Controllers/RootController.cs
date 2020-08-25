using Calories.Common;
using Microsoft.AspNetCore.Mvc;

namespace Calories.Payment.Controllers
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
        
        [HttpGet("versionPayment", Name = nameof(VersionPayment))]
        public string VersionPayment()
        {
            return "99.5080.1";
        }
        
        [HttpGet("configPayment", Name = nameof(ConfigPayment))]
        [ResponseCache(Duration = 86400)]
        public JsonResult ConfigPayment()
        {
            return new JsonResult(_settings);
        }

        [HttpGet("healthPayment", Name = nameof(HealthPayment))]
        public string HealthPayment()
        {
            return "{\"status\": \"OK\"}";
        }
    }
}