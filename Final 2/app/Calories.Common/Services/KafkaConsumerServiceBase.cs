using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Serilog;

namespace Calories.Common.Services
{
    public abstract class KafkaConsumerServiceBase : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly ConsumerConfig _consumerConfig;

        protected ILogger Logger { get; }

        private readonly string _subscribeTopic;
        
        public KafkaConsumerServiceBase(ConsumerConfig consumerConfig,
            string subscribeTopic)
        {  
            _consumerConfig = consumerConfig;

            Logger = Log.ForContext(GetType());
            _subscribeTopic = subscribeTopic;
        } 
        
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(() => StartConsumer(stoppingToken), stoppingToken);  
            return Task.CompletedTask;  
        }
        private void StartConsumer(CancellationToken stoppingToken)
        {
            Logger.Information("Building consumer.. {@Config}", _consumerConfig);
            using var consumer = new ConsumerBuilder<string, string>(_consumerConfig).Build();
            
            Logger.Information("Subscribing to topic {@topic}", _subscribeTopic);
            consumer.Subscribe(_subscribeTopic);
            
            Logger.Information("Starting message consuming..");
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var message = consumer.Consume().Message;

                        Logger.Information($"Received Kafka message with key = {_subscribeTopic + ":" + message.Key} and value = {message.Value}");
                        
                        if (string.IsNullOrEmpty(message.Value))
                            continue;

                        OnMessageReceive(message);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "Failed to process message");
                        Task.Delay(3000).GetAwaiter().GetResult();
                    }
                }
        }

        protected abstract void OnMessageReceive(Message<string,string> message);
    }
}