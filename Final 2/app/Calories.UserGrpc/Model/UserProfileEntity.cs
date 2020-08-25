using System;
using System.ComponentModel.DataAnnotations;

namespace Calories.UserGrpc.Model
{
    public class UserProfileEntity
    {
        [Key]
        public string Email { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DailyNumberOfCalories { get; set; }
        
        public DateTimeOffset CreatedAt { get; set; }
    }
}