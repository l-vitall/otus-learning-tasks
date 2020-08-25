using Calories.Common.Models;

namespace Calories.Recommendation.Model
{
    public class RecommendationsResponse : PagedCollection<CaloriesRecommendationEntity>
    {
        public Form CaloriesRecommendationQuery { get; set; }
    }
}