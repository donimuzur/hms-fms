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
    public class RptPoController : BaseController
    {
        //
        // GET: /RptPo/
        #region --------- Field and Constructor --------------

        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private IRptPoBLL _rptPoBLL;
        private ISettingBLL _settingBLL;

        public RptPoController(IPageBLL pageBll, IRptPoBLL rptPoBLL, ISettingBLL SettingBLL) 
            : base(pageBll, Core.Enums.MenuList.RptPo)
        {
            _pageBLL = pageBll;
            _rptPoBLL = rptPoBLL;
            _settingBLL = SettingBLL;
            _mainMenu = Enums.MenuList.RptPo;
        }

        #endregion

        public ActionResult Index()
        {
            var model = new RptPOModel();
            //model.SearchView.PeriodFrom = Convert.ToDateTime("2013-07-15");
            //model.SearchView.PeriodTo = Convert.ToDateTime("2018-07-14");
            //model.SearchView.MonthFrom = 11;
            //model.SearchView.MonthTo = 12;
            //model.SearchView.PoliceNumber = "L1976HS";
            var input = Mapper.Map<RptPoByParamInput>(model.SearchView);
            var data = _rptPoBLL.GetRptPo(input);
            model.MainMenu = Enums.MenuList.RptExecutiveSummary;
            model.TitleForm = "PO Report";
            model.TitleExport = "ExportPO";
            model.CurrentLogin = CurrentUser;
            var settingData = _settingBLL.GetSetting();

            model.RptPOItem = Mapper.Map<List<RptPOItem>>(data);

            var listEmployee = _rptPoBLL.GetRptPoData().Select(x => new { x.EmployeeName }).Distinct().OrderBy(x => x.EmployeeName).ToList();
            var listCost = _rptPoBLL.GetRptPoData().Select(x => new { x.CostCenter }).Distinct().OrderBy(x => x.CostCenter).ToList();
            var listSM = _rptPoBLL.GetRptPoData().Select(x => new { x.SupplyMethod }).Distinct().OrderBy(x => x.SupplyMethod).ToList();
            
            model.SearchView.EmployeeNameList = new SelectList(listEmployee, "EmployeeName", "EmployeeName");
            model.SearchView.CostCenterList = new SelectList(listCost, "CostCenter", "CostCenter");
            model.SearchView.SupplyMethodList = new SelectList(listSM, "SupplyMethod", "SupplyMethod");

            return View(model);
        }

        [HttpPost]
        public PartialViewResult FilterPO(RptPOModel model)
        {
            //model.startMonth = 10;
            //model.startYear = 2017;
            //model.toMonth = 12;
            //model.toYear = 2017;
            
            model.RptPOItem = GetPOData(model.SearchView);
            var input = Mapper.Map<RptPoByParamInput>(model.SearchView);
            return PartialView("_ListPo", model);
        }

        private List<RptPOItem> GetPOData(RptPOSearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _rptPoBLL.GetRptPo(new RptPoByParamInput());
                return Mapper.Map<List<RptPOItem>>(data);
            }

            //getbyparams
            var input = Mapper.Map<RptPoByParamInput>(filter);

            var dbData = _rptPoBLL.GetRptPo(input);
            return Mapper.Map<List<RptPOItem>>(dbData);
        }

        #region --------- Export --------------

        public void ExportPO(RptPOModel model)
        {
            string pathFile = "";

            var input = Mapper.Map<RptPoByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsRptPO(input);

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

        private string CreateXlsRptPO(RptPoByParamInput input)
        {
            //get data
            List<RptPODto> data = _rptPoBLL.GetRptPo(input);
            var listData = Mapper.Map<List<RptPOItem>>(data);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "PO Report Data");
            slDocument.MergeWorksheetCells(1, 1, 1, 18);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelDashboard(slDocument, listData);

            //create data
            slDocument = CreateDataExcelDashboard(slDocument, listData);

            var fileName = "RptPO" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelDashboard(SLDocument slDocument, List<RptPOItem> listData)
        {
            int iRow = 3;
            int iCol = 18;
            slDocument.SetCellValue(iRow, 1, "Police Number");
            slDocument.SetCellValue(iRow, 2, "Supply Method");
            slDocument.SetCellValue(iRow, 3, "Employee Name");
            slDocument.SetCellValue(iRow, 4, "Cost Center");
            slDocument.SetCellValue(iRow, 5, "Manufacturer");
            slDocument.SetCellValue(iRow, 6, "Model");
            slDocument.SetCellValue(iRow, 7, "Series");
            slDocument.SetCellValue(iRow, 8, "Body Type");
            slDocument.SetCellValue(iRow, 9, "Color");
            slDocument.SetCellValue(iRow, 10, "Chasis Number");
            slDocument.SetCellValue(iRow, 11, "Engine Number");
            slDocument.SetCellValue(iRow, 12, "Vehicle Type");
            slDocument.SetCellValue(iRow, 13, "Vehicle Usage");
            slDocument.SetCellValue(iRow, 14, "PO Number");
            slDocument.SetCellValue(iRow, 15, "PO Line");
            slDocument.SetCellValue(iRow, 16, "Start Contract");
            slDocument.SetCellValue(iRow, 17, "End Contract");
            slDocument.SetCellValue(iRow, 18, "Vendor");
            foreach (var data in listData)
            {
                if (data.JanAmount > 0)
                {
                    slDocument.SetCellValue(2, iCol + 1, "Januari");
                    slDocument.MergeWorksheetCells(2, iCol + 1, 2, iCol + 3);

                    slDocument.SetCellValue(iRow, iCol + 1, "Amount");
                    slDocument.SetCellValue(iRow, iCol + 2, "PPN");
                    slDocument.SetCellValue(iRow, iCol + 3, "Total Amount");
                    iCol = iCol + 3;
                }
                if (data.PebAmount > 0)
                {
                    slDocument.SetCellValue(2, iCol + 1, "Pebruari");
                    slDocument.MergeWorksheetCells(2, iCol + 1, 2, iCol + 3);

                    slDocument.SetCellValue(iRow, iCol + 1, "Amount");
                    slDocument.SetCellValue(iRow, iCol + 2, "PPN");
                    slDocument.SetCellValue(iRow, iCol + 3, "Total Amount");
                    iCol = iCol + 3;
                }
                if (data.MarAmount > 0)
                {
                    slDocument.SetCellValue(2, iCol + 1, "Maret");
                    slDocument.MergeWorksheetCells(2, iCol + 1, 2, iCol + 3);

                    slDocument.SetCellValue(iRow, iCol + 1, "Amount");
                    slDocument.SetCellValue(iRow, iCol + 2, "PPN");
                    slDocument.SetCellValue(iRow, iCol + 3, "Total Amount");
                    iCol = iCol + 3;
                }
                if (data.AprAmount > 0)
                {
                    slDocument.SetCellValue(2, iCol + 1, "April");
                    slDocument.MergeWorksheetCells(2, iCol + 1, 2, iCol + 3);

                    slDocument.SetCellValue(iRow, iCol + 1, "Amount");
                    slDocument.SetCellValue(iRow, iCol + 2, "PPN");
                    slDocument.SetCellValue(iRow, iCol + 3, "Total Amount");
                    iCol = iCol + 3;
                }
                if (data.MeiAmount > 0)
                {
                    slDocument.SetCellValue(2, iCol + 1, "Mei");
                    slDocument.MergeWorksheetCells(2, iCol + 1, 2, iCol + 3);

                    slDocument.SetCellValue(iRow, iCol + 1, "Amount");
                    slDocument.SetCellValue(iRow, iCol + 2, "PPN");
                    slDocument.SetCellValue(iRow, iCol + 3, "Total Amount");
                    iCol = iCol + 3;
                }
                if (data.JunAmount > 0)
                {
                    slDocument.SetCellValue(2, iCol + 1, "Juni");
                    slDocument.MergeWorksheetCells(2, iCol + 1, 2, iCol + 3);

                    slDocument.SetCellValue(iRow, iCol + 1, "Amount");
                    slDocument.SetCellValue(iRow, iCol + 2, "PPN");
                    slDocument.SetCellValue(iRow, iCol + 3, "Total Amount");
                    iCol = iCol + 3;
                }
                if (data.JulAmount > 0)
                {
                    slDocument.SetCellValue(2, iCol + 1, "Juli");
                    slDocument.MergeWorksheetCells(2, iCol + 1, 2, iCol + 3);

                    slDocument.SetCellValue(iRow, iCol + 1, "Amount");
                    slDocument.SetCellValue(iRow, iCol + 2, "PPN");
                    slDocument.SetCellValue(iRow, iCol + 3, "Total Amount");
                    iCol = iCol + 3;
                }
                if (data.AgusAmount > 0)
                {
                    slDocument.SetCellValue(2, iCol + 1, "Agustus");
                    slDocument.MergeWorksheetCells(2, iCol + 1, 2, iCol + 3);

                    slDocument.SetCellValue(iRow, iCol + 1, "Amount");
                    slDocument.SetCellValue(iRow, iCol + 2, "PPN");
                    slDocument.SetCellValue(iRow, iCol + 3, "Total Amount");
                    iCol = iCol + 3;
                }
                if (data.SepAmount > 0)
                {
                    slDocument.SetCellValue(2, iCol + 1, "September");
                    slDocument.MergeWorksheetCells(2, iCol + 1, 2, iCol + 3);

                    slDocument.SetCellValue(iRow, iCol + 1, "Amount");
                    slDocument.SetCellValue(iRow, iCol + 2, "PPN");
                    slDocument.SetCellValue(iRow, iCol + 3, "Total Amount");
                    iCol = iCol + 3;
                }
                if (data.OktAmount > 0)
                {
                    slDocument.SetCellValue(2, iCol + 1, "Oktober");
                    slDocument.MergeWorksheetCells(2, iCol + 1, 2, iCol + 3);

                    slDocument.SetCellValue(iRow, iCol + 1, "Amount");
                    slDocument.SetCellValue(iRow, iCol + 2, "PPN");
                    slDocument.SetCellValue(iRow, iCol + 3, "Total Amount");
                    iCol = iCol + 3;
                }
                if (data.NopAmount > 0)
                {
                    slDocument.SetCellValue(2, iCol + 1, "Nopember");
                    slDocument.MergeWorksheetCells(2, iCol + 1, 2, iCol + 3);

                    slDocument.SetCellValue(iRow, iCol+1, "Amount");
                    slDocument.SetCellValue(iRow, iCol+2, "PPN");
                    slDocument.SetCellValue(iRow, iCol+3, "Total Amount");
                    iCol = iCol + 3;
                }
                if (data.DesAmount > 0)
                {
                    slDocument.SetCellValue(2, iCol + 1, "Desember");
                    slDocument.MergeWorksheetCells(2, iCol + 1, 2, iCol + 3);

                    slDocument.SetCellValue(iRow, iCol + 1, "Amount");
                    slDocument.SetCellValue(iRow, iCol + 2, "PPN");
                    slDocument.SetCellValue(iRow, iCol + 3, "Total Amount");
                    iCol = iCol + 3;
                }
            }

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, 2, iCol, headerStyle);
            slDocument.SetCellStyle(iRow, 1, iRow, iCol, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelDashboard(SLDocument slDocument, List<RptPOItem> listData)
        {
            int iRow = 4; //starting row data
            int iCol = 18;

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 2, data.SupplyMethod);
                slDocument.SetCellValue(iRow, 3, data.EmployeeName);
                slDocument.SetCellValue(iRow, 4, data.CostCenter);
                slDocument.SetCellValue(iRow, 5, data.Manufacturer);
                slDocument.SetCellValue(iRow, 6, data.Models);
                slDocument.SetCellValue(iRow, 7, data.Series);
                slDocument.SetCellValue(iRow, 8, data.BodyType);
                slDocument.SetCellValue(iRow, 9, data.Color);
                slDocument.SetCellValue(iRow, 10, data.ChasisNumber);
                slDocument.SetCellValue(iRow, 11, data.EngineNumber);
                slDocument.SetCellValue(iRow, 12, data.VehicleType);
                slDocument.SetCellValue(iRow, 13, data.VehicleUsage);
                slDocument.SetCellValue(iRow, 14, data.PoNumber);
                slDocument.SetCellValue(iRow, 15, data.PoLine);
                slDocument.SetCellValue(iRow, 16, data.StartContract.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 17, data.EndContract.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 18, data.Vendor);
                if (data.JanAmount > 0)
                {
                    slDocument.SetCellValue(iRow, iCol + 1, data.JanAmount);
                    slDocument.SetCellValue(iRow, iCol + 2, data.JanPPN);
                    slDocument.SetCellValue(iRow, iCol + 3, data.JanTotal);
                    iCol = iCol + 3;
                }
                if (data.PebAmount > 0)
                {
                    slDocument.SetCellValue(iRow, iCol + 1, data.PebAmount);
                    slDocument.SetCellValue(iRow, iCol + 2, data.PebPPN);
                    slDocument.SetCellValue(iRow, iCol + 3, data.PebTotal);
                    iCol = iCol + 3;
                }
                if (data.MarAmount > 0)
                {
                    slDocument.SetCellValue(iRow, iCol + 1, data.MarAmount);
                    slDocument.SetCellValue(iRow, iCol + 2, data.MarPPN);
                    slDocument.SetCellValue(iRow, iCol + 3, data.MarTotal);
                    iCol = iCol + 3;
                }
                if (data.AprAmount > 0)
                {
                    slDocument.SetCellValue(iRow, iCol + 1, data.AprAmount);
                    slDocument.SetCellValue(iRow, iCol + 2, data.AprPPN);
                    slDocument.SetCellValue(iRow, iCol + 3, data.AprTotal);
                    iCol = iCol + 3;
                }
                if (data.MeiAmount > 0)
                {
                    slDocument.SetCellValue(iRow, iCol + 1, data.MeiAmount);
                    slDocument.SetCellValue(iRow, iCol + 2, data.MeiPPN);
                    slDocument.SetCellValue(iRow, iCol + 3, data.MeiTotal);
                    iCol = iCol + 3;
                }
                if (data.JunAmount > 0)
                {
                    slDocument.SetCellValue(iRow, iCol + 1, data.JunAmount);
                    slDocument.SetCellValue(iRow, iCol + 2, data.JunPPN);
                    slDocument.SetCellValue(iRow, iCol + 3, data.JunTotal);
                    iCol = iCol + 3;
                }
                if (data.JulAmount > 0)
                {
                    slDocument.SetCellValue(iRow, iCol + 1, data.JulAmount);
                    slDocument.SetCellValue(iRow, iCol + 2, data.JulPPN);
                    slDocument.SetCellValue(iRow, iCol + 3, data.JulTotal);
                    iCol = iCol + 3;
                }
                if (data.AgusAmount > 0)
                {
                    slDocument.SetCellValue(iRow, iCol + 1, data.AgusAmount);
                    slDocument.SetCellValue(iRow, iCol + 2, data.AgusPPN);
                    slDocument.SetCellValue(iRow, iCol + 3, data.AgusTotal);
                    iCol = iCol + 3;
                }
                if (data.SepAmount > 0)
                {
                    slDocument.SetCellValue(iRow, iCol + 1, data.SepAmount);
                    slDocument.SetCellValue(iRow, iCol + 2, data.SepPPN);
                    slDocument.SetCellValue(iRow, iCol + 3, data.SepTotal);
                    iCol = iCol + 3;
                }
                if (data.OktAmount > 0)
                {
                    slDocument.SetCellValue(iRow, iCol + 1, data.OktAmount);
                    slDocument.SetCellValue(iRow, iCol + 2, data.OktPPN);
                    slDocument.SetCellValue(iRow, iCol + 3, data.OktTotal);
                    iCol = iCol + 3;
                }
                if (data.NopAmount > 0)
                {
                    slDocument.SetCellValue(iRow, iCol+1, data.NopAmount);
                    slDocument.SetCellValue(iRow, iCol+2, data.NopPPN);
                    slDocument.SetCellValue(iRow, iCol+3, data.NopTotal);
                    iCol = iCol + 3;
                }
                if (data.DesAmount > 0)
                {
                    slDocument.SetCellValue(iRow, iCol+1, data.DesAmount);
                    slDocument.SetCellValue(iRow, iCol+2, data.DesPPN);
                    slDocument.SetCellValue(iRow, iCol+3, data.DesTotal);
                    iCol = iCol + 3;
                }
                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, iCol);
            slDocument.SetCellStyle(3, 1, iRow - 1, iCol, valueStyle);

            return slDocument;
        }

        #endregion

    }
}
