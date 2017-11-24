﻿using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using AutoMapper;
using FMS.BusinessObject.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BLL.Mapper
{
    public class FleetMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<MST_FLEET, FleetDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MstFleetId , opt => opt.MapFrom(src => src.MST_FLEET_ID))
                .ForMember(dest => dest.PoliceNumber, opt => opt.MapFrom(src => src.POLICE_NUMBER ))
                .ForMember(dest => dest.ChasisNumber, opt => opt.MapFrom(src => src.CHASIS_NUMBER ))
                .ForMember(dest => dest.EngineNumber, opt => opt.MapFrom(src => src.ENGINE_NUMBER ))
                .ForMember(dest => dest.EmployeeID , opt => opt.MapFrom(src => src.EMPLOYEE_ID ))
                .ForMember(dest => dest.EmployeeName  , opt => opt.MapFrom(src => src.EMPLOYEE_NAME ))
                .ForMember(dest => dest.GroupLevel , opt => opt.MapFrom(src => src.GROUP_LEVEL ))
                .ForMember(dest => dest.AssignedTo , opt => opt.MapFrom(src => src.ASSIGNED_TO))
                .ForMember(dest => dest.CostCenter , opt => opt.MapFrom(src => src.COST_CENTER))
                .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.VENDOR_NAME))
                .ForMember(dest => dest.Manufacturer , opt => opt.MapFrom(src => src.MANUFACTURER))
                .ForMember(dest => dest.Models , opt => opt.MapFrom(src => src.MODEL))
                .ForMember(dest => dest.Series, opt => opt.MapFrom(src => src.SERIES))
                .ForMember(dest => dest.BodyType, opt => opt.MapFrom(src => src.BODY_TYPE))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.COLOR))
                .ForMember(dest => dest.Transmission, opt => opt.MapFrom(src => src.TRANSMISSION))
                .ForMember(dest => dest.CarGroupLevel , opt => opt.MapFrom(src => src.CAR_GROUP_LEVEL))
                .ForMember(dest => dest.FuelType, opt => opt.MapFrom(src => src.FUEL_TYPE))
                .ForMember(dest => dest.Branding, opt => opt.MapFrom(src => src.BRANDING))
                .ForMember(dest => dest.Airbag, opt => opt.MapFrom(src => src.AIRBAG))
                .ForMember(dest => dest.VehicleYear , opt => opt.MapFrom(src => src.VEHICLE_YEAR ))
                .ForMember(dest => dest.VehicleType , opt => opt.MapFrom(src => src.VEHICLE_TYPE ))
                .ForMember(dest => dest.VehicleUsage , opt => opt.MapFrom(src => src.VEHICLE_USAGE ))
                .ForMember(dest => dest.SupplyMethod, opt => opt.MapFrom(src => src.SUPPLY_METHOD))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.CITY))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.ADDRESS))
                .ForMember(dest => dest.Vat, opt => opt.MapFrom(src => src.VAT))
                .ForMember(dest => dest.Restitution, opt => opt.MapFrom(src => src.RESTITUTION))
                .ForMember(dest => dest.StartDate , opt => opt.MapFrom(src => src.START_DATE))
                .ForMember(dest => dest.EndDate , opt => opt.MapFrom(src => src.END_DATE ))
                .ForMember(dest => dest.PoNumber , opt => opt.MapFrom(src => src.PO_NUMBER ))
                .ForMember(dest => dest.PoLine , opt => opt.MapFrom(src => src.PO_LINE ))
                .ForMember(dest => dest.StartContract , opt => opt.MapFrom(src => src.START_CONTRACT ))
                .ForMember(dest => dest.EndContract, opt => opt.MapFrom(src => src.END_CONTRACT ))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.MONTHLY_HMS_INSTALLMENT))
                .ForMember(dest => dest.VehicleStatus , opt => opt.MapFrom(src => src.VEHICLE_STATUS ))
                .ForMember(dest => dest.IsTaken , opt => opt.MapFrom(src => src.IS_TAKEN ))
                .ForMember(dest => dest.GrLeftQty , opt => opt.MapFrom(src => src.GR_LEFT_QTY  ))
                .ForMember(dest => dest.CreatedBy , opt => opt.MapFrom(src => src.CREATED_BY ))
                .ForMember(dest => dest.CreatedDate , opt => opt.MapFrom(src => src.CREATED_DATE ))
                .ForMember(dest => dest.ModifiedBy , opt => opt.MapFrom(src => src.MODIFIED_BY ))
                .ForMember(dest => dest.ModifiedDate , opt => opt.MapFrom(src => src.MODIFIED_DATE ))
                .ForMember(dest => dest.IsActive , opt => opt.MapFrom(src => src.IS_ACTIVE));

            AutoMapper.Mapper.CreateMap<FleetDto ,MST_FLEET  >().IgnoreAllNonExisting()
               .ForMember(dest => dest.MST_FLEET_ID , opt => opt.MapFrom(src => src.MstFleetId))
                .ForMember(dest => dest.POLICE_NUMBER , opt => opt.MapFrom(src => src.PoliceNumber))
                .ForMember(dest => dest.CHASIS_NUMBER , opt => opt.MapFrom(src => src.ChasisNumber))
                .ForMember(dest => dest.ENGINE_NUMBER, opt => opt.MapFrom(src => src.EngineNumber))
                .ForMember(dest => dest.EMPLOYEE_ID, opt => opt.MapFrom(src => src.EmployeeID))
                .ForMember(dest => dest.EMPLOYEE_NAME , opt => opt.MapFrom(src => src.EmployeeName))
                .ForMember(dest => dest.GROUP_LEVEL , opt => opt.MapFrom(src => src.GroupLevel))
                .ForMember(dest => dest.ASSIGNED_TO, opt => opt.MapFrom(src => src.AssignedTo))
                .ForMember(dest => dest.COST_CENTER , opt => opt.MapFrom(src => src.CostCenter))
                .ForMember(dest => dest.VENDOR_NAME, opt => opt.MapFrom(src => src.VendorName))
                .ForMember(dest => dest.MANUFACTURER, opt => opt.MapFrom(src => src.Manufacturer))
                .ForMember(dest => dest.MODEL, opt => opt.MapFrom(src => src.Models))
                .ForMember(dest => dest.SERIES, opt => opt.MapFrom(src => src.Series))
                .ForMember(dest => dest.BODY_TYPE , opt => opt.MapFrom(src => src.BodyType))
                .ForMember(dest => dest.COLOR, opt => opt.MapFrom(src => src.Color))
                .ForMember(dest => dest.TRANSMISSION , opt => opt.MapFrom(src => src.Transmission))
                .ForMember(dest => dest.CAR_GROUP_LEVEL , opt => opt.MapFrom(src => src.CarGroupLevel))
                .ForMember(dest => dest.FUEL_TYPE , opt => opt.MapFrom(src => src.FuelType))
                .ForMember(dest => dest.BRANDING , opt => opt.MapFrom(src => src.Branding))
                .ForMember(dest => dest.AIRBAG , opt => opt.MapFrom(src => src.Airbag))
                .ForMember(dest => dest.VEHICLE_YEAR , opt => opt.MapFrom(src => src.VehicleYear))
                .ForMember(dest => dest.VEHICLE_TYPE , opt => opt.MapFrom(src => src.VehicleType))
                .ForMember(dest => dest.VEHICLE_USAGE , opt => opt.MapFrom(src => src.VehicleUsage))
                .ForMember(dest => dest.SUPPLY_METHOD, opt => opt.MapFrom(src => src.SupplyMethod))
                .ForMember(dest => dest.CITY , opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.ADDRESS, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.VAT, opt => opt.MapFrom(src => src.Vat))
                .ForMember(dest => dest.RESTITUTION , opt => opt.MapFrom(src => src.Restitution))
                .ForMember(dest => dest.START_DATE, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.END_DATE, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.PO_NUMBER , opt => opt.MapFrom(src => src.PoNumber))
                .ForMember(dest => dest.PO_LINE , opt => opt.MapFrom(src => src.PoLine))
                .ForMember(dest => dest.START_CONTRACT, opt => opt.MapFrom(src => src.StartContract))
                .ForMember(dest => dest.END_CONTRACT, opt => opt.MapFrom(src => src.EndContract))
                .ForMember(dest => dest.MONTHLY_HMS_INSTALLMENT , opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.VEHICLE_STATUS , opt => opt.MapFrom(src => src.VehicleStatus))
                .ForMember(dest => dest.IS_TAKEN, opt => opt.MapFrom(src => src.IsTaken))
                .ForMember(dest => dest.GR_LEFT_QTY, opt => opt.MapFrom(src => src.GrLeftQty))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));
        }
    }
}
