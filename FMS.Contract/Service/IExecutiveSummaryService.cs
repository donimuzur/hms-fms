using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.Service
{
    public interface IExecutiveSummaryService
    {
        List<NO_OF_VEHICLE_REPORT_DATA> GetAllNoVehicle(VehicleGetByParamInput filter);
        List<NO_OF_WTC_VEHICLE_REPORT_DATA> GetAllNoVehicleWtc(VehicleWtcGetByParamInput filter);
        List<NO_OF_VEHICLE_MAKE_TYPE_REPORT_DATA> GetAllNoVehicleMake(VehicleMakeGetByParamInput filter);
        List<ODOMETER_REPORT_DATA> GetAllOdometer(OdometerGetByParamInput filter);
        List<LITER_BY_FUNC_REPORT_DATA> GetAllLiterByFunction(LiterFuncGetByParamInput filter);
        List<FUEL_COST_BY_FUNC_REPORT_DATA> GetAllFuelCostByFunction(FuelCostFuncGetByParamInput filter);
        List<LEASE_COST_BY_FUNC_REPORT_DATA> GetAllLeaseCostByFunction(LeaseCostFuncGetByParamInput filter);
    }
}
