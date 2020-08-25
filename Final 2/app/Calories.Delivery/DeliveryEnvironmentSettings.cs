using System;
using Calories.Common;

namespace Calories.Delivery
{
    public class DeliveryEnvironmentSettings : EnvironmentSettings
    {
        public bool TestMode { get; set; }

        public DeliveryEnvironmentSettings()
        {
            var boom = Environment.GetEnvironmentVariable("TEST_MODE");

            if (!string.IsNullOrWhiteSpace(boom))
                TestMode = boom == "1";
        }
    }
}