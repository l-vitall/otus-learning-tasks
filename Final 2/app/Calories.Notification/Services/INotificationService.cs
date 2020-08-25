using System.Threading.Tasks;
using Calories.Common.Models;

namespace Calories.Notification.Services
{
    public interface INotificationService
    {
        Task<PagedResults<Common.Models.NotificationMessage>> GetAllNotifications(string email);
        Task CreateNotification(Common.Models.NotificationMessage notification);
    }
}