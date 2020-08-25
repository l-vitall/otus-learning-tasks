using Calories.Common;
using Microsoft.AspNetCore.Mvc;

namespace Calories.Recommendation.Controllers
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
        
        [HttpGet("versionRecommendation", Name = nameof(VersionRecommendation))]
        public string VersionRecommendation()
        {
            return "0.0.1";
        }
        
        [HttpGet("configRecommendation", Name = nameof(ConfigRecommendation))]
        [ResponseCache(Duration = 86400)]
        public JsonResult ConfigRecommendation()
        {
            return new JsonResult(_settings);
        }

        [HttpGet("healthRecommendation", Name = nameof(HealthRecommendation))]
        public string HealthRecommendation()
        {
            return "{\"status\": \"OK\"}";
        }
    }
}