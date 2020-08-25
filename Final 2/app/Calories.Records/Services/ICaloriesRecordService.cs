using System;
using System.Threading.Tasks;
using Calories.Common.Models;
using Calories.Records.Models;

namespace Calories.Records.Services
{
    public interface ICaloriesRecordService
    {
        Task<CaloriesRecord> GetCaloriesRecordAsync(Guid externalId, string userEmail);
        Task<PagedResults<CaloriesRecord>> GetCaloriesRecordsAsync(PagingOptions pagingOptions
            , SortOptions<CaloriesRecord, CaloriesRecordEntity> sortOptions
            , SearchOptions<CaloriesRecord, CaloriesRecordEntity> searchOptions
            , string userEmail);

        Task<Guid> CreateCaloriesRecord(string userEmail, string text, int? formNumberOfCalories);
        Task DeleteCaloriesRecordAsync(Guid externalId, string userEmail);
        Task UpdateCaloriesRecord(UpdateCaloriesRecordForm caloriesRecordModel, string userEmail);
    }
}