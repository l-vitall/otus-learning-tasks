using System.Text.Json.Serialization;
using AspNet.Security.OpenIdConnect.Primitives;
using Calories.Common.Models;

namespace Calories.Auth.Model
{
    public class UserInfoResponse : Resource
    {
        [JsonPropertyName(OpenIdConnectConstants.Claims.Subject)]
        public string Subject { get; set; }

        [JsonPropertyName(OpenIdConnectConstants.Claims.GivenName)]
        public string GivenName { get; set; }

        [JsonPropertyName(OpenIdConnectConstants.Claims.FamilyName)]
        public string FamilyName { get; set; }
    }
}