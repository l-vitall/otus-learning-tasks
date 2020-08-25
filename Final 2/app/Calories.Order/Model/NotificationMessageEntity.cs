using System.ComponentModel.DataAnnotations;
using Calories.Common.Models;

namespace Calories.Order.Model
{
    public class NotificationMessageEntity
    {
        [Key]
        public long Id { get; set; }
        
        public string Topic { get; set; }
        public string Kind { get; set; }
        public string Body { get; set; }
        public bool IsProcessed { get; set; }
    }
}