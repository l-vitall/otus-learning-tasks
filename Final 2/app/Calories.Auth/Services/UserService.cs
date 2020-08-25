using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Calories.Auth.Model;
using Calories.Common.Kafka;
using Calories.Common.Models;
using Calories.Common.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Calories.Auth.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IConfigurationProvider _mappingConfiguration;
        private readonly IKafkaProducerService _kafkaProducerService;
        private readonly AuthEnvironmentSettings _environmentSettings;
        private readonly AppSettingsAuth _appSettings;

        public UserService(
            UserManager<UserEntity> userManager,
            IConfigurationProvider mappingConfiguration,
            IOptions<AppSettingsAuth> appSettings,
            IKafkaProducerService kafkaProducerService,
            AuthEnvironmentSettings environmentSettings)
        {
            _userManager = userManager;
            _mappingConfiguration = mappingConfiguration;
            _kafkaProducerService = kafkaProducerService;
            _environmentSettings = environmentSettings;
            _appSettings = appSettings.Value;
        }

        public async Task<(string code, string ErrorMessage)> CreateUserAsync(RegisterForm form)
        {
            var confirmEmail = _environmentSettings.EmailConfirmationEnabled || !_appSettings.BypassEmailConfirmation;
            
            var userEntity = new UserEntity()
            {
                Email = form.Email,
                UserName = form.Email,
                EmailConfirmed = !confirmEmail
            };

            var result = await _userManager.CreateAsync(userEntity, form.Password);

            if (!result.Succeeded)
            {
                var firstError = result.Errors.FirstOrDefault()?.Description;
                return (null, firstError);
            }

            await _userManager.AddToRoleAsync(userEntity, "User");
            await _userManager.UpdateAsync(userEntity);

            form.Password = null;
            
            await _kafkaProducerService.SendMessage(MessageTopic.User, MessageKind.UserCreated, JsonConvert.SerializeObject(form));

            
            var code = !confirmEmail ? null : await _userManager.GenerateEmailConfirmationTokenAsync(userEntity);
            
            return (code, null);
        }
        
        public async Task<User> GetUserAsync(ClaimsPrincipal user)
        {
            var entity = await _userManager.GetUserAsync(user);
            var mapper = _mappingConfiguration.CreateMapper();

            return mapper.Map<User>(entity);
        }

        public async Task<Guid?> GetUserIdAsync(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);
            if (user == null) return null;

            return user.Id;
        }
        
        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            var entity = await _userManager.Users
                .SingleOrDefaultAsync(x => x.Id == userId);
            var mapper = _mappingConfiguration.CreateMapper();

            return mapper.Map<User>(entity);
        }

        public async Task<IdentityResult> ConfrimEmailAsync(string email, string code)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return null;
            
            return await _userManager.ConfirmEmailAsync(user, code);
        }

        public async Task<IdentityResult> UnlockUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return null;
            
            await _userManager.ResetAccessFailedCountAsync(user);
            return await _userManager.SetLockoutEndDateAsync(user, null);
        }

        public async Task<IdentityResult> DeactivateUser(User user)
        {
            var userEntity = await _userManager.FindByEmailAsync(user.Email);
            
            var result = await _userManager.SetLockoutEndDateAsync(userEntity, DateTimeOffset.MaxValue);
            
            if(result.Succeeded)
                result = await _userManager.SetLockoutEnabledAsync(userEntity, true);

            return result;
        }

        //TODO: support password update / restore
        // public async Task<IdentityResult> UpdateUser(UpdateUserForm updateModel)
        // {
        //     var userEntity = await _userManager.FindByEmailAsync(updateModel.Email);
        //
        //     userEntity.FirstName = updateModel.FirstName;
        //     userEntity.LastName = updateModel.LastName;
        //     userEntity.DailyNumberOfCalories = updateModel.DailyNumberOfCalories;
        //
        //     return await _userManager.UpdateAsync(userEntity);
        // }

        public async Task<Guid?> GetUserIdAsync(string email)
        {
            var entity = await _userManager.Users.SingleOrDefaultAsync(x => x.Email == email);
            return entity?.Id;
        }

        public async Task<User> GetUserAsync(string email)
        {
            var entity = await _userManager.Users
                .SingleOrDefaultAsync(x => x.Email == email);
            
            var mapper = _mappingConfiguration.CreateMapper();

            return mapper.Map<User>(entity);
        }
    }
}