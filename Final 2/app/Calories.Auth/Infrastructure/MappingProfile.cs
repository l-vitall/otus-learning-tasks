using AutoMapper;
using Calories.Auth.Model;

namespace Calories.Auth.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserEntity, User>();
        }
    }
}