using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;

namespace FMS.BLL.Mapper
{
    public class ExecutiveSummaryMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<NO_OF_VEHICLE_REPORT_DATA, NoVehicleDto>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<NoVehicleDto, NO_OF_VEHICLE_REPORT_DATA>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<NO_OF_WTC_VEHICLE_REPORT_DATA, NoVehicleWtcDto>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<NoVehicleWtcDto, NO_OF_WTC_VEHICLE_REPORT_DATA>().IgnoreAllNonExisting();
        }
    }
}
