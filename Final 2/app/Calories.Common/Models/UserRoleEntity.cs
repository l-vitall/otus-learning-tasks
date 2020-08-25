using System;
using Microsoft.AspNetCore.Identity;

namespace Calories.Common.Models
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