using Calories.Recommendation.Model;
using Microsoft.EntityFrameworkCore;

namespace Calories.Recommendation.Data
{
    public sealed class RecommendationsDbContext : DbContext
    {
        public RecommendationsDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
        
        public DbSet<CaloriesRecommendationEntity> Recommendations { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.UseSerialColumns();
    }
}