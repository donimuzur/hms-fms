using System;
using System.Collections.Generic;
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

        public RptExecutiveSummaryController(IPageBLL pageBll, IExecutiveSummaryBLL execSummBLL, ISettingBLL settingBLL, ILocationMappingBLL locationMappingBLL,
            IGroupCostCenterBLL groupCostCenterBLL)
            : base(pageBll, Core.Enums.MenuList.RptExecutiveSummary)
        {
            _pageBLL = pageBll;
            _execSummBLL = execSummBLL;
            _settingBLL = settingBLL;
            _locationMappingBLL = locationMappingBLL;
            _groupCostCenterBLL = groupCostCenterBLL;
            _mainMenu = Enums.MenuList.RptExecutiveSummary;
        }

        #endregion

        #region --------- Set Data -----------------------

        public MultiSelectList GetFunctionsMultiSelectList()
        {
            var items = new List<SelectListItem>()
            {
                new SelectListItem() {Text = "SALES", Value = "Sales" },
                new SelectListItem() {Text = "MARKETING", Value = "Marketing" },
                new SelectListItem() {Text = "OPERATIONS", Value = "Operations" },
                new SelectListItem() {Text = "OTHERS", Value = "Others" }
            };

            return new MultiSelectList(items, "Value", "Text");
        }

        #endregion

        #region --------- Get Data Json -----------------------

        [HttpPost]
        public JsonResult GetVehicleDataVisual(int monthFrom, int? yearFrom, int monthTo, int? yearTo, bool isByRegion)
        {
            var input = new VehicleGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthTo;
            input.YearTo = yearTo == null ? 0 : yearTo.Value;
            if (isByRegion)
            {
                input.Function = "Sales,Marketing";
            }

            List<NoVehicleDto> data = _execSummBLL.GetNoOfVehicleData(input);

            var groupData = data.GroupBy(x => new { x.VEHICLE_TYPE })
                .Select(p => new NoVehicleDto()
                {
                    VEHICLE_TYPE = p.FirstOrDefault().VEHICLE_TYPE,
                    NO_OF_VEHICLE = p.Sum(c => c.NO_OF_VEHICLE)
                }).ToList();

            if (isByRegion)
            {
                groupData = data.GroupBy(x => new { x.FUNCTION })
                .Select(p => new NoVehicleDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    NO_OF_VEHICLE = p.Sum(c => c.NO_OF_VEHICLE)
                }).ToList();
            }

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult GetVehicleWtcDataVisual(int monthFrom, int? yearFrom, int monthTo, int? yearTo, bool isByRegion)
        {
            var input = new VehicleWtcGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthTo;
            input.YearTo = yearTo == null ? 0 : yearTo.Value;
            if (isByRegion)
            {
                input.Function = "Sales,Marketing";
            }

            List<NoVehicleWtcDto> data = _execSummBLL.GetNoOfVehicleWtcData(input);

            var groupData = data.GroupBy(x => new { x.FUNCTION })
                .Select(p => new NoVehicleWtcDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    NO_OF_VEHICLE = p.Sum(c => c.NO_OF_VEHICLE)
                }).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult GetVehicleMakeDataVisual(int monthFrom, int? yearFrom, int monthTo, int? yearTo, bool isByRegion)
        {
            var input = new VehicleMakeGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthTo;
            input.YearTo = yearTo == null ? 0 : yearTo.Value;

            List<NoVehicleMakeDto> data = _execSummBLL.GetNoOfVehicleMakeData(input);

            var groupData = data.GroupBy(x => new { x.MANUFACTURER })
                .Select(p => new NoVehicleMakeDto()
                {
                    MANUFACTURER = p.FirstOrDefault().MANUFACTURER,
                    NO_OF_VEHICLE = p.Sum(c => c.NO_OF_VEHICLE)
                }).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult GetOdometerDataVisual(int monthFrom, int? yearFrom, int monthTo, int? yearTo, bool isByRegion)
        {
            var input = new OdometerGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthTo;
            input.YearTo = yearTo == null ? 0 : yearTo.Value;
            if (isByRegion)
            {
                input.Function = "Sales,Marketing";
            }

            List<OdometerDto> data = _execSummBLL.GetOdometerData(input);

            var groupData = data.GroupBy(x => new { x.VEHICLE_TYPE })
                .Select(p => new OdometerDto()
                {
                    VEHICLE_TYPE = p.FirstOrDefault().VEHICLE_TYPE,
                    TOTAL_KM = p.Sum(c => c.TOTAL_KM)
                }).ToList();

            if (isByRegion)
            {
                groupData = data.GroupBy(x => new { x.FUNCTION })
                .Select(p => new OdometerDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    TOTAL_KM = p.Sum(c => c.TOTAL_KM)
                }).ToList();
            }

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult GetLiterByFunctionDataVisual(int monthFrom, int? yearFrom, int monthTo, int? yearTo, bool isByRegion)
        {
            var input = new LiterFuncGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthTo;
            input.YearTo = yearTo == null ? 0 : yearTo.Value;
            if (isByRegion)
            {
                input.Function = "Sales,Marketing";
            }

            List<LiterByFunctionDto> data = _execSummBLL.GetLiterByFunctionData(input);

            var groupData = data.GroupBy(x => new { x.VEHICLE_TYPE })
                .Select(p => new LiterByFunctionDto()
                {
                    VEHICLE_TYPE = p.FirstOrDefault().VEHICLE_TYPE,
                    TOTAL_LITER = p.Sum(c => c.TOTAL_LITER)
                }).ToList();

            if (isByRegion)
            {
                groupData = data.GroupBy(x => new { x.FUNCTION })
                .Select(p => new LiterByFunctionDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    TOTAL_LITER = p.Sum(c => c.TOTAL_LITER)
                }).ToList();
            }

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult GetFuelCostByFunctionDataVisual(int monthFrom, int? yearFrom, int monthTo, int? yearTo, bool isByRegion)
        {
            var input = new FuelCostFuncGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthTo;
            input.YearTo = yearTo == null ? 0 : yearTo.Value;
            if (isByRegion)
            {
                input.Function = "Sales,Marketing";
            }

            List<FuelCostByFunctionDto> data = _execSummBLL.GetFuelCostByFunctionData(input);

            var groupData = data.GroupBy(x => new { x.VEHICLE_TYPE })
                .Select(p => new FuelCostByFunctionDto()
                {
                    VEHICLE_TYPE = p.FirstOrDefault().VEHICLE_TYPE,
                    TOTAL_FUEL_COST = p.Sum(c => c.TOTAL_FUEL_COST)
                }).ToList();

            if (isByRegion)
            {
                groupData = data.GroupBy(x => new { x.FUNCTION })
                .Select(p => new FuelCostByFunctionDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    TOTAL_FUEL_COST = p.Sum(c => c.TOTAL_FUEL_COST)
                }).ToList();
            }

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult GetLeaseCostByFunctionDataVisual(int monthFrom, int? yearFrom, int monthTo, int? yearTo, bool isByRegion)
        {
            var input = new LeaseCostFuncGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthTo;
            input.YearTo = yearTo == null ? 0 : yearTo.Value;
            if (isByRegion)
            {
                input.Function = "Sales,Marketing";
            }

            List<LeaseCostByFunctionDto> data = _execSummBLL.GetLeaseCostByFunctionData(input);

            var groupData = data.GroupBy(x => new { x.FUNCTION })
                .Select(p => new LeaseCostByFunctionDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    TOTAL_LEASE_COST = p.Sum(c => c.TOTAL_LEASE_COST)
                }).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult SalesByRegionDataVisual(int monthFrom, int? yearFrom, int monthTo, int? yearTo, bool isByRegion)
        {
            var input = new SalesRegionGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthTo;
            input.YearTo = yearTo == null ? 0 : yearTo.Value;

            List<SalesByRegionDto> data = _execSummBLL.GetSalesByRegionData(input);

            var groupData = data.GroupBy(x => new { x.REGION })
                .Select(p => new SalesByRegionDto()
                {
                    REGION = p.FirstOrDefault().REGION,
                    STICK = p.Sum(c => c.STICK)
                }).ToList();

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult AccidentDataVisual(int monthFrom, int? yearFrom, int monthTo, int? yearTo, bool isByRegion)
        {
            var input = new AccidentGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthTo;
            input.YearTo = yearTo == null ? 0 : yearTo.Value;
            if (isByRegion)
            {
                input.Function = "Sales,Marketing";
            }

            List<AccidentDto> data = _execSummBLL.GetAccidentData(input);

            var groupData = data.GroupBy(x => new { x.VEHICLE_TYPE })
                .Select(p => new AccidentDto()
                {
                    VEHICLE_TYPE = p.FirstOrDefault().VEHICLE_TYPE,
                    ACCIDENT_COUNT = p.Sum(c => c.ACCIDENT_COUNT)
                }).ToList();

            if (isByRegion)
            {
                groupData = data.GroupBy(x => new { x.FUNCTION })
                .Select(p => new AccidentDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    ACCIDENT_COUNT = p.Sum(c => c.ACCIDENT_COUNT)
                }).ToList();
            }

            return Json(groupData);
        }

        [HttpPost]
        public JsonResult AcObDataVisual(int monthFrom, int? yearFrom, int monthTo, int? yearTo, bool isByRegion)
        {
            var input = new AcVsObGetByParamInput();
            input.MonthFrom = monthFrom;
            input.YearFrom = yearFrom == null ? 0 : yearFrom.Value;
            input.MonthTo = monthTo;
            input.YearTo = yearTo == null ? 0 : yearTo.Value;
            if (isByRegion)
            {
                input.Function = "Sales,Marketing";
            }

            List<AcVsObDto> data = _execSummBLL.GetAcVsObData(input);

            var groupData = data.GroupBy(x => new { x.FUNCTION })
                .Select(p => new AcVsObDto()
                {
                    FUNCTION = p.FirstOrDefault().FUNCTION,
                    COST_OB = p.Sum(c => c.COST_OB),
                    ACTUAL_COST = p.Sum(c => c.ACTUAL_COST)
                }).ToList();

            return Json(groupData);
        }

        #endregion

        #region --------- Summary All --------------

        public ActionResult SummaryAll()
        {
            var model = new SummaryAllModel();
            model.SearchView.YearFrom = DateTime.Now.Year;
            model.SearchView.YearTo = DateTime.Now.Year;

            model.TitleForm = "Executive Summary All";
            model.TitleExport = "ExportSummaryAll";
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        #region --------- Export --------------

        public void ExportSummaryAll(SummaryAllModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsSummaryAll(model.SearchViewExport);
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

        private string CreateXlsSummaryAll(SummarySearchViewExport inputExport)
        {
            //get data
            var input = Mapper.Map<VehicleGetByParamInput>(inputExport);
            List<NoVehicleDto> data = _execSummBLL.GetNoOfVehicleData(input);
            var listData = Mapper.Map<List<NoVehicleData>>(data);

            var inputWtc = Mapper.Map<VehicleWtcGetByParamInput>(inputExport);
            List<NoVehicleWtcDto> dataWtc = _execSummBLL.GetNoOfVehicleWtcData(inputWtc);
            var listDataWtc = Mapper.Map<List<NoVehicleWtcData>>(dataWtc);

            var inputMake = Mapper.Map<VehicleMakeGetByParamInput>(inputExport);
            List<NoVehicleMakeDto> dataMake = _execSummBLL.GetNoOfVehicleMakeData(inputMake);
            var listDataMake = Mapper.Map<List<NoVehicleMakeData>>(dataMake);

            var inputOdo = Mapper.Map<OdometerGetByParamInput>(inputExport);
            List<OdometerDto> dataOdo = _execSummBLL.GetOdometerData(inputOdo);
            var listDataOdo = Mapper.Map<List<OdometerData>>(dataOdo);

            var inputLiter = Mapper.Map<LiterFuncGetByParamInput>(inputExport);
            List<LiterByFunctionDto> dataLiter = _execSummBLL.GetLiterByFunctionData(inputLiter);
            var listDataLiter = Mapper.Map<List<LiterByFunctionData>>(dataLiter);

            var inputFuel = Mapper.Map<FuelCostFuncGetByParamInput>(inputExport);
            List<FuelCostByFunctionDto> dataFuel = _execSummBLL.GetFuelCostByFunctionData(inputFuel);
            var listDataFuel = Mapper.Map<List<FuelCostByFunctionData>>(dataFuel);

            var inputLease = Mapper.Map<LeaseCostFuncGetByParamInput>(inputExport);
            List<LeaseCostByFunctionDto> dataLease = _execSummBLL.GetLeaseCostByFunctionData(inputLease);
            var listDataLease = Mapper.Map<List<LeaseCostByFunctionData>>(dataLease);

            var inputSales = Mapper.Map<SalesRegionGetByParamInput>(inputExport);
            List<SalesByRegionDto> dataSales = _execSummBLL.GetSalesByRegionData(inputSales);
            var listDataSales = Mapper.Map<List<SalesByRegionData>>(dataSales);

            var inputAccident = Mapper.Map<AccidentGetByParamInput>(inputExport);
            List<AccidentDto> dataAccident = _execSummBLL.GetAccidentData(inputAccident);
            var listDataAccident = Mapper.Map<List<AccidentData>>(dataAccident);

            var inputAcOb = Mapper.Map<AcVsObGetByParamInput>(inputExport);
            List<AcVsObDto> dataAcOb = _execSummBLL.GetAcVsObData(inputAcOb);
            var listDataAcOb = Mapper.Map<List<AcVsObData>>(dataAcOb);

            var slDocument = new SLDocument();

            //title no of vehicle
            slDocument.SetCellValue(1, 1, "Number Of Vehicle");
            slDocument.MergeWorksheetCells(1, 1, 1, 7);
            slDocument.RenameWorksheet("Sheet1", "Number Of Vehicle");
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


            //title no of vehicle wtc
            slDocument.AddWorksheet("Number Of Vehicle WTC");
            slDocument.SetCellValue(1, 1, "Number Of Vehicle WTC");
            slDocument.MergeWorksheetCells(1, 1, 1, 5);
            slDocument.SetCellStyle(1, 1, valueStyle);
            //create header
            slDocument = CreateHeaderExcelDashboardWtc(slDocument);
            //create data
            slDocument = CreateDataExcelDashboardWtc(slDocument, listDataWtc);
            

            //title no of vehicle Make
            slDocument.AddWorksheet("Number Of Vehicle Make-Type");
            slDocument.SetCellValue(1, 1, "Number Of Vehicle Make-Type");
            slDocument.MergeWorksheetCells(1, 1, 1, 5);
            slDocument.SetCellStyle(1, 1, valueStyle);
            //create header
            slDocument = CreateHeaderExcelDashboardMake(slDocument);
            //create data
            slDocument = CreateDataExcelDashboardMake(slDocument, listDataMake);


            //title Odometer
            slDocument.AddWorksheet("Odometer");
            slDocument.SetCellValue(1, 1, "Odometer");
            slDocument.MergeWorksheetCells(1, 1, 1, 6);
            //create style
            slDocument.SetCellStyle(1, 1, valueStyle);
            //create header
            slDocument = CreateHeaderExcelDashboardOdometer(slDocument);
            //create data
            slDocument = CreateDataExcelDashboardOdometer(slDocument, listDataOdo);


            //title Liter By Function
            slDocument.AddWorksheet("Liter By Function");
            slDocument.SetCellValue(1, 1, "Liter By Function");
            slDocument.MergeWorksheetCells(1, 1, 1, 6);
            slDocument.SetCellStyle(1, 1, valueStyle);
            //create header
            slDocument = CreateHeaderExcelDashboardLiterByFunction(slDocument);
            //create data
            slDocument = CreateDataExcelDashboardLiterByFunction(slDocument, listDataLiter);

            
            //title Fuel Cost By Function
            slDocument.AddWorksheet("Fuel Cost By Function");
            slDocument.SetCellValue(1, 1, "Fuel Cost By Function");
            slDocument.MergeWorksheetCells(1, 1, 1, 6);
            slDocument.SetCellStyle(1, 1, valueStyle);
            //create header
            slDocument = CreateHeaderExcelDashboardFuelCostByFunction(slDocument);
            //create data
            slDocument = CreateDataExcelDashboardFuelCostByFunction(slDocument, listDataFuel);


            //title Lease Cost By Function
            slDocument.AddWorksheet("Lease Cost By Function");
            slDocument.SetCellValue(1, 1, "Lease Cost By Function");
            slDocument.MergeWorksheetCells(1, 1, 1, 5);
            slDocument.SetCellStyle(1, 1, valueStyle);
            //create header
            slDocument = CreateHeaderExcelDashboardLeaseCostByFunction(slDocument);
            //create data
            slDocument = CreateDataExcelDashboardLeaseCostByFunction(slDocument, listDataLease);


            //title Sales By Region
            slDocument.AddWorksheet("Sales By Region");
            slDocument.SetCellValue(1, 1, "Sales By Region");
            slDocument.MergeWorksheetCells(1, 1, 1, 6);
            slDocument.SetCellStyle(1, 1, valueStyle);
            //create header
            slDocument = CreateHeaderExcelDashboardSalesByRegion(slDocument);
            //create data
            slDocument = CreateDataExcelDashboardSalesByRegion(slDocument, listDataSales);


            //title Accident
            slDocument.AddWorksheet("Accident");
            slDocument.SetCellValue(1, 1, "Accident");
            slDocument.MergeWorksheetCells(1, 1, 1, 6);
            slDocument.SetCellStyle(1, 1, valueStyle);
            //create header
            slDocument = CreateHeaderExcelDashboardAccident(slDocument);
            //create data
            slDocument = CreateDataExcelDashboardAccident(slDocument, listDataAccident);


            //title AC Vs OB
            slDocument.AddWorksheet("AC Vs OB");
            slDocument.SetCellValue(1, 1, "AC Vs OB");
            slDocument.MergeWorksheetCells(1, 1, 1, 5);
            slDocument.SetCellStyle(1, 1, valueStyle);
            //create header
            slDocument = CreateHeaderExcelDashboardAcVsOb(slDocument);
            //create data
            slDocument = CreateDataExcelDashboardAcVsOb(slDocument, listDataAcOb);


            var fileName = "ExecSum_SummaryAll" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        #endregion

        #endregion

        #region --------- Summary By Region --------------

        public ActionResult SummaryRegion()
        {
            var model = new SummaryAllModel();
            model.SearchView.YearFrom = DateTime.Now.Year;
            model.SearchView.YearTo = DateTime.Now.Year;

            model.IsByRegion = true;
            model.TitleForm = "Executive Summary By Region";
            model.TitleExport = "ExportSummaryRegion";
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View("SummaryAll", model);
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
            var input = Mapper.Map<VehicleGetByParamInput>(inputExport);
            input.Function = "Sales,Marketing";
            List<NoVehicleDto> data = _execSummBLL.GetNoOfVehicleData(input);
            var listData = Mapper.Map<List<NoVehicleData>>(data);

            var inputWtc = Mapper.Map<VehicleWtcGetByParamInput>(inputExport);
            inputWtc.Function = "Sales,Marketing";
            List<NoVehicleWtcDto> dataWtc = _execSummBLL.GetNoOfVehicleWtcData(inputWtc);
            var listDataWtc = Mapper.Map<List<NoVehicleWtcData>>(dataWtc);

            var inputMake = Mapper.Map<VehicleMakeGetByParamInput>(inputExport);
            //inputMake.Function = "Sales,Marketing";
            List<NoVehicleMakeDto> dataMake = _execSummBLL.GetNoOfVehicleMakeData(inputMake);
            var listDataMake = Mapper.Map<List<NoVehicleMakeData>>(dataMake);

            var inputOdo = Mapper.Map<OdometerGetByParamInput>(inputExport);
            inputOdo.Function = "Sales,Marketing";
            List<OdometerDto> dataOdo = _execSummBLL.GetOdometerData(inputOdo);
            var listDataOdo = Mapper.Map<List<OdometerData>>(dataOdo);

            var inputLiter = Mapper.Map<LiterFuncGetByParamInput>(inputExport);
            inputLiter.Function = "Sales,Marketing";
            List<LiterByFunctionDto> dataLiter = _execSummBLL.GetLiterByFunctionData(inputLiter);
            var listDataLiter = Mapper.Map<List<LiterByFunctionData>>(dataLiter);

            var inputFuel = Mapper.Map<FuelCostFuncGetByParamInput>(inputExport);
            inputFuel.Function = "Sales,Marketing";
            List<FuelCostByFunctionDto> dataFuel = _execSummBLL.GetFuelCostByFunctionData(inputFuel);
            var listDataFuel = Mapper.Map<List<FuelCostByFunctionData>>(dataFuel);

            var inputLease = Mapper.Map<LeaseCostFuncGetByParamInput>(inputExport);
            inputLease.Function = "Sales,Marketing";
            List<LeaseCostByFunctionDto> dataLease = _execSummBLL.GetLeaseCostByFunctionData(inputLease);
            var listDataLease = Mapper.Map<List<LeaseCostByFunctionData>>(dataLease);

            var inputSales = Mapper.Map<SalesRegionGetByParamInput>(inputExport);
            //inputSales.Function = "Sales,Marketing";
            List<SalesByRegionDto> dataSales = _execSummBLL.GetSalesByRegionData(inputSales);
            var listDataSales = Mapper.Map<List<SalesByRegionData>>(dataSales);

            var inputAccident = Mapper.Map<AccidentGetByParamInput>(inputExport);
            inputAccident.Function = "Sales,Marketing";
            List<AccidentDto> dataAccident = _execSummBLL.GetAccidentData(inputAccident);
            var listDataAccident = Mapper.Map<List<AccidentData>>(dataAccident);

            var inputAcOb = Mapper.Map<AcVsObGetByParamInput>(inputExport);
            inputAcOb.Function = "Sales,Marketing";
            List<AcVsObDto> dataAcOb = _execSummBLL.GetAcVsObData(inputAcOb);
            var listDataAcOb = Mapper.Map<List<AcVsObData>>(dataAcOb);

            var slDocument = new SLDocument();

            //title no of vehicle
            slDocument.SetCellValue(1, 1, "Number Of Vehicle");
            slDocument.MergeWorksheetCells(1, 1, 1, 7);
            slDocument.RenameWorksheet("Sheet1", "Number Of Vehicle");
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


            //title no of vehicle wtc
            slDocument.AddWorksheet("Number Of Vehicle WTC");
            slDocument.SetCellValue(1, 1, "Number Of Vehicle WTC");
            slDocument.MergeWorksheetCells(1, 1, 1, 5);
            slDocument.SetCellStyle(1, 1, valueStyle);
            //create header
            slDocument = CreateHeaderExcelDashboardWtc(slDocument);
            //create data
            slDocument = CreateDataExcelDashboardWtc(slDocument, listDataWtc);


            //title no of vehicle Make
            slDocument.AddWorksheet("Number Of Vehicle Make-Type");
            slDocument.SetCellValue(1, 1, "Number Of Vehicle Make-Type");
            slDocument.MergeWorksheetCells(1, 1, 1, 5);
            slDocument.SetCellStyle(1, 1, valueStyle);
            //create header
            slDocument = CreateHeaderExcelDashboardMake(slDocument);
            //create data
            slDocument = CreateDataExcelDashboardMake(slDocument, listDataMake);


            //title Odometer
            slDocument.AddWorksheet("Odometer");
            slDocument.SetCellValue(1, 1, "Odometer");
            slDocument.MergeWorksheetCells(1, 1, 1, 6);
            //create style
            slDocument.SetCellStyle(1, 1, valueStyle);
            //create header
            slDocument = CreateHeaderExcelDashboardOdometer(slDocument);
            //create data
            slDocument = CreateDataExcelDashboardOdometer(slDocument, listDataOdo);


            //title Liter By Function
            slDocument.AddWorksheet("Liter By Function");
            slDocument.SetCellValue(1, 1, "Liter By Function");
            slDocument.MergeWorksheetCells(1, 1, 1, 6);
            slDocument.SetCellStyle(1, 1, valueStyle);
            //create header
            slDocument = CreateHeaderExcelDashboardLiterByFunction(slDocument);
            //create data
            slDocument = CreateDataExcelDashboardLiterByFunction(slDocument, listDataLiter);


            //title Fuel Cost By Function
            slDocument.AddWorksheet("Fuel Cost By Function");
            slDocument.SetCellValue(1, 1, "Fuel Cost By Function");
            slDocument.MergeWorksheetCells(1, 1, 1, 6);
            slDocument.SetCellStyle(1, 1, valueStyle);
            //create header
            slDocument = CreateHeaderExcelDashboardFuelCostByFunction(slDocument);
            //create data
            slDocument = CreateDataExcelDashboardFuelCostByFunction(slDocument, listDataFuel);


            //title Lease Cost By Function
            slDocument.AddWorksheet("Lease Cost By Function");
            slDocument.SetCellValue(1, 1, "Lease Cost By Function");
            slDocument.MergeWorksheetCells(1, 1, 1, 5);
            slDocument.SetCellStyle(1, 1, valueStyle);
            //create header
            slDocument = CreateHeaderExcelDashboardLeaseCostByFunction(slDocument);
            //create data
            slDocument = CreateDataExcelDashboardLeaseCostByFunction(slDocument, listDataLease);


            //title Sales By Region
            slDocument.AddWorksheet("Sales By Region");
            slDocument.SetCellValue(1, 1, "Sales By Region");
            slDocument.MergeWorksheetCells(1, 1, 1, 6);
            slDocument.SetCellStyle(1, 1, valueStyle);
            //create header
            slDocument = CreateHeaderExcelDashboardSalesByRegion(slDocument);
            //create data
            slDocument = CreateDataExcelDashboardSalesByRegion(slDocument, listDataSales);


            //title Accident
            slDocument.AddWorksheet("Accident");
            slDocument.SetCellValue(1, 1, "Accident");
            slDocument.MergeWorksheetCells(1, 1, 1, 6);
            slDocument.SetCellStyle(1, 1, valueStyle);
            //create header
            slDocument = CreateHeaderExcelDashboardAccident(slDocument);
            //create data
            slDocument = CreateDataExcelDashboardAccident(slDocument, listDataAccident);


            //title AC Vs OB
            slDocument.AddWorksheet("AC Vs OB");
            slDocument.SetCellValue(1, 1, "AC Vs OB");
            slDocument.MergeWorksheetCells(1, 1, 1, 5);
            slDocument.SetCellStyle(1, 1, valueStyle);
            //create header
            slDocument = CreateHeaderExcelDashboardAcVsOb(slDocument);
            //create data
            slDocument = CreateDataExcelDashboardAcVsOb(slDocument, listDataAcOb);


            var fileName = "ExecSum_SummaryByRegion" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        #endregion

        #endregion

        #region --------- Number Of Vehicle --------------

        public ActionResult Index()
        {
            var model = new ExecutiveSummaryModel();
            model.SearchView.YearFrom = DateTime.Now.Year;
            model.SearchView.YearTo = DateTime.Now.Year;

            var input = Mapper.Map<VehicleGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetNoOfVehicleData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            var settingData = _settingBLL.GetSetting();
            var listVehType = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.IsActive).Select(x => new { x.SettingValue }).ToList();
            var listSupMethod = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.SupplyMethod) && x.IsActive).Select(x => new { x.SettingValue }).ToList();
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();

            model.TitleForm = "Number Of Vehicle";
            model.TitleExport = "ExportNoVehicle";
            model.NoVehicleList = Mapper.Map<List<NoVehicleData>>(data);
            model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
            model.SearchView.SupplyMethodList = new SelectList(listSupMethod, "SettingValue", "SettingValue");
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
            model.SearchView.Functions = GetFunctionsMultiSelectList();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
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
            var input = Mapper.Map<VehicleGetByParamInput>(filter);

            var dbData = _execSummBLL.GetNoOfVehicleData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            return Mapper.Map<List<NoVehicleData>>(dbData);
        }

        #region --------- Export --------------

        public void ExportNoVehicle(ExecutiveSummaryModel model)
        {
            string pathFile = "";

            var input = Mapper.Map<VehicleGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsNoVehicle(input);
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

        #endregion

        #region --------- Number Of Vehicle WTC --------------

        public ActionResult VehicleWtc()
        {
            var model = new NumberVehicleWtcModel();
            model.SearchView.YearFrom = DateTime.Now.Year;
            model.SearchView.YearTo = DateTime.Now.Year;

            var input = Mapper.Map<VehicleWtcGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetNoOfVehicleWtcData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();

            model.TitleForm = "Number Of Vehicle WTC";
            model.TitleExport = "ExportNoVehicleWtc";
            model.NoVehicleWtcList = Mapper.Map<List<NoVehicleWtcData>>(data);
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
            model.SearchView.Functions = GetFunctionsMultiSelectList();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
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
            var input = Mapper.Map<VehicleWtcGetByParamInput>(filter);

            var dbData = _execSummBLL.GetNoOfVehicleWtcData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            return Mapper.Map<List<NoVehicleWtcData>>(dbData);
        }

        #region --------- Export --------------

        public void ExportNoVehicleWtc(NumberVehicleWtcModel model)
        {
            string pathFile = "";

            var input = Mapper.Map<VehicleWtcGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsNoVehicleWtc(input);

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

        #endregion

        #region --------- Number Of Vehicle Make --------------

        public ActionResult VehicleMake()
        {
            var model = new NumberVehicleMakeModel();
            model.SearchView.YearFrom = DateTime.Now.Year;
            model.SearchView.YearTo = DateTime.Now.Year;

            var input = Mapper.Map<VehicleMakeGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetNoOfVehicleMakeData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);

            model.TitleForm = "Number Of Vehicle Make";
            model.TitleExport = "ExportNoVehicleMake";
            model.NoVehicleMakeList = Mapper.Map<List<NoVehicleMakeData>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
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

        public void ExportNoVehicleMake(NumberVehicleMakeModel model)
        {
            string pathFile = "";

            var input = Mapper.Map<VehicleMakeGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsNoVehicleMake(input);

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

        #endregion

        #region --------- Odometer --------------

        public ActionResult Odometer()
        {
            var model = new OdometerModel();
            model.SearchView.YearFrom = DateTime.Now.Year;
            model.SearchView.YearTo = DateTime.Now.Year;

            var input = Mapper.Map<OdometerGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetOdometerData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();
            var listVehType = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.IsActive).Select(x => new { x.SettingValue }).ToList();

            model.TitleForm = "Odometer";
            model.TitleExport = "ExportOdometer";
            model.OdometerDataList = Mapper.Map<List<OdometerData>>(data);
            model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
            model.SearchView.Functions = GetFunctionsMultiSelectList();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
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
            var input = Mapper.Map<OdometerGetByParamInput>(filter);

            var dbData = _execSummBLL.GetOdometerData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            return Mapper.Map<List<OdometerData>>(dbData);
        }

        #region --------- Export --------------

        public void ExportOdometer(OdometerModel model)
        {
            string pathFile = "";

            var input = Mapper.Map<OdometerGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsOdometer(input);

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

        #endregion

        #region --------- Liter By Function --------------

        public ActionResult LiterByFunction()
        {
            var model = new LiterByFunctionModel();
            model.SearchView.YearFrom = DateTime.Now.Year;
            model.SearchView.YearTo = DateTime.Now.Year;

            var input = Mapper.Map<LiterFuncGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetLiterByFunctionData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();
            var listVehType = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.IsActive).Select(x => new { x.SettingValue }).ToList();

            model.TitleForm = "Liter By Function";
            model.TitleExport = "ExportLiterByFunction";
            model.LiterByFuncDataList = Mapper.Map<List<LiterByFunctionData>>(data);
            model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
            model.SearchView.Functions = GetFunctionsMultiSelectList();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
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
            var input = Mapper.Map<LiterFuncGetByParamInput>(filter);

            var dbData = _execSummBLL.GetLiterByFunctionData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            return Mapper.Map<List<LiterByFunctionData>>(dbData);
        }

        #region --------- Export --------------

        public void ExportLiterByFunction(LiterByFunctionModel model)
        {
            string pathFile = "";

            var input = Mapper.Map<LiterFuncGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsLiterByFunction(input);

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

        #endregion

        #region --------- Fuel Cost By Function --------------

        public ActionResult FuelCostByFunction()
        {
            var model = new FuelCostByFunctionModel();
            model.SearchView.YearFrom = DateTime.Now.Year;
            model.SearchView.YearTo = DateTime.Now.Year;

            var input = Mapper.Map<FuelCostFuncGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetFuelCostByFunctionData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();
            var listVehType = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.IsActive).Select(x => new { x.SettingValue }).ToList();

            model.TitleForm = "Fuel Cost By Function";
            model.TitleExport = "ExportFuelCostByFunction";
            model.FuelCostByFuncDataList = Mapper.Map<List<FuelCostByFunctionData>>(data);
            model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
            model.SearchView.Functions = GetFunctionsMultiSelectList();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
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
            var input = Mapper.Map<FuelCostFuncGetByParamInput>(filter);

            var dbData = _execSummBLL.GetFuelCostByFunctionData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            return Mapper.Map<List<FuelCostByFunctionData>>(dbData);
        }

        #region --------- Export --------------

        public void ExportFuelCostByFunction(FuelCostByFunctionModel model)
        {
            string pathFile = "";

            var input = Mapper.Map<FuelCostFuncGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsFuelCostByFunction(input);

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

        #endregion

        #region --------- Lease Cost By Function --------------

        public ActionResult LeaseCostByFunction()
        {
            var model = new LeaseCostByFunctionModel();
            model.SearchView.YearFrom = DateTime.Now.Year;
            model.SearchView.YearTo = DateTime.Now.Year;

            var input = Mapper.Map<LeaseCostFuncGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetLeaseCostByFunctionData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();
            
            model.TitleForm = "Lease Cost By Function";
            model.TitleExport = "ExportLeaseCostByFunction";
            model.LeaseCostByFuncDataList = Mapper.Map<List<LeaseCostByFunctionData>>(data);
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
            model.SearchView.Functions = GetFunctionsMultiSelectList();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
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
            var input = Mapper.Map<LeaseCostFuncGetByParamInput>(filter);

            var dbData = _execSummBLL.GetLeaseCostByFunctionData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            return Mapper.Map<List<LeaseCostByFunctionData>>(dbData);
        }

        #region --------- Export --------------

        public void ExportLeaseCostByFunction(LeaseCostByFunctionModel model)
        {
            string pathFile = "";

            var input = Mapper.Map<LeaseCostFuncGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsLeaseCostByFunction(input);

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

        #endregion

        #region --------- Sales By Region --------------

        public ActionResult SalesByRegion()
        {
            var model = new SalesByRegionModel();
            model.SearchView.YearFrom = DateTime.Now.Year;
            model.SearchView.YearTo = DateTime.Now.Year;

            var input = Mapper.Map<SalesRegionGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetSalesByRegionData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();

            model.TitleForm = "Sales By Region";
            model.TitleExport = "ExportSalesByRegion";
            model.SalesByRegionDataList = Mapper.Map<List<SalesByRegionData>>(data);
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
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
            var input = Mapper.Map<SalesRegionGetByParamInput>(filter);

            var dbData = _execSummBLL.GetSalesByRegionData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            return Mapper.Map<List<SalesByRegionData>>(dbData);
        }

        #region --------- Export --------------

        public void ExportSalesByRegion(SalesByRegionModel model)
        {
            string pathFile = "";

            var input = Mapper.Map<SalesRegionGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsSalesByRegion(input);

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

        #endregion

        #region --------- Accident --------------

        public ActionResult Accident()
        {
            var model = new AccidentModel();
            model.SearchView.YearFrom = DateTime.Now.Year;
            model.SearchView.YearTo = DateTime.Now.Year;

            var input = Mapper.Map<AccidentGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetAccidentData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();
            var listVehType = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.IsActive).Select(x => new { x.SettingValue }).ToList();

            model.TitleForm = "Accident";
            model.TitleExport = "ExportAccident";
            model.AccidentDataList = Mapper.Map<List<AccidentData>>(data);
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
            model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
            model.SearchView.Functions = GetFunctionsMultiSelectList();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
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
            var input = Mapper.Map<AccidentGetByParamInput>(filter);

            var dbData = _execSummBLL.GetAccidentData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);
            return Mapper.Map<List<AccidentData>>(dbData);
        }

        #region --------- Export --------------

        public void ExportAccident(AccidentModel model)
        {
            string pathFile = "";

            var input = Mapper.Map<AccidentGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsAccident(input);

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

        #endregion

        #region --------- AC Vs OB --------------

        public ActionResult AcVsOb()
        {
            var model = new AcVsObModel();
            model.SearchView.YearFrom = DateTime.Now.Year;
            model.SearchView.YearTo = DateTime.Now.Year;

            var input = Mapper.Map<AcVsObGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetAcVsObData(input).OrderBy(x => x.REPORT_MONTH).OrderBy(x => x.REPORT_YEAR);

            model.TitleForm = "AC Vs OB";
            model.TitleExport = "ExportAcVsOb";
            model.AcVsObDataList = Mapper.Map<List<AcVsObData>>(data);
            model.SearchView.Functions = GetFunctionsMultiSelectList();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
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

        public void ExportAcVsOb(AcVsObModel model)
        {
            string pathFile = "";

            var input = Mapper.Map<AcVsObGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsLeaseCostByFunction(input);

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

        public void ExportSumPtdByFunction(SumPtdByFunctionModel model)
        {
            string pathFile = "";

            var input = Mapper.Map<SumPtdFuncGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsSumPtdByFunction(input);

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
