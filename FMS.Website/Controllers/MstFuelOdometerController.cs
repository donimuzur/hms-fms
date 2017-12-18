using AutoMapper;
using FMS.BusinessObject;
using FMS.Contract.BLL;
using FMS.BusinessObject.Dto;
using FMS.Core;
using FMS.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;
using FMS.Website.Utility;
using FMS.BusinessObject.Inputs;
using FMS.Utils;

namespace FMS.Website.Controllers
{
    public class MstFuelOdometerController : BaseController
    {
        private IFuelOdometerBLL _fuelodometerBLL;
        private IFleetBLL _fleetBLL;
        private ISettingBLL _settingBLL;
        private IGroupCostCenterBLL _groupCostCenterBLL;
        private Enums.MenuList _mainMenu;

        public MstFuelOdometerController(IPageBLL PageBll, IFuelOdometerBLL FuelOdometerBLL, IFleetBLL FleetBLL, ISettingBLL SettingBLL, IGroupCostCenterBLL GroupCostCenterBLL) : base(PageBll, Enums.MenuList.MasterFuelOdoMeter)
        {
            _fuelodometerBLL = FuelOdometerBLL;
            _fleetBLL = FleetBLL;
            _settingBLL = SettingBLL;
            _groupCostCenterBLL = GroupCostCenterBLL;
            _mainMenu = Enums.MenuList.MasterData;
        }

        //
        // GET: /MstFuelOdometer/

        public ActionResult Index()
        {
            var model = new FuelOdometerModel();
            //model.Details = Mapper.Map<List<FuelOdometerItem>>(data);
            model.SearchView = new FuelOdometerSearchView();
            var fleetList = _fleetBLL.GetFleet().ToList();
            var costCenterList = _groupCostCenterBLL.GetGroupCenter().ToList();
            var fuelOdometerList = _fuelodometerBLL.GetFuelOdometer().ToList();
            var listVehType = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.IsActive).Select(x => new { x.MstSettingId, x.SettingValue }).ToList();

            model.SearchView.PoliceNumberList = new SelectList(fleetList, "PoliceNumber", "PoliceNumber");
            model.SearchView.EmployeeNameList = new SelectList(fleetList, "EmployeeName", "EmployeeName");
            model.SearchView.EmployeeIDList = new SelectList(fleetList, "EmployeeID", "EmployeeID");
            model.SearchView.CostCenterList = new SelectList(costCenterList, "CostCenter", "CostCenter");
            model.SearchView.EcsRmbTransIdList = new SelectList(fuelOdometerList, "EcsRmbTransId", "EcsRmbTransId");
            model.SearchView.ClaimTypeList = new SelectList(fuelOdometerList, "ClaimType", "ClaimType");
            model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View(model);
        }

        public ActionResult Detail(long MstFuelOdometerId)
        {
            var data = _fuelodometerBLL.GetByID(MstFuelOdometerId);
            var model = new FuelOdometerItem();
            model = Mapper.Map<FuelOdometerItem>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        #region Filters
        [HttpPost]
        public JsonResult SearchFuelOdoMeterAjax(DTParameters<FuelOdometerModel> param)
        {
            var model = param;

            var data = model != null ? SearchDataFuelOdometer(model) : SearchDataFuelOdometer();
            DTResult<FuelOdometerItem> result = new DTResult<FuelOdometerItem>();
            result.draw = param.Draw;
            result.recordsFiltered = data.Count;
            result.recordsTotal = data.Count;
            //param.TotalData = data.Count;
            //if (param != null && param.Start > 0)
            //{
            IEnumerable<FuelOdometerItem> dataordered;
            dataordered = data;
            if (param.Order.Length > 0)
            {
                foreach (var ordr in param.Order)
                {
                    if (ordr.Column == 0)
                    {
                        continue;
                    }
                    dataordered = FuelOdometerDataOrder(FuelOdometerDataOrderByIndex(ordr.Column), ordr.Dir, dataordered);
                }
            }
            data = dataordered.ToList();
            data = data.Skip(param.Start).Take(param.Length).ToList();

            //}
            result.data = data;

            return Json(result);
        }

        private List<FuelOdometerItem> SearchDataFuelOdometer(DTParameters<FuelOdometerModel> searchView = null)
        {
            var param = new FuelOdometerParamInput();
            param.Status = searchView.StatusSource;
            param.VehicleType = searchView.VehicleType;
            param.DateOfCost = searchView.DateOfCost;
            param.PostedTime = searchView.PostedTime;
            param.EmployeeId = searchView.EmployeeId;
            param.EmployeeName = searchView.EmployeeName;
            param.PoliceNumber = searchView.PoliceNumber;
            param.SeqNumber = searchView.SeqNumber;
            param.LastKM = searchView.LastKM;
            param.ClaimComment = searchView.ClaimComment;
            param.ClaimType = searchView.ClaimType;
            param.CostCenter = searchView.CostCenter;
            param.EcsRmbTransId = searchView.EcsRmbTransId;

            var data = _fuelodometerBLL.GetFuelOdometerByParam(param);
            return Mapper.Map<List<FuelOdometerItem>>(data);
        }

        private IEnumerable<FuelOdometerItem> FuelOdometerDataOrder(string column, DTOrderDir dir, IEnumerable<FuelOdometerItem> data)
        {

            switch (column)
            {
                case "VehicleType": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.VehicleType).ToList() : data.OrderByDescending(x => x.VehicleType).ToList();
                case "LastKM": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.LastKM).ToList() : data.OrderByDescending(x => x.LastKM).ToList();
                case "CostCenter": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.CostCenter).ToList() : data.OrderByDescending(x => x.CostCenter).ToList();
                case "EcsRmbTransId": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.EcsRmbTransId).ToList() : data.OrderByDescending(x => x.EcsRmbTransId).ToList();

            }
            return null;
        }

        private string FuelOdometerDataOrderByIndex(int index)
        {
            Dictionary<int, string> columnDict = new Dictionary<int, string>();
            columnDict.Add(17, "VehicleType");
            columnDict.Add(18, "LastKM");
            columnDict.Add(24, "CostCenter");
            columnDict.Add(9, "EcsRmbTransId");


            return columnDict[index];
        }
        #endregion

        #region ExportExcel
        public void ExportMasterFuelOdometer()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterFuelOdometer();

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

        private string CreateXlsMasterFuelOdometer()
        {
            //get data
            List<FuelOdometerDto> vendor = _fuelodometerBLL.GetFuelOdometer();
            var listData = Mapper.Map<List<FuelOdometerItem>>(vendor);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Fuel & Odometer");
            slDocument.MergeWorksheetCells(1, 1, 1, 19);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterFuelOdometer(slDocument);

            //create data
            slDocument = CreateDataExcelMasterFuelOdometer(slDocument, listData);

            var fileName = "Master Data Fuel and Odometer " + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterFuelOdometer(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Vehicle Type");
            slDocument.SetCellValue(iRow, 2, "Police Number");
            slDocument.SetCellValue(iRow, 3, "Employee ID");
            slDocument.SetCellValue(iRow, 4, "Employee Name");
            slDocument.SetCellValue(iRow, 5, "ECS RMB Trans ID");
            slDocument.SetCellValue(iRow, 6, "SEQ Number");
            slDocument.SetCellValue(iRow, 7, "Claim Type");
            slDocument.SetCellValue(iRow, 8, "Date Of Cost");
            slDocument.SetCellValue(iRow, 9, "Cost Center");
            slDocument.SetCellValue(iRow, 10, "Fuel Amount");
            slDocument.SetCellValue(iRow, 11, "Last KM");
            slDocument.SetCellValue(iRow, 12, "Cost");
            slDocument.SetCellValue(iRow, 13, "Claim Comment");
            slDocument.SetCellValue(iRow, 14, "Posted Time");
            slDocument.SetCellValue(iRow, 15, "Created By");
            slDocument.SetCellValue(iRow, 16, "Created Date");
            slDocument.SetCellValue(iRow, 17, "Modified By");
            slDocument.SetCellValue(iRow, 18, "Modified Date");
            slDocument.SetCellValue(iRow, 19, "Status");

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

        private SLDocument CreateDataExcelMasterFuelOdometer(SLDocument slDocument, List<FuelOdometerItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.VehicleType);
                slDocument.SetCellValue(iRow, 2, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 3, data.EmployeeId);
                slDocument.SetCellValue(iRow, 4, data.EmployeeName);
                slDocument.SetCellValue(iRow, 5, data.EcsRmbTransId);
                slDocument.SetCellValue(iRow, 6, data.SeqNumber);
                slDocument.SetCellValue(iRow, 7, data.ClaimType);
                slDocument.SetCellValue(iRow, 8, data.DateOfCost);
                slDocument.SetCellValue(iRow, 9, data.CostCenter);
                slDocument.SetCellValue(iRow, 10, data.FuelAmount);
                slDocument.SetCellValue(iRow, 11, data.LastKM);
                slDocument.SetCellValue(iRow, 12, data.Cost);
                slDocument.SetCellValue(iRow, 13, data.ClaimComment);
                slDocument.SetCellValue(iRow, 14, data.PostedTime.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 15, data.CreatedBy);
                slDocument.SetCellValue(iRow, 16, data.CreatedDate.ToString("dd-MMM-yyyy HH:mm:ss"));
                slDocument.SetCellValue(iRow, 17, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 18, data == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy HH:mm:ss"));
                if (data.IsActive)
                {
                    slDocument.SetCellValue(iRow, 19, "Active");
                }
                else
                {
                    slDocument.SetCellValue(iRow, 19, "InActive");
                }

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 8);
            slDocument.SetCellStyle(3, 1, iRow - 1, 19, valueStyle);

            return slDocument;
        }
        #endregion

    }
}
