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
    public class MenuItemsController : ControllerBase
    {
        private readonly IAuthorizationService _authService;

        private readonly IMenuItemService _menuItemsService;

        public MenuItemsController(
            IAuthorizationService authService
            ,IMenuItemService menuItemsService)
        {
            _authService = authService;
            _menuItemsService = menuItemsService;
        }

        //GET /caloriesRecords
        [HttpGet(Name = nameof(GetAllMenuItems))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Collection<RestaurantMenuItemEntity>>> GetAllMenuItems()
        {
            var records = await _menuItemsService.GetAllMenuitems();
            
            var collection = PagedCollection<RestaurantMenuItemEntity>.Create<MenuItemsResponse>(
                Link.ToCollection(nameof(GetAllMenuItems)),
                records,
                records.Length,
                new PagingOptions());

            collection.MenuItemsQuery = FormMetadata.FromResource<RestaurantMenuItemEntity>(
                Link.ToForm(nameof(GetAllMenuItems),
                    null,
                    Link.GetMethod,
                    Form.QueryRelation));
            
            return collection;
        }
    }
}