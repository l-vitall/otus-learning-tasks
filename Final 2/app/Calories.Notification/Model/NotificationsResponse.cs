using Calories.Common.Models;

namespace Calories.Notification.Model
{
    public class NotificationsResponse : PagedCollection<Common.Models.NotificationMessage>
    {
        public Form NotificationsQuery { get; set; }
    }
}