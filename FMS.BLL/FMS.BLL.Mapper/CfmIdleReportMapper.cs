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
    public class CfmIdleReportMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<CFM_IDLE_REPORT_DATA, CfmIdleReportDto>()
            .ForMember(dest => dest.CfmIdleId, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.PoliceNumber, opt => opt.MapFrom(src => src.POLICE_NUMBER))
            .ForMember(dest => dest.Manufacture, opt => opt.MapFrom(src => src.MANUFACTURER))
            .ForMember(dest => dest.Models, opt => opt.MapFrom(src => src.MODEL))
            .ForMember(dest => dest.Series, opt => opt.MapFrom(src => src.SERIES))
            .ForMember(dest => dest.BodyType, opt => opt.MapFrom(src => src.BODY_TYPE))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.COLOR))
            .ForMember(dest => dest.GroupLevel, opt => opt.MapFrom(src => src.GROUP_LEVEL))
            .ForMember(dest => dest.StartContract, opt => opt.MapFrom(src => src.START_CONTRACT))
            .ForMember(dest => dest.EndContract, opt => opt.MapFrom(src => src.END_CONTRACT))
            .ForMember(dest => dest.SupplyMethod, opt => opt.MapFrom(src => src.SUPPLY_METHOD))
            .ForMember(dest => dest.Vendor, opt => opt.MapFrom(src => src.VENDOR))
            .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.COST_CENTER))
            .ForMember(dest => dest.Transmission, opt => opt.MapFrom(src => src.TRANSMISSION))
            .ForMember(dest => dest.FuelType, opt => opt.MapFrom(src => src.FUEL_TYPE))
            .ForMember(dest => dest.StartIdle, opt => opt.MapFrom(src => src.START_IDLE))
            .ForMember(dest => dest.EndIdle, opt => opt.MapFrom(src => src.END_IDLE))
            .ForMember(dest => dest.MonthlyInstallment, opt => opt.MapFrom(src => src.MONTHLY_INSTALLMENT))
            .ForMember(dest => dest.ReportMonth, opt => opt.MapFrom(src => src.REPORT_MONTH))
            .ForMember(dest => dest.ReportYear, opt => opt.MapFrom(src => src.REPORT_YEAR))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE));

            AutoMapper.Mapper.CreateMap<CfmIdleReportDto, CFM_IDLE_REPORT_DATA >()
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.CfmIdleId))
            .ForMember(dest => dest.POLICE_NUMBER, opt => opt.MapFrom(src => src.PoliceNumber))
            .ForMember(dest => dest.MANUFACTURER, opt => opt.MapFrom(src => src.Manufacture))
            .ForMember(dest => dest.MODEL, opt => opt.MapFrom(src => src.Models))
            .ForMember(dest => dest.SERIES, opt => opt.MapFrom(src => src.Series))
            .ForMember(dest => dest.BODY_TYPE, opt => opt.MapFrom(src => src.BodyType))
            .ForMember(dest => dest.COLOR, opt => opt.MapFrom(src => src.Color))
            .ForMember(dest => dest.GROUP_LEVEL, opt => opt.MapFrom(src => src.GroupLevel))
            .ForMember(dest => dest.START_CONTRACT, opt => opt.MapFrom(src => src.StartContract))
            .ForMember(dest => dest.END_CONTRACT, opt => opt.MapFrom(src => src.EndContract))
            .ForMember(dest => dest.SUPPLY_METHOD, opt => opt.MapFrom(src => src.SupplyMethod))
            .ForMember(dest => dest.VENDOR, opt => opt.MapFrom(src => src.Vendor))
            .ForMember(dest => dest.COST_CENTER, opt => opt.MapFrom(src => src.CostCenter))
            .ForMember(dest => dest.TRANSMISSION, opt => opt.MapFrom(src => src.Transmission))
            .ForMember(dest => dest.FUEL_TYPE, opt => opt.MapFrom(src => src.FuelType))
            .ForMember(dest => dest.START_IDLE, opt => opt.MapFrom(src => src.StartIdle))
            .ForMember(dest => dest.END_IDLE, opt => opt.MapFrom(src => src.EndIdle))
            .ForMember(dest => dest.MONTHLY_INSTALLMENT, opt => opt.MapFrom(src => src.MonthlyInstallment))
            .ForMember(dest => dest.REPORT_MONTH, opt => opt.MapFrom(src => src.ReportMonth))
            .ForMember(dest => dest.REPORT_YEAR, opt => opt.MapFrom(src => src.ReportYear))
            .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate));
        }
    }
}
