using System;
using System.Linq;
using System.Threading.Tasks;
using Calories.Common;
using Calories.Common.Kafka;
using Calories.Common.Models;
using Calories.Common.Notifications;
using Calories.Common.Services;
using Calories.Order.Data;
using Calories.Order.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Calories.Order.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrdersDbContext _dbContext;

        public OrderService(OrdersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OrderEntity[]> GetAllOrders(string email)
        {
            return await _dbContext.Orders.Where(r => r.UserEmail == email).ToArrayAsync();
        }

        public async Task UpdateOrderStatus(Guid orderId, OrderStatus status)
        {
            var order = await _dbContext.Orders.SingleOrDefaultAsync(o => o.Id == orderId);
            if (order != null)
            {
                //Note: transactions are not supported for memory store
                //using (var transaction = await _dbContext.Database.BeginTransactionAsync())
                //{
                
                    order.Status = status;
                    var msg = new NotificationMessage()
                    {
                        Email = order.UserEmail,
                        Text = $"Order status changed to '{status}'"
                    }; 
                    await _dbContext.NotificationsOutlog.AddAsync(new NotificationMessageEntity()
                    {
                        Topic = MessageTopic.Notification,
                        Kind = MessageKind.NotificationCreated,
                        Body = JsonConvert.SerializeObject(msg)
                    });
                        
                    await _dbContext.SaveChangesAsync();
                    //await transaction.CommitAsync();
                //}
            }
            else
            {
                //Log error here
            }
        }

        public async Task<Guid> CreateOrder(OrderForm form, string creatorEmail)
        {
            if(await _dbContext.Orders.AnyAsync(o => o.Id == form.OrderId))
                throw new InvalidOperationException($"Order with Id = {form.OrderId} already exists.");
           
            var entity = await _dbContext.Orders.AddAsync(new OrderEntity()
            {
                Id = form.OrderId.Value,
                DeliveryAddress = form.DeliveryAddress,
                DeliveryTime = form.DeliveryTime,
                OrderContent = form.OrderContent,
                RestaurantId = form.RestaurantId,
                UserPhone = form.UserPhone,
                CaloriesRecommendationId = form.CaloriesRecommendationId,
                TotalCost = form.TotalCost,
                UserEmail = creatorEmail,
                CreatedAt = DateTimeOffset.UtcNow,
                Status = OrderStatus.New
            });
            
            var msg = new OrderStatusUpdateNotification()
            {
                OrderId = form.OrderId.Value,
                NewStatus = OrderStatus.New
            };
            await _dbContext.NotificationsOutlog.AddAsync(new NotificationMessageEntity()
            {
                Topic = MessageTopic.Order,
                Kind = MessageKind.OrderStatusUpdate,
                Body = JsonConvert.SerializeObject(msg)
            });
                        
            await _dbContext.SaveChangesAsync();
            
            var created = await _dbContext.SaveChangesAsync();
            
            return entity.Entity.Id;
        }
    }
}