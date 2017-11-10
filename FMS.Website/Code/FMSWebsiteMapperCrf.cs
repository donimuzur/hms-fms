using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using FMS.AutoMapperExtensions;
using FMS.BusinessObject.Dto;
using FMS.Website.Models;

namespace FMS.Website.Code
{
    public partial class FMSWebsiteMapper
    {
        public static void InitializeCRF()
        {
            Mapper.CreateMap<TraCrfDto, TraCrfItemDetails>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TraCrfId, opt => opt.MapFrom(src => src.TRA_CRF_ID))
                .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.DOCUMENT_NUMBER))
                .ForMember(dest => dest.DocumentStatus, opt => opt.MapFrom(src => src.DOCUMENT_STATUS))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EMPLOYEE_ID))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EMPLOYEE_NAME))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.REMARK))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE == null ? src.CREATED_DATE : src.MODIFIED_DATE))
                .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.EFFECTIVE_DATE))
                ;

            Mapper.CreateMap<TraCrfItemDetails,TraCrfDto>().ReverseMap().IgnoreAllNonExisting()
                .ForMember(dest => dest.TraCrfId, opt => opt.MapFrom(src => src.TRA_CRF_ID))
                .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.DOCUMENT_NUMBER))
                .ForMember(dest => dest.DocumentStatus, opt => opt.MapFrom(src => src.DOCUMENT_STATUS))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EMPLOYEE_ID))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EMPLOYEE_NAME))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.REMARK))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE == null ? src.CREATED_DATE : src.MODIFIED_DATE))
                .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.EFFECTIVE_DATE))
                ;

            Mapper.CreateMap<EpafDto, TraCrfEpafItem>().IgnoreAllNonExisting();
            Mapper.CreateMap<EpafDto, TraCrfEpafItem>().ReverseMap().IgnoreAllNonExisting();
        }
    }
}