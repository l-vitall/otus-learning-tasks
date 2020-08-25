using Calories.Common;
using Microsoft.AspNetCore.Http;

namespace Calories.Records.Services
{
    public class UserResolverService
    {
        private readonly IHttpContextAccessor _context;
        public UserResolverService(IHttpContextAccessor context)
        {
            _context = context;
        }

        public string GetUserEmail()
        {
            return _context.HttpContext?.User?.Email();
        }
    }
}