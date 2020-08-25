using System;
using System.Linq;
using System.Threading.Tasks;
using Calories.Auth.Model;
using Calories.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Calories.Auth
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            IHostEnvironment env = services.GetService<IHostEnvironment>();

            if (env.IsDevelopment())
            {
                await AddTestUsers(services.GetRequiredService<RoleManager<UserRoleEntity>>(), services.GetRequiredService<UserManager<UserEntity>>());
            }
        }

        private static async Task AddTestUsers(RoleManager<UserRoleEntity> roleManager, UserManager<UserEntity> userManager)
        {
            var dataExists = roleManager.Roles.Any() || userManager.Users.Any();
            
            if (dataExists)
                return;
            
            //Add a test role
            await roleManager.CreateAsync(new UserRoleEntity(AuthorizationRole.Administrator));
            await roleManager.CreateAsync(new UserRoleEntity(AuthorizationRole.User));
            await roleManager.CreateAsync(new UserRoleEntity(AuthorizationRole.Manager));
            
            //Add a test user Admin
            await CreateUser(userManager, "admin@mailinator.com", AuthorizationRole.Administrator);
            
            //Add a test user Manager
            await CreateUser(userManager, "manager@mailinator.com", AuthorizationRole.Manager);

            //Add a test user User
            await CreateUser(userManager, "user@mailinator.com", AuthorizationRole.User, Guid.Parse("FA24AA9B-409F-42B2-AA54-9A2084630C27"));
        }

        private static async Task<UserEntity> CreateUser(UserManager<UserEntity> userManager, string email, string role, Guid? userId = null)
        {
            var user = new UserEntity()
            {
                Email = email,
                UserName = email,
                SecurityStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true
            };

            if (userId.HasValue)
                user.Id = userId.Value;
            
            IdentityResult chkUser = await userManager.CreateAsync(user, "Qweqwe123?");
            
            if(chkUser.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
                await userManager.UpdateAsync(user);
            }

            return user;
        }
    }
}