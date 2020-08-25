using System;
using System.Threading.Tasks;
using Calories.Restaurant.Model;

namespace Calories.Restaurant.Service
{
    public interface IRestaurantOrderService
    {
        Task<RestaurantOrderEntity[]> GetAllOrders();
        Task CreateAcceptedOrder(RestaurantOrderEntity newOrder);

        Task RejectAcceptedOrderStatus(Guid orderId);

        Task ConfirmAcceptedOrder(Guid orderId);
    }
}