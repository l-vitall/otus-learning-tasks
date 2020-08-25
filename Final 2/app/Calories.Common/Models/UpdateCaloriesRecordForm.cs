using System;
using System.ComponentModel.DataAnnotations;

namespace Calories.Common.Models
{
    public class UpdateCaloriesRecordForm
    {
        [Required]
        [MaxLength(512)]
        [Display(Name = "text", Description = "Calories entry description")]
        public string Text { get; set; }
        
        [Display(Name = "numberOfCalories", Description = "Number Of Calories")]
        [Range(1, 10000)]
        public int NumberOfCalories { get; set; }
        
        [Display(Name = "id", Description = "Calories record identifier")]
        public Guid Id { get; set; }
    }
}