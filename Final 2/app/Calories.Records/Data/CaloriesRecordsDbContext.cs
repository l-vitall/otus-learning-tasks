using Calories.Common;
using Calories.Common.Models;
using Calories.Records.Models;
using Calories.Records.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Calories.Records.Data
{
    public sealed class CaloriesRecordsDbContext : DbContext

    {
        private readonly UserResolverService _userService;
        private readonly GrpcEnvironmentSettings _environmentSettings;
        private readonly AppSettings _settings;

        public CaloriesRecordsDbContext(DbContextOptions options
            , UserResolverService userService
            , IOptions<AppSettings> settings
            , GrpcEnvironmentSettings environmentSettings) : base(options)
        {
            _userService = userService;
            _environmentSettings = environmentSettings;
            _settings = settings.Value;
            Database.EnsureCreated();
        }

        public DbSet<CaloriesRecordEntity> CaloriesRecords { get; set; }
        public DbSet<DailyNumberOfCalories> CaloriesNumberByDate { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.UseSerialColumns();
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!_settings.UseInMemoryDatabase)
            {
                var connString = GetTenant(_userService.GetUserEmail());
                optionsBuilder.UseNpgsql(connString, o => o.SetPostgresVersion(8, 10));
            }
            else
            {
                optionsBuilder.UseInMemoryDatabase("caloriesdb");
            }
                
            base.OnConfiguring(optionsBuilder);
        }

        private string GetTenant(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return _environmentSettings.DatabaseUri ?? _settings.CaloriesRecordsDbContext;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(_environmentSettings.DatabaseUri))
                    return _settings.CaloriesRecordsDbContext;
                else
                {
                    //TODO: use statistics to split email equally between shards
                    if (email.GetHashCode() % 2 == 0)
                        return _environmentSettings.DatabaseUri;
                    else
                        return _environmentSettings.Database2Uri;
                }
            }
        }

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var mapper = new Npgsql.NpgsqlSnakeCaseNameTranslator();
            var types = modelBuilder.Model.GetEntityTypes().ToList();

            // Refer to tables in snake_case internally
            types.ForEach(e => e.Relational().TableName = mapper.TranslateMemberName(e.Relational().TableName));

            // Refer to columns in snake_case internally
            types.SelectMany(e => e.GetProperties())
                .ToList()
                .ForEach(p => p.Relational().ColumnName = mapper.TranslateMemberName(p.Relational().ColumnName));
        }*/
    }
}