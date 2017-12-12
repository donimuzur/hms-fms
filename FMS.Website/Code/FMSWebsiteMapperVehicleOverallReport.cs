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
        public static void InitializeVehicleOverallReport()
        {
            Mapper.CreateMap<VehicleOverallReportDto, VehicleOverallItem>().IgnoreAllNonExisting();
            Mapper.CreateMap<VehicleOverallItem, VehicleOverallReportDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<VehicleOverallSearchView, VehicleOverallReportGetByParamInput>()
            .ForMember(dest => dest.FromDate, opt => opt.MapFrom(src => src.FromDate))
            .ForMember(dest => dest.ToDate, opt => opt.MapFrom(src => src.ToDate))
            .ForMember(dest => dest.BodyType, opt => opt.MapFrom(src => src.BodyType))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.Function, opt => opt.MapFrom(src => src.Function))
            .ForMember(dest => dest.Regional, opt => opt.MapFrom(src => src.Regional))
            .ForMember(dest => dest.VehicleStatus, opt => opt.MapFrom(src => src.VehicleStatus))
            .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType))
            .ForMember(dest => dest.Vendor, opt => opt.MapFrom(src => src.Vendor))
            .ForMember(dest => dest.SupplyMethod, opt => opt.MapFrom(src => src.SupplyMethod))
            .IgnoreAllNonExisting();

            Mapper.CreateMap<VehicleOverallReportGetByParamInput, VehicleOverallSearchView>()
            .ForMember(dest => dest.FromDate, opt => opt.MapFrom(src => src.FromDate))
            .ForMember(dest => dest.ToDate, opt => opt.MapFrom(src => src.ToDate))
            .ForMember(dest => dest.BodyType, opt => opt.MapFrom(src => src.BodyType))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.Function, opt => opt.MapFrom(src => src.Function))
            .ForMember(dest => dest.Regional, opt => opt.MapFrom(src => src.Regional))
            .ForMember(dest => dest.VehicleStatus, opt => opt.MapFrom(src => src.VehicleStatus))
            .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType))
            .ForMember(dest => dest.Vendor, opt => opt.MapFrom(src => src.Vendor))
            .ForMember(dest => dest.SupplyMethod, opt => opt.MapFrom(src => src.SupplyMethod))
            .IgnoreAllNonExisting();

        }
    }
}