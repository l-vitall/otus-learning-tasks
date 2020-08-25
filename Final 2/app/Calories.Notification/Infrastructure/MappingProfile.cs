using AutoMapper;
using Calories.Common.Models;
using Calories.Notification.Model;

namespace Calories.Notification.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<NotificationEntity, NotificationMessage>();
        }
    }
}