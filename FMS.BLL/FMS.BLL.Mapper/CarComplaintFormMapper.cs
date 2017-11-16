using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;

namespace FMS.BLL.Mapper
{
    public class CarComplaintFormMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<TRA_CCF, CarComplaintFormDto>().IgnoreAllNonExisting()
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
                .ForMember(dest => dest.EndPeriod, opt => opt.MapFrom(src => src.END_PERIOD));

            AutoMapper.Mapper.CreateMap<CarComplaintFormDto, TRA_CCF>().IgnoreAllNonExisting()
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
                .ForMember(dest => dest.END_PERIOD, opt => opt.MapFrom(src => src.EndPeriod));

            AutoMapper.Mapper.CreateMap<TRA_CCF_DETAIL, CarComplaintFormDtoDetil>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TraCcfDetailId, opt => opt.MapFrom(src => src.TRA_CCF_DETAIL_ID))
                .ForMember(dest => dest.TraCcfId, opt => opt.MapFrom(src => src.TRA_CCF_ID))
                .ForMember(dest => dest.ComplaintDate, opt => opt.MapFrom(src => src.COMPLAINT_DATE))
                .ForMember(dest => dest.ComplaintNote, opt => opt.MapFrom(src => src.COMPLAINT_NOTE))
                .ForMember(dest => dest.ComplaintAtt, opt => opt.MapFrom(src => src.COMPLAINT_ATT))
                .ForMember(dest => dest.CoorResponseDate, opt => opt.MapFrom(src => src.COORDINATOR_RESPONSE_DATE))
                .ForMember(dest => dest.CoorNote, opt => opt.MapFrom(src => src.COORDINATOR_NOTE))
                .ForMember(dest => dest.CoorPromiseDate, opt => opt.MapFrom(src => src.COORDINATOR_PROMISED_DATE))
                .ForMember(dest => dest.CoorAtt, opt => opt.MapFrom(src => src.COORDINATOR_ATT))
                .ForMember(dest => dest.VendorResponseDate, opt => opt.MapFrom(src => src.VENDOR_RESPONSE_DATE))
                .ForMember(dest => dest.VendorNote, opt => opt.MapFrom(src => src.VENDOR_NOTE))
                .ForMember(dest => dest.VendorPromiseDate, opt => opt.MapFrom(src => src.VENDOR_PROMISED_DATE))
                .ForMember(dest => dest.VendorAtt, opt => opt.MapFrom(src => src.VENDOR_ATT))
                ;

            AutoMapper.Mapper.CreateMap<CarComplaintFormDtoDetil, TRA_CCF_DETAIL>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TRA_CCF_DETAIL_ID, opt => opt.MapFrom(src => src.TraCcfDetailId))
                .ForMember(dest => dest.TRA_CCF_ID, opt => opt.MapFrom(src => src.TraCcfId))
                .ForMember(dest => dest.COMPLAINT_DATE, opt => opt.MapFrom(src => src.ComplaintDate))
                .ForMember(dest => dest.COMPLAINT_NOTE, opt => opt.MapFrom(src => src.ComplaintNote))
                .ForMember(dest => dest.COMPLAINT_ATT, opt => opt.MapFrom(src => src.ComplaintAtt))
                .ForMember(dest => dest.COORDINATOR_RESPONSE_DATE, opt => opt.MapFrom(src => src.CoorResponseDate))
                .ForMember(dest => dest.COORDINATOR_NOTE, opt => opt.MapFrom(src => src.CoorNote))
                .ForMember(dest => dest.COORDINATOR_PROMISED_DATE, opt => opt.MapFrom(src => src.CoorPromiseDate))
                .ForMember(dest => dest.COORDINATOR_ATT, opt => opt.MapFrom(src => src.CoorAtt))
                .ForMember(dest => dest.VENDOR_RESPONSE_DATE, opt => opt.MapFrom(src => src.VendorResponseDate))
                .ForMember(dest => dest.VENDOR_NOTE, opt => opt.MapFrom(src => src.VendorNote))
                .ForMember(dest => dest.VENDOR_PROMISED_DATE, opt => opt.MapFrom(src => src.VendorPromiseDate))
                .ForMember(dest => dest.VENDOR_ATT, opt => opt.MapFrom(src => src.VendorAtt))
                ;
        }
    }
}
