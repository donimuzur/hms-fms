﻿using System;
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
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();

            model.TitleForm = "Number Of Vehicle";
            model.TitleExport = "ExportNoVehicle";
            model.NoVehicleList = Mapper.Map<List<NoVehicleData>>(data);
            model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
            model.SearchView.SupplyMethodList = new SelectList(listSupMethod, "SettingValue", "SettingValue");
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
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

            return slDocument;
        }

        #endregion

        #endregion

        #region --------- Odometer --------------

        public ActionResult Odometer()
        {
            var model = new OdometerModel();
            var input = Mapper.Map<OdometerGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetOdometerData(input);
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();
            var listVehType = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.IsActive).Select(x => new { x.SettingValue }).ToList();

            model.TitleForm = "Odometer";
            model.TitleExport = "ExportOdometer";
            model.OdometerDataList = Mapper.Map<List<OdometerData>>(data);
            model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
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
                var data = _execSummBLL.GetOdometerData(new OdometerGetByParamInput());
                return Mapper.Map<List<OdometerData>>(data);
            }

            //getbyparams
            var input = Mapper.Map<OdometerGetByParamInput>(filter);

            var dbData = _execSummBLL.GetOdometerData(input);
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

            return slDocument;
        }

        #endregion

        #endregion

        #region --------- Liter By Function --------------

        public ActionResult LiterByFunction()
        {
            var model = new LiterByFunctionModel();
            var input = Mapper.Map<LiterFuncGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetLiterByFunctionData(input);
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();
            var listVehType = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.IsActive).Select(x => new { x.SettingValue }).ToList();

            model.TitleForm = "Liter By Function";
            model.TitleExport = "ExportLiterByFunction";
            model.LiterByFuncDataList = Mapper.Map<List<LiterByFunctionData>>(data);
            model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
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
                var data = _execSummBLL.GetLiterByFunctionData(new LiterFuncGetByParamInput());
                return Mapper.Map<List<LiterByFunctionData>>(data);
            }

            //getbyparams
            var input = Mapper.Map<LiterFuncGetByParamInput>(filter);

            var dbData = _execSummBLL.GetLiterByFunctionData(input);
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

            return slDocument;
        }

        #endregion

        #endregion

        #region --------- Fuel Cost By Function --------------

        public ActionResult FuelCostByFunction()
        {
            var model = new FuelCostByFunctionModel();
            var input = Mapper.Map<FuelCostFuncGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetFuelCostByFunctionData(input);
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();
            var listVehType = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.IsActive).Select(x => new { x.SettingValue }).ToList();

            model.TitleForm = "Fuel Cost By Function";
            model.TitleExport = "ExportFuelCostByFunction";
            model.FuelCostByFuncDataList = Mapper.Map<List<FuelCostByFunctionData>>(data);
            model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
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
                var data = _execSummBLL.GetFuelCostByFunctionData(new FuelCostFuncGetByParamInput());
                return Mapper.Map<List<FuelCostByFunctionData>>(data);
            }

            //getbyparams
            var input = Mapper.Map<FuelCostFuncGetByParamInput>(filter);

            var dbData = _execSummBLL.GetFuelCostByFunctionData(input);
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

            return slDocument;
        }

        #endregion

        #endregion

        #region --------- Lease Cost By Function --------------

        public ActionResult LeaseCostByFunction()
        {
            var model = new LeaseCostByFunctionModel();
            var input = Mapper.Map<LeaseCostFuncGetByParamInput>(model.SearchView);
            var data = _execSummBLL.GetLeaseCostByFunctionData(input);
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();
            
            model.TitleForm = "Lease Cost By Function";
            model.TitleExport = "ExportLeaseCostByFunction";
            model.LeaseCostByFuncDataList = Mapper.Map<List<LeaseCostByFunctionData>>(data);
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
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
                var data = _execSummBLL.GetLeaseCostByFunctionData(new LeaseCostFuncGetByParamInput());
                return Mapper.Map<List<LeaseCostByFunctionData>>(data);
            }

            //getbyparams
            var input = Mapper.Map<LeaseCostFuncGetByParamInput>(filter);

            var dbData = _execSummBLL.GetLeaseCostByFunctionData(input);
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
            slDocument.MergeWorksheetCells(1, 1, 1, 6);
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

            slDocument.SetCellStyle(iRow, 1, iRow, 6, headerStyle);

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

            return slDocument;
        }

        #endregion

        #endregion
    }
}
