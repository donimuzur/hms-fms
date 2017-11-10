using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
namespace FMS.BLL.Mapper
{
    public class CsfMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<TRA_CSF, TraCsfDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.REASON_ID, opt => opt.MapFrom(src => src.REASON));

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
        }
    }
}
