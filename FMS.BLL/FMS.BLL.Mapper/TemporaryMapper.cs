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
                .ForMember(dest => dest.DOCUMENT_NUMBER_TEMP, opt => opt.MapFrom(src => src.DOCUMENT_NUMBER))
                .ForMember(dest => dest.LOCATION_CITY, opt => opt.MapFrom(src => src.LOCATION_CITY == null ? string.Empty : src.LOCATION_CITY));

            AutoMapper.Mapper.CreateMap<TemporaryDto, ARCH_TRA_TEMPORARY>().IgnoreAllNonExisting()
                .ForMember(dest => dest.REASON, opt => opt.MapFrom(src => src.REASON_ID))
                .ForMember(dest => dest.DOCUMENT_NUMBER, opt => opt.MapFrom(src => src.DOCUMENT_NUMBER_TEMP));

            AutoMapper.Mapper.CreateMap<ARCH_TRA_TEMPORARY, TemporaryDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.REASON_ID, opt => opt.MapFrom(src => src.REASON))
                .ForMember(dest => dest.DOCUMENT_NUMBER_TEMP, opt => opt.MapFrom(src => src.DOCUMENT_NUMBER))
                .ForMember(dest => dest.LOCATION_CITY, opt => opt.MapFrom(src => src.LOCATION_CITY == null ? string.Empty : src.LOCATION_CITY));

            AutoMapper.Mapper.CreateMap<TraCsfDto, TemporaryDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.DOCUMENT_NUMBER_RELATED, opt => opt.MapFrom(src => src.DOCUMENT_NUMBER));

            AutoMapper.Mapper.CreateMap<TraCrfDto, TemporaryDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.DOCUMENT_NUMBER_RELATED, opt => opt.MapFrom(src => src.DOCUMENT_NUMBER));

            AutoMapper.Mapper.CreateMap<TempWorkflowDocumentInput, WorkflowHistoryDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.FORM_ID, opt => opt.MapFrom(src => src.DocumentId))
                .ForMember(dest => dest.ACTION_BY, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ACTION, opt => opt.MapFrom(src => src.ActionType))
                ;

            AutoMapper.Mapper.CreateMap<TRA_TEMPORARY, MST_FLEET>().IgnoreAllNonExisting()
                .ForMember(dest => dest.POLICE_NUMBER, opt => opt.MapFrom(src => src.VENDOR_POLICE_NUMBER))
                .ForMember(dest => dest.CHASIS_NUMBER, opt => opt.MapFrom(src => src.VENDOR_CHASIS_NUMBER))
                .ForMember(dest => dest.ENGINE_NUMBER, opt => opt.MapFrom(src => src.VENDOR_ENGINE_NUMBER))
                .ForMember(dest => dest.VENDOR_NAME, opt => opt.MapFrom(src => src.VENDOR_VENDOR))
                .ForMember(dest => dest.MANUFACTURER, opt => opt.MapFrom(src => src.VENDOR_MANUFACTURER))
                .ForMember(dest => dest.MODEL, opt => opt.MapFrom(src => src.VENDOR_MODEL))
                .ForMember(dest => dest.SERIES, opt => opt.MapFrom(src => src.VENDOR_SERIES))
                .ForMember(dest => dest.BODY_TYPE, opt => opt.MapFrom(src => src.VENDOR_BODY_TYPE))
                .ForMember(dest => dest.COLOR, opt => opt.MapFrom(src => src.VENDOR_COLOUR))
                .ForMember(dest => dest.TRANSMISSION, opt => opt.MapFrom(src => src.VENDOR_TRANSMISSION))
                .ForMember(dest => dest.BRANDING, opt => opt.MapFrom(src => src.VENDOR_BRANDING))
                .ForMember(dest => dest.AIRBAG, opt => opt.MapFrom(src => src.VENDOR_AIR_BAG))
                .ForMember(dest => dest.VEHICLE_YEAR, opt => opt.MapFrom(src => src.CREATED_DATE.Year))
                .ForMember(dest => dest.CITY, opt => opt.MapFrom(src => src.LOCATION_CITY))
                .ForMember(dest => dest.ADDRESS, opt => opt.MapFrom(src => src.LOCATION_ADDRESS))
                .ForMember(dest => dest.VAT, opt => opt.MapFrom(src => src.VENDOR_VAT))
                .ForMember(dest => dest.RESTITUTION, opt => opt.MapFrom(src => src.VENDOR_RESTITUTION))
                .ForMember(dest => dest.PO_NUMBER, opt => opt.MapFrom(src => src.VENDOR_PO_NUMBER))
                .ForMember(dest => dest.PO_LINE, opt => opt.MapFrom(src => src.VENDOR_PO_LINE))
                .ForMember(dest => dest.START_CONTRACT, opt => opt.MapFrom(src => src.VENDOR_CONTRACT_START_DATE))
                .ForMember(dest => dest.END_CONTRACT, opt => opt.MapFrom(src => src.VENDOR_CONTRACT_END_DATE))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => "SYSTEM"))
                ;
        }
    }
}
