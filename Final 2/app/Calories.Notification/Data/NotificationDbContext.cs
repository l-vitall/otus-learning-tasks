using Calories.Notification.Model;
using Microsoft.EntityFrameworkCore;

namespace Calories.Notification.Data
{
    public sealed class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
        
        public DbSet<NotificationEntity> Notifications { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.UseSerialColumns();
    }
}