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
        public static void InitializeCTF()
        {
            Mapper.CreateMap<TraCtfDto, CsfData>().IgnoreAllNonExisting()
                //.ForMember(dest => dest.TraCsfId, opt => opt.MapFrom(src => src.TRA_CSF_ID))
                //.ForMember(dest => dest.CsfNumber, opt => opt.MapFrom(src => src.DOCUMENT_NUMBER))
                //.ForMember(dest => dest.CsfStatus, opt => opt.MapFrom(src => src.DOCUMENT_STATUS))
                //.ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EMPLOYEE_ID))
                //.ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EMPLOYEE_NAME))
                //.ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.REASON))
                //.ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE == null ? src.CREATED_DATE : src.MODIFIED_DATE))
                //.ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.EFFECTIVE_DATE))
                ;
        }
    }
}