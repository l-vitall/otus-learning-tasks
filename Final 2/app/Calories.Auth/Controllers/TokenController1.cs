// using System;
// using System.IdentityModel.Tokens.Jwt;
// using System.Linq;
// using System.Security.Claims;
// using System.Text;
// using System.Threading.Tasks;
// using Calories.Auth.Models;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Options;
// using Microsoft.IdentityModel.Tokens;
//
// namespace Calories.Auth.Controllers
// {
//     [Route("/[controller]")]
//     [ApiController]
//     public class TokenController : ControllerBase
//     {
//         private readonly OAuthSettings _oauthSettings;
//         private readonly SignInManager<UserEntity> _signInManager;
//         private readonly UserManager<UserEntity> _userManager;
//
//         public TokenController(
//             IOptions<OAuthSettings> oauthSettings,
//             SignInManager<UserEntity> signInManager,
//             UserManager<UserEntity> userManager)
//         {
//             _oauthSettings = oauthSettings.Value;
//             _signInManager = signInManager;
//             _userManager = userManager;
//         }
//
//         [HttpPost(Name = nameof(Login))]
//         [ProducesResponseType(200)]
//         [ProducesResponseType(400)]
//         public async Task<ActionResult<LoginResponse>> Login(LoginForm form)
//         {
//             var user = await _userManager.FindByNameAsync(form.Email);
//             if (user == null)
//             {
//                 return BadRequest(new ApiError
//                 {
//                     Message = "The username or password is invalid."
//                 });
//             }
//
//             // Ensure the user is allowed to sign in
//             if (!await _signInManager.CanSignInAsync(user))
//             {
//                 return BadRequest(new ApiError
//                 {
//                     Message = "The specified user is not allowed to sign in."
//                 });
//             }
//
//             // Ensure the user is not already locked out
//             if (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user))
//             {
//                 return BadRequest(new ApiError
//                 {
//                     Message = "The username or password is invalid."
//                 });
//             }
//
//             // Ensure the password is valid
//             if (!await _userManager.CheckPasswordAsync(user, form.Password))
//             {
//                 if (_userManager.SupportsUserLockout)
//                 {
//                     await _userManager.AccessFailedAsync(user);
//                 }
//
//                 return BadRequest(new ApiError
//                 {
//                     Message = "The username or password is invalid."
//                 });
//             }
//
//             // Reset the lockout count
//             if (_userManager.SupportsUserLockout)
//             {
//                 await _userManager.ResetAccessFailedCountAsync(user);
//             }
//
//             // Look up the user's roles (if any)
//             string role = null;
//             if (_userManager.SupportsUserRole)
//             {
//                 role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
//
//                 if (string.IsNullOrEmpty(role))
//                 {
//                     return BadRequest(new ApiError
//                     {
//                         Message = "The specified user is not allowed to sign in."
//                     });
//                 }
//             }
//
//             // Create a new authentication ticket w/ the user identity
//             var token = CreateJwtToken(user, role);
//
//             return Ok(new LoginResponse()
//             {
//                 Self = Link.ToForm(nameof(Login)),
//                 Email = form.Email,
//                 Token = token,
//                 TokenType = "Bearer",
//                 ExpiresIn = _oauthSettings.TokenLifetimeMinutes
//             });
//         }
//
//         private string CreateJwtToken(UserEntity user, string role)
//         {
//             // authentication successful so generate jwt token
//             var tokenHandler = new JwtSecurityTokenHandler();
//             var key = Encoding.ASCII.GetBytes(_oauthSettings.Secret);
//             var tokenDescriptor = new SecurityTokenDescriptor
//             {
//                 Subject = new ClaimsIdentity(new Claim[]
//                 {
//                     new Claim(ClaimTypes.Name, user.Id.ToString()),
//                     new Claim(ClaimTypes.Role, role)
//                 }),
//                 Expires = DateTime.UtcNow.AddMinutes(_oauthSettings.TokenLifetimeMinutes),
//                 SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//             };
//             var token = tokenHandler.CreateToken(tokenDescriptor);
//             return tokenHandler.WriteToken(token);
//         }
//     }
// }