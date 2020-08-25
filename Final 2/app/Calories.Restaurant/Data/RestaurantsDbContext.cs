using Calories.Restaurant.Model;
using Microsoft.EntityFrameworkCore;

namespace Calories.Restaurant.Data
{
    public sealed class RestaurantsDbContext : DbContext
    {
        public RestaurantsDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
        
        public DbSet<RestaurantMenuItemEntity> MenuItems { get; set; }
        public DbSet<RestaurantOrderEntity> Orders { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.UseSerialColumns();
    }
}