using System;
using System.Threading.Tasks;
using Calories.Recommendation.Model;
using Calories.Recommendation.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Calories.Recommendation
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            IHostEnvironment env = services.GetService<IHostEnvironment>();

            if (env.IsDevelopment())
            {
                await AddTestRecommendations(services.GetRequiredService<IRecommendationService>());
            }
        }

       private static async Task AddTestRecommendations(IRecommendationService service)
       {
           string email = "user@mailinator.com";
            var existingRecords = await service.GetAllRecommendations(email);
            
            if (existingRecords?.Length > 0)
                return;
            
            await service.CreateRecommendation(new CaloriesRecommendationsForm(){
                Text = "Eat MORE MORE AND MORE",
                CreatedAt = DateTimeOffset.UtcNow,
                StartDate = DateTimeOffset.UtcNow.Date,
                EndDate = DateTimeOffset.UtcNow.Date.AddDays(7),
                TotalCost = 999,
                UserEmail = email,
                DailyNumberOfCalories = 10000
            }, "manager@mailinator.com");
        }
    }
}