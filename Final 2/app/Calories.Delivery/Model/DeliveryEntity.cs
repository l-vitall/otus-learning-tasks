using System;
using System.ComponentModel.DataAnnotations;

namespace Calories.Delivery.Model
{
    public enum DeliveryStatus
    {
        Reserved,
        Active,
        Rejected
    }
    public class DeliveryEntity
    {
        [Key]
        public long Id { get; set; }
        public Guid OrderId { get; set; }
        public DeliveryStatus Status { get; set; }
    }
}