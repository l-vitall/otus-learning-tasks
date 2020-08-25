using System.ComponentModel.DataAnnotations;
using Calories.Common.Infrastructure;

namespace Calories.Common.Models
{
    public class RegisterForm
    {
        [Required]
        [Display(Name = "email", Description = "Email address")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        [Display(Name = "password", Description = "Password")]
        [Secret]
        public string Password { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        [Display(Name = "firstName", Description = "First name")]
        public string FirstName { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        [Display(Name = "lastName", Description = "Last name")]
        public string LastName { get; set; }

        [Required]
        [Range(1, 10000)]
        [Display(Name = "dailyNumberOfCalories", Description = "Expected number of calories per day")]
        public int DailyNumberOfCalories { get; set; }
    }

}