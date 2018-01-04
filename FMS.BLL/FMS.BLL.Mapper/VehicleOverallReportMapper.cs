﻿using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BLL.Mapper
{
    public class VehicleOverallReportMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<VEHICLE_REPORT_DATA, VehicleOverallReportDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.PoliceNumber, opt => opt.MapFrom(src => src.POLICE_NUMBER))
            .ForMember(dest => dest.Manufacture, opt => opt.MapFrom(src => src.MANUFACTURER))
            .ForMember(dest => dest.Models, opt => opt.MapFrom(src => src.MODEL))
            .ForMember(dest => dest.Series, opt => opt.MapFrom(src => src.SERIES))
            .ForMember(dest => dest.BodyType, opt => opt.MapFrom(src => src.BODY_TYPE))
            .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EMPLOYEE_ID))
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EMPLOYEE_NAME))
            .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VEHICLE_TYPE))
            .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.COST_CENTER))
            .ForMember(dest => dest.StartContract, opt => opt.MapFrom(src => src.START_RENT))
            .ForMember(dest => dest.EndContract, opt => opt.MapFrom(src => src.END_RENT))
            .ForMember(dest => dest.SupplyMethod, opt => opt.MapFrom(src => src.SUPPLY_METHOD))
            .ForMember(dest => dest.Vendor, opt => opt.MapFrom(src => src.VENDOR))
            .ForMember(dest => dest.Function, opt => opt.MapFrom(src => src.FUNCTION))
            .ForMember(dest => dest.Regional, opt => opt.MapFrom(src => src.REGIONAL))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.CITY))
            .ForMember(dest => dest.Transmission, opt => opt.MapFrom(src => src.TRANSMISSION))
            .ForMember(dest => dest.FuelType, opt => opt.MapFrom(src => src.FUEL_TYPE))
            .ForMember(dest => dest.Branding, opt => opt.MapFrom(src => src.BRANDING))
            .ForMember(dest => dest.Colour, opt => opt.MapFrom(src => src.COLOR))
            .ForMember(dest => dest.Airbag, opt => opt.MapFrom(src => src.AIRBAG))
            .ForMember(dest => dest.Abs, opt => opt.MapFrom(src => src.ABS))
            .ForMember(dest => dest.ChasisNumber, opt => opt.MapFrom(src => src.CHASIS_NUMBER))
            .ForMember(dest => dest.EngineNumber, opt => opt.MapFrom(src => src.ENGINE_NUMBER))
            .ForMember(dest => dest.VehicleStatus, opt => opt.MapFrom(src => src.VEHICLE_STATUS))
            .ForMember(dest => dest.AssetsNumber, opt => opt.MapFrom(src => src.ASSET_NUMBER))
            .ForMember(dest => dest.TerminationDate, opt => opt.MapFrom(src => src.TERMINATION_DATE))
            .ForMember(dest => dest.Restitution, opt => opt.MapFrom(src => src.RESTITUTION))
            .ForMember(dest => dest.MonthlyInstallment, opt => opt.MapFrom(src => src.MONTHLY_INSTALLMENT))
            .ForMember(dest => dest.Vat, opt => opt.MapFrom(src => src.VAT))
            .ForMember(dest => dest.TotalMonthlyInstallment, opt => opt.MapFrom(src => src.TOTAL_MONTHLY_CHARGE))
            .ForMember(dest => dest.PoNumber, opt => opt.MapFrom(src => src.PO_NUMBER))
            .ForMember(dest => dest.PoLine, opt => opt.MapFrom(src => src.PO_LINE))
            .ForMember(dest => dest.ReportMonth, opt => opt.MapFrom(src => src.REPORT_MONTH))
            .ForMember(dest => dest.ReportYear, opt => opt.MapFrom(src => src.REPORT_YEAR))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
            .ForMember(dest => dest.MstFleetId, opt => opt.MapFrom(src => src.MST_FLEET_ID));

            AutoMapper.Mapper.CreateMap<VehicleOverallReportDto, VEHICLE_REPORT_DATA >()
              .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.POLICE_NUMBER, opt => opt.MapFrom(src => src.PoliceNumber))
            .ForMember(dest => dest.MANUFACTURER, opt => opt.MapFrom(src => src.Manufacture))
            .ForMember(dest => dest.MODEL, opt => opt.MapFrom(src => src.Models))
            .ForMember(dest => dest.SERIES, opt => opt.MapFrom(src => src.Series))
            .ForMember(dest => dest.BODY_TYPE, opt => opt.MapFrom(src => src.BodyType))
            .ForMember(dest => dest.EMPLOYEE_ID, opt => opt.MapFrom(src => src.EmployeeId))
            .ForMember(dest => dest.EMPLOYEE_NAME, opt => opt.MapFrom(src => src.EmployeeName))
            .ForMember(dest => dest.VEHICLE_TYPE, opt => opt.MapFrom(src => src.VehicleType))
            .ForMember(dest => dest.COST_CENTER, opt => opt.MapFrom(src => src.CostCenter))
            .ForMember(dest => dest.START_RENT, opt => opt.MapFrom(src => src.StartContract))
            .ForMember(dest => dest.END_RENT, opt => opt.MapFrom(src => src.EndContract))
            .ForMember(dest => dest.SUPPLY_METHOD, opt => opt.MapFrom(src => src.SupplyMethod))
            .ForMember(dest => dest.VENDOR, opt => opt.MapFrom(src => src.Vendor))
            .ForMember(dest => dest.FUNCTION, opt => opt.MapFrom(src => src.Function))
            .ForMember(dest => dest.REGIONAL, opt => opt.MapFrom(src => src.Regional))
            .ForMember(dest => dest.CITY, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.TRANSMISSION, opt => opt.MapFrom(src => src.Transmission))
            .ForMember(dest => dest.FUEL_TYPE, opt => opt.MapFrom(src => src.FuelType))
            .ForMember(dest => dest.BRANDING, opt => opt.MapFrom(src => src.Branding))
            .ForMember(dest => dest.COLOR, opt => opt.MapFrom(src => src.Colour))
            .ForMember(dest => dest.AIRBAG, opt => opt.MapFrom(src => src.Airbag))
            .ForMember(dest => dest.ABS, opt => opt.MapFrom(src => src.Abs))
            .ForMember(dest => dest.CHASIS_NUMBER, opt => opt.MapFrom(src => src.ChasisNumber))
            .ForMember(dest => dest.ENGINE_NUMBER, opt => opt.MapFrom(src => src.EngineNumber))
            .ForMember(dest => dest.VEHICLE_STATUS, opt => opt.MapFrom(src => src.VehicleStatus))
            .ForMember(dest => dest.ASSET_NUMBER, opt => opt.MapFrom(src => src.AssetsNumber))
            .ForMember(dest => dest.TERMINATION_DATE, opt => opt.MapFrom(src => src.TerminationDate))
            .ForMember(dest => dest.RESTITUTION, opt => opt.MapFrom(src => src.Restitution))
            .ForMember(dest => dest.MONTHLY_INSTALLMENT, opt => opt.MapFrom(src => src.MonthlyInstallment))
            .ForMember(dest => dest.VAT, opt => opt.MapFrom(src => src.Vat))
            .ForMember(dest => dest.TOTAL_MONTHLY_CHARGE, opt => opt.MapFrom(src => src.TotalMonthlyInstallment))
            .ForMember(dest => dest.PO_NUMBER, opt => opt.MapFrom(src => src.PoNumber))
            .ForMember(dest => dest.PO_LINE, opt => opt.MapFrom(src => src.PoLine))
            .ForMember(dest => dest.REPORT_MONTH, opt => opt.MapFrom(src => src.ReportMonth))
            .ForMember(dest => dest.REPORT_YEAR, opt => opt.MapFrom(src => src.ReportYear))
            .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.MST_FLEET_ID, opt => opt.MapFrom(src => src.MstFleetId));
        }
    }
}
