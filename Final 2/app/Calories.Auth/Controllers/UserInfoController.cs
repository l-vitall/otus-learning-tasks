// using System;
// using System.Threading.Tasks;
// using AspNet.Security.OpenIdConnect.Primitives;
// using Calories.Common.Models;
// using Calories.Common.Services;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
//
// namespace Calories.Api.Controllers
// {
//     [Route("/[controller]")]
//     [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//     [ApiController]
//     public class UserInfoController : ControllerBase
//     {
//         private readonly IUserService _userService;
//         private readonly IAuthorizationService _authService;
//
//         public UserInfoController(IUserService userService,
//             IAuthorizationService authService)
//         {
//             _userService = userService;
//             _authService = authService;
//         }
//
//         [HttpGet("userInfoByEmail", Name = nameof(UserInfoByEmail))]
//         [ProducesResponseType(401)]
//         public async Task<ActionResult<UserInfoResponse>> UserInfoByEmail(EmailForm form)
//         {
//             var callerUser = await _userService.GetUserAsync(User);
//
//             if (callerUser == null)
//             {
//                 return BadRequest(new OpenIdConnectResponse()
//                 {
//                     Error = OpenIdConnectConstants.Errors.InvalidGrant,
//                     ErrorDescription = "The user does not exist."
//                 });
//             }
//
//             if (!callerUser.Email.Equals(form.Email, StringComparison.OrdinalIgnoreCase))
//             {
//                 var canUpdate = await _authService.AuthorizeAsync(User, AuthorizationPolicy.CruidAllUsers);
//                 if (!canUpdate.Succeeded)
//                     return Unauthorized();
//             }
//
//             var user = await _userService.GetUserAsync(form.Email);
//             var userId = await _userService.GetUserIdAsync(form.Email);
//             
//             return new UserInfoResponse()
//             {
//                 Self = Link.To(nameof(UserInfo)),
//                 GivenName = user.FirstName,
//                 FamilyName = user.LastName,
//                 Subject = Url.Link(nameof(UsersController.GetUserById),
//                     new {userId})
//             };
//         }
//
//         [HttpGet(Name = nameof(UserInfo))]
//         [ProducesResponseType(401)]
//         public async Task<ActionResult<UserInfoResponse>> UserInfo()
//         {
//             var user = await _userService.GetUserAsync(User);
//
//             if (user == null)
//             {
//                 return BadRequest(new OpenIdConnectResponse()
//                 {
//                     Error = OpenIdConnectConstants.Errors.InvalidGrant,
//                     ErrorDescription = "The user does not exist."
//                 });
//             }
//
//             var userId = await _userService.GetUserIdAsync(User);
//             return new UserInfoResponse()
//             {
//                 Self = Link.To(nameof(UserInfo)),
//                 GivenName = user.FirstName,
//                 FamilyName = user.LastName,
//                 Subject = Url.Link(nameof(UsersController.GetUserById),
//                     new {userId})
//             };
//         }
//     }
// }