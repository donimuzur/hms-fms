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
        private IDocumentTypeBLL _documentTypeBLL;

        public MstFuelOdometerController(IPageBLL PageBll, IFuelOdometerBLL FuelOdometerBLL, IFleetBLL FleetBLL, ISettingBLL SettingBLL, IGroupCostCenterBLL GroupCostCenterBLL, IDocumentTypeBLL DocTypeBLL)
            : base(PageBll, Enums.MenuList.MasterFuelOdoMeter)
        {
            _fuelodometerBLL = FuelOdometerBLL;
            _fleetBLL = FleetBLL;
            _settingBLL = SettingBLL;
            _groupCostCenterBLL = GroupCostCenterBLL;
            _mainMenu = Enums.MenuList.MasterData;
            _documentTypeBLL = DocTypeBLL;
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

            model.SearchView.PoliceNumberList = new SelectList(fleetList.Select(x => new { x.PoliceNumber }).Distinct().ToList(), "PoliceNumber", "PoliceNumber");
            model.SearchView.EmployeeNameList = new SelectList(fleetList.Select(x => new { x.EmployeeName }).Distinct().ToList(), "EmployeeName", "EmployeeName");
            model.SearchView.EmployeeIDList = new SelectList(fleetList.Select(x => new { x.EmployeeID }).Distinct().ToList(), "EmployeeID", "EmployeeID");
            model.SearchView.CostCenterList = new SelectList(costCenterList.Select(x => new { x.CostCenter }).Distinct().ToList(), "CostCenter", "CostCenter");
            model.SearchView.EcsRmbTransIdList = new SelectList(fuelOdometerList.Select(x => new { x.EcsRmbTransId }).Distinct().ToList(), "EcsRmbTransId", "EcsRmbTransId");
            model.SearchView.ClaimTypeList = new SelectList(fuelOdometerList.Select(x => new { x.ClaimType }).Distinct().ToList(), "ClaimType", "ClaimType");
            model.SearchView.VehicleTypeList = new SelectList(listVehType.Select(x => new { x.SettingValue }).Distinct().ToList(), "SettingValue", "SettingValue");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View(model);
        }

        public ActionResult Upload()
        {
            var model = new FuelOdometerModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
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


        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var qtyPacked = string.Empty;
            var qty = string.Empty;

            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<FuelOdometerItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    var item = new FuelOdometerItem();
                    item.ErrorMessage = "";
                    try
                    {
                        item.VehicleType = dataRow[0].ToString();
                        item.PoliceNumber = dataRow[1].ToString();
                        item.EmployeeId = dataRow[2].ToString();
                        item.EmployeeName = dataRow[3].ToString();
                        item.EcsRmbTransId = Convert.ToInt32(dataRow[4]);
                        item.SeqNumber = Convert.ToInt32(dataRow[5]);
                        item.ClaimType = dataRow[6].ToString();
                        item.DateOfCost = Convert.ToDateTime(dataRow[7].ToString());
                        item.CostCenter = dataRow[8].ToString();
                        item.LastKM = Convert.ToInt32(dataRow[9]);
                        item.Cost = Convert.ToInt32(dataRow[10]);
                        item.ClaimComment = dataRow[12].ToString();

                        item.PostedTime = DateTime.Now;

                        item.IsActive = dataRow[13].ToString() == "Active" ? true : false;
                      
                        item.ErrorMessage = string.Empty;

                        //var exist = _fuelodometerBLL.GetFuelOdometer().Where(x => x.SettingGroup.ToUpper() == item.SettingGroup.ToUpper()
                        //    && x.SettingName.ToUpper() == item.SettingName.ToUpper()
                        //    && x.SettingValue.ToUpper() == item.SettingValue.ToUpper()
                        //   && x.IsActive).FirstOrDefault();

                        if (string.IsNullOrEmpty(dataRow[0].ToString())) { item.ErrorMessage += "Setting Group empty,"; }
                        if (string.IsNullOrEmpty(dataRow[1].ToString())) { item.ErrorMessage += "Setting Name empty,"; }
                        if (string.IsNullOrEmpty(dataRow[2].ToString())) { item.ErrorMessage += "Setting Value empty,"; }
                        if (string.IsNullOrEmpty(dataRow[3].ToString())) { item.ErrorMessage += "Setting Value empty,"; }
                        if (string.IsNullOrEmpty(dataRow[4].ToString())) { item.ErrorMessage += "Setting Value empty,"; }
                        if (string.IsNullOrEmpty(dataRow[5].ToString())) { item.ErrorMessage += "Setting Value empty,"; }
                        if (string.IsNullOrEmpty(dataRow[6].ToString())) { item.ErrorMessage += "Setting Value empty,"; }
                        if (string.IsNullOrEmpty(dataRow[7].ToString())) { item.ErrorMessage += "Setting Value empty,"; }
                        if (string.IsNullOrEmpty(dataRow[8].ToString())) { item.ErrorMessage += "Setting Value empty,"; }
                        if (string.IsNullOrEmpty(dataRow[9].ToString())) { item.ErrorMessage += "Setting Value empty,"; }
                        if (string.IsNullOrEmpty(dataRow[10].ToString())) { item.ErrorMessage += "Setting Value empty,"; }
                        if (string.IsNullOrEmpty(dataRow[11].ToString())) { item.ErrorMessage += "Setting Value empty,"; }
                        if (string.IsNullOrEmpty(dataRow[12].ToString())) { item.ErrorMessage += "Setting Value empty,"; }
                        if (string.IsNullOrEmpty(dataRow[13].ToString())) { item.ErrorMessage += "Setting Value empty,"; }
               



                       // if (exist != null) { item.ErrorMessage += "Data Already Exist,"; }

                        model.Add(item);

                    }
                    catch (Exception ex)
                    {
                        item.ErrorMessage = ex.Message;
                    }
                }
            }
            return Json(model);
        }

        [HttpPost]
        public ActionResult Upload(FuelOdometerModel Model)
        {
            if (ModelState.IsValid)
            {
                foreach (var data in Model.Details)
                {
                    try
                    {
                        data.CreatedDate = DateTime.Now;
                        data.CreatedBy = CurrentUser.USERNAME;
                        data.IsActive = true;
                        if (data.ErrorMessage == "" | data.ErrorMessage == null)
                        {
                            //var Exist = _fuelodometerBLL.GetFuelOdometer().Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (data.VehicleType == null ? "" : data.VehicleType.ToUpper())
                            //                                        && (x.RoleType == null ? "" : x.RoleType.ToUpper()) == (data.RoleType == null ? "" : data.RoleType.ToUpper())
                            //                                        && x.IsActive).FirstOrDefault();
                            var Exist = _fuelodometerBLL.GetFuelOdometer().Where( x => x.VehicleType == data.VehicleType && x.PoliceNumber == data.PoliceNumber && x.EmployeeId == data.EmployeeId && x.EcsRmbTransId == data.EcsRmbTransId && x.SeqNumber == data.SeqNumber && x.DateOfCost == data.DateOfCost && x.CostCenter == data.CostCenter && x.IsActive == true).FirstOrDefault();
                            if (Exist != null)
                            {
                                Exist.IsActive = false;
                                Exist.ModifiedBy = "SYSTEM";
                                Exist.ModifiedDate = DateTime.Now;
                                _fuelodometerBLL.Save(Exist, CurrentUser);
                            }

                            var dto = Mapper.Map<FuelOdometerDto>(data);
                            _fuelodometerBLL.Save(dto, CurrentUser);
                        }

                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                    }
                    catch (Exception exception)
                    {
                        AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                        Model.CurrentLogin = CurrentUser;
                        return View(Model);
                    }
                }
                _fuelodometerBLL.SaveChanges();
            }
            return RedirectToAction("Index", "MstFuelOdometer");
        }

    }
}
