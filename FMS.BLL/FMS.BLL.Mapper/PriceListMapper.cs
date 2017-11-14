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
    public class PriceListMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<MST_PRICELIST, PriceListDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MstPriceListId , opt => opt.MapFrom(src => src.MST_PRICELIST_ID ))
                .ForMember(dest => dest.Year , opt => opt.MapFrom(src => src.YEAR ))
                .ForMember(dest => dest.Manufacture , opt => opt.MapFrom(src => src.MANUFACTURER ))
                .ForMember(dest => dest.Model , opt => opt.MapFrom(src => src.MODEL ))
                .ForMember(dest => dest.Series, opt => opt.MapFrom(src => src.SERIES))
                .ForMember(dest => dest.ZonePriceList, opt => opt.MapFrom(src => src.ZONE_PRICE_LIST))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.PRICE))
                .ForMember(dest => dest.InstallmenHMS, opt => opt.MapFrom(src => src.INSTALLMEN_HMS))
                .ForMember(dest => dest.InstallmenEMP, opt => opt.MapFrom(src => src.INSTALLMEN_EMP))
                .ForMember(dest => dest.Vendor, opt => opt.MapFrom(src => src.VENDOR))
                .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VEHICLE_TYPE))
                .ForMember(dest => dest.ModifiedBy , opt => opt.MapFrom(src => src.MODIFIED_BY ))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.CreatedDate , opt => opt.MapFrom(src => src.CREATED_DATE  ))
                .ForMember(dest => dest.CreatedBy , opt => opt.MapFrom(src => src.CREATED_BY ))
                .ForMember(dest => dest.IsActive , opt => opt.MapFrom(src => src.IS_ACTIVE ));

            AutoMapper.Mapper.CreateMap<PriceListDto, MST_PRICELIST>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MST_PRICELIST_ID, opt => opt.MapFrom(src => src.MstPriceListId))
                .ForMember(dest => dest.YEAR, opt => opt.MapFrom(src => src.Year))
                .ForMember(dest => dest.MANUFACTURER, opt => opt.MapFrom(src => src.Manufacture))
                .ForMember(dest => dest.MODEL, opt => opt.MapFrom(src => src.Model))
                .ForMember(dest => dest.SERIES, opt => opt.MapFrom(src => src.Series))
                .ForMember(dest => dest.ZONE_PRICE_LIST, opt => opt.MapFrom(src => src.ZonePriceList))
                .ForMember(dest => dest.PRICE, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.INSTALLMEN_HMS, opt => opt.MapFrom(src => src.InstallmenHMS))
                .ForMember(dest => dest.INSTALLMEN_EMP, opt => opt.MapFrom(src => src.InstallmenEMP))
                .ForMember(dest => dest.VENDOR, opt => opt.MapFrom(src => src.Vendor))
                .ForMember(dest => dest.VEHICLE_TYPE, opt => opt.MapFrom(src => src.VehicleType))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));

        }
    }
}
