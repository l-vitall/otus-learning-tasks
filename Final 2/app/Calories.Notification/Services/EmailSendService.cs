using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Calories.Notification.Services
{
    public class EmailSendingService : IEmailSendingService
    {
        private readonly EmailSendingSettings _settings;
        private SendGridClient _client;
        
        public EmailSendingService(IOptions<EmailSendingSettings> settings)
        {
            _settings = settings.Value;
            _client = new SendGridClient(settings.Value.ApiKey);
        }

        public async Task<Response> SendConfirmationEmailAsync(string targetAddress, string confirmationLink)
        {
                var from = new EmailAddress(_settings.ConfirmationFrom, "CaloriesApi Team");
                var to = new EmailAddress(targetAddress);

                var content = _settings.ConfirmationContent.Replace("{callbackUrl}", confirmationLink);
                
                var msg = MailHelper.CreateSingleEmail(from, to, _settings.ConfirmationSubject, null, content);
                return await _client.SendEmailAsync(msg);
        }

        public async Task<Response> SendInviteEmailAsync(string targetAddress, string submitUrl)
        {
            var from = new EmailAddress(_settings.InvitationFrom, "CaloriesApi Team");
            var to = new EmailAddress(targetAddress);

            var htmlContent = _settings.InviteUserHtmlTemplate.Replace("{actionLink}", submitUrl).Replace("{email}", targetAddress);
            
            var msg = MailHelper.CreateSingleEmail(from, to, _settings.InvitationSubject, null, htmlContent);
            return await _client.SendEmailAsync(msg);
        }
    }
}