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
               .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
              ;

            Mapper.CreateMap<ComplaintDto, MST_COMPLAINT_CATEGORY>().IgnoreAllNonExisting()
              .ForMember(dest => dest.MST_COMPLAINT_CATEGORY_ID, opt => opt.MapFrom(src => src.MstComplaintCategoryId))
               .ForMember(dest => dest.CATEGORY_NAME, opt => opt.MapFrom(src => src.CategoryName))
               .ForMember(dest => dest.ROLE_TYPE, opt => opt.MapFrom(src => src.RoleType))
               .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
               .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
               .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
               .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate));

            Mapper.CreateMap<ComplaintDto, ComplaintCategoryItem>().IgnoreAllNonExisting()
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate));
            //End Map Complaint

            Mapper.CreateMap<VendorDto, VendorItem>().IgnoreAllNonExisting()
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate ));

            Mapper.CreateMap<VendorItem, VendorDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<VendorUploadItem, VendorItem>().IgnoreAllNonExisting()
           .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate));

            Mapper.CreateMap<FleetItem, FleetDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<FleetDto, FleetItem>().IgnoreAllNonExisting();
          
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

            Mapper.CreateMap<RemarkDto, RemarkItem>().IgnoreAllNonExisting();

            Mapper.CreateMap<RemarkItem, RemarkDto>().IgnoreAllNonExisting();


            Mapper.CreateMap<ReasonDto, ReasonItem>().IgnoreAllNonExisting();

            Mapper.CreateMap<ReasonItem, ReasonDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<LocationMappingDto, LocationMappingItem>().IgnoreAllNonExisting();

            Mapper.CreateMap<LocationMappingItem, LocationMappingDto>().IgnoreAllNonExisting();

            //End Map Master Employee//
        }
    }
}