using System;

namespace Calories.Common.Models
{
    public class NotificationMessage
    {
        public string Email { get; set; }
        public string Text { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}