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
            Mapper.CreateMap<TraCcfDto, CcfItem>().IgnoreAllNonExisting();
            Mapper.CreateMap<CcfItem, TraCcfDto>().IgnoreAllNonExisting();
            Mapper.CreateMap<ComplaintCategoryItem, TraCcfDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ComplaintCategoryName, opt => opt.MapFrom(src => src.CategoryName));


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