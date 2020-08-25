using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Calories.Common.Infrastructure;
using Calories.Common.Models;
using Calories.Notification.Data;
using Calories.Notification.Model;
using Calories.Notification.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Calories.Notification.Controllers
{
    [Route("/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IAuthorizationService _authService;
        private readonly INotificationService _notificationService;

        public NotificationsController(//IUserService userService,
            IAuthorizationService authService
            ,INotificationService notificationService)
        {
            _authService = authService;
            _notificationService = notificationService;
        }

        //GET /caloriesRecords
        [HttpGet(Name = nameof(GetAllNotifications))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Collection<NotificationMessage>>> GetAllNotifications([FromBody] EmailForm form)
        {
            var records = await _notificationService.GetAllNotifications(form.Email);
            
            var collection = PagedCollection<NotificationMessage>.Create<NotificationsResponse>(
                Link.ToCollection(nameof(GetAllNotifications)),
                records.Items.ToArray(),
                records.TotalSize,
                new PagingOptions());

            collection.NotificationsQuery = FormMetadata.FromResource<NotificationMessage>(
                Link.ToForm(nameof(GetAllNotifications),
                    null,
                    Link.GetMethod,
                    Form.QueryRelation));
            
            return collection;
        }
        
        /*public async Task CreateNotification(string userEmail, string text)
        {
            await _dbContext.Notifications.AddAsync(new NotificationEntity(){Email = userEmail, Text = text, CreatedAt = DateTimeOffset.UtcNow});
            
            var created = await _dbContext.SaveChangesAsync();

            if (created < 1)
                throw new InvalidOperationException("Could not create a notification");
        }*/
    }
}