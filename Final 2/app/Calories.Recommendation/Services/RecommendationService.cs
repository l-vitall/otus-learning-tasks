using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Calories.Common.Kafka;
using Calories.Common.Models;
using Calories.Common.Services;
using Calories.Recommendation.Data;
using Calories.Recommendation.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Calories.Recommendation.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly RecommendationsDbContext _dbContext;
        private readonly IKafkaProducerService _kafkaProducerService;

        public RecommendationService(RecommendationsDbContext dbContext,
            IKafkaProducerService kafkaProducerService)
        {
            _dbContext = dbContext;
            _kafkaProducerService = kafkaProducerService;
        }

        public async Task<CaloriesRecommendationEntity[]> GetAllRecommendations(string email)
        {
            return await _dbContext.Recommendations.Where(r => r.UserEmail == email).ToArrayAsync();
        }
        
        public async Task<long> CreateRecommendation(CaloriesRecommendationsForm form, string creatorEmail)
        {
            var entity = await _dbContext.Recommendations.AddAsync(new CaloriesRecommendationEntity()
            {
                Text    = form.Text,
                CreatedAt = form.CreatedAt,
                CreatorEmail = creatorEmail,
                EndDate = form.EndDate,
                StartDate = form.StartDate,
                TotalCost = form.TotalCost,
                UserEmail = form.UserEmail
            });
           
            var created = await _dbContext.SaveChangesAsync();

            if (created < 1)
                throw new InvalidOperationException("Could not create a notification");

            var notification = new NotificationMessage()
            {
                Email = form.UserEmail,
                Text = "New Calories Recommendation is created by " + creatorEmail
            };
            await _kafkaProducerService.SendMessage(MessageTopic.Notification, MessageKind.NotificationCreated, JsonConvert.SerializeObject(notification));
            return entity.Entity.Id;
        }
    }
}