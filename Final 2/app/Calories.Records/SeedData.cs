using System;
using System.Linq;
using System.Threading.Tasks;
using Calories.Records.Data;
using Calories.Records.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Calories.Records
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            IHostEnvironment env = services.GetService<IHostEnvironment>();

            if (env.IsDevelopment())
            {
                await AddTestData(services.GetRequiredService<CaloriesRecordsDbContext>());
            }
        }

        public static async Task AddTestData(CaloriesRecordsDbContext context)
        {
            if (context.CaloriesRecords.Any())
                return;

            context.CaloriesRecords.Add(new CaloriesRecordEntity()
            {
                Date = DateTime.Parse("2020-07-21"),
                Time = TimeSpan.Parse("14:31"),
                Text = "Hamburger",
                NumberOfCalories = 900,
                UserEmail = "user@mailinator.com",
                ExternalId = Guid.Parse("9A4986DE-1D2C-4CC1-84B2-7898750F9754")
            });

            context.CaloriesRecords.Add(new CaloriesRecordEntity()
            {
                Date = DateTime.Parse("2020-07-21"),
                Time = TimeSpan.Parse("15:31"),
                Text = "Cheeseburger",
                NumberOfCalories = 600,
                UserEmail = "user@mailinator.com", 
                ExternalId = Guid.NewGuid()
            });

            context.CaloriesRecords.Add(new CaloriesRecordEntity()
            {
                Date = DateTime.Parse("2020-07-21"),
                Time = TimeSpan.Parse("16:31"),
                Text = "Bread",
                NumberOfCalories = 50,
                UserEmail = "user@mailinator.com", 
                ExternalId = Guid.NewGuid()
            });

            context.CaloriesRecords.Add(new CaloriesRecordEntity()
            {
                Date = DateTime.Parse("2020-07-21"),
                Time = TimeSpan.Parse("17:31"),
                Text = "Tea",
                NumberOfCalories = 10,
                UserEmail = "user@mailinator.com", 
                ExternalId = Guid.NewGuid()
            });

            context.CaloriesRecords.Add(new CaloriesRecordEntity()
            {
                Date = DateTime.Parse("2020-07-22"),
                Time = TimeSpan.Parse("18:31"),
                Text = "Pizza",
                NumberOfCalories = 1000,
                UserEmail = "user@mailinator.com", 
                ExternalId = Guid.NewGuid()
            });

            context.CaloriesNumberByDate.Add(new DailyNumberOfCalories()
            {
                Date = DateTime.Parse("2020-07-21"),
                UserEmail = "user@mailinator.com",
                TargetNumberOfCalories = 3000,
                NumberOfCalories = context.CaloriesRecords.Where(t => t.Date == DateTime.Parse("2020-07-21")).Sum(t => t.NumberOfCalories)
            });

            context.CaloriesNumberByDate.Add(new DailyNumberOfCalories()
            {
                Date = DateTime.Parse("2020-07-22"),
                UserEmail = "user@mailinator.com",
                TargetNumberOfCalories = 3000,
                NumberOfCalories = context.CaloriesRecords.Where(t => t.Date == DateTime.Parse("2020-07-22")).Sum(t => t.NumberOfCalories)
            });

            await context.SaveChangesAsync();
        }
    }
}