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
        }
    }
}
