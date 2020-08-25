using System;
using System.ComponentModel.DataAnnotations;

namespace Calories.Records.Models
{
    public class DailyNumberOfCalories
    {
        [Key]
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public int NumberOfCalories { get; set; }
        public int TargetNumberOfCalories { get; set; }
        public string UserEmail { get; set; }
    }
}