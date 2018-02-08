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
    public class NotifEmailDataChangeMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<FLEET_CHANGE, FleetChangeDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ChangeDate, opt => opt.MapFrom(src => src.CHANGE_DATE))
                .ForMember(dest => dest.ChasisNumber, opt => opt.MapFrom(src => src.CHASIS_NUMBER))
                .ForMember(dest => dest.DataAfter, opt => opt.MapFrom(src => src.DATA_AFTER))
                .ForMember(dest => dest.DataBefore, opt => opt.MapFrom(src => src.DATA_BEFORE))
                .ForMember(dest => dest.DateSend, opt => opt.MapFrom(src => src.DATE_SEND))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EMPLOYEE_ID))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EMPLOYEE_NAME))
                .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.FIELD_NAME))
                .ForMember(dest => dest.FleetChangeId, opt => opt.MapFrom(src => src.FLEET_CHANGE_ID))
                .ForMember(dest => dest.FleetId, opt => opt.MapFrom(src => src.FLEET_ID))
                .ForMember(dest => dest.PoliceNumber, opt => opt.MapFrom(src => src.POLICE_NUMBER));

            AutoMapper.Mapper.CreateMap<FleetChangeDto, FLEET_CHANGE>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CHANGE_DATE, opt => opt.MapFrom(src => src.ChangeDate))
                .ForMember(dest => dest.CHASIS_NUMBER, opt => opt.MapFrom(src => src.ChasisNumber))
                .ForMember(dest => dest.DATA_AFTER, opt => opt.MapFrom(src => src.DataAfter))
                .ForMember(dest => dest.DATA_BEFORE, opt => opt.MapFrom(src => src.DataBefore))
                .ForMember(dest => dest.DATE_SEND, opt => opt.MapFrom(src => src.DateSend))
                .ForMember(dest => dest.EMPLOYEE_ID, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.EMPLOYEE_NAME, opt => opt.MapFrom(src => src.EmployeeName))
                .ForMember(dest => dest.FIELD_NAME, opt => opt.MapFrom(src => src.FieldName))
                .ForMember(dest => dest.FLEET_CHANGE_ID, opt => opt.MapFrom(src => src.FleetChangeId))
                .ForMember(dest => dest.FLEET_ID, opt => opt.MapFrom(src => src.FleetId))
                .ForMember(dest => dest.POLICE_NUMBER, opt => opt.MapFrom(src => src.PoliceNumber));

            AutoMapper.Mapper.CreateMap<FUNCTION_CHANGE, FunctionChangeDto>().IgnoreAllNonExisting()
               .ForMember(dest => dest.ChangeDate, opt => opt.MapFrom(src => src.CHANGE_DATE))
               .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.COST_CENTER))
               .ForMember(dest => dest.DateSend, opt => opt.MapFrom(src => src.DATE_SEND))
               .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EMPLOYEE_ID))
               .ForMember(dest => dest.FormalName, opt => opt.MapFrom(src => src.FORMAL_NAME))
               .ForMember(dest => dest.FunctionNew, opt => opt.MapFrom(src => src.FUNCTION_NEW))
               .ForMember(dest => dest.FunctionOld, opt => opt.MapFrom(src => src.FUNCTION_OLD))
               .ForMember(dest => dest.FunctionChangeId, opt => opt.MapFrom(src => src.FUNCTION_CHANGE_ID));
            AutoMapper.Mapper.CreateMap<FunctionChangeDto, FUNCTION_CHANGE>().IgnoreAllNonExisting()
               .ForMember(dest => dest.CHANGE_DATE, opt => opt.MapFrom(src => src.ChangeDate))
               .ForMember(dest => dest.COST_CENTER, opt => opt.MapFrom(src => src.CostCenter))
               .ForMember(dest => dest.DATE_SEND, opt => opt.MapFrom(src => src.DateSend))
               .ForMember(dest => dest.EMPLOYEE_ID, opt => opt.MapFrom(src => src.EmployeeId))
               .ForMember(dest => dest.FORMAL_NAME, opt => opt.MapFrom(src => src.FormalName))
               .ForMember(dest => dest.FUNCTION_NEW, opt => opt.MapFrom(src => src.FunctionNew))
               .ForMember(dest => dest.FUNCTION_OLD, opt => opt.MapFrom(src => src.FunctionOld))
               .ForMember(dest => dest.FUNCTION_CHANGE_ID, opt => opt.MapFrom(src => src.FunctionChangeId));

            AutoMapper.Mapper.CreateMap<LOCATION_CHANGE, LocationChangeDto>().IgnoreAllNonExisting()
              .ForMember(dest => dest.AddressNew, opt => opt.MapFrom(src => src.ADDRESS_NEW))
              .ForMember(dest => dest.AddressOld, opt => opt.MapFrom(src => src.ADDRESS_OLD))
              .ForMember(dest => dest.BasetownNew, opt => opt.MapFrom(src => src.BASETOWN_NEW))
              .ForMember(dest => dest.BasetownOld, opt => opt.MapFrom(src => src.BASETOWN_OLD))
              .ForMember(dest => dest.ChangeDate, opt => opt.MapFrom(src => src.CHANGE_DATE))
              .ForMember(dest => dest.CityNew, opt => opt.MapFrom(src => src.CITY_NEW))
              .ForMember(dest => dest.CityOld, opt => opt.MapFrom(src => src.CITY_OLD))
              .ForMember(dest => dest.DateSend, opt => opt.MapFrom(src => src.DATE_SEND))
              .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EMPLOYEE_ID))
              .ForMember(dest => dest.FormalName, opt => opt.MapFrom(src => src.FORMAL_NAME))
              .ForMember(dest => dest.LocationChangeId, opt => opt.MapFrom(src => src.LOCATION_CHANGE_ID));
            AutoMapper.Mapper.CreateMap<LocationChangeDto, LOCATION_CHANGE>().IgnoreAllNonExisting()
               .ForMember(dest => dest.ADDRESS_NEW, opt => opt.MapFrom(src => src.AddressNew))
              .ForMember(dest => dest.ADDRESS_OLD, opt => opt.MapFrom(src => src.AddressOld))
              .ForMember(dest => dest.BASETOWN_NEW, opt => opt.MapFrom(src => src.BasetownNew))
              .ForMember(dest => dest.BASETOWN_OLD, opt => opt.MapFrom(src => src.BasetownOld))
              .ForMember(dest => dest.CHANGE_DATE, opt => opt.MapFrom(src => src.ChangeDate))
              .ForMember(dest => dest.CITY_NEW, opt => opt.MapFrom(src => src.CityNew))
              .ForMember(dest => dest.CITY_OLD, opt => opt.MapFrom(src => src.CityOld))
              .ForMember(dest => dest.DATE_SEND, opt => opt.MapFrom(src => src.DateSend))
              .ForMember(dest => dest.EMPLOYEE_ID, opt => opt.MapFrom(src => src.EmployeeId))
              .ForMember(dest => dest.FORMAL_NAME, opt => opt.MapFrom(src => src.FormalName))
              .ForMember(dest => dest.LOCATION_CHANGE_ID, opt => opt.MapFrom(src => src.LocationChangeId));
        }
    }
}
