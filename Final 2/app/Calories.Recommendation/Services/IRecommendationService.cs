using System.Threading.Tasks;
using Calories.Recommendation.Model;

namespace Calories.Recommendation.Services
{
    public interface IRecommendationService
    {
        Task<CaloriesRecommendationEntity[]> GetAllRecommendations(string email);
        Task<long> CreateRecommendation(CaloriesRecommendationsForm form, string creatorEmail);
    }
}