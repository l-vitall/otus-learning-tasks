using Calories.Common.Models;

namespace Calories.Delivery.Model
{
    public class DeliveriesResponse : PagedCollection<DeliveryEntity>
    {
        public Form DeliveriesQuery { get; set; }
    }
}