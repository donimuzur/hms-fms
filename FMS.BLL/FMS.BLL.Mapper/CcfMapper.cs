using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BLL.Mapper
{
    public class CcfMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<TRA_CCF, TraCcfDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TraCcfId, opt => opt.MapFrom(src => src.TRA_CCF_ID))
                .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.DOCUMENT_NUMBER))
                .ForMember(dest => dest.DocumentStatus, opt => opt.MapFrom(src => src.DOCUMENT_STATUS))
                .ForMember(dest => dest.ComplaintCategory, opt => opt.MapFrom(src => src.COMPLAINT_CATEGORY))
                .ForMember(dest => dest.EmployeeID, opt => opt.MapFrom(src => src.EMPLOYEE_ID))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EMPLOYEE_NAME))
                .ForMember(dest => dest.EmployeeIdComplaintFor, opt => opt.MapFrom(src => src.EMPLOYEE_ID_COMPLAINT_FOR))
                .ForMember(dest => dest.EmployeeNameComplaintFor, opt => opt.MapFrom(src => src.EMPLOYEE_NAME_COMPLAINT_FOR))
                .ForMember(dest => dest.PoliceNumber, opt => opt.MapFrom(src => src.POLICE_NUMBER))
                .ForMember(dest => dest.PoliceNumberGS, opt => opt.MapFrom(src => src.POLICE_NUMBER_GS))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE))
                .ForMember(dest => dest.LocationCity, opt => opt.MapFrom(src => src.LOCATION_CITY))
                .ForMember(dest => dest.LocationAddress, opt => opt.MapFrom(src => src.LOCATION_ADDRESS))
                .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VEHICLE_TYPE))
                .ForMember(dest => dest.VehicleUsage, opt => opt.MapFrom(src => src.VEHICLE_USAGE))
                .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom(src => src.MANUFACTURE))
                .ForMember(dest => dest.Models, opt => opt.MapFrom(src => src.MODEL))
                .ForMember(dest => dest.Series, opt => opt.MapFrom(src => src.SERIES))
                .ForMember(dest => dest.Vendor, opt => opt.MapFrom(src => src.VENDOR))
                .ForMember(dest => dest.StartPeriod, opt => opt.MapFrom(src => src.START_PERIOD))
                .ForMember(dest => dest.EndPeriod, opt => opt.MapFrom(src => src.END_PERIOD))
                .ForMember(dest => dest.ComplaintCategoryName, opt => opt.MapFrom(src => src.MST_COMPLAINT_CATEGORY.CATEGORY_NAME))
                .ForMember(dest => dest.ComplaintCategoryRole, opt => opt.MapFrom(src => src.MST_COMPLAINT_CATEGORY.ROLE_TYPE))
                ;

            AutoMapper.Mapper.CreateMap<TraCcfDto, TRA_CCF>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TRA_CCF_ID, opt => opt.MapFrom(src => src.TraCcfId))
                .ForMember(dest => dest.DOCUMENT_NUMBER, opt => opt.MapFrom(src => src.DocumentNumber))
                .ForMember(dest => dest.DOCUMENT_STATUS, opt => opt.MapFrom(src => src.DocumentStatus))
                .ForMember(dest => dest.COMPLAINT_CATEGORY, opt => opt.MapFrom(src => src.ComplaintCategory))
                .ForMember(dest => dest.EMPLOYEE_ID, opt => opt.MapFrom(src => src.EmployeeID))
                .ForMember(dest => dest.EMPLOYEE_NAME, opt => opt.MapFrom(src => src.EmployeeName))
                .ForMember(dest => dest.EMPLOYEE_ID_COMPLAINT_FOR, opt => opt.MapFrom(src => src.EmployeeIdComplaintFor))
                .ForMember(dest => dest.EMPLOYEE_NAME_COMPLAINT_FOR, opt => opt.MapFrom(src => src.EmployeeNameComplaintFor))
                .ForMember(dest => dest.POLICE_NUMBER, opt => opt.MapFrom(src => src.PoliceNumber))
                .ForMember(dest => dest.POLICE_NUMBER_GS, opt => opt.MapFrom(src => src.PoliceNumberGS))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.LOCATION_CITY, opt => opt.MapFrom(src => src.LocationCity))
                .ForMember(dest => dest.LOCATION_ADDRESS, opt => opt.MapFrom(src => src.LocationAddress))
                .ForMember(dest => dest.VEHICLE_TYPE, opt => opt.MapFrom(src => src.VehicleType))
                .ForMember(dest => dest.VEHICLE_USAGE, opt => opt.MapFrom(src => src.VehicleUsage))
                .ForMember(dest => dest.MANUFACTURE, opt => opt.MapFrom(src => src.Manufacturer))
                .ForMember(dest => dest.MODEL, opt => opt.MapFrom(src => src.Models))
                .ForMember(dest => dest.SERIES, opt => opt.MapFrom(src => src.Series))
                .ForMember(dest => dest.VENDOR, opt => opt.MapFrom(src => src.Vendor))
                .ForMember(dest => dest.START_PERIOD, opt => opt.MapFrom(src => src.StartPeriod))
                .ForMember(dest => dest.END_PERIOD, opt => opt.MapFrom(src => src.EndPeriod))
                ;

            AutoMapper.Mapper.CreateMap<TraCcfDto, TRA_CCF_DETAIL>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TRA_CCF_DETAIL_ID, opt => opt.MapFrom(src => src.TraCcfDetilId))
                .ForMember(dest => dest.TRA_CCF_ID, opt => opt.MapFrom(src => src.TraCcfId))
                .ForMember(dest => dest.COMPLAINT_DATE, opt => opt.MapFrom(src => src.ComplaintDate))
                .ForMember(dest => dest.COMPLAINT_NOTE, opt => opt.MapFrom(src => src.ComplaintNote))
                .ForMember(dest => dest.COMPLAINT_ATT, opt => opt.MapFrom(src => src.ComplaintAtt))
                .ForMember(dest => dest.COORDINATOR_RESPONSE_DATE, opt => opt.MapFrom(src => src.CoodinatorResponseDate))
                .ForMember(dest => dest.COORDINATOR_NOTE, opt => opt.MapFrom(src => src.CoodinatorNote))
                .ForMember(dest => dest.COORDINATOR_PROMISED_DATE, opt => opt.MapFrom(src => src.CoodinatorPromiseDate))
                .ForMember(dest => dest.COORDINATOR_ATT, opt => opt.MapFrom(src => src.CoodinatorAtt))
                .ForMember(dest => dest.VENDOR_RESPONSE_DATE, opt => opt.MapFrom(src => src.VendorResponseDate))
                .ForMember(dest => dest.VENDOR_NOTE, opt => opt.MapFrom(src => src.VendorNote))
                .ForMember(dest => dest.VENDOR_PROMISED_DATE, opt => opt.MapFrom(src => src.VendorPromiseDate))
                .ForMember(dest => dest.VENDOR_ATT, opt => opt.MapFrom(src => src.VendorAtt))
                ;

            AutoMapper.Mapper.CreateMap<TRA_CCF_DETAIL, TraCcfDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TraCcfDetilId, opt => opt.MapFrom(src => src.TRA_CCF_DETAIL_ID))
                .ForMember(dest => dest.TraCcfId, opt => opt.MapFrom(src => src.TRA_CCF_ID))
                .ForMember(dest => dest.ComplaintDate, opt => opt.MapFrom(src => src.COMPLAINT_DATE))
                .ForMember(dest => dest.ComplaintNote, opt => opt.MapFrom(src => src.COMPLAINT_NOTE))
                .ForMember(dest => dest.ComplaintAtt, opt => opt.MapFrom(src => src.COMPLAINT_ATT))
                .ForMember(dest => dest.CoodinatorResponseDate, opt => opt.MapFrom(src => src.COORDINATOR_RESPONSE_DATE))
                .ForMember(dest => dest.CoodinatorNote, opt => opt.MapFrom(src => src.COORDINATOR_NOTE))
                .ForMember(dest => dest.CoodinatorPromiseDate, opt => opt.MapFrom(src => src.COORDINATOR_PROMISED_DATE))
                .ForMember(dest => dest.CoodinatorAtt, opt => opt.MapFrom(src => src.COORDINATOR_ATT))
                .ForMember(dest => dest.VendorResponseDate, opt => opt.MapFrom(src => src.VENDOR_RESPONSE_DATE))
                .ForMember(dest => dest.VendorNote, opt => opt.MapFrom(src => src.VENDOR_NOTE))
                .ForMember(dest => dest.VendorPromiseDate, opt => opt.MapFrom(src => src.VENDOR_PROMISED_DATE))
                .ForMember(dest => dest.VendorAtt, opt => opt.MapFrom(src => src.VENDOR_ATT))
                ;

            AutoMapper.Mapper.CreateMap<WorkflowHistoryDto, CcfWorkflowDocumentInput>().IgnoreAllNonExisting()
             .ForMember(dest => dest.ActionType, opt => opt.MapFrom(src => src.ACTION))
             .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.ACTION_BY))
             .ForMember(dest => dest.DocumentId, opt => opt.MapFrom(src => src.FORM_ID));

            AutoMapper.Mapper.CreateMap<CcfWorkflowDocumentInput, WorkflowHistoryDto>().IgnoreAllNonExisting()
             .ForMember(dest => dest.ACTION, opt => opt.MapFrom(src => src.ActionType))
             .ForMember(dest => dest.ACTION_BY, opt => opt.MapFrom(src => src.UserId))
             .ForMember(dest => dest.FORM_ID, opt => opt.MapFrom(src => src.DocumentId));
            //AutoMapper.Mapper.CreateMap<MST_COMPLAINT_CATEGORY, TraCcfDto>().IgnoreAllNonExisting()
            //    .ForMember(dest => dest.ComplaintCategoryName, opt => opt.MapFrom(src => src.CATEGORY_NAME));
        }
    }
}
