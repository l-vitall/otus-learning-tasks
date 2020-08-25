using Calories.Common;
using Calories.Common.Kafka;
using Calories.Common.Notifications;
using Calories.Common.Services;
using Calories.Restaurant.Model;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace Calories.Restaurant.Service
{
    public class KafkaRestaurantService :  KafkaConsumerServiceBase
    {
        private readonly IRestaurantOrderService _orderService;

        public KafkaRestaurantService(ConsumerConfig consumerConfig
            , IRestaurantOrderService orderService) : base(consumerConfig, MessageTopic.Order)
        {  
            _orderService = orderService;
        } 
        
        protected override void OnMessageReceive(Message<string, string> message)
        {
            if (message.Key == MessageKind.OrderStatusUpdate)
            {
                var notification = JsonConvert.DeserializeObject<OrderStatusUpdateNotification>(message.Value);

                if (notification.NewStatus == OrderStatus.New)
                {
                    _orderService.CreateAcceptedOrder(new RestaurantOrderEntity()
                    {
                        Details = message.Value,
                        Status = RestaurantOrderStatus.Reserved,
                        OrderId = notification.OrderId
                    }).GetAwaiter().GetResult();
                }
                else if (notification.NewStatus == OrderStatus.DeliveryRejected)
                {
                    _orderService.RejectAcceptedOrderStatus(notification.OrderId);
                }
                else if (notification.NewStatus == OrderStatus.Paid)
                {
                    _orderService.ConfirmAcceptedOrder(notification.OrderId);
                }
            }
        }
    }
}