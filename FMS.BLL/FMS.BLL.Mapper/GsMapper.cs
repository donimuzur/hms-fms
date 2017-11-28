using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BLL.Mapper
{
    public class GsMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<MST_GS, GsDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MstGsId, opt => opt.MapFrom(src => src.MST_GS_ID))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EMPLOYEE_NAME))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EMPLOYEE_ID))
                .ForMember(dest => dest.VehicleUsage, opt => opt.MapFrom(src => src.VEHICLE_USAGE))
                .ForMember(dest => dest.PoliceNumber, opt => opt.MapFrom(src => src.POLICE_NUMBER))
                .ForMember(dest => dest.GroupLevel, opt => opt.MapFrom(src => src.GROUP_LEVEL))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.LOCATION))
                .ForMember(dest => dest.GsRequestDate, opt => opt.MapFrom(src => src.GS_REQUEST_DATE))
                .ForMember(dest => dest.GsFullfillmentDate, opt => opt.MapFrom(src => src.GS_FULLFILLMENT_DATE))
                .ForMember(dest => dest.GsUnitType, opt => opt.MapFrom(src => src.GS_UNIT_TYPE))
                .ForMember(dest => dest.GsPoliceNumber, opt => opt.MapFrom(src => src.GS_POLICE_NUMBER))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.START_DATE))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.END_DATE))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.REMARK))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE));

            AutoMapper.Mapper.CreateMap<GsDto, MST_GS>().IgnoreAllNonExisting()
             .ForMember(dest => dest.MST_GS_ID , opt => opt.MapFrom(src => src.MstGsId))
                .ForMember(dest => dest.EMPLOYEE_NAME, opt => opt.MapFrom(src => src.EmployeeName ))
                .ForMember(dest => dest.EMPLOYEE_ID, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.VEHICLE_USAGE, opt => opt.MapFrom(src => src.VehicleUsage))
                .ForMember(dest => dest.POLICE_NUMBER, opt => opt.MapFrom(src => src.PoliceNumber ))
                .ForMember(dest => dest.GROUP_LEVEL, opt => opt.MapFrom(src => src.GroupLevel))
                .ForMember(dest => dest.LOCATION, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.GS_REQUEST_DATE, opt => opt.MapFrom(src => src.GsRequestDate))
                .ForMember(dest => dest.GS_FULLFILLMENT_DATE, opt => opt.MapFrom(src => src.GsFullfillmentDate))
                .ForMember(dest => dest.GS_UNIT_TYPE, opt => opt.MapFrom(src => src.GsUnitType))
                .ForMember(dest => dest.GS_POLICE_NUMBER, opt => opt.MapFrom(src => src.GsPoliceNumber))
                .ForMember(dest => dest.START_DATE, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.END_DATE, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.REMARK, opt => opt.MapFrom(src => src.Remark))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));
        }
    }
}
