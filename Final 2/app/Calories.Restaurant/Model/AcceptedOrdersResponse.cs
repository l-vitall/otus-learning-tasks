using Calories.Common.Models;

namespace Calories.Restaurant.Model
{
    public class AcceptedOrdersResponse : PagedCollection<RestaurantOrderEntity>
    {
        public Form AcceptedOrdersQuery { get; set; }
    }
}