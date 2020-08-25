using System;
using System.Linq;
using System.Threading.Tasks;
using Calories.UserGrpc.Data;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcUserProfileServer;
using Microsoft.EntityFrameworkCore;

namespace Calories.UserGrpc.Services
{
    public class GrpcUserProfileService : GrpcUserProfile.GrpcUserProfileBase
    {
        private readonly UserDbContext _dbContext;

        public GrpcUserProfileService(UserDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<GrpcUserProfileResponse> GetShopperProfile(GrpcUserProfileRequest request, ServerCallContext context)
        {
            var user = await _dbContext.UserProfiles
                .SingleOrDefaultAsync(u => u.Email == request.Email);

            var result = new GrpcUserProfileResponse();
            
            if (user != null)
                result.ShopperDetails = new GrpcUserDetails()
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DailyNumberOfCalories = user.DailyNumberOfCalories,
                    CreatedAt = Timestamp.FromDateTimeOffset(user.CreatedAt)
                };

            return result;
        }
    }
}