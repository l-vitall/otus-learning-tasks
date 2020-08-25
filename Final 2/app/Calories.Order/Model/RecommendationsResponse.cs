using Calories.Common.Models;

namespace Calories.Order.Model
{
    public class OrdersResponse : PagedCollection<OrderEntity>
    {
        public Form OrdersQuery { get; set; }
    }
}