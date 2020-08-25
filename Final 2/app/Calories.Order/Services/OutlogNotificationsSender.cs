using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Calories.Common.Kafka;
using Calories.Common.Models;
using Calories.Common.Services;
using Calories.Order.Data;
using Confluent.Kafka;
using Newtonsoft.Json;
using Serilog;

namespace Calories.Order.Services
{
    public class OutlogNotificationsSender : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly OrdersDbContext _context;
        private readonly IKafkaProducerService _service;

        private ILogger _log;
        
        public OutlogNotificationsSender(OrdersDbContext context, IKafkaProducerService service)
        {
            _context = context;
            _service = service;
            
            _log = Log.ForContext(typeof(OutlogNotificationsSender));
        } 
        
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(() => StartProcessing(stoppingToken), stoppingToken);  
            return Task.CompletedTask;  
        }
        
        private void StartProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var message = _context.NotificationsOutlog.OrderBy(notificationMessage => notificationMessage.Id).FirstOrDefault(m => !m.IsProcessed);
                if(message == null)
                    Task.Delay(3000).GetAwaiter().GetResult();
                else
                {
                    try
                    {
                        _service.SendMessage(message.Topic, message.Kind, message.Body);
                        _log.Information("Sent notification from outbox: {@Message}", message);
                        message.IsProcessed = true;
                        _context.SaveChangesAsync().GetAwaiter().GetResult();
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception e)
                    {
                        _log.Error(e, "Failed to process message");
                        Task.Delay(3000).GetAwaiter().GetResult();
                    }
                }
            }
        }
    }
}