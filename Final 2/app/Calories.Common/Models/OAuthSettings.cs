namespace Calories.Common.Models
{
    public class OAuthSettings
    {
        public string Secret { get; set; }
        public int TokenLifetimeMinutes { get; set; }
    }
}