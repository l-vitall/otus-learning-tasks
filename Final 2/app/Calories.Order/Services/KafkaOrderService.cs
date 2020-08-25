using Calories.Common;
using Calories.Common.Kafka;
using Calories.Common.Notifications;
using Calories.Common.Services;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace Calories.Order.Services
{
    public class KafkaOrderService : KafkaConsumerServiceBase
    {
        private readonly IOrderService _service;

        public KafkaOrderService(ConsumerConfig consumerConfig, IOrderService service) : base(consumerConfig, MessageTopic.Order)
        {  
            _service = service;
        } 

        protected override void OnMessageReceive(Message<string, string> message)
        {
            if (message.Key == MessageKind.OrderStatusUpdate && !string.IsNullOrEmpty(message.Value))
            {
                var notification = JsonConvert.DeserializeObject<OrderStatusUpdateNotification>(message.Value);

                if (notification.NewStatus == OrderStatus.New)
                    return;
                
                _service.UpdateOrderStatus(notification.OrderId, notification.NewStatus).GetAwaiter().GetResult();
            }
        }
    }
}