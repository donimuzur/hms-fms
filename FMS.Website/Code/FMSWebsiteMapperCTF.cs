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
            Mapper.CreateMap<TraCtfDto, CtfItem>().IgnoreAllNonExisting();
            Mapper.CreateMap<CtfItem, TraCtfDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<EpafDto, CtfItem>().IgnoreAllNonExisting()
             .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EmployeeName))
            .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.CostCenter))
            .ForMember(dest => dest.GroupLevel, opt => opt.MapFrom(src => src.GroupLevel))
            .ForMember(dest => dest.EpafId, opt => opt.MapFrom(src => src.MstEpafId))
            .ForMember(dest => dest.ReasonS, opt => opt.MapFrom(src => src.EpafAction));
        }
    }
}