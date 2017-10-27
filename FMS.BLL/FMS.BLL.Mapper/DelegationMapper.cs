using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;

namespace FMS.BLL.Mapper
{
    public class DelegationMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<MST_DELEGATION, DelegationDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MstDelegationID, opt => opt.MapFrom(src => src.MST_DELEGATION_ID))
                .ForMember(dest => dest.EmployeeFrom, opt => opt.MapFrom(src => src.EMPLOYEE_FROM))
                .ForMember(dest => dest.EmployeeTo, opt => opt.MapFrom(src => src.EMPLOYEE_TO))
                .ForMember(dest => dest.DateFrom, opt => opt.MapFrom(src => src.DATE_FROM))
                .ForMember(dest => dest.DateTo, opt => opt.MapFrom(src => src.DATE_TO))
                .ForMember(dest => dest.IsComplaintFrom, opt => opt.MapFrom(src => src.IS_COMPLAINT_FORM))
                .ForMember(dest => dest.Attachment, opt => opt.MapFrom(src => src.ATTACHMENT))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE));

            AutoMapper.Mapper.CreateMap<DelegationDto, MST_DELEGATION>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MST_DELEGATION_ID, opt => opt.MapFrom(src => src.MstDelegationID))
                .ForMember(dest => dest.EMPLOYEE_FROM, opt => opt.MapFrom(src => src.EmployeeFrom))
                .ForMember(dest => dest.EMPLOYEE_TO, opt => opt.MapFrom(src => src.EmployeeTo))
                .ForMember(dest => dest.DATE_FROM, opt => opt.MapFrom(src => src.DateFrom))
                .ForMember(dest => dest.DATE_TO, opt => opt.MapFrom(src => src.DateTo))
                .ForMember(dest => dest.IS_COMPLAINT_FORM, opt => opt.MapFrom(src => src.IsComplaintFrom))
                .ForMember(dest => dest.ATTACHMENT, opt => opt.MapFrom(src => src.Attachment))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));

            AutoMapper.Mapper.CreateMap<MST_EMPLOYEE, DelegationDto>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.NameEmployeeFrom, opt => opt.MapFrom(src => src.FORMAL_NAME))
                 .ForMember(dest => dest.NameEmployeeTo, opt => opt.MapFrom(src => src.FORMAL_NAME));

        }
    }
}
