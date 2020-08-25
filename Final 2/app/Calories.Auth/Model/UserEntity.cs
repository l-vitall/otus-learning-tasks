using System;
using Microsoft.AspNetCore.Identity;

namespace Calories.Auth.Model
{
    public class UserEntity : IdentityUser<Guid>
    {
    }
}