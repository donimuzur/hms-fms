using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;

namespace FMS.BLL.Mapper
{
    public class TemporaryMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<TemporaryDto, TRA_TEMPORARY>().IgnoreAllNonExisting()
                .ForMember(dest => dest.REASON, opt => opt.MapFrom(src => src.REASON_ID))
                .ForMember(dest => dest.DOCUMENT_NUMBER, opt => opt.MapFrom(src => src.DOCUMENT_NUMBER_TEMP));

            AutoMapper.Mapper.CreateMap<TRA_TEMPORARY, TemporaryDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.REASON_ID, opt => opt.MapFrom(src => src.REASON))
                .ForMember(dest => dest.REASON_NAME, opt => opt.MapFrom(src => src.MST_REASON.REASON))
                .ForMember(dest => dest.DOCUMENT_NUMBER_TEMP, opt => opt.MapFrom(src => src.DOCUMENT_NUMBER));

            AutoMapper.Mapper.CreateMap<TraCsfDto, TemporaryDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.DOCUMENT_NUMBER_RELATED, opt => opt.MapFrom(src => src.DOCUMENT_NUMBER));

            AutoMapper.Mapper.CreateMap<TraCrfDto, TemporaryDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.DOCUMENT_NUMBER_RELATED, opt => opt.MapFrom(src => src.DOCUMENT_NUMBER));

            AutoMapper.Mapper.CreateMap<TempWorkflowDocumentInput, WorkflowHistoryDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.FORM_ID, opt => opt.MapFrom(src => src.DocumentId))
                .ForMember(dest => dest.ACTION_BY, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ACTION, opt => opt.MapFrom(src => src.ActionType))
                ;
        }
    }
}
