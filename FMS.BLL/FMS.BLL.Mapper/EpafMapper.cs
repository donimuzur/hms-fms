using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;

namespace FMS.BLL.Mapper
{
    public class EpafMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<MST_EPAF, EpafDto>().IgnoreAllNonExisting()
               .ForMember(dest => dest.MstEpafId, opt => opt.MapFrom(src => src.MST_EPAF_ID))
               .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.DOCUMENT_TYPE))
               .ForMember(dest => dest.EpafAction, opt => opt.MapFrom(src => src.EPAF_ACTION))
               .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EMPLOYEE_ID))
               .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EMPLOYEE_NAME))
               .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.COST_CENTER))
               .ForMember(dest => dest.EfectiveDate, opt => opt.MapFrom(src => src.EFFECTIVE_DATE))
               .ForMember(dest => dest.GroupLevel, opt => opt.MapFrom(src => src.GROUP_LEVEL))
               .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.CITY))
               .ForMember(dest => dest.BaseTown, opt => opt.MapFrom(src => src.BASE_TOWN))
               .ForMember(dest => dest.Expat, opt => opt.MapFrom(src => src.EXPAT))
               .ForMember(dest => dest.LetterSend, opt => opt.MapFrom(src => src.LETTER_SEND))
               .ForMember(dest => dest.LastUpdate, opt => opt.MapFrom(src => src.LAST_UPDATED))
               .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
               .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
               .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
               .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
               .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE));

            AutoMapper.Mapper.CreateMap<EpafDto, MST_EPAF>().IgnoreAllNonExisting()
               .ForMember(dest => dest.MST_EPAF_ID, opt => opt.MapFrom(src => src.MstEpafId))
               .ForMember(dest => dest.DOCUMENT_TYPE, opt => opt.MapFrom(src => src.DocumentType))
               .ForMember(dest => dest.EPAF_ACTION, opt => opt.MapFrom(src => src.EpafAction))
               .ForMember(dest => dest.EMPLOYEE_ID, opt => opt.MapFrom(src => src.EmployeeId))
               .ForMember(dest => dest.EMPLOYEE_NAME, opt => opt.MapFrom(src => src.EmployeeName))
               .ForMember(dest => dest.COST_CENTER, opt => opt.MapFrom(src => src.CostCenter))
               .ForMember(dest => dest.EFFECTIVE_DATE, opt => opt.MapFrom(src => src.EfectiveDate))
               .ForMember(dest => dest.GROUP_LEVEL, opt => opt.MapFrom(src => src.GroupLevel))
               .ForMember(dest => dest.CITY, opt => opt.MapFrom(src => src.City))
               .ForMember(dest => dest.BASE_TOWN, opt => opt.MapFrom(src => src.BaseTown))
               .ForMember(dest => dest.EXPAT, opt => opt.MapFrom(src => src.Expat))
               .ForMember(dest => dest.LETTER_SEND, opt => opt.MapFrom(src => src.LetterSend))
               .ForMember(dest => dest.LAST_UPDATED, opt => opt.MapFrom(src => src.LastUpdate))
               .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
               .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
               .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
               .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
               .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));
        }
    }
}
