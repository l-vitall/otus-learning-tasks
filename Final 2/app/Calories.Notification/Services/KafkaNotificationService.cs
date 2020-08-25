using Calories.Common;
using Calories.Common.Kafka;
using Calories.Common.Models;
using Calories.Common.Notifications;
using Calories.Common.Services;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace Calories.Notification.Services
{
    public class KafkaNotificationService : KafkaConsumerServiceBase
    {
        private readonly IEmailSendingService _emailSendingService;
        private readonly INotificationService _notificationService;

        public KafkaNotificationService(ConsumerConfig consumerConfig
            , IEmailSendingService emailService
            , INotificationService notificationService) : base(consumerConfig, MessageTopic.Notification)
        {  
            _emailSendingService = emailService;
            _notificationService = notificationService;
        } 

        protected override void OnMessageReceive(Message<string, string> message)
        {
            if (message.Key == MessageKind.ConfirmationEmailRequest)
            {
                var form = JsonConvert.DeserializeObject<ConfirmEmailNotification>(message.Value);
                Log.Information("Sending confirmation email..");
                var result = _emailSendingService.SendConfirmationEmailAsync(form.Email, form.Link).GetAwaiter().GetResult();
                Logger.Information("Confirmation email is sent with result = " + result.StatusCode);
            }
            else if (message.Key == MessageKind.NotificationCreated)
            {
                var notification =JsonConvert.DeserializeObject<NotificationMessage>(message.Value);
                _notificationService.CreateNotification(notification).GetAwaiter().GetResult();
            }
        }
    }
}