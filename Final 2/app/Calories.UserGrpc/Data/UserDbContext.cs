using Calories.UserGrpc.Model;
using Microsoft.EntityFrameworkCore;

namespace Calories.UserGrpc.Data
{
    public sealed class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
        
        public DbSet<UserProfileEntity> UserProfiles { get; set; }
    }
}