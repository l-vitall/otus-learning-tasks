using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Calories.Common.Notifications
{
    public class OrderStatusUpdateNotification
    {
        public Guid OrderId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderStatus OldStatus{ get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderStatus NewStatus{ get; set; }
    }
}