using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using AspNet.Security.OpenIdConnect.Primitives;
using Calories.Auth.Model;
using Calories.Auth.Services;
using Calories.Common;
using Calories.Common.Kafka;
using Calories.Common.Models;
using Calories.Common.Notifications;
using Calories.Common.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Calories.Auth.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly AppSettingsAuth _appSettings;
        private readonly IAuthorizationService _authService;

        private readonly IKafkaProducerService _kafkaProducerService;

        //private readonly IEmailSendingService _emailSendingService;
        private readonly IMemoryCache _memoryCache;
        private readonly AuthEnvironmentSettings _envSettings;
        private readonly PagingOptions _defaultPagingOptions;

        private static ConcurrentDictionary<int, object> _usersLocks = new ConcurrentDictionary<int, object>();

        public UsersController(
            IUserService userService,
            IOptions<PagingOptions> defaultPagingOptions,
            IOptions<AppSettingsAuth> appSettings,
            IAuthorizationService authService,
            IKafkaProducerService kafkaProducerService,
            IMemoryCache memoryCache,
            AuthEnvironmentSettings envSettings)
        {
            _userService = userService;
            _appSettings = appSettings.Value;
            _authService = authService;
            _kafkaProducerService = kafkaProducerService;
            //_emailSendingService = emailSendingService;
            _memoryCache = memoryCache;
            _envSettings = envSettings;
            _defaultPagingOptions = defaultPagingOptions.Value;
        }

        // [HttpGet(Name = nameof(GetVisibleUsers))]
        // [ResponseCache(Duration = 30, VaryByQueryKeys = new[] {"offset", "limit", "orderBy", "search"})]
        // public async Task<ActionResult<PagedCollection<User>>> GetVisibleUsers(
        //     [FromQuery] PagingOptions pagingOptions,
        //     [FromQuery] SortOptions<User, UserEntity> sortOptions,
        //     [FromQuery] SearchOptions<User, UserEntity> searchOptions)
        // {
        //     pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
        //     pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;
        //
        //     var users = new PagedResults<User>();
        //
        //     if (User.Identity.IsAuthenticated)
        //     {
        //         var canSeeEveryone = await _authService.AuthorizeAsync(User, AuthorizationPolicy.CruidAllUsers);
        //
        //         if (canSeeEveryone.Succeeded)
        //         {
        //             if (_envSettings.CacheEnabled)
        //             {
        //                 var cacheKey = $"{pagingOptions.Offset}{pagingOptions.Limit}{sortOptions.OrderBy}{searchOptions.Search}".GetHashCode();
        //                 users = _memoryCache.GetOrCreate(cacheKey, entry =>
        //                 {
        //                     lock (_usersLocks.GetOrAdd(cacheKey, new object()))
        //                     {
        //                         var value = _memoryCache.Get<PagedResults<User>>(cacheKey);
        //
        //                         if (value == null)
        //                         {
        //                             Task.Delay(1000).GetAwaiter().GetResult();
        //                             entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
        //                             value = _userService.GetUsersAsync(pagingOptions, sortOptions, searchOptions)
        //                                 .GetAwaiter().GetResult();
        //                         }
        //
        //                         _usersLocks.TryRemove(cacheKey, out _);
        //                         return value;
        //                     }
        //                 });
        //             }
        //             else
        //                 users = await _userService.GetUsersAsync(pagingOptions, sortOptions, searchOptions);
        //         }
        //         else
        //         {
        //             User myself = null;
        //             if (_envSettings.CacheEnabled)
        //             {
        //                 var cacheKey = User.Identity.Name;
        //                 myself = await _memoryCache.GetOrCreateAsync<User>(cacheKey, async entry =>
        //                 {
        //                     entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
        //                     return await _userService.GetUserAsync(User);
        //                 });
        //             }
        //             else
        //                 myself = await _userService.GetUserAsync(User);
        //
        //             users.Items = new[] {myself};
        //             users.TotalSize = 1;
        //         }
        //     }
        //     else
        //     {
        //         users.Items = new List<User>();
        //     }
        //
        //     var collection = PagedCollection<User>.Create(
        //         Link.ToCollection(nameof(GetVisibleUsers)),
        //         users.Items.ToArray(),
        //         users.TotalSize,
        //         pagingOptions);
        //
        //     return collection;
        // }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [HttpGet("{userId}", Name = nameof(GetUserById))]
        public async Task<ActionResult<User>> GetUserById(Guid userId)
        {
            var currentUserId = await _userService.GetUserIdAsync(User);
            if (currentUserId == null)
                return NotFound();
        
            if (currentUserId == userId)
            {
                var myself = await _userService.GetUserAsync(User);
                return myself;
            }
        
            var canSeeEveryone = await _authService.AuthorizeAsync(User, AuthorizationPolicy.CruidAllUsers);
            if (!canSeeEveryone.Succeeded)
                return NotFound();
        
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound();
        
            return user;
        }

        //POST /users
        [HttpPost("registerByInvitation", Name = nameof(RegisterByInvitation))]
        [ProducesResponseType(400)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> RegisterByInvitation([FromForm] RegisterForm form)
        {
            return await Register(form);
        }

        //POST /users
        [HttpPost(Name = nameof(RegisterUser))]
        [ProducesResponseType(400)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterForm form)
        {
            return await Register(form);
        }

        private async Task<IActionResult> Register(RegisterForm form)
        {
            var (confirmationCode, message) = await _userService.CreateUserAsync(form);

            if (!string.IsNullOrEmpty(confirmationCode))
            {
                var callbackUrl = Url.Action(
                    "ConfirmEmail",
                    "Users",
                    new {email = form.Email, confirmationCode = HttpUtility.UrlEncode(confirmationCode)},
                    protocol: HttpContext.Request.Scheme);

                var confirmMessage = new ConfirmEmailNotification()
                {
                    Email = form.Email,
                    Link = callbackUrl
                };

                await _kafkaProducerService.SendMessage(MessageTopic.Notification, MessageKind.ConfirmationEmailRequest, JsonConvert.SerializeObject(confirmMessage));
                
                return Content("To complete the registration please confirm your email");
            }

            if (string.IsNullOrEmpty(message) && _appSettings.BypassEmailConfirmation)
            {
                var userId = await _userService.GetUserIdAsync(form.Email);
                return Created(
                    Url.Link(nameof(GetUserById), new {userId}),
                    null);
            }

            return BadRequest(new ApiError()
            {
                Message = "Registration failed",
                Detail = message
            });
        }

        [HttpGet("confirmEmail", Name = nameof(ConfirmEmail))]
        [ProducesResponseType(400)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailForm form)
        {
            var result = await _userService.ConfrimEmailAsync(form.Email, HttpUtility.UrlDecode(form.ConfirmationCode));

            if (result == null)
                return NotFound();

            if (result.Succeeded)
            {
                await _kafkaProducerService.SendMessage(MessageTopic.Notification, MessageKind.NotificationCreated, JsonConvert.SerializeObject(new NotificationMessage()
                {
                    Email = form.Email,
                    Text = "Email confirmed!"
                }));
                return Ok("Email confirmed successfully.");
            }

            return BadRequest(new ApiError()
            {
                Message = "Failed to confirm email"
            });
        }

        [HttpPost("unlockUser", Name = nameof(UnlockUser))]
        [ProducesResponseType(400)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> UnlockUser(EmailForm form)
        {
            var canUnlock = await _authService.AuthorizeAsync(User, AuthorizationPolicy.UnlockUsers);
            if (!canUnlock.Succeeded)
                return Unauthorized();

            var result = await _userService.UnlockUser(form.Email);

            if (result.Succeeded)
                return Ok("User unlocked");

            return BadRequest(new ApiError()
            {
                Message = "Failed to unlock user"
            });
        }

        [HttpPost("inviteUser", Name = nameof(InviteUser))]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> InviteUser(EmailForm form)
        {
            var canUnlock = await _authService.AuthorizeAsync(User, AuthorizationPolicy.InviteUsers);
            if (!canUnlock.Succeeded)
                return Unauthorized();

            var registerUrl = Url.Link("registerByInvitation", null);

            //TODO: throw message to Notifications service here
            //var result = await _emailSendingService.SendInviteEmailAsync(form.Email, registerUrl);

            // if (result.StatusCode != HttpStatusCode.Accepted)
            //     return BadRequest(new ApiError()
            //     {
            //         Message = "Unable to send confirmation email"
            //     });

            return Ok("Invitation sent");
        }

        //DELETE /{caloriesRecordId}
        [HttpDelete(Name = nameof(DeleteUserByEmail))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUserByEmail(EmailForm form)
        {
            var canDelete = await _authService.AuthorizeAsync(User, AuthorizationPolicy.CruidAllUsers);
            if (!canDelete.Succeeded)
                return Unauthorized();

            var user = await _userService.GetUserAsync(form.Email);
            if (user == null)
                return NotFound();

            var result = await _userService.DeactivateUser(user);

            if (!result.Succeeded)
            {
                return BadRequest(new ApiError()
                {
                    Message = "Unable to deactivate user",
                    Detail = result.Errors.FirstOrDefault()?.Description ?? $"Failed to deactivate user {user.Email}"
                });
            }

            //TODO: throw message for UserService that user is deactivated
            
            return NoContent();
        }

        //TODO: support password updating / restoring
        // // PUT: /caloriesRecords
        // [HttpPut(Name = nameof(UpdateUser))]
        // [ProducesResponseType(204)]
        // [ProducesResponseType(401)]
        // [ProducesResponseType(400)]
        // [ProducesResponseType(404)]
        // public async Task<IActionResult> UpdateUser([FromBody] UpdateUserForm updateModel)
        // {
        //     var user = await _userService.GetUserAsync(User);
        //     if (user == null)
        //         return BadRequest(new OpenIdConnectResponse()
        //         {
        //             Error = OpenIdConnectConstants.Errors.InvalidGrant,
        //             ErrorDescription = "The user does not exist."
        //         });
        //
        //     if (!user.Email.Equals(updateModel.Email, StringComparison.OrdinalIgnoreCase))
        //     {
        //         var canUpdate = await _authService.AuthorizeAsync(User, AuthorizationPolicy.CruidAllUsers);
        //         if (!canUpdate.Succeeded)
        //             return Unauthorized();
        //     }
        //
        //     var result = await _userService.UpdateUser(updateModel);
        //
        //     if (!result.Succeeded)
        //     {
        //         return BadRequest(new ApiError()
        //         {
        //             Message = "Unable to update user",
        //             Detail = result.Errors.FirstOrDefault()?.Description ?? $"Failed to update user {user.Email}"
        //         });
        //     }
        //
        //     return NoContent();
        // }
    }
}