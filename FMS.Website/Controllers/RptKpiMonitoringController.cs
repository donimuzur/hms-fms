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
        private IRptFuelBLL _rptFuelBLL;
        private ISettingBLL _settingBLL;

        public RptKpiMonitoringController(IPageBLL pageBll, IRptFuelBLL rptFuelBLL, ISettingBLL SettingBLL)
            : base(pageBll, Core.Enums.MenuList.RptExecutiveSummary)
        {
            _pageBLL = pageBll;
            _rptFuelBLL = rptFuelBLL;
            _settingBLL = SettingBLL;
            _mainMenu = Enums.MenuList.RptExecutiveSummary;
        }

        public ActionResult Index()
        {
            FMSEntities fms = new FMSEntities();
            
            var model = new RptKpiMonitoringModel();
            model.MainMenu = _mainMenu;
            model.TitleForm = "Report Kpi Monitoring";
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            model.ReadAccess = CurrentPageAccess.ReadAccess == true ? 1 : 0;
            model.FormTyps = fms.KPI_REPORT_DATA.Select(x => x.FORM_TYPE).Where(x => x != null).Distinct().ToList();
            model.VehicleUsages = fms.KPI_REPORT_DATA.Select(x => x.VEHICLE_USAGE).Where(x => x != null).Distinct().ToList();
            var data = fms.KPI_REPORT_DATA.Where(x => x.FORM_TYPE != null).ToList();
            
            string formType = Request["formType"];
            if (!String.IsNullOrEmpty(formType))
                data = data.Where(x => x.FORM_TYPE.Equals(formType)).ToList();
            string effectiveDateFrom = Request["effectiveDateFrom"];
            string effectiveDateTo = Request["effectiveDateTo"];
            if (effectiveDateFrom != null)
            {
                DateTime enteredDateForm = DateTime.Parse(effectiveDateFrom);
                DateTime enteredDateTo = DateTime.Parse(effectiveDateTo);
                data = data.Where(x => (x.EFFECTIVE_DATE.Value.Year >= enteredDateForm.Year && x.EFFECTIVE_DATE.Value.Year <= enteredDateTo.Year) && (x.EFFECTIVE_DATE.Value.Month >= enteredDateForm.Month && x.EFFECTIVE_DATE.Value.Month <= enteredDateTo.Month) && (x.EFFECTIVE_DATE.Value.Day >= enteredDateForm.Day && x.EFFECTIVE_DATE.Value.Day <= enteredDateTo.Day)).ToList();
            }
            
            string vehicleUsage = Request["vehicleUsage"];
            if (!String.IsNullOrEmpty(vehicleUsage))
                data = data.Where(x => x.VEHICLE_USAGE == vehicleUsage).ToList();
            string location = Request["location"];
            if (location != null)
                data = data.Where(x => x.ADDRESS.Contains(location)).ToList();

            model.KpiReportDatas = data;
            return View(model);
        }

        public void ExportKpiMonitoring()
        {
            string pathFile = "";
            //gunakan ini jika export excel
            pathFile = CreateXlsKpiMonitoring();
            //gunakan ini jika export excel and graphic
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
        private string CreateXlsKpiMonitoring()
        {
            FMSEntities fms = new FMSEntities();
            var listData = fms.KPI_REPORT_DATA.Where(x => x.FORM_TYPE != null).ToList();

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Kpi Monitoring");
            slDocument.MergeWorksheetCells(1, 1, 1, 23);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelKpiMonitoring(slDocument);

            //create data
            slDocument = CreateDataExcelKpiMonitoring(slDocument, listData);

            var fileName = "Kpi Monitoring" + DateTime.Now.ToString(" yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private string createChart()
        {
            SLDocument sl = new SLDocument();

            sl.SetCellValue("C2", "Apple");
            sl.SetCellValue("D2", "Banana");
            sl.SetCellValue("E2", "Cherry");
            sl.SetCellValue("F2", "Durian");
            sl.SetCellValue("G2", "Elderberry");
            sl.SetCellValue("B3", "North");
            sl.SetCellValue("B4", "South");
            sl.SetCellValue("B5", "East");
            sl.SetCellValue("B6", "West");

            Random rand = new Random();
            for (int i = 3; i <= 6; ++i)
            {
                for (int j = 3; j <= 7; ++j)
                {
                    sl.SetCellValue(i, j, 9000 * rand.NextDouble() + 1000);
                }
            }

            SLChart chart = sl.CreateChart("B2", "G6");
            chart.SetChartStyle(SLChartStyle.Style1);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(7, 1, 22, 8.5);
            chart.PlotDataSeriesAsPrimaryLineChart(3, SLChartDataDisplayType.Normal, true);
            chart.PlotDataSeriesAsSecondaryLineChart(4, SLChartDataDisplayType.Normal, false);
            chart.PlotDataSeriesAsSecondaryLineChart(2, SLChartDataDisplayType.Normal, true);

            sl.InsertChart(chart);

            var fileName = "Kpi Monitoring" + DateTime.Now.ToString(" yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            sl.SaveAs(path);
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
            slDocument.SetCellValue(iRow, 15, "TEMPORARY REQUEST DATE");
            slDocument.SetCellValue(iRow, 16, "EE RECEIVED TEMP");
            slDocument.SetCellValue(iRow, 17, "SEND TO EMP DATE");
            slDocument.SetCellValue(iRow, 18, "SEND BACK TO HR");
            slDocument.SetCellValue(iRow, 19, "SEND TO FLEET DATE");
            slDocument.SetCellValue(iRow, 20, "SEND TO EMPLOYEE BENEFIT DATE");
            slDocument.SetCellValue(iRow, 21, "SEND SURAT KUASA");
            slDocument.SetCellValue(iRow, 22, "SEND AGREEMENT");
            slDocument.SetCellValue(iRow, 23, "REMARK");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 23, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelKpiMonitoring(SLDocument slDocument, List<KPI_REPORT_DATA> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.ID);
                slDocument.SetCellValue(iRow, 2, data.FORM_TYPE);
                slDocument.SetCellValue(iRow, 3, data.EMPLOYEE_ID);
                slDocument.SetCellValue(iRow, 4, data.EMPLOYEE_NAME);
                slDocument.SetCellValue(iRow, 5, data.EFFECTIVE_DATE.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 6, data.REASON);
                slDocument.SetCellValue(iRow, 7, data.ADDRESS);
                slDocument.SetCellValue(iRow, 8, data.PREVIOUS_BASE_TOWN);
                slDocument.SetCellValue(iRow, 9, data.NEW_BASE_TOWN);
                slDocument.SetCellValue(iRow, 10, data.VEHICLE_USAGE);
                slDocument.SetCellValue(iRow, 11, data.VEHICLE_GROUP_LEVEL.ToString());
                slDocument.SetCellValue(iRow, 12, data.VEHICLE_MODEL);
                slDocument.SetCellValue(iRow, 13, data.COLOR);
                slDocument.SetCellValue(iRow, 14, data.POLICE_NUMBER);
                slDocument.SetCellValue(iRow, 15, (data.TEMPORARY_REQUEST_DATE != null)?data.TEMPORARY_REQUEST_DATE.Value.ToString("dd-MMM-yyyy"):null);
                slDocument.SetCellValue(iRow, 16, (data.EE_RECEIVED_TEMP != null) ? data.EE_RECEIVED_TEMP.Value.ToString("dd-MMM-yyyy HH:mm:ss") : null);
                slDocument.SetCellValue(iRow, 17, (data.SEND_TO_EMP_DATE != null) ? data.SEND_TO_EMP_DATE.Value.ToString("dd-MMM-yyyy HH:mm:ss") : null);
                slDocument.SetCellValue(iRow, 18, (data.SEND_BACK_TO_HR != null) ? data.SEND_BACK_TO_HR.Value.ToString("dd-MMM-yyyy HH:mm:ss") : null);
                slDocument.SetCellValue(iRow, 19, (data.SEND_TO_FLEET_DATE != null) ? data.SEND_TO_FLEET_DATE.Value.ToString("dd-MMM-yyyy HH:mm:ss") : null);
                slDocument.SetCellValue(iRow, 20, (data.SEND_TO_EMPLOYEE_BENEFIT_DATE != null) ? data.SEND_TO_EMPLOYEE_BENEFIT_DATE.Value.ToString("dd-MMM-yyyy HH:mm:ss") : null);
                slDocument.SetCellValue(iRow, 21, (data.SEND_SURAT_KUASA != null) ? data.SEND_SURAT_KUASA.Value.ToString("dd-MMM-yyyy HH:mm:ss") : null);
                slDocument.SetCellValue(iRow, 22, (data.SEND_AGREEMENT != null) ? data.SEND_AGREEMENT.Value.ToString("dd-MMM-yyyy HH:mm:ss") : null);
                slDocument.SetCellValue(iRow, 23, data.REMARK);

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 11);
            slDocument.SetCellStyle(3, 1, iRow - 1, 23, valueStyle);

            return slDocument;
        }
    }
}
