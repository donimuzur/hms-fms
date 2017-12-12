using AutoMapper;
using FMS.AutoMapperExtensions;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FMS.Website.Code
{
    public partial class FMSWebsiteMapper
    {
        public static void InitializeCfmIdleReport()
        {
            Mapper.CreateMap<CfmIdleReportDto, CfmIdleVehicle>().IgnoreAllNonExisting();
            Mapper.CreateMap<CfmIdleVehicle, CfmIdleReportDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<CfmIdleGetByParamInput, CfmIdleSearchView>()
                .ForMember(dest => dest.FromDate, opt => opt.MapFrom(src => src.FromDate))
                .ForMember(dest => dest.ToDate, opt => opt.MapFrom(src => src.ToDate))
                .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.CostCenter))
                .ForMember(dest => dest.PoliceNumber, opt => opt.MapFrom(src => src.PoliceNumber))
                .IgnoreAllNonExisting(); 

            Mapper.CreateMap<CfmIdleSearchView, CfmIdleGetByParamInput>()
                .ForMember(dest => dest.FromDate, opt => opt.MapFrom(src => src.FromDate))
                .ForMember(dest => dest.ToDate, opt => opt.MapFrom(src => src.ToDate))
                .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.CostCenter))
                .ForMember(dest => dest.PoliceNumber, opt => opt.MapFrom(src => src.PoliceNumber))
                .IgnoreAllNonExisting();
        }
    }
}