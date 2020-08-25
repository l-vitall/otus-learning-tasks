using System;
using System.Linq;
using System.Threading.Tasks;
using Calories.Common;
using Calories.Common.Kafka;
using Calories.Common.Notifications;
using Calories.Common.Services;
using Calories.Restaurant.Data;
using Calories.Restaurant.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Calories.Restaurant.Service
{
    public class RestaurantOrderService : IRestaurantOrderService
    {
        private readonly RestaurantsDbContext _dbContext;
        private readonly IKafkaProducerService _service;

        public RestaurantOrderService(RestaurantsDbContext dbContext, IKafkaProducerService service)
        {
            _dbContext = dbContext;
            _service = service;
        }
        
        public async Task<RestaurantOrderEntity[]> GetAllOrders()
        {
            return await _dbContext.Orders.ToArrayAsync();
        }
        
        public async Task CreateAcceptedOrder(RestaurantOrderEntity newOrder)
        {
            try
            {
                await _dbContext.Orders.AddAsync(newOrder);
           
                var created = await _dbContext.SaveChangesAsync();

                if (created < 1)
                    throw new InvalidOperationException("Could not create a notification");

                await _service.SendMessage(MessageTopic.Order, MessageKind.OrderStatusUpdate, JsonConvert.SerializeObject(new OrderStatusUpdateNotification
                {
                    OrderId = newOrder.OrderId,
                    NewStatus = OrderStatus.Reserved
                }));
            }
            catch (Exception e)
            {
                await _service.SendMessage(MessageTopic.Order, MessageKind.OrderStatusUpdate, JsonConvert.SerializeObject(new OrderStatusUpdateNotification
                {
                    OrderId = newOrder.OrderId,
                    NewStatus = OrderStatus.Rejected
                }));
            }
           
        }

        public async Task RejectAcceptedOrderStatus(Guid orderId)
        {
            var order = await _dbContext.Orders.SingleOrDefaultAsync(o => o.OrderId == orderId);
            if (order != null)
            {
                order.Status = RestaurantOrderStatus.Rejected;
                await _dbContext.SaveChangesAsync();
                
                await _service.SendMessage(MessageTopic.Order, MessageKind.OrderStatusUpdate, JsonConvert.SerializeObject(new OrderStatusUpdateNotification
                {
                    OrderId = orderId,
                    NewStatus = OrderStatus.Rejected
                }));
            }
        }
        
        public async Task ConfirmAcceptedOrder(Guid orderId)
        {
            var order = await _dbContext.Orders.SingleOrDefaultAsync(o => o.OrderId == orderId);
            if (order != null)
            {
                order.Status = RestaurantOrderStatus.Active;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}