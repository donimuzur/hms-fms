using System;
using System.Collections.Generic;
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
using FMS.DAL;
using FMS.BusinessObject;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using SpreadsheetLight;
using SpreadsheetLight.Charts;
using System.Web.Helpers;

namespace FMS.Website.Controllers
{
    public class RptKpiMonitoringController : BaseController
    {
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private IKpiMonitoringBLL _kpiMonitoringBLL;
        private ISettingBLL _settingBLL;

        public RptKpiMonitoringController(IPageBLL pageBll, IKpiMonitoringBLL KpiMonitoringBLL, ISettingBLL SettingBLL)
            : base(pageBll, Core.Enums.MenuList.RptKpiMonitoring)
        {
            _pageBLL = pageBll;
            _kpiMonitoringBLL = KpiMonitoringBLL;
            _settingBLL = SettingBLL;
            _mainMenu = Enums.MenuList.RptExecutiveSummary;
        }

        public ActionResult Index()
        {
            var model = new RptKpiMonitoringModel();
            var filter = new KpiMonitoringGetByParamInput();
            
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;

            model.SearchView.FormDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            model.SearchView.ToDate = DateTime.Today;

            filter.FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            filter.ToDate = DateTime.Today;
            try
            {
                var ListTransaction = _kpiMonitoringBLL.GetTransaction(filter);
                model.ListTransaction = Mapper.Map<List<KpiMonitoringItem>>(ListTransaction);
            }
            catch (Exception exp)
            {

                model.ErrorMessage = exp.Message;
            }
           
            return View(model);
        }

        private List<KpiMonitoringItem> GetTransaction(KpiReportSearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _kpiMonitoringBLL.GetTransaction(new KpiMonitoringGetByParamInput());
                return Mapper.Map<List<KpiMonitoringItem>>(data);
            }

            //getbyparams
            var input = Mapper.Map<KpiMonitoringGetByParamInput>(filter);

            var dbData = _kpiMonitoringBLL.GetTransaction(input);
            return Mapper.Map<List<KpiMonitoringItem>>(dbData);
        }

        [HttpPost]
        public PartialViewResult ListTransaction(RptKpiMonitoringModel model)
        {
            model.ListTransaction = GetTransaction(model.SearchView);
            return PartialView("_ListTransaction", model);
        }
        public void ExportKpiMonitoring(RptKpiMonitoringModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsKpiMonitoring(model);

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
        private string CreateXlsKpiMonitoring(RptKpiMonitoringModel model)
        {

            var data = GetTransaction(model.SearchView);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "KPI MONITORING");
            slDocument.MergeWorksheetCells(1, 1, 1, 19);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelKpiMonitoring(slDocument);

            //create data
            slDocument = CreateDataExcelKpiMonitoring(slDocument, data);

            var fileName = "Kpi Monitoring" + DateTime.Now.ToString(" yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }
        
        private SLDocument CreateHeaderExcelKpiMonitoring(SLDocument slDocument)
        {
            int iRow = 2;
            slDocument.SetCellValue(iRow, 1, "ID");
            slDocument.SetCellValue(iRow, 2, "FORM TYPE");
            slDocument.SetCellValue(iRow, 3, "EMPLOYEE ID");
            slDocument.SetCellValue(iRow, 4, "EMPLOYEE NAME");
            slDocument.SetCellValue(iRow, 5, "EFFECTIVE DATE");
            slDocument.SetCellValue(iRow, 6, "REASON");
            slDocument.SetCellValue(iRow, 7, "ADDRESS");
            slDocument.SetCellValue(iRow, 8, "PREVIOUS BASE TOWN");
            slDocument.SetCellValue(iRow, 9, "NEW BASE TOWN");
            slDocument.SetCellValue(iRow, 10, "VEHICLE USAGE");
            slDocument.SetCellValue(iRow, 11, "VEHICLE GROUP LEVEL");
            slDocument.SetCellValue(iRow, 12, "VEHICLE MODEL");
            slDocument.SetCellValue(iRow, 13, "COLOR");
            slDocument.SetCellValue(iRow, 14, "POLICE NUMBER");
            slDocument.SetCellValue(iRow, 15, "SEND TO EMP DATE");
            slDocument.SetCellValue(iRow, 16, "SEND BACK TO HR");
            slDocument.SetCellValue(iRow, 17, "SEND TO FLEET DATE");
            slDocument.SetCellValue(iRow, 18, "SEND TO EMPLOYEE BENEFIT DATE");
            slDocument.SetCellValue(iRow, 19, "REMARK");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 19, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelKpiMonitoring(SLDocument slDocument, List<KpiMonitoringItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.Id);
                slDocument.SetCellValue(iRow, 2, data.FormType);
                slDocument.SetCellValue(iRow, 3, data.EmployeeId);
                slDocument.SetCellValue(iRow, 4, data.EmployeeName);
                slDocument.SetCellValue(iRow, 5, data.EffectiveDate == null ? "":data.EffectiveDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 6, data.Reason);
                slDocument.SetCellValue(iRow, 7, data.Address);
                slDocument.SetCellValue(iRow, 8, data.PreviousBaseTown);
                slDocument.SetCellValue(iRow, 9, data.NewBaseTown);
                slDocument.SetCellValue(iRow, 10, data.VehicleUsage);
                slDocument.SetCellValue(iRow, 11, data.VehicleGroup == null ? "" : data.VehicleGroup.ToString());
                slDocument.SetCellValue(iRow, 12, data.Model);
                slDocument.SetCellValue(iRow, 13, data.Color);
                slDocument.SetCellValue(iRow, 14, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 15, data.SendToEmpDate == null ?"": data.SendToEmpDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 16, data.SendBackToHr == null ? "":data.SendBackToHr.Value.ToString("dd-MMM-yyyy") );
                slDocument.SetCellValue(iRow, 17, data.SendToFleetDate == null ?"": data.SendToFleetDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 18, data.SendToEmpBenefit == null ? "":data.SendToEmpBenefit.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 19, data.Remark);

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 11);
            slDocument.SetCellStyle(3, 1, iRow - 1, 19, valueStyle);

            return slDocument;
        }
    }
}
