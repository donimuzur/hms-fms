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
    public class RptCcfController : BaseController
    {
        //
        // GET: /RptCcf/

        #region --------- Field and Constructor --------------

        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private IRptCcfBLL _rptCcfBLL;
        private IComplaintCategoryBLL _rptComplaintBLL;
        private ISettingBLL _settingBLL;

        public RptCcfController(IPageBLL pageBll, IRptCcfBLL rptCcfBLL, ISettingBLL SettingBLL, IComplaintCategoryBLL rptComplaintBLL) 
            : base(pageBll, Core.Enums.MenuList.RptCcf)
        {
            _pageBLL = pageBll;
            _rptCcfBLL = rptCcfBLL;
            _rptComplaintBLL = rptComplaintBLL;
            _settingBLL = SettingBLL;
            _mainMenu = Enums.MenuList.RptCcf;
        }

        #endregion

        public ActionResult Index()
        {
            var model = new RptCCFModel();
            var input = Mapper.Map<RptCCFInput>(model.SearchView);
            var data = _rptCcfBLL.GetRptCcf(input);
            model.MainMenu = Enums.MenuList.RptExecutiveSummary;
            model.TitleForm = "CCF Report";
            model.TitleExport = "ExportCCF";
            model.CurrentLogin = CurrentUser;
            var settingData = _settingBLL.GetSetting();

            model.RptCCFItem = Mapper.Map<List<RptCCFItem>>(data);

            var listCategory = _rptComplaintBLL.GetComplaints().Select(x => new { x.MstComplaintCategoryId,x.CategoryName }).Distinct().OrderBy(x => x.MstComplaintCategoryId).ToList();
            var listCoordinator= _rptCcfBLL.GetRptCcfData().Select(x => new { x.CoordinatorName }).Distinct().OrderBy(x => x.CoordinatorName).ToList();
            var listLocation = _rptCcfBLL.GetRptCcfData().Select(x => new { x.LocationCity }).Distinct().OrderBy(x => x.LocationCity).ToList();

            model.SearchView.Categorylist = new SelectList(listCategory, "MstComplaintCategoryId", "CategoryName");
            model.SearchView.Coordinatorlist = new SelectList(listCoordinator, "CoordinatorName", "CoordinatorName");
            model.SearchView.Locationlist = new SelectList(listLocation, "LocationCity", "LocationCity");

            return View(model);
        }

        [HttpPost]
        public PartialViewResult FilterCCF(RptCCFModel model)
        {
            model.RptCCFItem = GetCCFData(model.SearchView);
            var input = Mapper.Map<RptCCFInput>(model.SearchView);
            return PartialView("_ListCcf", model);
        }

        private List<RptCCFItem> GetCCFData(RptCCFSearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _rptCcfBLL.GetRptCcf(new RptCCFInput());
                return Mapper.Map<List<RptCCFItem>>(data);
            }

            //getbyparams
            var input = Mapper.Map<RptCCFInput>(filter);

            var dbData = _rptCcfBLL.GetRptCcf(input);
            return Mapper.Map<List<RptCCFItem>>(dbData);
        }

        #region --------- Export --------------

        public void ExportCCF(RptCCFModel model)
        {
            string pathFile = "";

            var input = Mapper.Map<RptCCFInput>(model.SearchViewExport);
            pathFile = CreateXlsRptCCF(input);

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

        private string CreateXlsRptCCF(RptCCFInput input)
        {
            //get data
            List<RptCCFDto> data = _rptCcfBLL.GetRptCcf(input);
            var listData = Mapper.Map<List<RptCCFItem>>(data);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "CCF Report Data");
            slDocument.MergeWorksheetCells(1, 1, 1, 13);
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

            var fileName = "RptCCF" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelDashboard(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Complaint Number");
            slDocument.SetCellValue(iRow, 2, "Police Number");
            slDocument.SetCellValue(iRow, 3, "Location City");
            slDocument.SetCellValue(iRow, 4, "Category");
            slDocument.SetCellValue(iRow, 5, "Coordinator");
            slDocument.SetCellValue(iRow, 6, "Coordinator Name");
            slDocument.SetCellValue(iRow, 7, "Created By");
            slDocument.SetCellValue(iRow, 8, "Created Date");
            slDocument.SetCellValue(iRow, 9, "Close Date");
            slDocument.SetCellValue(iRow, 10, "Status");
            slDocument.SetCellValue(iRow, 11, "Vendor Name");
            slDocument.SetCellValue(iRow, 12, "Late Respon Time Coordinator");
            slDocument.SetCellValue(iRow, 13, "Late Respon Time Vendor");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 13, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelDashboard(SLDocument slDocument, List<RptCCFItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.DocumentNumber);
                slDocument.SetCellValue(iRow, 2, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 3, data.LocationCity);
                slDocument.SetCellValue(iRow, 4, data.ComplaintCategoryName);
                slDocument.SetCellValue(iRow, 5, data.ComplaintCategoryRole);
                slDocument.SetCellValue(iRow, 6, data.CoordinatorName);
                slDocument.SetCellValue(iRow, 7, data.CreatedBy);
                slDocument.SetCellValue(iRow, 8, data.CreatedDate);
                slDocument.SetCellValue(iRow, 9, data.ModifiedDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 10, data.DocumentStatus.ToString());
                slDocument.SetCellValue(iRow, 11, data.Vendor);
                slDocument.SetCellValue(iRow, 12, data.CoordinatorKPI);
                slDocument.SetCellValue(iRow, 13, data.VendorKPI);

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 13);
            slDocument.SetCellStyle(3, 1, iRow - 1, 13, valueStyle);

            return slDocument;
        }

        #endregion
    }
}
