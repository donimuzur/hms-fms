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
using FMS.Website.Utility;
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
        private IVendorBLL _vendorBLL;
        private IRemarkBLL _remarkBLL;

        public TraTemporaryController(IPageBLL pageBll, IEmployeeBLL EmployeeBLL, IReasonBLL ReasonBLL, ITraTemporaryBLL TempBLL, ISettingBLL SettingBLL
            , IFleetBLL FleetBLL, IVehicleSpectBLL VehicleSpectBLL, IVendorBLL VendorBLL, IRemarkBLL RemarkBLL)
            : base(pageBll, Core.Enums.MenuList.TraTmp)
        {
            _pageBLL = pageBll;
            _employeeBLL = EmployeeBLL;
            _reasonBLL = ReasonBLL;
            _tempBLL = TempBLL;
            _settingBLL = SettingBLL;
            _fleetBLL = FleetBLL;
            _vehicleSpectBLL = VehicleSpectBLL;
            _vendorBLL = VendorBLL;
            _remarkBLL = RemarkBLL;
            _mainMenu = Enums.MenuList.Transaction;
        }

        #endregion

        #region --------- Open Document --------------

        public ActionResult Index()
        {
            //check temp in progress
            _tempBLL.CheckTempInProgress();

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

            var vehTypeBenefit = _settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_TYPE" && x.SettingName == "BENEFIT").FirstOrDefault().MstSettingId;
            model.Detail.IsBenefit = model.Detail.VehicleType == vehTypeBenefit.ToString() ? true : false;

            var list = allEmployee.Select(x => new { x.EMPLOYEE_ID, employee = x.EMPLOYEE_ID + " - " + x.FORMAL_NAME, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);
            var listReason = _reasonBLL.GetReason().Where(x => x.DocumentType == (int)Enums.DocumentType.TMP).Select(x => new { x.MstReasonId, x.Reason }).ToList().OrderBy(x => x.Reason);
            var listVehType = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType)).Select(x => new { x.MstSettingId, x.SettingValue }).ToList();
            var listSupMethod = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.SupplyMethod)).Select(x => new { x.MstSettingId, x.SettingValue }).ToList();
            var listVendor = _vendorBLL.GetVendor().Where(x => x.IsActive).ToList();
            
            model.Detail.EmployeeList = new SelectList(list, "EMPLOYEE_ID", "employee");
            model.Detail.ReasonList = new SelectList(listReason, "MstReasonId", "Reason");
            model.Detail.VehicleTypeList = new SelectList(listVehType, "MstSettingId", "SettingValue");
            model.Detail.SupplyMethodList = new SelectList(listSupMethod, "MstSettingId", "SettingValue");
            model.Detail.VendorList = new SelectList(listVendor, "ShortName", "ShortName");

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

        #region --------- Detail --------------

        public ActionResult Detail(int? id, bool isPersonalDashboard)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var tempData = _tempBLL.GetTempById(id.Value);

            if (tempData == null)
            {
                return HttpNotFound();
            }

            try
            {
                var model = new TempItemModel();
                model.IsPersonalDashboard = isPersonalDashboard;
                model.Detail = Mapper.Map<TempData>(tempData);
                model = InitialModel(model);

                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction(isPersonalDashboard ? "PersonalDashboard" : "Index");
            }
        }

        #endregion

        #region --------- Edit --------------

        public ActionResult Edit(int? id, bool isPersonalDashboard)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var tempData = _tempBLL.GetTempById(id.Value);

            if (tempData == null)
            {
                return HttpNotFound();
            }

            //if status waiting for fleet approval
            if (tempData.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingFleetApproval)
            {
                return RedirectToAction("ApproveFleet", "TraTemporary", new { id = tempData.TRA_TEMPORARY_ID, isPersonalDashboard = isPersonalDashboard });
            }

            //if status in progress
            if (tempData.DOCUMENT_STATUS == Enums.DocumentStatus.InProgress)
            {
                return RedirectToAction("InProgress", "TraTemporary", new { id = tempData.TRA_TEMPORARY_ID, isPersonalDashboard = isPersonalDashboard });
            }

            try
            {
                var model = new TempItemModel();
                model.IsPersonalDashboard = isPersonalDashboard;
                model.Detail = Mapper.Map<TempData>(tempData);
                model = InitialModel(model);

                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction(isPersonalDashboard ? "PersonalDashboard" : "Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TempItemModel model)
        {
            try
            {
                var dataToSave = Mapper.Map<TemporaryDto>(model.Detail);

                dataToSave.DOCUMENT_STATUS = Enums.DocumentStatus.Draft;
                dataToSave.MODIFIED_BY = CurrentUser.USER_ID;
                dataToSave.MODIFIED_DATE = DateTime.Now;

                bool isSubmit = model.Detail.IsSaveSubmit == "submit";

                var saveResult = _tempBLL.Save(dataToSave, CurrentUser);

                if (isSubmit)
                {
                    TempWorkflow(model.Detail.TraTempId, Enums.ActionType.Submit, null);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("Detail", "TraTemporary", new { id = model.Detail.TraTempId, isPersonalDashboard = model.IsPersonalDashboard });
                }

                //return RedirectToAction("Index");
                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                return RedirectToAction(model.IsPersonalDashboard ? "PersonalDashboard" : "Index");

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

        #region --------- Approve / Reject --------------

        public ActionResult ApproveFleet(int? id, bool isPersonalDashboard)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var tempData = _tempBLL.GetTempById(id.Value);

            if (tempData == null)
            {
                return HttpNotFound();
            }

            //if not fleet
            if (Enums.UserRole.Fleet != CurrentUser.UserRole)
            {
                return RedirectToAction("Detail", "TraTemporary", new { id = tempData.TRA_TEMPORARY_ID, isPersonalDashboard = isPersonalDashboard });
            }

            try
            {
                var model = new TempItemModel();
                model.IsPersonalDashboard = isPersonalDashboard;
                model.Detail = Mapper.Map<TempData>(tempData);
                model = InitialModel(model);

                var RemarkList = _remarkBLL.GetRemark().Where(x => x.RoleType == CurrentUser.UserRole.ToString() && x.DocumentType == (int)Enums.DocumentType.CSF).ToList();
                model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");

                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction(isPersonalDashboard ? "PersonalDashboard" : "Index");
            }
        }

        public ActionResult ApproveTemp(long TraTempId, bool IsPersonalDashboard)
        {
            bool isSuccess = false;
            try
            {
                TempWorkflow(TraTempId, Enums.ActionType.Approve, null);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            if (!isSuccess) return RedirectToAction("Detail", "TraTemporary", new { id = TraTempId, isPersonalDashboard = IsPersonalDashboard });
            AddMessageInfo("Success Approve Document", Enums.MessageInfoType.Success);
            return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
        }

        public ActionResult RejectTemp(int TraTempIdReject, int RemarkId, bool IsPersonalDashboard)
        {
            bool isSuccess = false;
            try
            {
                TempWorkflow(TraTempIdReject, Enums.ActionType.Reject, RemarkId);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("Detail", "TraTemporary", new { id = TraTempIdReject, isPersonalDashboard = IsPersonalDashboard });
            AddMessageInfo("Success Reject Document", Enums.MessageInfoType.Success);
            return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
        }

        #endregion

        #region --------- In Progress --------------

        public ActionResult InProgress(int? id, bool isPersonalDashboard)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var tempData = _tempBLL.GetTempById(id.Value);

            if (tempData == null)
            {
                return HttpNotFound();
            }

            //if not fleet
            if (Enums.UserRole.Fleet != CurrentUser.UserRole)
            {
                return RedirectToAction("Detail", "TraTemporary", new { id = tempData.TRA_TEMPORARY_ID, isPersonalDashboard = isPersonalDashboard });
            }

            try
            {
                var model = new TempItemModel();
                model.IsPersonalDashboard = isPersonalDashboard;
                model.Detail = Mapper.Map<TempData>(tempData);
                model = InitialModel(model);

                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction(isPersonalDashboard ? "PersonalDashboard" : "Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InProgress(int? id, string ModelVendorPoliceNumber, string ModelVendorManufacturer, string ModelVendorModels,
            string ModelVendorSeries, string ModelVendorBodyType, string ModelVendorVendorName, string ModelVendorColor, DateTime ModelVendorStartPeriod,
            DateTime ModelVendorEndPeriod, string ModelVendorPoNumber, string ModelVendorChasisNumber, string ModelVendorEngineNumber, bool ModelVendorIsAirBag,
            string ModelVendorTransmission, string ModelVendorBranding, string ModelVendorPurpose, string ModelVendorPoLine, bool ModelVendorIsVat, bool ModelVendorIsRestitution)
        {
            try
            {
                var tempData = _tempBLL.GetTempById(id.Value);
                tempData.VENDOR_POLICE_NUMBER = ModelVendorPoliceNumber;
                tempData.VENDOR_MANUFACTURER = ModelVendorManufacturer;
                tempData.VENDOR_MODEL = ModelVendorModels;
                tempData.VENDOR_SERIES = ModelVendorSeries;
                tempData.VENDOR_BODY_TYPE = ModelVendorBodyType;
                tempData.VENDOR_VENDOR = ModelVendorVendorName;
                tempData.VENDOR_COLOUR = ModelVendorColor;
                tempData.VENDOR_CONTRACT_START_DATE = ModelVendorStartPeriod;
                tempData.VENDOR_CONTRACT_END_DATE = ModelVendorEndPeriod;
                tempData.VENDOR_PO_NUMBER = ModelVendorPoNumber;
                tempData.VENDOR_CHASIS_NUMBER = ModelVendorChasisNumber;
                tempData.VENDOR_ENGINE_NUMBER = ModelVendorEngineNumber;
                tempData.VENDOR_AIR_BAG = ModelVendorIsAirBag;
                tempData.VENDOR_TRANSMISSION = ModelVendorTransmission;
                tempData.VENDOR_BRANDING = ModelVendorBranding;
                tempData.VENDOR_PURPOSE = ModelVendorPurpose;
                tempData.VENDOR_PO_LINE = ModelVendorPoLine;
                tempData.VENDOR_VAT = ModelVendorIsVat;
                tempData.VENDOR_RESTITUTION = ModelVendorIsRestitution;

                var saveResult = _tempBLL.Save(tempData, CurrentUser);

                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                return RedirectToAction("Index");

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return View();
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

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase itemExcelFile, int Detail_TraTempId)
        {
            var data = (new ExcelReader()).ReadExcel(itemExcelFile);
            var model = new List<TemporaryData>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[1] == "Request Number" || dataRow[1] == "")
                    {
                        continue;
                    }

                    var item = new TemporaryData();

                    item.CsfNumber = dataRow[1];
                    item.EmployeeName = dataRow[2];
                    item.VendorName = dataRow[3];
                    item.PoliceNumber = dataRow[4];
                    item.ChasisNumber = dataRow[5];
                    item.EngineNumber = dataRow[6];
                    double dStart = double.Parse(dataRow[7].ToString());
                    DateTime convStart = DateTime.FromOADate(dStart);
                    double dEnd = double.Parse(dataRow[8].ToString());
                    DateTime convEnd = DateTime.FromOADate(dEnd);
                    item.StartPeriod = convStart;
                    item.EndPeriod = convEnd;
                    item.StartPeriodName = convStart.ToString("dd-MMM-yyyy");
                    item.EndPeriodName = convEnd.ToString("dd-MMM-yyyy");
                    item.StartPeriodValue = convStart.ToString("MM/dd/yyyy");
                    item.EndPeriodValue = convEnd.ToString("MM/dd/yyyy");
                    item.IsAirBag = dataRow[9].ToUpper() == "YES" ? true : false;
                    item.Manufacturer = dataRow[10];
                    item.Models = dataRow[11];
                    item.Series = dataRow[12];
                    item.Transmission = dataRow[13];
                    item.Color = dataRow[14];
                    item.BodyType = dataRow[15];
                    item.Branding = dataRow[16];
                    item.Purpose = dataRow[17];
                    item.VehicleYear = Convert.ToInt32(dataRow[18]);
                    item.PoNumber = dataRow[19];
                    item.PoLine = dataRow[20];
                    item.IsVat = dataRow[21].ToUpper() == "YES" ? true : false;
                    item.IsRestitution = dataRow[22].ToUpper() == "YES" ? true : false;

                    model.Add(item);
                }
            }

            var input = Mapper.Map<List<VehicleFromVendorUpload>>(model);
            var outputResult = _tempBLL.ValidationUploadDocumentProcess(input, Detail_TraTempId);

            return Json(outputResult);


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
            slDocument.MergeWorksheetCells(1, 1, 1, 11);
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
            slDocument.SetCellValue(iRow, 3, "Vehicle Type");
            slDocument.SetCellValue(iRow, 4, "Employee ID");
            slDocument.SetCellValue(iRow, 5, "Employee Name");
            slDocument.SetCellValue(iRow, 6, "Reason");
            slDocument.SetCellValue(iRow, 7, "Start Date");
            slDocument.SetCellValue(iRow, 8, "End Date");
            slDocument.SetCellValue(iRow, 9, "Regional");
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

        private SLDocument CreateDataExcelTemp(SLDocument slDocument, List<TempData> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.TempNumber);
                slDocument.SetCellValue(iRow, 2, data.TempStatusName);
                slDocument.SetCellValue(iRow, 3, data.VehicleTypeName);
                slDocument.SetCellValue(iRow, 4, data.EmployeeId);
                slDocument.SetCellValue(iRow, 5, data.EmployeeName);
                slDocument.SetCellValue(iRow, 6, data.Reason);
                slDocument.SetCellValue(iRow, 7, data.StartPeriod.ToString("dd-MMM-yyyy HH:mm:ss"));
                slDocument.SetCellValue(iRow, 8, data.EndPeriod.ToString("dd-MMM-yyyy HH:mm:ss"));
                slDocument.SetCellValue(iRow, 9, data.Regional);
                slDocument.SetCellValue(iRow, 10, data.ModifiedBy == null ? data.CreateBy : data.ModifiedBy);
                slDocument.SetCellValue(iRow, 11, data.ModifiedDate == null ? data.CreateDate.ToString("dd-MMM-yyyy HH:mm:ss") : data.ModifiedDate.Value.ToString("dd-MMM-yyyy HH:mm:ss"));

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
