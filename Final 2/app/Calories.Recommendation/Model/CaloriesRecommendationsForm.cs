using System;
using System.ComponentModel.DataAnnotations;

namespace Calories.Recommendation.Model
{
    public class CaloriesRecommendationsForm
    {
        [Required]
        [MaxLength(512)]
        [Display(Name = "text", Description = "Calories entry description")]
        public string Text { get; set; }
        
        [Required]
        [Display(Name = "numberOfCalories", Description = "Number Of Calories")]
        [Range(1, 10000)]
        public int DailyNumberOfCalories { get; set; }
        
        [Required]
        [Display(Name = "totalCost", Description = "Dishes set total cost")]
        [Range(1, 10000)]
        public int TotalCost { get; set; }
        
        [Required]
        [Display(Name = "email", Description = "Optional target user Email address")]
        [EmailAddress]
        public string UserEmail { get; set; }
        
        [Required]
        [Display(Name = "creationDate", Description = "Recommendations creation date")]
        public DateTimeOffset CreatedAt { get; set; }
        
        [Required]
        [Display(Name = "startDate", Description = "Recommendations start date")]
        public DateTimeOffset StartDate { get; set; }
        [Required]
        [Display(Name = "endDate", Description = "Recommendations end date")]
        public DateTimeOffset EndDate { get; set; }
    }
}