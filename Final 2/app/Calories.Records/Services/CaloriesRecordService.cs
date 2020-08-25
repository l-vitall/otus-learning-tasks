using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Calories.Common.Models;
using Calories.Records.Data;
using Calories.Records.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Calories.Records.Services
{
    public class CaloriesRecordService : ICaloriesRecordService
    {
        private readonly CaloriesRecordsDbContext _dbContext;
        private readonly IConfigurationProvider _mappingConfiguration;
        private readonly ICaloriesApiService _caloriesApiService;
        private readonly IGrpcUserProfileClientService _userProfileService;

        public CaloriesRecordService(CaloriesRecordsDbContext context
            , IConfigurationProvider mappingConfiguration
            , ICaloriesApiService caloriesApiService
            , IGrpcUserProfileClientService userProfileService)
        {
            _dbContext = context;
            _mappingConfiguration = mappingConfiguration;
            _caloriesApiService = caloriesApiService;
            _userProfileService = userProfileService;
        }
        public async Task<CaloriesRecord> GetCaloriesRecordAsync(Guid externalId, string userEmail)
        {
            var record = await _dbContext.CaloriesRecords
                .SingleOrDefaultAsync(r => r.ExternalId == externalId && (userEmail == null || r.UserEmail == userEmail));
            
            var mapper = _mappingConfiguration.CreateMapper();
            return record == null ? null : mapper.Map<CaloriesRecord>(record);
        }

        public async Task<PagedResults<CaloriesRecord>> GetCaloriesRecordsAsync(PagingOptions pagingOptions
            , SortOptions<CaloriesRecord, CaloriesRecordEntity> sortOptions
            , SearchOptions<CaloriesRecord, CaloriesRecordEntity> searchOptions
            , string userEmail)
        {
            IQueryable<CaloriesRecordEntity> query = _dbContext.CaloriesRecords.Join(_dbContext.CaloriesNumberByDate,
                a => new {a.UserEmail, a.Date},
                b => new {b.UserEmail, b.Date},
                (t1, t2) => new CaloriesRecordEntity()
                {
                    Id = t1.Id,
                    Date = t1.Date,
                    Text = t1.Text,
                    Time = t1.Time,
                    UserEmail = t1.UserEmail,
                    ExternalId = t1.ExternalId,
                    NumberOfCalories = t1.NumberOfCalories,
                    DailyCaloriesLessThanExpected = t2.NumberOfCalories < t2.TargetNumberOfCalories
                });
            
            if(!string.IsNullOrWhiteSpace(userEmail))
                query = query.Where(r => r.UserEmail == userEmail);
            
            query = searchOptions.Apply(query);
            
            query = sortOptions.Apply(query);

            var size = await query.CountAsync();

            var items = await query
                .Skip(pagingOptions.Offset.Value)
                .Take(pagingOptions.Limit.Value)
                .ProjectTo<CaloriesRecord>(_mappingConfiguration)
                .ToArrayAsync();
            
            return new PagedResults<CaloriesRecord>()
            {
                Items = items,
                TotalSize = size
            };
        }

        public async Task<Guid> CreateCaloriesRecord(string userEmail, string text, int? numberOfCalories)
        {
            numberOfCalories ??= await _caloriesApiService.TryGetCalories(text);
            
            var utcNow = DateTimeOffset.UtcNow;
            var dailyCalories = _dbContext.CaloriesNumberByDate.SingleOrDefault(r => r.UserEmail == userEmail && r.Date == utcNow.Date);

            if (dailyCalories == null)
            {
                var userDailyCalories = await _userProfileService.GetDailyCaloriesNumber(userEmail);
                dailyCalories = new DailyNumberOfCalories()
                {
                    Date = utcNow.Date,
                    UserEmail = userEmail,
                    TargetNumberOfCalories = userDailyCalories ?? 3000
                };
                
                await _dbContext.CaloriesNumberByDate.AddAsync(dailyCalories);
            }

            dailyCalories.NumberOfCalories += numberOfCalories.Value; 
            
            var newcaloriesRecord = new CaloriesRecordEntity()
            {
                UserEmail = userEmail,
                Text = text,
                Date = utcNow.Date,
                Time = utcNow.TimeOfDay,
                NumberOfCalories = numberOfCalories.Value,
                ExternalId = Guid.NewGuid()
            };
            
            var newCaloriesRecord = await _dbContext.CaloriesRecords.AddAsync(newcaloriesRecord);

            var created = await _dbContext.SaveChangesAsync();
            if (created < 1)
                throw new InvalidOperationException("Could not create a calories record");
            
            return newCaloriesRecord.Entity.ExternalId;
        }

        public async Task DeleteCaloriesRecordAsync(Guid caloriesRecordId, string userEmail)
        {
            var caloriesRecord = await _dbContext.CaloriesRecords
                .SingleOrDefaultAsync(r => r.ExternalId == caloriesRecordId && (userEmail == null || r.UserEmail == userEmail));

            if (caloriesRecord == null)
                return;

            _dbContext.CaloriesRecords.Remove(caloriesRecord);
            
            var dailyRecord = await _dbContext.CaloriesNumberByDate.SingleOrDefaultAsync(c => c.UserEmail == caloriesRecord.UserEmail && c.Date == caloriesRecord.Date);
                
            if(dailyRecord != null)
                dailyRecord.NumberOfCalories -= caloriesRecord.NumberOfCalories;
            
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateCaloriesRecord(UpdateCaloriesRecordForm caloriesRecordModel, string userEmail)
        {
            var existingRecord = await _dbContext.CaloriesRecords.SingleOrDefaultAsync(r => r.ExternalId == caloriesRecordModel.Id && (userEmail == null || r.UserEmail == userEmail));

            if (existingRecord == null)
                return;

            var dailyCaloriesDelta = caloriesRecordModel.NumberOfCalories - existingRecord.NumberOfCalories;
            
            existingRecord.Text = caloriesRecordModel.Text;
            existingRecord.NumberOfCalories = caloriesRecordModel.NumberOfCalories;

            if (dailyCaloriesDelta != 0)
            {
                var dailyRecord = await _dbContext.CaloriesNumberByDate.SingleOrDefaultAsync(c => c.UserEmail == existingRecord.UserEmail && c.Date == existingRecord.Date);
                
                if(dailyRecord != null)
                    dailyRecord.NumberOfCalories += dailyCaloriesDelta;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}