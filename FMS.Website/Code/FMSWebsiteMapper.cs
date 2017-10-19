using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using FMS.AutoMapperExtensions;
using FMS.BusinessObject.Dto;
using FMS.Website.Models;
using FMS.BusinessObject;

namespace FMS.Website.Code
{
    public partial class FMSWebsiteMapper
    {
        public static void Initialize()
        {
            Mapper.CreateMap<ComplaintDto, ComplaintCategoryItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));

            Mapper.CreateMap<VendorDto, VendorItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate ));
            Mapper.CreateMap<VendorItem, VendorDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<VendorUploadItem, VendorItem>().IgnoreAllNonExisting()
           .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate));
            
            Mapper.CreateMap<MST_VENDOR, VendorDto>().IgnoreAllNonExisting()
               .ForMember(dest => dest.MstVendorId, opt => opt.MapFrom(src => src.MST_VENDOR_ID))
                .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.VENDOR_NAME))
                .ForMember(dest => dest.ShortName, opt => opt.MapFrom(src => src.SHORT_NAME))
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.EMAIL_ADDRESS))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreatedDate , opt => opt.MapFrom(src => src.MODIFIED_DATE ))
                .ForMember(dest => dest.ModifiedDate , opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE));

            Mapper.CreateMap<VendorDto, MST_VENDOR>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MST_VENDOR_ID , opt => opt.MapFrom(src => src.MstVendorId ))
                .ForMember(dest => dest.VENDOR_NAME , opt => opt.MapFrom(src => src.VendorName))
                .ForMember(dest => dest.SHORT_NAME, opt => opt.MapFrom(src => src.ShortName))
                .ForMember(dest => dest.EMAIL_ADDRESS, opt => opt.MapFrom(src => src.EmailAddress))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CREATED_DATE , opt => opt.MapFrom(src => src.CreatedDate ))
                .ForMember(dest => dest.MODIFIED_DATE , opt => opt.MapFrom(src => src.ModifiedDate ))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));

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

            //End Map Master Employee//

            //Begin Map Master Penalty//
            Mapper.CreateMap<PenaltyDto, PenaltyItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Models, opt => opt.MapFrom(src => src.Model))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate));

            Mapper.CreateMap<PenaltyItem, PenaltyDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Models));

            Mapper.CreateMap<MST_PENALTY, PenaltyDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MstPenaltyId, opt => opt.MapFrom(src => src.MST_PENALTY_ID))
                .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom(src => src.MANUFACTURER))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.MODEL))
                .ForMember(dest => dest.Series, opt => opt.MapFrom(src => src.SERIES))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.YEAR))
                .ForMember(dest => dest.MonthStart, opt => opt.MapFrom(src => src.MONTH_START))
                .ForMember(dest => dest.MonthEnd, opt => opt.MapFrom(src => src.MONTH_END))
                .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VEHICLE_TYPE))
                .ForMember(dest => dest.Penalty, opt => opt.MapFrom(src => src.PENALTY))
                .ForMember(dest => dest.Restitution, opt => opt.MapFrom(src => src.RESTITUTION))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE));


            Mapper.CreateMap<PenaltyDto, MST_PENALTY>().IgnoreAllNonExisting()
              .ForMember(dest => dest.MST_PENALTY_ID, opt => opt.MapFrom(src => src.MstPenaltyId))
               .ForMember(dest => dest.MANUFACTURER, opt => opt.MapFrom(src => src.Manufacturer))
               .ForMember(dest => dest.MODEL, opt => opt.MapFrom(src => src.Model))
               .ForMember(dest => dest.SERIES, opt => opt.MapFrom(src => src.Series))
               .ForMember(dest => dest.YEAR, opt => opt.MapFrom(src => src.Year))
               .ForMember(dest => dest.MONTH_START, opt => opt.MapFrom(src => src.MonthStart))
               .ForMember(dest => dest.MONTH_END, opt => opt.MapFrom(src => src.MonthEnd))
               .ForMember(dest => dest.VEHICLE_TYPE, opt => opt.MapFrom(src => src.VehicleType))
               .ForMember(dest => dest.PENALTY, opt => opt.MapFrom(src => src.Penalty))
               .ForMember(dest => dest.RESTITUTION, opt => opt.MapFrom(src => src.Restitution))
               .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
               .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
               .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
               .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
               .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));

           // Mapper.CreateMap<penaltyUploadItem, PenaltyItem>().IgnoreAllNonExisting()
           //.ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE == null ? src.CREATED_DATE : src.MODIFIED_DATE));

            //End Map Master Penalty//
        }
    }
}