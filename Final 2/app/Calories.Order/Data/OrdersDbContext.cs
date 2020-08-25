using Calories.Common.Models;
using Calories.Order.Model;
using Microsoft.EntityFrameworkCore;

namespace Calories.Order.Data
{
    public sealed class OrdersDbContext : DbContext
    {
        public OrdersDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
        
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<NotificationMessageEntity> NotificationsOutlog { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.UseSerialColumns();
    }
}