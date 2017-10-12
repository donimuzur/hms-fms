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
        }
    }
}