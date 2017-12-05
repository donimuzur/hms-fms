using AutoMapper;
using FMS.AutoMapperExtensions;
using FMS.BusinessObject.Dto;
using FMS.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FMS.Website.Code
{
    public partial class FMSWebsiteMapper
    {
        public static void InitializeCCF()
        {
            Mapper.CreateMap<TraCcfDto, CcfItem>().IgnoreAllNonExisting()
                .ForMember(dest=> dest.Details_d1, opt=> opt.MapFrom(src=> src.Details));
            Mapper.CreateMap<CcfItem, TraCcfDto>().IgnoreAllNonExisting()
                .ForMember(dest=> dest.Details, opt=> opt.MapFrom(src=> src.Details_d1))
                .ForMember(dest => dest.CoodinatorResponseDate, opt => opt.MapFrom(src => src.DetailSave.CoodinatorResponseDate))
                .ForMember(dest => dest.CoodinatorNote, opt => opt.MapFrom(src => src.DetailSave.CoodinatorNote))
                .ForMember(dest => dest.CoodinatorPromiseDate, opt => opt.MapFrom(src => src.DetailSave.CoodinatorPromiseDate))
                .ForMember(dest => dest.CoodinatorAtt, opt => opt.MapFrom(src => src.DetailSave.CoodinatorAtt))
                .ForMember(dest => dest.VendorResponseDate, opt => opt.MapFrom(src => src.DetailSave.VendorResponseDate))
                ;

            Mapper.CreateMap<CcfItemDetil, TraCcfDetailDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ComplaintUrl, opt => opt.MapFrom(src => src.ComplaintUrl))
                .ForMember(dest => dest.CoordinatorUrl, opt => opt.MapFrom(src => src.CoordinatorUrl))
                .ForMember(dest => dest.VendorUrl, opt => opt.MapFrom(src => src.VendorUrl))
                ;
            Mapper.CreateMap<TraCcfDetailDto, CcfItemDetil>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.ComplaintUrl, opt => opt.MapFrom(src => src.ComplaintUrl))
                .ForMember(dest => dest.CoordinatorUrl, opt => opt.MapFrom(src => src.CoordinatorUrl))
                .ForMember(dest => dest.VendorUrl, opt => opt.MapFrom(src => src.VendorUrl))
                ;

            Mapper.CreateMap<TraCcfDetailDto, CcfItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ComplaintUrl, opt => opt.MapFrom(src => src.ComplaintUrl))
                .ForMember(dest => dest.CoodinatorUrl, opt => opt.MapFrom(src => src.CoordinatorUrl))
                .ForMember(dest => dest.VendorUrl, opt => opt.MapFrom(src => src.VendorUrl))
                ;

            Mapper.CreateMap<TraCcfDto, CcfItemDetil>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TraCcfId, opt => opt.MapFrom(src => src.TraCcfId))
                ;
            Mapper.CreateMap<CcfItemDetil, TraCcfDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<ComplaintCategoryItem, TraCcfDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ComplaintCategoryName, opt => opt.MapFrom(src => src.CategoryName))
                .ForMember(dest => dest.ComplaintCategoryRole, opt => opt.MapFrom(src => src.RoleType))
                ;

            //Versi Lama

            Mapper.CreateMap<CarComplaintFormDto, CarComplaintFormItem>().IgnoreAllNonExisting()
               .ForMember(dest => dest.Models, opt => opt.MapFrom(src => src.Models))
                .ForMember(dest => dest.EmployeeNameComplaintFor, opt => opt.MapFrom(src => src.EmployeeNameComplaintFor))
                .ForMember(dest => dest.VStartPeriod, opt => opt.MapFrom(src => Convert.ToDateTime(src.StartPeriod)))
                .ForMember(dest => dest.VEndPeriod, opt => opt.MapFrom(src => Convert.ToDateTime(src.EndPeriod)))
                ;
            //.ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate));

            Mapper.CreateMap<CarComplaintFormItem, CarComplaintFormDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Models, opt => opt.MapFrom(src => src.Models))
                .ForMember(dest => dest.EmployeeNameComplaintFor, opt => opt.MapFrom(src => src.EmployeeNameComplaintFor))
                .ForMember(dest => dest.StartPeriod, opt => opt.MapFrom(src => Convert.ToDateTime(src.VStartPeriod)))
                .ForMember(dest => dest.EndPeriod, opt => opt.MapFrom(src => Convert.ToDateTime(src.VEndPeriod)))
                ;

            Mapper.CreateMap<CarComplaintFormItem, CarComplaintFormDtoDetil>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ComplaintDate, opt => opt.MapFrom(src => src.ComplaintDate))
                .ForMember(dest => dest.ComplaintNote, opt => opt.MapFrom(src => src.ComplaintNote))
                .ForMember(dest => dest.ComplaintAtt, opt => opt.MapFrom(src => src.ComplaintAtt))
                ;

            Mapper.CreateMap<CarComplaintFormDtoDetil, CarComplaintFormItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ComplaintDate, opt => opt.MapFrom(src => src.ComplaintDate))
                .ForMember(dest => dest.ComplaintNote, opt => opt.MapFrom(src => src.ComplaintNote))
                .ForMember(dest => dest.ComplaintAtt, opt => opt.MapFrom(src => src.ComplaintAtt))
                ;

            Mapper.CreateMap<CarComplaintFormItemDetil, CarComplaintFormDtoDetil>().IgnoreAllNonExisting();

            Mapper.CreateMap<CarComplaintFormDtoDetil, CarComplaintFormItemDetil>().IgnoreAllNonExisting();
        }
    }
}