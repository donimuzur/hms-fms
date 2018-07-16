using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Utils;
using FMS.Website.Models;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;
using SpreadsheetLight.Charts;

namespace FMS.Website.Controllers
{
    public class RptExecutiveSummaryController : BaseController
    {
        #region --------- Field and Constructor --------------

        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private IExecutiveSummaryBLL _execSummBLL;
        private ISettingBLL _settingBLL;
        private ILocationMappingBLL _locationMappingBLL;
        private IGroupCostCenterBLL _groupCostCenterBLL;
        private IEmployeeBLL _employeeBLL;


        public RptExecutiveSummaryController(IPageBLL pageBll, IExecutiveSummaryBLL execSummBLL, ISettingBLL settingBLL, ILocationMappingBLL locationMappingBLL,
            IGroupCostCenterBLL groupCostCenterBLL, IEmployeeBLL EmployeeBLL)
            : base(pageBll, Core.Enums.MenuList.RptExecutiveSummary)
        {
            _pageBLL = pageBll;
            _execSummBLL = execSummBLL;
            _settingBLL = settingBLL;
            _locationMappingBLL = locationMappingBLL;
            _groupCostCenterBLL = groupCostCenterBLL;
            _employeeBLL = EmployeeBLL;
            _mainMenu = Enums.MenuList.RptExecutiveSummary;
        }

        #endregion

        #region --------- Set Data -----------------------

        public MultiSelectList GetFunctionsMultiSelectList()
        {
            var items = new List<SelectListItem>()
            {
                new SelectListItem() {Text = "Sales", Value = "Sales" },
                new SelectListItem() {Text = "Marketing", Value = "Marketing" },
                new SelectListItem() {Text = "Operations", Value = "Operations" },
                new SelectListItem() {Text = "Others", Value = "Others" }
            };

            return new MultiSelectList(items, "Value", "Text");
        }

        #endregion

        #region --------- Get Data Json -----------------------

        [HttpPost]
        public JsonResult GetVehicleDataVisual(int monthFrom, int? yearFrom)
        {
            var input = new VehicleGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthFrom;
            input.YearTo = yearFrom == null ? 0 : yearFrom.Value;
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager
                        || CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager
                        || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                input.VehicleType = "WTC";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.HR || CurrentUser.UserRole == Enums.UserRole.HRManager)
            {
                input.VehicleType = "BENEFIT";
            }

            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager)
            {
                input.Function = "Sales,Marketing";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager)
            {
                input.Function = "Operations";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                input.Function = "Logistic";
            }
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
            var LocationMappingList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).ToList();
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.Logistic)
            {
                var GetCity = EmployeeList.Where(x => x.EMPLOYEE_ID == CurrentUser.EMPLOYEE_ID && x.IS_ACTIVE).FirstOrDefault();
                if (GetCity != null)
                {
                    var GetRegion = LocationMappingList.Where(x => (x.Location == null ? "" : x.Location.ToUpper()) == (GetCity.CITY == null ? "" : GetCity.CITY.ToUpper())).FirstOrDefault();
                    if (GetRegion != null)
                    {
                        input.ZoneId = GetRegion.ZoneSales;
                    }
                }
            }

            List<NoVehicleDto> data = _execSummBLL.GetNoOfVehicleData(input);

            var label1 = "BENEFIT (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT").Sum(x => x.NO_OF_VEHICLE) + ")";
            var label2 = "WTC (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC").Sum(x => x.NO_OF_VEHICLE) + ")";

            var groupData = data.GroupBy(x => new { x.FUNCTION })
                .Select(p => new NoVehicleDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    NO_OF_VEHICLE_BENEFIT = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT").Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_WTC = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC").Sum(c => c.NO_OF_VEHICLE),
                    LABEL1 = label1,
                    LABEL2 = label2
                }).OrderBy(x => x.FUNCTION).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult GetVehicleWtcDataVisual(int monthFrom, int? yearFrom)
        {
            var input = new VehicleWtcGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthFrom;
            input.YearTo = yearFrom == null ? 0 : yearFrom.Value;
            input.Function = "Sales,Marketing";
            if (CurrentUser.UserRole == Enums.UserRole.HR || CurrentUser.UserRole == Enums.UserRole.HRManager
                || CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager || CurrentUser.UserRole == Enums.UserRole.LDManager
                 || CurrentUser.UserRole == Enums.UserRole.Logistic)
            {
                input.Function = "---";
            }
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
            var LocationMappingList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).ToList();
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.Logistic)
            {
                var GetCity = EmployeeList.Where(x => x.EMPLOYEE_ID == CurrentUser.EMPLOYEE_ID && x.IS_ACTIVE).FirstOrDefault();
                if (GetCity != null)
                {
                    var GetRegion = LocationMappingList.Where(x => (x.Location == null ? "" : x.Location.ToUpper()) == (GetCity.CITY == null ? "" : GetCity.CITY.ToUpper())).FirstOrDefault();
                    if (GetRegion != null)
                    {
                        input.ZoneId = GetRegion.ZoneSales;
                    }
                }
            }
            List<NoVehicleWtcDto> data = _execSummBLL.GetNoOfVehicleWtcData(input);

            var label1 = "Sales (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES").Sum(x => x.NO_OF_VEHICLE) + ")";
            var label2 = "Marketing (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING").Sum(x => x.NO_OF_VEHICLE) + ")";

            var groupData = data.GroupBy(x => new { x.REGIONAL })
                .Select(p => new NoVehicleWtcDto()
                {
                    REGIONAL = p.FirstOrDefault().REGIONAL,
                    NO_OF_VEHICLE_SALES = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES").Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MARKETING = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING").Sum(c => c.NO_OF_VEHICLE),
                    LABEL1 = label1,
                    LABEL2 = label2
                }).OrderBy(x => x.REGIONAL).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult GetVehicleMakeDataVisual(int monthFrom, int? yearFrom)
        {
            var input = new VehicleMakeGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthFrom;
            input.YearTo = yearFrom == null ? 0 : yearFrom.Value;

            List<NoVehicleMakeDto> data = _execSummBLL.GetNoOfVehicleMakeData(input);

            var label1 = "Total (" + data.Sum(x => x.NO_OF_VEHICLE) + ")";

            var groupData = data.GroupBy(x => new { x.MANUFACTURER })
                .Select(p => new NoVehicleMakeDto()
                {
                    MANUFACTURER = p.FirstOrDefault().MANUFACTURER,
                    NO_OF_VEHICLE = p.Sum(c => c.NO_OF_VEHICLE),
                    LABEL1 = label1
                }).OrderBy(x => x.MANUFACTURER).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult GetVehicleTypeDataVisual(int monthFrom, int? yearFrom)
        {
            var input = new VehicleMakeGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthFrom;
            input.YearTo = yearFrom == null ? 0 : yearFrom.Value;

            List<NoVehicleMakeDto> data = _execSummBLL.GetNoOfVehicleMakeData(input);

            var label1 = "Total (" + data.Sum(x => x.NO_OF_VEHICLE) + ")";

            var groupData = data.GroupBy(x => new { x.BODY_TYPE })
                .Select(p => new NoVehicleMakeDto()
                {
                    BODY_TYPE = p.FirstOrDefault().BODY_TYPE,
                    NO_OF_VEHICLE = p.Sum(c => c.NO_OF_VEHICLE),
                    LABEL1 = label1
                }).OrderBy(x => x.BODY_TYPE).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult GetOdometerDataVisual(int monthFrom, int? yearFrom)
        {
            var input = new OdometerGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthFrom;
            input.YearTo = yearFrom == null ? 0 : yearFrom.Value;
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager
                        || CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager
                        || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                input.VehicleType = "WTC";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.HR || CurrentUser.UserRole == Enums.UserRole.HRManager)
            {
                input.VehicleType = "BENEFIT";
            }
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager)
            {
                input.Function = "Sales,Marketing";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager)
            {
                input.Function = "Operations";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                input.Function = "Logistic";
            }
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
            var LocationMappingList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).ToList();
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.Logistic)
            {
                var GetCity = EmployeeList.Where(x => x.EMPLOYEE_ID == CurrentUser.EMPLOYEE_ID && x.IS_ACTIVE).FirstOrDefault();
                if (GetCity != null)
                {
                    var GetRegion = LocationMappingList.Where(x => (x.Location == null ? "" : x.Location.ToUpper()) == (GetCity.CITY == null ? "" : GetCity.CITY.ToUpper())).FirstOrDefault();
                    if (GetRegion != null)
                    {
                        input.ZoneId = GetRegion.ZoneSales;
                    }
                }
            }
            List<OdometerDto> data = _execSummBLL.GetOdometerData(input);

            var label1 = "BENEFIT (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT").Sum(x => x.TOTAL_KM) + ")";
            var label2 = "WTC (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC").Sum(x => x.TOTAL_KM) + ")";

            var groupData = data.GroupBy(x => new { x.FUNCTION })
                .Select(p => new OdometerDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    TOTAL_KM_BENEFIT = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT").Sum(c => c.TOTAL_KM),
                    TOTAL_KM_WTC = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC").Sum(c => c.TOTAL_KM),
                    LABEL1 = label1,
                    LABEL2 = label2
                }).OrderBy(x => x.FUNCTION).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult GetOdometerDataVisualRegion(int monthFrom, int? yearFrom)
        {
            var input = new OdometerGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthFrom;
            input.VehicleType = "WTC";
            input.Function = "Sales,Marketing";
            List<OdometerDto> data = _execSummBLL.GetOdometerData(input);

            var numb1 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES").Sum(c => c.TOTAL_KM);
            var numb2 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING").Sum(c => c.TOTAL_KM);

            var label1 = "Sales (" + (numb1 == null ? "0" : numb1.Value.ToString("N2")) + ")";
            var label2 = "Marketing (" + (numb2 == null ? "0" : numb2.Value.ToString("N2")) + ")";

            var groupData = data.GroupBy(x => new { x.REGION })
                .Select(p => new OdometerDto()
                {
                    REGION = p.FirstOrDefault().REGION,
                    TOTAL_KM_SALES = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES").Sum(c => c.TOTAL_KM),
                    TOTAL_KM_MARKETING = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING").Sum(c => c.TOTAL_KM),
                    LABEL1 = label1,
                    LABEL2 = label2
                }).OrderBy(x => x.REGION).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult GetLiterByFunctionDataVisual(int monthFrom, int? yearFrom)
        {
            var input = new LiterFuncGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthFrom;
            input.YearTo = yearFrom == null ? 0 : yearFrom.Value;
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager
                        || CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager
                        || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                input.VehicleType = "WTC";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.HR || CurrentUser.UserRole == Enums.UserRole.HRManager)
            {
                input.VehicleType = "BENEFIT";
            }

            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager)
            {
                input.Function = "Sales,Marketing";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager)
            {
                input.Function = "Operations";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                input.Function = "Logistic";
            }
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
            var LocationMappingList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).ToList();
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.Logistic)
            {
                var GetCity = EmployeeList.Where(x => x.EMPLOYEE_ID == CurrentUser.EMPLOYEE_ID && x.IS_ACTIVE).FirstOrDefault();
                if (GetCity != null)
                {
                    var GetRegion = LocationMappingList.Where(x => (x.Location == null ? "" : x.Location.ToUpper()) == (GetCity.CITY == null ? "" : GetCity.CITY.ToUpper())).FirstOrDefault();
                    if (GetRegion != null)
                    {
                        input.ZoneId = GetRegion.ZoneSales;
                    }
                }
            }
            List<LiterByFunctionDto> data = _execSummBLL.GetLiterByFunctionData(input);

            var numb1 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT").Sum(c => c.TOTAL_LITER);
            var numb2 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC").Sum(c => c.TOTAL_LITER);

            var label1 = "BENEFIT (" + (numb1 == null ? "0" : numb1.Value.ToString("N2")) + ")";
            var label2 = "WTC (" + (numb2 == null ? "0" : numb2.Value.ToString("N2")) + ")";

            var groupData = data.GroupBy(x => new { x.FUNCTION })
                .Select(p => new LiterByFunctionDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    TOTAL_LITER_BENEFIT = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT").Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_WTC = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC").Sum(c => c.TOTAL_LITER),
                    LABEL1 = label1,
                    LABEL2 = label2
                }).OrderBy(x => x.FUNCTION).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult GetLiterByFunctionDataVisualRegion(int monthFrom, int? yearFrom)
        {
            var input = new LiterFuncGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthFrom;
            input.YearTo = yearFrom == null ? 0 : yearFrom.Value;
            input.VehicleType = "WTC";
            input.Function = "Sales,Marketing";
            List<LiterByFunctionDto> data = _execSummBLL.GetLiterByFunctionData(input);

            var numb1 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES").Sum(c => c.TOTAL_LITER);
            var numb2 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING").Sum(c => c.TOTAL_LITER);

            var label1 = "Sales (" + (numb1 == null ? "0" : numb1.Value.ToString("N2")) + ")";
            var label2 = "Marketing (" + (numb2 == null ? "0" : numb2.Value.ToString("N2")) + ")";

            var groupData = data.GroupBy(x => new { x.REGION })
                .Select(p => new LiterByFunctionDto()
                {
                    REGION = p.FirstOrDefault().REGION,
                    TOTAL_LITER_SALES = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES").Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_MARKETING = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING").Sum(c => c.TOTAL_LITER),
                    LABEL1 = label1,
                    LABEL2 = label2
                }).OrderBy(x => x.REGION).ToList();

            return Json(groupData);
        }
        
        [HttpPost]
        public JsonResult GetFuelCostByFunctionDataVisual(int monthFrom, int? yearFrom)
        {
            var input = new FuelCostFuncGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthFrom;
            input.YearTo = yearFrom == null ? 0 : yearFrom.Value;
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager
                        || CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager
                        || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                input.VehicleType = "WTC";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.HR || CurrentUser.UserRole == Enums.UserRole.HRManager)
            {
                input.VehicleType = "BENEFIT";
            }

            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager)
            {
                input.Function = "Sales,Marketing";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager)
            {
                input.Function = "Operations";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                input.Function = "Logistic";
            }
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
            var LocationMappingList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).ToList();
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.Logistic)
            {
                var GetCity = EmployeeList.Where(x => x.EMPLOYEE_ID == CurrentUser.EMPLOYEE_ID && x.IS_ACTIVE).FirstOrDefault();
                if (GetCity != null)
                {
                    var GetRegion = LocationMappingList.Where(x => (x.Location == null ? "" : x.Location.ToUpper()) == (GetCity.CITY == null ? "" : GetCity.CITY.ToUpper())).FirstOrDefault();
                    if (GetRegion != null)
                    {
                        input.ZoneId = GetRegion.ZoneSales;
                    }
                }
            }
            List<FuelCostByFunctionDto> data = _execSummBLL.GetFuelCostByFunctionData(input);

            var groupData = data.GroupBy(x => new { x.FUNCTION })
                .Select(p => new FuelLeaseDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    TOTAL_FUEL_COST = p.Sum(c => c.TOTAL_FUEL_COST)
                }).ToList();

            var input2 = new LeaseCostFuncGetByParamInput();
            input2.MonthFrom = monthFrom;
            input2.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input2.MonthTo = monthFrom;
            input2.YearTo = yearFrom == null ? 0 : yearFrom.Value;
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager)
            {
                input2.Function = "Sales,Marketing";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager)
            {
                input2.Function = "Operations";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                input2.Function = "Logistic";
            }
            List<LeaseCostByFunctionDto> data2 = _execSummBLL.GetLeaseCostByFunctionData(input2);

            var groupData2 = data2.GroupBy(x => new { x.FUNCTION })
                .Select(p => new FuelLeaseDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    TOTAL_LEASE_COST = p.Sum(c => c.TOTAL_LEASE_COST)
                }).ToList();

            groupData.AddRange(groupData2);

            var numb1 = groupData.Sum(x => x.TOTAL_FUEL_COST);
            var numb2 = groupData.Sum(x => x.TOTAL_LEASE_COST);

            var label1 = "Fuel (" + (numb1 == null ? "0" : numb1.Value.ToString("N2")) + ")";
            var label2 = "Lease (" + (numb2 == null ? "0" : numb2.Value.ToString("N2")) + ")";

            var groupData3 = groupData.GroupBy(x => new { x.FUNCTION })
                .Select(p => new FuelLeaseDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    TOTAL_FUEL_COST = p.Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_LEASE_COST = p.Sum(c => c.TOTAL_LEASE_COST),
                    LABEL1 = label1,
                    LABEL2 = label2
                }).OrderBy(x => x.FUNCTION).ToList();

            return Json(groupData3);
        }

        [HttpPost]
        public JsonResult GetFuelCostByFunctionDataVisualRegion(int monthFrom, int? yearFrom)
        {
            var input = new FuelCostFuncGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthFrom;
            input.YearTo = yearFrom == null ? 0 : yearFrom.Value;
            input.VehicleType = "WTC";
            input.Function = "Sales,Marketing";
            List<FuelCostByFunctionDto> data = _execSummBLL.GetFuelCostByFunctionData(input);

            var numb1 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES").Sum(c => c.TOTAL_FUEL_COST);
            var numb2 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING").Sum(c => c.TOTAL_FUEL_COST);

            var label1 = "Sales (" + (numb1 == null ? "0" : numb1.Value.ToString("N2")) + ")";
            var label2 = "Marketing (" + (numb2 == null ? "0" : numb2.Value.ToString("N2")) + ")";

            var groupData = data.GroupBy(x => new { x.REGION })
                .Select(p => new FuelCostByFunctionDto()
                {
                    REGION = p.FirstOrDefault().REGION,
                    TOTAL_FUEL_COST_SALES = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES").Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_MARKETING = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING").Sum(c => c.TOTAL_FUEL_COST),
                    LABEL1 = label1,
                    LABEL2 = label2
                }).OrderBy(x => x.REGION).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult GetLeaseCostByFunctionDataVisualRegion(int monthFrom, int? yearFrom)
        {
            var input = new LeaseCostFuncGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthFrom;
            input.YearTo = yearFrom == null ? 0 : yearFrom.Value;
            input.Function = "Sales,Marketing";
            List<LeaseCostByFunctionDto> data = _execSummBLL.GetLeaseCostByFunctionData(input);

            var numb1 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES").Sum(c => c.TOTAL_LEASE_COST);
            var numb2 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING").Sum(c => c.TOTAL_LEASE_COST);

            var label1 = "Sales (" + (numb1 == null ? "0" : numb1.Value.ToString("N2")) + ")";
            var label2 = "Marketing (" + (numb2 == null ? "0" : numb2.Value.ToString("N2")) + ")";

            var groupData = data.GroupBy(x => new { x.REGION })
                .Select(p => new LeaseCostByFunctionDto()
                {
                    REGION = p.FirstOrDefault().REGION,
                    TOTAL_LEASE_COST_SALES = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES").Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_MARKETING = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING").Sum(c => c.TOTAL_LEASE_COST),
                    LABEL1 = label1,
                    LABEL2 = label2
                }).OrderBy(x => x.REGION).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult SalesByRegionDataVisual(int monthFrom, int? yearFrom)
        {
            var input = new SalesRegionGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthFrom;
            input.YearTo = yearFrom == null ? 0 : yearFrom.Value;
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
            var LocationMappingList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).ToList();
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.Logistic)
            {
                var GetCity = EmployeeList.Where(x => x.EMPLOYEE_ID == CurrentUser.EMPLOYEE_ID && x.IS_ACTIVE).FirstOrDefault();
                if (GetCity != null)
                {
                    var GetRegion = LocationMappingList.Where(x => (x.Location == null ? "" : x.Location.ToUpper()) == (GetCity.CITY == null ? "" : GetCity.CITY.ToUpper())).FirstOrDefault();
                    if (GetRegion != null)
                    {
                        input.ZoneId = GetRegion.ZoneSales;
                    }
                }
            }
            List<SalesByRegionDto> data = _execSummBLL.GetSalesByRegionData(input);

            var numb1 = data.Sum(c => c.TOTAL_COST / c.STICK);
            var numb2 = data.Sum(c => c.TOTAL_COST / c.TOTAL_KM);

            var label1 = "per Stick (" + (numb1 == null ? "0" : numb1.Value.ToString("N2")) + ")";
            var label2 = "per KM (" + (numb2 == null ? "0" : numb2.Value.ToString("N2")) + ")";

            var groupData = data.GroupBy(x => new { x.REGION })
                .Select(p => new SalesByRegionDto()
                {
                    REGION = p.FirstOrDefault().REGION,
                    TOTAL_COST = p.Sum(c => c.TOTAL_COST / c.STICK),
                    TOTAL_KM = p.Sum(c => c.TOTAL_COST / c.TOTAL_KM),
                    LABEL1 = label1,
                    LABEL2 = label2
                }).OrderBy(x => x.REGION).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult AccidentDataVisual(int monthFrom, int? yearFrom)
        {
            var input = new AccidentGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthFrom;
            input.YearTo = yearFrom == null ? 0 : yearFrom.Value;
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager
                        || CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager
                        || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                input.VehicleType = "WTC";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.HR || CurrentUser.UserRole == Enums.UserRole.HRManager)
            {
                input.VehicleType = "BENEFIT";
            }

            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager)
            {
                input.Function = "Sales,Marketing";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager)
            {
                input.Function = "Operations";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                input.Function = "Logistic";
            }
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
            var LocationMappingList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).ToList();
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.Logistic)
            {
                var GetCity = EmployeeList.Where(x => x.EMPLOYEE_ID == CurrentUser.EMPLOYEE_ID && x.IS_ACTIVE).FirstOrDefault();
                if (GetCity != null)
                {
                    var GetRegion = LocationMappingList.Where(x => (x.Location == null ? "" : x.Location.ToUpper()) == (GetCity.CITY == null ? "" : GetCity.CITY.ToUpper())).FirstOrDefault();
                    if (GetRegion != null)
                    {
                        input.ZoneId = GetRegion.ZoneSales;
                    }
                }
            }
            List<AccidentDto> data = _execSummBLL.GetAccidentData(input);

            var label1 = "BENEFIT (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT").Sum(x => x.ACCIDENT_COUNT) + ")";
            var label2 = "WTC (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC").Sum(x => x.ACCIDENT_COUNT) + ")";

            var groupData = data.GroupBy(x => new { x.FUNCTION })
                .Select(p => new AccidentDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    ACCIDENT_COUNT = p.Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_BENEFIT = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT").Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_WTC = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC").Sum(c => c.ACCIDENT_COUNT),
                    LABEL1 = label1,
                    LABEL2 = label2
                }).OrderBy(x => x.FUNCTION).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult AccidentDataVisualRegion(int monthFrom, int? yearFrom)
        {
            var input = new AccidentGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthFrom;
            input.YearTo = yearFrom == null ? 0 : yearFrom.Value;
            input.VehicleType = "WTC";
            input.Function = "Sales,Marketing";
            List<AccidentDto> data = _execSummBLL.GetAccidentData(input);

            var label1 = "Sales (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES").Sum(x => x.ACCIDENT_COUNT) + ")";
            var label2 = "Marketing (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING").Sum(x => x.ACCIDENT_COUNT) + ")";

            var groupData = data.GroupBy(x => new { x.REGION })
                .Select(p => new AccidentDto()
                {
                    REGION = p.FirstOrDefault().REGION,
                    ACCIDENT_COUNT_SALES = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES").Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_MARKETING = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING").Sum(c => c.ACCIDENT_COUNT),
                    LABEL1 = label1,
                    LABEL2 = label2
                }).OrderBy(x => x.REGION).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult AcObDataVisual(int monthFrom, int? yearFrom)
        {
            var input = new AcVsObGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthFrom;
            input.YearTo = yearFrom == null ? 0 : yearFrom.Value;
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager)
            {
                input.Function = "Sales,Marketing";
                input.VehicleType = "WTC";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager)
            {
                input.Function = "Operations";
                input.VehicleType = "WTC";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                input.Function = "Logistic";
                input.VehicleType = "WTC";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.HR || CurrentUser.UserRole == Enums.UserRole.HRManager)
            {
                input.VehicleType = "BENEFIT";
            }
            List<AcVsObDto> data = _execSummBLL.GetAcVsObData(input);

            var numb1 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT").Sum(c => c.ACTUAL_COST);
            var numb2 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC").Sum(c => c.ACTUAL_COST);
            var numb3 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT").Sum(c => c.COST_OB);
            var numb4 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC").Sum(c => c.COST_OB);

            var label1 = "Actual BENEFIT (" + (numb1 == null ? "0" : numb1.Value.ToString("N2")) + ")";
            var label2 = "Actual WTC (" + (numb2 == null ? "0" : numb2.Value.ToString("N2")) + ")";
            var label3 = "Budget BENEFIT (" + (numb3 == null ? "0" : numb3.Value.ToString("N2")) + ")";
            var label4 = "Budget WTC (" + (numb4 == null ? "0" : numb4.Value.ToString("N2")) + ")";

            var groupData = data.GroupBy(x => new { x.FUNCTION })
                .Select(p => new AcVsObDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    ACTUAL_COST_BENEFIT = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT").Sum(c => c.ACTUAL_COST),
                    ACTUAL_COST_WTC = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC").Sum(c => c.ACTUAL_COST),
                    COST_OB_BENEFIT = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT").Sum(c => c.COST_OB),
                    COST_OB_WTC = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC").Sum(c => c.COST_OB),
                    LABEL1 = label1,
                    LABEL2 = label2,
                    LABEL3 = label3,
                    LABEL4 = label4
                }).OrderBy(x => x.FUNCTION).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult AcObUnitDataVisual(int monthFrom, int? yearFrom, bool isRegion)
        {
            var input = new AcVsObGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthFrom;
            input.YearTo = yearFrom == null ? 0 : yearFrom.Value;
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager)
            {
                input.Function = "Sales,Marketing";
                input.VehicleType = "WTC";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager)
            {
                input.Function = "Operations";
                input.VehicleType = "WTC";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                input.Function = "Logistic";
                input.VehicleType = "WTC";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.HR || CurrentUser.UserRole == Enums.UserRole.HRManager)
            {
                input.VehicleType = "BENEFIT";
                input.VehicleType = "WTC";
            }
            if (isRegion)
            {
                input.Function = "Sales,Marketing";
            }
            List<AcVsObDto> data = _execSummBLL.GetAcVsObData(input);

            var numb1 = data.Sum(c => c.UNIT);
            var numb2 = data.Sum(c => c.UNIT_BUDGET);

            var label1 = "Unit Actual (" + (numb1 == null ? 0 : numb1.Value) + ")";
            var label2 = "Unit Bugdet (" + (numb2 == null ? 0 : numb2.Value) + ")";

            var groupData = data.GroupBy(x => new { x.FUNCTION })
                .Select(p => new AcVsObDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    UNIT = p.Sum(c => c.UNIT),
                    UNIT_BUDGET = p.Sum(c => c.UNIT_BUDGET),
                    LABEL1 = label1,
                    LABEL2 = label2
                }).OrderBy(x => x.FUNCTION).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult AcObDataVisualRegion(int monthFrom, int? yearFrom)
        {
            var input = new AcVsObGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthFrom;
            input.YearTo = yearFrom == null ? 0 : yearFrom.Value;
            input.VehicleType = "WTC";
            input.Function = "Sales,Marketing";
            List<AcVsObDto> data = _execSummBLL.GetAcVsObData(input);

            var numb1 = data.Sum(c => c.ACTUAL_COST);
            var numb2 = data.Sum(c => c.COST_OB);

            var label1 = "Actual (" + (numb1 == null ? "0" : numb1.Value.ToString("N2")) + ")";
            var label2 = "Budget (" + (numb2 == null ? "0" : numb2.Value.ToString("N2")) + ")";

            var groupData = data.GroupBy(x => new { x.FUNCTION })
                .Select(p => new AcVsObDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    ACTUAL_COST = p.Sum(c => c.ACTUAL_COST),
                    COST_OB = p.Sum(c => c.COST_OB),
                    LABEL1 = label1,
                    LABEL2 = label2
                }).OrderBy(x => x.FUNCTION).ToList();

            return Json(groupData);
        }

        #endregion

        #region --------- Get Data Json Graphic New Request -----------------------
        [HttpPost]
        public JsonResult VisualNoVehicle(int monthFrom, int? yearFrom, int monthTo, int? yearTo, bool isByRegion, string vehType, string regional, string suppMethod, string functions, string zone)
        {
            var input = new VehicleGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthTo;
            input.YearTo = yearTo == null ? 0 : yearTo.Value;
            input.VehicleType = vehType;
            input.Regional = regional;
            input.SupplyMethod = suppMethod;
            input.Function = functions;
            input.ZoneId = zone;
            if (isByRegion)
            {
                input.Function = "Sales,Marketing";
            }
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
            var LocationMappingList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).ToList();
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.Logistic)
            {
                var GetCity = EmployeeList.Where(x => x.EMPLOYEE_ID == CurrentUser.EMPLOYEE_ID && x.IS_ACTIVE).FirstOrDefault();
                if (GetCity != null)
                {
                    var GetRegion = LocationMappingList.Where(x => (x.Location == null ? "" : x.Location.ToUpper()) == (GetCity.CITY == null ? "" : GetCity.CITY.ToUpper())).FirstOrDefault();
                    if (GetRegion != null)
                    {
                        input.ZoneId = GetRegion.ZoneSales;
                    }
                }
            }
            List<NoVehicleDto> data = _execSummBLL.GetNoOfVehicleData(input);

            var label1 = "BENEFIT Jan " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label2 = "BENEFIT Feb " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label3 = "BENEFIT Mar " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label4 = "BENEFIT Apr " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label5 = "BENEFIT May " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label6 = "BENEFIT Jun " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label7 = "BENEFIT Jul " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label8 = "BENEFIT Aug " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label9 = "BENEFIT Sep " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label10 = "BENEFIT Oct " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label11 = "BENEFIT Nov " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label12 = "BENEFIT Dec " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label13 = "WTC Jan " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label14 = "WTC Feb " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label15 = "WTC Mar " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label16 = "WTC Apr " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label17 = "WTC May " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label18 = "WTC Jun " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label19 = "WTC Jul " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label20 = "WTC Aug " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label21 = "WTC Sep " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label22 = "WTC Oct " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label23 = "WTC Nov " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label24 = "WTC Dec " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var groupData = data.GroupBy(x => new { x.FUNCTION })
                .Select(p => new NoVehicleDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    NO_OF_VEHICLE_BENEFIT_1 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_BENEFIT_2 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_BENEFIT_3 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_BENEFIT_4 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_BENEFIT_5 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_BENEFIT_6 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_BENEFIT_7 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_BENEFIT_8 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_BENEFIT_9 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_BENEFIT_10 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_BENEFIT_11 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_BENEFIT_12 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_WTC_1 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_WTC_2 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_WTC_3 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_WTC_4 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_WTC_5 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_WTC_6 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_WTC_7 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_WTC_8 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_WTC_9 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_WTC_10 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_WTC_11 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_WTC_12 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    LABEL1 = label1,
                    LABEL2 = label2,
                    LABEL3 = label3,
                    LABEL4 = label4,
                    LABEL5 = label5,
                    LABEL6 = label6,
                    LABEL7 = label7,
                    LABEL8 = label8,
                    LABEL9 = label9,
                    LABEL10 = label10,
                    LABEL11 = label11,
                    LABEL12 = label12,
                    LABEL13 = label13,
                    LABEL14 = label14,
                    LABEL15 = label15,
                    LABEL16 = label16,
                    LABEL17 = label17,
                    LABEL18 = label18,
                    LABEL19 = label19,
                    LABEL20 = label20,
                    LABEL21 = label21,
                    LABEL22 = label22,
                    LABEL23 = label23,
                    LABEL24 = label24,
                }).OrderBy(x => x.FUNCTION).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult VisualNoVehicleWtc(int monthFrom, int? yearFrom, int monthTo, int? yearTo, bool isByRegion, string regional, string functions, string zone)
        {
            var input = new VehicleWtcGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthTo;
            input.YearTo = yearTo == null ? 0 : yearTo.Value;
            input.Regional = regional;
            input.Function = functions;
            input.ZoneId = zone;
            if (isByRegion)
            {
                input.Function = "Sales,Marketing";
            }
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
            var LocationMappingList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).ToList();
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.Logistic)
            {
                var GetCity = EmployeeList.Where(x => x.EMPLOYEE_ID == CurrentUser.EMPLOYEE_ID && x.IS_ACTIVE).FirstOrDefault();
                if (GetCity != null)
                {
                    var GetRegion = LocationMappingList.Where(x => (x.Location == null ? "" : x.Location.ToUpper()) == (GetCity.CITY == null ? "" : GetCity.CITY.ToUpper())).FirstOrDefault();
                    if (GetRegion != null)
                    {
                        input.ZoneId = GetRegion.ZoneSales;
                    }
                }
            }
            List<NoVehicleWtcDto> data = _execSummBLL.GetNoOfVehicleWtcData(input);

            var label1 = "Sales Jan " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label2 = "Sales Feb " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label3 = "Sales Mar " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label4 = "Sales Apr " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label5 = "Sales May " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label6 = "Sales Jun " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label7 = "Sales Jul " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label8 = "Sales Aug " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label9 = "Sales Sep " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label10 = "Sales Oct " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label11 = "Sales Nov " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label12 = "Sales Dec " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label13 = "Marketing Jan " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label14 = "Marketing Feb " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label15 = "Marketing Mar " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label16 = "Marketing Apr " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label17 = "Marketing May " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label18 = "Marketing Jun " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label19 = "Marketing Jul " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label20 = "Marketing Aug " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label21 = "Marketing Sep " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label22 = "Marketing Oct " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label23 = "Marketing Nov " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label24 = "Marketing Dec " + yearFrom + " (" + data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var groupData = data.GroupBy(x => new { x.REGIONAL })
                .Select(p => new NoVehicleWtcDto()
                {
                    REGIONAL = (p.FirstOrDefault().REGIONAL == null || p.FirstOrDefault().REGIONAL == "0") ? "HO" : p.FirstOrDefault().REGIONAL,
                    NO_OF_VEHICLE_SALES_1 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_SALES_2 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_SALES_3 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_SALES_4 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_SALES_5 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_SALES_6 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_SALES_7 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_SALES_8 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_SALES_9 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_SALES_10 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_SALES_11 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_SALES_12 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MARKETING_1 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MARKETING_2 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MARKETING_3 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MARKETING_4 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MARKETING_5 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MARKETING_6 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MARKETING_7 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MARKETING_8 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MARKETING_9 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MARKETING_10 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MARKETING_11 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MARKETING_12 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    LABEL1 = label1,
                    LABEL2 = label2,
                    LABEL3 = label3,
                    LABEL4 = label4,
                    LABEL5 = label5,
                    LABEL6 = label6,
                    LABEL7 = label7,
                    LABEL8 = label8,
                    LABEL9 = label9,
                    LABEL10 = label10,
                    LABEL11 = label11,
                    LABEL12 = label12,
                    LABEL13 = label13,
                    LABEL14 = label14,
                    LABEL15 = label15,
                    LABEL16 = label16,
                    LABEL17 = label17,
                    LABEL18 = label18,
                    LABEL19 = label19,
                    LABEL20 = label20,
                    LABEL21 = label21,
                    LABEL22 = label22,
                    LABEL23 = label23,
                    LABEL24 = label24,
                }).OrderBy(x => x.REGIONAL).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult VisualNoVehicleMake(int monthFrom, int? yearFrom, int monthTo, int? yearTo, bool isByRegion, string make, string bodType)
        {
            var input = new VehicleMakeGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthTo;
            input.YearTo = yearTo == null ? 0 : yearTo.Value;
            input.Manufacturer = make;
            input.BodyType = bodType;

            List<NoVehicleMakeDto> data = _execSummBLL.GetNoOfVehicleMakeData(input);

            var label1 = "Total Jan " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label2 = "Total Feb " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label3 = "Total Mar " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label4 = "Total Apr " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label5 = "Total May " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label6 = "Total Jun " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label7 = "Total Jul " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label8 = "Total Aug " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label9 = "Total Sep " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label10 = "Total Oct " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label11 = "Total Nov " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label12 = "Total Dec " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var groupData = data.GroupBy(x => new { x.MANUFACTURER })
                .Select(p => new NoVehicleMakeDto()
                {
                    MANUFACTURER = p.FirstOrDefault().MANUFACTURER,
                    NO_OF_VEHICLE_MAN_1 = p.Where(x => x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MAN_2 = p.Where(x => x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MAN_3 = p.Where(x => x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MAN_4 = p.Where(x => x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MAN_5 = p.Where(x => x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MAN_6 = p.Where(x => x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MAN_7 = p.Where(x => x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MAN_8 = p.Where(x => x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MAN_9 = p.Where(x => x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MAN_10 = p.Where(x => x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MAN_11 = p.Where(x => x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_MAN_12 = p.Where(x => x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    LABEL1 = label1,
                    LABEL2 = label2,
                    LABEL3 = label3,
                    LABEL4 = label4,
                    LABEL5 = label5,
                    LABEL6 = label6,
                    LABEL7 = label7,
                    LABEL8 = label8,
                    LABEL9 = label9,
                    LABEL10 = label10,
                    LABEL11 = label11,
                    LABEL12 = label12,
                }).OrderBy(x => x.MANUFACTURER).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult VisualNoVehicleType(int monthFrom, int? yearFrom, int monthTo, int? yearTo, bool isByRegion, string make, string bodType)
        {
            var input = new VehicleMakeGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthTo;
            input.YearTo = yearTo == null ? 0 : yearTo.Value;
            input.Manufacturer = make;
            input.BodyType = bodType;

            List<NoVehicleMakeDto> data = _execSummBLL.GetNoOfVehicleMakeData(input);

            var label13 = "Total Jan " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label14 = "Total Feb " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label15 = "Total Mar " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label16 = "Total Apr " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label17 = "Total May " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label18 = "Total Jun " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label19 = "Total Jul " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label20 = "Total Aug " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label21 = "Total Sep " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label22 = "Total Oct " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label23 = "Total Nov " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var label24 = "Total Dec " + yearFrom + " (" + data.Where(x => x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(x => x.NO_OF_VEHICLE) + ")";

            var groupData = data.GroupBy(x => new { x.BODY_TYPE })
                .Select(p => new NoVehicleMakeDto()
                {
                    BODY_TYPE = p.FirstOrDefault().BODY_TYPE,
                    NO_OF_VEHICLE_BOD_1 = p.Where(x => x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_BOD_2 = p.Where(x => x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_BOD_3 = p.Where(x => x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_BOD_4 = p.Where(x => x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_BOD_5 = p.Where(x => x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_BOD_6 = p.Where(x => x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_BOD_7 = p.Where(x => x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_BOD_8 = p.Where(x => x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_BOD_9 = p.Where(x => x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_BOD_10 = p.Where(x => x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_BOD_11 = p.Where(x => x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    NO_OF_VEHICLE_BOD_12 = p.Where(x => x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.NO_OF_VEHICLE),
                    LABEL13 = label13,
                    LABEL14 = label14,
                    LABEL15 = label15,
                    LABEL16 = label16,
                    LABEL17 = label17,
                    LABEL18 = label18,
                    LABEL19 = label19,
                    LABEL20 = label20,
                    LABEL21 = label21,
                    LABEL22 = label22,
                    LABEL23 = label23,
                    LABEL24 = label24,
                }).OrderBy(x => x.BODY_TYPE).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult VisualOdometer(int monthFrom, int? yearFrom, int monthTo, int? yearTo, bool isByRegion, string vehType, string regional, string functions, string zone)
        {
            var input = new OdometerGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthTo;
            input.YearTo = yearTo == null ? 0 : yearTo.Value;
            input.VehicleType = vehType;
            input.Region = regional;
            input.Function = functions;
            input.ZoneId = zone;
            if (isByRegion)
            {
                input.Function = "Sales,Marketing";
            }
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
            var LocationMappingList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).ToList();
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                var GetCity = EmployeeList.Where(x => x.EMPLOYEE_ID == CurrentUser.EMPLOYEE_ID && x.IS_ACTIVE).FirstOrDefault();
                if (GetCity != null)
                {
                    var GetRegion = LocationMappingList.Where(x => (x.Location == null ? "" : x.Location.ToUpper()) == (GetCity.CITY == null ? "" : GetCity.CITY.ToUpper())).FirstOrDefault();
                    if (GetRegion != null)
                    {
                        input.ZoneId = GetRegion.ZoneSales;
                    }
                }
            }
            List<OdometerDto> data = _execSummBLL.GetOdometerData(input);

            var numb1 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);
            var numb2 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);
            var numb3 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);
            var numb4 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);
            var numb5 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);
            var numb6 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);
            var numb7 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);
            var numb8 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);
            var numb9 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);
            var numb10 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);
            var numb11 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);
            var numb12 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);
            var numb13 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);
            var numb14 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);
            var numb15 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);
            var numb16 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);
            var numb17 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);
            var numb18 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);
            var numb19 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);
            var numb20 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);
            var numb21 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);
            var numb22 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);
            var numb23 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);
            var numb24 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM);

            var label1 = "BENEFIT Jan " + yearFrom + " (" + (numb1 == null ? "0" : numb1.Value.ToString("N2")) + ")";
            var label2 = "BENEFIT Feb " + yearFrom + " (" + (numb2 == null ? "0" : numb2.Value.ToString("N2")) + ")";
            var label3 = "BENEFIT Mar " + yearFrom + " (" + (numb3 == null ? "0" : numb3.Value.ToString("N2")) + ")";
            var label4 = "BENEFIT Apr " + yearFrom + " (" + (numb4 == null ? "0" : numb4.Value.ToString("N2")) + ")";
            var label5 = "BENEFIT May " + yearFrom + " (" + (numb5 == null ? "0" : numb5.Value.ToString("N2")) + ")";
            var label6 = "BENEFIT Jun " + yearFrom + " (" + (numb6 == null ? "0" : numb6.Value.ToString("N2")) + ")";
            var label7 = "BENEFIT Jul " + yearFrom + " (" + (numb7 == null ? "0" : numb7.Value.ToString("N2")) + ")";
            var label8 = "BENEFIT Aug " + yearFrom + " (" + (numb8 == null ? "0" : numb8.Value.ToString("N2")) + ")";
            var label9 = "BENEFIT Sep " + yearFrom + " (" + (numb9 == null ? "0" : numb9.Value.ToString("N2")) + ")";
            var label10 = "BENEFIT Oct " + yearFrom + " (" + (numb10 == null ? "0" : numb10.Value.ToString("N2")) + ")";
            var label11 = "BENEFIT Nov " + yearFrom + " (" + (numb11 == null ? "0" : numb11.Value.ToString("N2")) + ")";
            var label12 = "BENEFIT Dec " + yearFrom + " (" + (numb12 == null ? "0" : numb12.Value.ToString("N2")) + ")";
            var label13 = "WTC Jan " + yearFrom + " (" + (numb13 == null ? "0" : numb13.Value.ToString("N2")) + ")";
            var label14 = "WTC Feb " + yearFrom + " (" + (numb14 == null ? "0" : numb14.Value.ToString("N2")) + ")";
            var label15 = "WTC Mar " + yearFrom + " (" + (numb15 == null ? "0" : numb15.Value.ToString("N2")) + ")";
            var label16 = "WTC Apr " + yearFrom + " (" + (numb16 == null ? "0" : numb16.Value.ToString("N2")) + ")";
            var label17 = "WTC May " + yearFrom + " (" + (numb17 == null ? "0" : numb17.Value.ToString("N2")) + ")";
            var label18 = "WTC Jun " + yearFrom + " (" + (numb18 == null ? "0" : numb18.Value.ToString("N2")) + ")";
            var label19 = "WTC Jul " + yearFrom + " (" + (numb19 == null ? "0" : numb19.Value.ToString("N2")) + ")";
            var label20 = "WTC Aug " + yearFrom + " (" + (numb20 == null ? "0" : numb20.Value.ToString("N2")) + ")";
            var label21 = "WTC Sep " + yearFrom + " (" + (numb21 == null ? "0" : numb21.Value.ToString("N2")) + ")";
            var label22 = "WTC Oct " + yearFrom + " (" + (numb22 == null ? "0" : numb22.Value.ToString("N2")) + ")";
            var label23 = "WTC Nov " + yearFrom + " (" + (numb23 == null ? "0" : numb23.Value.ToString("N2")) + ")";
            var label24 = "WTC Dec " + yearFrom + " (" + (numb24 == null ? "0" : numb24.Value.ToString("N2")) + ")";

            var groupData = data.GroupBy(x => new { x.FUNCTION })
                .Select(p => new OdometerDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,

                    TOTAL_KM_BENEFIT_1 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    TOTAL_KM_BENEFIT_2 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    TOTAL_KM_BENEFIT_3 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    TOTAL_KM_BENEFIT_4 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    TOTAL_KM_BENEFIT_5 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    TOTAL_KM_BENEFIT_6 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    TOTAL_KM_BENEFIT_7 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    TOTAL_KM_BENEFIT_8 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    TOTAL_KM_BENEFIT_9 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    TOTAL_KM_BENEFIT_10 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    TOTAL_KM_BENEFIT_11 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    TOTAL_KM_BENEFIT_12 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    TOTAL_KM_WTC_1 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    TOTAL_KM_WTC_2 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    TOTAL_KM_WTC_3 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    TOTAL_KM_WTC_4 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    TOTAL_KM_WTC_5 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    TOTAL_KM_WTC_6 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    TOTAL_KM_WTC_7 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    TOTAL_KM_WTC_8 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    TOTAL_KM_WTC_9 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    TOTAL_KM_WTC_10 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    TOTAL_KM_WTC_11 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    TOTAL_KM_WTC_12 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_KM),
                    LABEL1 = label1,
                    LABEL2 = label2,
                    LABEL3 = label3,
                    LABEL4 = label4,
                    LABEL5 = label5,
                    LABEL6 = label6,
                    LABEL7 = label7,
                    LABEL8 = label8,
                    LABEL9 = label9,
                    LABEL10 = label10,
                    LABEL11 = label11,
                    LABEL12 = label12,
                    LABEL13 = label13,
                    LABEL14 = label14,
                    LABEL15 = label15,
                    LABEL16 = label16,
                    LABEL17 = label17,
                    LABEL18 = label18,
                    LABEL19 = label19,
                    LABEL20 = label20,
                    LABEL21 = label21,
                    LABEL22 = label22,
                    LABEL23 = label23,
                    LABEL24 = label24,
                }).OrderBy(x => x.FUNCTION).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult VisualLiter(int monthFrom, int? yearFrom, int monthTo, int? yearTo, bool isByRegion, string vehType, string regional, string functions, string zone)
        {
            var input = new LiterFuncGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthTo;
            input.YearTo = yearTo == null ? 0 : yearTo.Value;
            input.VehicleType = vehType;
            input.Region = regional;
            input.Function = functions;
            input.ZoneId = zone;
            if (isByRegion)
            {
                input.Function = "Sales,Marketing";
            }
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
            var LocationMappingList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).ToList();
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.Logistic)
            {
                var GetCity = EmployeeList.Where(x => x.EMPLOYEE_ID == CurrentUser.EMPLOYEE_ID && x.IS_ACTIVE).FirstOrDefault();
                if (GetCity != null)
                {
                    var GetRegion = LocationMappingList.Where(x => (x.Location == null ? "" : x.Location.ToUpper()) == (GetCity.CITY == null ? "" : GetCity.CITY.ToUpper())).FirstOrDefault();
                    if (GetRegion != null)
                    {
                        input.ZoneId = GetRegion.ZoneSales;
                    }
                }
            }
            List<LiterByFunctionDto> data = _execSummBLL.GetLiterByFunctionData(input);

            var numb1 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);
            var numb2 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);
            var numb3 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);
            var numb4 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);
            var numb5 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);
            var numb6 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);
            var numb7 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);
            var numb8 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);
            var numb9 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);
            var numb10 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);
            var numb11 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);
            var numb12 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);
            var numb13 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);
            var numb14 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);
            var numb15 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);
            var numb16 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);
            var numb17 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);
            var numb18 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);
            var numb19 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);
            var numb20 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);
            var numb21 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);
            var numb22 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);
            var numb23 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);
            var numb24 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER);

            var label1 = "BENEFIT Jan " + yearFrom + " (" + (numb1 == null ? "0" : numb1.Value.ToString("N2")) + ")";
            var label2 = "BENEFIT Feb " + yearFrom + " (" + (numb2 == null ? "0" : numb2.Value.ToString("N2")) + ")";
            var label3 = "BENEFIT Mar " + yearFrom + " (" + (numb3 == null ? "0" : numb3.Value.ToString("N2")) + ")";
            var label4 = "BENEFIT Apr " + yearFrom + " (" + (numb4 == null ? "0" : numb4.Value.ToString("N2")) + ")";
            var label5 = "BENEFIT May " + yearFrom + " (" + (numb5 == null ? "0" : numb5.Value.ToString("N2")) + ")";
            var label6 = "BENEFIT Jun " + yearFrom + " (" + (numb6 == null ? "0" : numb6.Value.ToString("N2")) + ")";
            var label7 = "BENEFIT Jul " + yearFrom + " (" + (numb7 == null ? "0" : numb7.Value.ToString("N2")) + ")";
            var label8 = "BENEFIT Aug " + yearFrom + " (" + (numb8 == null ? "0" : numb8.Value.ToString("N2")) + ")";
            var label9 = "BENEFIT Sep " + yearFrom + " (" + (numb9 == null ? "0" : numb9.Value.ToString("N2")) + ")";
            var label10 = "BENEFIT Oct " + yearFrom + " (" + (numb10 == null ? "0" : numb10.Value.ToString("N2")) + ")";
            var label11 = "BENEFIT Nov " + yearFrom + " (" + (numb11 == null ? "0" : numb11.Value.ToString("N2")) + ")";
            var label12 = "BENEFIT Dec " + yearFrom + " (" + (numb12 == null ? "0" : numb12.Value.ToString("N2")) + ")";
            var label13 = "WTC Jan " + yearFrom + " (" + (numb13 == null ? "0" : numb13.Value.ToString("N2")) + ")";
            var label14 = "WTC Feb " + yearFrom + " (" + (numb14 == null ? "0" : numb14.Value.ToString("N2")) + ")";
            var label15 = "WTC Mar " + yearFrom + " (" + (numb15 == null ? "0" : numb15.Value.ToString("N2")) + ")";
            var label16 = "WTC Apr " + yearFrom + " (" + (numb16 == null ? "0" : numb16.Value.ToString("N2")) + ")";
            var label17 = "WTC May " + yearFrom + " (" + (numb17 == null ? "0" : numb17.Value.ToString("N2")) + ")";
            var label18 = "WTC Jun " + yearFrom + " (" + (numb18 == null ? "0" : numb18.Value.ToString("N2")) + ")";
            var label19 = "WTC Jul " + yearFrom + " (" + (numb19 == null ? "0" : numb19.Value.ToString("N2")) + ")";
            var label20 = "WTC Aug " + yearFrom + " (" + (numb20 == null ? "0" : numb20.Value.ToString("N2")) + ")";
            var label21 = "WTC Sep " + yearFrom + " (" + (numb21 == null ? "0" : numb21.Value.ToString("N2")) + ")";
            var label22 = "WTC Oct " + yearFrom + " (" + (numb22 == null ? "0" : numb22.Value.ToString("N2")) + ")";
            var label23 = "WTC Nov " + yearFrom + " (" + (numb23 == null ? "0" : numb23.Value.ToString("N2")) + ")";
            var label24 = "WTC Dec " + yearFrom + " (" + (numb24 == null ? "0" : numb24.Value.ToString("N2")) + ")";

            var groupData = data.GroupBy(x => new { x.FUNCTION })
                .Select(p => new LiterByFunctionDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,

                    TOTAL_LITER_BENEFIT_1 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_BENEFIT_2 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_BENEFIT_3 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_BENEFIT_4 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_BENEFIT_5 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_BENEFIT_6 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_BENEFIT_7 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_BENEFIT_8 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_BENEFIT_9 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_BENEFIT_10 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_BENEFIT_11 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_BENEFIT_12 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_WTC_1 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_WTC_2 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_WTC_3 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_WTC_4 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_WTC_5 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_WTC_6 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_WTC_7 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_WTC_8 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_WTC_9 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_WTC_10 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_WTC_11 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    TOTAL_LITER_WTC_12 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LITER),
                    LABEL1 = label1,
                    LABEL2 = label2,
                    LABEL3 = label3,
                    LABEL4 = label4,
                    LABEL5 = label5,
                    LABEL6 = label6,
                    LABEL7 = label7,
                    LABEL8 = label8,
                    LABEL9 = label9,
                    LABEL10 = label10,
                    LABEL11 = label11,
                    LABEL12 = label12,
                    LABEL13 = label13,
                    LABEL14 = label14,
                    LABEL15 = label15,
                    LABEL16 = label16,
                    LABEL17 = label17,
                    LABEL18 = label18,
                    LABEL19 = label19,
                    LABEL20 = label20,
                    LABEL21 = label21,
                    LABEL22 = label22,
                    LABEL23 = label23,
                    LABEL24 = label24,
                }).OrderBy(x => x.FUNCTION).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult VisualFuel(int monthFrom, int? yearFrom, int monthTo, int? yearTo, bool isByRegion, string vehType, string regional, string functions, string zone)
        {
            var input = new FuelCostFuncGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthTo;
            input.YearTo = yearTo == null ? 0 : yearTo.Value;
            input.VehicleType = vehType;
            input.Region = regional;
            input.Function = functions;
            input.ZoneId = zone;
            if (isByRegion)
            {
                input.Function = "Sales,Marketing";
            }
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
            var LocationMappingList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).ToList();
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.Logistic)
            {
                var GetCity = EmployeeList.Where(x => x.EMPLOYEE_ID == CurrentUser.EMPLOYEE_ID && x.IS_ACTIVE).FirstOrDefault();
                if (GetCity != null)
                {
                    var GetRegion = LocationMappingList.Where(x => (x.Location == null ? "" : x.Location.ToUpper()) == (GetCity.CITY == null ? "" : GetCity.CITY.ToUpper())).FirstOrDefault();
                    if (GetRegion != null)
                    {
                        input.ZoneId = GetRegion.ZoneSales;
                    }
                }
            }
            List<FuelCostByFunctionDto> data = _execSummBLL.GetFuelCostByFunctionData(input);

            var numb1 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);
            var numb2 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);
            var numb3 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);
            var numb4 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);
            var numb5 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);
            var numb6 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);
            var numb7 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);
            var numb8 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);
            var numb9 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);
            var numb10 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);
            var numb11 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);
            var numb12 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);
            var numb13 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);
            var numb14 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);
            var numb15 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);
            var numb16 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);
            var numb17 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);
            var numb18 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);
            var numb19 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);
            var numb20 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);
            var numb21 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);
            var numb22 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);
            var numb23 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);
            var numb24 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST);

            var label1 = "BENEFIT Jan " + yearFrom + " (" + (numb1 == null ? "0" : numb1.Value.ToString("N2")) + ")";
            var label2 = "BENEFIT Feb " + yearFrom + " (" + (numb2 == null ? "0" : numb2.Value.ToString("N2")) + ")";
            var label3 = "BENEFIT Mar " + yearFrom + " (" + (numb3 == null ? "0" : numb3.Value.ToString("N2")) + ")";
            var label4 = "BENEFIT Apr " + yearFrom + " (" + (numb4 == null ? "0" : numb4.Value.ToString("N2")) + ")";
            var label5 = "BENEFIT May " + yearFrom + " (" + (numb5 == null ? "0" : numb5.Value.ToString("N2")) + ")";
            var label6 = "BENEFIT Jun " + yearFrom + " (" + (numb6 == null ? "0" : numb6.Value.ToString("N2")) + ")";
            var label7 = "BENEFIT Jul " + yearFrom + " (" + (numb7 == null ? "0" : numb7.Value.ToString("N2")) + ")";
            var label8 = "BENEFIT Aug " + yearFrom + " (" + (numb8 == null ? "0" : numb8.Value.ToString("N2")) + ")";
            var label9 = "BENEFIT Sep " + yearFrom + " (" + (numb9 == null ? "0" : numb9.Value.ToString("N2")) + ")";
            var label10 = "BENEFIT Oct " + yearFrom + " (" + (numb10 == null ? "0" : numb10.Value.ToString("N2")) + ")";
            var label11 = "BENEFIT Nov " + yearFrom + " (" + (numb11 == null ? "0" : numb11.Value.ToString("N2")) + ")";
            var label12 = "BENEFIT Dec " + yearFrom + " (" + (numb12 == null ? "0" : numb12.Value.ToString("N2")) + ")";
            var label13 = "WTC Jan " + yearFrom + " (" + (numb13 == null ? "0" : numb13.Value.ToString("N2")) + ")";
            var label14 = "WTC Feb " + yearFrom + " (" + (numb14 == null ? "0" : numb14.Value.ToString("N2")) + ")";
            var label15 = "WTC Mar " + yearFrom + " (" + (numb15 == null ? "0" : numb15.Value.ToString("N2")) + ")";
            var label16 = "WTC Apr " + yearFrom + " (" + (numb16 == null ? "0" : numb16.Value.ToString("N2")) + ")";
            var label17 = "WTC May " + yearFrom + " (" + (numb17 == null ? "0" : numb17.Value.ToString("N2")) + ")";
            var label18 = "WTC Jun " + yearFrom + " (" + (numb18 == null ? "0" : numb18.Value.ToString("N2")) + ")";
            var label19 = "WTC Jul " + yearFrom + " (" + (numb19 == null ? "0" : numb19.Value.ToString("N2")) + ")";
            var label20 = "WTC Aug " + yearFrom + " (" + (numb20 == null ? "0" : numb20.Value.ToString("N2")) + ")";
            var label21 = "WTC Sep " + yearFrom + " (" + (numb21 == null ? "0" : numb21.Value.ToString("N2")) + ")";
            var label22 = "WTC Oct " + yearFrom + " (" + (numb22 == null ? "0" : numb22.Value.ToString("N2")) + ")";
            var label23 = "WTC Nov " + yearFrom + " (" + (numb23 == null ? "0" : numb23.Value.ToString("N2")) + ")";
            var label24 = "WTC Dec " + yearFrom + " (" + (numb24 == null ? "0" : numb24.Value.ToString("N2")) + ")";

            var groupData = data.GroupBy(x => new { x.FUNCTION })
                .Select(p => new FuelCostByFunctionDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,

                    TOTAL_FUEL_COST_BENEFIT_1 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_BENEFIT_2 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_BENEFIT_3 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_BENEFIT_4 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_BENEFIT_5 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_BENEFIT_6 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_BENEFIT_7 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_BENEFIT_8 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_BENEFIT_9 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_BENEFIT_10 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_BENEFIT_11 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_BENEFIT_12 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_WTC_1 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_WTC_2 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_WTC_3 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_WTC_4 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_WTC_5 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_WTC_6 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_WTC_7 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_WTC_8 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_WTC_9 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_WTC_10 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_WTC_11 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    TOTAL_FUEL_COST_WTC_12 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_FUEL_COST),
                    LABEL1 = label1,
                    LABEL2 = label2,
                    LABEL3 = label3,
                    LABEL4 = label4,
                    LABEL5 = label5,
                    LABEL6 = label6,
                    LABEL7 = label7,
                    LABEL8 = label8,
                    LABEL9 = label9,
                    LABEL10 = label10,
                    LABEL11 = label11,
                    LABEL12 = label12,
                    LABEL13 = label13,
                    LABEL14 = label14,
                    LABEL15 = label15,
                    LABEL16 = label16,
                    LABEL17 = label17,
                    LABEL18 = label18,
                    LABEL19 = label19,
                    LABEL20 = label20,
                    LABEL21 = label21,
                    LABEL22 = label22,
                    LABEL23 = label23,
                    LABEL24 = label24,
                }).OrderBy(x => x.FUNCTION).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult VisualLease(int monthFrom, int? yearFrom, int monthTo, int? yearTo, bool isByRegion, string regional, string functions, string zone)
        {
            var input = new LeaseCostFuncGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthTo;
            input.YearTo = yearTo == null ? 0 : yearTo.Value;
            input.Region = regional;
            input.Function = functions;
            input.ZoneId = zone;
            if (isByRegion)
            {
                input.Function = "Sales,Marketing";
            }
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
            var LocationMappingList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).ToList();
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.Logistic)
            {
                var GetCity = EmployeeList.Where(x => x.EMPLOYEE_ID == CurrentUser.EMPLOYEE_ID && x.IS_ACTIVE).FirstOrDefault();
                if (GetCity != null)
                {
                    var GetRegion = LocationMappingList.Where(x => (x.Location == null ? "" : x.Location.ToUpper()) == (GetCity.CITY == null ? "" : GetCity.CITY.ToUpper())).FirstOrDefault();
                    if (GetRegion != null)
                    {
                        input.ZoneId = GetRegion.ZoneSales;
                    }
                }
            }
            List<LeaseCostByFunctionDto> data = _execSummBLL.GetLeaseCostByFunctionData(input);

            var numb1 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);
            var numb2 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);
            var numb3 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);
            var numb4 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);
            var numb5 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);
            var numb6 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);
            var numb7 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);
            var numb8 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);
            var numb9 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);
            var numb10 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);
            var numb11 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);
            var numb12 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);
            var numb13 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);
            var numb14 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);
            var numb15 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);
            var numb16 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);
            var numb17 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);
            var numb18 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);
            var numb19 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);
            var numb20 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);
            var numb21 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);
            var numb22 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);
            var numb23 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);
            var numb24 = data.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST);

            var label1 = "Sales Jan " + yearFrom + " (" + (numb1 == null ? "0" : numb1.Value.ToString("N2")) + ")";
            var label2 = "Sales Feb " + yearFrom + " (" + (numb2 == null ? "0" : numb2.Value.ToString("N2")) + ")";
            var label3 = "Sales Mar " + yearFrom + " (" + (numb3 == null ? "0" : numb3.Value.ToString("N2")) + ")";
            var label4 = "Sales Apr " + yearFrom + " (" + (numb4 == null ? "0" : numb4.Value.ToString("N2")) + ")";
            var label5 = "Sales May " + yearFrom + " (" + (numb5 == null ? "0" : numb5.Value.ToString("N2")) + ")";
            var label6 = "Sales Jun " + yearFrom + " (" + (numb6 == null ? "0" : numb6.Value.ToString("N2")) + ")";
            var label7 = "Sales Jul " + yearFrom + " (" + (numb7 == null ? "0" : numb7.Value.ToString("N2")) + ")";
            var label8 = "Sales Aug " + yearFrom + " (" + (numb8 == null ? "0" : numb8.Value.ToString("N2")) + ")";
            var label9 = "Sales Sep " + yearFrom + " (" + (numb9 == null ? "0" : numb9.Value.ToString("N2")) + ")";
            var label10 = "Sales Oct " + yearFrom + " (" + (numb10 == null ? "0" : numb10.Value.ToString("N2")) + ")";
            var label11 = "Sales Nov " + yearFrom + " (" + (numb11 == null ? "0" : numb11.Value.ToString("N2")) + ")";
            var label12 = "Sales Dec " + yearFrom + " (" + (numb12 == null ? "0" : numb12.Value.ToString("N2")) + ")";
            var label13 = "Marketing Jan " + yearFrom + " (" + (numb13 == null ? "0" : numb13.Value.ToString("N2")) + ")";
            var label14 = "Marketing Feb " + yearFrom + " (" + (numb14 == null ? "0" : numb14.Value.ToString("N2")) + ")";
            var label15 = "Marketing Mar " + yearFrom + " (" + (numb15 == null ? "0" : numb15.Value.ToString("N2")) + ")";
            var label16 = "Marketing Apr " + yearFrom + " (" + (numb16 == null ? "0" : numb16.Value.ToString("N2")) + ")";
            var label17 = "Marketing May " + yearFrom + " (" + (numb17 == null ? "0" : numb17.Value.ToString("N2")) + ")";
            var label18 = "Marketing Jun " + yearFrom + " (" + (numb18 == null ? "0" : numb18.Value.ToString("N2")) + ")";
            var label19 = "Marketing Jul " + yearFrom + " (" + (numb19 == null ? "0" : numb19.Value.ToString("N2")) + ")";
            var label20 = "Marketing Aug " + yearFrom + " (" + (numb20 == null ? "0" : numb20.Value.ToString("N2")) + ")";
            var label21 = "Marketing Sep " + yearFrom + " (" + (numb21 == null ? "0" : numb21.Value.ToString("N2")) + ")";
            var label22 = "Marketing Oct " + yearFrom + " (" + (numb22 == null ? "0" : numb22.Value.ToString("N2")) + ")";
            var label23 = "Marketing Nov " + yearFrom + " (" + (numb23 == null ? "0" : numb23.Value.ToString("N2")) + ")";
            var label24 = "Marketing Dec " + yearFrom + " (" + (numb24 == null ? "0" : numb24.Value.ToString("N2")) + ")";

            var groupData = data.GroupBy(x => new { x.REGION })
                .Select(p => new LeaseCostByFunctionDto()
                {
                    REGION = (p.FirstOrDefault().REGION == null || p.FirstOrDefault().REGION == "0") ? "HO" : p.FirstOrDefault().REGION,

                    TOTAL_LEASE_COST_SALES_1 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_SALES_2 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_SALES_3 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_SALES_4 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_SALES_5 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_SALES_6 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_SALES_7 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_SALES_8 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_SALES_9 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_SALES_10 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_SALES_11 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_SALES_12 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "SALES"
                                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_MARKETING_1 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_MARKETING_2 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_MARKETING_3 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_MARKETING_4 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_MARKETING_5 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_MARKETING_6 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_MARKETING_7 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_MARKETING_8 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_MARKETING_9 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_MARKETING_10 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_MARKETING_11 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    TOTAL_LEASE_COST_MARKETING_12 = p.Where(x => (x.FUNCTION == null ? "" : x.FUNCTION.ToUpper()) == "MARKETING"
                                                        && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.TOTAL_LEASE_COST),
                    LABEL1 = label1,
                    LABEL2 = label2,
                    LABEL3 = label3,
                    LABEL4 = label4,
                    LABEL5 = label5,
                    LABEL6 = label6,
                    LABEL7 = label7,
                    LABEL8 = label8,
                    LABEL9 = label9,
                    LABEL10 = label10,
                    LABEL11 = label11,
                    LABEL12 = label12,
                    LABEL13 = label13,
                    LABEL14 = label14,
                    LABEL15 = label15,
                    LABEL16 = label16,
                    LABEL17 = label17,
                    LABEL18 = label18,
                    LABEL19 = label19,
                    LABEL20 = label20,
                    LABEL21 = label21,
                    LABEL22 = label22,
                    LABEL23 = label23,
                    LABEL24 = label24,
                }).OrderBy(x => x.REGION).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult VisualSales(int monthFrom, int? yearFrom, int monthTo, int? yearTo, bool isByRegion, string regional, string zone)
        {
            var input = new SalesRegionGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthTo;
            input.YearTo = yearTo == null ? 0 : yearTo.Value;
            input.Region = regional;
            input.ZoneId = zone;
            
            List<SalesByRegionDto> data = _execSummBLL.GetSalesByRegionData(input);

            var numb1 = data.Where(x => x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK);
            var numb2 = data.Where(x => x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK);
            var numb3 = data.Where(x => x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK);
            var numb4 = data.Where(x => x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK);
            var numb5 = data.Where(x => x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK);
            var numb6 = data.Where(x => x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK);
            var numb7 = data.Where(x => x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK);
            var numb8 = data.Where(x => x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK);
            var numb9 = data.Where(x => x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK);
            var numb10 = data.Where(x => x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK);
            var numb11 = data.Where(x => x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK);
            var numb12 = data.Where(x => x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK);
            var numb13 = data.Where(x => x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM);
            var numb14 = data.Where(x => x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM);
            var numb15 = data.Where(x => x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM);
            var numb16 = data.Where(x => x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM);
            var numb17 = data.Where(x => x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM);
            var numb18 = data.Where(x => x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM);
            var numb19 = data.Where(x => x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM);
            var numb20 = data.Where(x => x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM);
            var numb21 = data.Where(x => x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM);
            var numb22 = data.Where(x => x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM);
            var numb23 = data.Where(x => x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM);
            var numb24 = data.Where(x => x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM);

            var label1 = "per Stick Jan " + yearFrom + " (" + (numb1 == null ? "0" : numb1.Value.ToString("N2")) + ")";
            var label2 = "per Stick Feb " + yearFrom + " (" + (numb2 == null ? "0" : numb2.Value.ToString("N2")) + ")";
            var label3 = "per Stick Mar " + yearFrom + " (" + (numb3 == null ? "0" : numb3.Value.ToString("N2")) + ")";
            var label4 = "per Stick Apr " + yearFrom + " (" + (numb4 == null ? "0" : numb4.Value.ToString("N2")) + ")";
            var label5 = "per Stick May " + yearFrom + " (" + (numb5 == null ? "0" : numb5.Value.ToString("N2")) + ")";
            var label6 = "per Stick Jun " + yearFrom + " (" + (numb6 == null ? "0" : numb6.Value.ToString("N2")) + ")";
            var label7 = "per Stick Jul " + yearFrom + " (" + (numb7 == null ? "0" : numb7.Value.ToString("N2")) + ")";
            var label8 = "per Stick Aug " + yearFrom + " (" + (numb8 == null ? "0" : numb8.Value.ToString("N2")) + ")";
            var label9 = "per Stick Sep " + yearFrom + " (" + (numb9 == null ? "0" : numb9.Value.ToString("N2")) + ")";
            var label10 = "per Stick Oct " + yearFrom + " (" + (numb10 == null ? "0" : numb10.Value.ToString("N2")) + ")";
            var label11 = "per Stick Nov " + yearFrom + " (" + (numb11 == null ? "0" : numb11.Value.ToString("N2")) + ")";
            var label12 = "per Stick Dec " + yearFrom + " (" + (numb12 == null ? "0" : numb12.Value.ToString("N2")) + ")";
            var label13 = "per KM Jan " + yearFrom + " (" + (numb13 == null ? "0" : numb13.Value.ToString("N2")) + ")";
            var label14 = "per KM Feb " + yearFrom + " (" + (numb14 == null ? "0" : numb14.Value.ToString("N2")) + ")";
            var label15 = "per KM Mar " + yearFrom + " (" + (numb15 == null ? "0" : numb15.Value.ToString("N2")) + ")";
            var label16 = "per KM Apr " + yearFrom + " (" + (numb16 == null ? "0" : numb16.Value.ToString("N2")) + ")";
            var label17 = "per KM May " + yearFrom + " (" + (numb17 == null ? "0" : numb17.Value.ToString("N2")) + ")";
            var label18 = "per KM Jun " + yearFrom + " (" + (numb18 == null ? "0" : numb18.Value.ToString("N2")) + ")";
            var label19 = "per KM Jul " + yearFrom + " (" + (numb19 == null ? "0" : numb19.Value.ToString("N2")) + ")";
            var label20 = "per KM Aug " + yearFrom + " (" + (numb20 == null ? "0" : numb20.Value.ToString("N2")) + ")";
            var label21 = "per KM Sep " + yearFrom + " (" + (numb21 == null ? "0" : numb21.Value.ToString("N2")) + ")";
            var label22 = "per KM Oct " + yearFrom + " (" + (numb22 == null ? "0" : numb22.Value.ToString("N2")) + ")";
            var label23 = "per KM Nov " + yearFrom + " (" + (numb23 == null ? "0" : numb23.Value.ToString("N2")) + ")";
            var label24 = "per KM Dec " + yearFrom + " (" + (numb24 == null ? "0" : numb24.Value.ToString("N2")) + ")";

            var groupData = data.GroupBy(x => new { x.REGION })
                .Select(p => new SalesByRegionDto()
                {
                    REGION = (p.FirstOrDefault().REGION == null || p.FirstOrDefault().REGION == "0") ? "HO" : p.FirstOrDefault().REGION,

                    TOTAL_COST_1 = p.Where(x => x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM),
                    TOTAL_COST_2 = p.Where(x => x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM),
                    TOTAL_COST_3 = p.Where(x => x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM),
                    TOTAL_COST_4 = p.Where(x => x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM),
                    TOTAL_COST_5 = p.Where(x => x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM),
                    TOTAL_COST_6 = p.Where(x => x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM),
                    TOTAL_COST_7 = p.Where(x => x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM),
                    TOTAL_COST_8 = p.Where(x => x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM),
                    TOTAL_COST_9 = p.Where(x => x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM),
                    TOTAL_COST_10 = p.Where(x => x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM),
                    TOTAL_COST_11 = p.Where(x => x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM),
                    TOTAL_COST_12 = p.Where(x => x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => (c.TOTAL_KM == null || c.TOTAL_KM == 0) ? 0 : c.TOTAL_COST / c.TOTAL_KM),
                    TOTAL_KM_1 = p.Where(x => x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK),
                    TOTAL_KM_2 = p.Where(x => x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK),
                    TOTAL_KM_3 = p.Where(x => x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK),
                    TOTAL_KM_4 = p.Where(x => x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK),
                    TOTAL_KM_5 = p.Where(x => x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK),
                    TOTAL_KM_6 = p.Where(x => x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK),
                    TOTAL_KM_7 = p.Where(x => x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK),
                    TOTAL_KM_8 = p.Where(x => x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK),
                    TOTAL_KM_9 = p.Where(x => x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK),
                    TOTAL_KM_10 = p.Where(x => x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK),
                    TOTAL_KM_11 = p.Where(x => x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK),
                    TOTAL_KM_12 = p.Where(x => x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => (c.STICK == null || c.STICK == 0) ? 0 : c.TOTAL_COST / c.STICK),
                    LABEL1 = label1,
                    LABEL2 = label2,
                    LABEL3 = label3,
                    LABEL4 = label4,
                    LABEL5 = label5,
                    LABEL6 = label6,
                    LABEL7 = label7,
                    LABEL8 = label8,
                    LABEL9 = label9,
                    LABEL10 = label10,
                    LABEL11 = label11,
                    LABEL12 = label12,
                    LABEL13 = label13,
                    LABEL14 = label14,
                    LABEL15 = label15,
                    LABEL16 = label16,
                    LABEL17 = label17,
                    LABEL18 = label18,
                    LABEL19 = label19,
                    LABEL20 = label20,
                    LABEL21 = label21,
                    LABEL22 = label22,
                    LABEL23 = label23,
                    LABEL24 = label24,
                }).OrderBy(x => x.REGION).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult VisualAccident(int monthFrom, int? yearFrom, int monthTo, int? yearTo, bool isByRegion, string vehType, string regional, string functions, string zone)
        {
            var input = new AccidentGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthTo;
            input.YearTo = yearTo == null ? 0 : yearTo.Value;
            input.VehicleType = vehType;
            input.Region = regional;
            input.Function = functions;
            input.ZoneId = zone;
            if (isByRegion)
            {
                input.Function = "Sales,Marketing";
            }
            List<AccidentDto> data = _execSummBLL.GetAccidentData(input);

            var label1 = "BENEFIT Jan " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var label2 = "BENEFIT Feb " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var label3 = "BENEFIT Mar " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var label4 = "BENEFIT Apr " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var label5 = "BENEFIT May " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var label6 = "BENEFIT Jun " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var label7 = "BENEFIT Jul " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var label8 = "BENEFIT Aug " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var label9 = "BENEFIT Sep " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var label10 = "BENEFIT Oct " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var label11 = "BENEFIT Nov " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var label12 = "BENEFIT Dec " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var label13 = "WTC Jan " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var label14 = "WTC Feb " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var label15 = "WTC Mar " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var label16 = "WTC Apr " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var label17 = "WTC May " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var label18 = "WTC Jun " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var label19 = "WTC Jul " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var label20 = "WTC Aug " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var label21 = "WTC Sep " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var label22 = "WTC Oct " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var label23 = "WTC Nov " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var label24 = "WTC Dec " + yearFrom + " (" + data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(x => x.ACCIDENT_COUNT) + ")";

            var groupData = data.GroupBy(x => new { x.FUNCTION })
                .Select(p => new AccidentDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    ACCIDENT_COUNT_BENEFIT_1 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_BENEFIT_2 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_BENEFIT_3 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_BENEFIT_4 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_BENEFIT_5 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_BENEFIT_6 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_BENEFIT_7 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_BENEFIT_8 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_BENEFIT_9 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_BENEFIT_10 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_BENEFIT_11 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_BENEFIT_12 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_WTC_1 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_WTC_2 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_WTC_3 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_WTC_4 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_WTC_5 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_WTC_6 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_WTC_7 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_WTC_8 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_WTC_9 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_WTC_10 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_WTC_11 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    ACCIDENT_COUNT_WTC_12 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACCIDENT_COUNT),
                    LABEL1 = label1,
                    LABEL2 = label2,
                    LABEL3 = label3,
                    LABEL4 = label4,
                    LABEL5 = label5,
                    LABEL6 = label6,
                    LABEL7 = label7,
                    LABEL8 = label8,
                    LABEL9 = label9,
                    LABEL10 = label10,
                    LABEL11 = label11,
                    LABEL12 = label12,
                    LABEL13 = label13,
                    LABEL14 = label14,
                    LABEL15 = label15,
                    LABEL16 = label16,
                    LABEL17 = label17,
                    LABEL18 = label18,
                    LABEL19 = label19,
                    LABEL20 = label20,
                    LABEL21 = label21,
                    LABEL22 = label22,
                    LABEL23 = label23,
                    LABEL24 = label24,
                }).OrderBy(x => x.FUNCTION).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult VisualAcOb(int monthFrom, int? yearFrom, int monthTo, int? yearTo, bool isByRegion, string functions, string vehType)
        {
            var input = new AcVsObGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthTo;
            input.YearTo = yearTo == null ? 0 : yearTo.Value;
            input.Function = functions;
            input.VehicleType = vehType;
            if (isByRegion)
            {
                input.Function = "Sales,Marketing";
            }
            List<AcVsObDto> data = _execSummBLL.GetAcVsObData(input);

            var numb1 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb2 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb3 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb4 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb5 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb6 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb7 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb8 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb9 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb10 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb11 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb12 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb13 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);
            var numb14 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);
            var numb15 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);
            var numb16 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);
            var numb17 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);
            var numb18 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);
            var numb19 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);
            var numb20 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);
            var numb21 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);
            var numb22 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);
            var numb23 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);
            var numb24 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);
            var numb25 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb26 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb27 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb28 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb29 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb30 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb31 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb32 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb33 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb34 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb35 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb36 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST);
            var numb37 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);
            var numb38 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);
            var numb39 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);
            var numb40 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);
            var numb41 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);
            var numb42 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);
            var numb43 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);
            var numb44 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);
            var numb45 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);
            var numb46 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);
            var numb47 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);
            var numb48 = data.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB);

            var label1 = "Actual BENEFIT Jan " + yearFrom + " (" + (numb1 == null ? "0" : numb1.Value.ToString("N2")) + ")";
            var label2 = "Actual BENEFIT Feb " + yearFrom + " (" + (numb2 == null ? "0" : numb2.Value.ToString("N2")) + ")";
            var label3 = "Actual BENEFIT Mar " + yearFrom + " (" + (numb3 == null ? "0" : numb3.Value.ToString("N2")) + ")";
            var label4 = "Actual BENEFIT Apr " + yearFrom + " (" + (numb4 == null ? "0" : numb4.Value.ToString("N2")) + ")";
            var label5 = "Actual BENEFIT May " + yearFrom + " (" + (numb5 == null ? "0" : numb5.Value.ToString("N2")) + ")";
            var label6 = "Actual BENEFIT Jun " + yearFrom + " (" + (numb6 == null ? "0" : numb6.Value.ToString("N2")) + ")";
            var label7 = "Actual BENEFIT Jul " + yearFrom + " (" + (numb7 == null ? "0" : numb7.Value.ToString("N2")) + ")";
            var label8 = "Actual BENEFIT Aug " + yearFrom + " (" + (numb8 == null ? "0" : numb8.Value.ToString("N2")) + ")";
            var label9 = "Actual BENEFIT Sep " + yearFrom + " (" + (numb9 == null ? "0" : numb9.Value.ToString("N2")) + ")";
            var label10 = "Actual BENEFIT Oct " + yearFrom + " (" + (numb10 == null ? "0" : numb10.Value.ToString("N2")) + ")";
            var label11 = "Actual BENEFIT Nov " + yearFrom + " (" + (numb11 == null ? "0" : numb11.Value.ToString("N2")) + ")";
            var label12 = "Actual BENEFIT Dec " + yearFrom + " (" + (numb12 == null ? "0" : numb12.Value.ToString("N2")) + ")";
            var label13 = "Budget BENEFIT Jan " + yearFrom + " (" + (numb13 == null ? "0" : numb13.Value.ToString("N2")) + ")";
            var label14 = "Budget BENEFIT Feb " + yearFrom + " (" + (numb14 == null ? "0" : numb14.Value.ToString("N2")) + ")";
            var label15 = "Budget BENEFIT Mar " + yearFrom + " (" + (numb15 == null ? "0" : numb15.Value.ToString("N2")) + ")";
            var label16 = "Budget BENEFIT Apr " + yearFrom + " (" + (numb16 == null ? "0" : numb16.Value.ToString("N2")) + ")";
            var label17 = "Budget BENEFIT May " + yearFrom + " (" + (numb17 == null ? "0" : numb17.Value.ToString("N2")) + ")";
            var label18 = "Budget BENEFIT Jun " + yearFrom + " (" + (numb18 == null ? "0" : numb18.Value.ToString("N2")) + ")";
            var label19 = "Budget BENEFIT Jul " + yearFrom + " (" + (numb19 == null ? "0" : numb19.Value.ToString("N2")) + ")";
            var label20 = "Budget BENEFIT Aug " + yearFrom + " (" + (numb20 == null ? "0" : numb20.Value.ToString("N2")) + ")";
            var label21 = "Budget BENEFIT Sep " + yearFrom + " (" + (numb21 == null ? "0" : numb21.Value.ToString("N2")) + ")";
            var label22 = "Budget BENEFIT Oct " + yearFrom + " (" + (numb22 == null ? "0" : numb22.Value.ToString("N2")) + ")";
            var label23 = "Budget BENEFIT Nov " + yearFrom + " (" + (numb23 == null ? "0" : numb23.Value.ToString("N2")) + ")";
            var label24 = "Budget BENEFIT Dec " + yearFrom + " (" + (numb24 == null ? "0" : numb24.Value.ToString("N2")) + ")";
            var label25 = "Actual WTC Jan " + yearFrom + " (" + (numb25 == null ? "0" : numb25.Value.ToString("N2")) + ")";
            var label26 = "Actual WTC Feb " + yearFrom + " (" + (numb26 == null ? "0" : numb26.Value.ToString("N2")) + ")";
            var label27 = "Actual WTC Mar " + yearFrom + " (" + (numb27 == null ? "0" : numb27.Value.ToString("N2")) + ")";
            var label28 = "Actual WTC Apr " + yearFrom + " (" + (numb28 == null ? "0" : numb28.Value.ToString("N2")) + ")";
            var label29 = "Actual WTC May " + yearFrom + " (" + (numb29 == null ? "0" : numb29.Value.ToString("N2")) + ")";
            var label30 = "Actual WTC Jun " + yearFrom + " (" + (numb30 == null ? "0" : numb30.Value.ToString("N2")) + ")";
            var label31 = "Actual WTC Jul " + yearFrom + " (" + (numb31 == null ? "0" : numb31.Value.ToString("N2")) + ")";
            var label32 = "Actual WTC Aug " + yearFrom + " (" + (numb32 == null ? "0" : numb32.Value.ToString("N2")) + ")";
            var label33 = "Actual WTC Sep " + yearFrom + " (" + (numb33 == null ? "0" : numb33.Value.ToString("N2")) + ")";
            var label34 = "Actual WTC Oct " + yearFrom + " (" + (numb34 == null ? "0" : numb34.Value.ToString("N2")) + ")";
            var label35 = "Actual WTC Nov " + yearFrom + " (" + (numb35 == null ? "0" : numb35.Value.ToString("N2")) + ")";
            var label36 = "Actual WTC Dec " + yearFrom + " (" + (numb36 == null ? "0" : numb36.Value.ToString("N2")) + ")";
            var label37 = "Budget WTC Jan " + yearFrom + " (" + (numb37 == null ? "0" : numb37.Value.ToString("N2")) + ")";
            var label38 = "Budget WTC Feb " + yearFrom + " (" + (numb38 == null ? "0" : numb38.Value.ToString("N2")) + ")";
            var label39 = "Budget WTC Mar " + yearFrom + " (" + (numb39 == null ? "0" : numb39.Value.ToString("N2")) + ")";
            var label40 = "Budget WTC Apr " + yearFrom + " (" + (numb40 == null ? "0" : numb40.Value.ToString("N2")) + ")";
            var label41 = "Budget WTC May " + yearFrom + " (" + (numb41 == null ? "0" : numb41.Value.ToString("N2")) + ")";
            var label42 = "Budget WTC Jun " + yearFrom + " (" + (numb42 == null ? "0" : numb42.Value.ToString("N2")) + ")";
            var label43 = "Budget WTC Jul " + yearFrom + " (" + (numb43 == null ? "0" : numb43.Value.ToString("N2")) + ")";
            var label44 = "Budget WTC Aug " + yearFrom + " (" + (numb44 == null ? "0" : numb44.Value.ToString("N2")) + ")";
            var label45 = "Budget WTC Sep " + yearFrom + " (" + (numb45 == null ? "0" : numb45.Value.ToString("N2")) + ")";
            var label46 = "Budget WTC Oct " + yearFrom + " (" + (numb46 == null ? "0" : numb46.Value.ToString("N2")) + ")";
            var label47 = "Budget WTC Nov " + yearFrom + " (" + (numb47 == null ? "0" : numb47.Value.ToString("N2")) + ")";
            var label48 = "Budget WTC Dec " + yearFrom + " (" + (numb48 == null ? "0" : numb48.Value.ToString("N2")) + ")";

            var groupData = data.GroupBy(x => new { x.FUNCTION })
                .Select(p => new AcVsObDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,

                    ACTUAL_COST_BENEFIT_1 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    ACTUAL_COST_BENEFIT_2 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    ACTUAL_COST_BENEFIT_3 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    ACTUAL_COST_BENEFIT_4 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    ACTUAL_COST_BENEFIT_5 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    ACTUAL_COST_BENEFIT_6 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    ACTUAL_COST_BENEFIT_7 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    ACTUAL_COST_BENEFIT_8 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    ACTUAL_COST_BENEFIT_9 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    ACTUAL_COST_BENEFIT_10 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    ACTUAL_COST_BENEFIT_11 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    ACTUAL_COST_BENEFIT_12 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    COST_OB_BENEFIT_1 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                        && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    COST_OB_BENEFIT_2 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                        && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    COST_OB_BENEFIT_3 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                        && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    COST_OB_BENEFIT_4 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                        && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    COST_OB_BENEFIT_5 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                        && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    COST_OB_BENEFIT_6 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                        && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    COST_OB_BENEFIT_7 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                        && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    COST_OB_BENEFIT_8 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                        && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    COST_OB_BENEFIT_9 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                        && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    COST_OB_BENEFIT_10 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                        && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    COST_OB_BENEFIT_11 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                        && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    COST_OB_BENEFIT_12 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                                        && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    ACTUAL_COST_WTC_1 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                            && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    ACTUAL_COST_WTC_2 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                            && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    ACTUAL_COST_WTC_3 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                            && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    ACTUAL_COST_WTC_4 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                            && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    ACTUAL_COST_WTC_5 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                            && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    ACTUAL_COST_WTC_6 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                            && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    ACTUAL_COST_WTC_7 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                            && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    ACTUAL_COST_WTC_8 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                            && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    ACTUAL_COST_WTC_9 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                            && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    ACTUAL_COST_WTC_10 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                            && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    ACTUAL_COST_WTC_11 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                            && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    ACTUAL_COST_WTC_12 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                            && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.ACTUAL_COST),
                    COST_OB_WTC_1 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    COST_OB_WTC_2 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    COST_OB_WTC_3 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    COST_OB_WTC_4 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    COST_OB_WTC_5 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    COST_OB_WTC_6 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    COST_OB_WTC_7 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    COST_OB_WTC_8 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    COST_OB_WTC_9 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    COST_OB_WTC_10 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    COST_OB_WTC_11 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    COST_OB_WTC_12 = p.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC"
                                                        && x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.COST_OB),
                    LABEL1 = label1,
                    LABEL2 = label2,
                    LABEL3 = label3,
                    LABEL4 = label4,
                    LABEL5 = label5,
                    LABEL6 = label6,
                    LABEL7 = label7,
                    LABEL8 = label8,
                    LABEL9 = label9,
                    LABEL10 = label10,
                    LABEL11 = label11,
                    LABEL12 = label12,
                    LABEL13 = label13,
                    LABEL14 = label14,
                    LABEL15 = label15,
                    LABEL16 = label16,
                    LABEL17 = label17,
                    LABEL18 = label18,
                    LABEL19 = label19,
                    LABEL20 = label20,
                    LABEL21 = label21,
                    LABEL22 = label22,
                    LABEL23 = label23,
                    LABEL24 = label24,
                    LABEL25 = label25,
                    LABEL26 = label26,
                    LABEL27 = label27,
                    LABEL28 = label28,
                    LABEL29 = label29,
                    LABEL30 = label30,
                    LABEL31 = label31,
                    LABEL32 = label32,
                    LABEL33 = label33,
                    LABEL34 = label34,
                    LABEL35 = label35,
                    LABEL36 = label36,
                    LABEL37 = label37,
                    LABEL38 = label38,
                    LABEL39 = label39,
                    LABEL40 = label40,
                    LABEL41 = label41,
                    LABEL42 = label42,
                    LABEL43 = label43,
                    LABEL44 = label44,
                    LABEL45 = label45,
                    LABEL46 = label46,
                    LABEL47 = label47,
                    LABEL48 = label48,
                }).OrderBy(x => x.FUNCTION).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult VisualAcObUnit(int monthFrom, int? yearFrom, int monthTo, int? yearTo, bool isByRegion, string functions, string vehType)
        {
            var input = new AcVsObGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthTo;
            input.YearTo = yearTo == null ? 0 : yearTo.Value;
            input.Function = functions;
            input.VehicleType = vehType;
            if (isByRegion)
            {
                input.Function = "Sales,Marketing";
            }
            List<AcVsObDto> data = _execSummBLL.GetAcVsObUnitData(input);

            var numb1 = data.Where(x => x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT);
            var numb2 = data.Where(x => x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT);
            var numb3 = data.Where(x => x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT);
            var numb4 = data.Where(x => x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT);
            var numb5 = data.Where(x => x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT);
            var numb6 = data.Where(x => x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT);
            var numb7 = data.Where(x => x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT);
            var numb8 = data.Where(x => x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT);
            var numb9 = data.Where(x => x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT);
            var numb10 = data.Where(x => x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT);
            var numb11 = data.Where(x => x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT);
            var numb12 = data.Where(x => x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT);
            var numb13 = data.Where(x => x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET);
            var numb14 = data.Where(x => x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET);
            var numb15 = data.Where(x => x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET);
            var numb16 = data.Where(x => x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET);
            var numb17 = data.Where(x => x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET);
            var numb18 = data.Where(x => x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET);
            var numb19 = data.Where(x => x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET);
            var numb20 = data.Where(x => x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET);
            var numb21 = data.Where(x => x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET);
            var numb22 = data.Where(x => x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET);
            var numb23 = data.Where(x => x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET);
            var numb24 = data.Where(x => x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET);

            var label1 = "Unit Jan " + yearFrom + " (" + (numb1 == null ? "0" : numb1.Value.ToString()) + ")";
            var label2 = "Unit Feb " + yearFrom + " (" + (numb2 == null ? "0" : numb2.Value.ToString()) + ")";
            var label3 = "Unit Mar " + yearFrom + " (" + (numb3 == null ? "0" : numb3.Value.ToString()) + ")";
            var label4 = "Unit Apr " + yearFrom + " (" + (numb4 == null ? "0" : numb4.Value.ToString()) + ")";
            var label5 = "Unit May " + yearFrom + " (" + (numb5 == null ? "0" : numb5.Value.ToString()) + ")";
            var label6 = "Unit Jun " + yearFrom + " (" + (numb6 == null ? "0" : numb6.Value.ToString()) + ")";
            var label7 = "Unit Jul " + yearFrom + " (" + (numb7 == null ? "0" : numb7.Value.ToString()) + ")";
            var label8 = "Unit Aug " + yearFrom + " (" + (numb8 == null ? "0" : numb8.Value.ToString()) + ")";
            var label9 = "Unit Sep " + yearFrom + " (" + (numb9 == null ? "0" : numb9.Value.ToString()) + ")";
            var label10 = "Unit Oct " + yearFrom + " (" + (numb10 == null ? "0" : numb10.Value.ToString()) + ")";
            var label11 = "Unit Nov " + yearFrom + " (" + (numb11 == null ? "0" : numb11.Value.ToString()) + ")";
            var label12 = "Unit Dec " + yearFrom + " (" + (numb12 == null ? "0" : numb12.Value.ToString()) + ")";
            var label13 = "Unit Budget Jan " + yearFrom + " (" + (numb13 == null ? "0" : numb13.Value.ToString()) + ")";
            var label14 = "Unit Budget Feb " + yearFrom + " (" + (numb14 == null ? "0" : numb14.Value.ToString()) + ")";
            var label15 = "Unit Budget Mar " + yearFrom + " (" + (numb15 == null ? "0" : numb15.Value.ToString()) + ")";
            var label16 = "Unit Budget Apr " + yearFrom + " (" + (numb16 == null ? "0" : numb16.Value.ToString()) + ")";
            var label17 = "Unit Budget May " + yearFrom + " (" + (numb17 == null ? "0" : numb17.Value.ToString()) + ")";
            var label18 = "Unit Budget Jun " + yearFrom + " (" + (numb18 == null ? "0" : numb18.Value.ToString()) + ")";
            var label19 = "Unit Budget Jul " + yearFrom + " (" + (numb19 == null ? "0" : numb19.Value.ToString()) + ")";
            var label20 = "Unit Budget Aug " + yearFrom + " (" + (numb20 == null ? "0" : numb20.Value.ToString()) + ")";
            var label21 = "Unit Budget Sep " + yearFrom + " (" + (numb21 == null ? "0" : numb21.Value.ToString()) + ")";
            var label22 = "Unit Budget Oct " + yearFrom + " (" + (numb22 == null ? "0" : numb22.Value.ToString()) + ")";
            var label23 = "Unit Budget Nov " + yearFrom + " (" + (numb23 == null ? "0" : numb23.Value.ToString()) + ")";
            var label24 = "Unit Budget Dec " + yearFrom + " (" + (numb24 == null ? "0" : numb24.Value.ToString()) + ")";

            var groupData = data.GroupBy(x => new { x.FUNCTION })
                .Select(p => new AcVsObDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,

                    UNIT_1 = p.Where(x => x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT),
                    UNIT_2 = p.Where(x => x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT),
                    UNIT_3 = p.Where(x => x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT),
                    UNIT_4 = p.Where(x => x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT),
                    UNIT_5 = p.Where(x => x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT),
                    UNIT_6 = p.Where(x => x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT),
                    UNIT_7 = p.Where(x => x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT),
                    UNIT_8 = p.Where(x => x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT),
                    UNIT_9 = p.Where(x => x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT),
                    UNIT_10 = p.Where(x => x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT),
                    UNIT_11 = p.Where(x => x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT),
                    UNIT_12 = p.Where(x => x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT),
                    UNIT_BUDGET_1 = p.Where(x => x.REPORT_MONTH == 1 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET),
                    UNIT_BUDGET_2 = p.Where(x => x.REPORT_MONTH == 2 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET),
                    UNIT_BUDGET_3 = p.Where(x => x.REPORT_MONTH == 3 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET),
                    UNIT_BUDGET_4 = p.Where(x => x.REPORT_MONTH == 4 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET),
                    UNIT_BUDGET_5 = p.Where(x => x.REPORT_MONTH == 5 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET),
                    UNIT_BUDGET_6 = p.Where(x => x.REPORT_MONTH == 6 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET),
                    UNIT_BUDGET_7 = p.Where(x => x.REPORT_MONTH == 7 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET),
                    UNIT_BUDGET_8 = p.Where(x => x.REPORT_MONTH == 8 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET),
                    UNIT_BUDGET_9 = p.Where(x => x.REPORT_MONTH == 9 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET),
                    UNIT_BUDGET_10 = p.Where(x => x.REPORT_MONTH == 10 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET),
                    UNIT_BUDGET_11 = p.Where(x => x.REPORT_MONTH == 11 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET),
                    UNIT_BUDGET_12 = p.Where(x => x.REPORT_MONTH == 12 && x.REPORT_YEAR == yearFrom).Sum(c => c.UNIT_BUDGET),
                    LABEL1 = label1,
                    LABEL2 = label2,
                    LABEL3 = label3,
                    LABEL4 = label4,
                    LABEL5 = label5,
                    LABEL6 = label6,
                    LABEL7 = label7,
                    LABEL8 = label8,
                    LABEL9 = label9,
                    LABEL10 = label10,
                    LABEL11 = label11,
                    LABEL12 = label12,
                    LABEL13 = label13,
                    LABEL14 = label14,
                    LABEL15 = label15,
                    LABEL16 = label16,
                    LABEL17 = label17,
                    LABEL18 = label18,
                    LABEL19 = label19,
                    LABEL20 = label20,
                    LABEL21 = label21,
                    LABEL22 = label22,
                    LABEL23 = label23,
                    LABEL24 = label24,
                }).OrderBy(x => x.FUNCTION).ToList();

            return Json(groupData);
        }

        #endregion

        #region --------- Summary All --------------

        public ActionResult SummaryAll()
        {
            var model = new SummaryAllModel();
            model.SearchView.YearFrom = DateTime.Now.Year;

            model.TitleForm = "Executive Summary All";
            model.TitleExport = "ExportSummaryAll";
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View("SummaryAllNew", model);
        }

        #region --------- Export --------------
        public string ExportSummaryAllReportGenerateReport(SummaryAllModel model = null)
        {
            string pathFile = "";
            if (model.TitleExport == "ExportSummaryAll")
            {
                pathFile = CreateXlsSummaryAll(model.SearchViewExport);
            }
            else
            {
                pathFile = CreateXlsSummaryRegion(model.SearchViewExport);
            }
            
            return pathFile;
        }
        public void GetExcelFile(string pathFile)
        {
            var newFile = new FileInfo(pathFile);
            var fileName = Path.GetFileName(pathFile);
            string attachment = string.Format("attachment; filename={0}", fileName);
            Response.Clear();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.WriteFile(newFile.FullName);
            Response.Flush();
            newFile.Delete();
            Response.End();
        }
        private string CreateXlsSummaryAll(SummarySearchViewExport inputExport)
        {
            //get data
            var input = Mapper.Map<VehicleGetByParamInput>(inputExport);
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager
                        || CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager
                        || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                input.VehicleType = "WTC";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.HR || CurrentUser.UserRole == Enums.UserRole.HRManager)
            {
                input.VehicleType = "BENEFIT";
            }

            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager)
            {
                input.Function = "Sales,Marketing";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager)
            {
                input.Function = "Operations";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                input.Function = "Logistic";
            }
            List<NoVehicleDto> data = _execSummBLL.GetNoOfVehicleData(input);
            var listData = Mapper.Map<List<NoVehicleData>>(data);

            var inputWtc = Mapper.Map<VehicleWtcGetByParamInput>(inputExport);
            inputWtc.Function = "Sales,Marketing";
            List<NoVehicleWtcDto> dataWtc = _execSummBLL.GetNoOfVehicleWtcData(inputWtc);
            var listDataWtc = Mapper.Map<List<NoVehicleWtcData>>(dataWtc);

            var inputMake = Mapper.Map<VehicleMakeGetByParamInput>(inputExport);
            List<NoVehicleMakeDto> dataMake = _execSummBLL.GetNoOfVehicleMakeData(inputMake);
            var listDataMake = Mapper.Map<List<NoVehicleMakeData>>(dataMake);

            var inputOdo = Mapper.Map<OdometerGetByParamInput>(inputExport);
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager
                        || CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager
                        || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                inputOdo.VehicleType = "WTC";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.HR || CurrentUser.UserRole == Enums.UserRole.HRManager)
            {
                inputOdo.VehicleType = "BENEFIT";
            }

            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager)
            {
                inputOdo.Function = "Sales,Marketing";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager)
            {
                inputOdo.Function = "Operations";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                inputOdo.Function = "Logistic";
            }
            List<OdometerDto> dataOdo = _execSummBLL.GetOdometerData(inputOdo);
            var listDataOdo = Mapper.Map<List<OdometerData>>(dataOdo);

            var inputLiter = Mapper.Map<LiterFuncGetByParamInput>(inputExport);
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager
                        || CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager
                        || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                inputLiter.VehicleType = "WTC";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.HR || CurrentUser.UserRole == Enums.UserRole.HRManager)
            {
                inputLiter.VehicleType = "BENEFIT";
            }

            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager)
            {
                inputLiter.Function = "Sales,Marketing";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager)
            {
                inputLiter.Function = "Operations";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                inputLiter.Function = "Logistic";
            }
            List<LiterByFunctionDto> dataLiter = _execSummBLL.GetLiterByFunctionData(inputLiter);
            var listDataLiter = Mapper.Map<List<LiterByFunctionData>>(dataLiter);

            var inputFuel = Mapper.Map<FuelCostFuncGetByParamInput>(inputExport);
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager
                        || CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager
                        || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                inputFuel.VehicleType = "WTC";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.HR || CurrentUser.UserRole == Enums.UserRole.HRManager)
            {
                inputFuel.VehicleType = "BENEFIT";
            }

            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager)
            {
                inputFuel.Function = "Sales,Marketing";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager)
            {
                inputFuel.Function = "Operations";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                inputFuel.Function = "Logistic";
            }
            List<FuelCostByFunctionDto> dataFuel = _execSummBLL.GetFuelCostByFunctionData(inputFuel);
            var listDataFuel = Mapper.Map<List<FuelCostByFunctionData>>(dataFuel);

            var inputLease = Mapper.Map<LeaseCostFuncGetByParamInput>(inputExport);
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager)
            {
                inputLease.Function = "Sales,Marketing";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager)
            {
                inputLease.Function = "Operations";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                inputLease.Function = "Logistic";
            }
            List<LeaseCostByFunctionDto> dataLease = _execSummBLL.GetLeaseCostByFunctionData(inputLease);
            var listDataLease = Mapper.Map<List<LeaseCostByFunctionData>>(dataLease);

            var inputSales = Mapper.Map<SalesRegionGetByParamInput>(inputExport);
            List<SalesByRegionDto> dataSales = _execSummBLL.GetSalesByRegionData(inputSales);
            var listDataSales = Mapper.Map<List<SalesByRegionData>>(dataSales);

            var inputAccident = Mapper.Map<AccidentGetByParamInput>(inputExport);
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager
                        || CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager
                        || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                inputAccident.VehicleType = "WTC";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.HR || CurrentUser.UserRole == Enums.UserRole.HRManager)
            {
                inputAccident.VehicleType = "BENEFIT";
            }

            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager)
            {
                inputAccident.Function = "Sales,Marketing";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager)
            {
                inputAccident.Function = "Operations";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                inputAccident.Function = "Logistic";
            }
            List<AccidentDto> dataAccident = _execSummBLL.GetAccidentData(inputAccident);
            var listDataAccident = Mapper.Map<List<AccidentData>>(dataAccident);

            var inputAcOb = Mapper.Map<AcVsObGetByParamInput>(inputExport);
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.ComFinanceManager)
            {
                inputAcOb.Function = "Sales,Marketing";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.OpsFinanceManager)
            {
                inputAcOb.Function = "Operations";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Logistic || CurrentUser.UserRole == Enums.UserRole.LDManager)
            {
                inputAcOb.Function = "Logistic";
            }
            List<AcVsObDto> dataAcOb = _execSummBLL.GetAcVsObData(inputAcOb);
            var listDataAcOb = Mapper.Map<List<AcVsObData>>(dataAcOb);

            List<AcVsObDto> dataAcObUnit = _execSummBLL.GetAcVsObUnitData(inputAcOb);
            var listDataAcObUnit = Mapper.Map<List<AcVsObData>>(dataAcObUnit);

            var slDocument = new SLDocument();

            //title no of vehicle
            slDocument.SetCellValue(1, 2, "Executive Summary " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + "-" + inputExport.YearFrom);
            slDocument.MergeWorksheetCells(1, 2, 1, 10);
            slDocument.RenameWorksheet("Sheet1", "Number Of Vehicle");
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 14;
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Aqua, System.Drawing.Color.Aqua);
            slDocument.SetCellStyle(1, 2, 1, 10, valueStyle);

            slDocument = CreateDataExcelSheet1(slDocument, listData);


            if (CurrentUser.UserRole != Enums.UserRole.HR && CurrentUser.UserRole != Enums.UserRole.HRManager
                && CurrentUser.UserRole != Enums.UserRole.OpsFinanceManager && CurrentUser.UserRole != Enums.UserRole.LDManager
                 && CurrentUser.UserRole != Enums.UserRole.Logistic)
            { 
                //title no of vehicle wtc
                slDocument.AddWorksheet("Vehicle By Regional");
                slDocument.SetCellValue(1, 2, "Executive Summary " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + "-" + inputExport.YearFrom);
                slDocument.MergeWorksheetCells(1, 2, 1, 10);
                slDocument.SetCellStyle(1, 2, 1, 10, valueStyle);

                slDocument = CreateDataExcelSheet2(slDocument, listDataWtc);
            }

            //title no of vehicle Make
            slDocument.AddWorksheet("Make Type");
            slDocument.SetCellValue(1, 2, "Executive Summary " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + "-" + inputExport.YearFrom);
            slDocument.MergeWorksheetCells(1, 2, 1, 16);
            slDocument.SetCellStyle(1, 2, 1, 16, valueStyle);

            slDocument = CreateDataExcelSheet3(slDocument, listDataMake);


            //title Odometer
            slDocument.AddWorksheet("Odometer");
            slDocument.SetCellValue(1, 2, "Executive Summary " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + "-" + inputExport.YearFrom);
            slDocument.MergeWorksheetCells(1, 2, 1, 10);
            //create style
            slDocument.SetCellStyle(1, 2, 1, 10, valueStyle);

            slDocument = CreateDataExcelSheet4(slDocument, listDataOdo);


            //title Liter By Function
            slDocument.AddWorksheet("Liters");
            slDocument.SetCellValue(1, 2, "Executive Summary " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + "-" + inputExport.YearFrom);
            slDocument.MergeWorksheetCells(1, 2, 1, 10);
            slDocument.SetCellStyle(1, 2, 1, 10, valueStyle);

            slDocument = CreateDataExcelSheet5(slDocument, listDataLiter);


            //title Fuel & Lease Cost By Function
            slDocument.AddWorksheet("Fuel & Lease Cost");
            slDocument.SetCellValue(1, 2, "Executive Summary " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + "-" + inputExport.YearFrom);
            slDocument.MergeWorksheetCells(1, 2, 1, 10);
            slDocument.SetCellStyle(1, 2, 1, 10, valueStyle);

            slDocument = CreateDataExcelSheet6(slDocument, listDataFuel, listDataLease);


            //title Sales By Region
            slDocument.AddWorksheet("Operational Cost");
            slDocument.SetCellValue(1, 2, "Executive Summary " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + "-" + inputExport.YearFrom);
            slDocument.MergeWorksheetCells(1, 2, 1, 10);
            slDocument.SetCellStyle(1, 2, 1, 10, valueStyle);

            slDocument = CreateDataExcelSheet7(slDocument, listDataSales);


            //title Accident
            slDocument.AddWorksheet("Accident");
            slDocument.SetCellValue(1, 2, "Executive Summary " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + "-" + inputExport.YearFrom);
            slDocument.MergeWorksheetCells(1, 2, 1, 10);
            slDocument.SetCellStyle(1, 2, 1, 10, valueStyle);

            slDocument = CreateDataExcelSheet9(slDocument, listDataAccident);


            //title AC Vs OB
            slDocument.AddWorksheet("Actual Cost Vs Budget");
            slDocument.SetCellValue(1, 2, "Executive Summary " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + "-" + inputExport.YearFrom);
            slDocument.MergeWorksheetCells(1, 2, 1, 10);
            slDocument.SetCellStyle(1, 2, 1, 10, valueStyle);

            slDocument = CreateDataExcelSheet8(slDocument, listDataAcOb);

            //title AC Vs OB Unit
            slDocument.AddWorksheet("Actual Unit Vs Budget");
            slDocument.SetCellValue(1, 2, "Executive Summary " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + "-" + inputExport.YearFrom);
            slDocument.MergeWorksheetCells(1, 2, 1, 10);
            slDocument.SetCellStyle(1, 2, 1, 10, valueStyle);

            slDocument = CreateDataExcelSheet13(slDocument, listDataAcObUnit);


            //title Sum Total
            slDocument.AddWorksheet("Sum Total");

            slDocument = CreateDataExcelSheet10(slDocument, listData, listDataOdo, listDataLiter, listDataFuel, listDataLease, listDataAccident, inputExport);


            //title Sum Benefit
            slDocument.AddWorksheet("Sum Benefit");

            slDocument = CreateDataExcelSheet11(slDocument, listData, listDataOdo, listDataLiter, listDataFuel, listDataLease, listDataAccident, inputExport);

            //title Sum Working Tool Car
            slDocument.AddWorksheet("Sum Working Tool Car");

            slDocument = CreateDataExcelSheet12(slDocument, listData, listDataOdo, listDataLiter, listDataFuel, listDataLease, listDataAccident, inputExport);
            

            var fileName = "ExecSum_SummaryAll" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        #endregion

        #region ------------ New Export Summary All ----------

        private SLDocument CreateDataExcelSheet1(SLDocument slDocument, List<NoVehicleData> listData)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var total1 = 0;
            var total2 = 0;

            //select distinct data
            var dataList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY FUNCTION");
            slDocument.SetCellValue(firstRow, firstColumn + 1, "BENEFIT");
            slDocument.SetCellValue(firstRow, firstColumn + 2, "WTC");

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "No Function" : item);

                var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT"
                                                        && x.Function == item).Sum(x => x.NoOfVehicle);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 1, countData.ToString());

                var countData2 = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC"
                                                        && x.Function == item).Sum(x => x.NoOfVehicle);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 2, countData2.ToString());

                total1 += countData == null ? 0 : countData.Value;
                total2 += countData2 == null ? 0 : countData2.Value;

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");
            slDocument.SetCellValue(contRow, firstColumn + 1, total1);
            slDocument.SetCellValue(contRow, firstColumn + 2, total2);

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.AutoFitColumn(firstColumn);
            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, 10, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, 10, headerStyleNumbChart);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, firstColumn + 2);
            chart.SetChartStyle(SLChartStyle.Style32);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, 10);
            chart.Title.SetTitle("Number Of Vehicle");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        private SLDocument CreateDataExcelSheet2(SLDocument slDocument, List<NoVehicleWtcData> listData)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var total1 = 0;
            var total2 = 0;

            //select distinct data
            var dataList = listData.OrderBy(x => x.Regional).Select(x => x.Regional).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY REGIONAL");
            slDocument.SetCellValue(firstRow, firstColumn + 1, "SALES");
            slDocument.SetCellValue(firstRow, firstColumn + 2, "MARKETING");

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "HO" : item);

                var countData = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == "SALES"
                                                        && x.Regional == item).Sum(x => x.NoOfVehicle);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 1, countData.ToString());

                var countData2 = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == "MARKETING"
                                                        && x.Regional == item).Sum(x => x.NoOfVehicle);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 2, countData2.ToString());

                total1 += countData == null ? 0 : countData.Value;
                total2 += countData2 == null ? 0 : countData2.Value;

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");
            slDocument.SetCellValue(contRow, firstColumn + 1, total1);
            slDocument.SetCellValue(contRow, firstColumn + 2, total2);

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.AutoFitColumn(firstColumn, firstColumn + 2);
            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, 10, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, 10, headerStyleNumbChart);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, firstColumn + 2);
            chart.SetChartStyle(SLChartStyle.Style31);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, 10);
            chart.Title.SetTitle("Vehicle By Regional");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        private SLDocument CreateDataExcelSheet3(SLDocument slDocument, List<NoVehicleMakeData> listData)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var total1 = 0;

            //select distinct data
            var dataList = listData.OrderBy(x => x.Manufacturer).Select(x => x.Manufacturer).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY MANUFACTURER");
            slDocument.SetCellValue(firstRow, firstColumn + 1, "TOTAL");

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "No Manufacturer" : item);

                var countData = listData.Where(x => x.Manufacturer == item).Sum(x => x.NoOfVehicle);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 1, countData.ToString());

                total1 += countData == null ? 0 : countData.Value;

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");
            slDocument.SetCellValue(contRow, firstColumn + 1, total1);

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.AutoFitColumn(firstColumn);
            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, 8, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, 8, headerStyleNumbChart);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, firstColumn + 1);
            chart.SetChartStyle(SLChartStyle.Style30);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, 8);
            chart.Title.SetTitle("Vehicle By Make");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            #endregion

            #region --------- Chart 2--------------

            firstRow = 14;
            contRow = 15;
            firstColumn = 10;
            total1 = 0;

            //select distinct data
            dataList = listData.OrderBy(x => x.BodyType).Select(x => x.BodyType).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY TYPE");
            slDocument.SetCellValue(firstRow, firstColumn + 1, "TOTAL");

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "No Body Type" : item);

                var countData = listData.Where(x => x.BodyType == item).Sum(x => x.NoOfVehicle);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 1, countData.ToString());

                total1 += countData == null ? 0 : countData.Value;

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");
            slDocument.SetCellValue(contRow, firstColumn + 1, total1);

            slDocument.AutoFitColumn(firstColumn);
            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, 16, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, 16, headerStyleNumbChart);

            chart = slDocument.CreateChart(firstRow, firstColumn, contRow, firstColumn + 1);
            chart.SetChartStyle(SLChartStyle.Style32);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 9, firstRow - 2, 16);
            chart.Title.SetTitle("Vehicle By Type");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        private SLDocument CreateDataExcelSheet4(SLDocument slDocument, List<OdometerData> listData)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var total1 = Convert.ToDecimal(0);

            //select distinct data
            var dataList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY FUNCTION");
            slDocument.SetCellValue(firstRow, firstColumn + 1, "TOTAL");

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "No Function" : item);

                var countData = listData.Where(x => x.Function == item).Sum(x => x.TotalKm);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 1, countData.ToString());

                total1 += countData == null ? Convert.ToDecimal(0) : countData.Value;

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");
            slDocument.SetCellValue(contRow, firstColumn + 1, total1);

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle numberStyle = slDocument.CreateStyle();
            numberStyle.FormatCode = "#,##0.00";

            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, 10, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, 10, headerStyleNumbChart);
            slDocument.SetCellStyle(firstRow + 1, firstColumn + 1, contRow, firstColumn + 1, numberStyle);
            slDocument.AutoFitColumn(firstColumn, firstColumn + 1);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, firstColumn + 1);
            chart.SetChartStyle(SLChartStyle.Style31);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, 10);
            chart.Title.SetTitle("Monthly KM's driven By Function");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        private SLDocument CreateDataExcelSheet5(SLDocument slDocument, List<LiterByFunctionData> listData)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var total1 = Convert.ToDecimal(0);

            //select distinct data
            var dataList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY FUNCTION");
            slDocument.SetCellValue(firstRow, firstColumn + 1, "TOTAL");

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "No Function" : item);

                var countData = listData.Where(x => x.Function == item).Sum(x => x.TotalLiter);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 1, countData.ToString());

                total1 += countData == null ? Convert.ToDecimal(0) : countData.Value;

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");
            slDocument.SetCellValue(contRow, firstColumn + 1, total1);

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle numberStyle = slDocument.CreateStyle();
            numberStyle.FormatCode = "#,##0.00";

            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, 10, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, 10, headerStyleNumbChart);
            slDocument.SetCellStyle(firstRow + 1, firstColumn + 1, contRow, firstColumn + 1, numberStyle);
            slDocument.AutoFitColumn(firstColumn, firstColumn + 1);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, firstColumn + 1);
            chart.SetChartStyle(SLChartStyle.Style32);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, 10);
            chart.Title.SetTitle("Monthly Fuel Purchased (in liters) By Function");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }
 
        private SLDocument CreateDataExcelSheet6(SLDocument slDocument, List<FuelCostByFunctionData> listData, List<LeaseCostByFunctionData> listData2)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var total1 = Convert.ToDecimal(0);
            var total2 = Convert.ToDecimal(0);

            //select distinct data
            var dataList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct().ToList();
            var dataList2 = listData2.OrderBy(x => x.Function).Select(x => x.Function).Distinct();

            dataList.AddRange(dataList2);
            dataList = dataList.Distinct().ToList();

            slDocument.SetCellValue(firstRow, firstColumn, "BY FUNCTION");
            slDocument.SetCellValue(firstRow, firstColumn + 1, "Fuel Cost");
            slDocument.SetCellValue(firstRow, firstColumn + 2, "Lease Cost");

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "No Function" : item);

                var countData = listData.Where(x => x.Function == item).Sum(x => x.TotalFuelCost);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 1, countData.ToString());

                var countData2 = listData2.Where(x => x.Function == item).Sum(x => x.TotalLeaseCost);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 2, countData2.ToString());

                total1 += countData == null ? Convert.ToDecimal(0) : countData.Value;
                total2 += countData2 == null ? Convert.ToDecimal(0) : countData2.Value;

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");
            slDocument.SetCellValue(contRow, firstColumn + 1, total1);
            slDocument.SetCellValue(contRow, firstColumn + 2, total2);

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle numberStyle = slDocument.CreateStyle();
            numberStyle.FormatCode = "#,##0.00";

            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, 10, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, 10, headerStyleNumbChart);
            slDocument.SetCellStyle(firstRow + 1, firstColumn + 1, contRow, firstColumn + 2, numberStyle);
            slDocument.AutoFitColumn(firstColumn, firstColumn + 2);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, firstColumn + 2);
            chart.SetChartStyle(SLChartStyle.Style30);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, 10);
            chart.Title.SetTitle("Fuel & Lease Cost");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        private SLDocument CreateDataExcelSheet7(SLDocument slDocument, List<SalesByRegionData> listData)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var total1 = Convert.ToDecimal(0);
            var total2 = Convert.ToDecimal(0);

            //select distinct data
            var dataList = listData.OrderBy(x => x.Region).Select(x => x.Region).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY REGIONAL");
            slDocument.SetCellValue(firstRow, firstColumn + 1, "Operational Cost Per Stick");
            slDocument.SetCellValue(firstRow, firstColumn + 2, "Operational Cost Per KM");

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "HO" : item);

                var countData = listData.Where(x => x.Region == item).Sum(x => x.TotalCost / x.Stick);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 1, countData.ToString());

                var countData2 = listData.Where(x => x.Region == item).Sum(x => x.TotalCost / x.TotalKm);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 2, countData2.ToString());

                total1 += countData == null ? Convert.ToDecimal(0) : countData.Value;
                total2 += countData2 == null ? Convert.ToDecimal(0) : countData2.Value;

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");
            slDocument.SetCellValue(contRow, firstColumn + 1, total1);
            slDocument.SetCellValue(contRow, firstColumn + 2, total2);

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle numberStyle = slDocument.CreateStyle();
            numberStyle.FormatCode = "#,##0.00";

            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, 10, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, 10, headerStyleNumbChart);
            slDocument.SetCellStyle(firstRow + 1, firstColumn + 1, contRow, firstColumn + 2, numberStyle);
            slDocument.AutoFitColumn(firstColumn, firstColumn + 2);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, firstColumn + 2);
            chart.SetChartStyle(SLChartStyle.Style32);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, 10);
            chart.Title.SetTitle("Operational Cost");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        private SLDocument CreateDataExcelSheet8(SLDocument slDocument, List<AcVsObData> listData)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var total1 = Convert.ToDecimal(0);
            var total2 = Convert.ToDecimal(0);
            var total3 = Convert.ToDecimal(0);
            var total4 = Convert.ToDecimal(0);

            //select distinct data
            var dataList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY FUNCTION");
            slDocument.SetCellValue(firstRow, firstColumn + 1, "Actual BENEFIT");
            slDocument.SetCellValue(firstRow, firstColumn + 2, "Actual WTC");
            slDocument.SetCellValue(firstRow, firstColumn + 3, "Budget BENEFIT");
            slDocument.SetCellValue(firstRow, firstColumn + 4, "Budget WTC");

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "No Function" : item);

                var countData = listData.Where(x => x.Function == item && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT").Sum(x => x.ActualCost);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 1, countData.ToString());

                var countData2 = listData.Where(x => x.Function == item && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC").Sum(x => x.ActualCost);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 2, countData2.ToString());

                var countData3 = listData.Where(x => x.Function == item && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT").Sum(x => x.CostOb);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 3, countData3.ToString());

                var countData4 = listData.Where(x => x.Function == item && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC").Sum(x => x.CostOb);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 4, countData4.ToString());

                total1 += countData == null ? Convert.ToDecimal(0) : countData.Value;
                total2 += countData2 == null ? Convert.ToDecimal(0) : countData2.Value;
                total3 += countData3 == null ? Convert.ToDecimal(0) : countData3.Value;
                total4 += countData4 == null ? Convert.ToDecimal(0) : countData4.Value;

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");
            slDocument.SetCellValue(contRow, firstColumn + 1, total1);
            slDocument.SetCellValue(contRow, firstColumn + 2, total2);
            slDocument.SetCellValue(contRow, firstColumn + 3, total3);
            slDocument.SetCellValue(contRow, firstColumn + 4, total4);

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle numberStyle = slDocument.CreateStyle();
            numberStyle.FormatCode = "#,##0.00";

            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, 10, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, 10, headerStyleNumbChart);
            slDocument.SetCellStyle(firstRow + 1, firstColumn + 1, contRow, firstColumn + 4, numberStyle);
            slDocument.AutoFitColumn(firstColumn, firstColumn + 4);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, firstColumn + 4);
            chart.SetChartStyle(SLChartStyle.Style31);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, 10);
            chart.Title.SetTitle("Actual Cost Vs Budget");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        private SLDocument CreateDataExcelSheet9(SLDocument slDocument, List<AccidentData> listData)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var total2 = Convert.ToDecimal(0);
            var total3 = Convert.ToDecimal(0);

            //select distinct data
            var dataList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY FUNCTION");
            slDocument.SetCellValue(firstRow, firstColumn + 1, "BENEFIT");
            slDocument.SetCellValue(firstRow, firstColumn + 2, "WTC");

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "No Function" : item);

                var countData2 = listData.Where(x => x.Function == item && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT").Sum(x => x.AccidentCount);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 1, countData2.ToString());

                var countData3 = listData.Where(x => x.Function == item && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC").Sum(x => x.AccidentCount);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 2, countData3.ToString());

                total2 += countData2 == null ? Convert.ToDecimal(0) : countData2.Value;
                total3 += countData3 == null ? Convert.ToDecimal(0) : countData3.Value;

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");
            slDocument.SetCellValue(contRow, firstColumn + 1, total2);
            slDocument.SetCellValue(contRow, firstColumn + 2, total3);

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.AutoFitColumn(firstColumn, firstColumn + 2);
            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, 10, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, 10, headerStyleNumbChart);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, firstColumn + 2);
            chart.SetChartStyle(SLChartStyle.Style30);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, 10);
            chart.Title.SetTitle("Accident");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        private SLDocument CreateDataExcelSheet10(SLDocument slDocument, List<NoVehicleData> listData,
            List<OdometerData> listData2, List<LiterByFunctionData> listData3, List<FuelCostByFunctionData> listData4,
            List<LeaseCostByFunctionData> listData5, List<AccidentData> listData6, SummarySearchViewExport inputExport)
        {
            #region --------- Chart --------------

            var firstRow = 3;
            var contRow = 5;
            var firstColumn = 2;
            var nextColumn = 3;

            var listFunction = listData.Select(x => x.Function == null ? "" : x.Function.ToUpper()).Distinct().ToList();
            var listFunction2 = listData2.Select(x => x.Function == null ? "" : x.Function.ToUpper()).Distinct().ToList();
            var listFunction3 = listData3.Select(x => x.Function == null ? "" : x.Function.ToUpper()).Distinct().ToList();
            var listFunction4 = listData4.Select(x => x.Function == null ? "" : x.Function.ToUpper()).Distinct().ToList();
            var listFunction5 = listData5.Select(x => x.Function == null ? "" : x.Function.ToUpper()).Distinct().ToList();
            var listFunction6 = listData6.Select(x => x.Function == null ? "" : x.Function.ToUpper()).Distinct().ToList();

            listFunction.AddRange(listFunction2);
            listFunction.AddRange(listFunction3);
            listFunction.AddRange(listFunction4);
            listFunction.AddRange(listFunction5);
            listFunction.AddRange(listFunction6);

            var groupFunction = listFunction.Distinct();

            slDocument.SetCellValue(contRow, firstColumn, "Number of Vehicle");
            slDocument.SetCellValue(contRow + 2, firstColumn, "Vehicle Usage :");
            slDocument.SetCellValue(contRow + 4, firstColumn, "     Fuel Purchased (liters)");
            slDocument.SetCellValue(contRow + 6, firstColumn, "     KM's Driven");
            slDocument.SetCellValue(contRow + 8, firstColumn, "Vehicle Cost :");
            slDocument.SetCellValue(contRow + 10, firstColumn, "     Fuel Cost");
            slDocument.SetCellValue(contRow + 12, firstColumn, "     Leasing Cost");
            slDocument.SetCellValue(contRow + 14, firstColumn, "Accident");
            slDocument.SetCellValue(contRow + 16, firstColumn, "KM's Driven/Vehicle");
            slDocument.SetCellValue(contRow + 18, firstColumn, "Accident Index");
            slDocument.SetCellValue(contRow + 20, firstColumn, "Operational Cost per KM");

            slDocument.SetCellValue(firstRow, firstColumn + 1, "Period : " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + " " + inputExport.YearFrom);

            foreach (var item in groupFunction)
            {
                slDocument.SetCellValue(firstRow + 1, nextColumn, item);

                //set data 1
                var count1 = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == item).Sum(x => x.NoOfVehicle);
                slDocument.SetCellValue(contRow, nextColumn, count1.ToString());

                //set data 2
                var count2 = listData3.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == item).Sum(x => x.TotalLiter);
                slDocument.SetCellValue(contRow + 4, nextColumn, count2.Value.ToString("N2"));

                //set data 3
                var count3 = listData2.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == item).Sum(x => x.TotalKm);
                slDocument.SetCellValue(contRow + 6, nextColumn, count3.Value.ToString("N2"));

                //set data 4
                var count4 = listData4.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == item).Sum(x => x.TotalFuelCost);
                slDocument.SetCellValue(contRow + 10, nextColumn, count4.Value.ToString("N2"));

                //set data 5
                var count5 = listData5.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == item).Sum(x => x.TotalLeaseCost);
                slDocument.SetCellValue(contRow + 12, nextColumn, count5.Value.ToString("N2"));

                //set data 6
                var count6 = listData6.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == item).Sum(x => x.AccidentCount);
                slDocument.SetCellValue(contRow + 14, nextColumn, count6.ToString());

                //set data 7
                var count7 = count1 == 0 ? 0 : (count3 / count1);
                slDocument.SetCellValue(contRow + 16, nextColumn, count7.Value.ToString("N2"));

                //set data 8
                var count8 = count3 == 0 ? 0 : ((count6 / count3) * 1000000);
                slDocument.SetCellValue(contRow + 18, nextColumn, count8.Value.ToString("N2"));

                //set data 9
                var count9 = count3 == 0 ? 0 : (((count4 + count5) * 1000000) / count3);
                slDocument.SetCellValue(contRow + 20, nextColumn, count9.Value.ToString("N2"));

                nextColumn++;
            }

            slDocument.SetCellValue(firstRow + 1, nextColumn, "Total");

            //set data 1
            var count1Total = listData.Sum(x => x.NoOfVehicle);
            slDocument.SetCellValue(contRow, nextColumn, count1Total.ToString());

            //set data 2
            var count2Total = listData3.Sum(x => x.TotalLiter);
            slDocument.SetCellValue(contRow + 4, nextColumn, count2Total.Value.ToString("N2"));

            //set data 3
            var count3Total = listData2.Sum(x => x.TotalKm);
            slDocument.SetCellValue(contRow + 6, nextColumn, count3Total.Value.ToString("N2"));

            //set data 4
            var count4Total = listData4.Sum(x => x.TotalFuelCost);
            slDocument.SetCellValue(contRow + 10, nextColumn, count4Total.Value.ToString("N2"));

            //set data 5
            var count5Total = listData5.Sum(x => x.TotalLeaseCost);
            slDocument.SetCellValue(contRow + 12, nextColumn, count5Total.Value.ToString("N2"));

            //set data 6
            var count6Total = listData6.Sum(x => x.AccidentCount);
            slDocument.SetCellValue(contRow + 14, nextColumn, count6Total.ToString());

            //set data 7
            var count7Total = count1Total == 0 ? 0 : (count3Total / count1Total);
            slDocument.SetCellValue(contRow + 16, nextColumn, count7Total.Value.ToString("N2"));

            //set data 8
            var count8Total = count3Total == 0 ? 0 : ((count6Total / count3Total) * 1000000);
            slDocument.SetCellValue(contRow + 18, nextColumn, count8Total.Value.ToString("N2"));

            //set data 9
            var count9Total = count3Total == 0 ? 0 : (((count4Total + count5Total) * 1000000) / count3Total);
            slDocument.SetCellValue(contRow + 20, nextColumn, count9Total.Value.ToString("N2"));

            slDocument.MergeWorksheetCells(firstRow, firstColumn, firstRow + 1, firstColumn);
            slDocument.MergeWorksheetCells(firstRow, firstColumn + 1, firstRow, nextColumn);

            SLStyle boldStyle = slDocument.CreateStyle();
            boldStyle.Font.Bold = true;

            slDocument.SetCellStyle(contRow, firstColumn, contRow, firstColumn, boldStyle);
            slDocument.SetCellStyle(contRow + 2, firstColumn, contRow + 2, firstColumn, boldStyle);
            slDocument.SetCellStyle(contRow + 8, firstColumn, contRow + 8, firstColumn, boldStyle);
            slDocument.SetCellStyle(contRow + 14, firstColumn, contRow + 14, firstColumn, boldStyle);
            slDocument.SetCellStyle(contRow + 16, firstColumn, contRow + 16, firstColumn, boldStyle);
            slDocument.SetCellStyle(contRow + 18, firstColumn, contRow + 18, firstColumn, boldStyle);
            slDocument.SetCellStyle(contRow + 20, firstColumn, contRow + 20, firstColumn, boldStyle);

            SLStyle borderStyle = slDocument.CreateStyle();
            borderStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            borderStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.SetCellStyle(contRow, firstColumn, contRow + 20, nextColumn, borderStyle);

            SLStyle borderBottomStyle = slDocument.CreateStyle();
            borderBottomStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.SetCellStyle(contRow + 20, firstColumn, contRow + 20, nextColumn, borderBottomStyle);

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            slDocument.AutoFitColumn(firstColumn, nextColumn);
            slDocument.SetCellStyle(firstRow, firstColumn, firstRow + 1, nextColumn, headerStyleChart);

            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 14;
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Aqua, System.Drawing.Color.Aqua);

            slDocument.SetCellValue(1, 2, "Executive Summary " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + "-" + inputExport.YearFrom);
            slDocument.MergeWorksheetCells(1, 2, 1, nextColumn);
            slDocument.SetCellStyle(1, 2, 1, nextColumn, valueStyle);

            #endregion

            return slDocument;
        }

        private SLDocument CreateDataExcelSheet11(SLDocument slDocument, List<NoVehicleData> listData,
            List<OdometerData> listData2, List<LiterByFunctionData> listData3, List<FuelCostByFunctionData> listData4,
            List<LeaseCostByFunctionData> listData5, List<AccidentData> listData6, SummarySearchViewExport inputExport)
        {
            #region --------- Chart --------------

            var firstRow = 3;
            var contRow = 5;
            var firstColumn = 2;
            var nextColumn = 3;

            var listFunction = listData.Select(x => x.Function == null ? "" : x.Function.ToUpper()).Distinct().ToList();
            var listFunction2 = listData2.Select(x => x.Function == null ? "" : x.Function.ToUpper()).Distinct().ToList();
            var listFunction3 = listData3.Select(x => x.Function == null ? "" : x.Function.ToUpper()).Distinct().ToList();
            var listFunction4 = listData4.Select(x => x.Function == null ? "" : x.Function.ToUpper()).Distinct().ToList();
            var listFunction5 = listData5.Select(x => x.Function == null ? "" : x.Function.ToUpper()).Distinct().ToList();
            var listFunction6 = listData6.Select(x => x.Function == null ? "" : x.Function.ToUpper()).Distinct().ToList();

            listFunction.AddRange(listFunction2);
            listFunction.AddRange(listFunction3);
            listFunction.AddRange(listFunction4);
            listFunction.AddRange(listFunction5);
            listFunction.AddRange(listFunction6);

            var groupFunction = listFunction.Distinct();

            slDocument.SetCellValue(contRow, firstColumn, "Number of Vehicle");
            slDocument.SetCellValue(contRow + 2, firstColumn, "Vehicle Usage :");
            slDocument.SetCellValue(contRow + 4, firstColumn, "     Fuel Purchased (liters)");
            slDocument.SetCellValue(contRow + 6, firstColumn, "     KM's Driven");
            slDocument.SetCellValue(contRow + 8, firstColumn, "Vehicle Cost :");
            slDocument.SetCellValue(contRow + 10, firstColumn, "     Fuel Cost");
            slDocument.SetCellValue(contRow + 12, firstColumn, "     Leasing Cost");
            slDocument.SetCellValue(contRow + 14, firstColumn, "Accident");
            slDocument.SetCellValue(contRow + 16, firstColumn, "KM's Driven/Vehicle");
            slDocument.SetCellValue(contRow + 18, firstColumn, "Accident Index");
            slDocument.SetCellValue(contRow + 20, firstColumn, "Operational Cost per KM");

            slDocument.SetCellValue(firstRow, firstColumn + 1, "Period : " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + " " + inputExport.YearFrom);

            foreach (var item in groupFunction)
            {
                slDocument.SetCellValue(firstRow + 1, nextColumn, item);

                //set data 1
                var count1 = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == item
                                                && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT").Sum(x => x.NoOfVehicle);
                slDocument.SetCellValue(contRow, nextColumn, count1.ToString());

                //set data 2
                var count2 = listData3.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == item
                                                && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT").Sum(x => x.TotalLiter);
                slDocument.SetCellValue(contRow + 4, nextColumn, count2.Value.ToString("N2"));

                //set data 3
                var count3 = listData2.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == item
                                                && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT").Sum(x => x.TotalKm);
                slDocument.SetCellValue(contRow + 6, nextColumn, count3.Value.ToString("N2"));

                //set data 4
                var count4 = listData4.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == item
                                                && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT").Sum(x => x.TotalFuelCost);
                slDocument.SetCellValue(contRow + 10, nextColumn, count4.Value.ToString("N2"));

                //set data 5
                var count5 = listData5.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == item).Sum(x => x.TotalLeaseCost);
                slDocument.SetCellValue(contRow + 12, nextColumn, count5.Value.ToString("N2"));

                //set data 6
                var count6 = listData6.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == item
                                                && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT").Sum(x => x.AccidentCount);
                slDocument.SetCellValue(contRow + 14, nextColumn, count6.ToString());

                //set data 7
                var count7 = count1 == 0 ? 0 : (count3 / count1);
                slDocument.SetCellValue(contRow + 16, nextColumn, count7.Value.ToString("N2"));

                //set data 8
                var count8 = count3 == 0 ? 0 : ((count6 / count3) * 1000000);
                slDocument.SetCellValue(contRow + 18, nextColumn, count8.Value.ToString("N2"));

                //set data 9
                var count9 = count3 == 0 ? 0 : (((count4 + count5) * 1000000) / count3);
                slDocument.SetCellValue(contRow + 20, nextColumn, count9.Value.ToString("N2"));

                nextColumn++;
            }

            slDocument.SetCellValue(firstRow + 1, nextColumn, "Total");

            //set data 1
            var count1Total = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT").Sum(x => x.NoOfVehicle);
            slDocument.SetCellValue(contRow, nextColumn, count1Total.ToString());

            //set data 2
            var count2Total = listData3.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT").Sum(x => x.TotalLiter);
            slDocument.SetCellValue(contRow + 4, nextColumn, count2Total.Value.ToString("N2"));

            //set data 3
            var count3Total = listData2.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT").Sum(x => x.TotalKm);
            slDocument.SetCellValue(contRow + 6, nextColumn, count3Total.Value.ToString("N2"));

            //set data 4
            var count4Total = listData4.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT").Sum(x => x.TotalFuelCost);
            slDocument.SetCellValue(contRow + 10, nextColumn, count4Total.Value.ToString("N2"));

            //set data 5
            var count5Total = listData5.Sum(x => x.TotalLeaseCost);
            slDocument.SetCellValue(contRow + 12, nextColumn, count5Total.Value.ToString("N2"));

            //set data 6
            var count6Total = listData6.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT").Sum(x => x.AccidentCount);
            slDocument.SetCellValue(contRow + 14, nextColumn, count6Total.ToString());

            //set data 7
            var count7Total = count1Total == 0 ? 0 : (count3Total / count1Total);
            slDocument.SetCellValue(contRow + 16, nextColumn, count7Total.Value.ToString("N2"));

            //set data 8
            var count8Total = count3Total == 0 ? 0 : ((count6Total / count3Total) * 1000000);
            slDocument.SetCellValue(contRow + 18, nextColumn, count8Total.Value.ToString("N2"));

            //set data 9
            var count9Total = count3Total == 0 ? 0 : (((count4Total + count5Total) * 1000000) / count3Total);
            slDocument.SetCellValue(contRow + 20, nextColumn, count9Total.Value.ToString("N2"));

            slDocument.MergeWorksheetCells(firstRow, firstColumn, firstRow + 1, firstColumn);
            slDocument.MergeWorksheetCells(firstRow, firstColumn + 1, firstRow, nextColumn);

            SLStyle boldStyle = slDocument.CreateStyle();
            boldStyle.Font.Bold = true;

            slDocument.SetCellStyle(contRow, firstColumn, contRow, firstColumn, boldStyle);
            slDocument.SetCellStyle(contRow + 2, firstColumn, contRow + 2, firstColumn, boldStyle);
            slDocument.SetCellStyle(contRow + 8, firstColumn, contRow + 8, firstColumn, boldStyle);
            slDocument.SetCellStyle(contRow + 14, firstColumn, contRow + 14, firstColumn, boldStyle);
            slDocument.SetCellStyle(contRow + 16, firstColumn, contRow + 16, firstColumn, boldStyle);
            slDocument.SetCellStyle(contRow + 18, firstColumn, contRow + 18, firstColumn, boldStyle);
            slDocument.SetCellStyle(contRow + 20, firstColumn, contRow + 20, firstColumn, boldStyle);

            SLStyle borderStyle = slDocument.CreateStyle();
            borderStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            borderStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.SetCellStyle(contRow, firstColumn, contRow + 20, nextColumn, borderStyle);

            SLStyle borderBottomStyle = slDocument.CreateStyle();
            borderBottomStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.SetCellStyle(contRow + 20, firstColumn, contRow + 20, nextColumn, borderBottomStyle);

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            slDocument.AutoFitColumn(firstColumn, nextColumn);
            slDocument.SetCellStyle(firstRow, firstColumn, firstRow + 1, nextColumn, headerStyleChart);

            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 14;
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Aqua, System.Drawing.Color.Aqua);

            slDocument.SetCellValue(1, 2, "Executive Summary " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + "-" + inputExport.YearFrom);
            slDocument.MergeWorksheetCells(1, 2, 1, nextColumn);
            slDocument.SetCellStyle(1, 2, 1, nextColumn, valueStyle);

            #endregion

            return slDocument;
        }

        private SLDocument CreateDataExcelSheet12(SLDocument slDocument, List<NoVehicleData> listData,
            List<OdometerData> listData2, List<LiterByFunctionData> listData3, List<FuelCostByFunctionData> listData4,
            List<LeaseCostByFunctionData> listData5, List<AccidentData> listData6, SummarySearchViewExport inputExport)
        {
            #region --------- Chart --------------

            var firstRow = 3;
            var contRow = 5;
            var firstColumn = 2;
            var nextColumn = 3;

            var listFunction = listData.Select(x => x.Function == null ? "" : x.Function.ToUpper()).Distinct().ToList();
            var listFunction2 = listData2.Select(x => x.Function == null ? "" : x.Function.ToUpper()).Distinct().ToList();
            var listFunction3 = listData3.Select(x => x.Function == null ? "" : x.Function.ToUpper()).Distinct().ToList();
            var listFunction4 = listData4.Select(x => x.Function == null ? "" : x.Function.ToUpper()).Distinct().ToList();
            var listFunction5 = listData5.Select(x => x.Function == null ? "" : x.Function.ToUpper()).Distinct().ToList();
            var listFunction6 = listData6.Select(x => x.Function == null ? "" : x.Function.ToUpper()).Distinct().ToList();

            listFunction.AddRange(listFunction2);
            listFunction.AddRange(listFunction3);
            listFunction.AddRange(listFunction4);
            listFunction.AddRange(listFunction5);
            listFunction.AddRange(listFunction6);

            var groupFunction = listFunction.Distinct();

            slDocument.SetCellValue(contRow, firstColumn, "Number of Vehicle");
            slDocument.SetCellValue(contRow + 2, firstColumn, "Vehicle Usage :");
            slDocument.SetCellValue(contRow + 4, firstColumn, "     Fuel Purchased (liters)");
            slDocument.SetCellValue(contRow + 6, firstColumn, "     KM's Driven");
            slDocument.SetCellValue(contRow + 8, firstColumn, "Vehicle Cost :");
            slDocument.SetCellValue(contRow + 10, firstColumn, "     Fuel Cost");
            slDocument.SetCellValue(contRow + 12, firstColumn, "     Leasing Cost");
            slDocument.SetCellValue(contRow + 14, firstColumn, "Accident");
            slDocument.SetCellValue(contRow + 16, firstColumn, "KM's Driven/Vehicle");
            slDocument.SetCellValue(contRow + 18, firstColumn, "Accident Index");
            slDocument.SetCellValue(contRow + 20, firstColumn, "Operational Cost per KM");

            slDocument.SetCellValue(firstRow, firstColumn + 1, "Period : " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + " " + inputExport.YearFrom);

            foreach (var item in groupFunction)
            {
                slDocument.SetCellValue(firstRow + 1, nextColumn, item);

                //set data 1
                var count1 = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == item
                                                && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC").Sum(x => x.NoOfVehicle);
                slDocument.SetCellValue(contRow, nextColumn, count1.ToString());

                //set data 2
                var count2 = listData3.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == item
                                                && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC").Sum(x => x.TotalLiter);
                slDocument.SetCellValue(contRow + 4, nextColumn, count2.Value.ToString("N2"));

                //set data 3
                var count3 = listData2.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == item
                                                && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC").Sum(x => x.TotalKm);
                slDocument.SetCellValue(contRow + 6, nextColumn, count3.Value.ToString("N2"));

                //set data 4
                var count4 = listData4.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == item
                                                && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC").Sum(x => x.TotalFuelCost);
                slDocument.SetCellValue(contRow + 10, nextColumn, count4.Value.ToString("N2"));

                //set data 5
                var count5 = listData5.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == item).Sum(x => x.TotalLeaseCost);
                slDocument.SetCellValue(contRow + 12, nextColumn, count5.Value.ToString("N2"));

                //set data 6
                var count6 = listData6.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == item
                                                && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC").Sum(x => x.AccidentCount);
                slDocument.SetCellValue(contRow + 14, nextColumn, count6.ToString());

                //set data 7
                var count7 = count1 == 0 ? 0 : (count3 / count1);
                slDocument.SetCellValue(contRow + 16, nextColumn, count7.Value.ToString("N2"));

                //set data 8
                var count8 = count3 == 0 ? 0 : ((count6 / count3) * 1000000);
                slDocument.SetCellValue(contRow + 18, nextColumn, count8.Value.ToString("N2"));

                //set data 9
                var count9 = count3 == 0 ? 0 : (((count4 + count5) * 1000000) / count3);
                slDocument.SetCellValue(contRow + 20, nextColumn, count9.Value.ToString("N2"));

                nextColumn++;
            }

            slDocument.SetCellValue(firstRow + 1, nextColumn, "Total");

            //set data 1
            var count1Total = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC").Sum(x => x.NoOfVehicle);
            slDocument.SetCellValue(contRow, nextColumn, count1Total.ToString());

            //set data 2
            var count2Total = listData3.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC").Sum(x => x.TotalLiter);
            slDocument.SetCellValue(contRow + 4, nextColumn, count2Total.Value.ToString("N2"));

            //set data 3
            var count3Total = listData2.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC").Sum(x => x.TotalKm);
            slDocument.SetCellValue(contRow + 6, nextColumn, count3Total.Value.ToString("N2"));

            //set data 4
            var count4Total = listData4.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC").Sum(x => x.TotalFuelCost);
            slDocument.SetCellValue(contRow + 10, nextColumn, count4Total.Value.ToString("N2"));

            //set data 5
            var count5Total = listData5.Sum(x => x.TotalLeaseCost);
            slDocument.SetCellValue(contRow + 12, nextColumn, count5Total.Value.ToString("N2"));

            //set data 6
            var count6Total = listData6.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC").Sum(x => x.AccidentCount);
            slDocument.SetCellValue(contRow + 14, nextColumn, count6Total.ToString());

            //set data 7
            var count7Total = count1Total == 0 ? 0 : (count3Total / count1Total);
            slDocument.SetCellValue(contRow + 16, nextColumn, count7Total.Value.ToString("N2"));

            //set data 8
            var count8Total = count3Total == 0 ? 0 : ((count6Total / count3Total) * 1000000);
            slDocument.SetCellValue(contRow + 18, nextColumn, count8Total.Value.ToString("N2"));

            //set data 9
            var count9Total = count3Total == 0 ? 0 : (((count4Total + count5Total) * 1000000) / count3Total);
            slDocument.SetCellValue(contRow + 20, nextColumn, count9Total.Value.ToString("N2"));

            slDocument.MergeWorksheetCells(firstRow, firstColumn, firstRow + 1, firstColumn);
            slDocument.MergeWorksheetCells(firstRow, firstColumn + 1, firstRow, nextColumn);

            SLStyle boldStyle = slDocument.CreateStyle();
            boldStyle.Font.Bold = true;

            slDocument.SetCellStyle(contRow, firstColumn, contRow, firstColumn, boldStyle);
            slDocument.SetCellStyle(contRow + 2, firstColumn, contRow + 2, firstColumn, boldStyle);
            slDocument.SetCellStyle(contRow + 8, firstColumn, contRow + 8, firstColumn, boldStyle);
            slDocument.SetCellStyle(contRow + 14, firstColumn, contRow + 14, firstColumn, boldStyle);
            slDocument.SetCellStyle(contRow + 16, firstColumn, contRow + 16, firstColumn, boldStyle);
            slDocument.SetCellStyle(contRow + 18, firstColumn, contRow + 18, firstColumn, boldStyle);
            slDocument.SetCellStyle(contRow + 20, firstColumn, contRow + 20, firstColumn, boldStyle);

            SLStyle borderStyle = slDocument.CreateStyle();
            borderStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            borderStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.SetCellStyle(contRow, firstColumn, contRow + 20, nextColumn, borderStyle);

            SLStyle borderBottomStyle = slDocument.CreateStyle();
            borderBottomStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.SetCellStyle(contRow + 20, firstColumn, contRow + 20, nextColumn, borderBottomStyle);

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            slDocument.AutoFitColumn(firstColumn, nextColumn);
            slDocument.SetCellStyle(firstRow, firstColumn, firstRow + 1, nextColumn, headerStyleChart);

            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 14;
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Aqua, System.Drawing.Color.Aqua);

            slDocument.SetCellValue(1, 2, "Executive Summary " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + "-" + inputExport.YearFrom);
            slDocument.MergeWorksheetCells(1, 2, 1, nextColumn);
            slDocument.SetCellStyle(1, 2, 1, nextColumn, valueStyle);

            #endregion

            return slDocument;
        }

        private SLDocument CreateDataExcelSheet13(SLDocument slDocument, List<AcVsObData> listData)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var total1 = 0;
            var total2 = 0;

            //select distinct data
            var dataList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY FUNCTION");
            slDocument.SetCellValue(firstRow, firstColumn + 1, "Unit Actual");
            slDocument.SetCellValue(firstRow, firstColumn + 2, "Unit Budget");

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "No Function" : item);

                var countData = listData.Where(x => x.Function == item).Sum(x => x.Unit);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 1, countData.ToString());

                var countData2 = listData.Where(x => x.Function == item).Sum(x => x.UnitBudget);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 2, countData2.ToString());

                total1 += countData == null ? 0 : countData.Value;
                total2 += countData2 == null ? 0 : countData2.Value;

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");
            slDocument.SetCellValue(contRow, firstColumn + 1, total1);
            slDocument.SetCellValue(contRow, firstColumn + 2, total2);

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.AutoFitColumn(firstColumn, firstColumn + 2);
            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, 10, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, 10, headerStyleNumbChart);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, firstColumn + 2);
            chart.SetChartStyle(SLChartStyle.Style31);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, 10);
            chart.Title.SetTitle("Actual Unit Vs Budget");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }
        #endregion

        #endregion

        #region --------- Summary By Region --------------

        public ActionResult SummaryRegion()
        {
            var model = new SummaryAllModel();
            model.SearchView.YearFrom = DateTime.Now.Year;

            model.IsByRegion = true;
            model.TitleForm = "Executive Summary Working Tool Car";
            model.TitleExport = "ExportSummaryRegion";
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View("SummaryRegion", model);
        }

        #region --------- Export --------------

        public void ExportSummaryRegion(SummaryAllModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsSummaryRegion(model.SearchViewExport);
            //pathFile = createChart();

            var newFile = new FileInfo(pathFile);

            var fileName = Path.GetFileName(pathFile);

            string attachment = string.Format("attachment; filename={0}", fileName);
            Response.Clear();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.WriteFile(newFile.FullName);
            Response.Flush();
            newFile.Delete();
            Response.End();
        }

        private string CreateXlsSummaryRegion(SummarySearchViewExport inputExport)
        {
            //get data
            var inputWtc = Mapper.Map<VehicleWtcGetByParamInput>(inputExport);
            inputWtc.Function = "Sales,Marketing";
            List<NoVehicleWtcDto> dataWtc = _execSummBLL.GetNoOfVehicleWtcData(inputWtc);
            var listDataWtc = Mapper.Map<List<NoVehicleWtcData>>(dataWtc);

            var inputMake = Mapper.Map<VehicleMakeGetByParamInput>(inputExport);
            List<NoVehicleMakeDto> dataMake = _execSummBLL.GetNoOfVehicleMakeData(inputMake);
            var listDataMake = Mapper.Map<List<NoVehicleMakeData>>(dataMake);

            var inputOdo = Mapper.Map<OdometerGetByParamInput>(inputExport);
            inputOdo.Function = "Sales,Marketing";
            inputOdo.VehicleType = "WTC";
            List<OdometerDto> dataOdo = _execSummBLL.GetOdometerData(inputOdo);
            var listDataOdo = Mapper.Map<List<OdometerData>>(dataOdo);

            var inputLiter = Mapper.Map<LiterFuncGetByParamInput>(inputExport);
            inputLiter.Function = "Sales,Marketing";
            inputLiter.VehicleType = "WTC";
            List<LiterByFunctionDto> dataLiter = _execSummBLL.GetLiterByFunctionData(inputLiter);
            var listDataLiter = Mapper.Map<List<LiterByFunctionData>>(dataLiter);

            var inputFuel = Mapper.Map<FuelCostFuncGetByParamInput>(inputExport);
            inputFuel.Function = "Sales,Marketing";
            inputFuel.VehicleType = "WTC";
            List<FuelCostByFunctionDto> dataFuel = _execSummBLL.GetFuelCostByFunctionData(inputFuel);
            var listDataFuel = Mapper.Map<List<FuelCostByFunctionData>>(dataFuel);

            var inputLease = Mapper.Map<LeaseCostFuncGetByParamInput>(inputExport);
            inputLease.Function = "Sales,Marketing";
            List<LeaseCostByFunctionDto> dataLease = _execSummBLL.GetLeaseCostByFunctionData(inputLease);
            var listDataLease = Mapper.Map<List<LeaseCostByFunctionData>>(dataLease);

            var inputSales = Mapper.Map<SalesRegionGetByParamInput>(inputExport);
            List<SalesByRegionDto> dataSales = _execSummBLL.GetSalesByRegionData(inputSales);
            var listDataSales = Mapper.Map<List<SalesByRegionData>>(dataSales);

            var inputAccident = Mapper.Map<AccidentGetByParamInput>(inputExport);
            inputAccident.Function = "Sales,Marketing";
            inputAccident.VehicleType = "WTC";
            List<AccidentDto> dataAccident = _execSummBLL.GetAccidentData(inputAccident);
            var listDataAccident = Mapper.Map<List<AccidentData>>(dataAccident);

            var inputAcOb = Mapper.Map<AcVsObGetByParamInput>(inputExport);
            inputAcOb.Function = "Sales,Marketing";
            inputAcOb.VehicleType = "WTC";
            List<AcVsObDto> dataAcOb = _execSummBLL.GetAcVsObData(inputAcOb);
            var listDataAcOb = Mapper.Map<List<AcVsObData>>(dataAcOb);

            List<AcVsObDto> dataAcObUnit = _execSummBLL.GetAcVsObUnitData(inputAcOb);
            var listDataAcObUnit = Mapper.Map<List<AcVsObData>>(dataAcObUnit);

            var slDocument = new SLDocument();

            //title no of vehicle
            slDocument.SetCellValue(1, 2, "Executive Summary Working Tool Car " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + "-" + inputExport.YearFrom);
            slDocument.MergeWorksheetCells(1, 2, 1, 10);
            slDocument.RenameWorksheet("Sheet1", "Vehicle By Regional");
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 14;
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Aqua, System.Drawing.Color.Aqua);
            slDocument.SetCellStyle(1, 2, 1, 10, valueStyle);

            slDocument = CreateDataExcelSheet1Region(slDocument, listDataWtc);


            //title no of vehicle Make
            slDocument.AddWorksheet("Make Type");
            slDocument.SetCellValue(1, 2, "Executive Summary Working Tool Car " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + "-" + inputExport.YearFrom);
            slDocument.MergeWorksheetCells(1, 2, 1, 16);
            slDocument.SetCellStyle(1, 2, 1, 16, valueStyle);

            slDocument = CreateDataExcelSheet3(slDocument, listDataMake);


            //title Odometer
            slDocument.AddWorksheet("Odometer");
            slDocument.SetCellValue(1, 2, "Executive Summary Working Tool Car " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + "-" + inputExport.YearFrom);
            slDocument.MergeWorksheetCells(1, 2, 1, 10);
            //create style
            slDocument.SetCellStyle(1, 2, 1, 10, valueStyle);

            slDocument = CreateDataExcelSheet2Region(slDocument, listDataOdo);


            //title Liter
            slDocument.AddWorksheet("Liters");
            slDocument.SetCellValue(1, 2, "Executive Summary Working Tool Car " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + "-" + inputExport.YearFrom);
            slDocument.MergeWorksheetCells(1, 2, 1, 10);
            slDocument.SetCellStyle(1, 2, 1, 10, valueStyle);

            slDocument = CreateDataExcelSheet3Region(slDocument, listDataLiter);


            //title Fuel Cost
            slDocument.AddWorksheet("Fuel Cost");
            slDocument.SetCellValue(1, 2, "Executive Summary Working Tool Car " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + "-" + inputExport.YearFrom);
            slDocument.MergeWorksheetCells(1, 2, 1, 10);
            slDocument.SetCellStyle(1, 2, 1, 10, valueStyle);

            slDocument = CreateDataExcelSheet4Region(slDocument, listDataFuel);


            //title Lease Cost
            slDocument.AddWorksheet("Lease Cost");
            slDocument.SetCellValue(1, 2, "Executive Summary Working Tool Car " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + "-" + inputExport.YearFrom);
            slDocument.MergeWorksheetCells(1, 2, 1, 10);
            slDocument.SetCellStyle(1, 2, 1, 10, valueStyle);

            slDocument = CreateDataExcelSheet5Region(slDocument, listDataLease);


            //title Sales By Region
            slDocument.AddWorksheet("Operational Cost");
            slDocument.SetCellValue(1, 2, "Executive Summary Working Tool Car " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + "-" + inputExport.YearFrom);
            slDocument.MergeWorksheetCells(1, 2, 1, 10);
            slDocument.SetCellStyle(1, 2, 1, 10, valueStyle);

            slDocument = CreateDataExcelSheet7(slDocument, listDataSales);


            //title Accident
            slDocument.AddWorksheet("Accident");
            slDocument.SetCellValue(1, 2, "Executive Summary Working Tool Car " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + "-" + inputExport.YearFrom);
            slDocument.MergeWorksheetCells(1, 2, 1, 10);
            slDocument.SetCellStyle(1, 2, 1, 10, valueStyle);

            slDocument = CreateDataExcelSheet6Region(slDocument, listDataAccident);

            //title AC Vs Ob
            slDocument.AddWorksheet("Actual Cost Vs Budget");
            slDocument.SetCellValue(1, 2, "Executive Summary Working Tool Car " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + "-" + inputExport.YearFrom);
            slDocument.MergeWorksheetCells(1, 2, 1, 10);
            slDocument.SetCellStyle(1, 2, 1, 10, valueStyle);

            slDocument = CreateDataExcelSheet7Region(slDocument, listDataAcOb);

            //title AC Vs OB Unit
            slDocument.AddWorksheet("Actual Unit Vs Budget");
            slDocument.SetCellValue(1, 2, "Executive Summary Working Tool Car " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(inputExport.MonthFrom) + "-" + inputExport.YearFrom);
            slDocument.MergeWorksheetCells(1, 2, 1, 10);
            slDocument.SetCellStyle(1, 2, 1, 10, valueStyle);

            slDocument = CreateDataExcelSheet13(slDocument, listDataAcObUnit);


            var fileName = "ExecSum_SummaryWorkingToolCar" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        #endregion

        #region ------------ New Export Summary Region ----------

        private SLDocument CreateDataExcelSheet1Region(SLDocument slDocument, List<NoVehicleWtcData> listData)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var total1 = 0;
            var total2 = 0;

            //select distinct data
            var dataList = listData.OrderBy(x => x.Regional).Select(x => x.Regional).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY REGIONAL");
            slDocument.SetCellValue(firstRow, firstColumn + 1, "SALES");
            slDocument.SetCellValue(firstRow, firstColumn + 2, "MARKETING");

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "HO" : item);

                var countData = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == "SALES"
                                                        && x.Regional == item).Sum(x => x.NoOfVehicle);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 1, countData.ToString());

                var countData2 = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == "MARKETING"
                                                        && x.Regional == item).Sum(x => x.NoOfVehicle);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 2, countData2.ToString());

                total1 += countData == null ? 0 : countData.Value;
                total2 += countData2 == null ? 0 : countData2.Value;

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");
            slDocument.SetCellValue(contRow, firstColumn + 1, total1);
            slDocument.SetCellValue(contRow, firstColumn + 2, total2);

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.AutoFitColumn(firstColumn, firstColumn + 2);
            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, 10, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, 10, headerStyleNumbChart);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, firstColumn + 2);
            chart.SetChartStyle(SLChartStyle.Style32);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, 10);
            chart.Title.SetTitle("Vehicle By Regional");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        private SLDocument CreateDataExcelSheet2Region(SLDocument slDocument, List<OdometerData> listData)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var total1 = Convert.ToDecimal(0);
            var total2 = Convert.ToDecimal(0);

            //select distinct data
            var dataList = listData.OrderBy(x => x.Region).Select(x => x.Region).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY REGIONAL");
            slDocument.SetCellValue(firstRow, firstColumn + 1, "SALES");
            slDocument.SetCellValue(firstRow, firstColumn + 2, "MARKETING");

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "HO" : item);

                var countData = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == "SALES"
                                                        && x.Region == item).Sum(x => x.TotalKm);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 1, countData.ToString());

                var countData2 = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == "MARKETING"
                                                        && x.Region == item).Sum(x => x.TotalKm);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 2, countData2.ToString());

                total1 += countData == null ? 0 : countData.Value;
                total2 += countData2 == null ? 0 : countData2.Value;

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");
            slDocument.SetCellValue(contRow, firstColumn + 1, total1);
            slDocument.SetCellValue(contRow, firstColumn + 2, total2);

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle numberStyle = slDocument.CreateStyle();
            numberStyle.FormatCode = "#,##0.00";

            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, 10, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, 10, headerStyleNumbChart);
            slDocument.SetCellStyle(firstRow + 1, firstColumn + 1, contRow, firstColumn + 2, numberStyle);
            slDocument.AutoFitColumn(firstColumn, firstColumn + 2);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, firstColumn + 2);
            chart.SetChartStyle(SLChartStyle.Style31);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, 10);
            chart.Title.SetTitle("Monthly KM's driven By Regional");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        private SLDocument CreateDataExcelSheet3Region(SLDocument slDocument, List<LiterByFunctionData> listData)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var total1 = Convert.ToDecimal(0);
            var total2 = Convert.ToDecimal(0);

            //select distinct data
            var dataList = listData.OrderBy(x => x.Region).Select(x => x.Region).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY REGIONAL");
            slDocument.SetCellValue(firstRow, firstColumn + 1, "SALES");
            slDocument.SetCellValue(firstRow, firstColumn + 2, "MARKETING");

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "HO" : item);

                var countData = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == "SALES"
                                                        && x.Region == item).Sum(x => x.TotalLiter);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 1, countData.ToString());

                var countData2 = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == "MARKETING"
                                                        && x.Region == item).Sum(x => x.TotalLiter);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 2, countData2.ToString());

                total1 += countData == null ? 0 : countData.Value;
                total2 += countData2 == null ? 0 : countData2.Value;

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");
            slDocument.SetCellValue(contRow, firstColumn + 1, total1);
            slDocument.SetCellValue(contRow, firstColumn + 2, total2);

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle numberStyle = slDocument.CreateStyle();
            numberStyle.FormatCode = "#,##0.00";

            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, 10, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, 10, headerStyleNumbChart);
            slDocument.SetCellStyle(firstRow + 1, firstColumn + 1, contRow, firstColumn + 2, numberStyle);
            slDocument.AutoFitColumn(firstColumn, firstColumn + 2);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, firstColumn + 2);
            chart.SetChartStyle(SLChartStyle.Style30);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, 10);
            chart.Title.SetTitle("Monthly Fuel Purchased (in liters) By Regional");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        private SLDocument CreateDataExcelSheet4Region(SLDocument slDocument, List<FuelCostByFunctionData> listData)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var total1 = Convert.ToDecimal(0);
            var total2 = Convert.ToDecimal(0);

            //select distinct data
            var dataList = listData.OrderBy(x => x.Region).Select(x => x.Region).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY REGIONAL");
            slDocument.SetCellValue(firstRow, firstColumn + 1, "SALES");
            slDocument.SetCellValue(firstRow, firstColumn + 2, "MARKETING");

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "HO" : item);

                var countData = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == "SALES"
                                                        && x.Region == item).Sum(x => x.TotalFuelCost);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 1, countData.ToString());

                var countData2 = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == "MARKETING"
                                                        && x.Region == item).Sum(x => x.TotalFuelCost);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 2, countData2.ToString());

                total1 += countData == null ? 0 : countData.Value;
                total2 += countData2 == null ? 0 : countData2.Value;

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");
            slDocument.SetCellValue(contRow, firstColumn + 1, total1);
            slDocument.SetCellValue(contRow, firstColumn + 2, total2);

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle numberStyle = slDocument.CreateStyle();
            numberStyle.FormatCode = "#,##0.00";

            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, 10, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, 10, headerStyleNumbChart);
            slDocument.SetCellStyle(firstRow + 1, firstColumn + 1, contRow, firstColumn + 2, numberStyle);
            slDocument.AutoFitColumn(firstColumn, firstColumn + 2);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, firstColumn + 2);
            chart.SetChartStyle(SLChartStyle.Style32);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, 10);
            chart.Title.SetTitle("Fuel Cost");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        private SLDocument CreateDataExcelSheet5Region(SLDocument slDocument, List<LeaseCostByFunctionData> listData)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var total1 = Convert.ToDecimal(0);
            var total2 = Convert.ToDecimal(0);

            //select distinct data
            var dataList = listData.OrderBy(x => x.Region).Select(x => x.Region).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY REGIONAL");
            slDocument.SetCellValue(firstRow, firstColumn + 1, "SALES");
            slDocument.SetCellValue(firstRow, firstColumn + 2, "MARKETING");

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "HO" : item);

                var countData = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == "SALES"
                                                        && x.Region == item).Sum(x => x.TotalLeaseCost);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 1, countData.ToString());

                var countData2 = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == "MARKETING"
                                                        && x.Region == item).Sum(x => x.TotalLeaseCost);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 2, countData2.ToString());

                total1 += countData == null ? 0 : countData.Value;
                total2 += countData2 == null ? 0 : countData2.Value;

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");
            slDocument.SetCellValue(contRow, firstColumn + 1, total1);
            slDocument.SetCellValue(contRow, firstColumn + 2, total2);

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle numberStyle = slDocument.CreateStyle();
            numberStyle.FormatCode = "#,##0.00";

            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, 10, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, 10, headerStyleNumbChart);
            slDocument.SetCellStyle(firstRow + 1, firstColumn + 1, contRow, firstColumn + 2, numberStyle);
            slDocument.AutoFitColumn(firstColumn, firstColumn + 2);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, firstColumn + 2);
            chart.SetChartStyle(SLChartStyle.Style31);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, 10);
            chart.Title.SetTitle("Lease Cost");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        private SLDocument CreateDataExcelSheet6Region(SLDocument slDocument, List<AccidentData> listData)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var total1 = 0;
            var total2 = 0;

            //select distinct data
            var dataList = listData.OrderBy(x => x.Region).Select(x => x.Region).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY REGIONAL");
            slDocument.SetCellValue(firstRow, firstColumn + 1, "SALES");
            slDocument.SetCellValue(firstRow, firstColumn + 2, "MARKETING");

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "HO" : item);

                var countData = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == "SALES"
                                                        && x.Region == item).Sum(x => x.AccidentCount);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 1, countData.ToString());

                var countData2 = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == "MARKETING"
                                                        && x.Region == item).Sum(x => x.AccidentCount);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 2, countData2.ToString());

                total1 += countData == null ? 0 : countData.Value;
                total2 += countData2 == null ? 0 : countData2.Value;

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");
            slDocument.SetCellValue(contRow, firstColumn + 1, total1);
            slDocument.SetCellValue(contRow, firstColumn + 2, total2);

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.AutoFitColumn(firstColumn, firstColumn + 2);
            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, 10, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, 10, headerStyleNumbChart);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, firstColumn + 2);
            chart.SetChartStyle(SLChartStyle.Style30);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, 10);
            chart.Title.SetTitle("Accident");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        private SLDocument CreateDataExcelSheet7Region(SLDocument slDocument, List<AcVsObData> listData)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var total1 = Convert.ToDecimal(0);
            var total2 = Convert.ToDecimal(0);

            //select distinct data
            var dataList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY FUNCTION");
            slDocument.SetCellValue(firstRow, firstColumn + 1, "Actual Cost");
            slDocument.SetCellValue(firstRow, firstColumn + 2, "Operational Budget");

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "No Function" : item);

                var countData = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == item).Sum(x => x.ActualCost);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 1, countData.ToString());

                var countData2 = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == item).Sum(x => x.CostOb);
                slDocument.SetCellValueNumeric(contRow, firstColumn + 2, countData2.ToString());

                total1 += countData == null ? 0 : countData.Value;
                total2 += countData2 == null ? 0 : countData2.Value;

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");
            slDocument.SetCellValue(contRow, firstColumn + 1, total1);
            slDocument.SetCellValue(contRow, firstColumn + 2, total2);

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle numberStyle = slDocument.CreateStyle();
            numberStyle.FormatCode = "#,##0.00";

            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, 10, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, 10, headerStyleNumbChart);
            slDocument.SetCellStyle(firstRow + 1, firstColumn + 1, contRow, firstColumn + 2, numberStyle);
            slDocument.AutoFitColumn(firstColumn, firstColumn + 2);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, firstColumn + 2);
            chart.SetChartStyle(SLChartStyle.Style30);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, 10);
            chart.Title.SetTitle("Actual Cost Vs Budget");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        #endregion

        #endregion

        #region --------- Number Of Vehicle --------------

        public ActionResult Index(int? month, int? year)
        {
            var model = new ExecutiveSummaryModel();
            model.SearchView.YearFrom = DateTime.Now.Year;
            model.SearchView.YearTo = DateTime.Now.Year;
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
            var LocationMappingList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).ToList();
            if (month.HasValue)
            {
                model.SearchView.MonthFrom = month.Value;
                model.SearchView.MonthTo = month.Value;
            }
            if (year.HasValue)
            {
                model.SearchView.YearFrom = year.Value;
                model.SearchView.YearTo = year.Value;
            }
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.Logistic)
            {
                var GetCity = EmployeeList.Where(x => x.EMPLOYEE_ID == CurrentUser.EMPLOYEE_ID && x.IS_ACTIVE).FirstOrDefault();
                if (GetCity != null)
                {
                    var GetRegion = LocationMappingList.Where(x => (x.Location == null ? "" : x.Location.ToUpper()) == (GetCity.CITY == null ? "" : GetCity.CITY.ToUpper())).FirstOrDefault();
                    if (GetRegion != null)
                    {
                        model.SearchView.Zone = GetRegion.ZoneSales;
                        model.SearchView.ZoneId = GetRegion.ZoneSales;
                    }
                }
            }

            var input = Mapper.Map<VehicleGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetNoOfVehicleData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            var settingData = _settingBLL.GetSetting();
            var listVehType = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.IsActive).Select(x => new { x.SettingValue }).ToList();
            var listSupMethod = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.SupplyMethod) && x.IsActive).Select(x => new { x.SettingValue }).ToList();
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();
            var listZone = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.ZoneSales }).Distinct().ToList();

            model.TitleForm = "Number Of Vehicle - All";
            model.TitleExport = "ExportNoVehicleNew";
            model.NoVehicleList = Mapper.Map<List<NoVehicleData>>(data);
            model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
            model.SearchView.SupplyMethodList = new SelectList(listSupMethod, "SettingValue", "SettingValue");
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
            model.SearchView.Functions = GetFunctionsMultiSelectList();
            model.SearchView.ZoneList = new SelectList(listZone, "ZoneSales", "ZoneSales");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View(model);
        }

        [HttpPost]
        public PartialViewResult FilterNoVehicle(ExecutiveSummaryModel model)
        {
            model.NoVehicleList = GetVehicleData(model.SearchView);
            return PartialView("_ListVehicle", model);
        }

        private List<NoVehicleData> GetVehicleData(VehicleSearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _execSummBLL.GetNoOfVehicleData(new VehicleGetByParamInput()).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
                return Mapper.Map<List<NoVehicleData>>(data);
            }

            //getbyparams
            filter.Function = filter.FunctionId;
            filter.ZoneId = filter.Zone;
            var input = Mapper.Map<VehicleGetByParamInput>(filter);

            var dbData = _execSummBLL.GetNoOfVehicleData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            return Mapper.Map<List<NoVehicleData>>(dbData);
        }

        #region --------- Export --------------
        public string ExportNoVehicle(ExecutiveSummaryModel model = null)
        {
            string pathFile = "";
            var input = Mapper.Map<VehicleGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsNoVehicle(input);
            //pathFile = createChart();
            return pathFile;

        }

        private string CreateXlsNoVehicle(VehicleGetByParamInput input)
        {
            //get data
            List<NoVehicleDto> data = _execSummBLL.GetNoOfVehicleData(input);
            var listData = Mapper.Map<List<NoVehicleData>>(data);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Number Of Vehicle");
            slDocument.MergeWorksheetCells(1, 1, 1, 7);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelDashboard(slDocument);

            //create data
            slDocument = CreateDataExcelDashboard(slDocument, listData);

            var fileName = "ExecSum_NumbVehicle" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelDashboard(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Vehicle Type");
            slDocument.SetCellValue(iRow, 2, "Regional");
            slDocument.SetCellValue(iRow, 3, "Supply Method");
            slDocument.SetCellValue(iRow, 4, "Function");
            slDocument.SetCellValue(iRow, 5, "Month");
            slDocument.SetCellValue(iRow, 6, "Year");
            slDocument.SetCellValue(iRow, 7, "No Of Vehicle");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 7, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelDashboard(SLDocument slDocument, List<NoVehicleData> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData.OrderBy(x => x.ReportMonth).OrderBy(x => x.ReportYear))
            {
                slDocument.SetCellValue(iRow, 1, data.VehicleType);
                slDocument.SetCellValue(iRow, 2, data.Regional);
                slDocument.SetCellValue(iRow, 3, data.SupplyMethod);
                slDocument.SetCellValue(iRow, 4, data.Function);
                slDocument.SetCellValue(iRow, 5, data.Month);
                slDocument.SetCellValue(iRow, 6, data.ReportYear.ToString());
                slDocument.SetCellValueNumeric(iRow, 7, data.NoOfVehicle.ToString());

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 7);
            slDocument.SetCellStyle(3, 1, iRow - 1, 7, valueStyle);

            //add row for total
            slDocument.SetCellValue(iRow, 1, "Total");
            slDocument.MergeWorksheetCells(iRow, 1, iRow, 6);
            slDocument.SetCellValueNumeric(iRow, 7, listData.Sum(x => x.NoOfVehicle.Value).ToString());

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 7, headerStyle);

            #region --------- Chart --------------

            //select distinct year
            var yearList = listData.OrderBy(x => x.ReportYear).Select(x => x.ReportYear).Distinct();
            var startColumYear = 9;
            var startRowYear = 3;
            var endRowYear = 3;

            foreach (var year in yearList)
            {
                slDocument.SetCellValue(endRowYear, startColumYear, year.ToString());
                endRowYear++;
            }

            slDocument.SetCellValue(endRowYear, startColumYear, "Total");

            //select distinct function
            var functionList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct();
            var startColum = 10;
            var endColum = 10;
            var startRow = 2;
            var startRowCount = 3;

            foreach (var item in functionList)
            {
                slDocument.SetCellValue(startRow, endColum, string.IsNullOrEmpty(item) ? "No Function" : item);
                startRowCount = 3;

                foreach (var year in yearList)
                {
                    var vehicleCountYear = listData.Where(x => x.Function == item && x.ReportYear == year).Sum(x => x.NoOfVehicle);

                    slDocument.SetCellValueNumeric(startRowCount, endColum, vehicleCountYear.ToString());
                    startRowCount++;
                }

                var vehicleCount = listData.Where(x => x.Function == item).Sum(x => x.NoOfVehicle);
                slDocument.SetCellValueNumeric(startRowCount, endColum, vehicleCount.ToString());

                endColum++;
            }

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(startRowYear, startColumYear, endRowYear, startColumYear, headerStyleChart);
            slDocument.SetCellStyle(startRow, startColum, startRow, endColum - 1, headerStyleChart);
            slDocument.SetCellStyle(startRowCount, startColum, startRowCount, endColum - 1, headerStyleNumbChart);

            SLChart chart = slDocument.CreateChart(startRowYear - 1, startColumYear, endRowYear, endColum - 1);
            chart.SetChartStyle(SLChartStyle.Style46);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(endRowYear + 2, 9, endRowYear + 24, 18);
            chart.PlotDataSeriesAsPrimaryLineChart(3, SLChartDataDisplayType.Normal, true);
            chart.PlotDataSeriesAsSecondaryLineChart(4, SLChartDataDisplayType.Normal, false);
            chart.PlotDataSeriesAsSecondaryLineChart(2, SLChartDataDisplayType.Normal, true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        #endregion

        #region --------- Export New --------------
        public string ExportNoVehicleNew(ExecutiveSummaryModel model = null)
        {
            string pathFile = "";
            var input = Mapper.Map<VehicleGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsNoVehicleNew(input);
            //pathFile = createChart();
            return pathFile;

        }

        private string CreateXlsNoVehicleNew(VehicleGetByParamInput input)
        {
            //get data
            List<NoVehicleDto> data = _execSummBLL.GetNoOfVehicleData(input);
            var listData = Mapper.Map<List<NoVehicleData>>(data);

            var slDocument = new SLDocument();

            //create data
            slDocument = CreateDataExcelDashboardNew(slDocument, listData, input);

            var fileName = "ExecSum_NumbVehicle" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateDataExcelDashboardNew(SLDocument slDocument, List<NoVehicleData> listData, VehicleGetByParamInput input)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var nextColumn = 3;
            var endColumn = 3;
            var minColumn = 10;

            //select distinct data
            var dataList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY FUNCTION");

            nextColumn = firstColumn + 1;
            endColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "BENEFIT " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "WTC " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "No Function" : item);

                nextColumn = firstColumn + 1;
                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT"
                                                        && x.Function == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.NoOfVehicle);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC"
                                                        && x.Function == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.NoOfVehicle);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");

            nextColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT"
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.NoOfVehicle);

                slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC"
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.NoOfVehicle);

                slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle rightStyleChart = slDocument.CreateStyle();
            rightStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Right;

            slDocument.AutoFitColumn(firstColumn, endColumn - 1);

            if (minColumn < endColumn)
            {
                minColumn = endColumn;
            }
            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, minColumn, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, minColumn, headerStyleNumbChart);
            slDocument.SetCellStyle(contRow, firstColumn + 1, contRow, minColumn, rightStyleChart);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, endColumn - 1);
            chart.SetChartStyle(SLChartStyle.Style32);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, minColumn);
            chart.Title.SetTitle("Number Of Vehicle");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            //title
            slDocument.SetCellValue(1, 2, "Number Of Vehicle");
            slDocument.MergeWorksheetCells(1, 2, 1, minColumn);

            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 14;
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Aqua, System.Drawing.Color.Aqua);
            slDocument.SetCellStyle(1, 2, 1, minColumn, valueStyle);

            #endregion

            return slDocument;
        }

        #endregion

        #endregion

        #region --------- Number Of Vehicle WTC --------------
        public ActionResult VehicleWtc(int? month, int? year)
        {
            var model = new NumberVehicleWtcModel();
            model.SearchView.YearFrom = DateTime.Now.Year;
            model.SearchView.YearTo = DateTime.Now.Year;
            if (month.HasValue)
            {
                model.SearchView.MonthFrom = month.Value;
                model.SearchView.MonthTo = month.Value;
            }
            if (year.HasValue)
            {
                model.SearchView.YearFrom = year.Value;
                model.SearchView.YearTo = year.Value;
            }
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
            var LocationMappingList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).ToList();
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.Logistic)
            {
                var GetCity = EmployeeList.Where(x => x.EMPLOYEE_ID == CurrentUser.EMPLOYEE_ID && x.IS_ACTIVE).FirstOrDefault();
                if (GetCity != null)
                {
                    var GetRegion = LocationMappingList.Where(x => (x.Location == null ? "" : x.Location.ToUpper()) == (GetCity.CITY == null ? "" : GetCity.CITY.ToUpper())).FirstOrDefault();
                    if (GetRegion != null)
                    {
                        model.SearchView.Zone = GetRegion.ZoneSales;
                        model.SearchView.ZoneId = GetRegion.ZoneSales;
                    }
                }
            }
            var input = Mapper.Map<VehicleWtcGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetNoOfVehicleWtcData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();
            var listZone = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.ZoneSales }).Distinct().ToList();

            model.TitleForm = "Number Of Vehicle - Working Tool Car";
            model.TitleExport = "ExportNoVehicleWtcNew";
            model.NoVehicleWtcList = Mapper.Map<List<NoVehicleWtcData>>(data);
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
            model.SearchView.Functions = GetFunctionsMultiSelectList();
            model.SearchView.ZoneList = new SelectList(listZone, "ZoneSales", "ZoneSales");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View(model);
        }

        [HttpPost]
        public PartialViewResult FilterNoVehicleWtc(NumberVehicleWtcModel model)
        {
            model.NoVehicleWtcList = GetVehicleWtcData(model.SearchView);
            return PartialView("_ListVehicleWtc", model);
        }

        private List<NoVehicleWtcData> GetVehicleWtcData(VehicleSearchViewWtc filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _execSummBLL.GetNoOfVehicleWtcData(new VehicleWtcGetByParamInput()).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
                return Mapper.Map<List<NoVehicleWtcData>>(data);
            }

            //getbyparams
            filter.Function = filter.FunctionId;
            filter.ZoneId = filter.Zone;
            var input = Mapper.Map<VehicleWtcGetByParamInput>(filter);

            var dbData = _execSummBLL.GetNoOfVehicleWtcData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            return Mapper.Map<List<NoVehicleWtcData>>(dbData);
        }

        #region --------- Export --------------
        public string ExportNoVehicleWtc(NumberVehicleWtcModel model = null)
        {
            string pathFile = "";
            var input = Mapper.Map<VehicleWtcGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsNoVehicleWtc(input);
            //pathFile = createChart();
            return pathFile;

        }
        private string CreateXlsNoVehicleWtc(VehicleWtcGetByParamInput input)
        {
            //get data
            List<NoVehicleWtcDto> data = _execSummBLL.GetNoOfVehicleWtcData(input);
            var listData = Mapper.Map<List<NoVehicleWtcData>>(data);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Number Of Vehicle WTC");
            slDocument.MergeWorksheetCells(1, 1, 1, 5);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelDashboardWtc(slDocument);

            //create data
            slDocument = CreateDataExcelDashboardWtc(slDocument, listData);

            var fileName = "ExecSum_NumbVehicleWtc" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelDashboardWtc(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Regional");
            slDocument.SetCellValue(iRow, 2, "Function");
            slDocument.SetCellValue(iRow, 3, "Month");
            slDocument.SetCellValue(iRow, 4, "Year");
            slDocument.SetCellValue(iRow, 5, "No Of Vehicle");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 5, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelDashboardWtc(SLDocument slDocument, List<NoVehicleWtcData> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData.OrderBy(x => x.ReportMonth).OrderBy(x => x.ReportYear))
            {
                slDocument.SetCellValue(iRow, 1, data.Regional);
                slDocument.SetCellValue(iRow, 2, data.Function);
                slDocument.SetCellValue(iRow, 3, data.Month);
                slDocument.SetCellValue(iRow, 4, data.ReportYear.ToString());
                slDocument.SetCellValueNumeric(iRow, 5, data.NoOfVehicle.ToString());

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 5);
            slDocument.SetCellStyle(3, 1, iRow - 1, 5, valueStyle);

            //add row for total
            slDocument.SetCellValue(iRow, 1, "Total");
            slDocument.MergeWorksheetCells(iRow, 1, iRow, 4);
            slDocument.SetCellValueNumeric(iRow, 5, listData.Sum(x => x.NoOfVehicle.Value).ToString());

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 5, headerStyle);

            #region --------- Chart --------------

            //select distinct year
            var yearList = listData.OrderBy(x => x.ReportYear).Select(x => x.ReportYear).Distinct();
            var startColumYear = 7;
            var startRowYear = 3;
            var endRowYear = 3;

            foreach (var year in yearList)
            {
                slDocument.SetCellValue(endRowYear, startColumYear, year.ToString());
                endRowYear++;
            }

            slDocument.SetCellValue(endRowYear, startColumYear, "Total");

            //select distinct function
            var functionList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct();
            var startColum = 8;
            var endColum = 8;
            var startRow = 2;
            var startRowCount = 3;

            foreach (var item in functionList)
            {
                slDocument.SetCellValue(startRow, endColum, string.IsNullOrEmpty(item) ? "No Function" : item);
                startRowCount = 3;

                foreach (var year in yearList)
                {
                    var vehicleCountYear = listData.Where(x => x.Function == item && x.ReportYear == year).Sum(x => x.NoOfVehicle);

                    slDocument.SetCellValueNumeric(startRowCount, endColum, vehicleCountYear.ToString());
                    startRowCount++;
                }

                var vehicleCount = listData.Where(x => x.Function == item).Sum(x => x.NoOfVehicle);
                slDocument.SetCellValueNumeric(startRowCount, endColum, vehicleCount.ToString());

                endColum++;
            }

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(startRowYear, startColumYear, endRowYear, startColumYear, headerStyleChart);
            slDocument.SetCellStyle(startRow, startColum, startRow, endColum - 1, headerStyleChart);
            slDocument.SetCellStyle(startRowCount, startColum, startRowCount, endColum - 1, headerStyleNumbChart);

            SLChart chart = slDocument.CreateChart(startRowYear - 1, startColumYear, endRowYear, endColum - 1);
            chart.SetChartStyle(SLChartStyle.Style46);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(endRowYear + 2, 7, endRowYear + 24, 16);
            chart.PlotDataSeriesAsPrimaryLineChart(3, SLChartDataDisplayType.Normal, true);
            chart.PlotDataSeriesAsSecondaryLineChart(4, SLChartDataDisplayType.Normal, false);
            chart.PlotDataSeriesAsSecondaryLineChart(2, SLChartDataDisplayType.Normal, true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        #endregion

        #region --------- Export New --------------
        public string ExportNoVehicleWtcNew(NumberVehicleWtcModel model = null)
        {
            string pathFile = "";
            var input = Mapper.Map<VehicleWtcGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsNoVehicleWtcNew(input);
            //pathFile = createChart();
            return pathFile;

        }

        private string CreateXlsNoVehicleWtcNew(VehicleWtcGetByParamInput input)
        {
            //get data
            List<NoVehicleWtcDto> data = _execSummBLL.GetNoOfVehicleWtcData(input);
            var listData = Mapper.Map<List<NoVehicleWtcData>>(data);

            var slDocument = new SLDocument();

            //create data
            slDocument = CreateDataExcelDashboardWtcNew(slDocument, listData, input);

            var fileName = "ExecSum_NumbVehicleWtc" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateDataExcelDashboardWtcNew(SLDocument slDocument, List<NoVehicleWtcData> listData, VehicleWtcGetByParamInput input)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var nextColumn = 3;
            var endColumn = 3;
            var minColumn = 10;

            //select distinct data
            var dataList = listData.OrderBy(x => x.Regional).Select(x => x.Regional).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY REGIONAL");

            nextColumn = firstColumn + 1;
            endColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "SALES " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "MARKETING " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, (string.IsNullOrEmpty(item) || item == "0") ? "HO" : item);

                nextColumn = firstColumn + 1;
                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == "SALES"
                                                        && x.Regional == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.NoOfVehicle);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == "MARKETING"
                                                        && x.Regional == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.NoOfVehicle);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");

            nextColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == "SALES"
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.NoOfVehicle);

                slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == "MARKETING"
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.NoOfVehicle);

                slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle rightStyleChart = slDocument.CreateStyle();
            rightStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Right;

            slDocument.AutoFitColumn(firstColumn, endColumn - 1);

            if (minColumn < endColumn)
            {
                minColumn = endColumn;
            }
            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, minColumn, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, minColumn, headerStyleNumbChart);
            slDocument.SetCellStyle(contRow, firstColumn + 1, contRow, minColumn, rightStyleChart);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, endColumn - 1);
            chart.SetChartStyle(SLChartStyle.Style31);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, minColumn);
            chart.Title.SetTitle("Vehicle By Regional");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            //title
            slDocument.SetCellValue(1, 2, "Number Of Vehicle WTC");
            slDocument.MergeWorksheetCells(1, 2, 1, minColumn);

            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 14;
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Aqua, System.Drawing.Color.Aqua);
            slDocument.SetCellStyle(1, 2, 1, minColumn, valueStyle);

            #endregion

            return slDocument;
        }

        #endregion

        #endregion

        #region --------- Number Of Vehicle Make --------------

        public ActionResult VehicleMake(int? month, int? year)
        {
            var model = new NumberVehicleMakeModel();
            model.SearchView.YearFrom = DateTime.Now.Year;
            model.SearchView.YearTo = DateTime.Now.Year;

            if (month.HasValue)
            {
                model.SearchView.MonthFrom = month.Value;
                model.SearchView.MonthTo = month.Value;
            }
            if (year.HasValue)
            {
                model.SearchView.YearFrom = year.Value;
                model.SearchView.YearTo = year.Value;
            }

            var input = Mapper.Map<VehicleMakeGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetNoOfVehicleMakeData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);

            model.TitleForm = "Number Of Vehicle Make - Type";
            model.TitleExport = "ExportNoVehicleMakeNew";
            model.NoVehicleMakeList = Mapper.Map<List<NoVehicleMakeData>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View(model);
        }

        [HttpPost]
        public PartialViewResult FilterNoVehicleMake(NumberVehicleMakeModel model)
        {
            model.NoVehicleMakeList = GetVehicleMakeData(model.SearchView);
            return PartialView("_ListVehicleMake", model);
        }

        private List<NoVehicleMakeData> GetVehicleMakeData(VehicleSearchViewMake filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _execSummBLL.GetNoOfVehicleMakeData(new VehicleMakeGetByParamInput()).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
                return Mapper.Map<List<NoVehicleMakeData>>(data);
            }

            //getbyparams
            var input = Mapper.Map<VehicleMakeGetByParamInput>(filter);

            var dbData = _execSummBLL.GetNoOfVehicleMakeData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            return Mapper.Map<List<NoVehicleMakeData>>(dbData);
        }

        #region --------- Export --------------
        public string ExportNoVehicleMake(NumberVehicleMakeModel model = null)
        {
            string pathFile = "";

            var input = Mapper.Map<VehicleMakeGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsNoVehicleMake(input);

            //pathFile = createChart();
            return pathFile;

        }
       
        private string CreateXlsNoVehicleMake(VehicleMakeGetByParamInput input)
        {
            //get data
            List<NoVehicleMakeDto> data = _execSummBLL.GetNoOfVehicleMakeData(input);
            var listData = Mapper.Map<List<NoVehicleMakeData>>(data);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Number Of Vehicle Make-Type");
            slDocument.MergeWorksheetCells(1, 1, 1, 5);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelDashboardMake(slDocument);

            //create data
            slDocument = CreateDataExcelDashboardMake(slDocument, listData);

            var fileName = "ExecSum_NumbVehicleMake" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelDashboardMake(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Manufacturer");
            slDocument.SetCellValue(iRow, 2, "Body Type");
            slDocument.SetCellValue(iRow, 3, "Month");
            slDocument.SetCellValue(iRow, 4, "Year");
            slDocument.SetCellValue(iRow, 5, "No Of Vehicle");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 5, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelDashboardMake(SLDocument slDocument, List<NoVehicleMakeData> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData.OrderBy(x => x.ReportMonth).OrderBy(x => x.ReportYear))
            {
                slDocument.SetCellValue(iRow, 1, data.Manufacturer);
                slDocument.SetCellValue(iRow, 2, data.BodyType);
                slDocument.SetCellValue(iRow, 3, data.Month);
                slDocument.SetCellValue(iRow, 4, data.ReportYear.ToString());
                slDocument.SetCellValueNumeric(iRow, 5, data.NoOfVehicle.ToString());

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 5);
            slDocument.SetCellStyle(3, 1, iRow - 1, 5, valueStyle);

            //add row for total
            slDocument.SetCellValue(iRow, 1, "Total");
            slDocument.MergeWorksheetCells(iRow, 1, iRow, 4);
            slDocument.SetCellValueNumeric(iRow, 5, listData.Sum(x => x.NoOfVehicle.Value).ToString());

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 5, headerStyle);

            #region --------- Chart --------------

            //select distinct year
            var yearList = listData.OrderBy(x => x.ReportYear).Select(x => x.ReportYear).Distinct();
            var startColumYear = 7;
            var startRowYear = 3;
            var endRowYear = 3;

            foreach (var year in yearList)
            {
                slDocument.SetCellValue(endRowYear, startColumYear, year.ToString());
                endRowYear++;
            }

            slDocument.SetCellValue(endRowYear, startColumYear, "Total");

            //select distinct Manufacturer
            var functionList = listData.OrderBy(x => x.Manufacturer).Select(x => x.Manufacturer).Distinct();
            var startColum = 8;
            var endColum = 8;
            var startRow = 2;
            var startRowCount = 3;

            foreach (var item in functionList)
            {
                slDocument.SetCellValue(startRow, endColum, string.IsNullOrEmpty(item) ? "No Manufacturer" : item);
                startRowCount = 3;

                foreach (var year in yearList)
                {
                    var vehicleCountYear = listData.Where(x => x.Manufacturer == item && x.ReportYear == year).Sum(x => x.NoOfVehicle);

                    slDocument.SetCellValueNumeric(startRowCount, endColum, vehicleCountYear.ToString());
                    startRowCount++;
                }

                var vehicleCount = listData.Where(x => x.Manufacturer == item).Sum(x => x.NoOfVehicle);
                slDocument.SetCellValueNumeric(startRowCount, endColum, vehicleCount.ToString());

                endColum++;
            }

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(startRowYear, startColumYear, endRowYear, startColumYear, headerStyleChart);
            slDocument.SetCellStyle(startRow, startColum, startRow, endColum - 1, headerStyleChart);
            slDocument.SetCellStyle(startRowCount, startColum, startRowCount, endColum - 1, headerStyleNumbChart);

            SLChart chart = slDocument.CreateChart(startRowYear - 1, startColumYear, endRowYear, endColum - 1);
            chart.SetChartStyle(SLChartStyle.Style46);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(endRowYear + 2, 7, endRowYear + 24, 16);
            chart.PlotDataSeriesAsPrimaryLineChart(3, SLChartDataDisplayType.Normal, true);
            chart.PlotDataSeriesAsSecondaryLineChart(4, SLChartDataDisplayType.Normal, false);
            chart.PlotDataSeriesAsSecondaryLineChart(2, SLChartDataDisplayType.Normal, true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        #endregion

        #region --------- Export New --------------
        public string ExportNoVehicleMakeNew(NumberVehicleMakeModel model = null)
        {
            string pathFile = "";

            var input = Mapper.Map<VehicleMakeGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsNoVehicleMakeNew(input);

            //pathFile = createChart();
            return pathFile;
        }

        private string CreateXlsNoVehicleMakeNew(VehicleMakeGetByParamInput input)
        {
            //get data
            List<NoVehicleMakeDto> data = _execSummBLL.GetNoOfVehicleMakeData(input);
            var listData = Mapper.Map<List<NoVehicleMakeData>>(data);

            var slDocument = new SLDocument();

            //create data
            slDocument = CreateDataExcelDashboardMakeNew(slDocument, listData, input);

            var fileName = "ExecSum_NumbVehicleMake" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateDataExcelDashboardMakeNew(SLDocument slDocument, List<NoVehicleMakeData> listData, VehicleMakeGetByParamInput input)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var nextColumn = 3;
            var endColumn = 3;
            var minColumn = 8;

            //select distinct data
            var dataList = listData.OrderBy(x => x.Manufacturer).Select(x => x.Manufacturer).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY MANUFACTURER");

            nextColumn = firstColumn + 1;
            endColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "TOTAL " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "No Manufacturer" : item);

                nextColumn = firstColumn + 1;
                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => x.Manufacturer == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.NoOfVehicle);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");

            nextColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.NoOfVehicle);

                slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle rightStyleChart = slDocument.CreateStyle();
            rightStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Right;

            slDocument.AutoFitColumn(firstColumn, endColumn - 1);

            if (minColumn < endColumn)
            {
                minColumn = endColumn;
            }
            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, minColumn, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, minColumn, headerStyleNumbChart);
            slDocument.SetCellStyle(contRow, firstColumn + 1, contRow, minColumn, rightStyleChart);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, endColumn - 1);
            chart.SetChartStyle(SLChartStyle.Style30);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, minColumn);
            chart.Title.SetTitle("Vehicle By Make");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            #endregion

            #region --------- Chart 2--------------

            firstRow = 14;
            contRow = 15;
            firstColumn = minColumn + 2;
            nextColumn = minColumn + 3;
            endColumn = minColumn + 3;
            minColumn = minColumn + 8;

            //select distinct data
            dataList = listData.OrderBy(x => x.BodyType).Select(x => x.BodyType).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY TYPE");

            nextColumn = firstColumn + 1;
            endColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "TOTAL " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "No Body Type" : item);

                nextColumn = firstColumn + 1;
                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => x.BodyType == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.NoOfVehicle);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");

            nextColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.NoOfVehicle);

                slDocument.SetCellValue(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            slDocument.AutoFitColumn(firstColumn, endColumn - 1);

            if (minColumn < endColumn)
            {
                minColumn = endColumn;
            }
            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, minColumn, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, minColumn, headerStyleNumbChart);
            slDocument.SetCellStyle(contRow, firstColumn + 1, contRow, minColumn, rightStyleChart);

            chart = slDocument.CreateChart(firstRow, firstColumn, contRow, endColumn - 1);
            chart.SetChartStyle(SLChartStyle.Style32);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, firstColumn - 1, firstRow - 2, minColumn);
            chart.Title.SetTitle("Vehicle By Type");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            //title
            slDocument.SetCellValue(1, 2, "Number Of Vehicle Make-Type");
            slDocument.MergeWorksheetCells(1, 2, 1, minColumn);

            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 14;
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Aqua, System.Drawing.Color.Aqua);
            slDocument.SetCellStyle(1, 2, 1, minColumn, valueStyle);

            #endregion

            return slDocument;
        }

        #endregion

        #endregion

        #region --------- Odometer --------------

        public ActionResult Odometer(int? month, int? year)
        {
            var model = new OdometerModel();
            model.SearchView.YearFrom = DateTime.Now.Year;
            model.SearchView.YearTo = DateTime.Now.Year;

            if (month.HasValue)
            {
                model.SearchView.MonthFrom = month.Value;
                model.SearchView.MonthTo = month.Value;
            }
            if (year.HasValue)
            {
                model.SearchView.YearFrom = year.Value;
                model.SearchView.YearTo = year.Value;
            }
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
            var LocationMappingList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).ToList();
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.Logistic)
            {
                var GetCity = EmployeeList.Where(x => x.EMPLOYEE_ID == CurrentUser.EMPLOYEE_ID && x.IS_ACTIVE).FirstOrDefault();
                if (GetCity != null)
                {
                    var GetRegion = LocationMappingList.Where(x => (x.Location == null ? "" : x.Location.ToUpper()) == (GetCity.CITY == null ? "" : GetCity.CITY.ToUpper())).FirstOrDefault();
                    if (GetRegion != null)
                    {
                        model.SearchView.Zone = GetRegion.ZoneSales;
                        model.SearchView.ZoneId = GetRegion.ZoneSales;
                    }
                }
            }
            var input = Mapper.Map<OdometerGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetOdometerData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();
            var listVehType = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.IsActive).Select(x => new { x.SettingValue }).ToList();
            var listZone = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.ZoneSales }).Distinct().ToList();

            model.TitleForm = "Odometer";
            model.TitleExport = "ExportOdometerNew";
            model.OdometerDataList = Mapper.Map<List<OdometerData>>(data);
            model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
            model.SearchView.Functions = GetFunctionsMultiSelectList();
            model.SearchView.ZoneList = new SelectList(listZone, "ZoneSales", "ZoneSales");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View(model);
        }

        [HttpPost]
        public PartialViewResult FilterOdometer(OdometerModel model)
        {
            model.OdometerDataList = GetOdometerData(model.SearchView);
            return PartialView("_ListOdometer", model);
        }

        private List<OdometerData> GetOdometerData(OdometerSearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _execSummBLL.GetOdometerData(new OdometerGetByParamInput()).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
                return Mapper.Map<List<OdometerData>>(data);
            }

            //getbyparams
            filter.Function = filter.FunctionId;
            filter.ZoneId = filter.Zone;
            var input = Mapper.Map<OdometerGetByParamInput>(filter);

            var dbData = _execSummBLL.GetOdometerData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            return Mapper.Map<List<OdometerData>>(dbData);
        }

        #region --------- Export --------------
        public string ExportOdometer(OdometerModel model = null)
        {
            string pathFile = "";

            var input = Mapper.Map<OdometerGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsOdometer(input);

            //pathFile = createChart();
            return pathFile;

        }
      
        private string CreateXlsOdometer(OdometerGetByParamInput input)
        {
            //get data
            List<OdometerDto> data = _execSummBLL.GetOdometerData(input);
            var listData = Mapper.Map<List<OdometerData>>(data);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Odometer");
            slDocument.MergeWorksheetCells(1, 1, 1, 6);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelDashboardOdometer(slDocument);

            //create data
            slDocument = CreateDataExcelDashboardOdometer(slDocument, listData);

            var fileName = "ExecSum_Odometer" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelDashboardOdometer(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Vehicle Type");
            slDocument.SetCellValue(iRow, 2, "Regional");
            slDocument.SetCellValue(iRow, 3, "Function");
            slDocument.SetCellValue(iRow, 4, "Month");
            slDocument.SetCellValue(iRow, 5, "Year");
            slDocument.SetCellValue(iRow, 6, "Total KM");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 6, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelDashboardOdometer(SLDocument slDocument, List<OdometerData> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData.OrderBy(x => x.ReportMonth).OrderBy(x => x.ReportYear))
            {
                slDocument.SetCellValue(iRow, 1, data.VehicleType);
                slDocument.SetCellValue(iRow, 2, data.Region);
                slDocument.SetCellValue(iRow, 3, data.Function);
                slDocument.SetCellValue(iRow, 4, data.Month);
                slDocument.SetCellValue(iRow, 5, data.ReportYear.ToString());
                slDocument.SetCellValueNumeric(iRow, 6, data.TotalKm.ToString());

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 6);
            slDocument.SetCellStyle(3, 1, iRow - 1, 6, valueStyle);

            //add row for total
            slDocument.SetCellValue(iRow, 1, "Total");
            slDocument.MergeWorksheetCells(iRow, 1, iRow, 5);
            slDocument.SetCellValueNumeric(iRow, 6, listData.Sum(x => x.TotalKm.Value).ToString());

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 6, headerStyle);

            #region --------- Chart --------------

            //select distinct year
            var yearList = listData.OrderBy(x => x.ReportYear).Select(x => x.ReportYear).Distinct();
            var startColumYear = 8;
            var startRowYear = 3;
            var endRowYear = 3;

            foreach (var year in yearList)
            {
                slDocument.SetCellValue(endRowYear, startColumYear, year.ToString());
                endRowYear++;
            }

            slDocument.SetCellValue(endRowYear, startColumYear, "Total");

            //select distinct Function
            var functionList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct();
            var startColum = 9;
            var endColum = 9;
            var startRow = 2;
            var startRowCount = 3;

            foreach (var item in functionList)
            {
                slDocument.SetCellValue(startRow, endColum, string.IsNullOrEmpty(item) ? "No Function" : item);
                startRowCount = 3;

                foreach (var year in yearList)
                {
                    var vehicleCountYear = listData.Where(x => x.Function == item && x.ReportYear == year).Sum(x => x.TotalKm);

                    slDocument.SetCellValueNumeric(startRowCount, endColum, vehicleCountYear.ToString());
                    startRowCount++;
                }

                var vehicleCount = listData.Where(x => x.Function == item).Sum(x => x.TotalKm);
                slDocument.SetCellValueNumeric(startRowCount, endColum, vehicleCount.ToString());

                endColum++;
            }

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(startRowYear, startColumYear, endRowYear, startColumYear, headerStyleChart);
            slDocument.SetCellStyle(startRow, startColum, startRow, endColum - 1, headerStyleChart);
            slDocument.SetCellStyle(startRowCount, startColum, startRowCount, endColum - 1, headerStyleNumbChart);

            SLChart chart = slDocument.CreateChart(startRowYear - 1, startColumYear, endRowYear, endColum - 1);
            chart.SetChartStyle(SLChartStyle.Style46);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(endRowYear + 2, 8, endRowYear + 24, 17);
            chart.PlotDataSeriesAsPrimaryLineChart(3, SLChartDataDisplayType.Normal, true);
            chart.PlotDataSeriesAsSecondaryLineChart(4, SLChartDataDisplayType.Normal, false);
            chart.PlotDataSeriesAsSecondaryLineChart(2, SLChartDataDisplayType.Normal, true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        #endregion

        #region --------- Export New --------------
        public string ExportOdometerNew(OdometerModel model = null)
        {
            string pathFile = "";

            var input = Mapper.Map<OdometerGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsOdometerNew(input);

            //pathFile = createChart();
            return pathFile;

        }

        private string CreateXlsOdometerNew(OdometerGetByParamInput input)
        {
            //get data
            List<OdometerDto> data = _execSummBLL.GetOdometerData(input);
            var listData = Mapper.Map<List<OdometerData>>(data);

            var slDocument = new SLDocument();

            

            //create data
            slDocument = CreateDataExcelDashboardOdometerNew(slDocument, listData, input);

            var fileName = "ExecSum_Odometer" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateDataExcelDashboardOdometerNew(SLDocument slDocument, List<OdometerData> listData, OdometerGetByParamInput input)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var nextColumn = 3;
            var endColumn = 3;
            var minColumn = 10;

            //select distinct data
            var dataList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY FUNCTION");

            nextColumn = firstColumn + 1;
            endColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "BENEFIT " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "WTC " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "No Function" : item);

                nextColumn = firstColumn + 1;
                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT"
                                                        && x.Function == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.TotalKm);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC"
                                                        && x.Function == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.TotalKm);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");

            nextColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT"
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.TotalKm);

                slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC"
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.TotalKm);

                slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle rightStyleChart = slDocument.CreateStyle();
            rightStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Right;

            SLStyle numberStyle = slDocument.CreateStyle();
            numberStyle.FormatCode = "#,##0.00";

            if (minColumn < endColumn)
            {
                minColumn = endColumn;
            }
            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, minColumn, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, minColumn, headerStyleNumbChart);
            slDocument.SetCellStyle(contRow, firstColumn + 1, contRow, minColumn, rightStyleChart);
            slDocument.SetCellStyle(firstRow + 1, firstColumn + 1, contRow, minColumn, numberStyle);
            slDocument.AutoFitColumn(firstColumn, endColumn - 1);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, endColumn - 1);
            chart.SetChartStyle(SLChartStyle.Style31);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, minColumn);
            chart.Title.SetTitle("Monthly KM's driven By Function");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            //title
            slDocument.SetCellValue(1, 2, "Odometer");
            slDocument.MergeWorksheetCells(1, 2, 1, minColumn);

            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 14;
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Aqua, System.Drawing.Color.Aqua);
            slDocument.SetCellStyle(1, 2, 1, minColumn, valueStyle);

            #endregion

            return slDocument;
        }

        #endregion

        #endregion

        #region --------- Liter By Function --------------

        public ActionResult LiterByFunction(int? month, int? year)
        {
            var model = new LiterByFunctionModel();
            model.SearchView.YearFrom = DateTime.Now.Year;
            model.SearchView.YearTo = DateTime.Now.Year;

            if (month.HasValue)
            {
                model.SearchView.MonthFrom = month.Value;
                model.SearchView.MonthTo = month.Value;
            }
            if (year.HasValue)
            {
                model.SearchView.YearFrom = year.Value;
                model.SearchView.YearTo = year.Value;
            }
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
            var LocationMappingList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).ToList();
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.Logistic)
            {
                var GetCity = EmployeeList.Where(x => x.EMPLOYEE_ID == CurrentUser.EMPLOYEE_ID && x.IS_ACTIVE).FirstOrDefault();
                if (GetCity != null)
                {
                    var GetRegion = LocationMappingList.Where(x => (x.Location == null ? "" : x.Location.ToUpper()) == (GetCity.CITY == null ? "" : GetCity.CITY.ToUpper())).FirstOrDefault();
                    if (GetRegion != null)
                    {
                        model.SearchView.ZoneId = GetRegion.ZoneSales;
                        model.SearchView.Zone = GetRegion.ZoneSales;
                    }
                }
            }
            var input = Mapper.Map<LiterFuncGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetLiterByFunctionData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();
            var listVehType = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.IsActive).Select(x => new { x.SettingValue }).ToList();
            var listZone = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.ZoneSales }).Distinct().ToList();

            model.TitleForm = "Fuel Purchased (liters)";
            model.TitleExport = "ExportLiterByFunctionNew";
            model.LiterByFuncDataList = Mapper.Map<List<LiterByFunctionData>>(data);
            model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
            model.SearchView.Functions = GetFunctionsMultiSelectList();
            model.SearchView.ZoneList = new SelectList(listZone, "ZoneSales", "ZoneSales");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View(model);
        }

        [HttpPost]
        public PartialViewResult FilterLiterByFunction(LiterByFunctionModel model)
        {
            model.LiterByFuncDataList = GetLiterByFunctionData(model.SearchView);
            return PartialView("_ListLiterByFunction", model);
        }

        private List<LiterByFunctionData> GetLiterByFunctionData(LiterByFuncSearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _execSummBLL.GetLiterByFunctionData(new LiterFuncGetByParamInput()).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
                return Mapper.Map<List<LiterByFunctionData>>(data);
            }

            //getbyparams
            filter.Function = filter.FunctionId;
            filter.ZoneId = filter.Zone;
            var input = Mapper.Map<LiterFuncGetByParamInput>(filter);

            var dbData = _execSummBLL.GetLiterByFunctionData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            return Mapper.Map<List<LiterByFunctionData>>(dbData);
        }

        #region --------- Export --------------
        public string ExportLiterByFunction(LiterByFunctionModel model = null)
        {
            string pathFile = "";

            var input = Mapper.Map<LiterFuncGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsLiterByFunction(input);

            //pathFile = createChart();
            return pathFile;

        }
      
        private string CreateXlsLiterByFunction(LiterFuncGetByParamInput input)
        {
            //get data
            List<LiterByFunctionDto> data = _execSummBLL.GetLiterByFunctionData(input);
            var listData = Mapper.Map<List<LiterByFunctionData>>(data);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Liter By Function");
            slDocument.MergeWorksheetCells(1, 1, 1, 6);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelDashboardLiterByFunction(slDocument);

            //create data
            slDocument = CreateDataExcelDashboardLiterByFunction(slDocument, listData);

            var fileName = "ExecSum_LiterByFunction" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelDashboardLiterByFunction(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Vehicle Type");
            slDocument.SetCellValue(iRow, 2, "Regional");
            slDocument.SetCellValue(iRow, 3, "Function");
            slDocument.SetCellValue(iRow, 4, "Month");
            slDocument.SetCellValue(iRow, 5, "Year");
            slDocument.SetCellValue(iRow, 6, "Total Liter");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 6, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelDashboardLiterByFunction(SLDocument slDocument, List<LiterByFunctionData> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData.OrderBy(x => x.ReportMonth).OrderBy(x => x.ReportYear))
            {
                slDocument.SetCellValue(iRow, 1, data.VehicleType);
                slDocument.SetCellValue(iRow, 2, data.Region);
                slDocument.SetCellValue(iRow, 3, data.Function);
                slDocument.SetCellValue(iRow, 4, data.Month);
                slDocument.SetCellValue(iRow, 5, data.ReportYear.ToString());
                slDocument.SetCellValueNumeric(iRow, 6, data.TotalLiter.ToString());

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 6);
            slDocument.SetCellStyle(3, 1, iRow - 1, 6, valueStyle);

            //add row for total
            slDocument.SetCellValue(iRow, 1, "Total");
            slDocument.MergeWorksheetCells(iRow, 1, iRow, 5);
            slDocument.SetCellValueNumeric(iRow, 6, listData.Sum(x => x.TotalLiter.Value).ToString());

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 6, headerStyle);

            #region --------- Chart --------------

            //select distinct year
            var yearList = listData.OrderBy(x => x.ReportYear).Select(x => x.ReportYear).Distinct();
            var startColumYear = 8;
            var startRowYear = 3;
            var endRowYear = 3;

            foreach (var year in yearList)
            {
                slDocument.SetCellValue(endRowYear, startColumYear, year.ToString());
                endRowYear++;
            }

            slDocument.SetCellValue(endRowYear, startColumYear, "Total");

            //select distinct Function
            var functionList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct();
            var startColum = 9;
            var endColum = 9;
            var startRow = 2;
            var startRowCount = 3;

            foreach (var item in functionList)
            {
                slDocument.SetCellValue(startRow, endColum, string.IsNullOrEmpty(item) ? "No Function" : item);
                startRowCount = 3;

                foreach (var year in yearList)
                {
                    var vehicleCountYear = listData.Where(x => x.Function == item && x.ReportYear == year).Sum(x => x.TotalLiter);

                    slDocument.SetCellValueNumeric(startRowCount, endColum, vehicleCountYear.ToString());
                    startRowCount++;
                }

                var vehicleCount = listData.Where(x => x.Function == item).Sum(x => x.TotalLiter);
                slDocument.SetCellValueNumeric(startRowCount, endColum, vehicleCount.ToString());

                endColum++;
            }

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(startRowYear, startColumYear, endRowYear, startColumYear, headerStyleChart);
            slDocument.SetCellStyle(startRow, startColum, startRow, endColum - 1, headerStyleChart);
            slDocument.SetCellStyle(startRowCount, startColum, startRowCount, endColum - 1, headerStyleNumbChart);

            SLChart chart = slDocument.CreateChart(startRowYear - 1, startColumYear, endRowYear, endColum - 1);
            chart.SetChartStyle(SLChartStyle.Style46);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(endRowYear + 2, 8, endRowYear + 24, 17);
            chart.PlotDataSeriesAsPrimaryLineChart(3, SLChartDataDisplayType.Normal, true);
            chart.PlotDataSeriesAsSecondaryLineChart(4, SLChartDataDisplayType.Normal, false);
            chart.PlotDataSeriesAsSecondaryLineChart(2, SLChartDataDisplayType.Normal, true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        #endregion

        #region --------- Export New --------------
        public string ExportLiterByFunctionNew(LiterByFunctionModel model = null)
        {
            string pathFile = "";

            var input = Mapper.Map<LiterFuncGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsLiterByFunctionNew(input);

            //pathFile = createChart();
            return pathFile;

        }

        private string CreateXlsLiterByFunctionNew(LiterFuncGetByParamInput input)
        {
            //get data
            List<LiterByFunctionDto> data = _execSummBLL.GetLiterByFunctionData(input);
            var listData = Mapper.Map<List<LiterByFunctionData>>(data);

            var slDocument = new SLDocument();

            //create data
            slDocument = CreateDataExcelDashboardLiterByFunctionNew(slDocument, listData, input);

            var fileName = "ExecSum_LiterByFunction" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateDataExcelDashboardLiterByFunctionNew(SLDocument slDocument, List<LiterByFunctionData> listData, LiterFuncGetByParamInput input)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var nextColumn = 3;
            var endColumn = 3;
            var minColumn = 10;

            //select distinct data
            var dataList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY FUNCTION");

            nextColumn = firstColumn + 1;
            endColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "BENEFIT " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "WTC " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "No Function" : item);

                nextColumn = firstColumn + 1;
                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT"
                                                        && x.Function == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.TotalLiter);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC"
                                                        && x.Function == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.TotalLiter);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");

            nextColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT"
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.TotalLiter);

                slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC"
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.TotalLiter);

                slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle rightStyleChart = slDocument.CreateStyle();
            rightStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Right;

            SLStyle numberStyle = slDocument.CreateStyle();
            numberStyle.FormatCode = "#,##0.00";

            if (minColumn < endColumn)
            {
                minColumn = endColumn;
            }
            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, minColumn, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, minColumn, headerStyleNumbChart);
            slDocument.SetCellStyle(contRow, firstColumn + 1, contRow, minColumn, rightStyleChart);
            slDocument.SetCellStyle(firstRow + 1, firstColumn + 1, contRow, minColumn, numberStyle);
            slDocument.AutoFitColumn(firstColumn, endColumn - 1);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, endColumn - 1);
            chart.SetChartStyle(SLChartStyle.Style30);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, minColumn);
            chart.Title.SetTitle("Monthly Fuel Purchased (in liters) By Regional");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            //title
            slDocument.SetCellValue(1, 2, "Liter By Function");
            slDocument.MergeWorksheetCells(1, 2, 1, minColumn);

            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 14;
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Aqua, System.Drawing.Color.Aqua);
            slDocument.SetCellStyle(1, 2, 1, minColumn, valueStyle);

            #endregion

            return slDocument;
        }

        #endregion

        #endregion

        #region --------- Fuel Cost By Function --------------

        public ActionResult FuelCostByFunction(int? month, int? year)
        {
            var model = new FuelCostByFunctionModel();
            model.SearchView.YearFrom = DateTime.Now.Year;
            model.SearchView.YearTo = DateTime.Now.Year;

            if (month.HasValue)
            {
                model.SearchView.MonthFrom = month.Value;
                model.SearchView.MonthTo = month.Value;
            }
            if (year.HasValue)
            {
                model.SearchView.YearFrom = year.Value;
                model.SearchView.YearTo = year.Value;
            }
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
            var LocationMappingList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).ToList();
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.Logistic)
            {
                var GetCity = EmployeeList.Where(x => x.EMPLOYEE_ID == CurrentUser.EMPLOYEE_ID && x.IS_ACTIVE).FirstOrDefault();
                if (GetCity != null)
                {
                    var GetRegion = LocationMappingList.Where(x => (x.Location == null ? "" : x.Location.ToUpper()) == (GetCity.CITY == null ? "" : GetCity.CITY.ToUpper())).FirstOrDefault();
                    if (GetRegion != null)
                    {
                        model.SearchView.ZoneId = GetRegion.ZoneSales;
                        model.SearchView.Zone = GetRegion.ZoneSales;
                    }
                }
            }
            var input = Mapper.Map<FuelCostFuncGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetFuelCostByFunctionData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();
            var listVehType = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.IsActive).Select(x => new { x.SettingValue }).ToList();
            var listZone = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.ZoneSales }).Distinct().ToList();

            model.TitleForm = "Fuel Cost";
            model.TitleExport = "ExportFuelCostByFunctionNew";
            model.FuelCostByFuncDataList = Mapper.Map<List<FuelCostByFunctionData>>(data);
            model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
            model.SearchView.Functions = GetFunctionsMultiSelectList();
            model.SearchView.ZoneList = new SelectList(listZone, "ZoneSales", "ZoneSales");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View(model);
        }

        [HttpPost]
        public PartialViewResult FilterFuelCostByFunction(FuelCostByFunctionModel model)
        {
            model.FuelCostByFuncDataList = GetFuelCostByFunctionData(model.SearchView);
            return PartialView("_ListFuelCostByFunction", model);
        }

        private List<FuelCostByFunctionData> GetFuelCostByFunctionData(FuelCostByFuncSearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _execSummBLL.GetFuelCostByFunctionData(new FuelCostFuncGetByParamInput()).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
                return Mapper.Map<List<FuelCostByFunctionData>>(data);
            }

            //getbyparams
            filter.Function = filter.FunctionId;
            filter.ZoneId = filter.Zone;
            var input = Mapper.Map<FuelCostFuncGetByParamInput>(filter);

            var dbData = _execSummBLL.GetFuelCostByFunctionData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            return Mapper.Map<List<FuelCostByFunctionData>>(dbData);
        }

        #region --------- Export --------------
        public string ExportFuelCostByFunction(FuelCostByFunctionModel model = null)
        {
            string pathFile = "";

            var input = Mapper.Map<FuelCostFuncGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsFuelCostByFunction(input);

            //pathFile = createChart();
            return pathFile;

        }
     
        private string CreateXlsFuelCostByFunction(FuelCostFuncGetByParamInput input)
        {
            //get data
            List<FuelCostByFunctionDto> data = _execSummBLL.GetFuelCostByFunctionData(input);
            var listData = Mapper.Map<List<FuelCostByFunctionData>>(data);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Fuel Cost By Function");
            slDocument.MergeWorksheetCells(1, 1, 1, 6);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelDashboardFuelCostByFunction(slDocument);

            //create data
            slDocument = CreateDataExcelDashboardFuelCostByFunction(slDocument, listData);

            var fileName = "ExecSum_FuelCostByFunction" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelDashboardFuelCostByFunction(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Vehicle Type");
            slDocument.SetCellValue(iRow, 2, "Regional");
            slDocument.SetCellValue(iRow, 3, "Function");
            slDocument.SetCellValue(iRow, 4, "Month");
            slDocument.SetCellValue(iRow, 5, "Year");
            slDocument.SetCellValue(iRow, 6, "Total Fuel Cost");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 6, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelDashboardFuelCostByFunction(SLDocument slDocument, List<FuelCostByFunctionData> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData.OrderBy(x => x.ReportMonth).OrderBy(x => x.ReportYear))
            {
                slDocument.SetCellValue(iRow, 1, data.VehicleType);
                slDocument.SetCellValue(iRow, 2, data.Region);
                slDocument.SetCellValue(iRow, 3, data.Function);
                slDocument.SetCellValue(iRow, 4, data.Month);
                slDocument.SetCellValue(iRow, 5, data.ReportYear.ToString());
                slDocument.SetCellValueNumeric(iRow, 6, data.TotalFuelCost.ToString());

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 6);
            slDocument.SetCellStyle(3, 1, iRow - 1, 6, valueStyle);

            //add row for total
            slDocument.SetCellValue(iRow, 1, "Total");
            slDocument.MergeWorksheetCells(iRow, 1, iRow, 5);
            slDocument.SetCellValueNumeric(iRow, 6, listData.Sum(x => x.TotalFuelCost.Value).ToString());

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 6, headerStyle);

            #region --------- Chart --------------

            //select distinct year
            var yearList = listData.OrderBy(x => x.ReportYear).Select(x => x.ReportYear).Distinct();
            var startColumYear = 8;
            var startRowYear = 3;
            var endRowYear = 3;

            foreach (var year in yearList)
            {
                slDocument.SetCellValue(endRowYear, startColumYear, year.ToString());
                endRowYear++;
            }

            slDocument.SetCellValue(endRowYear, startColumYear, "Total");

            //select distinct Function
            var functionList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct();
            var startColum = 9;
            var endColum = 9;
            var startRow = 2;
            var startRowCount = 3;

            foreach (var item in functionList)
            {
                slDocument.SetCellValue(startRow, endColum, string.IsNullOrEmpty(item) ? "No Function" : item);
                startRowCount = 3;

                foreach (var year in yearList)
                {
                    var vehicleCountYear = listData.Where(x => x.Function == item && x.ReportYear == year).Sum(x => x.TotalFuelCost);

                    slDocument.SetCellValueNumeric(startRowCount, endColum, vehicleCountYear.ToString());
                    startRowCount++;
                }

                var vehicleCount = listData.Where(x => x.Function == item).Sum(x => x.TotalFuelCost);
                slDocument.SetCellValueNumeric(startRowCount, endColum, vehicleCount.ToString());

                endColum++;
            }

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(startRowYear, startColumYear, endRowYear, startColumYear, headerStyleChart);
            slDocument.SetCellStyle(startRow, startColum, startRow, endColum - 1, headerStyleChart);
            slDocument.SetCellStyle(startRowCount, startColum, startRowCount, endColum - 1, headerStyleNumbChart);

            SLChart chart = slDocument.CreateChart(startRowYear - 1, startColumYear, endRowYear, endColum - 1);
            chart.SetChartStyle(SLChartStyle.Style46);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(endRowYear + 2, 8, endRowYear + 24, 17);
            chart.PlotDataSeriesAsPrimaryLineChart(3, SLChartDataDisplayType.Normal, true);
            chart.PlotDataSeriesAsSecondaryLineChart(4, SLChartDataDisplayType.Normal, false);
            chart.PlotDataSeriesAsSecondaryLineChart(2, SLChartDataDisplayType.Normal, true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        #endregion

        #region --------- Export New --------------
        public string ExportFuelCostByFunctionNew(FuelCostByFunctionModel model = null)
        {
            string pathFile = "";

            var input = Mapper.Map<FuelCostFuncGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsFuelCostByFunctionNew(input);

            //pathFile = createChart();
            return pathFile;

        }

        private string CreateXlsFuelCostByFunctionNew(FuelCostFuncGetByParamInput input)
        {
            //get data
            List<FuelCostByFunctionDto> data = _execSummBLL.GetFuelCostByFunctionData(input);
            var listData = Mapper.Map<List<FuelCostByFunctionData>>(data);

            var slDocument = new SLDocument();

            //create data
            slDocument = CreateDataExcelDashboardFuelCostByFunctionNew(slDocument, listData, input);

            var fileName = "ExecSum_FuelCostByFunction" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateDataExcelDashboardFuelCostByFunctionNew(SLDocument slDocument, List<FuelCostByFunctionData> listData, FuelCostFuncGetByParamInput input)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var nextColumn = 3;
            var endColumn = 3;
            var minColumn = 10;

            //select distinct data
            var dataList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY FUNCTION");

            nextColumn = firstColumn + 1;
            endColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "BENEFIT " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "WTC " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "No Function" : item);

                nextColumn = firstColumn + 1;
                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT"
                                                        && x.Function == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.TotalFuelCost);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC"
                                                        && x.Function == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.TotalFuelCost);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");

            nextColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT"
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.TotalFuelCost);

                slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC"
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.TotalFuelCost);

                slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle rightStyleChart = slDocument.CreateStyle();
            rightStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Right;

            SLStyle numberStyle = slDocument.CreateStyle();
            numberStyle.FormatCode = "#,##0.00";

            if (minColumn < endColumn)
            {
                minColumn = endColumn;
            }
            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, minColumn, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, minColumn, headerStyleNumbChart);
            slDocument.SetCellStyle(contRow, firstColumn + 1, contRow, minColumn, rightStyleChart);
            slDocument.SetCellStyle(firstRow + 1, firstColumn + 1, contRow, minColumn, numberStyle);
            slDocument.AutoFitColumn(firstColumn, endColumn - 1);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, endColumn - 1);
            chart.SetChartStyle(SLChartStyle.Style32);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, minColumn);
            chart.Title.SetTitle("Fuel Cost");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            //title
            slDocument.SetCellValue(1, 2, "Fuel Cost By Function");
            slDocument.MergeWorksheetCells(1, 2, 1, minColumn);

            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 14;
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Aqua, System.Drawing.Color.Aqua);
            slDocument.SetCellStyle(1, 2, 1, minColumn, valueStyle);

            #endregion

            return slDocument;
        }

        #endregion

        #endregion

        #region --------- Lease Cost By Function --------------

        public ActionResult LeaseCostByFunction(int? month, int? year)
        {
            var model = new LeaseCostByFunctionModel();
            model.SearchView.YearFrom = DateTime.Now.Year;
            model.SearchView.YearTo = DateTime.Now.Year;

            if (month.HasValue)
            {
                model.SearchView.MonthFrom = month.Value;
                model.SearchView.MonthTo = month.Value;
            }
            if (year.HasValue)
            {
                model.SearchView.YearFrom = year.Value;
                model.SearchView.YearTo = year.Value;
            }
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
            var LocationMappingList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).ToList();
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.Logistic)
            {
                var GetCity = EmployeeList.Where(x => x.EMPLOYEE_ID == CurrentUser.EMPLOYEE_ID && x.IS_ACTIVE).FirstOrDefault();
                if (GetCity != null)
                {
                    var GetRegion = LocationMappingList.Where(x => (x.Location == null ? "" : x.Location.ToUpper()) == (GetCity.CITY == null ? "" : GetCity.CITY.ToUpper())).FirstOrDefault();
                    if (GetRegion != null)
                    {
                        model.SearchView.ZoneId = GetRegion.ZoneSales;
                        model.SearchView.Zone = GetRegion.ZoneSales;
                    }
                }
            }
            var input = Mapper.Map<LeaseCostFuncGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetLeaseCostByFunctionData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();
            var listZone = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.ZoneSales }).Distinct().ToList();
            
            model.TitleForm = "Lease Cost";
            model.TitleExport = "ExportLeaseCostByFunctionNew";
            model.LeaseCostByFuncDataList = Mapper.Map<List<LeaseCostByFunctionData>>(data);
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
            model.SearchView.Functions = GetFunctionsMultiSelectList();
            model.SearchView.ZoneList = new SelectList(listZone, "ZoneSales", "ZoneSales");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View(model);
        }

        [HttpPost]
        public PartialViewResult FilterLeaseCostByFunction(LeaseCostByFunctionModel model)
        {
            model.LeaseCostByFuncDataList = GetLeaseCostByFunctionData(model.SearchView);
            return PartialView("_ListLeaseCostByFunction", model);
        }

        private List<LeaseCostByFunctionData> GetLeaseCostByFunctionData(LeaseCostByFuncSearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _execSummBLL.GetLeaseCostByFunctionData(new LeaseCostFuncGetByParamInput()).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
                return Mapper.Map<List<LeaseCostByFunctionData>>(data);
            }

            //getbyparams
            filter.Function = filter.FunctionId;
            filter.ZoneId = filter.Zone;
            var input = Mapper.Map<LeaseCostFuncGetByParamInput>(filter);

            var dbData = _execSummBLL.GetLeaseCostByFunctionData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            return Mapper.Map<List<LeaseCostByFunctionData>>(dbData);
        }

        #region --------- Export --------------
        public string ExportLeaseCostByFunction(LeaseCostByFunctionModel model = null)
        {
            string pathFile = "";
            
            var input = Mapper.Map<LeaseCostFuncGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsLeaseCostByFunction(input);

            //pathFile = createChart();
            return pathFile;

        }

        private string CreateXlsLeaseCostByFunction(LeaseCostFuncGetByParamInput input)
        {
            //get data
            List<LeaseCostByFunctionDto> data = _execSummBLL.GetLeaseCostByFunctionData(input);
            var listData = Mapper.Map<List<LeaseCostByFunctionData>>(data);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Lease Cost By Function");
            slDocument.MergeWorksheetCells(1, 1, 1, 5);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelDashboardLeaseCostByFunction(slDocument);

            //create data
            slDocument = CreateDataExcelDashboardLeaseCostByFunction(slDocument, listData);

            var fileName = "ExecSum_LeaseCostByFunction" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelDashboardLeaseCostByFunction(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Regional");
            slDocument.SetCellValue(iRow, 2, "Function");
            slDocument.SetCellValue(iRow, 3, "Month");
            slDocument.SetCellValue(iRow, 4, "Year");
            slDocument.SetCellValue(iRow, 5, "Total Lease Cost");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 5, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelDashboardLeaseCostByFunction(SLDocument slDocument, List<LeaseCostByFunctionData> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData.OrderBy(x => x.ReportMonth).OrderBy(x => x.ReportYear))
            {
                slDocument.SetCellValue(iRow, 1, data.Region);
                slDocument.SetCellValue(iRow, 2, data.Function);
                slDocument.SetCellValue(iRow, 3, data.Month);
                slDocument.SetCellValue(iRow, 4, data.ReportYear.ToString());
                slDocument.SetCellValueNumeric(iRow, 5, data.TotalLeaseCost.ToString());

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 5);
            slDocument.SetCellStyle(3, 1, iRow - 1, 5, valueStyle);

            //add row for total
            slDocument.SetCellValue(iRow, 1, "Total");
            slDocument.MergeWorksheetCells(iRow, 1, iRow, 4);
            slDocument.SetCellValueNumeric(iRow, 5, listData.Sum(x => x.TotalLeaseCost.Value).ToString());

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 5, headerStyle);

            #region --------- Chart --------------

            //select distinct year
            var yearList = listData.OrderBy(x => x.ReportYear).Select(x => x.ReportYear).Distinct();
            var startColumYear = 7;
            var startRowYear = 3;
            var endRowYear = 3;

            foreach (var year in yearList)
            {
                slDocument.SetCellValue(endRowYear, startColumYear, year.ToString());
                endRowYear++;
            }

            slDocument.SetCellValue(endRowYear, startColumYear, "Total");

            //select distinct Function
            var functionList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct();
            var startColum = 8;
            var endColum = 8;
            var startRow = 2;
            var startRowCount = 3;

            foreach (var item in functionList)
            {
                slDocument.SetCellValue(startRow, endColum, string.IsNullOrEmpty(item) ? "No Function" : item);
                startRowCount = 3;

                foreach (var year in yearList)
                {
                    var vehicleCountYear = listData.Where(x => x.Function == item && x.ReportYear == year).Sum(x => x.TotalLeaseCost);

                    slDocument.SetCellValueNumeric(startRowCount, endColum, vehicleCountYear.ToString());
                    startRowCount++;
                }

                var vehicleCount = listData.Where(x => x.Function == item).Sum(x => x.TotalLeaseCost);
                slDocument.SetCellValueNumeric(startRowCount, endColum, vehicleCount.ToString());

                endColum++;
            }

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(startRowYear, startColumYear, endRowYear, startColumYear, headerStyleChart);
            slDocument.SetCellStyle(startRow, startColum, startRow, endColum - 1, headerStyleChart);
            slDocument.SetCellStyle(startRowCount, startColum, startRowCount, endColum - 1, headerStyleNumbChart);

            SLChart chart = slDocument.CreateChart(startRowYear - 1, startColumYear, endRowYear, endColum - 1);
            chart.SetChartStyle(SLChartStyle.Style46);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(endRowYear + 2, 7, endRowYear + 24, 16);
            chart.PlotDataSeriesAsPrimaryLineChart(3, SLChartDataDisplayType.Normal, true);
            chart.PlotDataSeriesAsSecondaryLineChart(4, SLChartDataDisplayType.Normal, false);
            chart.PlotDataSeriesAsSecondaryLineChart(2, SLChartDataDisplayType.Normal, true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        #endregion

        #region --------- Export New --------------
        public string ExportLeaseCostByFunctionNew(LeaseCostByFunctionModel model = null)
        {
            string pathFile = "";

            var input = Mapper.Map<LeaseCostFuncGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsLeaseCostByFunctionNew(input);

            //pathFile = createChart();
            return pathFile;

        }

        private string CreateXlsLeaseCostByFunctionNew(LeaseCostFuncGetByParamInput input)
        {
            //get data
            List<LeaseCostByFunctionDto> data = _execSummBLL.GetLeaseCostByFunctionData(input);
            var listData = Mapper.Map<List<LeaseCostByFunctionData>>(data);

            var slDocument = new SLDocument();

            //create data
            slDocument = CreateDataExcelDashboardLeaseCostByFunctionNew(slDocument, listData, input);

            var fileName = "ExecSum_LeaseCostByFunction" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateDataExcelDashboardLeaseCostByFunctionNew(SLDocument slDocument, List<LeaseCostByFunctionData> listData, LeaseCostFuncGetByParamInput input)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var nextColumn = 3;
            var endColumn = 3;
            var minColumn = 10;

            //select distinct data
            var dataList = listData.OrderBy(x => x.Region).Select(x => x.Region).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY REGIONAL");

            nextColumn = firstColumn + 1;
            endColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "SALES " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "MARKETING " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, (string.IsNullOrEmpty(item) || item == "0") ? "HO" : item);

                nextColumn = firstColumn + 1;
                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == "SALES"
                                                        && x.Region == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.TotalLeaseCost);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == "MARKETING"
                                                        && x.Region == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.TotalLeaseCost);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");

            nextColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == "SALES"
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.TotalLeaseCost);

                slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => (x.Function == null ? "" : x.Function.ToUpper()) == "MARKETING"
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.TotalLeaseCost);

                slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle rightStyleChart = slDocument.CreateStyle();
            rightStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Right;

            SLStyle numberStyle = slDocument.CreateStyle();
            numberStyle.FormatCode = "#,##0.00";

            if (minColumn < endColumn)
            {
                minColumn = endColumn;
            }
            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, minColumn, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, minColumn, headerStyleNumbChart);
            slDocument.SetCellStyle(contRow, firstColumn + 1, contRow, minColumn, rightStyleChart);
            slDocument.SetCellStyle(firstRow + 1, firstColumn + 1, contRow, minColumn, numberStyle);
            slDocument.AutoFitColumn(firstColumn, endColumn - 1);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, endColumn - 1);
            chart.SetChartStyle(SLChartStyle.Style31);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, minColumn);
            chart.Title.SetTitle("Lease Cost");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            //title
            slDocument.SetCellValue(1, 2, "Lease Cost By Function");
            slDocument.MergeWorksheetCells(1, 2, 1, minColumn);

            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 14;
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Aqua, System.Drawing.Color.Aqua);
            slDocument.SetCellStyle(1, 2, 1, minColumn, valueStyle);
            
            #endregion

            return slDocument;
        }

        #endregion

        #endregion

        #region --------- Sales By Region --------------

        public ActionResult SalesByRegion(int? month, int? year)
        {
            var model = new SalesByRegionModel();
            model.SearchView.YearFrom = DateTime.Now.Year;
            model.SearchView.YearTo = DateTime.Now.Year;

            if (month.HasValue)
            {
                model.SearchView.MonthFrom = month.Value;
                model.SearchView.MonthTo = month.Value;
            }
            if (year.HasValue)
            {
                model.SearchView.YearFrom = year.Value;
                model.SearchView.YearTo = year.Value;
            }
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
            var LocationMappingList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).ToList();
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.Logistic)
            {
                var GetCity = EmployeeList.Where(x => x.EMPLOYEE_ID == CurrentUser.EMPLOYEE_ID && x.IS_ACTIVE).FirstOrDefault();
                if (GetCity != null)
                {
                    var GetRegion = LocationMappingList.Where(x => (x.Location == null ? "" : x.Location.ToUpper()) == (GetCity.CITY == null ? "" : GetCity.CITY.ToUpper())).FirstOrDefault();
                    if (GetRegion != null)
                    {
                        model.SearchView.ZoneId = GetRegion.ZoneSales;
                        model.SearchView.Zone = GetRegion.ZoneSales;
                    }
                }
            }
            var input = Mapper.Map<SalesRegionGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetSalesByRegionData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();
            var listZone = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.ZoneSales }).Distinct().ToList();

            model.TitleForm = "Operational Cost";
            model.TitleExport = "ExportSalesByRegionNew";
            model.SalesByRegionDataList = Mapper.Map<List<SalesByRegionData>>(data);
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
            model.SearchView.ZoneList = new SelectList(listZone, "ZoneSales", "ZoneSales");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View(model);
        }

        [HttpPost]
        public PartialViewResult FilterSalesByRegion(SalesByRegionModel model)
        {
            model.SalesByRegionDataList = GetSalesByRegionData(model.SearchView);
            return PartialView("_ListSalesByRegion", model);
        }

        private List<SalesByRegionData> GetSalesByRegionData(SalesByRegionSearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _execSummBLL.GetSalesByRegionData(new SalesRegionGetByParamInput()).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
                return Mapper.Map<List<SalesByRegionData>>(data);
            }

            //getbyparams
            filter.ZoneId = filter.Zone;
            var input = Mapper.Map<SalesRegionGetByParamInput>(filter);

            var dbData = _execSummBLL.GetSalesByRegionData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            return Mapper.Map<List<SalesByRegionData>>(dbData);
        }

        #region --------- Export --------------
        public string ExportSalesByRegion(SalesByRegionModel model = null)
        {
            string pathFile = "";

            var input = Mapper.Map<SalesRegionGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsSalesByRegion(input);


            //pathFile = createChart();
            return pathFile;

        }
      

        private string CreateXlsSalesByRegion(SalesRegionGetByParamInput input)
        {
            //get data
            List<SalesByRegionDto> data = _execSummBLL.GetSalesByRegionData(input);
            var listData = Mapper.Map<List<SalesByRegionData>>(data);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Sales By Region");
            slDocument.MergeWorksheetCells(1, 1, 1, 6);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelDashboardSalesByRegion(slDocument);

            //create data
            slDocument = CreateDataExcelDashboardSalesByRegion(slDocument, listData);

            var fileName = "ExecSum_SalesByRegion" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelDashboardSalesByRegion(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Regional");
            slDocument.SetCellValue(iRow, 2, "Month");
            slDocument.SetCellValue(iRow, 3, "Year");
            slDocument.SetCellValue(iRow, 4, "Total KM");
            slDocument.SetCellValue(iRow, 5, "Total Cost");
            slDocument.SetCellValue(iRow, 6, "Stick");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 6, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelDashboardSalesByRegion(SLDocument slDocument, List<SalesByRegionData> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData.OrderBy(x => x.ReportMonth).OrderBy(x => x.ReportYear))
            {
                slDocument.SetCellValue(iRow, 1, data.Region);
                slDocument.SetCellValue(iRow, 2, data.Month);
                slDocument.SetCellValue(iRow, 3, data.ReportYear.ToString());
                slDocument.SetCellValueNumeric(iRow, 4, data.TotalKm.ToString());
                slDocument.SetCellValueNumeric(iRow, 5, data.TotalCost.ToString());
                slDocument.SetCellValueNumeric(iRow, 6, data.Stick.ToString());

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 6);
            slDocument.SetCellStyle(3, 1, iRow - 1, 6, valueStyle);

            //add row for total
            slDocument.SetCellValue(iRow, 1, "Total");
            slDocument.MergeWorksheetCells(iRow, 1, iRow, 3);
            slDocument.SetCellValueNumeric(iRow, 4, listData.Sum(x => x.TotalKm.Value).ToString());
            slDocument.SetCellValueNumeric(iRow, 5, listData.Sum(x => x.TotalCost.Value).ToString());
            slDocument.SetCellValueNumeric(iRow, 6, listData.Sum(x => x.Stick.Value).ToString());

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 6, headerStyle);

            #region --------- Chart --------------

            //select distinct year
            var yearList = listData.OrderBy(x => x.ReportYear).Select(x => x.ReportYear).Distinct();
            var startColumYear = 8;
            var startRowYear = 3;
            var endRowYear = 3;

            foreach (var year in yearList)
            {
                slDocument.SetCellValue(endRowYear, startColumYear, year.ToString());
                endRowYear++;
            }

            slDocument.SetCellValue(endRowYear, startColumYear, "Total");

            //select distinct Region
            var functionList = listData.OrderBy(x => x.Region).Select(x => x.Region).Distinct();
            var startColum = 9;
            var endColum = 9;
            var startRow = 2;
            var startRowCount = 3;

            foreach (var item in functionList)
            {
                slDocument.SetCellValue(startRow, endColum, string.IsNullOrEmpty(item) ? "No Region" : item);
                startRowCount = 3;

                foreach (var year in yearList)
                {
                    var vehicleCountYear = listData.Where(x => x.Region == item && x.ReportYear == year).Sum(x => x.TotalCost);

                    slDocument.SetCellValueNumeric(startRowCount, endColum, vehicleCountYear.ToString());
                    startRowCount++;
                }

                var vehicleCount = listData.Where(x => x.Region == item).Sum(x => x.TotalCost);
                slDocument.SetCellValueNumeric(startRowCount, endColum, vehicleCount.ToString());

                endColum++;
            }

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(startRowYear, startColumYear, endRowYear, startColumYear, headerStyleChart);
            slDocument.SetCellStyle(startRow, startColum, startRow, endColum - 1, headerStyleChart);
            slDocument.SetCellStyle(startRowCount, startColum, startRowCount, endColum - 1, headerStyleNumbChart);

            SLChart chart = slDocument.CreateChart(startRowYear - 1, startColumYear, endRowYear, endColum - 1);
            chart.SetChartStyle(SLChartStyle.Style46);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(endRowYear + 2, 8, endRowYear + 24, 17);
            chart.PlotDataSeriesAsPrimaryLineChart(3, SLChartDataDisplayType.Normal, true);
            chart.PlotDataSeriesAsSecondaryLineChart(4, SLChartDataDisplayType.Normal, false);
            chart.PlotDataSeriesAsSecondaryLineChart(2, SLChartDataDisplayType.Normal, true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        #endregion

        #region --------- Export New --------------
        public string ExportSalesByRegionNew(SalesByRegionModel model = null)
        {
            string pathFile = "";

            var input = Mapper.Map<SalesRegionGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsSalesByRegionNew(input);


            //pathFile = createChart();
            return pathFile;

        }

        private string CreateXlsSalesByRegionNew(SalesRegionGetByParamInput input)
        {
            //get data
            List<SalesByRegionDto> data = _execSummBLL.GetSalesByRegionData(input);
            var listData = Mapper.Map<List<SalesByRegionData>>(data);

            var slDocument = new SLDocument();

            //create data
            slDocument = CreateDataExcelDashboardSalesByRegionNew(slDocument, listData, input);

            var fileName = "ExecSum_SalesByRegion" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateDataExcelDashboardSalesByRegionNew(SLDocument slDocument, List<SalesByRegionData> listData, SalesRegionGetByParamInput input)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var nextColumn = 3;
            var endColumn = 3;
            var minColumn = 10;

            //select distinct data
            var dataList = listData.OrderBy(x => x.Region).Select(x => x.Region).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY REGIONAL");

            nextColumn = firstColumn + 1;
            endColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "Per Stick " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "Per KM " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, (string.IsNullOrEmpty(item) || item == "0") ? "HO" : item);

                nextColumn = firstColumn + 1;
                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => x.Region == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.TotalCost / x.Stick);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => x.Region == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.TotalCost / x.TotalKm);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");

            nextColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.TotalCost / x.Stick);

                slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.TotalCost / x.TotalKm);

                slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle rightStyleChart = slDocument.CreateStyle();
            rightStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Right;

            SLStyle numberStyle = slDocument.CreateStyle();
            numberStyle.FormatCode = "#,##0.00";

            if (minColumn < endColumn)
            {
                minColumn = endColumn;
            }
            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, minColumn, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, minColumn, headerStyleNumbChart);
            slDocument.SetCellStyle(contRow, firstColumn + 1, contRow, minColumn, rightStyleChart);
            slDocument.SetCellStyle(firstRow + 1, firstColumn + 1, contRow, minColumn, numberStyle);
            slDocument.AutoFitColumn(firstColumn, endColumn - 1);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, endColumn - 1);
            chart.SetChartStyle(SLChartStyle.Style32);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, minColumn);
            chart.Title.SetTitle("Sales By Region");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            //title
            slDocument.SetCellValue(1, 2, "Sales By Region");
            slDocument.MergeWorksheetCells(1, 2, 1, minColumn);

            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 14;
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Aqua, System.Drawing.Color.Aqua);
            slDocument.SetCellStyle(1, 2, 1, minColumn, valueStyle);
            
            #endregion

            return slDocument;
        }

        #endregion

        #endregion

        #region --------- Accident --------------

        public ActionResult Accident(int? month, int? year)
        {
            var model = new AccidentModel();
            model.SearchView.YearFrom = DateTime.Now.Year;
            model.SearchView.YearTo = DateTime.Now.Year;

            if (month.HasValue)
            {
                model.SearchView.MonthFrom = month.Value;
                model.SearchView.MonthTo = month.Value;
            }
            if (year.HasValue)
            {
                model.SearchView.YearFrom = year.Value;
                model.SearchView.YearTo = year.Value;
            }
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
            var LocationMappingList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).ToList();
            if (CurrentUser.UserRole == Enums.UserRole.FinanceZone || CurrentUser.UserRole == Enums.UserRole.Logistic)
            {
                var GetCity = EmployeeList.Where(x => x.EMPLOYEE_ID == CurrentUser.EMPLOYEE_ID && x.IS_ACTIVE).FirstOrDefault();
                if (GetCity != null)
                {
                    var GetRegion = LocationMappingList.Where(x => (x.Location == null ? "" : x.Location.ToUpper()) == (GetCity.CITY == null ? "" : GetCity.CITY.ToUpper())).FirstOrDefault();
                    if (GetRegion != null)
                    {
                        model.SearchView.ZoneId = GetRegion.ZoneSales;
                        model.SearchView.Zone = GetRegion.ZoneSales;
                    }
                }
            }
            var input = Mapper.Map<AccidentGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetAccidentData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();
            var listVehType = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.IsActive).Select(x => new { x.SettingValue }).ToList();
            var listZone = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.ZoneSales }).Distinct().ToList();

            model.TitleForm = "Accident";
            model.TitleExport = "ExportAccidentNew";
            model.AccidentDataList = Mapper.Map<List<AccidentData>>(data);
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
            model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
            model.SearchView.Functions = GetFunctionsMultiSelectList();
            model.SearchView.ZoneList = new SelectList(listZone, "ZoneSales", "ZoneSales");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View(model);
        }

        [HttpPost]
        public PartialViewResult FilterAccident(AccidentModel model)
        {
            model.AccidentDataList = GetAccidentData(model.SearchView);
            return PartialView("_ListAccident", model);
        }

        private List<AccidentData> GetAccidentData(AccidentSearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _execSummBLL.GetAccidentData(new AccidentGetByParamInput()).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
                return Mapper.Map<List<AccidentData>>(data);
            }

            //getbyparams
            filter.Function = filter.FunctionId;
            filter.ZoneId = filter.Zone;
            var input = Mapper.Map<AccidentGetByParamInput>(filter);

            var dbData = _execSummBLL.GetAccidentData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            return Mapper.Map<List<AccidentData>>(dbData);
        }

        #region --------- Export --------------
        public string ExportAccident(AccidentModel model = null)
        {
            string pathFile = "";

            var input = Mapper.Map<AccidentGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsAccident(input);
            
            //pathFile = createChart();
            return pathFile;

        }
    
        private string CreateXlsAccident(AccidentGetByParamInput input)
        {
            //get data
            List<AccidentDto> data = _execSummBLL.GetAccidentData(input);
            var listData = Mapper.Map<List<AccidentData>>(data);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Accident");
            slDocument.MergeWorksheetCells(1, 1, 1, 6);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelDashboardAccident(slDocument);

            //create data
            slDocument = CreateDataExcelDashboardAccident(slDocument, listData);

            var fileName = "ExecSum_Accident" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelDashboardAccident(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Vehicle Type");
            slDocument.SetCellValue(iRow, 2, "Regional");
            slDocument.SetCellValue(iRow, 3, "Function");
            slDocument.SetCellValue(iRow, 4, "Month");
            slDocument.SetCellValue(iRow, 5, "Year");
            slDocument.SetCellValue(iRow, 6, "Accident Count");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 6, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelDashboardAccident(SLDocument slDocument, List<AccidentData> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData.OrderBy(x => x.ReportMonth).OrderBy(x => x.ReportYear))
            {
                slDocument.SetCellValue(iRow, 1, data.VehicleType);
                slDocument.SetCellValue(iRow, 2, data.Region);
                slDocument.SetCellValue(iRow, 3, data.Function);
                slDocument.SetCellValue(iRow, 4, data.Month);
                slDocument.SetCellValue(iRow, 5, data.ReportYear.ToString());
                slDocument.SetCellValueNumeric(iRow, 6, data.AccidentCount.ToString());

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 6);
            slDocument.SetCellStyle(3, 1, iRow - 1, 6, valueStyle);

            //add row for total
            slDocument.SetCellValue(iRow, 1, "Total");
            slDocument.MergeWorksheetCells(iRow, 1, iRow, 5);
            slDocument.SetCellValueNumeric(iRow, 6, listData.Sum(x => x.AccidentCount.Value).ToString());

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 6, headerStyle);

            #region --------- Chart --------------

            //select distinct year
            var yearList = listData.OrderBy(x => x.ReportYear).Select(x => x.ReportYear).Distinct();
            var startColumYear = 8;
            var startRowYear = 3;
            var endRowYear = 3;

            foreach (var year in yearList)
            {
                slDocument.SetCellValue(endRowYear, startColumYear, year.ToString());
                endRowYear++;
            }

            slDocument.SetCellValue(endRowYear, startColumYear, "Total");

            //select distinct Function
            var functionList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct();
            var startColum = 9;
            var endColum = 9;
            var startRow = 2;
            var startRowCount = 3;

            foreach (var item in functionList)
            {
                slDocument.SetCellValue(startRow, endColum, string.IsNullOrEmpty(item) ? "No Function" : item);
                startRowCount = 3;

                foreach (var year in yearList)
                {
                    var vehicleCountYear = listData.Where(x => x.Function == item && x.ReportYear == year).Sum(x => x.AccidentCount);

                    slDocument.SetCellValueNumeric(startRowCount, endColum, vehicleCountYear.ToString());
                    startRowCount++;
                }

                var vehicleCount = listData.Where(x => x.Function == item).Sum(x => x.AccidentCount);
                slDocument.SetCellValueNumeric(startRowCount, endColum, vehicleCount.ToString());

                endColum++;
            }

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(startRowYear, startColumYear, endRowYear, startColumYear, headerStyleChart);
            slDocument.SetCellStyle(startRow, startColum, startRow, endColum - 1, headerStyleChart);
            slDocument.SetCellStyle(startRowCount, startColum, startRowCount, endColum - 1, headerStyleNumbChart);

            SLChart chart = slDocument.CreateChart(startRowYear - 1, startColumYear, endRowYear, endColum - 1);
            chart.SetChartStyle(SLChartStyle.Style46);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(endRowYear + 2, 8, endRowYear + 24, 17);
            chart.PlotDataSeriesAsPrimaryLineChart(3, SLChartDataDisplayType.Normal, true);
            chart.PlotDataSeriesAsSecondaryLineChart(4, SLChartDataDisplayType.Normal, false);
            chart.PlotDataSeriesAsSecondaryLineChart(2, SLChartDataDisplayType.Normal, true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        #endregion

        #region --------- Export New --------------
        public string ExportAccidentNew(AccidentModel model = null)
        {
            string pathFile = "";

            var input = Mapper.Map<AccidentGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsAccidentNew(input);

            //pathFile = createChart();
            return pathFile;

        }

        private string CreateXlsAccidentNew(AccidentGetByParamInput input)
        {
            //get data
            List<AccidentDto> data = _execSummBLL.GetAccidentData(input);
            var listData = Mapper.Map<List<AccidentData>>(data);

            var slDocument = new SLDocument();

            //create data
            slDocument = CreateDataExcelDashboardAccidentNew(slDocument, listData, input);

            var fileName = "ExecSum_Accident" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateDataExcelDashboardAccidentNew(SLDocument slDocument, List<AccidentData> listData, AccidentGetByParamInput input)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var nextColumn = 3;
            var endColumn = 3;
            var minColumn = 10;

            //select distinct data
            var dataList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY FUNCTION");

            nextColumn = firstColumn + 1;
            endColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "BENEFIT " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "WTC " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "No Function" : item);

                nextColumn = firstColumn + 1;
                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT"
                                                        && x.Function == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.AccidentCount);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC"
                                                        && x.Function == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.AccidentCount);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");

            nextColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT"
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.AccidentCount);

                slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC"
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.AccidentCount);

                slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle rightStyleChart = slDocument.CreateStyle();
            rightStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Right;

            slDocument.AutoFitColumn(firstColumn, endColumn - 1);

            if (minColumn < endColumn)
            {
                minColumn = endColumn;
            }
            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, minColumn, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, minColumn, headerStyleNumbChart);
            slDocument.SetCellStyle(contRow, firstColumn + 1, contRow, minColumn, rightStyleChart);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, endColumn - 1);
            chart.SetChartStyle(SLChartStyle.Style30);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, minColumn);
            chart.Title.SetTitle("Accident");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            //title
            slDocument.SetCellValue(1, 2, "Accident");
            slDocument.MergeWorksheetCells(1, 2, 1, 10);

            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 14;
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Aqua, System.Drawing.Color.Aqua);
            slDocument.SetCellStyle(1, 2, 1, 10, valueStyle);
            
            #endregion

            return slDocument;
        }

        #endregion

        #endregion

        #region --------- AC Vs OB --------------

        public ActionResult AcVsOb(int? month, int? year)
        {
            var model = new AcVsObModel();
            model.SearchView.YearFrom = DateTime.Now.Year;
            model.SearchView.YearTo = DateTime.Now.Year;

            if (month.HasValue)
            {
                model.SearchView.MonthFrom = month.Value;
                model.SearchView.MonthTo = month.Value;
            }
            if (year.HasValue)
            {
                model.SearchView.YearFrom = year.Value;
                model.SearchView.YearTo = year.Value;
            }

            var input = Mapper.Map<AcVsObGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetAcVsObData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            var settingData = _settingBLL.GetSetting();
            var listVehType = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.IsActive).Select(x => new { x.SettingValue }).ToList();

            model.TitleForm = "Actual Cost Vs Budget";
            model.TitleExport = "ExportAcVsObNew";
            model.AcVsObDataList = Mapper.Map<List<AcVsObData>>(data);
            model.SearchView.Functions = GetFunctionsMultiSelectList();
            model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View(model);
        }

        [HttpPost]
        public PartialViewResult FilterAcVsOb(AcVsObModel model)
        {
            model.AcVsObDataList = GetAcVsObData(model.SearchView);
            return PartialView("_ListAcVsOb", model);
        }

        private List<AcVsObData> GetAcVsObData(AcVsObSearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _execSummBLL.GetAcVsObData(new AcVsObGetByParamInput()).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
                return Mapper.Map<List<AcVsObData>>(data);
            }

            //getbyparams
            filter.Function = filter.FunctionId;
            var input = Mapper.Map<AcVsObGetByParamInput>(filter);

            var dbData = _execSummBLL.GetAcVsObData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            return Mapper.Map<List<AcVsObData>>(dbData);
        }

        #region --------- Export --------------
        public string ExportAcVsOb(AcVsObModel model = null)
        {
            string pathFile = "";

            var input = Mapper.Map<AcVsObGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsLeaseCostByFunction(input);

            //pathFile = createChart();
            return pathFile;

        }
      
        private string CreateXlsLeaseCostByFunction(AcVsObGetByParamInput input)
        {
            //get data
            List<AcVsObDto> data = _execSummBLL.GetAcVsObData(input);
            var listData = Mapper.Map<List<AcVsObData>>(data);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "AC Vs OB");
            slDocument.MergeWorksheetCells(1, 1, 1, 5);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelDashboardAcVsOb(slDocument);

            //create data
            slDocument = CreateDataExcelDashboardAcVsOb(slDocument, listData);

            var fileName = "ExecSum_AcVsOb" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelDashboardAcVsOb(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Function");
            slDocument.SetCellValue(iRow, 2, "Month");
            slDocument.SetCellValue(iRow, 3, "Year");
            slDocument.SetCellValue(iRow, 4, "Cost OB");
            slDocument.SetCellValue(iRow, 5, "Actual Cost");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 5, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelDashboardAcVsOb(SLDocument slDocument, List<AcVsObData> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData.OrderBy(x => x.ReportMonth).OrderBy(x => x.ReportYear))
            {
                slDocument.SetCellValue(iRow, 1, data.Function);
                slDocument.SetCellValue(iRow, 2, data.Month);
                slDocument.SetCellValue(iRow, 3, data.ReportYear.ToString());
                slDocument.SetCellValueNumeric(iRow, 4, data.CostOb.ToString());
                slDocument.SetCellValueNumeric(iRow, 5, data.ActualCost.ToString());

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 5);
            slDocument.SetCellStyle(3, 1, iRow - 1, 5, valueStyle);

            //add row for total
            slDocument.SetCellValue(iRow, 1, "Total");
            slDocument.MergeWorksheetCells(iRow, 1, iRow, 3);
            slDocument.SetCellValueNumeric(iRow, 4, listData.Sum(x => x.CostOb.Value).ToString());
            slDocument.SetCellValueNumeric(iRow, 5, listData.Sum(x => x.ActualCost == null ? 0 : x.ActualCost.Value).ToString());

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 5, headerStyle);

            #region --------- Chart --------------

            //select distinct year
            var yearList = listData.OrderBy(x => x.ReportYear).Select(x => x.ReportYear).Distinct();
            var startColumYear = 7;
            var startRowYear = 3;
            var endRowYear = 3;

            foreach (var year in yearList)
            {
                slDocument.SetCellValue(endRowYear, startColumYear, year.ToString());
                endRowYear++;
            }

            slDocument.SetCellValue(endRowYear, startColumYear, "Total");

            //select distinct Function
            var functionList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct();
            var startColum = 8;
            var endColum = 8;
            var startRow = 2;
            var startRowCount = 3;

            foreach (var item in functionList)
            {
                slDocument.SetCellValue(startRow, endColum, string.IsNullOrEmpty(item) ? "No Function" : item);
                startRowCount = 3;

                foreach (var year in yearList)
                {
                    var vehicleCountYear = listData.Where(x => x.Function == item && x.ReportYear == year).Sum(x => x.CostOb);

                    slDocument.SetCellValueNumeric(startRowCount, endColum, vehicleCountYear.ToString());
                    startRowCount++;
                }

                var vehicleCount = listData.Where(x => x.Function == item).Sum(x => x.CostOb);
                slDocument.SetCellValueNumeric(startRowCount, endColum, vehicleCount.ToString());

                endColum++;
            }

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(startRowYear, startColumYear, endRowYear, startColumYear, headerStyleChart);
            slDocument.SetCellStyle(startRow, startColum, startRow, endColum - 1, headerStyleChart);
            slDocument.SetCellStyle(startRowCount, startColum, startRowCount, endColum - 1, headerStyleNumbChart);

            SLChart chart = slDocument.CreateChart(startRowYear - 1, startColumYear, endRowYear, endColum - 1);
            chart.SetChartStyle(SLChartStyle.Style46);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(endRowYear + 2, 7, endRowYear + 24, 16);
            chart.PlotDataSeriesAsPrimaryLineChart(3, SLChartDataDisplayType.Normal, true);
            chart.PlotDataSeriesAsSecondaryLineChart(4, SLChartDataDisplayType.Normal, false);
            chart.PlotDataSeriesAsSecondaryLineChart(2, SLChartDataDisplayType.Normal, true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        #endregion

        #region --------- Export New --------------
        public string ExportAcVsObNew(AcVsObModel model = null)
        {
            string pathFile = "";

            var input = Mapper.Map<AcVsObGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsAcVsObNew(input);

            //pathFile = createChart();
            return pathFile;

        }

        private string CreateXlsAcVsObNew(AcVsObGetByParamInput input)
        {
            //get data
            List<AcVsObDto> data = _execSummBLL.GetAcVsObData(input);
            var listData = Mapper.Map<List<AcVsObData>>(data);

            List<AcVsObDto> dataUnit = _execSummBLL.GetAcVsObUnitData(input);
            var listDataUnit = Mapper.Map<List<AcVsObData>>(dataUnit);

            var slDocument = new SLDocument();

            //create data
            slDocument.RenameWorksheet("Sheet1", "Actual Cost Vs Budget");
            slDocument = CreateDataExcelDashboardAcVsObNew(slDocument, listData, input);

            //title no of vehicle Make
            slDocument.AddWorksheet("Actual Unit Vs Budget");
            slDocument = CreateDataExcelDashboardAcVsObUnitNew(slDocument, listDataUnit, input);

            var fileName = "ExecSum_AcVsOb" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateDataExcelDashboardAcVsObNew(SLDocument slDocument, List<AcVsObData> listData, AcVsObGetByParamInput input)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var nextColumn = 3;
            var endColumn = 3;
            var minColumn = 10;

            //select distinct data
            var dataList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY FUNCTION");

            nextColumn = firstColumn + 1;
            endColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "Actual BENEFIT " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "Budget BENEFIT " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "Actual WTC " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "Budget WTC " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "No Function" : item);

                nextColumn = firstColumn + 1;
                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT"
                                                        && x.Function == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.ActualCost);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT"
                                                        && x.Function == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.CostOb);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC"
                                                        && x.Function == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.ActualCost);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC"
                                                        && x.Function == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.CostOb);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");

            nextColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT"
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.ActualCost);

                slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT"
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.CostOb);

                slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC"
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.ActualCost);

                slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC"
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.CostOb);

                slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle rightStyleChart = slDocument.CreateStyle();
            rightStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Right;

            SLStyle numberStyle = slDocument.CreateStyle();
            numberStyle.FormatCode = "#,##0.00";

            if (minColumn < endColumn)
            {
                minColumn = endColumn;
            }
            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, minColumn, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, minColumn, headerStyleNumbChart);
            slDocument.SetCellStyle(contRow, firstColumn + 1, contRow, minColumn, rightStyleChart);
            slDocument.SetCellStyle(firstRow + 1, firstColumn + 1, contRow, minColumn, numberStyle);
            slDocument.AutoFitColumn(firstColumn, endColumn - 1);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, endColumn - 1);
            chart.SetChartStyle(SLChartStyle.Style31);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, minColumn);
            chart.Title.SetTitle("Actual Cost Vs Budget");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            //title
            slDocument.SetCellValue(1, 2, "Actual Cost Vs Budget");
            slDocument.MergeWorksheetCells(1, 2, 1, minColumn);

            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 14;
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Aqua, System.Drawing.Color.Aqua);
            slDocument.SetCellStyle(1, 2, 1, minColumn, valueStyle);
            
            #endregion

            return slDocument;
        }

        private SLDocument CreateDataExcelDashboardAcVsObUnitNew(SLDocument slDocument, List<AcVsObData> listData, AcVsObGetByParamInput input)
        {
            #region --------- Chart --------------

            var firstRow = 14;
            var contRow = 15;
            var firstColumn = 2;
            var nextColumn = 3;
            var endColumn = 3;
            var minColumn = 10;

            //select distinct data
            var dataList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct();

            slDocument.SetCellValue(firstRow, firstColumn, "BY FUNCTION");

            nextColumn = firstColumn + 1;
            endColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "Unit " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                slDocument.SetCellValue(firstRow, nextColumn, "Unit Budget " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) + " " + input.YearFrom);
                nextColumn++;
                endColumn++;
            }

            foreach (var item in dataList)
            {
                slDocument.SetCellValue(contRow, firstColumn, string.IsNullOrEmpty(item) ? "No Function" : item);

                nextColumn = firstColumn + 1;
                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => x.Function == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.Unit);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                for (int i = input.MonthFrom; i <= input.MonthTo; i++)
                {
                    var countData = listData.Where(x => x.Function == item
                                                        && x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.UnitBudget);

                    slDocument.SetCellValueNumeric(contRow, nextColumn, countData.ToString());
                    nextColumn++;
                }

                contRow++;
            }

            slDocument.SetCellValue(contRow, firstColumn, "Total");

            nextColumn = firstColumn + 1;
            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.Unit);

                slDocument.SetCellValue(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            for (int i = input.MonthFrom; i <= input.MonthTo; i++)
            {
                var countData = listData.Where(x => x.ReportMonth == i && x.ReportYear == input.YearFrom).Sum(x => x.UnitBudget);

                slDocument.SetCellValue(contRow, nextColumn, countData.ToString());
                nextColumn++;
            }

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.GreenYellow, System.Drawing.Color.GreenYellow);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle rightStyleChart = slDocument.CreateStyle();
            rightStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Right;

            if (minColumn < endColumn)
            {
                minColumn = endColumn;
            }
            slDocument.SetCellStyle(firstRow, firstColumn, firstRow, minColumn, headerStyleChart);
            slDocument.SetCellStyle(contRow, firstColumn, contRow, minColumn, headerStyleNumbChart);
            slDocument.SetCellStyle(contRow, firstColumn + 1, contRow, minColumn, rightStyleChart);
            slDocument.AutoFitColumn(firstColumn, endColumn - 1);

            SLChart chart = slDocument.CreateChart(firstRow, firstColumn, contRow, endColumn - 1);
            chart.SetChartStyle(SLChartStyle.Style31);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(2, 1, firstRow - 2, minColumn);
            chart.Title.SetTitle("Actual Unit Vs Budget");
            chart.ShowChartTitle(true);

            slDocument.InsertChart(chart);

            //title
            slDocument.SetCellValue(1, 2, "Actual Unit Vs Budget");
            slDocument.MergeWorksheetCells(1, 2, 1, minColumn);

            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 14;
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Aqua, System.Drawing.Color.Aqua);
            slDocument.SetCellStyle(1, 2, 1, minColumn, valueStyle);

            #endregion

            return slDocument;
        }

        #endregion

        #endregion

        #region --------- Sum PTD By Function --------------

        public ActionResult SumPtdByFunction()
        {
            var model = new SumPtdByFunctionModel();
            model.SearchView.YearFrom = DateTime.Now.Year;
            model.SearchView.YearTo = DateTime.Now.Year;

            var input = Mapper.Map<SumPtdFuncGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetSumPtdByFunctionData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();
            var listVehType = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.IsActive).Select(x => new { x.SettingValue }).ToList();

            model.TitleForm = "Sum PTD By Function";
            model.TitleExport = "ExportSumPtdByFunction";
            model.SumPtdByFuncDataList = Mapper.Map<List<SumPtdByFunctionData>>(data);
            model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
            model.SearchView.Functions = GetFunctionsMultiSelectList();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        [HttpPost]
        public PartialViewResult FilterSumPtdByFunction(SumPtdByFunctionModel model)
        {
            model.SumPtdByFuncDataList = GetSumPtdByFunctionData(model.SearchView);
            return PartialView("_ListSumPtdByFunction", model);
        }

        private List<SumPtdByFunctionData> GetSumPtdByFunctionData(SumPtdByFuncSearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _execSummBLL.GetSumPtdByFunctionData(new SumPtdFuncGetByParamInput()).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
                return Mapper.Map<List<SumPtdByFunctionData>>(data);
            }

            //getbyparams
            filter.Function = filter.FunctionId;
            var input = Mapper.Map<SumPtdFuncGetByParamInput>(filter);

            var dbData = _execSummBLL.GetSumPtdByFunctionData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            return Mapper.Map<List<SumPtdByFunctionData>>(dbData);
        }

        #region --------- Export --------------
        public string ExportSumPtdByFunction(SumPtdByFunctionModel model = null)
        {
            string pathFile = "";

            var input = Mapper.Map<SumPtdFuncGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsSumPtdByFunction(input);

            //pathFile = createChart();
            return pathFile;

        }
  
        private string CreateXlsSumPtdByFunction(SumPtdFuncGetByParamInput input)
        {
            //get data
            List<SumPtdByFunctionDto> data = _execSummBLL.GetSumPtdByFunctionData(input);
            var listData = Mapper.Map<List<SumPtdByFunctionData>>(data);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Sum PTD By Function");
            slDocument.MergeWorksheetCells(1, 1, 1, 12);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelDashboardSumPtdByFunction(slDocument);

            //create data
            slDocument = CreateDataExcelDashboardSumPtdByFunction(slDocument, listData);

            var fileName = "ExecSum_SumPtdByFunction" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelDashboardSumPtdByFunction(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Vehicle Type");
            slDocument.SetCellValue(iRow, 2, "Regional");
            slDocument.SetCellValue(iRow, 3, "Function");
            slDocument.SetCellValue(iRow, 4, "Month");
            slDocument.SetCellValue(iRow, 5, "Year");
            slDocument.SetCellValue(iRow, 6, "Total Vehicle");
            slDocument.SetCellValue(iRow, 7, "Total Vehicle Cost");
            slDocument.SetCellValue(iRow, 8, "Total Fuel Amount");
            slDocument.SetCellValue(iRow, 9, "Total Fuel Cost");
            slDocument.SetCellValue(iRow, 10, "Total KM");
            slDocument.SetCellValue(iRow, 11, "Total Operational Cost");
            slDocument.SetCellValue(iRow, 12, "Accident Count");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 12, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelDashboardSumPtdByFunction(SLDocument slDocument, List<SumPtdByFunctionData> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData.OrderBy(x => x.ReportMonth).OrderBy(x => x.ReportYear))
            {
                slDocument.SetCellValue(iRow, 1, data.VehicleType);
                slDocument.SetCellValue(iRow, 2, data.Region);
                slDocument.SetCellValue(iRow, 3, data.Function);
                slDocument.SetCellValue(iRow, 4, data.Month);
                slDocument.SetCellValue(iRow, 5, data.ReportYear.ToString());
                slDocument.SetCellValueNumeric(iRow, 6, data.TotalVehicle.ToString());
                slDocument.SetCellValueNumeric(iRow, 7, data.TotalVehicleCost.ToString());
                slDocument.SetCellValueNumeric(iRow, 8, data.TotalFuelAmount.ToString());
                slDocument.SetCellValueNumeric(iRow, 9, data.TotalFuelCost.ToString());
                slDocument.SetCellValueNumeric(iRow, 10, data.TotalKm.ToString());
                slDocument.SetCellValueNumeric(iRow, 11, data.TotalOperationalCost.ToString());
                slDocument.SetCellValueNumeric(iRow, 12, data.AccidentCount.ToString());

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 12);
            slDocument.SetCellStyle(3, 1, iRow - 1, 12, valueStyle);

            //add row for total
            slDocument.SetCellValue(iRow, 1, "Total");
            slDocument.MergeWorksheetCells(iRow, 1, iRow, 5);
            slDocument.SetCellValueNumeric(iRow, 6, listData.Sum(x => x.TotalVehicle.Value).ToString());
            slDocument.SetCellValueNumeric(iRow, 7, listData.Sum(x => x.TotalVehicleCost.Value).ToString());
            slDocument.SetCellValueNumeric(iRow, 8, listData.Sum(x => x.TotalFuelAmount.Value).ToString());
            slDocument.SetCellValueNumeric(iRow, 9, listData.Sum(x => x.TotalFuelCost.Value).ToString());
            slDocument.SetCellValueNumeric(iRow, 10, listData.Sum(x => x.TotalKm.Value).ToString());
            slDocument.SetCellValueNumeric(iRow, 11, listData.Sum(x => x.TotalOperationalCost.Value).ToString());
            slDocument.SetCellValueNumeric(iRow, 12, listData.Sum(x => x.AccidentCount.Value).ToString());

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 12, headerStyle);

            #region --------- Chart --------------

            //select distinct year
            var yearList = listData.OrderBy(x => x.ReportYear).Select(x => x.ReportYear).Distinct();
            var startColumYear = 14;
            var startRowYear = 3;
            var endRowYear = 3;

            foreach (var year in yearList)
            {
                slDocument.SetCellValue(endRowYear, startColumYear, year.ToString());
                endRowYear++;
            }

            slDocument.SetCellValue(endRowYear, startColumYear, "Total");

            //select distinct Function
            var functionList = listData.OrderBy(x => x.Function).Select(x => x.Function).Distinct();
            var startColum = 15;
            var endColum = 15;
            var startRow = 2;
            var startRowCount = 3;

            foreach (var item in functionList)
            {
                slDocument.SetCellValue(startRow, endColum, string.IsNullOrEmpty(item) ? "No Function" : item);
                startRowCount = 3;

                foreach (var year in yearList)
                {
                    var vehicleCountYear = listData.Where(x => x.Function == item && x.ReportYear == year).Sum(x => x.TotalVehicle);

                    slDocument.SetCellValueNumeric(startRowCount, endColum, vehicleCountYear.ToString());
                    startRowCount++;
                }

                var vehicleCount = listData.Where(x => x.Function == item).Sum(x => x.TotalVehicle);
                slDocument.SetCellValueNumeric(startRowCount, endColum, vehicleCount.ToString());

                endColum++;
            }

            SLStyle headerStyleChart = slDocument.CreateStyle();
            headerStyleChart.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyleChart.Font.Bold = true;
            headerStyleChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            SLStyle headerStyleNumbChart = slDocument.CreateStyle();
            headerStyleNumbChart.Font.Bold = true;
            headerStyleNumbChart.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyleNumbChart.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(startRowYear, startColumYear, endRowYear, startColumYear, headerStyleChart);
            slDocument.SetCellStyle(startRow, startColum, startRow, endColum - 1, headerStyleChart);
            slDocument.SetCellStyle(startRowCount, startColum, startRowCount, endColum - 1, headerStyleNumbChart);

            SLChart chart = slDocument.CreateChart(startRowYear - 1, startColumYear, endRowYear, endColum - 1);
            chart.SetChartStyle(SLChartStyle.Style46);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(endRowYear + 2, 14, endRowYear + 24, 23);
            chart.PlotDataSeriesAsPrimaryLineChart(3, SLChartDataDisplayType.Normal, true);
            chart.PlotDataSeriesAsSecondaryLineChart(4, SLChartDataDisplayType.Normal, false);
            chart.PlotDataSeriesAsSecondaryLineChart(2, SLChartDataDisplayType.Normal, true);

            slDocument.InsertChart(chart);

            #endregion

            return slDocument;
        }

        #endregion

        #endregion
    }
}
