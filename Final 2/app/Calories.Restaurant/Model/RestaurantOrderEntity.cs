using System;
using System.ComponentModel.DataAnnotations;

namespace Calories.Restaurant.Model
{
    public enum RestaurantOrderStatus
    {
        Reserved,
        Active,
        Completed,
        Rejected
    }
    
    public class RestaurantOrderEntity
    {
        [Key]
        public long Id { get; set; }
        public string Details { get; set; }
        
        public Guid  OrderId { get; set; }
        public RestaurantOrderStatus Status { get; set; }
    }
}