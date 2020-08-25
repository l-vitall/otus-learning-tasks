using System.Threading.Tasks;
using Calories.Common;
using Calories.Common.Infrastructure;
using Calories.Common.Models;
using Calories.Recommendation.Model;
using Calories.Recommendation.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Calories.Recommendation.Controllers
{
    [Route("/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class CaloriesRecommendationsController : ControllerBase
    {
        //private readonly IAuthorizationService _authService;

        private readonly IRecommendationService _service;

        public CaloriesRecommendationsController(
            //IAuthorizationService authService,
            IRecommendationService service)
        {
            //_authService = authService;
            _service = service;
        }

        //GET /caloriesRecords
        [HttpGet(Name = nameof(GetAllRecommendations))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Collection<CaloriesRecommendationEntity>>> GetAllRecommendations([FromBody] EmailForm emailForm)
        {
            var records = await _service.GetAllRecommendations(emailForm.Email);
            
            var collection = PagedCollection<CaloriesRecommendationEntity>.Create<RecommendationsResponse>(
                Link.ToCollection(nameof(GetAllRecommendations)),
                records,
                records.Length,
                new PagingOptions());

            collection.CaloriesRecommendationQuery = FormMetadata.FromResource<CaloriesRecommendationEntity>(
                Link.ToForm(nameof(GetAllRecommendations),
                    null,
                    Link.GetMethod,
                    Form.QueryRelation));
            
            return collection;
        }
        
        //CaloriesRecommendation
        [HttpPost(Name = nameof(CreateCaloriesRecommendations))]
        [ProducesResponseType(201)]
        public async Task<ActionResult> CreateCaloriesRecommendations([FromBody] CaloriesRecommendationsForm form)
        {
            var id = await _service.CreateRecommendation(form, User.Email());

            return Ok();
            /*return Created(
                Url.Link(nameof(GetCaloriesRecordById), new { id }),
                null);*/
        }
    }
}