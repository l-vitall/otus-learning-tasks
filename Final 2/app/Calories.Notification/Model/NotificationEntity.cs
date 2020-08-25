using System;
using System.ComponentModel.DataAnnotations;

namespace Calories.Notification.Model
{
    public class NotificationEntity
    {
        [Key]
        public long Id { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}