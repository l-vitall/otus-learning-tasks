using System;
using Calories.Common.Infrastructure;
using Calories.Common.Models;

namespace Calories.Records.Models
{
    public class CaloriesRecord : Resource
    {
        public Guid Id { get; set; }
        
        [Sortable(Default = true)]
        [Searchable]
        public string Date { get; set; }

        [Searchable]
        public string Time { get; set; }
        
        [Sortable]
        [Searchable]
        public string Text { get; set; }
        
        [Sortable]
        [Searchable]
        public string UserEmail { get; set; }
        
        [Sortable]
        [Searchable]
        public int NumberOfCalories { get; set; }
        
        public bool DailyCaloriesLessThanExpected { get; set; }
    }
}