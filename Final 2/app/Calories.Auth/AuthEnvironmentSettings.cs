using System;
using Calories.Common;

namespace Calories.Auth
{
    public class AuthEnvironmentSettings : EnvironmentSettings
    {
        public bool EmailConfirmationEnabled { get; set; }

        public AuthEnvironmentSettings()
        {
            var boom = Environment.GetEnvironmentVariable("CONFIRM_EMAIL");

            if (!string.IsNullOrWhiteSpace(boom))
                EmailConfirmationEnabled = boom == "1";
        }
    }
}