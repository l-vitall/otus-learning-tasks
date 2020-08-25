using System;
using System.ComponentModel.DataAnnotations;

namespace Calories.Order.Model
{
    public class OrderForm
    {
        [Required]
        [Display(Name = "orderId", Description = "Unique identifier for order")]
        public Guid? OrderId { get; set; }
        
        [Required]
        [MaxLength(512)]
        [Display(Name = "orderContent", Description = "Full order content")]
        public string OrderContent { get; set; }
        
        [Required]
        [Display(Name = "restaurantId", Description = "restaurantId")]
        [Range(1, 10000)]
        public long RestaurantId { get; set; }
        
        [Required]
        [Display(Name = "totalCost", Description = "Dishes set total cost")]
        [Range(1, 10000)]
        public int TotalCost { get; set; }
        
        [Required]
        [Display(Name = "creationDate", Description = "Recommendations creation date")]
        public DateTimeOffset CreatedAt { get; set; }
        
        [Required]
        [Display(Name = "caloriesRecommendationId", Description = "Recommendation Id")]
        public long CaloriesRecommendationId { get; set; }
        
        [Required]
        [Display(Name = "deliveryAddress", Description = "Delivery address")]
        public string DeliveryAddress { get; set; }
        
        [Required]
        [Display(Name = "deliveryTime", Description = "Delivery time")]
        public string DeliveryTime{ get; set; }
        
        [Required]
        [Display(Name = "userPhone", Description = "User Phone number")]
        public string UserPhone { get; set; }
    }
}