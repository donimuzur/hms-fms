using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;
using SpreadsheetLight;

namespace FMS.Website.Controllers
{
    public class RptAutoGrController : BaseController
    {

        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private IAutoGrBLL _autoGrBLL;
        //
        // GET: /RptAutoGr/

        public RptAutoGrController(IPageBLL pageBll,IAutoGrBLL autoGrBLL) : base(pageBll, Enums.MenuList.RptAutoGr)
        {
            _pageBLL = pageBll;
            _autoGrBLL = autoGrBLL;
            _mainMenu = Enums.MenuList.RptExecutiveSummary;
        }

        public RptAutoGrModel InitModel(RptAutoGrModel model)
        {
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            model.MainMenu = _mainMenu;
            return model;
        }

        public ActionResult Index()
        {
            var rptModel = InitModel(new RptAutoGrModel());
            rptModel.PeriodStart = new DateTime(DateTime.Today.Year,
                DateTime.Today.Month,1);
            rptModel.PeriodEnd = new DateTime(DateTime.Today.Year,
                DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year,DateTime.Today.Month));

            var data = _autoGrBLL.GetAutoGR(new RptAutoGrInput());

            rptModel.Details = Mapper.Map<List<RptAutoGrItem>>(data);
            return View(rptModel);
        }

        [HttpPost]
        public ActionResult Index(RptAutoGrModel model)
        {
            model = InitModel(model);
            
            var data = _autoGrBLL.GetAutoGR(new RptAutoGrInput()
            {
                PeriodEnd = model.PeriodEnd,
                PeriodStart = model.PeriodStart
            });

            model.Details = Mapper.Map<List<RptAutoGrItem>>(data);



            return View(model);
        }


        public void ExportToExcel(RptAutoGrModel model)
        {
            

            string pathFile = "";

            pathFile = CreateXlsDashboard(model);

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

        private string CreateXlsDashboard(RptAutoGrModel model)
        {
            //get data
            var data = _autoGrBLL.GetAutoGR(new RptAutoGrInput()
            {
                PeriodEnd = model.PeriodEnd,
                PeriodStart = model.PeriodStart
            });

            var listData = Mapper.Map<List<RptAutoGrItem>>(data);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Dashboard CRF");
            slDocument.MergeWorksheetCells(1, 1, 1, 12);
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

            var fileName = "Auto_GR_Report" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelDashboard(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "PO Number");
            slDocument.SetCellValue(iRow, 2, "PO Line");
            slDocument.SetCellValue(iRow, 3, "GR Date");
            slDocument.SetCellValue(iRow, 4, "Police Number");
            slDocument.SetCellValue(iRow, 5, "Start Contract");
            slDocument.SetCellValue(iRow, 6, "End Contract");
            slDocument.SetCellValue(iRow, 7, "Termination Date");
            slDocument.SetCellValue(iRow, 8, "All Quantity");
            //slDocument.SetCellValue(iRow, 9, "CRF No");
            //slDocument.SetCellValue(iRow, 10, "CRF Status");
            slDocument.SetCellValue(iRow, 9, "Quantity Remaining");
            

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 9, headerStyle);

            return slDocument;

        }


        private SLDocument CreateDataExcelDashboard(SLDocument slDocument, List<RptAutoGrItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.PoNumber);
                slDocument.SetCellValue(iRow, 2, data.PoLine);
                slDocument.SetCellValue(iRow, 3, data.GrDateString);
                slDocument.SetCellValue(iRow, 4, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 5, data.StartContractString);
                slDocument.SetCellValue(iRow, 6, data.EndContractString);
                slDocument.SetCellValue(iRow, 7, data.TerminationDateString);
                slDocument.SetCellValue(iRow, 8, data.QtyAutoGr);
                //slDocument.SetCellValue(iRow, 9, data.);
                //slDocument.SetCellValue(iRow, 10, data.CRFStatus);
                slDocument.SetCellValue(iRow, 9, data.QtyRemaining);
                //slDocument.SetCellValue(iRow, 10, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 9);
            slDocument.SetCellStyle(3, 1, iRow - 1, 9, valueStyle);

            return slDocument;
        }
    }
}
