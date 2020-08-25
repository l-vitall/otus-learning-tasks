using AutoMapper;
using Calories.Common.Models;
using Calories.Records.Controllers;
using Calories.Records.Models;

namespace Calories.Records.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CaloriesRecordEntity, CaloriesRecord>()
                .ForMember(dest => dest.Self, opt => opt.MapFrom(src =>
                    Link.To(
                        nameof(CaloriesRecordsController.GetCaloriesRecordById),
                        new {caloriesRecordId = src.Id})))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src =>
                    src.Date.ToString("yyy-MM-dd")))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src =>
                    src.Time.ToString()))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src =>
                    src.ExternalId));
        }
    }
}