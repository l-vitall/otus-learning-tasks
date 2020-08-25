using System.Net;
using System.Threading.Tasks;
using Calories.Common.Models;
using Calories.Notification.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Calories.Notification.Controllers
{
    [Route("/")]
    [ApiController]
    public class RootController : ControllerBase
    {
        private readonly IEmailSendingService _emailService;
        private readonly EmailSendingSettings _emailSendingSettings;
        
        public RootController(IOptions<EmailSendingSettings> settingsWrapper, IEmailSendingService emailService)
        {
            _emailService = emailService;
            _emailSendingSettings = settingsWrapper.Value;
        }
        
        [HttpGet("versionNotification", Name = nameof(VersionNotification))]
        public string VersionNotification()
        {
            return "0.0.1";
        }
        
        [HttpGet("configNotification", Name = nameof(ConfigNotification))]
        [ResponseCache(Duration = 86400)]
        public JsonResult ConfigNotification()
        {
            return new JsonResult(_emailSendingSettings);
        }

        [HttpGet("healthNotification", Name = nameof(HealthNotification))]
        public string HealthNotification()
        {
            return "{\"status\": \"OK\"}";
        }
    }
}