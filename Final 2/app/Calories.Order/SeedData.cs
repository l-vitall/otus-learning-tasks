using System;
using System.Threading.Tasks;
using Calories.Order.Model;
using Calories.Order.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Calories.Order
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            IHostEnvironment env = services.GetService<IHostEnvironment>();

            if (env.IsDevelopment())
            {
                await AddTestOrders(services.GetRequiredService<IOrderService>());
            }
        }

       private static async Task AddTestOrders(IOrderService service)
       {
           string email = "user@mailinator.com";
            var existingRecords = await service.GetAllOrders(email);
            
            if (existingRecords?.Length > 0)
                return;
            
            await service.CreateOrder(new OrderForm(){
                CreatedAt = DateTimeOffset.UtcNow,
                TotalCost = 999,
                DeliveryAddress = "User's address here",
                OrderContent = "Alot of pizza every day thrice a day",
                RestaurantId = 1,
                UserPhone = "999 -99-99-99",
                CaloriesRecommendationId = 1,
                OrderId = Guid.NewGuid()
            }, "user@mailinator.com");
        }
    }
}