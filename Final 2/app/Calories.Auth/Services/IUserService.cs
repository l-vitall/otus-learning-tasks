using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Calories.Auth.Model;
using Calories.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace Calories.Auth.Services
{
    public interface IUserService
    {
        Task<(string code, string ErrorMessage)> CreateUserAsync(RegisterForm form);
        
        
        Task<Guid?> GetUserIdAsync(ClaimsPrincipal principal);

        Task<User> GetUserAsync(ClaimsPrincipal user);
        Task<User> GetUserAsync(string email);
        
        Task<User> GetUserByIdAsync(Guid userId);
        Task<Guid?> GetUserIdAsync(string email);
        Task<IdentityResult> ConfrimEmailAsync(string email, string code);
        Task<IdentityResult> UnlockUser(string email);
        Task<IdentityResult> DeactivateUser(User user);
        //Task<IdentityResult> UpdateUser(UpdateUserForm updateModel);
    }

}