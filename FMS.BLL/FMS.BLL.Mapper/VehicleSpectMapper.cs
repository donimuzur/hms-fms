using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;

namespace FMS.BLL.Mapper
{
    public class VehicleSpectMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<MST_VEHICLE_SPECT, VehicleSpectDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MstVehicleSpectId, opt => opt.MapFrom(src => src.MST_VEHICLE_SPECT_ID))
                .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom(src => src.MANUFACTURER))
                .ForMember(dest => dest.Models, opt => opt.MapFrom(src => src.MODEL))
                .ForMember(dest => dest.Series, opt => opt.MapFrom(src => src.SERIES))
                .ForMember(dest => dest.BodyType, opt => opt.MapFrom(src => src.BODY_TYPE))
                .ForMember(dest => dest.FuelTypeSpect, opt => opt.MapFrom(src => src.FUEL_TYPE))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.YEAR))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.COLOUR))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.IMAGE))
                .ForMember(dest => dest.GroupLevel, opt => opt.MapFrom(src => src.GROUP_LEVEL))
                .ForMember(dest => dest.FlexPoint, opt => opt.MapFrom(src => src.FLEX_POINT))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE));

            AutoMapper.Mapper.CreateMap<VehicleSpectDto, MST_VEHICLE_SPECT>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MST_VEHICLE_SPECT_ID, opt => opt.MapFrom(src => src.MstVehicleSpectId))
                .ForMember(dest => dest.MANUFACTURER, opt => opt.MapFrom(src => src.Manufacturer))
                .ForMember(dest => dest.MODEL, opt => opt.MapFrom(src => src.Models))
                .ForMember(dest => dest.SERIES, opt => opt.MapFrom(src => src.Series))
                .ForMember(dest => dest.BODY_TYPE, opt => opt.MapFrom(src => src.BodyType))
                .ForMember(dest => dest.FUEL_TYPE, opt => opt.MapFrom(src => src.FuelTypeSpect))
                .ForMember(dest => dest.YEAR, opt => opt.MapFrom(src => src.Year))
                .ForMember(dest => dest.COLOUR, opt => opt.MapFrom(src => src.Color))
                .ForMember(dest => dest.IMAGE, opt => opt.MapFrom(src => src.Image))
                .ForMember(dest => dest.GROUP_LEVEL, opt => opt.MapFrom(src => src.GroupLevel))
                .ForMember(dest => dest.FLEX_POINT, opt => opt.MapFrom(src => src.FlexPoint))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));

            AutoMapper.Mapper.CreateMap<ARCH_MST_VEHICLE_SPECT, VehicleSpectDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MstVehicleSpectId, opt => opt.MapFrom(src => src.MST_VEHICLE_SPECT_ID))
                .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom(src => src.MANUFACTURER))
                .ForMember(dest => dest.Models, opt => opt.MapFrom(src => src.MODEL))
                .ForMember(dest => dest.Series, opt => opt.MapFrom(src => src.SERIES))
                .ForMember(dest => dest.BodyType, opt => opt.MapFrom(src => src.BODY_TYPE))
                .ForMember(dest => dest.FuelTypeSpect, opt => opt.MapFrom(src => src.FUEL_TYPE))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.YEAR))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.COLOUR))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.IMAGE))
                .ForMember(dest => dest.GroupLevel, opt => opt.MapFrom(src => src.GROUP_LEVEL))
                .ForMember(dest => dest.FlexPoint, opt => opt.MapFrom(src => src.FLEX_POINT))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE));

            AutoMapper.Mapper.CreateMap<VehicleSpectDto, ARCH_MST_VEHICLE_SPECT>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.MST_VEHICLE_SPECT_ID, opt => opt.MapFrom(src => src.MstVehicleSpectId))
                 .ForMember(dest => dest.MANUFACTURER, opt => opt.MapFrom(src => src.Manufacturer))
                 .ForMember(dest => dest.MODEL, opt => opt.MapFrom(src => src.Models))
                 .ForMember(dest => dest.SERIES, opt => opt.MapFrom(src => src.Series))
                 .ForMember(dest => dest.BODY_TYPE, opt => opt.MapFrom(src => src.BodyType))
                 .ForMember(dest => dest.FUEL_TYPE, opt => opt.MapFrom(src => src.FuelTypeSpect))
                 .ForMember(dest => dest.YEAR, opt => opt.MapFrom(src => src.Year))
                 .ForMember(dest => dest.COLOUR, opt => opt.MapFrom(src => src.Color))
                 .ForMember(dest => dest.IMAGE, opt => opt.MapFrom(src => src.Image))
                 .ForMember(dest => dest.GROUP_LEVEL, opt => opt.MapFrom(src => src.GroupLevel))
                 .ForMember(dest => dest.FLEX_POINT, opt => opt.MapFrom(src => src.FlexPoint))
                 .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                 .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                 .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                 .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                 .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));
        }
    }
}

