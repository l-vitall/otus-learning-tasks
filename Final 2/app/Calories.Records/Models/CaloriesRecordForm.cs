using System.ComponentModel.DataAnnotations;

namespace Calories.Records.Models
{
    public class CaloriesRecordForm
    {
        [Required]
        [MaxLength(512)]
        [Display(Name = "text", Description = "Calories entry description")]
        public string Text { get; set; }
        
        [Display(Name = "numberOfCalories", Description = "Number Of Calories")]
        [Range(1, 10000)]
        public int? NumberOfCalories { get; set; }
        
        [Display(Name = "email", Description = "Optional target user Email address")]
        [EmailAddress]
        public string Email { get; set; }
    }
}