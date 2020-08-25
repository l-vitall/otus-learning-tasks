using System;
using System.Linq;
using System.Threading.Tasks;
using Calories.Notification.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Calories.Notification
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            IHostEnvironment env = services.GetService<IHostEnvironment>();

            if (env.IsDevelopment())
            {
                await AddTestUserProfiles(services.GetRequiredService<INotificationService>());
            }
        }

        private static async Task AddTestUserProfiles(INotificationService userService)
        {
            var existingRecords = await userService.GetAllNotifications("user@mailinator.com");
            
            if (existingRecords?.Items?.Count() > 0)
                return;
            
            await userService.CreateNotification(new Common.Models.NotificationMessage(){Email = "user@mailinator.com", Text = "Your all all set!", CreatedAt = DateTimeOffset.UtcNow});
            await userService.CreateNotification(new Common.Models.NotificationMessage(){Email = "user@mailinator.com", Text = "You are registered!", CreatedAt = DateTimeOffset.UtcNow});
            await userService.CreateNotification(new Common.Models.NotificationMessage(){Email = "user@mailinator.com", Text = "You have new recommendations!", CreatedAt = DateTimeOffset.UtcNow});
        }
    }
}