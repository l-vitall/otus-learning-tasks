using System;
using Microsoft.AspNetCore.Identity;

namespace Calories.Auth.Model
{
    public class UserRoleEntity : IdentityRole<Guid>
    {
        public UserRoleEntity()
        {
        }
        
        public UserRoleEntity(string roleName) : base(roleName)
        {
        }
    }
}