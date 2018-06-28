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
                //.ForMember(dest => dest.COST_CENTER, opt => opt.MapFrom(src => src.CostCenter))
                .ForMember(dest => dest.COST_CENTER_NEW, opt => opt.MapFrom(src => src.CostCenter))
                .ForMember(dest => dest.LOCATION_OFFICE_NEW, opt => opt.MapFrom(src => src.BaseTown))
                .ForMember(dest => dest.LOCATION_CITY_NEW, opt => opt.MapFrom(src => src.City))
                //.ForMember(dest => dest.LOCATION_CITY, opt=> opt.MapFrom(src=> src.City))
                .ForMember(dest => dest.TEMPORARY_DELIVERABLE_DATE, opt => opt.MapFrom(src => src.EfectiveDate))
                .ForMember(dest => dest.EXPECTED_DATE, opt => opt.MapFrom(src => src.EfectiveDate))
                //.ForMember(dest => dest.DELIV_CITY, opt => opt.MapFrom(src => src.City))
                //.ForMember(dest => dest.DELIV_ADDRESS, opt => opt.MapFrom(src => src.BaseTown))
                //.ForMember(dest => dest.DELIV_p, opt => opt.MapFrom(src => src.))
                //.ForMember(dest => dest.DELIV_PIC, opt => opt.MapFrom(src => src))

                ;

            AutoMapper.Mapper.CreateMap<MST_EPAF, TraCrfDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.EMPLOYEE_ID, opt => opt.MapFrom(src => src.EMPLOYEE_ID))
                .ForMember(dest => dest.EMPLOYEE_NAME, opt => opt.MapFrom(src => src.EMPLOYEE_NAME))
                .ForMember(dest => dest.EFFECTIVE_DATE, opt => opt.MapFrom(src => src.EFFECTIVE_DATE))
                .ForMember(dest => dest.EPAF_ID, opt => opt.MapFrom(src => src.MST_EPAF_ID))
                .ForMember(dest => dest.COST_CENTER, opt => opt.MapFrom(src => src.COST_CENTER))
                .ForMember(dest => dest.COST_CENTER_NEW, opt => opt.MapFrom(src => src.COST_CENTER))
                .ForMember(dest => dest.LOCATION_OFFICE_NEW, opt => opt.MapFrom(src => src.BASE_TOWN))
                .ForMember(dest => dest.LOCATION_CITY_NEW, opt => opt.MapFrom(src => src.CITY))
                //.ForMember(dest => dest.LOCATION_CITY, opt=> opt.MapFrom(src=> src.City))
                .ForMember(dest => dest.TEMPORARY_DELIVERABLE_DATE, opt => opt.MapFrom(src => src.EFFECTIVE_DATE))
                .ForMember(dest => dest.EXPECTED_DATE, opt => opt.MapFrom(src => src.EFFECTIVE_DATE))
                //.ForMember(dest => dest.DELIV_CITY, opt => opt.MapFrom(src => src.City))
                //.ForMember(dest => dest.DELIV_ADDRESS, opt => opt.MapFrom(src => src.BaseTown))
                //.ForMember(dest => dest.DELIV_p, opt => opt.MapFrom(src => src.))
                //.ForMember(dest => dest.DELIV_PIC, opt => opt.MapFrom(src => src))

                ;

            AutoMapper.Mapper.CreateMap<TRA_CRF, TraCrfDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.RelocationType, opt=> opt.MapFrom(src=> src.RELOCATION_TYPE))
                .ForMember(dest => dest.Body, opt => opt.MapFrom(src => src.BODY_TYPE));

            AutoMapper.Mapper.CreateMap<TraCrfDto, TRA_CRF>().IgnoreAllNonExisting()
                .ForMember(dest => dest.RELOCATION_TYPE, opt => opt.MapFrom(src => src.RelocationType))
                .ForMember(dest => dest.BODY_TYPE, opt => opt.MapFrom(src => src.Body));

            AutoMapper.Mapper.CreateMap<ARCH_TRA_CRF, TraCrfDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.RelocationType, opt => opt.MapFrom(src => src.RELOCATION_TYPE))
                .ForMember(dest => dest.Body, opt => opt.MapFrom(src => src.BODY_TYPE));

            AutoMapper.Mapper.CreateMap<TraCrfDto, ARCH_TRA_CRF>().IgnoreAllNonExisting()
                .ForMember(dest => dest.RELOCATION_TYPE, opt => opt.MapFrom(src => src.RelocationType))
                .ForMember(dest => dest.BODY_TYPE, opt => opt.MapFrom(src => src.Body));

        }
    }
}
