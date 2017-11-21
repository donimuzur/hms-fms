using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;

namespace FMS.BLL.Mapper
{
    public class CsfMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<TRA_CSF, TraCsfDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.REASON_ID, opt => opt.MapFrom(src => src.REASON))
                .ForMember(dest => dest.REASON_NAME, opt => opt.MapFrom(src => src.MST_REASON.REASON));

            AutoMapper.Mapper.CreateMap<TraCsfDto, TRA_CSF>().IgnoreAllNonExisting()
                .ForMember(dest => dest.REASON, opt => opt.MapFrom(src => src.REASON_ID));

            AutoMapper.Mapper.CreateMap<EpafDto, TraCsfDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.EMPLOYEE_ID, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.EMPLOYEE_NAME, opt => opt.MapFrom(src => src.EmployeeName))
                .ForMember(dest => dest.EFFECTIVE_DATE, opt => opt.MapFrom(src => src.EfectiveDate))
                .ForMember(dest => dest.EPAF_ID, opt => opt.MapFrom(src => src.MstEpafId))
                .ForMember(dest => dest.COST_CENTER, opt => opt.MapFrom(src => src.CostCenter))
                .ForMember(dest => dest.GROUP_LEVEL, opt => opt.MapFrom(src => src.GroupLevel))
                ;

            AutoMapper.Mapper.CreateMap<CsfWorkflowDocumentInput, WorkflowHistoryDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.FORM_ID, opt => opt.MapFrom(src => src.DocumentId))
                .ForMember(dest => dest.ACTION_BY, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ACTION, opt => opt.MapFrom(src => src.ActionType))
                ;

            AutoMapper.Mapper.CreateMap<TRA_CSF, MST_FLEET>().IgnoreAllNonExisting()
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
                .ForMember(dest => dest.PURPOSE, opt => opt.MapFrom(src => src.VENDOR_PURPOSE))
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
