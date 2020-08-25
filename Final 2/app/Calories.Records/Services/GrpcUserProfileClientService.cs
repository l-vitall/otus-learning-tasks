using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcUserProfileServer;
using Serilog;

namespace Calories.Records.Services
{
    public class GrpcUserProfileClientService : IGrpcUserProfileClientService
    {
        private GrpcUserProfile.GrpcUserProfileClient _client;
        
        protected ILogger Logger { get; }

        public GrpcUserProfileClientService(GrpcEnvironmentSettings settings)
        {
            Logger = Log.ForContext(GetType());
            
            Logger.Information($"Constructing GPRC service...");
            var channel = GrpcChannel.ForAddress(settings.GrpcUserServer ?? "http://localhost:5025");
            _client = new GrpcUserProfile.GrpcUserProfileClient(channel);
            Logger.Information($"Constructed GPRC service.");
        }
        
        public async Task<int?> GetDailyCaloriesNumber(string email)
        {
            try
            {
                Logger.Information($"Getting GPRC user profile for email = {email}...");
                var userProfile = await _client.GetShopperProfileAsync(new GrpcUserProfileRequest() { Email = email });
                Logger.Information("Gprc user profile obtained.");
                return userProfile.ShopperDetails?.DailyNumberOfCalories;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to get GPRC profile");
                return 7000;
            }

        }
    }
}