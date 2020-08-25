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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Calories.Notification.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationDbContext _dbContext;
        private readonly IConfigurationProvider _mappingConfiguration;

        public NotificationService(NotificationDbContext dbContext
        ,IConfigurationProvider mappingConfiguration)
        {
            _dbContext = dbContext;
            _mappingConfiguration = mappingConfiguration;
        }
        
        public async Task CreateNotification(Common.Models.NotificationMessage notification)
       {
           await _dbContext.Notifications.AddAsync(new NotificationEntity(){Email = notification.Email, Text = notification.Text, CreatedAt = notification.CreatedAt});
           
           var created = await _dbContext.SaveChangesAsync();

           if (created < 1)
               throw new InvalidOperationException("Could not create a notification");
       }
        
        public async Task<PagedResults<Common.Models.NotificationMessage>> GetAllNotifications(string email)
        {
            var records = new PagedResults<Common.Models.NotificationMessage>()
            {
                Items = await _dbContext.Notifications.Where(n => n.Email == email)
                    .ProjectTo<Common.Models.NotificationMessage>(_mappingConfiguration)
                    .ToArrayAsync()
            };

            records.TotalSize = records.Items.Count();

            return records;
        }
    }
}