using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Calories.Common;
using Calories.Common.Kafka;
using Calories.Common.Notifications;
using Calories.Common.Services;
using Calories.Delivery.Data;
using Calories.Delivery.Model;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace Calories.Delivery.Services
{
    public class KafkaDeliveryService :  KafkaConsumerServiceBase
    {
        private readonly IKafkaProducerService _service;
        private readonly DeliveryDbContext _context;
        private readonly DeliveryEnvironmentSettings _settings;

        public KafkaDeliveryService(ConsumerConfig consumerConfig
            , IKafkaProducerService service
            , DeliveryDbContext context
            ,DeliveryEnvironmentSettings settings
            ) : base(consumerConfig, MessageTopic.Order)
        {
            _service = service;
            _context = context;
            _settings = settings;
        } 
        
        protected override void OnMessageReceive(Message<string, string> message)
        {
            if (message.Key == MessageKind.OrderStatusUpdate)
            {
                var notification = JsonConvert.DeserializeObject<OrderStatusUpdateNotification>(message.Value);

                if (notification.NewStatus == OrderStatus.Reserved)
                {
                    try
                    {
                        _context.Deliveries.Add(new DeliveryEntity()
                        {
                            OrderId = notification.OrderId,
                            Status = DeliveryStatus.Reserved
                        });

                        _context.SaveChanges();
                        
                        //Note: code below is to test saga failure
                        if(_settings.TestMode)
                            throw new Exception();
                        
                        _service.SendMessage(MessageTopic.Order, MessageKind.OrderStatusUpdate, JsonConvert.SerializeObject(new OrderStatusUpdateNotification()
                        {
                            NewStatus = OrderStatus.DeliveryReserved,
                            OrderId = notification.OrderId
                        })).GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        var delivery = _context.Deliveries.SingleOrDefault(d => d.OrderId == notification.OrderId);

                        if (delivery != null)
                        {
                            delivery.Status = DeliveryStatus.Rejected;
                            _context.SaveChanges();
                        }
                        
                        _service.SendMessage(MessageTopic.Order, MessageKind.OrderStatusUpdate, JsonConvert.SerializeObject(new OrderStatusUpdateNotification()
                        {
                            NewStatus = OrderStatus.DeliveryRejected,
                            OrderId = notification.OrderId
                        })).GetAwaiter().GetResult();

                        throw e;
                    }
                }
                else if (notification.NewStatus == OrderStatus.Paid)
                {
                    var delivery = _context.Deliveries.SingleOrDefault(d => d.OrderId == notification.OrderId);
                    if (delivery != null)
                    {
                        delivery.Status = DeliveryStatus.Active;
                        _context.SaveChanges();
                    }
                }
            }
        }
    }
}