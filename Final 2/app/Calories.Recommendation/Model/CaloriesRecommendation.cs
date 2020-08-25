using System;

namespace Calories.Recommendation.Model
{
    public class CaloriesRecommendation
    {
        public string Text { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string UserEmail { get; set; }
        public string CreatorEmail { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public int TotalCost { get; set; }
    }
}