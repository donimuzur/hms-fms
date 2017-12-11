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

        public RptExecutiveSummaryController(IPageBLL pageBll, IExecutiveSummaryBLL execSummBLL, ISettingBLL settingBLL, ILocationMappingBLL locationMappingBLL)
            : base(pageBll, Core.Enums.MenuList.RptExecutiveSummary)
        {
            _pageBLL = pageBll;
            _execSummBLL = execSummBLL;
            _settingBLL = settingBLL;
            _locationMappingBLL = locationMappingBLL;
            _mainMenu = Enums.MenuList.RptExecutiveSummary;
        }

        #endregion

        #region --------- Number Of Vehicle --------------

        public ActionResult Index()
        {
            var model = new ExecutiveSummaryModel();
            var input = Mapper.Map<VehicleGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetNoOfVehicleData(input);
            var settingData = _settingBLL.GetSetting();
            var listVehType = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.IsActive).Select(x => new { x.SettingValue }).ToList();
            var listSupMethod = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.SupplyMethod) && x.IsActive).Select(x => new { x.SettingValue }).ToList();

            model.TitleForm = "Number Of Vehicle";
            model.TitleExport = "ExportNoVehicle";
            model.NoVehicleList = Mapper.Map<List<NoVehicleData>>(data);
            model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
            model.SearchView.SupplyMethodList = new SelectList(listSupMethod, "SettingValue", "SettingValue");
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
                var data = _execSummBLL.GetNoOfVehicleData(new VehicleGetByParamInput());
                return Mapper.Map<List<NoVehicleData>>(data);
            }

            //getbyparams
            var input = Mapper.Map<VehicleGetByParamInput>(filter);

            var dbData = _execSummBLL.GetNoOfVehicleData(input);
            return Mapper.Map<List<NoVehicleData>>(dbData);
        }

        #region --------- Export --------------

        public void ExportNoVehicle(ExecutiveSummaryModel model)
        {
            string pathFile = "";

            var input = Mapper.Map<VehicleGetByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsNoVehicle(input);

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
            slDocument.MergeWorksheetCells(1, 1, 1, 6);
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
            slDocument.SetCellValue(iRow, 2, "Supply Method");
            slDocument.SetCellValue(iRow, 3, "Function");
            slDocument.SetCellValue(iRow, 4, "No Of Vehicle");
            slDocument.SetCellValue(iRow, 5, "Month");
            slDocument.SetCellValue(iRow, 6, "Year");

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

        private SLDocument CreateDataExcelDashboard(SLDocument slDocument, List<NoVehicleData> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.VehicleType);
                slDocument.SetCellValue(iRow, 2, data.SupplyMethod);
                slDocument.SetCellValue(iRow, 3, data.Function);
                slDocument.SetCellValue(iRow, 4, data.NoOfVehicle.ToString());
                slDocument.SetCellValue(iRow, 5, data.Month);
                slDocument.SetCellValue(iRow, 6, data.ReportYear.ToString());

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

            return slDocument;
        }

        #endregion

        #endregion

        #region --------- Number Of Vehicle WTC --------------

        public ActionResult VehicleWtc()
        {
            var model = new NumberVehicleWtcModel();
            var input = Mapper.Map<VehicleWtcGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetNoOfVehicleWtcData(input);
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();

            model.TitleForm = "Number Of Vehicle WTC";
            model.TitleExport = "ExportNoVehicleWtc";
            model.NoVehicleWtcList = Mapper.Map<List<NoVehicleWtcData>>(data);
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
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
                var data = _execSummBLL.GetNoOfVehicleWtcData(new VehicleWtcGetByParamInput());
                return Mapper.Map<List<NoVehicleWtcData>>(data);
            }

            //getbyparams
            var input = Mapper.Map<VehicleWtcGetByParamInput>(filter);

            var dbData = _execSummBLL.GetNoOfVehicleWtcData(input);
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
            slDocument.SetCellValue(iRow, 3, "No Of Vehicle");
            slDocument.SetCellValue(iRow, 4, "Month");
            slDocument.SetCellValue(iRow, 5, "Year");

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

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.Regional);
                slDocument.SetCellValue(iRow, 2, data.Function);
                slDocument.SetCellValue(iRow, 3, data.NoOfVehicle.ToString());
                slDocument.SetCellValue(iRow, 4, data.Month);
                slDocument.SetCellValue(iRow, 5, data.ReportYear.ToString());

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

            return slDocument;
        }

        #endregion

        #endregion

        #region --------- Number Of Vehicle Make --------------

        public ActionResult VehicleMake()
        {
            var model = new NumberVehicleMakeModel();
            var input = Mapper.Map<VehicleMakeGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetNoOfVehicleMakeData(input);

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
                var data = _execSummBLL.GetNoOfVehicleMakeData(new VehicleMakeGetByParamInput());
                return Mapper.Map<List<NoVehicleMakeData>>(data);
            }

            //getbyparams
            var input = Mapper.Map<VehicleMakeGetByParamInput>(filter);

            var dbData = _execSummBLL.GetNoOfVehicleMakeData(input);
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
            slDocument.SetCellValue(iRow, 3, "No Of Vehicle");
            slDocument.SetCellValue(iRow, 4, "Month");
            slDocument.SetCellValue(iRow, 5, "Year");

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

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.Manufacturer);
                slDocument.SetCellValue(iRow, 2, data.BodyType);
                slDocument.SetCellValue(iRow, 3, data.NoOfVehicle.ToString());
                slDocument.SetCellValue(iRow, 4, data.Month);
                slDocument.SetCellValue(iRow, 5, data.ReportYear.ToString());

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

            return slDocument;
        }

        #endregion

        #endregion
    }
}
