using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FMS.BLL.Mapper
{
    public class PenaltyMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<MST_PENALTY, PenaltyDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MstPenaltyId, opt => opt.MapFrom(src => src.MST_PENALTY_ID))
                .ForMember(dest => dest.Vendor, opt => opt.MapFrom(src => src.VENDOR))
                .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom(src => src.MANUFACTURER))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.MODEL))
                .ForMember(dest => dest.Series, opt => opt.MapFrom(src => src.SERIES))
                .ForMember(dest => dest.BodyType, opt => opt.MapFrom(src => src.BODY_TYPE))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.YEAR))
                .ForMember(dest => dest.MonthStart, opt => opt.MapFrom(src => src.MONTH_START))
                .ForMember(dest => dest.MonthEnd, opt => opt.MapFrom(src => src.MONTH_END))
                .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VEHICLE_TYPE))
                .ForMember(dest => dest.Penalty, opt => opt.MapFrom(src => src.PENALTY))
                .ForMember(dest => dest.Restitution, opt => opt.MapFrom(src => src.RESTITUTION))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE));

            AutoMapper.Mapper.CreateMap<PenaltyDto, MST_PENALTY>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MST_PENALTY_ID, opt => opt.MapFrom(src => src.MstPenaltyId))
                .ForMember(dest => dest.MANUFACTURER, opt => opt.MapFrom(src => src.Manufacturer))
                .ForMember(dest => dest.MODEL, opt => opt.MapFrom(src => src.Model))
                .ForMember(dest => dest.SERIES, opt => opt.MapFrom(src => src.Series))
                .ForMember(dest => dest.BODY_TYPE, opt => opt.MapFrom(src => src.BodyType))
                .ForMember(dest => dest.YEAR, opt => opt.MapFrom(src => src.Year))
                .ForMember(dest => dest.MONTH_START, opt => opt.MapFrom(src => src.MonthStart))
                .ForMember(dest => dest.MONTH_END, opt => opt.MapFrom(src => src.MonthEnd))
                .ForMember(dest => dest.VEHICLE_TYPE, opt => opt.MapFrom(src => src.VehicleType))
                .ForMember(dest => dest.PENALTY, opt => opt.MapFrom(src => src.Penalty))
                .ForMember(dest => dest.RESTITUTION, opt => opt.MapFrom(src => src.Restitution))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.VENDOR, opt => opt.MapFrom(src => src.Vendor))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));
        }
    }
}
