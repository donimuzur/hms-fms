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

            Mapper.CreateMap<FleetDto, CtfItem>().IgnoreAllNonExisting()
             .ForMember(dest => dest.MstFleetId, opt => opt.MapFrom(src => src.MstFleetId))
             .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeID))
             .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EmployeeName))
             .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.CostCenter))
             .ForMember(dest => dest.GroupLevel, opt => opt.MapFrom(src => src.GroupLevel))
             .ForMember(dest => dest.SupplyMethod, opt => opt.MapFrom(src => src.SupplyMethod))
             .ForMember(dest => dest.PoliceNumber, opt => opt.MapFrom(src => src.PoliceNumber))
             .ForMember(dest => dest.VehicleYear, opt => opt.MapFrom(src => src.VehicleYear))
             .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType))
             .ForMember(dest => dest.VehicleUsage, opt => opt.MapFrom(src => src.VehicleUsage))
             .ForMember(dest => dest.EndRendDate, opt => opt.MapFrom(src => src.EndContract));

            Mapper.CreateMap<CtfItem, FleetDto >().IgnoreAllNonExisting()
             .ForMember(dest => dest.MstFleetId, opt => opt.MapFrom(src => src.MstFleetId))
             .ForMember(dest => dest.EmployeeID, opt => opt.MapFrom(src => src.EmployeeId))
             .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EmployeeName))
             .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.CostCenter))
             .ForMember(dest => dest.GroupLevel, opt => opt.MapFrom(src => src.GroupLevel))
             .ForMember(dest => dest.SupplyMethod, opt => opt.MapFrom(src => src.SupplyMethod))
             .ForMember(dest => dest.PoliceNumber, opt => opt.MapFrom(src => src.PoliceNumber))
             .ForMember(dest => dest.VehicleYear, opt => opt.MapFrom(src => src.VehicleYear))
             .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType))
             .ForMember(dest => dest.VehicleUsage, opt => opt.MapFrom(src => src.VehicleUsage))
             .ForMember(dest => dest.EndContract, opt => opt.MapFrom(src => src.EndRendDate));
        }
    }
}