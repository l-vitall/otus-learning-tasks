using Calories.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Calories.Restaurant.Controllers
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
        
        [HttpGet("versionRestaurant", Name = nameof(VersionRestaurant))]
        public string VersionRestaurant()
        {
            return "99.5040.1";
        }
        
        [HttpGet("configRestaurant", Name = nameof(ConfigRestaurant))]
        [ResponseCache(Duration = 86400)]
        public JsonResult ConfigRestaurant()
        {
            return new JsonResult(_settings);
        }

        [HttpGet("healthRestaurant", Name = nameof(HealthRestaurant))]
        public string HealthRestaurant()
        {
            return "{\"status\": \"OK\"}";
        }
    }
}