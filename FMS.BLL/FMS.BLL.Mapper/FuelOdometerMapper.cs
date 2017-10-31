using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;

namespace FMS.BLL.Mapper
{
    public class FuelOdometerMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<MST_FUEL_ODOMETER, FuelOdometerDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MstFuelOdometerId, opt => opt.MapFrom(src => src.MST_FUEL_ODOMETER_ID))
                .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VEHICLE_TYPE))
                .ForMember(dest => dest.PoliceNumber, opt => opt.MapFrom(src => src.POLICE_NUMBER))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EMPLOYEE_ID))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EMPLOYEE_NAME))
                .ForMember(dest => dest.EcsRmbTransId, opt => opt.MapFrom(src => src.ECS_RMB_TRANSID))
                .ForMember(dest => dest.SeqNumber, opt => opt.MapFrom(src => src.SEQ_NUMBER))
                .ForMember(dest => dest.ClaimType, opt => opt.MapFrom(src => src.CLAIM_TYPE))
                .ForMember(dest => dest.DateOfCost, opt => opt.MapFrom(src => src.DATE_OF_COST))
                .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.COST_CENTER))
                .ForMember(dest => dest.FuelAmount, opt => opt.MapFrom(src => src.FUEL_AMOUNT))
                .ForMember(dest => dest.LastKM, opt => opt.MapFrom(src => src.LAST_KM))
                .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.COST))
                .ForMember(dest => dest.ClaimComment, opt => opt.MapFrom(src => src.CLAIM_COMMENT))
                .ForMember(dest => dest.PostedTime, opt => opt.MapFrom(src => src.POSTED_TIME))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE));

            AutoMapper.Mapper.CreateMap<FuelOdometerDto, MST_FUEL_ODOMETER>().IgnoreAllNonExisting()
              .ForMember(dest => dest.MST_FUEL_ODOMETER_ID, opt => opt.MapFrom(src => src.MstFuelOdometerId))
                .ForMember(dest => dest.VEHICLE_TYPE, opt => opt.MapFrom(src => src.VehicleType))
                .ForMember(dest => dest.POLICE_NUMBER, opt => opt.MapFrom(src => src.PoliceNumber))
                .ForMember(dest => dest.EMPLOYEE_ID, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.EMPLOYEE_NAME, opt => opt.MapFrom(src => src.EmployeeName))
                .ForMember(dest => dest.ECS_RMB_TRANSID, opt => opt.MapFrom(src => src.EcsRmbTransId))
                .ForMember(dest => dest.SEQ_NUMBER, opt => opt.MapFrom(src => src.SeqNumber))
                .ForMember(dest => dest.CLAIM_TYPE, opt => opt.MapFrom(src => src.ClaimType))
                .ForMember(dest => dest.DATE_OF_COST, opt => opt.MapFrom(src => src.DateOfCost))
                .ForMember(dest => dest.COST_CENTER, opt => opt.MapFrom(src => src.CostCenter))
                .ForMember(dest => dest.FUEL_AMOUNT, opt => opt.MapFrom(src => src.FuelAmount))
                .ForMember(dest => dest.LAST_KM, opt => opt.MapFrom(src => src.LastKM))
                .ForMember(dest => dest.COST, opt => opt.MapFrom(src => src.Cost))
                .ForMember(dest => dest.CLAIM_COMMENT, opt => opt.MapFrom(src => src.ClaimComment))
                .ForMember(dest => dest.POSTED_TIME, opt => opt.MapFrom(src => src.PostedTime))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));
        }
    }
}
