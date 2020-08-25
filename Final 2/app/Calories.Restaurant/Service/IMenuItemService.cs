using System.Collections.Generic;
using System.Threading.Tasks;
using Calories.Restaurant.Model;

namespace Calories.Restaurant.Service
{
    public interface IMenuItemService
    {
        Task<RestaurantMenuItemEntity[]> GetAllMenuitems();
        Task CreateMenuItem(RestaurantMenuItemEntity menuItem);
    }
}