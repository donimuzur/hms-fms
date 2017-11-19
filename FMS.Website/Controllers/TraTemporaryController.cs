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
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;

namespace FMS.Website.Controllers
{
    public class TraTemporaryController : BaseController
    {
        #region --------- Field and Constructor --------------

        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private IEmployeeBLL _employeeBLL;
        private IReasonBLL _reasonBLL;
        private ITraTemporaryBLL _tempBLL;
        private ISettingBLL _settingBLL;
        private IFleetBLL _fleetBLL;
        private IVehicleSpectBLL _vehicleSpectBLL;

        public TraTemporaryController(IPageBLL pageBll, IEmployeeBLL EmployeeBLL, IReasonBLL ReasonBLL, ITraTemporaryBLL TempBLL, ISettingBLL SettingBLL
            , IFleetBLL FleetBLL, IVehicleSpectBLL VehicleSpectBLL)
            : base(pageBll, Core.Enums.MenuList.TraTmp)
        {
            _pageBLL = pageBll;
            _employeeBLL = EmployeeBLL;
            _reasonBLL = ReasonBLL;
            _tempBLL = TempBLL;
            _settingBLL = SettingBLL;
            _fleetBLL = FleetBLL;
            _vehicleSpectBLL = VehicleSpectBLL;
            _mainMenu = Enums.MenuList.Transaction;
        }

        #endregion

        #region --------- Open Document --------------

        public ActionResult Index()
        {
            var data = _tempBLL.GetTemporary(CurrentUser, false);
            var model = new TemporaryIndexModel();
            model.TitleForm = "Temporary Open Document";
            model.TitleExport = "ExportOpen";
            model.TempList = Mapper.Map<List<TempData>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        #endregion

        #region --------- Completed Document --------------

        public ActionResult Completed()
        {
            var data = _tempBLL.GetTemporary(CurrentUser, true);
            var model = new TemporaryIndexModel();
            model.TitleForm = "Temporary Completed Document";
            model.TitleExport = "ExportCompleted";
            model.TempList = Mapper.Map<List<TempData>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.IsCompleted = true;
            return View("Index", model);
        }

        #endregion

        #region --------- Personal Dashboard --------------

        public ActionResult PersonalDashboard()
        {
            var data = _tempBLL.GetTempPersonal(CurrentUser);
            var model = new TemporaryIndexModel();
            model.TitleForm = "Temporary Personal Dashboard";
            model.TitleExport = "ExportOpen";
            model.TempList = Mapper.Map<List<TempData>>(data);
            model.MainMenu = Enums.MenuList.PersonalDashboard;
            model.CurrentLogin = CurrentUser;
            model.IsPersonalDashboard = true;
            return View("Index", model);
        }

        #endregion

        #region --------- Create --------------

        public ActionResult Create()
        {
            if (CurrentUser.UserRole != Enums.UserRole.HR && CurrentUser.UserRole != Enums.UserRole.Fleet)
            {
                return RedirectToAction("Index");
            }

            var model = new TempItemModel();

            model = InitialModel(model);
            model.Detail.StartPeriod = DateTime.Now;
            model.Detail.EndPeriod = DateTime.Now;
            model.Detail.CreateDate = DateTime.Now;
            model.Detail.CreateBy = CurrentUser.USERNAME;
            model.Detail.IsBenefit = CurrentUser.UserRole == Enums.UserRole.HR ? true : false;

            var listVehType = _settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_TYPE").Select(x => new { x.MstSettingId, x.SettingValue }).ToList();
            model.Detail.VehicleType = listVehType.Where(x => x.SettingValue.ToLower() == "benefit").FirstOrDefault().MstSettingId.ToString();

            if (CurrentUser.UserRole == Enums.UserRole.Fleet)
            {
                model.Detail.VehicleType = listVehType.Where(x => x.SettingValue.ToLower() == "wtc").FirstOrDefault().MstSettingId.ToString();
            }

            return View(model);
        }

        public TempItemModel InitialModel(TempItemModel model)
        {
            var allEmployee = _employeeBLL.GetEmployee();

            if (CurrentUser.UserRole == Enums.UserRole.HR)
            {
                allEmployee = allEmployee.Where(x => x.GROUP_LEVEL > 0).ToList();
            }

            var vehTypeBenefit = _settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_TYPE" && x.SettingName == "BENEFIT").FirstOrDefault().MstSettingId;
            model.Detail.IsBenefit = model.Detail.VehicleType == vehTypeBenefit.ToString() ? true : false;

            var list = allEmployee.Select(x => new { x.EMPLOYEE_ID, employee = x.EMPLOYEE_ID + " - " + x.FORMAL_NAME, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);
            var listReason = _reasonBLL.GetReason().Where(x => x.DocumentType == (int)Enums.DocumentType.TMP).Select(x => new { x.MstReasonId, x.Reason }).ToList().OrderBy(x => x.Reason);
            var listVehType = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType)).Select(x => new { x.MstSettingId, x.SettingValue }).ToList();
            var listSupMethod = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.SupplyMethod)).Select(x => new { x.MstSettingId, x.SettingValue }).ToList();
            
            model.Detail.EmployeeList = new SelectList(list, "EMPLOYEE_ID", "employee");
            model.Detail.ReasonList = new SelectList(listReason, "MstReasonId", "Reason");
            model.Detail.VehicleTypeList = new SelectList(listVehType, "MstSettingId", "SettingValue");
            model.Detail.SupplyMethodList = new SelectList(listSupMethod, "MstSettingId", "SettingValue");

            var employeeData = _employeeBLL.GetByID(model.Detail.EmployeeId);
            if (employeeData != null)
            {
                model.Detail.LocationCity = employeeData.CITY;
                model.Detail.LocationAddress = employeeData.ADDRESS;
            }

            model.CurrentLogin = CurrentUser;
            model.MainMenu = model.IsPersonalDashboard ? Enums.MenuList.PersonalDashboard : _mainMenu;

            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.TraTmp, model.Detail.TraTempId);
            model.WorkflowLogs = GetWorkflowHistory((int)Enums.MenuList.TraTmp, model.Detail.TraTempId);

            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TempItemModel model)
        {
            try
            {
                TemporaryDto item = new TemporaryDto();

                item = AutoMapper.Mapper.Map<TemporaryDto>(model.Detail);

                item.CREATED_BY = CurrentUser.USER_ID;
                item.CREATED_DATE = DateTime.Now;
                item.DOCUMENT_STATUS = Enums.DocumentStatus.Draft;
                item.IS_ACTIVE = true;

                var listVehType = _settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_TYPE").Select(x => new { x.MstSettingId, x.SettingValue }).ToList();
                item.VEHICLE_TYPE = listVehType.Where(x => x.SettingValue.ToLower() == "benefit").FirstOrDefault().MstSettingId.ToString();

                if (CurrentUser.UserRole == Enums.UserRole.Fleet)
                {
                    item.VEHICLE_TYPE = listVehType.Where(x => x.SettingValue.ToLower() == "wtc").FirstOrDefault().MstSettingId.ToString();
                }

                var tempData = _tempBLL.Save(item, CurrentUser);

                bool isSubmit = model.Detail.IsSaveSubmit == "submit";

                if (isSubmit)
                {
                    TempWorkflow(tempData.TRA_TEMPORARY_ID, Enums.ActionType.Submit, null);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("Edit", "TraTemporary", new { id = tempData.TRA_TEMPORARY_ID, isPersonalDashboard = false });
                }

                AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
                TempWorkflow(tempData.TRA_TEMPORARY_ID, Enums.ActionType.Created, null);
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = InitialModel(model);
                model.ErrorMessage = exception.Message;
                return View(model);
            }
        }

        #endregion

        #region --------- Workflow --------------

        private void TempWorkflow(long id, Enums.ActionType actionType, int? comment)
        {
            var input = new TempWorkflowDocumentInput
            {
                DocumentId = id,
                UserId = CurrentUser.USER_ID,
                EmployeeId = CurrentUser.EMPLOYEE_ID,
                UserRole = CurrentUser.UserRole,
                ActionType = actionType,
                Comment = comment
            };

            _tempBLL.TempWorkflow(input);
        }

        #endregion

        #region --------- Get Data Post JS --------------

        [HttpPost]
        public JsonResult GetEmployee(string Id)
        {
            var model = _employeeBLL.GetByID(Id);
            return Json(model);
        }

        [HttpPost]
        public JsonResult GetVehicleData(string vehType, string groupLevel, DateTime createdDate)
        {
            var vehicleType = _settingBLL.GetByID(Convert.ToInt32(vehType)).SettingName.ToLower();
            var vehicleData = _vehicleSpectBLL.GetVehicleSpect().Where(x => x.IsActive && x.Year == createdDate.Year).ToList();

            var fleetDto = new List<FleetDto>();

            if (vehicleType == "benefit")
            {
                var modelVehicle = vehicleData.Where(x => x.GroupLevel <= Convert.ToInt32(groupLevel)).ToList();

                var fleetData = _fleetBLL.GetFleet().Where(x => x.VehicleUsage.ToUpper() == "CFM IDLE" && x.IsActive && x.VehicleYear == createdDate.Year).ToList();

                var modelCFMIdle = fleetData.Where(x => x.CarGroupLevel <= Convert.ToInt32(groupLevel)).ToList();

                fleetDto = modelCFMIdle;

                if (modelCFMIdle.Count == 0)
                {
                    return Json(modelVehicle);
                }

                return Json(fleetDto);
            }
            else
            {
                vehicleData = vehicleData.Where(x => x.GroupLevel == 0).ToList();

                return Json(vehicleData);
            }
        }

        #endregion

        #region --------- Export --------------

        public void ExportOpen()
        {
            string pathFile = "";

            pathFile = CreateXlsTemp(false);

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

        public void ExportCompleted()
        {
            string pathFile = "";

            pathFile = CreateXlsTemp(true);

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

        private string CreateXlsTemp(bool isCompleted)
        {
            //get data
            List<TemporaryDto> temp = _tempBLL.GetTemporary(CurrentUser, isCompleted);
            var listData = Mapper.Map<List<TempData>>(temp);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, isCompleted ? "Completed Document Temporary" : "Open Document Temporary");
            slDocument.MergeWorksheetCells(1, 1, 1, 9);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelTemp(slDocument);

            //create data
            slDocument = CreateDataExcelTemp(slDocument, listData);

            var fileName = "Data_TEMP" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelTemp(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Temporary No");
            slDocument.SetCellValue(iRow, 2, "Temporary Status");
            slDocument.SetCellValue(iRow, 3, "Employee ID");
            slDocument.SetCellValue(iRow, 4, "Employee Name");
            slDocument.SetCellValue(iRow, 5, "Reason");
            slDocument.SetCellValue(iRow, 6, "Start Date");
            slDocument.SetCellValue(iRow, 7, "End Date");
            slDocument.SetCellValue(iRow, 8, "Modified By");
            slDocument.SetCellValue(iRow, 9, "Modified Date");

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

        private SLDocument CreateDataExcelTemp(SLDocument slDocument, List<TempData> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.TempNumber);
                slDocument.SetCellValue(iRow, 2, data.TempStatusName);
                slDocument.SetCellValue(iRow, 3, data.EmployeeId);
                slDocument.SetCellValue(iRow, 4, data.EmployeeName);
                slDocument.SetCellValue(iRow, 5, data.Reason);
                slDocument.SetCellValue(iRow, 6, data.StartPeriod.ToString("dd-MMM-yyyy HH:mm:ss"));
                slDocument.SetCellValue(iRow, 7, data.EndPeriod.ToString("dd-MMM-yyyy HH:mm:ss"));
                slDocument.SetCellValue(iRow, 8, data.ModifiedBy == null ? data.CreateBy : data.ModifiedBy);
                slDocument.SetCellValue(iRow, 9, data.ModifiedDate == null ? data.CreateDate.ToString("dd-MMM-yyyy HH:mm:ss") : data.ModifiedDate.Value.ToString("dd-MMM-yyyy HH:mm:ss"));

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

        #endregion
    }
}
