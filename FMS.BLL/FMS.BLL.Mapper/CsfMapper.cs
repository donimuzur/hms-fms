using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
namespace FMS.BLL.Mapper
{
    public class CsfMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<TRA_CSF, TraCsfDto>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<TraCsfDto, TRA_CSF>().IgnoreAllNonExisting();
        }
    }
}
