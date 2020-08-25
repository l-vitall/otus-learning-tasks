using Calories.Common;
using Calories.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Calories.Records.Controllers
{
    [Route("/")]
    [ApiController]
    [ApiVersion("1.0")]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = nameof(GetRoot))]
        [ProducesResponseType(200)]
        [ResponseCache(Duration = 86400)]
        public IActionResult GetRoot()
        {
            var response = new RootResponse
            {
                Self = Link.To(nameof(GetRoot)),
                CaloriesRecords = Link.ToCollection(nameof(CaloriesRecordsController.GetAllCaloriesRecords))
            };

            return Ok(response);
        }

        [HttpGet("versionRecords", Name = nameof(VersionRecords))]
        public string VersionRecords()
        {
            return "99.5030.1";
        }

        [HttpGet("configRecords", Name = nameof(ConfigRecords))]
        public JsonResult ConfigRecords()
        {
            return new JsonResult(new GrpcEnvironmentSettings());
        }

        [HttpGet("healthRecords", Name = nameof(HealthRecords))]
        public string HealthRecords()
        {
            return "{\"status\": \"OK\"}";
        }
    }
}