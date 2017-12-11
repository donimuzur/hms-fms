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

            AutoMapper.Mapper.CreateMap<CFM_IDLE_REPORT_DATA, CfmIdleReportDto>().IgnoreAllNonExisting();
        }
    }
}
