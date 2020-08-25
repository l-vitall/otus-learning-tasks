using System.ComponentModel.DataAnnotations;
using Calories.Common.Infrastructure;

namespace Calories.Common.Models
{
    public class PasswordGrantForm
    {
        [Required]
        [Display(Name = "grant_type")]
        public string GrantType { get; set; } = "password";

        [Required]
        [Display(Name = "username", Description = "Email address")]
        public string Username { get; set; }

        [Required, Secret]
        [Display(Name = "password", Description = "Password")]
        public string Password { get; set; }
    }
}