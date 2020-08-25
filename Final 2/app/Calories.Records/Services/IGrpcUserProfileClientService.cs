using System.Threading.Tasks;

namespace Calories.Records.Services
{
    public interface IGrpcUserProfileClientService
    {
        Task<int?> GetDailyCaloriesNumber(string email);
    }
}