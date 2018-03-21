using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.DAL.Services;

namespace FMS.BLL.ExecutiveSummary
{
    public class ExecutiveSummaryBLL : IExecutiveSummaryBLL
    {
        private IExecutiveSummaryService _ExecSummService;
        private IUnitOfWork _uow;

        public ExecutiveSummaryBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _ExecSummService = new ExecutiveSummaryService(_uow);
        }

        public List<NoVehicleDto> GetNoOfVehicleData(VehicleGetByParamInput filter)
        {
            var data = _ExecSummService.GetAllNoVehicle(filter);

            var listData = Mapper.Map<List<NoVehicleDto>>(data);

            var groupData = listData.GroupBy(x => new { x.VEHICLE_TYPE, x.SUPPLY_METHOD, x.REGION, x.FUNCTION, x.REPORT_MONTH, x.REPORT_YEAR })
                .Select(p => new NoVehicleDto()
                {
                    VEHICLE_TYPE = p.FirstOrDefault().VEHICLE_TYPE,
                    SUPPLY_METHOD = p.FirstOrDefault().SUPPLY_METHOD,
                    REGION = p.FirstOrDefault().REGION,
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    REPORT_MONTH = p.FirstOrDefault().REPORT_MONTH,
                    REPORT_YEAR = p.FirstOrDefault().REPORT_YEAR,
                    NO_OF_VEHICLE = p.Sum(c => c.NO_OF_VEHICLE)
                }).ToList();

            return groupData;
        }

        public List<NoVehicleWtcDto> GetNoOfVehicleWtcData(VehicleWtcGetByParamInput filter)
        {
            var data = _ExecSummService.GetAllNoVehicleWtc(filter);

            var listData = Mapper.Map<List<NoVehicleWtcDto>>(data);

            var groupData = listData.GroupBy(x => new { x.FUNCTION, x.REGIONAL, x.REPORT_MONTH, x.REPORT_YEAR })
                .Select(p => new NoVehicleWtcDto()
                {
                    REGIONAL = p.FirstOrDefault().REGIONAL,
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    REPORT_MONTH = p.FirstOrDefault().REPORT_MONTH,
                    REPORT_YEAR = p.FirstOrDefault().REPORT_YEAR,
                    NO_OF_VEHICLE = p.Sum(c => c.NO_OF_VEHICLE)
                }).ToList();

            return groupData;
        }

        public List<NoVehicleMakeDto> GetNoOfVehicleMakeData(VehicleMakeGetByParamInput filter)
        {
            var data = _ExecSummService.GetAllNoVehicleMake(filter);

            var listData = Mapper.Map<List<NoVehicleMakeDto>>(data);

            var groupData = listData.GroupBy(x => new { x.MANUFACTURER, x.BODY_TYPE, x.REPORT_MONTH, x.REPORT_YEAR })
                .Select(p => new NoVehicleMakeDto()
                {
                    MANUFACTURER = p.FirstOrDefault().MANUFACTURER,
                    BODY_TYPE = p.FirstOrDefault().BODY_TYPE,
                    REPORT_MONTH = p.FirstOrDefault().REPORT_MONTH,
                    REPORT_YEAR = p.FirstOrDefault().REPORT_YEAR,
                    NO_OF_VEHICLE = p.Sum(c => c.NO_OF_VEHICLE)
                }).ToList();

            return groupData;
        }

        public List<OdometerDto> GetOdometerData(OdometerGetByParamInput filter)
        {
            var data = _ExecSummService.GetAllOdometer(filter);

            var listData = Mapper.Map<List<OdometerDto>>(data);

            var groupData = listData.GroupBy(x => new { x.REGION, x.FUNCTION, x.VEHICLE_TYPE, x.REPORT_MONTH, x.REPORT_YEAR })
                .Select(p => new OdometerDto()
                {
                    REGION = p.FirstOrDefault().REGION,
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    VEHICLE_TYPE = p.FirstOrDefault().VEHICLE_TYPE,
                    REPORT_MONTH = p.FirstOrDefault().REPORT_MONTH,
                    REPORT_YEAR = p.FirstOrDefault().REPORT_YEAR,
                    TOTAL_KM = p.Sum(c => c.TOTAL_KM)
                }).ToList();

            return groupData;
        }

        public List<LiterByFunctionDto> GetLiterByFunctionData(LiterFuncGetByParamInput filter)
        {
            var data = _ExecSummService.GetAllLiterByFunction(filter);

            var listData = Mapper.Map<List<LiterByFunctionDto>>(data);

            var groupData = listData.GroupBy(x => new { x.REGION, x.FUNCTION, x.VEHICLE_TYPE, x.REPORT_MONTH, x.REPORT_YEAR })
                .Select(p => new LiterByFunctionDto()
                {
                    REGION = p.FirstOrDefault().REGION,
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    VEHICLE_TYPE = p.FirstOrDefault().VEHICLE_TYPE,
                    REPORT_MONTH = p.FirstOrDefault().REPORT_MONTH,
                    REPORT_YEAR = p.FirstOrDefault().REPORT_YEAR,
                    TOTAL_LITER = p.Sum(c => c.TOTAL_LITER)
                }).ToList();

            return groupData;
        }

        public List<FuelCostByFunctionDto> GetFuelCostByFunctionData(FuelCostFuncGetByParamInput filter)
        {
            var data = _ExecSummService.GetAllFuelCostByFunction(filter);

            var listData = Mapper.Map<List<FuelCostByFunctionDto>>(data);

            var groupData = listData.GroupBy(x => new { x.REGION, x.FUNCTION, x.VEHICLE_TYPE, x.REPORT_MONTH, x.REPORT_YEAR })
                .Select(p => new FuelCostByFunctionDto()
                {
                    REGION = p.FirstOrDefault().REGION,
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    VEHICLE_TYPE = p.FirstOrDefault().VEHICLE_TYPE,
                    REPORT_MONTH = p.FirstOrDefault().REPORT_MONTH,
                    REPORT_YEAR = p.FirstOrDefault().REPORT_YEAR,
                    TOTAL_FUEL_COST = p.Sum(c => c.TOTAL_FUEL_COST)
                }).ToList();

            return groupData;
        }

        public List<LeaseCostByFunctionDto> GetLeaseCostByFunctionData(LeaseCostFuncGetByParamInput filter)
        {
            var data = _ExecSummService.GetAllLeaseCostByFunction(filter);

            var listData = Mapper.Map<List<LeaseCostByFunctionDto>>(data);

            var groupData = listData.GroupBy(x => new { x.REGION, x.FUNCTION, x.REPORT_MONTH, x.REPORT_YEAR })
                .Select(p => new LeaseCostByFunctionDto()
                {
                    REGION = p.FirstOrDefault().REGION,
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    REPORT_MONTH = p.FirstOrDefault().REPORT_MONTH,
                    REPORT_YEAR = p.FirstOrDefault().REPORT_YEAR,
                    TOTAL_LEASE_COST = p.Sum(c => c.TOTAL_LEASE_COST)
                }).ToList();

            return groupData;
        }

        public List<SalesByRegionDto> GetSalesByRegionData(SalesRegionGetByParamInput filter)
        {
            var data = _ExecSummService.GetAllSalesByRegion(filter);

            var listData = Mapper.Map<List<SalesByRegionDto>>(data);

            var groupData = listData.GroupBy(x => new { x.REGION, x.REPORT_MONTH, x.REPORT_YEAR })
                .Select(p => new SalesByRegionDto()
                {
                    REGION = p.FirstOrDefault().REGION,
                    REPORT_MONTH = p.FirstOrDefault().REPORT_MONTH,
                    REPORT_YEAR = p.FirstOrDefault().REPORT_YEAR,
                    TOTAL_KM = p.Max(c => c.TOTAL_KM),
                    TOTAL_COST = p.Sum(c => c.TOTAL_COST),
                    STICK = p.Sum(c => c.STICK)
                }).ToList();

            return groupData;
        }

        public List<AccidentDto> GetAccidentData(AccidentGetByParamInput filter)
        {
            var data = _ExecSummService.GetAllAccident(filter);

            var listData = Mapper.Map<List<AccidentDto>>(data);

            var groupData = listData.GroupBy(x => new { x.REGION, x.FUNCTION, x.VEHICLE_TYPE, x.REPORT_MONTH, x.REPORT_YEAR })
                .Select(p => new AccidentDto()
                {
                    REGION = p.FirstOrDefault().REGION,
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    VEHICLE_TYPE = p.FirstOrDefault().VEHICLE_TYPE,
                    REPORT_MONTH = p.FirstOrDefault().REPORT_MONTH,
                    REPORT_YEAR = p.FirstOrDefault().REPORT_YEAR,
                    ACCIDENT_COUNT = p.Sum(c => c.ACCIDENT_COUNT)
                }).ToList();

            return groupData;
        }

        public List<AcVsObDto> GetAcVsObData(AcVsObGetByParamInput filter)
        {
            var data = _ExecSummService.GetAllAcVsOb(filter);

            var listData = Mapper.Map<List<AcVsObDto>>(data);

            var groupData = listData.GroupBy(x => new { x.FUNCTION, x.VEHICLE_TYPE, x.REPORT_MONTH, x.REPORT_YEAR })
                .Select(p => new AcVsObDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    VEHICLE_TYPE = p.FirstOrDefault().VEHICLE_TYPE,
                    REPORT_MONTH = p.FirstOrDefault().REPORT_MONTH,
                    REPORT_YEAR = p.FirstOrDefault().REPORT_YEAR,
                    UNIT = p.Sum(c => c.UNIT),
                    ACTUAL_COST = p.Sum(c => c.ACTUAL_COST),
                    COST_OB = p.Sum(c => c.COST_OB)
                }).ToList();

            return groupData;
        }

        public List<SumPtdByFunctionDto> GetSumPtdByFunctionData(SumPtdFuncGetByParamInput filter)
        {
            var data = _ExecSummService.GetAllSumPtdByFunction(filter);

            var listData = Mapper.Map<List<SumPtdByFunctionDto>>(data);

            var groupData = listData.GroupBy(x => new { x.REGION, x.FUNCTION, x.VEHICLE_TYPE, x.REPORT_MONTH, x.REPORT_YEAR })
                .Select(p => new SumPtdByFunctionDto()
                {
                    REGION = p.FirstOrDefault().REGION,
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    VEHICLE_TYPE = p.FirstOrDefault().VEHICLE_TYPE,
                    REPORT_MONTH = p.FirstOrDefault().REPORT_MONTH,
                    REPORT_YEAR = p.FirstOrDefault().REPORT_YEAR,
                    TOTAL_VEHICLE = p.Sum(c => c.TOTAL_VEHICLE),
                    TOTAL_VEHICLE_COST = p.Sum(c => c.TOTAL_VEHICLE_COST),
                    TOTAL_FUEL_AMOUNT = p.Sum(c => c.TOTAL_FUEL_AMOUNT),
                    TOTAL_FUEL_COST = p.Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_KM = p.Max(c => c.TOTAL_KM),
                    TOTAL_OPERATIONAL_COST = p.Sum(c => c.TOTAL_OPERATIONAL_COST),
                    ACCIDENT_COUNT = p.Sum(c => c.ACCIDENT_COUNT)
                }).ToList();

            return groupData;
        }
    }
}
