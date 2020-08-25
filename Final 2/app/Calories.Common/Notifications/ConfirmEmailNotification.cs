using Calories.Common.Models;

namespace Calories.Common.Notifications
{
    public class ConfirmEmailNotification : NotificationMessage
    {
        public string Link { get; set; }
    }
}