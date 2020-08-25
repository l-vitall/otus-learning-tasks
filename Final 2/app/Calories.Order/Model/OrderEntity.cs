using System;
using System.ComponentModel.DataAnnotations;
using Calories.Common;

namespace Calories.Order.Model
{
    public class OrderEntity
    {
        [Key]
        public Guid Id { get; set; }
        
        public long CaloriesRecommendationId { get; set; }
        public long RestaurantId { get; set; }
        public string OrderContent { get; set; }
        public int TotalCost { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryTime { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public OrderStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}