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

            AutoMapper.Mapper.CreateMap<NO_OF_VEHICLE_MAKE_TYPE_REPORT_DATA, NoVehicleMakeDto>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<NoVehicleMakeDto, NO_OF_VEHICLE_MAKE_TYPE_REPORT_DATA>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<ODOMETER_REPORT_DATA, OdometerDto>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<OdometerDto, ODOMETER_REPORT_DATA>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<LITER_BY_FUNC_REPORT_DATA, LiterByFunctionDto>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<LiterByFunctionDto, LITER_BY_FUNC_REPORT_DATA>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<FUEL_COST_BY_FUNC_REPORT_DATA, FuelCostByFunctionDto>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<FuelCostByFunctionDto, FUEL_COST_BY_FUNC_REPORT_DATA>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<LEASE_COST_BY_FUNC_REPORT_DATA, LeaseCostByFunctionDto>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<LeaseCostByFunctionDto, LEASE_COST_BY_FUNC_REPORT_DATA>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<SALES_BY_REGION_REPORT_DATA, SalesByRegionDto>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<SalesByRegionDto, SALES_BY_REGION_REPORT_DATA>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<ACCIDENT_REPORT_DATA, AccidentDto>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<AccidentDto, ACCIDENT_REPORT_DATA>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<AC_VS_OB_REPORT_DATA, AcVsObDto>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<AcVsObDto, AC_VS_OB_REPORT_DATA>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<SUM_REPORT_DATA, SumPtdByFunctionDto>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<SumPtdByFunctionDto, SUM_REPORT_DATA>().IgnoreAllNonExisting();
        }
    }
}
