using System.Threading.Tasks;
using Calories.Common.Infrastructure;
using Calories.Common.Models;
using Calories.Restaurant.Model;
using Calories.Restaurant.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calories.Restaurant.Controllers
{
    [Route("/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class RestaurantOrdersController : ControllerBase
    {
        private readonly IAuthorizationService _authService;

        private readonly IRestaurantOrderService _ordersService;
        private readonly IMenuItemService _menuItemsService;

        public RestaurantOrdersController(
            IAuthorizationService authService,
            IRestaurantOrderService ordersService
            ,IMenuItemService menuItemsService)
        {
            _authService = authService;
            _ordersService = ordersService;
            _menuItemsService = menuItemsService;
        }

        //GET /caloriesRecords
        [HttpGet(Name = nameof(GetAllOrders))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Collection<RestaurantOrderEntity>>> GetAllOrders()
        {
            var canReadAll = await _authService.AuthorizeAsync(User, AuthorizationPolicy.GetAllRestaurantOrders );

            RestaurantOrderEntity[] records = null;
            if(!canReadAll.Succeeded)
                return Unauthorized();
            
            records = await _ordersService.GetAllOrders();
            
            var collection = PagedCollection<RestaurantOrderEntity>.Create<AcceptedOrdersResponse>(
                Link.ToCollection(nameof(GetAllOrders)),
                records,
                records.Length,
                new PagingOptions());

            collection.AcceptedOrdersQuery = FormMetadata.FromResource<RestaurantOrderEntity>(
                Link.ToForm(nameof(GetAllOrders),
                    null,
                    Link.GetMethod,
                    Form.QueryRelation));
            
            return collection;
        }
    }
}