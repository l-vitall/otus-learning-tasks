using System;
using System.Linq;
using System.Security.Claims;

namespace Calories.Common
{
    public static class ClaimsPrincipalExtensions
    {
        public static string Email(this ClaimsPrincipal principal)
        {
            return principal.Claims.SingleOrDefault(c => c.Type.Equals("name", StringComparison.OrdinalIgnoreCase))?.Value;
        }
    }
}