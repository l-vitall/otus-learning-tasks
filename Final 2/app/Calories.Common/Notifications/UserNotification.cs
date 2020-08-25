using System;
using System.Globalization;

namespace Calories.Common.Notifications
{
    public class UserNotification
    {
        public string Email { get; set; }
        public string Text { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}