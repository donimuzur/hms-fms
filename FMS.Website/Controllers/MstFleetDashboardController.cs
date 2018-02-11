using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using FMS.BLL.Vendor;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Utils;
using FMS.Website.Models;
using FMS.Website.Utility;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Controllers
{
    public class MstFleetDashboardController : BaseController
    {
        private IFleetBLL _fleetBLL;
        private IPageBLL _pageBLL;
        private Enums.MenuList _mainMenu;

        public MstFleetDashboardController(IPageBLL PageBll, IFleetBLL FleetBLL)
            : base(PageBll, Enums.MenuList.DashboardFleet)
        {
            _fleetBLL = FleetBLL;
            _pageBLL = PageBll;
            _mainMenu = Enums.MenuList.MasterData;
        }

        #region List View
        public ActionResult Index()
        {
            var model = new FleetDashboardModel();

            model.SearchView = new FleetDashboardSearchView();

            var fleetChangeList = _fleetBLL.GetFleetChange().ToList();

            model.SearchView.PoliceNumberList = new SelectList(fleetChangeList.Select(x => new { x.PoliceNumber }).Distinct().ToList(), "PoliceNumber", "PoliceNumber");
            model.SearchView.EmployeeNameList = new SelectList(fleetChangeList.Select(x => new { x.EmployeeName }).Distinct().ToList(), "EmployeeName", "EmployeeName");
            model.SearchView.ChasisNumberList = new SelectList(fleetChangeList.Select(x => new { x.ChasisNumber }).Distinct().ToList(), "ChasisNumber", "ChasisNumber");
            model.SearchView.EmployeeIDList = new SelectList(fleetChangeList.Select(x => new { x.EmployeeId }).Distinct().ToList(), "EmployeeId", "EmployeeId");
            
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            model.WriteAccess = CurrentPageAccess.WriteAccess == true ? 1 : 0;
            model.ReadAccess = CurrentPageAccess.ReadAccess == true ? 1 : 0;

            return View("Index", model);
        }

        [HttpPost]
        public JsonResult SearchFleetChangeAjax(DTParameters<FleetDashboardModel> param)
        {
            var model = param;

            var data = model != null ? SearchDataFleet(model) : SearchDataFleet();
            DTResult<FleetDashboardItem> result = new DTResult<FleetDashboardItem>();
            result.draw = param.Draw;
            result.recordsFiltered = data.Count;
            result.recordsTotal = data.Count;
            
            result.data = data;

            return Json(result);
        }

        private List<FleetDashboardItem> SearchDataFleet(DTParameters<FleetDashboardModel> searchView = null)
        {
            var param = new FleetChangeParamInput();
            param.EmployeeId = searchView.EmployeeId;
            param.FormalName = searchView.FormalName;
            param.PoliceNumber = searchView.PoliceNumber;

            var data = _fleetBLL.GetFleetChangeByParam(param);
            return Mapper.Map<List<FleetDashboardItem>>(data);
        }

        private List<FleetDashboardItem> SearchDataFleetExport(FleetDashboardSearchView searchView = null)
        {
            var param = new FleetChangeParamInput();
            param.EmployeeId = searchView.EmployeeId;
            param.FormalName = searchView.EmployeeName;
            param.PoliceNumber = searchView.PoliceNumber;

            var data = _fleetBLL.GetFleetChangeByParam(param);
            return Mapper.Map<List<FleetDashboardItem>>(data);
        }

        #endregion

        #region Update Data

        public ActionResult UpdateMasterFleet(string FleetChangeId)
        {
            bool isSuccess = false;
            try
            {
                isSuccess = _fleetBLL.UpdateFleetChange(FleetChangeId, CurrentUser);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            if (!isSuccess) return RedirectToAction("Index");
            AddMessageInfo("Success Update Master Fleet", Enums.MessageInfoType.Success);
            return RedirectToAction("Index");
        }

        #endregion

        #region Export xls

        public string ExportFleetChangeGenerateReport(FleetDashboardModel model = null)
        {
            string pathFile = "";
            pathFile = CreateXlsFleetChange(model);
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

        private string CreateXlsFleetChange(FleetDashboardModel model = null)
        {
            //get data
            //List<FleetChangeDto> fleetChange = _fleetBLL.GetFleetChange();
            var listData = SearchDataFleetExport(model.SearchView);
            //var listData = Mapper.Map<List<FleetItem>>(fleet);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Dashboard Fleet");
            slDocument.MergeWorksheetCells(1, 1, 1, 11);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelFleetChange(slDocument);

            //create data
            slDocument = CreateDataExcelFleetChange(slDocument, listData);

            var fileName = "Dashboard_Fleet" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelFleetChange(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Police Number");
            slDocument.SetCellValue(iRow, 2, "Chasis Number");
            slDocument.SetCellValue(iRow, 3, "Employee Id");
            slDocument.SetCellValue(iRow, 4, "Employee Name");
            slDocument.SetCellValue(iRow, 5, "Field Name");
            slDocument.SetCellValue(iRow, 6, "Data Before");
            slDocument.SetCellValue(iRow, 7, "Data After");
            slDocument.SetCellValue(iRow, 8, "Created Date");
            slDocument.SetCellValue(iRow, 9, "Notification Date");
            slDocument.SetCellValue(iRow, 10, "Modified By");
            slDocument.SetCellValue(iRow, 11, "Modified Date");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 11, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelFleetChange(SLDocument slDocument, List<FleetDashboardItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 2, data.ChasisNumber);
                slDocument.SetCellValue(iRow, 3, data.EmployeeId);
                slDocument.SetCellValue(iRow, 4, data.EmployeeName);
                slDocument.SetCellValue(iRow, 5, data.FieldName);
                slDocument.SetCellValue(iRow, 6, data.DataBefore);
                slDocument.SetCellValue(iRow, 7, data.DataAfter);
                slDocument.SetCellValue(iRow, 8, data.ChangeDate == null ? "" : data.ChangeDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 9, data.DateSend == null ? "" : data.DateSend.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 10, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 11, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;


            slDocument.AutoFitColumn(1, 11);
            slDocument.SetCellStyle(3, 1, iRow - 1, 11, valueStyle);

            return slDocument;
        }

        #endregion
    }
}
