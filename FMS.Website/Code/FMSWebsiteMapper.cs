using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using AutoMapper;
using FMS.AutoMapperExtensions;
using FMS.BusinessObject.Dto;
using FMS.Website.Models;
using FMS.BusinessObject;
using FMS.BusinessObject.Inputs;

namespace FMS.Website.Code
{
    public partial class FMSWebsiteMapper
    {
        public static void Initialize()
        {
            InitializeCSF();
            InitializeCTF();
            InitializeCCF();
            InitializeCRF();
            InitializeTEMP();
	        InitializeCAF();
            InitializeExecutiveSummary();
            InitializeCfmIdleReport();
            InitializeVehicleOverallReport();
            InitializeRptFuel();
            InitializeRptPo();
            InitializeRptCCF();
            InitializeKpiMonitoring();
            InitializeGs();
            InitializePenalty();
            InitializeVehicleSpect();
            InitializeGroupCostCenter();
            InitializeHolidayCalender();

            Mapper.CreateMap<ChangesHistoryDto, ChangesLogs>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Action, opt => opt.MapFrom(src => src.ACTION))
                .ForMember(dest => dest.ActionDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.MODIFIED_BY));

            Mapper.CreateMap<WorkflowHistoryDto, WorkflowLogs>().IgnoreAllNonExisting()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.ROLE_NAME))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.REMARK_DESCRIPTION))
                .ForMember(dest => dest.Action, opt => opt.MapFrom(src => src.ACTION))
                .ForMember(dest => dest.ActionDate, opt => opt.MapFrom(src => src.ACTION_DATE))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.ACTION_BY))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.ACTION_BY));

            //Begin Map Complait
            Mapper.CreateMap<ComplaintDto, ComplaintCategoryItem>().IgnoreAllNonExisting();

            Mapper.CreateMap<ComplaintCategoryItem, ComplaintDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<MST_COMPLAINT_CATEGORY, ComplaintDto>().IgnoreAllNonExisting()
               .ForMember(dest => dest.MstComplaintCategoryId, opt => opt.MapFrom(src => src.MST_COMPLAINT_CATEGORY_ID))
               .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CATEGORY_NAME))
               .ForMember(dest => dest.RoleType, opt => opt.MapFrom(src => src.ROLE_TYPE))
               .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
               .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
               .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
               .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE));

            Mapper.CreateMap<ComplaintDto, MST_COMPLAINT_CATEGORY>().IgnoreAllNonExisting()
              .ForMember(dest => dest.MST_COMPLAINT_CATEGORY_ID, opt => opt.MapFrom(src => src.MstComplaintCategoryId))
               .ForMember(dest => dest.CATEGORY_NAME, opt => opt.MapFrom(src => src.CategoryName))
               .ForMember(dest => dest.ROLE_TYPE, opt => opt.MapFrom(src => src.RoleType))
               .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
               .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
               .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
               .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate));

            Mapper.CreateMap<ComplaintDto, ComplaintCategoryItem>().IgnoreAllNonExisting()
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate))
            .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.ModifiedBy == null ? src.CreatedBy : src.ModifiedBy));
            //End Map Complaint

            Mapper.CreateMap<VendorDto, VendorItem>().IgnoreAllNonExisting(); 

            Mapper.CreateMap<VendorItem, VendorDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<VendorUploadItem, VendorItem>().IgnoreAllNonExisting()
           .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate));

            Mapper.CreateMap<FleetItem, FleetDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<FleetDto, FleetItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.ModifiedBy == null ? src.CreatedBy : src.ModifiedBy))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate))
                ;

            Mapper.CreateMap<FleetDashboardItem, FleetChangeDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<FleetChangeDto, FleetDashboardItem>().IgnoreAllNonExisting();
          
            //Begin Map Master Employee//
            Mapper.CreateMap<EmployeeDto, EmployeeItem>().IgnoreAllNonExisting()
            .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.MODIFIED_DATE == null ? src.CREATED_DATE : src.MODIFIED_DATE));

            Mapper.CreateMap<EmployeeItem, EmployeeDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<MST_EMPLOYEE, EmployeeDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.EMPLOYEE_ID, opt => opt.MapFrom(src => src.EMPLOYEE_ID))
                .ForMember(dest => dest.FORMAL_NAME, opt => opt.MapFrom(src => src.FORMAL_NAME))
                .ForMember(dest => dest.GROUP_LEVEL, opt => opt.MapFrom(src => src.GROUP_LEVEL))
                .ForMember(dest => dest.POSITION_TITLE, opt => opt.MapFrom(src => src.POSITION_TITLE))
                .ForMember(dest => dest.DIVISON, opt => opt.MapFrom(src => src.DIVISON))
                .ForMember(dest => dest.DIRECTORATE, opt => opt.MapFrom(src => src.DIRECTORATE))
                .ForMember(dest => dest.ADDRESS, opt => opt.MapFrom(src => src.ADDRESS))
                .ForMember(dest => dest.CITY, opt => opt.MapFrom(src => src.CITY))
                .ForMember(dest => dest.BASETOWN, opt => opt.MapFrom(src => src.BASETOWN))
                .ForMember(dest => dest.COMPANY, opt => opt.MapFrom(src => src.COMPANY))
                .ForMember(dest => dest.COST_CENTER, opt => opt.MapFrom(src => src.COST_CENTER))
                .ForMember(dest => dest.GROUP_LEVEL, opt => opt.MapFrom(src => src.GROUP_LEVEL))
                .ForMember(dest => dest.EMAIL_ADDRESS, opt => opt.MapFrom(src => src.EMAIL_ADDRESS))
                .ForMember(dest => dest.FLEX_POINT, opt => opt.MapFrom(src => src.FLEX_POINT))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IS_ACTIVE));

            Mapper.CreateMap<EmployeeDto, MST_EMPLOYEE>().IgnoreAllNonExisting()
               .ForMember(dest => dest.EMPLOYEE_ID, opt => opt.MapFrom(src => src.EMPLOYEE_ID))
                .ForMember(dest => dest.FORMAL_NAME, opt => opt.MapFrom(src => src.FORMAL_NAME))
                .ForMember(dest => dest.GROUP_LEVEL, opt => opt.MapFrom(src => src.GROUP_LEVEL))
                .ForMember(dest => dest.POSITION_TITLE, opt => opt.MapFrom(src => src.POSITION_TITLE))
                .ForMember(dest => dest.DIVISON, opt => opt.MapFrom(src => src.DIVISON))
                .ForMember(dest => dest.DIRECTORATE, opt => opt.MapFrom(src => src.DIRECTORATE))
                .ForMember(dest => dest.ADDRESS, opt => opt.MapFrom(src => src.ADDRESS))
                .ForMember(dest => dest.CITY, opt => opt.MapFrom(src => src.CITY))
                .ForMember(dest => dest.BASETOWN, opt => opt.MapFrom(src => src.BASETOWN))
                .ForMember(dest => dest.COMPANY, opt => opt.MapFrom(src => src.COMPANY))
                .ForMember(dest => dest.COST_CENTER, opt => opt.MapFrom(src => src.COST_CENTER))
                .ForMember(dest => dest.GROUP_LEVEL, opt => opt.MapFrom(src => src.GROUP_LEVEL))
                .ForMember(dest => dest.EMAIL_ADDRESS, opt => opt.MapFrom(src => src.EMAIL_ADDRESS))
                .ForMember(dest => dest.FLEX_POINT, opt => opt.MapFrom(src => src.FLEX_POINT))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IS_ACTIVE));

            Mapper.CreateMap<EmployeeUploadItem, EmployeeItem>().IgnoreAllNonExisting()
           .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.MODIFIED_DATE == null ? src.CREATED_DATE : src.MODIFIED_DATE));

            Mapper.CreateMap<EmployeeItem, EmployeeUploadItem>().IgnoreAllNonExisting();

            Mapper.CreateMap<EmployeeDto, EmployeeUploadItem>().IgnoreAllNonExisting();

            Mapper.CreateMap<EmployeeUploadItem, EmployeeDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<RemarkDto, RemarkItem>().IgnoreAllNonExisting();

            //Begin Map Master Penalty//
            Mapper.CreateMap<PenaltyDto, PenaltyItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Models, opt => opt.MapFrom(src => src.Model))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.ModifiedBy == null ? src.CreatedBy : src.ModifiedBy));

            Mapper.CreateMap<PenaltyItem, PenaltyDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Models));
            //End Map Master Penalty//

            Mapper.CreateMap<RemarkItem, RemarkDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<ReasonDto, ReasonItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PenaltyForFleet, opt => opt.MapFrom(src => src.PenaltyForFleet.HasValue == false? false: src.PenaltyForFleet))
                .ForMember(dest => dest.PenaltyForEmplloyee, opt => opt.MapFrom(src => src.PenaltyForEmplloyee.HasValue == false? false : src.PenaltyForEmplloyee)); 
           
           
            Mapper.CreateMap<PriceListDto, PriceListItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Models, opt => opt.MapFrom(src => src.Model));

            Mapper.CreateMap<PriceListItem, PriceListDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Models));

            Mapper.CreateMap<PriceListItem, PriceListItem>().IgnoreAllNonExisting();

            Mapper.CreateMap<ReasonItem, ReasonDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<LocationMappingDto, LocationMappingItem>().IgnoreAllNonExisting();

            Mapper.CreateMap<LocationMappingItem, LocationMappingDto>().IgnoreAllNonExisting();

            //End Map Master Employee//

            //BEGIN Master Data Vehicle Spect//
            Mapper.CreateMap<VehicleSpectDto, VehicleSpectItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Colour, opt => opt.MapFrom(src => src.Color))
               .ForMember(dest => dest.Models, opt => opt.MapFrom(src => src.Models))
               .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate));

            Mapper.CreateMap<VehicleSpectItem, VehicleSpectDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Colour))
                .ForMember(dest => dest.Models, opt => opt.MapFrom(src => src.Models));

            Mapper.CreateMap<MST_VEHICLE_SPECT, VehicleSpectDto>().IgnoreAllNonExisting()
              .ForMember(dest => dest.MstVehicleSpectId, opt => opt.MapFrom(src => src.MST_VEHICLE_SPECT_ID))
                .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom(src => src.MANUFACTURER))
                .ForMember(dest => dest.Models, opt => opt.MapFrom(src => src.MODEL))
                .ForMember(dest => dest.Series, opt => opt.MapFrom(src => src.SERIES))
                .ForMember(dest => dest.Transmission, opt => opt.MapFrom(src => src.TRANSMISSION))
                .ForMember(dest => dest.BodyType, opt => opt.MapFrom(src => src.BODY_TYPE))
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

            Mapper.CreateMap<VehicleSpectDto, MST_VEHICLE_SPECT>().IgnoreAllNonExisting()
              .ForMember(dest => dest.MST_VEHICLE_SPECT_ID, opt => opt.MapFrom(src => src.MstVehicleSpectId))
                .ForMember(dest => dest.MANUFACTURER, opt => opt.MapFrom(src => src.Manufacturer))
                .ForMember(dest => dest.MODEL, opt => opt.MapFrom(src => src.Models))
                .ForMember(dest => dest.SERIES, opt => opt.MapFrom(src => src.Series))
                .ForMember(dest => dest.TRANSMISSION, opt => opt.MapFrom(src => src.Transmission))
                .ForMember(dest => dest.BODY_TYPE, opt => opt.MapFrom(src => src.BodyType))
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

            //END Master Data Vehicle Spect//

            // Start --- Master Data -> PriceList
            Mapper.CreateMap<PriceListDto, PriceListItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate));
            Mapper.CreateMap<PriceListItem, PriceListDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<MST_PRICELIST, PriceListDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MstPriceListId, opt => opt.MapFrom(src => src.MST_PRICELIST_ID))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.YEAR))
                .ForMember(dest => dest.Manufacture, opt => opt.MapFrom(src => src.MANUFACTURER))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.MODEL))
                .ForMember(dest => dest.Series, opt => opt.MapFrom(src => src.SERIES))
                .ForMember(dest => dest.ZonePriceList, opt => opt.MapFrom(src => src.ZONE_PRICE_LIST))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.PRICE))
                .ForMember(dest => dest.InstallmenHMS, opt => opt.MapFrom(src => src.INSTALLMEN_HMS))
                .ForMember(dest => dest.InstallmenEMP, opt => opt.MapFrom(src => src.INSTALLMEN_EMP))
                .ForMember(dest => dest.Vendor, opt => opt.MapFrom(src => src.VENDOR))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE));

            Mapper.CreateMap<PriceListDto, MST_PRICELIST>().IgnoreAllNonExisting()
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
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));

            // End --- Master Data -> PriceList


            // Start --- Master Data -> Setting
            Mapper.CreateMap<SettingDto, SettingItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate));
            Mapper.CreateMap<SettingItem, SettingDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<SettingItem, SettingDto>().IgnoreAllNonExisting()
           .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate));


            Mapper.CreateMap<MST_SETTING, SettingDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MstSettingId, opt => opt.MapFrom(src => src.MST_SETTING_ID))
                .ForMember(dest => dest.SettingGroup, opt => opt.MapFrom(src => src.SETTING_GROUP))
                .ForMember(dest => dest.SettingName, opt => opt.MapFrom(src => src.SETTING_NAME))
                .ForMember(dest => dest.SettingValue, opt => opt.MapFrom(src => src.SETTING_VALUE))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE));

            Mapper.CreateMap<SettingDto, MST_SETTING>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MST_SETTING_ID, opt => opt.MapFrom(src => src.MstSettingId))
                .ForMember(dest => dest.SETTING_GROUP, opt => opt.MapFrom(src => src.SettingGroup))
                .ForMember(dest => dest.SETTING_NAME, opt => opt.MapFrom(src => src.SettingName))
                .ForMember(dest => dest.SETTING_VALUE, opt => opt.MapFrom(src => src.SettingValue))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));

            Mapper.CreateMap<SettingItem, SettingDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<SettingItem, SettingDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<GroupCostCenterDto, GroupCostCenterItem>().IgnoreAllNonExisting();

            Mapper.CreateMap<GroupCostCenterItem, GroupCostCenterDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<PenaltyLogicDto, PenaltyLogicItem>().IgnoreAllNonExisting();

            Mapper.CreateMap<PenaltyLogicItem, PenaltyLogicDto>().IgnoreAllNonExisting();

            //BEGIN Epaf
            Mapper.CreateMap<EpafDto, EpafItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.ModifiedBy == null ? src.CreatedBy : src.ModifiedBy));
            //END Epaf

            //BEGIN Holiday Calender            
            Mapper.CreateMap<HolidayCalenderDto, HolidayCalenderItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate));
            Mapper.CreateMap<HolidayCalenderItem, HolidayCalenderDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<MST_HOLIDAY_CALENDAR, HolidayCalenderDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MstHolidayDateId, opt => opt.MapFrom(src => src.MST_HOLIDAY_ID))
                .ForMember(dest => dest.MstHolidayDate, opt => opt.MapFrom(src => src.MST_HOLIDAY_DATE))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.DESCRIPTION))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE));

            Mapper.CreateMap<HolidayCalenderDto, MST_HOLIDAY_CALENDAR>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MST_HOLIDAY_ID, opt => opt.MapFrom(src => src.MstHolidayDateId))
                .ForMember(dest => dest.MST_HOLIDAY_DATE, opt => opt.MapFrom(src => src.MstHolidayDate))
                .ForMember(dest => dest.DESCRIPTION, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));
            //END Holiday Calender

            //BEGIN FuelOdometer
            Mapper.CreateMap<FuelOdometerDto, FuelOdometerItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate));
            //END FuelOdometer
            Mapper.CreateMap<FuelOdometerItem, FuelOdometerDto>().IgnoreAllNonExisting()
           .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate));
            //BEGIN Delegation
            Mapper.CreateMap<DelegationDto, DelegationItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate));

            Mapper.CreateMap<DelegationItem, DelegationDto>().IgnoreAllNonExisting();
            //END Delegation

            //BEGIN Sales Volume
            Mapper.CreateMap<SalesVolumeDto, SalesVolumeItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MonthS, opt => opt.MapFrom(src => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(src.Month)))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate));

            Mapper.CreateMap<SalesVolumeItem, SalesVolumeDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<SearchView, SalesVolumeParamInput>().IgnoreAllNonExisting();
            Mapper.CreateMap<SearchViewExport, SalesVolumeParamInput>().IgnoreAllNonExisting();
            //END Sales Volume

            Mapper.CreateMap<SysAccessDto, SysAccessItem>().IgnoreAllNonExisting();

            Mapper.CreateMap<SysAccessItem, SysAccessDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<GsDto, GsItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Models, opt => opt.MapFrom(src => src.Model)); ;
            Mapper.CreateMap<GsItem, GsDto>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Models)); ;

            // Start --- Master Data -> CostOb
            Mapper.CreateMap<CostObDto, CostObItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Models, opt => opt.MapFrom(src => src.Model));
            Mapper.CreateMap<CostObItem, CostObDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Models));

            Mapper.CreateMap<MST_COST_OB, CostObDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MstCostObId, opt => opt.MapFrom(src => src.MST_COST_OB_ID))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.YEAR))
                .ForMember(dest => dest.Zone, opt => opt.MapFrom(src => src.ZONE))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.MODEL))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TYPE))
                .ForMember(dest => dest.ObCost, opt => opt.MapFrom(src => src.OB_COST))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.REMARK))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE));

            Mapper.CreateMap<CostObDto, MST_COST_OB>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MST_COST_OB_ID, opt => opt.MapFrom(src => src.MstCostObId))
                .ForMember(dest => dest.YEAR, opt => opt.MapFrom(src => src.Year))
                .ForMember(dest => dest.ZONE, opt => opt.MapFrom(src => src.Zone))
                .ForMember(dest => dest.MODEL, opt => opt.MapFrom(src => src.Model))
                .ForMember(dest => dest.TYPE, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.OB_COST, opt => opt.MapFrom(src => src.ObCost))
                .ForMember(dest => dest.REMARK, opt => opt.MapFrom(src => src.Remark))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));

            Mapper.CreateMap<CostObSearchView, CostObParamInput>().IgnoreAllNonExisting();
            Mapper.CreateMap<CostObParamInput, CostObSearchView>().IgnoreAllNonExisting();
            // End --- Master Data -> CostOb
            Mapper.CreateMap<PricelistParamInput, PricelistSearchView>().IgnoreAllNonExisting();
            Mapper.CreateMap<PricelistSearchView, PricelistParamInput>().IgnoreAllNonExisting();

            Mapper.CreateMap<EpafParamInput, EpafSearchView>().IgnoreAllNonExisting();
            Mapper.CreateMap<EpafSearchView, EpafParamInput>().IgnoreAllNonExisting();
            #region AutoGR
            Mapper.CreateMap<RptAutoGrDto, RptAutoGrItem>().IgnoreAllNonExisting();
            #endregion

            Mapper.CreateMap<LocationMappingParamInput, LocationMappingSearchView>().IgnoreAllNonExisting();
            Mapper.CreateMap<LocationMappingSearchView, LocationMappingParamInput>().IgnoreAllNonExisting();

        }
    }
}