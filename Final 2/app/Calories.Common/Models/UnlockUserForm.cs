using System.ComponentModel.DataAnnotations;

namespace Calories.Common.Models
{
    public class UnlockUserForm
    {
        [Required]
        [Display(Name = "email", Description = "Email address")]
        [EmailAddress]
        public string Email { get; set; }
    }
}