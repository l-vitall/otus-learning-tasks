using System;

namespace Calories.Common
{
    public class EnvironmentSettings
    {
        public string Secret { get; set; }
        public string DatabaseUri { get; set; }
        public string KafkaConsumerUri { get; set; }
        public string KafkaProducerUri { get; set; }
        public string HostName { get; set; }
        public string Greeting { get; set; }
        
        public bool CacheEnabled { get; set; }

        public EnvironmentSettings()
        {
            Secret = Environment.GetEnvironmentVariable("SECRET");
            Greeting = Environment.GetEnvironmentVariable("GREETING");
            DatabaseUri = Environment.GetEnvironmentVariable("DATABASE_URI");
            KafkaConsumerUri = Environment.GetEnvironmentVariable("KAFKA_CONSUMER_URI");
            KafkaProducerUri = Environment.GetEnvironmentVariable("KAFKA_PRODUCER_URI");
            HostName = Environment.GetEnvironmentVariable("HOSTNAME");

            var cacheEnabled = Environment.GetEnvironmentVariable("CACHE_ENABLED");

            if (!string.IsNullOrWhiteSpace(cacheEnabled))
                CacheEnabled = cacheEnabled == "1";
        }
    }
}