using Calories.Common;
using Calories.Common.Infrastructure;
using Calories.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Calories.Auth.Controllers
{
    [Route("/")]
    [ApiController]
    [ApiVersion("1.0")]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = nameof(GetRoot))]
        [ProducesResponseType(200)]
        [ResponseCache(CacheProfileName = "Static")]
        public IActionResult GetRoot()
        {
            var response = new RootResponse
            {
                Self = Link.To(nameof(GetRoot)),
                Token = FormMetadata.FromModel(
                    new PasswordGrantForm(),
                    Link.ToForm(nameof(TokenController.TokenExchange),
                        null, relations: Form.Relation))
            };

            return Ok(response);
        }

        [HttpGet("versionAuth", Name = nameof(VersionAuth))]
        public string VersionAuth()
        {
            return "9.0.1";
        }
        
        [HttpGet("configAuth", Name = nameof(ConfigAuth))]
        [ResponseCache(CacheProfileName = "Static")]
        public JsonResult ConfigAuth()
        {
            return new JsonResult(new AuthEnvironmentSettings());
        }

        [HttpGet("healthAuth", Name = nameof(HealthAuth))]
        public string HealthAuth()
        {
            return "{\"status\": \"OK\"}";
        }
    }
}