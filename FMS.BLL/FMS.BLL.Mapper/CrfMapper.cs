using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;

namespace FMS.BLL.Mapper
{
    public class CrfMapper
    {
        public static void Initialize()
        {
            

            AutoMapper.Mapper.CreateMap<EpafDto, TraCrfDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.EMPLOYEE_ID, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.EMPLOYEE_NAME, opt => opt.MapFrom(src => src.EmployeeName))
                .ForMember(dest => dest.EFFECTIVE_DATE, opt => opt.MapFrom(src => src.EfectiveDate))
                .ForMember(dest => dest.EPAF_ID, opt => opt.MapFrom(src => src.MstEpafId))
                .ForMember(dest => dest.COST_CENTER, opt => opt.MapFrom(src => src.CostCenter))
                .ForMember(dest => dest.LOCATION_CITY, opt=> opt.MapFrom(src=> src.City))
                ;

            AutoMapper.Mapper.CreateMap<TRA_CRF, TraCrfDto>().IgnoreAllNonExisting()
                ;

            AutoMapper.Mapper.CreateMap<TraCrfDto, TRA_CRF>().IgnoreAllNonExisting()
                ;
        }
    }
}
