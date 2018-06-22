using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;

namespace FMS.BLL.Mapper
{
    public class HolidayCalenderMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<MST_HOLIDAY_CALENDAR, HolidayCalenderDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MstHolidayDate, opt => opt.MapFrom(src => src.MST_HOLIDAY_DATE))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.DESCRIPTION))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE));

            AutoMapper.Mapper.CreateMap<HolidayCalenderDto, MST_HOLIDAY_CALENDAR>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MST_HOLIDAY_DATE, opt => opt.MapFrom(src => src.MstHolidayDate))
                .ForMember(dest => dest.DESCRIPTION, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));

            AutoMapper.Mapper.CreateMap<ARCH_MST_HOLIDAY_CALENDAR, HolidayCalenderDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MstHolidayDate, opt => opt.MapFrom(src => src.MST_HOLIDAY_DATE))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.DESCRIPTION))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE));

            AutoMapper.Mapper.CreateMap<HolidayCalenderDto, ARCH_MST_HOLIDAY_CALENDAR>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MST_HOLIDAY_DATE, opt => opt.MapFrom(src => src.MstHolidayDate))
                .ForMember(dest => dest.DESCRIPTION, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));
        }
    }
}
