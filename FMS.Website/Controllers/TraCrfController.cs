using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.EMMA;
using FMS.BusinessObject.Inputs;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;
using System.IO;
using FMS.BusinessObject.Dto;
using iTextSharp.text;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;

namespace FMS.Website.Controllers
{
    public class TraCrfController : BaseController
    {
        //
        // GET: /TraCrf/
        private Enums.MenuList _mainMenu;
        
        private IEpafBLL _epafBLL;
        private ITraCrfBLL _CRFBLL;
        private IRemarkBLL _remarkBLL;
        private IEmployeeBLL _employeeBLL;
        private IReasonBLL _reasonBLL;
        private ISettingBLL _settingBLL;
        private IFleetBLL _fleetBLL;
        private ITraCsfBLL _csfBLL;
        private ITraTemporaryBLL _tempBLL;
        //private IVendorBLL _vendorBLL;

        
        private List<SettingDto> _settingList;

        public TraCrfController(IPageBLL pageBll, IEpafBLL epafBll, ITraCrfBLL crfBLL, IRemarkBLL RemarkBLL, IEmployeeBLL EmployeeBLL, IReasonBLL ReasonBLL,
            ISettingBLL SettingBLL, IFleetBLL FleetBLL,IVendorBLL vendorBLL,ITraCsfBLL csfBLL,ITraTemporaryBLL tempBLL)
            : base(pageBll, Core.Enums.MenuList.TraCrf)
        {
            _epafBLL = epafBll;
            _CRFBLL = crfBLL;
            _remarkBLL = RemarkBLL;
            _employeeBLL = EmployeeBLL;
            _reasonBLL = ReasonBLL;
            _settingBLL = SettingBLL;
            _mainMenu = Enums.MenuList.Transaction;
            _fleetBLL = FleetBLL;
            //_vendorBLL = vendorBLL;
            _settingList = _settingBLL.GetSetting();
            _csfBLL = csfBLL;
            _tempBLL = tempBLL;

        }


        public TraCrfItemViewModel InitialModel(TraCrfItemViewModel model)
        {
            var list = _employeeBLL.GetEmployee().Where(x=> x.IS_ACTIVE && x.GROUP_LEVEL > 0).Select(x => new { x.EMPLOYEE_ID, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);
            var listReason = _reasonBLL.GetReason().Where(x => x.DocumentType == (int)Enums.DocumentType.TMP).Select(x => new { x.MstReasonId, x.Reason }).ToList().OrderBy(x => x.Reason);
            var listVehType = _settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_TYPE").Select(x => new { x.SettingName, x.SettingValue }).ToList();
            var listVehCat = _settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_CATEGORY").Select(x => new { x.SettingName, x.SettingValue }).ToList();
            var listVehUsage = _settingBLL.GetSetting().Where(x => x.SettingGroup.Contains("VEHICLE_USAGE_BENEFIT")).Select(x => new { x.SettingName, x.SettingValue }).ToList();
            var listSupMethod = _settingBLL.GetSetting().Where(x => x.SettingGroup == "SUPPLY_METHOD").Select(x => new { x.SettingName, x.SettingValue }).ToList();
            var listProject = _settingBLL.GetSetting().Where(x => x.SettingGroup == "PROJECT").Select(x => new { x.SettingName, x.SettingValue }).ToList();
            var listRelocate = _settingBLL.GetSetting().Where(x => x.SettingGroup == "RELOCATION_TYPE").Select(x => new { x.SettingName, x.SettingValue }).ToList();
            var listLocation = _employeeBLL.GetCityLocation().OrderBy(x=> x.City).ToList();
            

            
            model.EmployeeList = new SelectList(list, "EMPLOYEE_ID", "FORMAL_NAME");
            model.LocationList = new SelectList(listLocation.Select(x => new { x.City }).Distinct().ToList(), "City", "City");
            model.ReasonList = new SelectList(listReason, "MstReasonId", "Reason");
            model.VehicleTypeList = new SelectList(listVehType, "SettingName", "SettingValue");
            model.VehicleCatList = new SelectList(listVehCat, "SettingName", "SettingValue");
            model.VehicleUsageList = new SelectList(listVehUsage, "SettingName", "SettingValue");
            model.SupplyMethodList = new SelectList(listSupMethod, "SettingName", "SettingValue");
            model.ProjectList = new SelectList(listProject, "SettingName", "SettingValue");
            model.RelocateList = new SelectList(listRelocate, "SettingName", "SettingValue");
            
            model.CurrentLogin = CurrentUser;
            
            model.MainMenu = _mainMenu;

            return model;
        }

        public ActionResult Index()
        {
            var model = new TraCrfIndexViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            var data = _CRFBLL.GetList(CurrentUser);
            model.CurrentPageAccess = CurrentPageAccess;
            if (CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Administrator)
            {
                model.CurrentPageAccess.ReadAccess = true;
                model.CurrentPageAccess.WriteAccess = true;
            }
            model.Details = Mapper.Map<List<TraCrfItemDetails>>(data);
            foreach (var traCrfDto in model.Details)
            {
                traCrfDto.CurrentLogin = CurrentUser;
            }
            return View(model);
        }

        public ActionResult PersonalDashboard()
        {
            var data = _CRFBLL.GetCrfPersonal(CurrentUser);
            var model = new TraCrfIndexViewModel
            {
                Details = Mapper.Map<List<TraCrfItemDetails>>(data),
                MainMenu = Enums.MenuList.PersonalDashboard,
                CurrentLogin = CurrentUser,
                CurrentPageAccess = new RoleDto()
                {
                    ReadAccess = true,
                    
                },
                IsPersonalDashboard = true
            };
            foreach (var traCrfDto in model.Details)
            {
                traCrfDto.CurrentLogin = CurrentUser;
            }
            //model.TitleForm = "CRF Personal Dashboard";
           // model.TitleExport = "ExportOpen";
            return View("Index", model);
        }

        public ActionResult Completed()
        {
            var model = new TraCrfIndexViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            var data = _CRFBLL.GetCompleted();
            model.Details = Mapper.Map<List<TraCrfItemDetails>>(data);
            return View(model);
        }

        public ActionResult Dashboard()
        {
            var model = new TraCrfDashboardViewModel();
            model.MainMenu = _mainMenu;
            var RemarkList = _remarkBLL.GetRemark().Where(x => x.RoleType == CurrentUser.UserRole.ToString()).ToList();
            model.CurrentLogin = CurrentUser;
            var data = _CRFBLL.GetCrfEpaf().Where(x=> x.CrfId == null);
            model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");
            model.CurrentPageAccess = CurrentPageAccess;
            
            model.Details = Mapper.Map<List<TraCrfEpafItem>>(data);
           
            return View(model);
        }

        public ActionResult Create(long? epafId)
        {
            var model = new TraCrfItemViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            try
            {
                
                model.MainMenu = _mainMenu;
                model.CurrentLogin = CurrentUser;
                model = InitialModel(model);
                if (epafId.HasValue && epafId.Value > 0)
                {
                    var dataEpaf = _epafBLL.GetEpafById(epafId);
                    var dataFromEpaf = Mapper.Map<TraCrfItemDetails>(dataEpaf);
                    model.Detail = dataFromEpaf;
                }
                model.Detail.CreatedBy = CurrentUser.USER_ID;
                model.Detail.CreatedDate = DateTime.Now;
                var list = _employeeBLL.GetEmployee().Where(x=> x.IS_ACTIVE && x.GROUP_LEVEL > 0).Select(x => new { x.EMPLOYEE_ID, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);
                if (CurrentUser.UserRole == Enums.UserRole.HR)
                {

                    model.Detail.VehicleType = "BENEFIT";
                }
                else if (CurrentUser.UserRole == Enums.UserRole.Fleet)
                {
                    list = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).Select(x => new { x.EMPLOYEE_ID, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);
                    model.Detail.VehicleType = "WTC";
                }
                model.EmployeeList = new SelectList(list, "EMPLOYEE_ID", "FORMAL_NAME");
                model.LocationNewList = new SelectList(new List<SelectListItem>());
                model.Detail.DocumentStatus = (int)Enums.DocumentStatus.Draft;
                return View(model);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = InitialModel(model);
                model.ErrorMessage = ex.Message;
                model.LocationNewList = new SelectList(new List<SelectListItem>());
                return View(model);
            }
            
        }

        public ActionResult AssignEpaf(int MstEpafId)
        {

            try
            {

                _CRFBLL.AssignCrfFromEpaf(MstEpafId, CurrentUser);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            return RedirectToAction("Dashboard", "TraCrf");
        }

        [HttpPost]
        public ActionResult Create(TraCrfItemViewModel model)
        {
            try
            {
                model.Detail.CurrentLogin = CurrentUser;
                var dataToSave = Mapper.Map<TraCrfDto>(model.Detail);
                dataToSave.CREATED_BY = CurrentUser.USER_ID;
                dataToSave.CREATED_DATE = DateTime.Now;
                dataToSave.DOCUMENT_STATUS = (int)Enums.DocumentStatus.Draft;
                dataToSave.IS_ACTIVE = true;
                dataToSave.IsSend = model.IsSend;
                var data = _CRFBLL.SaveCrf(dataToSave, CurrentUser);
                return RedirectToAction("Edit", new { id = data.TRA_CRF_ID, isPersonalDashboard = model.IsPersonalDashboard });
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = InitialModel(model);
                model.Detail.CurrentLogin = CurrentUser;
                model.ErrorMessage = ex.Message;
                model.LocationNewList = new SelectList(new List<SelectListItem>());
                return View(model);
            }
            
        }

        public ActionResult Edit(long id,bool isPersonalDashboard)
        {
            var model = new TraCrfItemViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            
            
            var data = _CRFBLL.GetDataById(id);
            if (!_CRFBLL.IsAllowedEdit(CurrentUser,data))
            {
                return RedirectToAction("Details", new { id = data.TRA_CRF_ID, isPersonalDashboard = isPersonalDashboard });
            }
            model = InitialModel(model);
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.TraCrf, id);
            model.Detail = Mapper.Map<TraCrfItemDetails>(data);
            if (!string.IsNullOrEmpty(model.Detail.LocationCityNew))
            {
                var dataLocationNew = _employeeBLL.GetLocationByCity(model.Detail.LocationCityNew);

                model.LocationNewList = new SelectList(dataLocationNew, "Location", "Location");
            }
            model.WorkflowLogs = GetWorkflowHistory((int)Enums.MenuList.TraCrf, model.Detail.TraCrfId);

            var list = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE && x.GROUP_LEVEL > 0).Select(x => new { x.EMPLOYEE_ID, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);
            if (CurrentUser.UserRole == Enums.UserRole.Fleet)
            {
                list = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).Select(x => new { x.EMPLOYEE_ID, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);
                
            }
            model.EmployeeList = new SelectList(list, "EMPLOYEE_ID", "FORMAL_NAME");
            
            var tempData = _CRFBLL.GetTempByCsf(model.Detail.DocumentNumber);
            model.TemporaryList = Mapper.Map<List<TemporaryData>>(tempData);
            model.DetailTemporary.StartDate = DateTime.Now;
            model.DetailTemporary.EndDate = DateTime.Now;

            var RemarkList = _remarkBLL.GetRemark().Where(x => x.RoleType == CurrentUser.UserRole.ToString() && x.DocumentType == (int) Enums.DocumentType.CRF).ToList();
            model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");
            model.IsPersonalDashboard = isPersonalDashboard;
            return View(model);
        }

        public ActionResult Details(long id, bool isPersonalDashboard)
        {
            var model = new TraCrfItemViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;

            model = InitialModel(model);
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.TraCrf, id);
            model.WorkflowLogs = GetWorkflowHistory((int)Enums.MenuList.TraCsf, model.Detail.TraCrfId);
            var data = _CRFBLL.GetDataById(id);
            var dataLocations = _employeeBLL.GetLocationAll();
            model.LocationNewList = new SelectList(dataLocations, "Location", "Location");

            model.IsAllowedApprove = _CRFBLL.IsAllowedApprove(CurrentUser, data);
            model.Detail = Mapper.Map<TraCrfItemDetails>(data);

            var RemarkList =
                _remarkBLL.GetRemark()
                    .Where(
                        x =>
                            x.RoleType == CurrentUser.UserRole.ToString() &&
                            x.DocumentType == (int) Enums.DocumentType.CRF).ToList();
            model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");
            
            model.WorkflowLogs = GetWorkflowHistory((int)Enums.MenuList.TraCrf, model.Detail.TraCrfId);

            var tempData = _CRFBLL.GetTempByCsf(model.Detail.DocumentNumber);
            model.TemporaryList = Mapper.Map<List<TemporaryData>>(tempData);
            model.DetailTemporary.StartDate = DateTime.Now;
            model.DetailTemporary.EndDate = DateTime.Now;
            model.IsPersonalDashboard = isPersonalDashboard;
            return View(model);
        }

        public ActionResult Cancel(long TraCrfId)
        {
            var data = _CRFBLL.GetDataById(TraCrfId);
            data.DOCUMENT_STATUS = (int)Enums.DocumentStatus.Cancelled;
            
            _CRFBLL.SaveCrf(data, CurrentUser);

            return RedirectToAction("Index", "TraCrf");
        }

        [HttpPost]
        
        public ActionResult Edit(TraCrfItemViewModel model)
        {
            try
            {
                model.Detail.CurrentLogin = CurrentUser;
                var dataToSave = Mapper.Map<TraCrfDto>(model.Detail);
                dataToSave.IS_ACTIVE = true;
                dataToSave.MODIFIED_BY = CurrentUser.USER_ID;
                dataToSave.MODIFIED_DATE = DateTime.Now;
                _CRFBLL.SaveCrf(dataToSave, CurrentUser);
                return RedirectToAction("Edit", new { id = model.Detail.TraCrfId, isPersonalDashboard = model.IsPersonalDashboard });
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = InitialModel(model);
                model.Detail.CurrentLogin = CurrentUser;
                model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.TraCrf, model.Detail.TraCrfId);
                if (!string.IsNullOrEmpty(model.Detail.LocationCityNew))
                {
                    var dataLocationNew = _employeeBLL.GetLocationByCity(model.Detail.LocationCityNew);

                    model.LocationNewList = new SelectList(dataLocationNew, "Location", "Location");
                }
                model.WorkflowLogs = GetWorkflowHistory((int)Enums.MenuList.TraCrf, model.Detail.TraCrfId);

                var list = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE && x.GROUP_LEVEL > 0).Select(x => new { x.EMPLOYEE_ID, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);
                if (CurrentUser.UserRole == Enums.UserRole.Fleet)
                {
                    list = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).Select(x => new { x.EMPLOYEE_ID, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);

                }
                model.EmployeeList = new SelectList(list, "EMPLOYEE_ID", "FORMAL_NAME");

                var tempData = _CRFBLL.GetTempByCsf(model.Detail.DocumentNumber);
                model.TemporaryList = Mapper.Map<List<TemporaryData>>(tempData);
                model.ErrorMessage = ex.Message;

                var RemarkList = _remarkBLL.GetRemark().Where(x => x.RoleType == CurrentUser.UserRole.ToString() && x.DocumentType == (int) Enums.DocumentType.CRF).ToList();
                model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");

                return View(model);
            }
            
        }

        [HttpPost]
        public ActionResult Approve(long TraCrfId, int? remark, bool IsApproved)
        {
            var model = new TraCrfItemViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;

            try
            {
                if (IsApproved)
                {
                    _CRFBLL.Approve(TraCrfId,CurrentUser);
                }
                else
                {
                    _CRFBLL.Reject(TraCrfId, remark,CurrentUser);

                }
                return RedirectToAction("Edit", new { id = TraCrfId, isPersonalDashboard = model.IsPersonalDashboard });
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = InitialModel(model);
                
                model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.TraCrf, TraCrfId);
                model.WorkflowLogs = GetWorkflowHistory((int)Enums.MenuList.TraCsf, model.Detail.TraCrfId);
                var data = _CRFBLL.GetDataById(TraCrfId);
                var dataLocations = _employeeBLL.GetLocationAll();
                model.LocationNewList = new SelectList(dataLocations, "Location", "Location");

                model.IsAllowedApprove = _CRFBLL.IsAllowedApprove(CurrentUser, data);
                model.Detail = Mapper.Map<TraCrfItemDetails>(data);
                model.Detail.CurrentLogin = CurrentUser;
                var RemarkList = _remarkBLL.GetRemark().Where(x => x.RoleType == CurrentUser.UserRole.ToString() && x.DocumentType == (int) Enums.DocumentType.CRF).ToList();

                model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");

                return View("Edit", model);
            }

            
        }

        [HttpPost]
        
        public ActionResult Submit(TraCrfItemViewModel model)
        {
            //var model = new TraCrfItemViewModel();
            
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.Detail.CurrentLogin = CurrentUser;
            model = InitialModel(model);
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.TraCrf, (int)model.Detail.TraCrfId);
            //var data = _CRFBLL.GetDataById((int)model.Detail.TraCrfId);
            //model.Detail = Mapper.Map<TraCrfItemDetails>(data);
            try
            {
                var dataSubmit = Mapper.Map<TraCrfDto>(model.Detail);
                dataSubmit.IS_ACTIVE = true;
                _CRFBLL.SubmitCrf(dataSubmit, CurrentUser);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = InitialModel(model);
                model.Detail.CurrentLogin = CurrentUser;
                model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.TraCrf, model.Detail.TraCrfId);
                if (!string.IsNullOrEmpty(model.Detail.LocationCityNew))
                {
                    var dataLocationNew = _employeeBLL.GetLocationByCity(model.Detail.LocationCityNew);

                    model.LocationNewList = new SelectList(dataLocationNew, "Location", "Location");
                }
                model.WorkflowLogs = GetWorkflowHistory((int)Enums.MenuList.TraCrf, model.Detail.TraCrfId);

                var list = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE && x.GROUP_LEVEL > 0).Select(x => new { x.EMPLOYEE_ID, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);
                if (CurrentUser.UserRole == Enums.UserRole.Fleet)
                {
                    list = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).Select(x => new { x.EMPLOYEE_ID, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);

                }
                model.EmployeeList = new SelectList(list, "EMPLOYEE_ID", "FORMAL_NAME");

                var tempData = _CRFBLL.GetTempByCsf(model.Detail.DocumentNumber);
                model.TemporaryList = Mapper.Map<List<TemporaryData>>(tempData);
                
                model.ErrorMessage = ex.Message;
                return View("Edit",model);
            }
            
        }

        #region --------- Close EPAF --------------

        public ActionResult CloseEpaf(int EpafId, int RemarkId)
        {
            var model = new TraCrfDashboardViewModel();
            model.MainMenu = _mainMenu;
            var RemarkList = _remarkBLL.GetRemark().Where(x => x.RoleType == CurrentUser.UserRole.ToString()).ToList();
            model.CurrentLogin = CurrentUser;
            var data = _CRFBLL.GetCrfEpaf().Where(x => x.CrfId == null);
            model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");
            model.Details = Mapper.Map<List<TraCrfEpafItem>>(data);
           
            
                try
                {
                    //if (ModelState.IsValid)
                    //{
                        _epafBLL.DeactivateEpaf(EpafId, RemarkId, CurrentUser.USERNAME);
                        return RedirectToAction("Dashboard", "TraCRF");
                    //}
                    
                }
                catch (Exception ex)
                {
                    
                    AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                    //model = InitialModel(model);
                    model.ErrorMessage = ex.Message;
                    return View("Dashboard", model);
                }

            
            
        }

        #endregion
        #region --------- Get Data Post JS --------------

        [HttpPost]
        public JsonResult GetEmployee(string Id)
        {
            var model = _employeeBLL.GetByID(Id);
            FleetDto data = new FleetDto();
            if (CurrentUser.UserRole == Enums.UserRole.Fleet)
            {
                data = _fleetBLL.GetVehicleByEmployeeId(Id, "WTC");
                model.EmployeeVehicle = data;
                //model.
            }
            else
            {
                data = _fleetBLL.GetVehicleByEmployeeId(Id,"BENEFIT");
                model.EmployeeVehicle = data;
            }
            model.EmployeeVehicle = data;
            return Json(model);
        }

        
        public JsonResult GetEmployeeList()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Fleet)
            {
                var modelFleet = _fleetBLL.GetFleet().Where(x => x.VehicleType == "WTC" && x.IsActive).ToList();
                var employeeWtc = modelFleet.GroupBy(x => x.EmployeeID).Select(x => x.Key).ToList();

                var modelWtc = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE && employeeWtc.Contains(x.EMPLOYEE_ID)).Select(x => new { x.EMPLOYEE_ID, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);

                return Json(modelWtc, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var model = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE && x.GROUP_LEVEL > 0).Select(x => new { x.EMPLOYEE_ID, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            
        }

        [HttpPost]
        public JsonResult GetVehicleData(string vehType,string employeeId)
        {
            //var modelVehicle = _fleetBLL.GetFleet().Where(x => x.IsActive && x.VehicleStatus == "ACTIVE").ToList();
            var data = new List<FleetDto>();

            if (vehType == "BENEFIT")
            {
                
                //data = modelCFMIdle;

                
                
                var cfmIdleListSelected = _tempBLL.GetList().Where(x => x.DOCUMENT_STATUS != Enums.DocumentStatus.Cancelled && x.DOCUMENT_STATUS != Enums.DocumentStatus.Completed
                                                                            && x.CFM_IDLE_ID != null && x.CFM_IDLE_ID.Value > 0).Select(x => x.CFM_IDLE_ID.Value).ToList();

                //get selectedCfmIdle csf
                var cfmIdleListSelectedCsf = _csfBLL.GetList().Where(x => x.DOCUMENT_STATUS != Enums.DocumentStatus.Cancelled && x.DOCUMENT_STATUS != Enums.DocumentStatus.Completed
                                                                        && x.CFM_IDLE_ID != null && x.CFM_IDLE_ID.Value > 0).Select(x => x.CFM_IDLE_ID.Value).ToList();

                var cfmIdleListSelectedCrf = _CRFBLL.GetList().Where(x => x.DOCUMENT_STATUS != (int) Enums.DocumentStatus.Cancelled && x.MST_FLEET_ID != null && x.MST_FLEET_ID.Value > 0)
                    .Select(x => x.MST_FLEET_ID.Value).ToList();

                var fleetData = _fleetBLL.GetFleet().Where(x => x.VehicleUsage.ToUpper() == "CFM IDLE"
                                                                && x.IsActive
                                                                && !cfmIdleListSelected.Contains(x.MstFleetId)
                                                                && !cfmIdleListSelectedCsf.Contains(x.MstFleetId)
                                                                && !cfmIdleListSelectedCrf.Contains(x.MstFleetId)).ToList();

                //var modelCFMIdle = fleetData.Where(x => x.CarGroupLevel == Convert.ToInt32(groupLevel)).ToList();
                var modelCFMIdle = fleetData;
                

                var fleetDto = modelCFMIdle;

                if (modelCFMIdle.Count == 0)
                {
                    return Json(modelCFMIdle);
                }

                return Json(fleetDto);

            }

            if (vehType == "WTC")
            {
                var modelWtc = _fleetBLL.GetFleet().Where(x => x.IsActive && x.VehicleType == "WTC" && x.EmployeeID == employeeId).ToList();
                //data = modelCFMIdle;
                data = modelWtc;
                if (modelWtc.Count == 0)
                {
                    data = new List<FleetDto>();
                }

            }

            return Json(data);
        }

        [HttpPost]
        public JsonResult GetLocationByCity(string city)
        {
            var model = _employeeBLL.GetLocationByCity(city);
            var data = model.Select(x => new SelectListItem()
            {
                Text = x.Location,
                Value = x.Location
            }).OrderBy(x=> x.Text).ToList();

            return Json(data);
        }

        #endregion
        #region --------- Export --------------
        [HttpPost]
        public void ExportMasterEpaf()
        {
            
        }


        public void ExportDashboard()
        {
            string pathFile = "";

            pathFile = CreateXlsDashboard();

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

        private string CreateXlsDashboard()
        {
            //get data
            List<EpafDto> epaf = _epafBLL.GetEpafByDocType(Enums.DocumentType.CRF);
            var listData = Mapper.Map<List<EpafData>>(epaf);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Dashboard CRF");
            slDocument.MergeWorksheetCells(1, 1, 1, 12);
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

            var fileName = "Dashboard_CRF" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelDashboard(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "ePAF Effective Date");
            slDocument.SetCellValue(iRow, 2, "ePAF Approved Date");
            slDocument.SetCellValue(iRow, 3, "eLetter sent(s)");
            slDocument.SetCellValue(iRow, 4, "Action");
            slDocument.SetCellValue(iRow, 5, "Employee ID");
            slDocument.SetCellValue(iRow, 6, "Employee Name");
            slDocument.SetCellValue(iRow, 7, "Cost Centre");
            slDocument.SetCellValue(iRow, 8, "Group Level");
            //slDocument.SetCellValue(iRow, 9, "CRF No");
            //slDocument.SetCellValue(iRow, 10, "CRF Status");
            slDocument.SetCellValue(iRow, 9, "Modified By");
            slDocument.SetCellValue(iRow, 10, "Modified Date");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 10, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelDashboard(SLDocument slDocument, List<EpafData> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.EpafEffectiveDate.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 2, data.EpafApprovedDate == null ? "" : data.EpafApprovedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 3, data.LetterSend ? "Yes" : "No");
                slDocument.SetCellValue(iRow, 4, data.Action);
                slDocument.SetCellValue(iRow, 5, data.EmployeeId);
                slDocument.SetCellValue(iRow, 6, data.EmployeeName);
                slDocument.SetCellValue(iRow, 7, data.CostCentre);
                slDocument.SetCellValue(iRow, 8, data.GroupLevel);
                //slDocument.SetCellValue(iRow, 9, data.);
                //slDocument.SetCellValue(iRow, 10, data.CRFStatus);
                slDocument.SetCellValue(iRow, 9, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 10, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 10);
            slDocument.SetCellStyle(3, 1, iRow - 1, 10, valueStyle);

            return slDocument;
        }

        public void ExportOpen()
        {
            string pathFile = "";

            pathFile = CreateXlsCRF(false);

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

            pathFile = CreateXlsCRF(true);

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

        private string CreateXlsCRF(bool isCompleted)
        {
            //get data
            List<TraCrfDto> CRF = new List<TraCrfDto>(); //_CRFBLL.GetCRF();
            if (isCompleted)
            {
                CRF = _CRFBLL.GetCompleted();

            }
            else
            {
                CRF = _CRFBLL.GetList(CurrentUser);
            }
            var listData = Mapper.Map<List<TraCrfItemDetails>>(CRF);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, isCompleted ? "Completed Document CRF" : "Open Document CRF");
            slDocument.MergeWorksheetCells(1, 1, 1, 8);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelCRF(slDocument);

            //create data
            slDocument = CreateDataExcelCRF(slDocument, listData);

            var fileName = "Data_CRF" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);
           
            return path;

        }

        private SLDocument CreateHeaderExcelCRF(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "CRF No");
            slDocument.SetCellValue(iRow, 2, "CRF Status");
            slDocument.SetCellValue(iRow, 3, "Vehicle Type");
            slDocument.SetCellValue(iRow, 4, "Employee ID");
            slDocument.SetCellValue(iRow, 5, "Employee Name");
            slDocument.SetCellValue(iRow, 6, "Reason");
            slDocument.SetCellValue(iRow, 7, "Effective Date");
            slDocument.SetCellValue(iRow, 8, "Current Location");
            slDocument.SetCellValue(iRow, 9, "Relocate Location");
            slDocument.SetCellValue(iRow, 10, "Regional");
            slDocument.SetCellValue(iRow, 11, "Coordinator");
            slDocument.SetCellValue(iRow, 12, "Updated By");
            slDocument.SetCellValue(iRow, 13, "Updated Date");

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

        private SLDocument CreateDataExcelCRF(SLDocument slDocument, List<TraCrfItemDetails> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.DocumentNumber);
                slDocument.SetCellValue(iRow, 2, data.DocumentStatusString);
                slDocument.SetCellValue(iRow, 3, data.VehicleType);
                slDocument.SetCellValue(iRow, 4, data.EmployeeId);
                slDocument.SetCellValue(iRow, 5, data.EmployeeName);
                slDocument.SetCellValue(iRow, 6, data.RelocationType);
                slDocument.SetCellValue(iRow, 7, data.EffectiveDate.HasValue ? data.EffectiveDate.Value.ToString("dd-MMM-yyyy hh:mm:ss") : "");
                slDocument.SetCellValue(iRow, 8, data.LocationCity);
                slDocument.SetCellValue(iRow, 9, data.LocationCityNew);
                slDocument.SetCellValue(iRow, 10, data.Region);
                slDocument.SetCellValue(iRow, 11, data.CreatedBy);
                slDocument.SetCellValue(iRow, 12, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 13, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));

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

        public ActionResult CreateTemporary(TraCrfItemViewModel model)
        {
            bool isSuccess = false;
            try
            {
                TemporaryDto item = new TemporaryDto();

                var csfData = _CRFBLL.GetDataById(model.Detail.TraCrfId);

                if (csfData == null)
                {
                    return HttpNotFound();
                }

                item = AutoMapper.Mapper.Map<TemporaryDto>(csfData);
                item.CREATED_BY = CurrentUser.USER_ID;
                item.CREATED_DATE = DateTime.Now;
                item.DOCUMENT_STATUS = Enums.DocumentStatus.Draft;
                item.START_DATE = model.DetailTemporary.StartDate;
                item.END_DATE = model.DetailTemporary.EndDate;
                item.REASON_ID = model.DetailTemporary.ReasonId.Value;
                item.BODY_TYPE = csfData.Body;
                var fleetData = _fleetBLL.GetVehicleByEmployeeId(csfData.EMPLOYEE_ID, csfData.VEHICLE_TYPE);
                var settingData = _settingBLL.GetSetting().FirstOrDefault(x => x.SettingGroup == "VEHICLE_TYPE" && x.SettingName == csfData.VEHICLE_TYPE);
                var employeeData = _employeeBLL.GetByID(csfData.EMPLOYEE_ID);
                item.COLOR = fleetData.Color;
                item.POLICE_NUMBER = csfData.POLICE_NUMBER;
                if (settingData != null) item.VEHICLE_TYPE = settingData.MstSettingId.ToString();

                item.VENDOR_MANUFACTURER = null;
                item.VENDOR_MODEL = null;
                item.VENDOR_SERIES = null;
                item.VENDOR_BODY_TYPE = null;
                item.GROUP_LEVEL = employeeData.GROUP_LEVEL;
                var tempData = _CRFBLL.SaveTemp(item,model.Detail.ExpectedDate.Value, CurrentUser);
                
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("Details", "TraCrf", new { id = model.Detail.TraCrfId, isPersonalDashboard = model.IsPersonalDashboard });
            AddMessageInfo("Success Add Temporary Data", Enums.MessageInfoType.Success);
            return RedirectToAction("Edit", "TraCrf", new { id = model.Detail.TraCrfId, isPersonalDashboard = model.IsPersonalDashboard });
        }
    }
}
