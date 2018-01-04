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
    public class CafMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<TRA_CAF, TraCafDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TraCafId, opt => opt.MapFrom(src=> src.TRA_CAF_ID))
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.AREA))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.DOCUMENT_NUMBER))
                .ForMember(dest => dest.DocumentStatus, opt => opt.MapFrom(src => src.DOCUMENT_STATUS))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EMPLOYEE_ID))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EMPLOYEE_NAME))
                .ForMember(dest => dest.IncidentDate, opt => opt.MapFrom(src => src.INCIDENT_DATE))
                .ForMember(dest => dest.IncidentDescription, opt => opt.MapFrom(src => src.INCIDENT_DESCRIPTION))
                .ForMember(dest => dest.IncidentLocation, opt => opt.MapFrom(src => src.INCIDENT_LOCATION))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.PoliceNumber, opt => opt.MapFrom(src => src.POLICE_NUMBER))
                .ForMember(dest => dest.RemarkId, opt => opt.MapFrom(src => src.REMARK))
                .ForMember(dest => dest.SirsNumber, opt => opt.MapFrom(src => src.SIRS_NUMBER))
                .ForMember(dest => dest.Supervisor, opt => opt.MapFrom(src => src.SUPERVISOR))
                .ForMember(dest => dest.VehicleModel, opt => opt.MapFrom(src => src.VEHICLE_MODEL))
                .ForMember(dest => dest.VendorId, opt => opt.MapFrom(src => src.VENDOR_ID))
                .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.VENDOR_NAME))
                .ForMember(dest => dest.ProgressDetails, opt => opt.MapFrom(src => AutoMapper.Mapper.Map<List<TraCafProgressDto>>(src.TRA_CAF_PROGRESS)))

                ;

            AutoMapper.Mapper.CreateMap<TraCafDto, TRA_CAF>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TRA_CAF_ID, opt => opt.MapFrom(src => src.TraCafId))
                .ForMember(dest => dest.AREA, opt => opt.MapFrom(src => src.Area))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.DOCUMENT_NUMBER, opt => opt.MapFrom(src => src.DocumentNumber))
                .ForMember(dest => dest.DOCUMENT_STATUS, opt => opt.MapFrom(src => src.DocumentStatus))
                .ForMember(dest => dest.EMPLOYEE_ID, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.EMPLOYEE_NAME, opt => opt.MapFrom(src => src.EmployeeName))
                .ForMember(dest => dest.INCIDENT_DATE, opt => opt.MapFrom(src => src.IncidentDate))
                .ForMember(dest => dest.INCIDENT_DESCRIPTION, opt => opt.MapFrom(src => src.IncidentDescription))
                .ForMember(dest => dest.INCIDENT_LOCATION, opt => opt.MapFrom(src => src.IncidentLocation))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.POLICE_NUMBER, opt => opt.MapFrom(src => src.PoliceNumber))
                .ForMember(dest => dest.REMARK, opt => opt.MapFrom(src => src.RemarkId))
                .ForMember(dest => dest.SIRS_NUMBER, opt => opt.MapFrom(src => src.SirsNumber))
                .ForMember(dest => dest.SUPERVISOR, opt => opt.MapFrom(src => src.Supervisor))
                .ForMember(dest => dest.VEHICLE_MODEL, opt => opt.MapFrom(src => src.VehicleModel))
                .ForMember(dest => dest.VENDOR_ID, opt => opt.MapFrom(src => src.VendorId))
                .ForMember(dest => dest.VENDOR_NAME, opt => opt.MapFrom(src => src.VendorName))

                ;

            AutoMapper.Mapper.CreateMap<TraCafProgressDto, TRA_CAF_PROGRESS>().IgnoreAllNonExisting()
                .ForMember(dest=> dest.TRA_CAF_ID, opt=> opt.MapFrom(src=> src.TraCafId))
                .ForMember(dest => dest.ACTUAL, opt => opt.MapFrom(src => src.Actual))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.ESTIMATION, opt => opt.MapFrom(src => src.Estimation))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.PROGRESS_DATE, opt => opt.MapFrom(src => src.ProgressDate))
                .ForMember(dest => dest.REMARK, opt => opt.MapFrom(src => src.Remark))
                .ForMember(dest => dest.STATUS_ID, opt => opt.MapFrom(src => src.StatusId))
                ;
            AutoMapper.Mapper.CreateMap<TRA_CAF_PROGRESS, TraCafProgressDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TraCafId, opt => opt.MapFrom(src => src.TRA_CAF_ID))
                .ForMember(dest => dest.Actual, opt => opt.MapFrom(src => src.ACTUAL))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.Estimation, opt => opt.MapFrom(src => src.ESTIMATION))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.ProgressDate, opt => opt.MapFrom(src => src.PROGRESS_DATE))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.REMARK))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.STATUS_ID))
                ;
        }
    }
}
