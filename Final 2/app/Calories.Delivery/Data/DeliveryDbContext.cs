using Calories.Delivery.Model;
using Microsoft.EntityFrameworkCore;

namespace Calories.Delivery.Data
{
    public sealed class DeliveryDbContext : DbContext
    {
        public DeliveryDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
        
        public DbSet<DeliveryEntity> Deliveries { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.UseSerialColumns();
    }
}