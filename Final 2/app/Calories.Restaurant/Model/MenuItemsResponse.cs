using Calories.Common.Models;

namespace Calories.Restaurant.Model
{
    public class MenuItemsResponse : PagedCollection<RestaurantMenuItemEntity>
    {
        public Form MenuItemsQuery { get; set; }
    }
}