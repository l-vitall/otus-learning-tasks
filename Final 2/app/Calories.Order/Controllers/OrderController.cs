using System.Threading.Tasks;
using Calories.Common;
using Calories.Common.Infrastructure;
using Calories.Common.Models;
using Calories.Order.Model;
using Calories.Order.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calories.Order.Controllers
{
    [Route("/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class OrderController : ControllerBase
    {
        //private readonly IAuthorizationService _authService;

        private readonly IOrderService _service;

        public OrderController(
            //IAuthorizationService authService,
            IOrderService service)
        {
            //_authService = authService;
            _service = service;
        }

        //GET /caloriesRecords
        [HttpGet(Name = nameof(GetAllOrders))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Collection<OrderEntity>>> GetAllOrders([FromBody] EmailForm emailForm)
        {
            var records = await _service.GetAllOrders(emailForm.Email);
            
            var collection = PagedCollection<OrderEntity>.Create<OrdersResponse>(
                Link.ToCollection(nameof(GetAllOrders)),
                records,
                records.Length,
                new PagingOptions());

            collection.OrdersQuery = FormMetadata.FromResource<OrderEntity>(
                Link.ToForm(nameof(GetAllOrders),
                    null,
                    Link.GetMethod,
                    Form.QueryRelation));
            
            return collection;
        }
        
        //CaloriesRecommendation
        [HttpPost(Name = nameof(CreateOrder))]
        [ProducesResponseType(201)]
        public async Task<ActionResult> CreateOrder([FromBody] OrderForm form)
        {
            var id = await _service.CreateOrder(form, User.Email());

            return Ok();
            /*return Created(
                Url.Link(nameof(GetCaloriesRecordById), new { id }),
                null);*/
        }
    }
}