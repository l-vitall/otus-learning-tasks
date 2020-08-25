using System.Threading.Tasks;

namespace Calories.Records.Services
{
    public interface ICaloriesApiService
    {
        Task<int> TryGetCalories(string text);
    }
}