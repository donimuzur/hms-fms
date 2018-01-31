using AutoMapper;
using FMS.AutoMapperExtensions;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FMS.Website.Code
{
    public partial class FMSWebsiteMapper
    {
        public static void InitializeVehicleOverallReport()
        {
            Mapper.CreateMap<VehicleOverallReportDto, VehicleOverallItem>().IgnoreAllNonExisting();
            Mapper.CreateMap<VehicleOverallItem, VehicleOverallReportDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<VehicleOverallItem, FleetDto>().IgnoreAllNonExisting()
            .ForMember(dest => dest.Airbag, opt => opt.MapFrom(src => src.Airbag))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Assets, opt => opt.MapFrom(src => src.AssetsNumber))
            .ForMember(dest => dest.AssignedTo, opt => opt.MapFrom(src => src.AssignedTo))
            .ForMember(dest => dest.BodyType, opt => opt.MapFrom(src => src.BodyType))
            .ForMember(dest => dest.Branding, opt => opt.MapFrom(src => src.Branding))
            .ForMember(dest => dest.VatDecimal, opt => opt.MapFrom(src => src.Vat))
            .ForMember(dest => dest.CarGroupLevel, opt => opt.MapFrom(src => src.carGrouplevel))
            .ForMember(dest => dest.CertificateOwnership, opt => opt.MapFrom(src => src.CertificateOfOwnership))
            .ForMember(dest => dest.ChasisNumber, opt => opt.MapFrom(src => src.ChasisNumber))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Colour))
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
            .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.CostCenter))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.EmployeeID, opt => opt.MapFrom(src => src.EmployeeId))
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EmployeeName))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
            .ForMember(dest => dest.EngineNumber, opt => opt.MapFrom(src => src.EngineNumber))
            .ForMember(dest => dest.FuelType, opt => opt.MapFrom(src => src.FuelType))
            .ForMember(dest => dest.Function, opt => opt.MapFrom(src => src.Function))
            .ForMember(dest => dest.GroupLevel, opt => opt.MapFrom(src => src.EmployeeGroupLevel))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.VehicleStatus))
            .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType))
            .ForMember(dest => dest.VehicleUsage, opt => opt.MapFrom(src => src.VehicleUsage))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.VehicleYear, opt => opt.MapFrom(src => src.VehicleYear))
            .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.Vendor))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.TerminationDate))
            .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom(src => src.Manufacture))
            .ForMember(dest => dest.Models, opt => opt.MapFrom(src => src.Models))
            .ForMember(dest => dest.Series, opt => opt.MapFrom(src => src.Series))
            .ForMember(dest => dest.TotalMonthlyCharge, opt => opt.MapFrom(src => src.TotalMonthlyInstallment)); 

            Mapper.CreateMap<FleetDto, VehicleOverallItem>().IgnoreAllNonExisting()
            .ForMember(dest => dest.TotalMonthlyInstallment, opt => opt.MapFrom(src => src.TotalMonthlyCharge))
            .ForMember(dest => dest.MonthlyInstallment, opt => opt.MapFrom(src => src.MonthlyHMSInstallment))
            .ForMember(dest => dest.MstFleetId, opt => opt.MapFrom(src => src.MstFleetId))
            .ForMember(dest => dest.PoLine, opt => opt.MapFrom(src => src.PoLine))
            .ForMember(dest => dest.PoNumber, opt => opt.MapFrom(src => src.PoNumber))
            .ForMember(dest => dest.Regional, opt => opt.MapFrom(src => src.Regional))
            .ForMember(dest => dest.Restitution, opt => opt.MapFrom(src => src.Restitution))
            .ForMember(dest => dest.StartContract, opt => opt.MapFrom(src => src.StartContract))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.SupplyMethod, opt => opt.MapFrom(src => src.SupplyMethod))
            .ForMember(dest => dest.Manufacture, opt => opt.MapFrom(src => src.Manufacturer))
            .ForMember(dest => dest.EndContract, opt => opt.MapFrom(src => src.EndContract))
            .ForMember(dest => dest.Airbag, opt => opt.MapFrom(src => src.Airbag))
            .ForMember(dest => dest.Vat, opt => opt.MapFrom(src => src.VatDecimal))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.AssetsNumber, opt => opt.MapFrom(src => src.Assets))
            .ForMember(dest => dest.AssignedTo, opt => opt.MapFrom(src => src.AssignedTo))
            .ForMember(dest => dest.BodyType, opt => opt.MapFrom(src => src.BodyType))
            .ForMember(dest => dest.Branding, opt => opt.MapFrom(src => src.Branding))
            .ForMember(dest => dest.carGrouplevel, opt => opt.MapFrom(src => src.CarGroupLevel))
            .ForMember(dest => dest.CertificateOfOwnership, opt => opt.MapFrom(src => src.CertificateOwnership))
            .ForMember(dest => dest.ChasisNumber, opt => opt.MapFrom(src => src.ChasisNumber))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.Colour, opt => opt.MapFrom(src => src.Color))
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
            .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.CostCenter))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeID))
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EmployeeName))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
            .ForMember(dest => dest.EngineNumber, opt => opt.MapFrom(src => src.EngineNumber))
            .ForMember(dest => dest.FuelType, opt => opt.MapFrom(src => src.FuelType))
            .ForMember(dest => dest.Function, opt => opt.MapFrom(src => src.Function))
            .ForMember(dest => dest.EmployeeGroupLevel, opt => opt.MapFrom(src => src.GroupLevel))
            .ForMember(dest => dest.VehicleStatus, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType))
            .ForMember(dest => dest.VehicleUsage, opt => opt.MapFrom(src => src.VehicleUsage))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.VehicleYear, opt => opt.MapFrom(src => src.VehicleYear))
            .ForMember(dest => dest.Vendor, opt => opt.MapFrom(src => src.VendorName))
            .ForMember(dest => dest.TerminationDate, opt => opt.MapFrom(src => src.EndDate));

            Mapper.CreateMap<VehicleOverallSearchView, VehicleOverallReportGetByParamInput>()
            .ForMember(dest => dest.FromDate, opt => opt.MapFrom(src => src.FromDate))
            .ForMember(dest => dest.ToDate, opt => opt.MapFrom(src => src.ToDate))
            .ForMember(dest => dest.BodyType, opt => opt.MapFrom(src => src.BodyType))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.Function, opt => opt.MapFrom(src => src.Function))
            .ForMember(dest => dest.Regional, opt => opt.MapFrom(src => src.Regional))
            .ForMember(dest => dest.VehicleStatus, opt => opt.MapFrom(src => src.VehicleStatus))
            .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType))
            .ForMember(dest => dest.Vendor, opt => opt.MapFrom(src => src.Vendor))
            .ForMember(dest => dest.SupplyMethod, opt => opt.MapFrom(src => src.SupplyMethod))
            .IgnoreAllNonExisting();

            Mapper.CreateMap<VehicleOverallReportGetByParamInput, VehicleOverallSearchView>()
            .ForMember(dest => dest.FromDate, opt => opt.MapFrom(src => src.FromDate))
            .ForMember(dest => dest.ToDate, opt => opt.MapFrom(src => src.ToDate))
            .ForMember(dest => dest.BodyType, opt => opt.MapFrom(src => src.BodyType))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.Function, opt => opt.MapFrom(src => src.Function))
            .ForMember(dest => dest.Regional, opt => opt.MapFrom(src => src.Regional))
            .ForMember(dest => dest.VehicleStatus, opt => opt.MapFrom(src => src.VehicleStatus))
            .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType))
            .ForMember(dest => dest.Vendor, opt => opt.MapFrom(src => src.Vendor))
            .ForMember(dest => dest.SupplyMethod, opt => opt.MapFrom(src => src.SupplyMethod))
            .IgnoreAllNonExisting();

        }
    }
}