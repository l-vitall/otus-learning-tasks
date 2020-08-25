using System.ComponentModel.DataAnnotations;

namespace Calories.Common.Models
{
    public class SendConfirmationEmailForm
    {
        [Required]
        [Display(Name = "email", Description = "Email address")]
        [EmailAddress]
        public string Email { get; set; }
        
        public string ConfirmationLink { get; set; }
    }
}