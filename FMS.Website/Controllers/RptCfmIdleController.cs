using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;
using FMS.BusinessObject.Inputs;
using AutoMapper;
using System.IO;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;

namespace FMS.Website.Controllers
{
    public class RptCfmIdleController :BaseController
    {
        
        private IPageBLL _pageBLL;
        private ISettingBLL _settingBLL;
        private ICfmIdleReportBLL _cfmIdleReportBLL;
        private Enums.MenuList _mainMenu;

        public RptCfmIdleController(IPageBLL pageBll, ICfmIdleReportBLL CfmIdleReportBLL, ISettingBLL SettingBLL)
            : base(pageBll, Core.Enums.MenuList.RptCfmIdle)
        {
            _pageBLL = pageBll;
            _cfmIdleReportBLL = CfmIdleReportBLL;
            _settingBLL = SettingBLL;
            _mainMenu = Enums.MenuList.RptExecutiveSummary;
        }

        public ActionResult Index()
        {
            var model = new CfmIdleReportModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.SearchView.FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            model.SearchView.ToDate = DateTime.Today;
            var filter = new CfmIdleGetByParamInput();
            filter.FromDate = new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
            filter.ToDate = DateTime.Today;

            var data = _cfmIdleReportBLL.GetCfmIdle(filter);
            var ListData = Mapper.Map<List<CfmIdleVehicle>>(data);
            model.ListCfmIdle = ListData;

            foreach (var item in model.ListCfmIdle)
            {
                var CfmIdleVehicle = new CfmIdleVehicle();

                var today = DateTime.Today;
                var StartIdle = (decimal)(today - new DateTime(1900, 1, 1)).TotalDays;
                var EndIdle = (decimal)(today - new DateTime(1900, 1, 1)).TotalDays ;

                if (item.StartIdle.HasValue)
                {
                    StartIdle = (decimal)(item.StartIdle.Value.Date - new DateTime(1900, 1, 1)).TotalDays;
                }
                if (item.EndIdle.HasValue)
                {
                    EndIdle = (decimal)(item.EndIdle.Value.Date - new DateTime(1900, 1, 1)).TotalDays;
                }

                item.IdleDuration = Math.Round((decimal)(EndIdle - StartIdle) / 30, 2);

                item.TotalMonthly = Math.Round((decimal)(item.IdleDuration * (item.MonthlyInstallment.HasValue ? item.MonthlyInstallment : 0)), 2);

            }

            var GrandTotal = new CfmIdleVehicle
            {
                PoliceNumber = "",
                Note = "GrandTotal",
                CreatedDate = DateTime.Today,
                IdleDuration = model.ListCfmIdle.Sum(x => x.IdleDuration),
                TotalMonthly = model.ListCfmIdle.Sum(x => x.TotalMonthly)
            };

            if (model.ListCfmIdle != null && model.ListCfmIdle.Count > 0)
            {
                var result = model.ListCfmIdle.GroupBy(x => x.PoliceNumber).Select(grouping => new CfmIdleVehicle
                {
                    PoliceNumber = grouping.First().PoliceNumber,
                    Note = "SubTotal",
                    CreatedDate = DateTime.Today,
                    IdleDuration = grouping.Sum(x => x.IdleDuration),
                    TotalMonthly = grouping.Sum(x => x.TotalMonthly)
                }).ToList();
                foreach (var itemResult in result)
                {
                    model.ListCfmIdle.Add(itemResult);
                }
            }
            model.ListCfmIdle= model.ListCfmIdle.OrderBy(x => x.PoliceNumber).ToList();

            if (model.ListCfmIdle != null && model.ListCfmIdle.Count > 0) model.ListCfmIdle.Add(GrandTotal);

            return View(model);
        }

        [HttpPost]
        public PartialViewResult ListCfmIdleVehicle(CfmIdleReportModel model)
        {
            model.ListCfmIdle = new List<CfmIdleVehicle>();
            model.ListCfmIdle = GetVehicleData(model.SearchView);

            foreach (var item in model.ListCfmIdle)
            {
                var CfmIdleVehicle = new CfmIdleVehicle();

                var today = DateTime.Today;
                var StartIdle = (decimal)(today - new DateTime(1900, 1, 1)).TotalDays;
                var EndIdle = (decimal)(today - new DateTime(1900, 1, 1)).TotalDays;

                if (item.StartIdle.HasValue)
                {
                    StartIdle = (decimal)(item.StartIdle.Value.Date - new DateTime(1900, 1, 1)).TotalDays;
                }
                if (item.EndIdle.HasValue)
                {
                    EndIdle = (decimal)(item.EndIdle.Value.Date - new DateTime(1900, 1, 1)).TotalDays;
                }

                item.IdleDuration = Math.Round((decimal)(EndIdle - StartIdle) / 30, 2);

                item.TotalMonthly = Math.Round((decimal)(item.IdleDuration * (item.MonthlyInstallment.HasValue ? item.MonthlyInstallment : 0)), 2);
                
            }

            var GrandTotal = new CfmIdleVehicle
            {
                PoliceNumber = "",
                Note = "GrandTotal",
                CreatedDate = DateTime.Today,
                IdleDuration = model.ListCfmIdle.Sum(x => x.IdleDuration),
                TotalMonthly = model.ListCfmIdle.Sum(x => x.TotalMonthly)
            };

            if (model.ListCfmIdle != null && model.ListCfmIdle.Count > 0)
            {
                var result = model.ListCfmIdle.GroupBy(x => x.PoliceNumber).Select(grouping => new CfmIdleVehicle
                {
                    PoliceNumber = grouping.First().PoliceNumber,
                    Note = "SubTotal",
                    CreatedDate = DateTime.Today,
                    IdleDuration = grouping.Sum(x => x.IdleDuration),
                    TotalMonthly = grouping.Sum(x => x.TotalMonthly)
                }).ToList();
                foreach (var itemResult in result)
                {
                    model.ListCfmIdle.Add(itemResult);
                }
            }
            model.ListCfmIdle = model.ListCfmIdle.OrderBy(x => x.PoliceNumber).ToList();

            if (model.ListCfmIdle != null && model.ListCfmIdle.Count > 0) model.ListCfmIdle.Add(GrandTotal);

            return PartialView("_ListCfmIdleVehicle", model);
        }

        private List<CfmIdleVehicle> GetVehicleData(CfmIdleSearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _cfmIdleReportBLL.GetCfmIdle(new CfmIdleGetByParamInput());
                return Mapper.Map<List<CfmIdleVehicle>>(data);
            }

            //getbyparams
            var input = Mapper.Map<CfmIdleGetByParamInput>(filter);

            var dbData = _cfmIdleReportBLL.GetCfmIdle(input);
            return Mapper.Map<List<CfmIdleVehicle>>(dbData);
        }


        #region ------------ Export ---------------
        public void ExportCfmIdle(CfmIdleReportModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsCfmIdle(model);

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
        private string CreateXlsCfmIdle(CfmIdleReportModel model)
        {
            //get data
          var data = GetVehicleData(model.SearchView);
            foreach (var item in data)
            {
                var CfmIdleVehicle = new CfmIdleVehicle();

                var StartIdle = (decimal)(item.StartIdle.Value.Date - new DateTime(1900, 1, 1)).TotalDays - 1;

                var today = DateTime.Today;
                var EndIdle = (decimal)(today - new DateTime(1900, 1, 1)).TotalDays + 1;

                if (item.EndIdle.HasValue)
                {
                    EndIdle = (decimal)(item.EndIdle.Value.Date - new DateTime(1900, 1, 1)).TotalDays + 1;
                }

                item.IdleDuration = Math.Round((decimal)(EndIdle - StartIdle) / 30, 2);

                item.TotalMonthly = Math.Round((decimal)(item.IdleDuration * item.MonthlyInstallment), 2);

            }

            var GrandTotal = new CfmIdleVehicle
            {
                PoliceNumber = "",
                Note = "GrandTotal",
                CreatedDate = DateTime.Today,
                IdleDuration = data.Sum(x => x.IdleDuration),
                TotalMonthly = data.Sum(x => x.TotalMonthly)
            };

            if (data.Count > 0 && data != null)
            {
                var result = data.GroupBy(x => x.PoliceNumber).Select(grouping => new CfmIdleVehicle
                {
                    PoliceNumber = grouping.First().PoliceNumber,
                    Note = "SubTotal",
                    CreatedDate = DateTime.Today,
                    IdleDuration = grouping.Sum(x => x.IdleDuration),
                    TotalMonthly = grouping.Sum(x => x.TotalMonthly)
                }).ToList();
                foreach (var itemResult in result)
                {
                    data.Add(itemResult);
                }
            }
            data = data.OrderBy(x => x.PoliceNumber).ToList();

            if (data.Count > 0 && data != null) data.Add(GrandTotal);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "CFM IDLE Report");
            slDocument.MergeWorksheetCells(1, 1, 1, 16);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderCfmIdle(slDocument);

            //create data
            slDocument = CreateDataExcelCfmIdle(slDocument, data);

            var fileName = "Cfm_Idle_Report" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;
        }
        private SLDocument CreateHeaderCfmIdle(SLDocument slDocument)
        {
            int iRow = 2;
            slDocument.SetCellValue(iRow, 1, "Police Number");
            slDocument.SetCellValue(iRow, 2, "Manufacturer");
            slDocument.SetCellValue(iRow, 3, "Model");
            slDocument.SetCellValue(iRow, 4, "Series");
            slDocument.SetCellValue(iRow, 5, "Body Type");
            slDocument.SetCellValue(iRow, 6, "Colour");
            slDocument.SetCellValue(iRow, 7, "Group Level");
            slDocument.SetCellValue(iRow, 8, "Start Contract");
            slDocument.SetCellValue(iRow, 9, "End Contract");
            slDocument.SetCellValue(iRow, 10, "Supplier");
            slDocument.SetCellValue(iRow, 11, "Cost Center");
            slDocument.SetCellValue(iRow, 12, "Start Idle");
            slDocument.SetCellValue(iRow, 13, "End Idle");
            slDocument.SetCellValue(iRow, 14, "Idle Duration");
            slDocument.SetCellValue(iRow, 15, "Monthly Installment");
            slDocument.SetCellValue(iRow, 16, "Total Monthly");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 16, headerStyle);

            return slDocument;

        }
        private SLDocument CreateDataExcelCfmIdle(SLDocument slDocument, List<CfmIdleVehicle> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {

                if (string.IsNullOrEmpty(data.Note))
                {
                    slDocument.SetCellValue(iRow, 1, data.PoliceNumber);
                    slDocument.SetCellValue(iRow, 2, data.Manufacture);
                    slDocument.SetCellValue(iRow, 3, data.Models);
                    slDocument.SetCellValue(iRow, 4, data.Series);
                    slDocument.SetCellValue(iRow, 5, data.BodyType);
                    slDocument.SetCellValue(iRow, 6, data.Color);
                    slDocument.SetCellValue(iRow, 7, data.GroupLevel.HasValue ? data.GroupLevel.Value.ToString() : "");
                    slDocument.SetCellValue(iRow, 8, data.StartContract.HasValue ? data.StartContract.Value.ToString("yyyy-MMM-dd") : "");
                    slDocument.SetCellValue(iRow, 9, data.EndContract.HasValue ? data.EndContract.Value.ToString("yyyy-MMM-dd") : "");
                    slDocument.SetCellValue(iRow, 10, data.Vendor);
                    slDocument.SetCellValue(iRow, 11, data.CostCenter);
                    slDocument.SetCellValue(iRow, 12, data.StartIdle.HasValue ? data.StartIdle.Value.ToString("yyyy-MMM-dd") : "");
                    slDocument.SetCellValue(iRow, 13, data.EndIdle.HasValue ? data.EndIdle.Value.ToString("yyyy-MMM-dd") : "");
                    slDocument.SetCellValue(iRow, 14, data.IdleDuration.HasValue ? data.IdleDuration.Value.ToString() : "");
                    slDocument.SetCellValue(iRow, 15, data.MonthlyInstallment.HasValue ? data.MonthlyInstallment.Value.ToString() : "");
                    slDocument.SetCellValue(iRow, 16, data.TotalMonthly.HasValue ? data.TotalMonthly.Value.ToString() : "");

                    SLStyle valueStyle = slDocument.CreateStyle();
                    valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

                    slDocument.AutoFitColumn(1, 16);
                    slDocument.SetCellStyle(iRow, 1, iRow , 16, valueStyle);
                    
                }
                else if (data.Note == "SubTotal")
                {
                    slDocument.SetCellValue(iRow, 1, "");
                    slDocument.SetCellValue(iRow, 2, "");
                    slDocument.SetCellValue(iRow, 3, "");
                    slDocument.SetCellValue(iRow, 4, "");
                    slDocument.SetCellValue(iRow, 5, "");
                    slDocument.SetCellValue(iRow, 6, "");
                    slDocument.SetCellValue(iRow, 7, "");
                    slDocument.SetCellValue(iRow, 8, "");
                    slDocument.SetCellValue(iRow, 9, "");
                    slDocument.SetCellValue(iRow, 10, "");
                    slDocument.SetCellValue(iRow, 11, "");
                    slDocument.SetCellValue(iRow, 12, "");
                    slDocument.SetCellValue(iRow, 13, "Total Idle Duration");
                    slDocument.SetCellValue(iRow, 14, data.IdleDuration.HasValue ? data.IdleDuration.Value.ToString() : "");
                    slDocument.SetCellValue(iRow, 15, "Total Installment");
                    slDocument.SetCellValue(iRow, 16, data.TotalMonthly.HasValue ? data.TotalMonthly.Value.ToString() : "");

                    SLStyle headerStyle = slDocument.CreateStyle();
                    headerStyle.Font.Bold = true;
                    headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

                    slDocument.SetCellStyle(iRow, 1, iRow, 16, headerStyle);
                }
                else if (data.Note == "GrandTotal")
                {
                    slDocument.SetCellValue(iRow, 1, data.PoliceNumber);
                    slDocument.SetCellValue(iRow, 2, "");
                    slDocument.SetCellValue(iRow, 3, "");
                    slDocument.SetCellValue(iRow, 4, "");
                    slDocument.SetCellValue(iRow, 5, "");
                    slDocument.SetCellValue(iRow, 6, "");
                    slDocument.SetCellValue(iRow, 7, "");
                    slDocument.SetCellValue(iRow, 8, "");
                    slDocument.SetCellValue(iRow, 9, "");
                    slDocument.SetCellValue(iRow, 10, "");
                    slDocument.SetCellValue(iRow, 11, "");
                    slDocument.SetCellValue(iRow, 12, "Grand Total");
                    slDocument.SetCellValue(iRow, 13, "Idle Duration");
                    slDocument.SetCellValue(iRow, 14, data.IdleDuration.HasValue ? data.IdleDuration.Value.ToString() : "");
                    slDocument.SetCellValue(iRow, 15, "Installment");
                    slDocument.SetCellValue(iRow, 16, data.TotalMonthly.HasValue ? data.TotalMonthly.Value.ToString() : "");

                    SLStyle headerStyle = slDocument.CreateStyle();
                    headerStyle.Font.Bold = true;
                    headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

                    slDocument.SetCellStyle(iRow, 1, iRow, 16, headerStyle);
                }
                   
                iRow++;
            }


            slDocument.AutoFitColumn(1, 16);
            return slDocument;
        }
        #endregion
    }
}
