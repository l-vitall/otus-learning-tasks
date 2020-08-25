using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserEntity = Calories.Auth.Model.UserEntity;
using UserRoleEntity = Calories.Auth.Model.UserRoleEntity;

namespace Calories.Auth.Data
{
    public sealed class AuthDbContext : IdentityDbContext<UserEntity, UserRoleEntity, Guid>
    {
        public AuthDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}