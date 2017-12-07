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
    public class LocationMapingMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<MST_LOCATION_MAPPING, LocationMappingDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MstLocationMappingId, opt => opt.MapFrom(src => src.MST_LOCATION_MAPPING_ID))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.LOCATION))
                .ForMember(dest => dest.Basetown, opt => opt.MapFrom(src => src.BASETOWN))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.ADDRESS))
                .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.REGION))
                .ForMember(dest => dest.ZoneSales, opt => opt.MapFrom(src => src.ZONE_SALES))
                .ForMember(dest => dest.ZonePriceList, opt => opt.MapFrom(src => src.ZONE_PRICE_LIST))
                .ForMember(dest => dest.ValidFrom, opt => opt.MapFrom(src => src.VALIDITY_FROM))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE));
               

            AutoMapper.Mapper.CreateMap<LocationMappingDto, MST_LOCATION_MAPPING>().IgnoreAllNonExisting()
                  .ForMember(dest => dest.MST_LOCATION_MAPPING_ID, opt => opt.MapFrom(src => src.MstLocationMappingId))
                .ForMember(dest => dest.LOCATION, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.BASETOWN, opt => opt.MapFrom(src => src.Basetown))
                .ForMember(dest => dest.ADDRESS, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.REGION, opt => opt.MapFrom(src => src.Region))
                .ForMember(dest => dest.ZONE_SALES, opt => opt.MapFrom(src => src.ZoneSales))
                .ForMember(dest => dest.ZONE_PRICE_LIST, opt => opt.MapFrom(src => src.ZonePriceList))
                .ForMember(dest => dest.VALIDITY_FROM, opt => opt.MapFrom(src => src.ValidFrom))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));
         
        }
    }
}
