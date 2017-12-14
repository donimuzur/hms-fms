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
    public class RptPOMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<PO_REPORT_DATA, RptPODto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.PoliceNumber, opt => opt.MapFrom(src => src.POLICE_NUMBER))
                .ForMember(dest => dest.SupplyMethod, opt => opt.MapFrom(src => src.SUPPLY_METHOD))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EMPLOYEE_NAME))
                .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.COST_CENTER))
                .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom(src => src.MANUFACTURER))
                .ForMember(dest => dest.Models, opt => opt.MapFrom(src => src.MODEL))
                .ForMember(dest => dest.Series, opt => opt.MapFrom(src => src.SERIES))
                .ForMember(dest => dest.BodyType, opt => opt.MapFrom(src => src.BODY_TYPE))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.COLOR))
                .ForMember(dest => dest.ChasisNumber, opt => opt.MapFrom(src => src.CHASIS_NUMBER))
                .ForMember(dest => dest.EngineNumber, opt => opt.MapFrom(src => src.ENGINE_NUMBER))
                .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VEHICLE_TYPE))
                .ForMember(dest => dest.VehicleUsage, opt => opt.MapFrom(src => src.VEHICLE_USAGE))
                .ForMember(dest => dest.PoNumber, opt => opt.MapFrom(src => src.PO_NUMBER))
                .ForMember(dest => dest.PoLine, opt => opt.MapFrom(src => src.PO_LINE))
                .ForMember(dest => dest.ReportMonth, opt => opt.MapFrom(src => src.REPORT_MONTH))
                .ForMember(dest => dest.ReportYear, opt => opt.MapFrom(src => src.REPORT_YEAR))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.StartContract, opt => opt.MapFrom(src => src.START_CONTRACT))
                .ForMember(dest => dest.EndContract, opt => opt.MapFrom(src => src.END_CONTRACT))
                .ForMember(dest => dest.Vendor, opt => opt.MapFrom(src => src.VENDOR))
                .ForMember(dest => dest.MonthlyInstallment, opt => opt.MapFrom(src => src.MONTHLY_INSTALLMENT))
                .ForMember(dest => dest.Gst, opt => opt.MapFrom(src => src.GST))
                .ForMember(dest => dest.TotMonthInstallment, opt => opt.MapFrom(src => src.TOTAL_MONTHLY_INSTALLMENT))
                .ForMember(dest => dest.MstFleetId, opt => opt.MapFrom(src => src.MST_FLEET_ID))
                ;

            AutoMapper.Mapper.CreateMap<RptPODto, PO_REPORT_DATA>().IgnoreAllNonExisting()
               .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
               .ForMember(dest => dest.POLICE_NUMBER, opt => opt.MapFrom(src => src.PoliceNumber))
               .ForMember(dest => dest.SUPPLY_METHOD, opt => opt.MapFrom(src => src.SupplyMethod))
               .ForMember(dest => dest.EMPLOYEE_NAME, opt => opt.MapFrom(src => src.EmployeeName))
               .ForMember(dest => dest.COST_CENTER, opt => opt.MapFrom(src => src.CostCenter))
               .ForMember(dest => dest.MANUFACTURER, opt => opt.MapFrom(src => src.Manufacturer))
               .ForMember(dest => dest.MODEL, opt => opt.MapFrom(src => src.Models))
               .ForMember(dest => dest.SERIES, opt => opt.MapFrom(src => src.Series))
               .ForMember(dest => dest.BODY_TYPE, opt => opt.MapFrom(src => src.BodyType))
               .ForMember(dest => dest.COLOR, opt => opt.MapFrom(src => src.Color))
               .ForMember(dest => dest.CHASIS_NUMBER, opt => opt.MapFrom(src => src.ChasisNumber))
               .ForMember(dest => dest.ENGINE_NUMBER, opt => opt.MapFrom(src => src.EngineNumber))
               .ForMember(dest => dest.VEHICLE_TYPE, opt => opt.MapFrom(src => src.VehicleType))
               .ForMember(dest => dest.VehicleUsage, opt => opt.MapFrom(src => src.VEHICLE_USAGE))
               .ForMember(dest => dest.PoNumber, opt => opt.MapFrom(src => src.PO_NUMBER))
               .ForMember(dest => dest.PoLine, opt => opt.MapFrom(src => src.PO_LINE))
               .ForMember(dest => dest.ReportMonth, opt => opt.MapFrom(src => src.REPORT_MONTH))
               .ForMember(dest => dest.ReportYear, opt => opt.MapFrom(src => src.REPORT_YEAR))
               .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
               .ForMember(dest => dest.StartContract, opt => opt.MapFrom(src => src.START_CONTRACT))
               .ForMember(dest => dest.EndContract, opt => opt.MapFrom(src => src.END_CONTRACT))
               .ForMember(dest => dest.Vendor, opt => opt.MapFrom(src => src.VENDOR))
               .ForMember(dest => dest.MonthlyInstallment, opt => opt.MapFrom(src => src.MONTHLY_INSTALLMENT))
               .ForMember(dest => dest.Gst, opt => opt.MapFrom(src => src.GST))
               .ForMember(dest => dest.TotMonthInstallment, opt => opt.MapFrom(src => src.TOTAL_MONTHLY_INSTALLMENT))
               .ForMember(dest => dest.MstFleetId, opt => opt.MapFrom(src => src.MST_FLEET_ID))
               ;
        }
    }
}
