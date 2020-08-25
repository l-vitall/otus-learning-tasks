using System.ComponentModel.DataAnnotations;

namespace Calories.Common.Models
{
    public class EmailForm
    {
        [Required]
        [Display(Name = "email", Description = "Email address")]
        [EmailAddress]
        public string Email { get; set; }
    }
}