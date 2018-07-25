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
        private ITraCsfBLL _csfBLL;
        private ITraCrfBLL _crfBLL;

        public TraTemporaryController(IPageBLL pageBll, IEmployeeBLL EmployeeBLL, IReasonBLL ReasonBLL, ITraTemporaryBLL TempBLL, ISettingBLL SettingBLL
            , IFleetBLL FleetBLL, IVehicleSpectBLL VehicleSpectBLL, IVendorBLL VendorBLL, IRemarkBLL RemarkBLL, ITraCsfBLL csfBll, ITraCrfBLL crfBLL)
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
            _csfBLL = csfBll;
            _crfBLL = crfBLL;
            _mainMenu = Enums.MenuList.Transaction;
        }

        #endregion

        #region --------- Open Document --------------

        public ActionResult Index()
        {
            //check temp in progress
            //_tempBLL.CheckTempInProgress();

            var data = _tempBLL.GetTemporary(CurrentUser, false);
            var model = new TemporaryIndexModel();
            model.TitleForm = "Temporary Open Document";
            model.TitleExport = "ExportOpen";
            model.TempList = Mapper.Map<List<TempData>>(data.OrderByDescending(x => x.CREATED_DATE));
            model.MainMenu = _mainMenu;
            model.CurrentPageAccess = CurrentPageAccess;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        #endregion

        #region --------- Completed Document --------------

        public ActionResult Completed()
        {
            var model = new TemporaryIndexModel();
            model.TitleForm = "Temporary Completed Document";
            model.TitleExport = "ExportCompleted";
            //model.TempList = Mapper.Map<List<TempData>>(data.OrderByDescending(x => x.MODIFIED_DATE));
            model.TempList = GetTempCompleted();
            model.MainMenu = _mainMenu;
            model.CurrentPageAccess = CurrentPageAccess;
            model.CurrentLogin = CurrentUser;
            model.IsCompleted = true;
            return View("Index", model);
        }
        private List<TempData> GetTempCompleted(TempSearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _tempBLL.GetTemporary(CurrentUser, true);
                return Mapper.Map<List<TempData>>(data);
            }

            //getbyparams
            var input = Mapper.Map<TempParamInput>(filter);
            var dbData = _tempBLL.GetTemporary(CurrentUser, true, input);
            return Mapper.Map<List<TempData>>(dbData);
        }

        #endregion

        #region --------- Personal Dashboard --------------

        public ActionResult PersonalDashboard()
        {
            var data = _tempBLL.GetTempPersonal(CurrentUser);
            var model = new TemporaryIndexModel();
            model.TitleForm = "Temporary Personal Dashboard";
            model.TitleExport = "ExportPersonal";
            model.TempList = Mapper.Map<List<TempData>>(data.OrderByDescending(x => x.CREATED_DATE));
            model.MainMenu = Enums.MenuList.PersonalDashboard;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
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
            model.Detail.IsIncludeCfmIdle = true;

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
            var settingData = _settingBLL.GetSetting();
            var allEmployee = _employeeBLL.GetEmployee();
            var reasonType = "WTC";

            var vehTypeBenefit = settingData.Where(x => x.SettingGroup == "VEHICLE_TYPE" && x.SettingName == "BENEFIT").FirstOrDefault().MstSettingId;
            model.Detail.IsBenefit = model.Detail.VehicleType == vehTypeBenefit.ToString() ? true : false;

            var paramVehUsage = EnumHelper.GetDescription(Enums.SettingGroup.VehicleUsageWtc);
            if (model.Detail.IsBenefit || CurrentUser.UserRole == Enums.UserRole.HR)
            {
                paramVehUsage = EnumHelper.GetDescription(Enums.SettingGroup.VehicleUsageBenefit);
                reasonType = "BENEFIT";
            }

            var list = allEmployee.Select(x => new { x.EMPLOYEE_ID, employee = x.EMPLOYEE_ID + " - " + x.FORMAL_NAME, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);
            var listReason = _reasonBLL.GetReason().Where(x => x.DocumentType == (int)Enums.DocumentType.TMP && x.IsActive && x.VehicleType == reasonType).Select(x => new { x.MstReasonId, x.Reason }).ToList().OrderBy(x => x.Reason);
            var listVehType = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.IsActive).Select(x => new { x.MstSettingId, x.SettingValue }).ToList();
            var listSupMethod = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.SupplyMethod) && x.IsActive).Select(x => new { x.MstSettingId, x.SettingValue }).ToList();
            var listVendor = _vendorBLL.GetVendor().Where(x => x.IsActive).ToList();
            var listProject = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.Project) && x.IsActive).Select(x => new { x.MstSettingId, x.SettingValue }).ToList();
            var listVehUsage = settingData.Where(x => x.SettingGroup == paramVehUsage && x.IsActive && x.SettingValue != "COP").Select(x => new { x.MstSettingId, x.SettingValue }).ToList();
            var listCity = allEmployee.Select(x => new { x.BASETOWN }).Distinct().ToList();

            model.Detail.EmployeeList = new SelectList(list, "EMPLOYEE_ID", "employee");
            model.Detail.ReasonList = new SelectList(listReason, "MstReasonId", "Reason");
            model.Detail.VehicleTypeList = new SelectList(listVehType, "MstSettingId", "SettingValue");
            model.Detail.SupplyMethodList = new SelectList(listSupMethod, "MstSettingId", "SettingValue");
            model.Detail.VendorList = new SelectList(listVendor, "ShortName", "ShortName");
            model.Detail.ProjectList = new SelectList(listProject, "MstSettingId", "SettingValue");
            model.Detail.VehicleUsageList = new SelectList(listVehUsage, "MstSettingId", "SettingValue");
            model.Detail.LocationCityList = new SelectList(listCity, "BASETOWN", "BASETOWN");

            var employeeData = _employeeBLL.GetByID(model.Detail.EmployeeId);
            if (employeeData != null)
            {
                model.Detail.LocationCity = employeeData.BASETOWN;
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

                model.Detail.EmployeeId = model
                    .Detail
                    .EmployeeId.Split('-')[0].Trim();

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

                var checkExistInFleet = _tempBLL.CheckTempExistsInFleet(item);
                //only check for benefit in master fleet
                if (checkExistInFleet && CurrentUser.UserRole == Enums.UserRole.HR)
                {
                    model = InitialModel(model);
                    model.ErrorMessage = "Data already exists in master fleet";
                    return View(model);
                }

                var checkExistTempOpen = _tempBLL.CheckTempOpenExists(item);
                //only check for benefit in temporary
                if (checkExistTempOpen && CurrentUser.UserRole == Enums.UserRole.HR)
                {
                    model = InitialModel(model);
                    model.ErrorMessage = "Data temporary already exists";
                    return View(model);
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
                return RedirectToAction("Edit", "TraTemporary", new { id = tempData.TRA_TEMPORARY_ID, isPersonalDashboard = false });
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = InitialModel(model);
                model.ErrorMessage = "Error when save data, please contact administrator";
                return View(model);
            }
        }

        #endregion

        #region --------- Detail --------------

        public ActionResult Detail(int? id, bool isPersonalDashboard, bool? ArchiveData = null)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var tempData = _tempBLL.GetTempById(id.Value, ArchiveData);

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

        public ActionResult Edit(int? id, bool isPersonalDashboard, bool? ArchiveData = null)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var tempData = _tempBLL.GetTempById(id.Value, ArchiveData);

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
            if ( (tempData.EMPLOYEE_ID_FLEET_APPROVAL == CurrentUser.EMPLOYEE_ID || (CurrentUser.LoginFor == null ? false : (CurrentUser.LoginFor.Where(x => x.EMPLOYEE_ID == tempData.EMPLOYEE_ID_FLEET_APPROVAL).Count() > 0 ? true : false))) &&  tempData.DOCUMENT_STATUS == Enums.DocumentStatus.InProgress)
            {
                return RedirectToAction("InProgress", "TraTemporary", new { id = tempData.TRA_TEMPORARY_ID, isPersonalDashboard = isPersonalDashboard });
            }

            try
            {
                var model = new TempItemModel();
                model.IsPersonalDashboard = isPersonalDashboard;
                model.Detail = Mapper.Map<TempData>(tempData);
                model = InitialModel(model);
                model.Detail.IsIncludeCfmIdle = true;

                var RemarkList = _remarkBLL.GetRemark().Where(x => x.RoleType == CurrentUser.UserRole.ToString() && x.DocumentType == (int)Enums.DocumentType.TMP).ToList();
                model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");

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

                model.Detail.EmployeeId = model
                    .Detail
                    .EmployeeId.Split('-')[0].Trim();
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
                return RedirectToAction("Edit", "TraTemporary", new { id = model.Detail.TraTempId, isPersonalDashboard = model.IsPersonalDashboard });

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = InitialModel(model);
                model.ErrorMessage = "Error when save data, please contact administrator";
                return View(model);
            }
        }

        #endregion

        #region --------- Approve / Reject --------------

        public ActionResult ApproveFleet(int? id, bool isPersonalDashboard, bool? ArchivedData = null)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var tempData = _tempBLL.GetTempById(id.Value, ArchivedData);

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

        public ActionResult InProgress(int? id, bool isPersonalDashboard, bool? ArchivedData = null)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var tempData = _tempBLL.GetTempById(id.Value, ArchivedData);

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

                var RemarkList = _remarkBLL.GetRemark().Where(x => x.RoleType == CurrentUser.UserRole.ToString() && x.DocumentType == (int)Enums.DocumentType.TMP).ToList();
                model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");

                //set data from cfm idle
                if (model.Detail.CfmIdleId != null)
                {
                    var cfmData = _fleetBLL.GetFleetById((int)model.Detail.CfmIdleId);

                    if (cfmData != null)
                    {
                        model.Detail.ModelsVendor = cfmData.Models;
                        model.Detail.PoliceNumberVendor = cfmData.PoliceNumber;
                        model.Detail.ManufacturerVendor = cfmData.Manufacturer;
                        model.Detail.SeriesVendor = cfmData.Series;
                        model.Detail.BodyTypeVendor = cfmData.BodyType;
                        model.Detail.ColorVendor = cfmData.Color;
                        model.Detail.VendorNameVendor = cfmData.VendorName;
                        model.Detail.ChasisNumberVendor = cfmData.ChasisNumber;
                        model.Detail.EngineNumberVendor = cfmData.EngineNumber;
                        model.Detail.IsAirBagVendor = cfmData.Airbag;
                        model.Detail.TransmissionVendor = cfmData.Transmission;
                        model.Detail.BrandingVendor = cfmData.Branding;
                        model.Detail.PurposeVendor = cfmData.Purpose;
                        model.Detail.VatDecimalVendor = cfmData.VatDecimal == null ? 0 : cfmData.VatDecimal.Value;
                        model.Detail.IsRestitutionVendor = cfmData.Restitution;
                        model.Detail.CommentsVendor = cfmData.Comments;
                    }
                }

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
        public ActionResult InProgress(TempItemModel model, bool? ArchivedData = null)
        {
            try
            {
                var tempData = _tempBLL.GetTempById(model.Detail.TraTempId, ArchivedData);
                tempData.VENDOR_POLICE_NUMBER = model.Detail.PoliceNumberVendor;
                tempData.VENDOR_MANUFACTURER = model.Detail.ManufacturerVendor;
                tempData.VENDOR_MODEL = model.Detail.ModelsVendor;
                tempData.VENDOR_SERIES = model.Detail.SeriesVendor;
                tempData.VENDOR_BODY_TYPE = model.Detail.BodyTypeVendor;
                tempData.VENDOR_VENDOR = model.Detail.VendorNameVendor;
                tempData.VENDOR_COLOUR = model.Detail.ColorVendor;
                tempData.VENDOR_CONTRACT_START_DATE = model.Detail.StartPeriodVendor;
                tempData.VENDOR_CONTRACT_END_DATE = model.Detail.EndPeriodVendor;
                tempData.VENDOR_PO_NUMBER = model.Detail.PoNumberVendor;
                tempData.VENDOR_CHASIS_NUMBER = model.Detail.ChasisNumberVendor;
                tempData.VENDOR_ENGINE_NUMBER = model.Detail.EngineNumberVendor;
                tempData.VENDOR_AIR_BAG = model.Detail.IsAirBagVendor;
                tempData.VENDOR_BRANDING = model.Detail.BrandingVendor;
                tempData.VENDOR_PURPOSE = model.Detail.PurposeVendor;
                tempData.VENDOR_PO_LINE = model.Detail.PoLineVendor;
                tempData.VAT_DECIMAL = model.Detail.VatDecimalVendor;
                tempData.VENDOR_RESTITUTION = model.Detail.IsRestitutionVendor;
                tempData.PRICE = model.Detail.PriceVendor;
                tempData.COMMENTS = model.Detail.CommentsVendor;

                var saveResult = _tempBLL.Save(tempData, CurrentUser);
                //send email to user if police number and contract start date is fill
                if (tempData.VENDOR_CONTRACT_START_DATE != null && !string.IsNullOrEmpty(tempData.VENDOR_POLICE_NUMBER))
                {
                    TempWorkflow(tempData.TRA_TEMPORARY_ID, Enums.ActionType.InProgress, null);
                }

                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                return RedirectToAction("InProgress", "TraTemporary", new { id = tempData.TRA_TEMPORARY_ID, isPersonalDashboard = model.IsPersonalDashboard });

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return View();
            }
        }

        #endregion

        #region --------- Cancel Document TEMP --------------

        public ActionResult CancelTemp(int TraTempCancelId, int RemarkId, bool IsPersonalDashboard)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _tempBLL.CancelTemp(TraTempCancelId, RemarkId, CurrentUser.USER_ID);
                    TempWorkflow(TraTempCancelId, Enums.ActionType.Cancel, null);
                    AddMessageInfo("Success Cancelled Document", Enums.MessageInfoType.Success);
                }
                catch (Exception)
                {

                }

            }
            return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
        }

        #endregion

        #region --------- Workflow --------------

        private void TempWorkflow(long id, Enums.ActionType actionType, int? comment)
        {
            var attachmentsList = new List<string>();

            //if fleet approve and send csv to vendor
            if (CurrentUser.UserRole == Enums.UserRole.Fleet &&
                                        (actionType == Enums.ActionType.Approve || actionType == Enums.ActionType.Submit))
            {
                var docForVendor = CreateExcelForVendor(id);
                attachmentsList.Add(docForVendor);
            }

            var input = new TempWorkflowDocumentInput
            {
                DocumentId = id,
                UserId = CurrentUser.USER_ID,
                EmployeeId = CurrentUser.EMPLOYEE_ID,
                UserRole = CurrentUser.UserRole,
                ActionType = actionType,
                Comment = comment,
                Attachments = attachmentsList
            };

            _tempBLL.TempWorkflow(input);
        }

        #endregion

        #region --------- Get Data Post JS --------------

        public JsonResult GetEmployeeList()
        {
            var allEmployee = _employeeBLL
                    .GetEmployee()
                    .Where(x => x.IS_ACTIVE)
                    .Select(x
                        => new
                        {
                            DATA = string.Concat(x.EMPLOYEE_ID, " - ", x.FORMAL_NAME)
                        })
                     .OrderBy(X => X.DATA)
                     .ToList();

            return Json(allEmployee, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetEmployee(string Id)
        {
            var data = Id.Split('-');
            var model = _employeeBLL.GetByID(data[0].Trim());
            return Json(model);
        }

        [HttpPost]
        public JsonResult GetVehicleData(string vehType, string groupLevel, DateTime createdDate, bool includeCfm, string vendor)
        {
            var vehicleType = _settingBLL.GetByID(Convert.ToInt32(vehType)).SettingName.ToLower();
            var vehicleData = _vehicleSpectBLL.GetVehicleSpect().Where(x => x.IsActive).ToList();

            var fleetDto = new List<FleetDto>();

            if (vehicleType == "benefit")
            {
                var modelVehicle = vehicleData.Where(x => x.GroupLevel > 0 && x.GroupLevel <= Convert.ToInt32(groupLevel)).ToList();

                if (!includeCfm)
                {
                    return Json(modelVehicle);
                }

                //get selectedCfmIdle temp
                var cfmIdleListSelected = _tempBLL.GetList().Where(x => x.DOCUMENT_STATUS != Enums.DocumentStatus.Cancelled && x.DOCUMENT_STATUS != Enums.DocumentStatus.Completed
                                                                        && x.CFM_IDLE_ID != null && x.CFM_IDLE_ID.Value > 0).Select(x => x.CFM_IDLE_ID.Value).ToList();

                //get selectedCfmIdle csf
                var cfmIdleListSelectedCsf = _csfBLL.GetList().Where(x => x.DOCUMENT_STATUS != Enums.DocumentStatus.Cancelled && x.DOCUMENT_STATUS != Enums.DocumentStatus.Completed
                                                                        && x.CFM_IDLE_ID != null && x.CFM_IDLE_ID.Value > 0).Select(x => x.CFM_IDLE_ID.Value).ToList();

                //get selectedCfmIdle crf
                var cfmIdleListSelectedCrf = _crfBLL.GetList().Where(x => x.DOCUMENT_STATUS != (int)Enums.DocumentStatus.Cancelled
                                                                        && x.MST_FLEET_ID != null && x.MST_FLEET_ID.Value > 0).Select(x => x.MST_FLEET_ID.Value).ToList();

                var fleetDataGood = _fleetBLL.GetFleet().Where(x => x.VehicleUsage != null);

                if (!string.IsNullOrEmpty(vendor))
                {
                    fleetDataGood = fleetDataGood.Where(x => (x.VendorName == null ? string.Empty : x.VendorName.ToUpper()) == vendor.ToUpper()).ToList();
                }

                var fleetData = fleetDataGood.Where(x => x.VehicleUsage.ToUpper() == "CFM IDLE" && 
                                                                x.IsActive &&
                                                                !cfmIdleListSelected.Contains(x.MstFleetId) &&
                                                                !cfmIdleListSelectedCsf.Contains(x.MstFleetId) &&
                                                                !cfmIdleListSelectedCrf.Contains(x.MstFleetId)).ToList();

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
                    item.Branding = dataRow[17];
                    item.Purpose = dataRow[18];
                    item.VehicleYear = Convert.ToInt32(dataRow[19]);
                    item.PoNumber = dataRow[20];
                    item.PoLine = dataRow[21];
                    item.VatDecimal = Convert.ToDecimal(dataRow[22]);
                    item.IsRestitution = dataRow[23].ToUpper() == "YES" ? true : false;
                    item.Price = Convert.ToDecimal(dataRow[24]);
                    item.Comments = dataRow[25];

                    model.Add(item);
                }
            }

            var input = Mapper.Map<List<VehicleFromVendorUpload>>(model);
            var outputResult = _tempBLL.ValidationUploadDocumentProcess(input, Detail_TraTempId);

            return Json(outputResult);


        }

        [HttpPost]
        public JsonResult MassUploadFile(HttpPostedFileBase itemExcelFile)
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
                    item.VehicleYear = Convert.ToInt32(dataRow[19]);
                    item.PoNumber = dataRow[20];
                    item.PoLine = dataRow[21];

                    item.VendorName = dataRow[3];
                    item.PoliceNumber = dataRow[4];
                    item.ChasisNumber = dataRow[5];
                    item.EngineNumber = dataRow[6];
                    item.IsAirBag = dataRow[9].ToUpper() == "YES" ? true : false;
                    item.Manufacturer = dataRow[10];
                    item.Models = dataRow[11];
                    item.Series = dataRow[12];
                    item.Transmission = dataRow[13];
                    item.Color = dataRow[14];
                    item.BodyType = dataRow[15];
                    item.Branding = dataRow[17];
                    item.Purpose = dataRow[18];
                    item.VatDecimal = Convert.ToDecimal(dataRow[22]);
                    item.IsRestitution = dataRow[23].ToUpper() == "YES" ? true : false;
                    item.Price = Convert.ToDecimal(dataRow[24]);
                    item.Comments = dataRow[25];

                    model.Add(item);
                }
            }

            var input = Mapper.Map<List<VehicleFromVendorUpload>>(model);
            var outputResult = _tempBLL.ValidationUploadDocumentProcessMassUpload(input);

            return Json(outputResult);


        }

        #endregion

        #region --------- Mass Upload From Vendor --------------

        public ActionResult Upload(bool IsPersonalDashboard)
        {
            var model = new CsfIndexModel();
            model.TitleForm = "Mass upload from Vendor";
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.IsPersonalDashboard = IsPersonalDashboard;

            if (IsPersonalDashboard)
            {
                model.TitleForm = "Mass upload from Vendor - Personal Dashboard";
                model.MainMenu = Enums.MenuList.PersonalDashboard;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(CsfItemModel model)
        {
            try
            {
                foreach (var data in model.TemporaryList)
                {

                    var tempData = _tempBLL.GetList().Where(x => x.DOCUMENT_NUMBER_TEMP.ToLower() == data.CsfNumber.ToLower()).FirstOrDefault();

                    if (tempData != null)
                    {
                        tempData.VENDOR_POLICE_NUMBER = data.PoliceNumber;
                        tempData.VENDOR_MANUFACTURER = data.Manufacturer;
                        tempData.VENDOR_MODEL = data.Models;
                        tempData.VENDOR_SERIES = data.Series;
                        tempData.VENDOR_BODY_TYPE = data.BodyType;
                        tempData.VENDOR_VENDOR = data.VendorName;
                        tempData.VENDOR_COLOUR = data.Color;
                        tempData.VENDOR_CONTRACT_START_DATE = data.StartPeriod;
                        tempData.VENDOR_CONTRACT_END_DATE = data.EndPeriod;
                        tempData.VENDOR_PO_NUMBER = data.PoNumber;
                        tempData.VENDOR_CHASIS_NUMBER = data.ChasisNumber;
                        tempData.VENDOR_ENGINE_NUMBER = data.EngineNumber;
                        tempData.VENDOR_AIR_BAG = data.IsAirBag;
                        tempData.VENDOR_TRANSMISSION = data.Transmission;
                        tempData.VENDOR_BRANDING = data.Branding;
                        tempData.VENDOR_PURPOSE = data.Purpose;
                        tempData.VENDOR_PO_LINE = data.PoLine;
                        tempData.VAT_DECIMAL = data.VatDecimal;
                        tempData.VENDOR_RESTITUTION = data.IsRestitution;
                        tempData.PRICE = data.Price;
                        tempData.COMMENTS = data.Comments;

                        var saveResult = _tempBLL.Save(tempData, CurrentUser);
                        //send email to user if police number and contract start date is fill
                    }
                }

                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);

                return RedirectToAction(model.IsPersonalDashboard ? "PersonalDashboard" : "Index");

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return View();
            }
        }

        #endregion

        #region --------- Add Attachment File For Vendor --------------

        private string CreateExcelForVendor(long id, bool? ArchivedData = null)
        {
            //get data
            var tempData = _tempBLL.GetTempById(id,ArchivedData);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(2, 2, "System");
            slDocument.MergeWorksheetCells(2, 2, 2, 4);

            slDocument.SetCellValue(2, 5, "Vendor");
            slDocument.MergeWorksheetCells(2, 5, 2, 10);

            slDocument.SetCellValue(2, 11, "User");
            slDocument.MergeWorksheetCells(2, 11, 2, 17);

            slDocument.SetCellValue(2, 18, "Fleet");
            slDocument.MergeWorksheetCells(2, 18, 2, 26);

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Font.FontSize = 11;
            slDocument.SetCellStyle(2, 2, 2, 26, valueStyle);

            //create header
            slDocument = CreateHeaderExcelForVendor(slDocument);

            //create data
            slDocument = CreateDataExcelForVendor(slDocument, tempData);

            var fileName = "ExcelForVendor_TMP_" + tempData.TRA_TEMPORARY_ID + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelForVendor(SLDocument slDocument)
        {
            int iRow = 3;

            slDocument.SetCellValue(iRow, 2, "Request Number");
            slDocument.SetCellValue(iRow, 3, "Employee Name");
            slDocument.SetCellValue(iRow, 4, "Vendor");
            slDocument.SetCellValue(iRow, 5, "Police Number");
            slDocument.SetCellValue(iRow, 6, "Chasis Number");
            slDocument.SetCellValue(iRow, 7, "Engine Number");
            slDocument.SetCellValue(iRow, 8, "Contract Start Date");
            slDocument.SetCellValue(iRow, 9, "Contract End Date");
            slDocument.SetCellValue(iRow, 10, "AirBag");
            slDocument.SetCellValue(iRow, 11, "Make");
            slDocument.SetCellValue(iRow, 12, "Model");
            slDocument.SetCellValue(iRow, 13, "Series");
            slDocument.SetCellValue(iRow, 14, "Transmission");
            slDocument.SetCellValue(iRow, 15, "Color");
            slDocument.SetCellValue(iRow, 16, "Body type");
            slDocument.SetCellValue(iRow, 17, "Location");
            slDocument.SetCellValue(iRow, 18, "Branding");
            slDocument.SetCellValue(iRow, 19, "Purpose");
            slDocument.SetCellValue(iRow, 20, "Request Year");
            slDocument.SetCellValue(iRow, 21, "PO");
            slDocument.SetCellValue(iRow, 22, "PO Line");
            slDocument.SetCellValue(iRow, 23, "Vat");
            slDocument.SetCellValue(iRow, 24, "Restitution");
            slDocument.SetCellValue(iRow, 25, "Price");
            slDocument.SetCellValue(iRow, 26, "Comments");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.SetCellStyle(iRow, 2, iRow, 26, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelForVendor(SLDocument slDocument, TemporaryDto tempData)
        {
            int iRow = 4; //starting row data

            var vSpecListData = _vehicleSpectBLL.GetVehicleSpect().Where(x => x.Year == tempData.CREATED_DATE.Year
                                                                        && x.Manufacturer != null
                                                                        && x.Models != null
                                                                        && x.Series != null
                                                                        && x.BodyType != null
                                                                        && x.IsActive).ToList();

            var vSpecList = vSpecListData.Where(x => x.Manufacturer.ToUpper() == tempData.MANUFACTURER.ToUpper()
                                                    && x.Models.ToUpper() == tempData.MODEL.ToUpper()
                                                    && x.Series.ToUpper() == tempData.SERIES.ToUpper()
                                                    && x.BodyType.ToUpper() == tempData.BODY_TYPE.ToUpper()).FirstOrDefault();

            var transmissionData = vSpecList == null ? string.Empty : vSpecList.Transmission;

            var policeNumberCfmIdle = string.Empty;
            var chasCfmIdle = string.Empty;
            var engCfmIdle = string.Empty;
            if (tempData.CFM_IDLE_ID != null)
            {
                var cfmData = _fleetBLL.GetFleetById((int)tempData.CFM_IDLE_ID);
                if (cfmData != null)
                {
                    policeNumberCfmIdle = cfmData.PoliceNumber == null ? string.Empty : cfmData.PoliceNumber;
                    chasCfmIdle = cfmData.ChasisNumber == null ? string.Empty : cfmData.ChasisNumber;
                    engCfmIdle = cfmData.EngineNumber == null ? string.Empty : cfmData.EngineNumber;
                    transmissionData = cfmData.Transmission == null ? string.Empty : cfmData.Transmission;
                }
            }

            slDocument.SetCellValue(iRow, 2, tempData.DOCUMENT_NUMBER_TEMP);
            slDocument.SetCellValue(iRow, 3, tempData.EMPLOYEE_NAME);
            slDocument.SetCellValue(iRow, 4, tempData.VENDOR_NAME);
            slDocument.SetCellValue(iRow, 5, policeNumberCfmIdle);
            slDocument.SetCellValue(iRow, 6, chasCfmIdle);
            slDocument.SetCellValue(iRow, 7, engCfmIdle);
            slDocument.SetCellValue(iRow, 8, tempData.START_DATE.Value.ToOADate());
            slDocument.SetCellValue(iRow, 9, tempData.END_DATE.Value.ToOADate());
            slDocument.SetCellValue(iRow, 10, "YES");
            slDocument.SetCellValue(iRow, 11, tempData.MANUFACTURER);
            slDocument.SetCellValue(iRow, 12, tempData.MODEL);
            slDocument.SetCellValue(iRow, 13, tempData.SERIES);
            slDocument.SetCellValue(iRow, 14, transmissionData);
            slDocument.SetCellValue(iRow, 15, tempData.COLOR);
            slDocument.SetCellValue(iRow, 16, tempData.BODY_TYPE);
            slDocument.SetCellValue(iRow, 17, tempData.LOCATION_CITY);
            slDocument.SetCellValue(iRow, 18, string.Empty);
            slDocument.SetCellValue(iRow, 19, string.Empty);
            slDocument.SetCellValue(iRow, 20, tempData.CREATED_DATE.Year.ToString());
            slDocument.SetCellValue(iRow, 21, string.Empty);
            slDocument.SetCellValue(iRow, 22, string.Empty);
            slDocument.SetCellValue(iRow, 23, 0);
            slDocument.SetCellValue(iRow, 24, "NO");
            slDocument.SetCellValue(iRow, 25, 0);
            slDocument.SetCellValue(iRow, 26, string.Empty);

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(2, 26);
            slDocument.SetCellStyle(iRow, 2, iRow, 26, valueStyle);

            SLStyle dateStyle = slDocument.CreateStyle();
            dateStyle.FormatCode = "dd/MM/yyyy";

            slDocument.SetCellStyle(iRow, 8, iRow, 9, dateStyle);

            return slDocument;
        }

        #endregion

        #region --------- Export --------------

        public void ExportPersonal()
        {
            string pathFile = "";

            pathFile = CreateXlsTempPersonal();

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

        private string CreateXlsTempPersonal()
        {
            //get data
            List<TemporaryDto> temp = _tempBLL.GetTempPersonal(CurrentUser);
            var listData = Mapper.Map<List<TempData>>(temp);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Personal Dashboard Temporary");
            slDocument.MergeWorksheetCells(1, 1, 1, 12);
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

        private string CreateXlsTemp(bool isCompleted)
        {
            //get data
            List<TemporaryDto> temp = _tempBLL.GetTemporary(CurrentUser, isCompleted);
            var listData = Mapper.Map<List<TempData>>(temp);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, isCompleted ? "Completed Document Temporary" : "Open Document Temporary");
            slDocument.MergeWorksheetCells(1, 1, 1, 12);
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

            slDocument.SetCellValue(iRow, 1, "Temporary Number");
            slDocument.SetCellValue(iRow, 2, "Temporary Status");
            slDocument.SetCellValue(iRow, 3, "Vehicle Type");
            slDocument.SetCellValue(iRow, 4, "Employee ID");
            slDocument.SetCellValue(iRow, 5, "Employee Name");
            slDocument.SetCellValue(iRow, 6, "Reason");
            slDocument.SetCellValue(iRow, 7, "Start Date");
            slDocument.SetCellValue(iRow, 8, "End Date");
            slDocument.SetCellValue(iRow, 9, "PO Number");
            slDocument.SetCellValue(iRow, 10, "Regional");
            slDocument.SetCellValue(iRow, 11, "Updated By");
            slDocument.SetCellValue(iRow, 12, "Updated Date");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 12, headerStyle);

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
                slDocument.SetCellValue(iRow, 7, data.StartPeriod.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 8, data.EndPeriod.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 9, data.PoNumberVendor);
                slDocument.SetCellValue(iRow, 10, data.Regional);
                slDocument.SetCellValue(iRow, 11, data.ModifiedBy == null ? data.CreateBy : data.ModifiedBy);
                slDocument.SetCellValue(iRow, 12, data.ModifiedDate == null ? data.CreateDate.ToString("dd-MMM-yyyy HH:mm:ss") : data.ModifiedDate.Value.ToString("dd-MMM-yyyy HH:mm:ss"));

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 12);
            slDocument.SetCellStyle(3, 1, iRow - 1, 12, valueStyle);

            return slDocument;
        }

        #endregion

        #region  --------- Archieve --------------
        [HttpPost]
        public PartialViewResult ListTraTempCompleted(TemporaryIndexModel model)
        {
            model.TempList = new List<TempData>();
            model.TempList = GetTempCompleted(model.SearchView);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            model.IsCompleted = true;
            return PartialView("_ListTemporary", model);
        }
        #endregion
    }
}
