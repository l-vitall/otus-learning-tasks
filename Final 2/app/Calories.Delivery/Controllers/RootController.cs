using Calories.Common;
using Microsoft.AspNetCore.Mvc;

namespace Calories.Delivery.Controllers
{
    [Route("/")]
    [ApiController]
    public class RootController : ControllerBase
    {
        private readonly DeliveryEnvironmentSettings _settings;

        public RootController(DeliveryEnvironmentSettings settings)
        {
            _settings = settings;
        }
        
        [HttpGet("versionDelivery", Name = nameof(VersionDelivery))]
        public string VersionDelivery()
        {
            return "99.5090.1";
        }
        
        [HttpGet("configDelivery", Name = nameof(ConfigDelivery))]
        [ResponseCache(Duration = 86400)]
        public JsonResult ConfigDelivery()
        {
            return new JsonResult(_settings);
        }

        [HttpGet("healthDelivery", Name = nameof(HealthDelivery))]
        public string HealthDelivery()
        {
            return "{\"status\": \"OK\"}";
        }
    }
}