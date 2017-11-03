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
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE));

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
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));
        }
    }
}
