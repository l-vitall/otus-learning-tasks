using System;
using System.Threading.Tasks;
using Calories.Restaurant.Model;
using Calories.Restaurant.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Calories.Restaurant
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            IHostEnvironment env = services.GetService<IHostEnvironment>();

            if (env.IsDevelopment())
            {
                await AddTestMenuItems(services.GetRequiredService<IMenuItemService>());
                await AddTestOrders(services.GetRequiredService<IRestaurantOrderService>());
            }
        }

        private static async Task AddTestOrders(IRestaurantOrderService service)
        {
            var existingRecords = await service.GetAllOrders();
            
            if (existingRecords?.Length > 0)
                return;
            
            await service.CreateAcceptedOrder(new RestaurantOrderEntity()
            {
               Details = "Vasia Pupkin's order. Consists of these positions: {... list ...}.",
               Status = RestaurantOrderStatus.Completed,
               OrderId = Guid.NewGuid()
            });
        }

        private static async Task AddTestMenuItems(IMenuItemService service)
        {
            var existingRecords = await service.GetAllMenuitems();
            
            if (existingRecords?.Length > 0)
                return;
            
            await service.CreateMenuItem(new RestaurantMenuItemEntity()
            {
                Kind= MenuItemKind.Soup,
                Name = "Borscht",
                    CaloriesCount = 600,
                    CreatedAt = DateTimeOffset.UtcNow,
                    RestaurantId = 1,
                    Comment = "Includes donuts"
            });
            
            await service.CreateMenuItem(new RestaurantMenuItemEntity()
            {
                Kind= MenuItemKind.Hot,
                Name = "Grilled pork & potatoes",
                CaloriesCount = 1000,
                CreatedAt = DateTimeOffset.UtcNow,
                RestaurantId = 1,
                Comment = "Medium degree of roast by default"
            });
            
            await service.CreateMenuItem(new RestaurantMenuItemEntity()
            {
                Kind= MenuItemKind.Poison,
                Name = "Coca-Cola",
                CaloriesCount = 10,
                CreatedAt = DateTimeOffset.UtcNow,
                RestaurantId = 1,
                Comment = null
            });
        }
    }
}