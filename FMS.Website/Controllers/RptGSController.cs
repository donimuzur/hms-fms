using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Controllers
{
    public class RptGSController : BaseController
    {
        #region -------------- field and Cunstructor ------------------
        private IGsBLL _gsBLL;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private IRemarkBLL _remarkBLL;
        private IFleetBLL _fleetBLL;
        private IEmployeeBLL _employeeBLL;
        private ILocationMappingBLL _locationMappingBLL;
        private ISettingBLL _settingBLL;
        public RptGSController(IPageBLL pageBll, IGsBLL gsBLL, ISettingBLL settingBLL, IRemarkBLL RemarkBLL, IFleetBLL FleetBLL, IEmployeeBLL EmployeeBLL, ILocationMappingBLL LocationMapping) : base(pageBll, Enums.MenuList.MasterGS)
        {
            _gsBLL = gsBLL;
            _pageBLL = pageBll;
            _remarkBLL = RemarkBLL;
            _fleetBLL = FleetBLL;
            _employeeBLL = EmployeeBLL;
            _locationMappingBLL = LocationMapping;
            _settingBLL = settingBLL;
            _mainMenu = Enums.MenuList.RptExecutiveSummary;
        }
        #endregion
        //
        // GET: /RptGS/

        public ActionResult Index()
        {
            var model = new GsModel();
            var data = _gsBLL.GetGsReport(new RptGsInput());
            model.Details = Mapper.Map<List<GsItem>>(data);
            model.MainMenu = Enums.MenuList.RptGs;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;


            var settingList = _settingBLL.GetSetting().Where(x => x.SettingGroup.StartsWith("VEHICLE_USAGE")).Select(x => new { x.SettingName, x.SettingValue }).ToList();
            model.VehicleUsageList = new SelectList(settingList, "SettingName", "SettingValue");
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(GsModel model)
        {
            //var data = _gsBLL.GetGs();
            //model.Details = Mapper.Map<List<GsItem>>(data);
            model.MainMenu = Enums.MenuList.RptGs;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;

            List<GsDto> data = _gsBLL.GetGsReport(new RptGsInput()
            {
                StartDateBegin = model.FilterReport.StartDateBegin,
                EndDateBegin = model.FilterReport.EndDateBegin,
                StartDateEnd = model.FilterReport.StartDateEnd,
                EndDateEnd = model.FilterReport.EndDateEnd,
                Location = model.FilterReport.Location,
                VehicleUsage = model.FilterReport.VehicleUsage
            });

            model.Details = Mapper.Map<List<GsItem>>(data);
            var settingList = _settingBLL.GetSetting().Where(x => x.SettingGroup.StartsWith("VEHICLE_USAGE")).Select(x => new { x.SettingName, x.SettingValue }).ToList();
            model.VehicleUsageList = new SelectList(settingList, "SettingName", "SettingValue");
            return View(model);
        }

        #region export xls
        public string ExportGsReportGenerateReport(GsModel model = null)
        {
            string pathFile = "";
            pathFile = CreateXlsReportGs(model.FilterReport);
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

        private string CreateXlsReportGs(ReportFilter filter)
        {
            //get data
            List<GsDto> Gs = _gsBLL.GetGsReport(new RptGsInput()
            {
                EndDateBegin = filter.EndDateBegin,
                EndDateEnd = filter.EndDateEnd,
                StartDateBegin = filter.StartDateBegin,
                StartDateEnd = filter.StartDateEnd,
                Location = filter.Location,
                VehicleUsage = filter.VehicleUsage
            });
            var listData = Mapper.Map<List<GsItem>>(Gs);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Gs Report");
            slDocument.MergeWorksheetCells(1, 1, 1, 18);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelReportGs(slDocument);

            //create data
            slDocument = CreateDataExcelReportGs(slDocument, listData);

            var fileName = "Gs_Report" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }


        private SLDocument CreateHeaderExcelReportGs(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Employee Name");
            slDocument.SetCellValue(iRow, 2, "Vehicle Usage");
            slDocument.SetCellValue(iRow, 3, "Police Number");
            slDocument.SetCellValue(iRow, 4, "Group Level");
            slDocument.SetCellValue(iRow, 5, "Location");
            slDocument.SetCellValue(iRow, 6, "Gs Request Date");
            slDocument.SetCellValue(iRow, 7, "Gs Fullfillment Date");
            slDocument.SetCellValue(iRow, 8, "Gs Manufacturer");
            slDocument.SetCellValue(iRow, 9, "Gs Model");
            slDocument.SetCellValue(iRow, 10, "Gs Series");
            slDocument.SetCellValue(iRow, 11, "Gs Transmission");
            slDocument.SetCellValue(iRow, 12, "Gs Police Number");
            slDocument.SetCellValue(iRow, 13, "Start Date");
            slDocument.SetCellValue(iRow, 14, "End Date");
            slDocument.SetCellValue(iRow, 15, "Lead Time");
            slDocument.SetCellValue(iRow, 16, "KPI Fulfillment");
            slDocument.SetCellValue(iRow, 17, "Rent Time");
            slDocument.SetCellValue(iRow, 18, "Remark");


            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 18, headerStyle);

            return slDocument;
        }

        private SLDocument CreateDataExcelReportGs(SLDocument slDocument, List<GsItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.EmployeeName);
                slDocument.SetCellValue(iRow, 2, data.VehicleUsage);
                slDocument.SetCellValue(iRow, 3, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 4, data.GroupLevel == null ? "" : data.GroupLevel.ToString());
                slDocument.SetCellValue(iRow, 5, data.Location);
                slDocument.SetCellValue(iRow, 6, data.GsRequestDate == null ? "" : data.GsRequestDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 7, data.GsFullfillmentDate == null ? "" : data.GsFullfillmentDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 8, data.GsManufacturer);
                slDocument.SetCellValue(iRow, 9, data.GsModel);
                slDocument.SetCellValue(iRow, 10, data.GsSeries);
                slDocument.SetCellValue(iRow, 11, data.GsTransmission);
                slDocument.SetCellValue(iRow, 12, data.GsPoliceNumber);
                slDocument.SetCellValue(iRow, 13, data.StartDate == null ? "" : data.StartDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 14, data.EndDate == null ? "" : data.EndDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 15, data.LeadTimeS);
                slDocument.SetCellValue(iRow, 16, data.KpiFulfillment);
                slDocument.SetCellValue(iRow, 17, data.RentTime);
                slDocument.SetCellValue(iRow, 18, data.Remark);

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 18);
            slDocument.SetCellStyle(3, 1, iRow - 1, 18, valueStyle);

            return slDocument;


        }
        #endregion

    }
}
