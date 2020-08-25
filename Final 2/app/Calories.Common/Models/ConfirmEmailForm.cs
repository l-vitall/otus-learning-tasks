using System.ComponentModel.DataAnnotations;

namespace Calories.Common.Models
{
    public class ConfirmEmailForm : EmailForm
    {
        [Required]
        [MinLength(100)]
        [MaxLength(500)]
        [Display(Name = "confirmationCode", Description = "Confirmation code")]
        public string ConfirmationCode { get; set; }
    }
}