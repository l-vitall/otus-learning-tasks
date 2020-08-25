using System;
using System.Threading.Tasks;
using Calories.Common;
using Calories.Order.Model;

namespace Calories.Order.Services
{
    public interface IOrderService
    {
        Task<OrderEntity[]> GetAllOrders(string email);
        Task<Guid> CreateOrder(OrderForm form, string creatorEmail);
        Task UpdateOrderStatus(Guid orderId, OrderStatus status);
    }
}