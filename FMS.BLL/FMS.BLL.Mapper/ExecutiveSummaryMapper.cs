﻿using FMS.AutoMapperExtensions;
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

            AutoMapper.Mapper.CreateMap<NO_OF_VEHICLE_MAKE_TYPE_REPORT_DATA, NoVehicleMakeDto>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<NoVehicleMakeDto, NO_OF_VEHICLE_MAKE_TYPE_REPORT_DATA>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<ODOMETER_REPORT_DATA, OdometerDto>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<OdometerDto, ODOMETER_REPORT_DATA>().IgnoreAllNonExisting();
        }
    }
}
