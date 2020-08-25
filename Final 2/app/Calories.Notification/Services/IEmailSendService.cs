using System.Threading.Tasks;
using SendGrid;

namespace Calories.Notification.Services
{
    public interface IEmailSendingService
    {
        Task<Response> SendConfirmationEmailAsync(string targetAddress, string confirmationLink);
        Task<Response> SendInviteEmailAsync(string targetAddress, string submitUrl);
    }
}