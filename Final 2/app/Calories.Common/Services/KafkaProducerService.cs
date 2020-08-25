using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Serilog;

namespace Calories.Common.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private ProducerConfig _config;
        protected ILogger Logger { get; }
        
        public KafkaProducerService(ProducerConfig config)
        {
            _config = config;
            Logger = Log.ForContext(GetType());
        }

        public async Task SendMessage(string messageTopic, string messageKind, string messageBody)
        {
            Logger.Information("Building config.. {@Config}", _config);
            // Create a producer that can be used to send messages to kafka that have no key and a value of type string 
            using var p = new ProducerBuilder<string, string>(_config).Build();

            try
            {
                // Construct the message to send (generic type must match what was used above when creating the producer)
                var message = new Message<string, string>
                {
                    Key = messageKind,
                    Value = messageBody
                };

                // Send the message to our test topic in Kafka
                Logger.Information("Sending message..");
                var dr = await p.ProduceAsync(messageTopic, message);
                Logger.Information($"Message to {messageTopic}.{messageKind} is sent with status = {dr.Status}");
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to send message");
            }
            
        }
    }
}