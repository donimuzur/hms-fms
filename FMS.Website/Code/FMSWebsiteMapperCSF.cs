using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using AutoMapper;
using FMS.AutoMapperExtensions;
using FMS.BusinessObject.Dto;
using FMS.Website.Models;
using FMS.BusinessObject;
using FMS.BusinessObject.Inputs;
using FMS.Utils;

namespace FMS.Website.Code
{
    public partial class FMSWebsiteMapper
    {
        public static void InitializeCSF()
        {
            Mapper.CreateMap<TraCsfDto, CsfData>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TraCsfId, opt => opt.MapFrom(src => src.TRA_CSF_ID))
                .ForMember(dest => dest.CsfNumber, opt => opt.MapFrom(src => src.DOCUMENT_NUMBER))
                .ForMember(dest => dest.EpafId, opt => opt.MapFrom(src => src.EPAF_ID))
                .ForMember(dest => dest.CsfStatus, opt => opt.MapFrom(src => src.DOCUMENT_STATUS))
                .ForMember(dest => dest.CsfStatusName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.DOCUMENT_STATUS)))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EMPLOYEE_ID))
                .ForMember(dest => dest.EmployeeIdCreator, opt => opt.MapFrom(src => src.EMPLOYEE_ID_CREATOR))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EMPLOYEE_NAME))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.REASON_NAME))
                .ForMember(dest => dest.ReasonId, opt => opt.MapFrom(src => src.REASON_ID))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE == null ? src.CREATED_DATE : src.MODIFIED_DATE))
                .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.EFFECTIVE_DATE))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreateBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE))
                .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.COST_CENTER))
                .ForMember(dest => dest.GroupLevel, opt => opt.MapFrom(src => src.GROUP_LEVEL))
                .ForMember(dest => dest.CarGroupLevel, opt => opt.MapFrom(src => src.CAR_GROUP_LEVEL))
                .ForMember(dest => dest.CfmIdleId, opt => opt.MapFrom(src => src.CFM_IDLE_ID))
                .ForMember(dest => dest.FlexBenefit, opt => opt.MapFrom(src => src.FLEXBEN))
                .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VEHICLE_TYPE))
                .ForMember(dest => dest.VehicleTypeName, opt => opt.MapFrom(src => src.VEHICLE_TYPE_NAME))
                .ForMember(dest => dest.Regional, opt => opt.MapFrom(src => src.REGIONAL))
                .ForMember(dest => dest.VehicleCat, opt => opt.MapFrom(src => src.VEHICLE_CATEGORY))
                .ForMember(dest => dest.VehicleUsage, opt => opt.MapFrom(src => src.VEHICLE_USAGE))
                .ForMember(dest => dest.LocationCity, opt => opt.MapFrom(src => src.LOCATION_CITY))
                .ForMember(dest => dest.LocationAddress, opt => opt.MapFrom(src => src.LOCATION_ADDRESS))
                .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom(src => src.MANUFACTURER))
                .ForMember(dest => dest.Models, opt => opt.MapFrom(src => src.MODEL))
                .ForMember(dest => dest.Series, opt => opt.MapFrom(src => src.SERIES))
                .ForMember(dest => dest.BodyType, opt => opt.MapFrom(src => src.BODY_TYPE))
                .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.VENDOR_NAME))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.COLOUR))
                .ForMember(dest => dest.StartPeriod, opt => opt.MapFrom(src => src.START_PERIOD))
                .ForMember(dest => dest.EndPeriod, opt => opt.MapFrom(src => src.END_PERIOD))
                .ForMember(dest => dest.ManufacturerVendor, opt => opt.MapFrom(src => src.VENDOR_MANUFACTURER))
                .ForMember(dest => dest.ModelsVendor, opt => opt.MapFrom(src => src.VENDOR_MODEL))
                .ForMember(dest => dest.SeriesVendor, opt => opt.MapFrom(src => src.VENDOR_SERIES))
                .ForMember(dest => dest.BodyTypeVendor, opt => opt.MapFrom(src => src.VENDOR_BODY_TYPE))
                .ForMember(dest => dest.VendorNameVendor, opt => opt.MapFrom(src => src.VENDOR_VENDOR))
                .ForMember(dest => dest.ColorVendor, opt => opt.MapFrom(src => src.VENDOR_COLOUR))
                .ForMember(dest => dest.PoliceNumberVendor, opt => opt.MapFrom(src => src.VENDOR_POLICE_NUMBER))
                .ForMember(dest => dest.PoNumberVendor, opt => opt.MapFrom(src => src.VENDOR_PO_NUMBER))
                .ForMember(dest => dest.ChasisNumberVendor, opt => opt.MapFrom(src => src.VENDOR_CHASIS_NUMBER))
                .ForMember(dest => dest.EngineNumberVendor, opt => opt.MapFrom(src => src.VENDOR_ENGINE_NUMBER))
                .ForMember(dest => dest.TransmissionVendor, opt => opt.MapFrom(src => src.VENDOR_TRANSMISSION))
                .ForMember(dest => dest.BrandingVendor, opt => opt.MapFrom(src => src.VENDOR_BRANDING))
                .ForMember(dest => dest.PurposeVendor, opt => opt.MapFrom(src => src.VENDOR_PURPOSE))
                .ForMember(dest => dest.PoLineVendor, opt => opt.MapFrom(src => src.VENDOR_PO_LINE))
                .ForMember(dest => dest.IsAirBagVendor, opt => opt.MapFrom(src => src.VENDOR_AIR_BAG))
                .ForMember(dest => dest.IsVatVendor, opt => opt.MapFrom(src => src.VENDOR_VAT))
                .ForMember(dest => dest.IsRestitutionVendor, opt => opt.MapFrom(src => src.VENDOR_RESTITUTION))
                .ForMember(dest => dest.StartPeriodVendor, opt => opt.MapFrom(src => src.VENDOR_CONTRACT_START_DATE))
                .ForMember(dest => dest.EndPeriodVendor, opt => opt.MapFrom(src => src.VENDOR_CONTRACT_END_DATE))
                .ForMember(dest => dest.ExpectedDate, opt => opt.MapFrom(src => src.EXPECTED_DATE))
                .ForMember(dest => dest.EndRentDate, opt => opt.MapFrom(src => src.END_RENT_DATE))
                .ForMember(dest => dest.SupplyMethod, opt => opt.MapFrom(src => src.SUPPLY_METHOD))
                .ForMember(dest => dest.Project, opt => opt.MapFrom(src => src.PROJECT_NAME))
                .ForMember(dest => dest.AssignedTo, opt => opt.MapFrom(src => src.ASSIGNED_TO))
                ;

            Mapper.CreateMap<CsfData, TraCsfDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TRA_CSF_ID, opt => opt.MapFrom(src => src.TraCsfId))
                .ForMember(dest => dest.DOCUMENT_NUMBER, opt => opt.MapFrom(src => src.CsfNumber))
                .ForMember(dest => dest.EPAF_ID, opt => opt.MapFrom(src => src.EpafId))
                .ForMember(dest => dest.DOCUMENT_STATUS, opt => opt.MapFrom(src => src.CsfStatus))
                .ForMember(dest => dest.EMPLOYEE_ID, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.EMPLOYEE_ID_CREATOR, opt => opt.MapFrom(src => src.EmployeeIdCreator))
                .ForMember(dest => dest.EMPLOYEE_NAME, opt => opt.MapFrom(src => src.EmployeeName))
                .ForMember(dest => dest.COST_CENTER, opt => opt.MapFrom(src => src.CostCenter))
                .ForMember(dest => dest.GROUP_LEVEL, opt => opt.MapFrom(src => src.GroupLevel))
                .ForMember(dest => dest.CAR_GROUP_LEVEL, opt => opt.MapFrom(src => src.CarGroupLevel))
                .ForMember(dest => dest.FLEXBEN, opt => opt.MapFrom(src => src.FlexBenefit))
                .ForMember(dest => dest.CFM_IDLE_ID, opt => opt.MapFrom(src => src.CfmIdleId))
                .ForMember(dest => dest.REASON_ID, opt => opt.MapFrom(src => src.ReasonId))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.EFFECTIVE_DATE, opt => opt.MapFrom(src => src.EffectiveDate))
                .ForMember(dest => dest.REMARK_ID, opt => opt.MapFrom(src => src.RemarkId))
                .ForMember(dest => dest.VEHICLE_TYPE, opt => opt.MapFrom(src => src.VehicleType))
                .ForMember(dest => dest.VEHICLE_CATEGORY, opt => opt.MapFrom(src => src.VehicleCat))
                .ForMember(dest => dest.VEHICLE_USAGE, opt => opt.MapFrom(src => src.VehicleUsage))
                .ForMember(dest => dest.LOCATION_CITY, opt => opt.MapFrom(src => src.LocationCity))
                .ForMember(dest => dest.LOCATION_ADDRESS, opt => opt.MapFrom(src => src.LocationAddress))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreateBy))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.MANUFACTURER, opt => opt.MapFrom(src => src.Manufacturer))
                .ForMember(dest => dest.MODEL, opt => opt.MapFrom(src => src.Models))
                .ForMember(dest => dest.SERIES, opt => opt.MapFrom(src => src.Series))
                .ForMember(dest => dest.BODY_TYPE, opt => opt.MapFrom(src => src.BodyType))
                .ForMember(dest => dest.VENDOR_NAME, opt => opt.MapFrom(src => src.VendorName))
                .ForMember(dest => dest.COLOUR, opt => opt.MapFrom(src => src.Color))
                .ForMember(dest => dest.START_PERIOD, opt => opt.MapFrom(src => src.StartPeriod))
                .ForMember(dest => dest.END_PERIOD, opt => opt.MapFrom(src => src.EndPeriod))
                .ForMember(dest => dest.VENDOR_MANUFACTURER, opt => opt.MapFrom(src => src.ManufacturerVendor))
                .ForMember(dest => dest.VENDOR_MODEL, opt => opt.MapFrom(src => src.ModelsVendor))
                .ForMember(dest => dest.VENDOR_SERIES, opt => opt.MapFrom(src => src.SeriesVendor))
                .ForMember(dest => dest.VENDOR_BODY_TYPE, opt => opt.MapFrom(src => src.BodyTypeVendor))
                .ForMember(dest => dest.VENDOR_VENDOR, opt => opt.MapFrom(src => src.VendorNameVendor))
                .ForMember(dest => dest.VENDOR_COLOUR, opt => opt.MapFrom(src => src.ColorVendor))
                .ForMember(dest => dest.VENDOR_POLICE_NUMBER, opt => opt.MapFrom(src => src.PoliceNumberVendor))
                .ForMember(dest => dest.VENDOR_PO_NUMBER, opt => opt.MapFrom(src => src.PoNumberVendor))
                .ForMember(dest => dest.VENDOR_CONTRACT_START_DATE, opt => opt.MapFrom(src => src.StartPeriodVendor))
                .ForMember(dest => dest.VENDOR_CONTRACT_END_DATE, opt => opt.MapFrom(src => src.EndPeriodVendor))
                .ForMember(dest => dest.VENDOR_CHASIS_NUMBER, opt => opt.MapFrom(src => src.ChasisNumberVendor))
                .ForMember(dest => dest.VENDOR_ENGINE_NUMBER, opt => opt.MapFrom(src => src.EngineNumberVendor))
                .ForMember(dest => dest.VENDOR_TRANSMISSION, opt => opt.MapFrom(src => src.TransmissionVendor))
                .ForMember(dest => dest.VENDOR_BRANDING, opt => opt.MapFrom(src => src.BrandingVendor))
                .ForMember(dest => dest.VENDOR_PURPOSE, opt => opt.MapFrom(src => src.PurposeVendor))
                .ForMember(dest => dest.VENDOR_PO_LINE, opt => opt.MapFrom(src => src.PoLineVendor))
                .ForMember(dest => dest.VENDOR_AIR_BAG, opt => opt.MapFrom(src => src.IsAirBagVendor))
                .ForMember(dest => dest.VENDOR_VAT, opt => opt.MapFrom(src => src.IsVatVendor))
                .ForMember(dest => dest.VENDOR_RESTITUTION, opt => opt.MapFrom(src => src.IsRestitutionVendor))
                .ForMember(dest => dest.EXPECTED_DATE, opt => opt.MapFrom(src => src.ExpectedDate))
                .ForMember(dest => dest.END_RENT_DATE, opt => opt.MapFrom(src => src.EndRentDate))
                .ForMember(dest => dest.SUPPLY_METHOD, opt => opt.MapFrom(src => src.SupplyMethod))
                .ForMember(dest => dest.PROJECT_NAME, opt => opt.MapFrom(src => src.Project))
                .ForMember(dest => dest.ASSIGNED_TO, opt => opt.MapFrom(src => src.AssignedTo))
                ;

            Mapper.CreateMap<EpafDto, EpafData>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.ModifiedBy == null ? src.CreatedBy : src.ModifiedBy))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate))
                .ForMember(dest => dest.EpafEffectiveDate, opt => opt.MapFrom(src => src.EfectiveDate))
                .ForMember(dest => dest.EpafApprovedDate, opt => opt.MapFrom(src => src.ApprovedDate))
                .ForMember(dest => dest.Action, opt => opt.MapFrom(src => src.EpafAction))
                .ForMember(dest => dest.CostCentre, opt => opt.MapFrom(src => src.CostCenter));

            Mapper.CreateMap<TemporaryDto, TemporaryData>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TraTemporaryId, opt => opt.MapFrom(src => src.TRA_TEMPORARY_ID))
                .ForMember(dest => dest.StartPeriod, opt => opt.MapFrom(src => src.START_DATE))
                .ForMember(dest => dest.EndPeriod, opt => opt.MapFrom(src => src.END_DATE))
                .ForMember(dest => dest.ReasonTemp, opt => opt.MapFrom(src => src.REASON_NAME))
                .ForMember(dest => dest.TemporaryNumber, opt => opt.MapFrom(src => src.DOCUMENT_NUMBER_TEMP))
                .ForMember(dest => dest.UrlTemp, opt => opt.MapFrom(src => ConfigurationManager.AppSettings["WebRootUrl"] + "/TraTemporary/Edit/" + src.TRA_TEMPORARY_ID + "?isPersonalDashboard=False"));

            Mapper.CreateMap<VehicleFromVendorUpload, TemporaryData>().IgnoreAllNonExisting();

            Mapper.CreateMap<TemporaryData, VehicleFromVendorUpload>().IgnoreAllNonExisting();

            Mapper.CreateMap<VehicleFromUserUpload, VehicleData>().IgnoreAllNonExisting();

            Mapper.CreateMap<VehicleData, VehicleFromUserUpload>().IgnoreAllNonExisting();
        }
    }
}