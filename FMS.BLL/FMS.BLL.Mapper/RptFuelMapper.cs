using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;

namespace FMS.BLL.Mapper
{
    public class RptFuelMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<FUEL_REPORT_DATA, RptFuelDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.PoliceNumber, opt => opt.MapFrom(src => src.POLICE_NUMBER))
                .ForMember(dest => dest.Liter, opt => opt.MapFrom(src => src.LITER))
                .ForMember(dest => dest.Odometer, opt => opt.MapFrom(src => src.ODOMETER))
                .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.COST))
                .ForMember(dest => dest.FuelType, opt => opt.MapFrom(src => src.FUEL_TYPE))
                .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.COST_CENTER))
                .ForMember(dest => dest.Function, opt => opt.MapFrom(src => src.FUNCTION))
                .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom(src => src.MANUFACTURER))
                .ForMember(dest => dest.Models, opt => opt.MapFrom(src => src.MODEL))
                .ForMember(dest => dest.Series, opt => opt.MapFrom(src => src.SERIES))
                .ForMember(dest => dest.BodyType, opt => opt.MapFrom(src => src.BODY_TYPE))
                .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VEHICLE_TYPE))
                .ForMember(dest => dest.VehicleUsage, opt => opt.MapFrom(src => src.VEHICLE_USAGE))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.LOCATION))
                .ForMember(dest => dest.ReportMonth, opt => opt.MapFrom(src => src.REPORT_MONTH))
                .ForMember(dest => dest.ReportYear, opt => opt.MapFrom(src => src.REPORT_YEAR))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                ;
        }
    }
}
