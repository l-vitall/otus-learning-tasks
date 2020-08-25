using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Calories.Records.Models
{
    public class CaloriesRecordEntity
    {
	    [Key]
	    public long Id { get; set; }
	    public DateTime Date { get; set; }
	    
	    public string UserEmail { get; set; }
	    public TimeSpan Time { get; set; }
	    
	    public Guid ExternalId { get; set; }
	    
	    public string Text { get; set; }
	    public int NumberOfCalories { get; set; }
	    
	    [IgnoreDataMember]
	    public bool DailyCaloriesLessThanExpected { get; set; }
    }
}