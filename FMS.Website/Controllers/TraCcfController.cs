using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FMS.Website.Models;
using FMS.Contract.BLL;
using FMS.Core;
using AutoMapper;
using FMS.BusinessObject.Dto;
using System.Web;
using System.IO;
using ExcelDataReader;
using System.Data;
using FMS.Website.Utility;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;
using FMS.BusinessObject.Inputs;

namespace FMS.Website.Controllers
{
    public class TraCcfController : BaseController
    {

        #region --------- Field & Constructor--------------
        private IEpafBLL _epafBLL;
        private ITraCcfBLL _ccfBLL;
        private IRemarkBLL _remarkBLL;
        private IDelegationBLL _delegationBLL;
        private ISettingBLL _settingBLL;
        private IFleetBLL _fleetBLL;
        private IEmployeeBLL _employeeBLL;
        private IComplaintCategoryBLL _complaintCategoryBLL;
        private IReasonBLL _reasonBLL;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private ILocationMappingBLL _locationMappingBLL;
        public TraCcfController(IPageBLL pageBll, IEpafBLL epafBll, ITraCcfBLL ccfBll, IRemarkBLL RemarkBLL, IDelegationBLL DelegationBLL,
                                ISettingBLL settingBLL,
                                IEmployeeBLL EmployeeBLL, IReasonBLL ReasonBLL, IFleetBLL FleetBLL, IComplaintCategoryBLL complaintCategoryBLL,
                                ILocationMappingBLL LocationMappingBLL): base(pageBll, Core.Enums.MenuList.TraCtf)
        {

            _epafBLL = epafBll;
            _ccfBLL = ccfBll;
            _employeeBLL = EmployeeBLL;
            _complaintCategoryBLL = complaintCategoryBLL;
            _delegationBLL = DelegationBLL;
            _settingBLL = settingBLL;
            _pageBLL = pageBll;
            _remarkBLL = RemarkBLL;
            _reasonBLL = ReasonBLL;
            _fleetBLL = FleetBLL;
            _locationMappingBLL = LocationMappingBLL;
            _mainMenu = Enums.MenuList.Transaction;
        }
        #endregion

        #region --------- Open Document--------------
        public ActionResult Index()
        {
            var data = _ccfBLL.GetCcf();
            var model = new CcfModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.TitleForm = "Car Complaint Form";
            if (CurrentUser.EMPLOYEE_ID == "")
            {
                return RedirectToAction("Unauthorized", "Error");
            }
            else
            {
                if (CurrentUser.UserRole == Enums.UserRole.HR)
                {
                    model.Details = Mapper.Map<List<CcfItem>>(data.Where(x => 
                    (x.DocumentStatus == Enums.DocumentStatus.AssignedForHR && x.ComplaintCategoryRole == "HR") || 
                    (x.DocumentStatus == Enums.DocumentStatus.InProgress && x.ComplaintCategoryRole == "HR") || 
                    (x.DocumentStatus == Enums.DocumentStatus.Draft && x.ComplaintCategoryRole == "HR") ||
                    (x.EmployeeID == CurrentUser.EMPLOYEE_ID)
                    ));
                }
                else if (CurrentUser.UserRole == Enums.UserRole.Fleet)
                {
                    model.Details = Mapper.Map<List<CcfItem>>(data.Where(x => 
                    (x.DocumentStatus == Enums.DocumentStatus.AssignedForFleet && x.ComplaintCategoryRole == "Fleet") || 
                    (x.DocumentStatus == Enums.DocumentStatus.InProgress && x.ComplaintCategoryRole == "Fleet") || 
                    (x.DocumentStatus == Enums.DocumentStatus.Draft && x.ComplaintCategoryRole == "Fleet") ||
                    (x.EmployeeID == CurrentUser.EMPLOYEE_ID)
                    ));
                }
                else 
                {
                    model.Details = Mapper.Map<List<CcfItem>>(data.Where(x => x.EmployeeID == CurrentUser.EMPLOYEE_ID));
                }
                return View("Index",model);
            }
        }

        public ActionResult Completed()
        {
            var data = _ccfBLL.GetCcf();
            var model = new CcfModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.TitleForm = "Car Complaint Form";
            if (CurrentUser.EMPLOYEE_ID == "")
            {
                return RedirectToAction("Unauthorized", "Error");
            }
            else
            {
                if (CurrentUser.UserRole == Enums.UserRole.HR)
                {
                    model.Details = Mapper.Map<List<CcfItem>>(data.Where(x => (x.DocumentStatus == Enums.DocumentStatus.AssignedForHR && x.ComplaintCategoryRole == "HR") || (x.DocumentStatus == Enums.DocumentStatus.Completed && x.ComplaintCategoryRole == "HR")));
                }
                else if (CurrentUser.UserRole == Enums.UserRole.Fleet)
                {
                    model.Details = Mapper.Map<List<CcfItem>>(data.Where(x => (x.DocumentStatus == Enums.DocumentStatus.AssignedForFleet && x.ComplaintCategoryRole == "Fleet") || (x.DocumentStatus == Enums.DocumentStatus.Completed && x.ComplaintCategoryRole == "Fleet")));
                }
                else
                {
                    model.Details = Mapper.Map<List<CcfItem>>(data.Where(x => x.EmployeeID == CurrentUser.EMPLOYEE_ID));
                }
                return View("Index", model);
            }
        }
        #endregion

        #region ---------  Details --------------
        public ActionResult DetailsCcf(int? TraCcfId, bool IsPersonalDashboard)
        {
            if (!TraCcfId.HasValue)
            {
                return HttpNotFound();
            }

            var ctfData = _ccfBLL.GetCcf().Where(x => x.TraCcfId == TraCcfId.Value).FirstOrDefault();

            if (ctfData == null)
            {
                return HttpNotFound();
            }
            try
            {
                var model = new CcfItem();
                model = Mapper.Map<CcfItem>(ctfData);
                //model.IsPersonalDashboard = IsPersonalDashboard;
                model = listdata(model, model.EmployeeID);
                model.CurrentLogin = CurrentUser;
                model = initCreate(model);
                model.TitleForm = "Car Complaint Form Details";
                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
            }
        }

        #endregion

        #region --------- Create --------------
        public CcfItem initCreate(CcfItem model)
        {
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.TraCcf, model.TraCcfId);
            model.WorkflowLogs = GetWorkflowHistory((int)Enums.MenuList.TraCcf, model.TraCcfId);
            return model;
        }

        public CcfItem listdata(CcfItem model, string IdEmployee)
        {
            var listemployeefromdelegation = _delegationBLL.GetDelegation().Select(x => new { dataemployeefrom = x.EmployeeFrom + x.NameEmployeeFrom, x.EmployeeFrom, x.NameEmployeeFrom, x.EmployeeTo, x.NameEmployeeTo, x.DateTo }).ToList().Where(x => x.EmployeeTo == CurrentUser.EMPLOYEE_ID && x.DateTo >= DateTime.Today).OrderBy(x => x.EmployeeFrom);
            model.EmployeeFromDelegationList = new SelectList(listemployeefromdelegation, "EmployeeFrom", "dataemployeefrom");

            var listcomplaintcategory = _complaintCategoryBLL.GetComplaints().Select(x => new { x.MstComplaintCategoryId, x.CategoryName }).ToList().OrderBy(x => x.MstComplaintCategoryId);
            model.ComplaintCategoryList = new SelectList(listcomplaintcategory, "MstComplaintCategoryId", "CategoryName");

            var listsettingvtype = _settingBLL.GetSetting().Select(x => new { x.SettingGroup, x.SettingName, x.SettingValue }).ToList().Where(x => x.SettingGroup == "VEHICLE_TYPE").OrderBy(x => x.SettingValue);
            model.SettingListVType = new SelectList(listsettingvtype, "SettingValue", "SettingName");

            var listsettingvusage = _settingBLL.GetSetting().Select(x => new { x.SettingGroup, x.SettingName, x.SettingValue }).ToList().Where(x => x.SettingGroup == "VEHICLE_USAGE").OrderBy(x => x.SettingValue);
            model.SettingListVUsage = new SelectList(listsettingvusage, "SettingValue", "SettingName");

            var listsettingfleet = _fleetBLL.GetFleet().Select(x => new { x.MstFleetId, x.VehicleType, x.VehicleUsage, x.PoliceNumber, x.EmployeeID, x.EmployeeName, x.Manufacturer, x.Models, x.Series, x.VendorName, x.StartContract, x.EndContract }).ToList().Where(x => x.EmployeeID == IdEmployee).OrderBy(x => x.EmployeeName);
            model.SettingListFleet = new SelectList(listsettingfleet, "PoliceNumber", "PoliceNumber");

            return model;
        }

        public ActionResult GetData(string id)
        {
            var model = new CcfItem();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;

            var data = _employeeBLL.GetByID(id);
            model.EmployeeID = data.EMPLOYEE_ID;
            model.EmployeeName = data.FORMAL_NAME;
            //model.EmployeeAddress = data.ADDRESS;
            //model.EmployeeCity = data.CITY;
            model = listdata(model, id);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataComplaint(int id)
        {
            var model = new CcfItem();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;

            var data = _complaintCategoryBLL.GetByID(id);
            model.ComplaintCategoryRole = data.RoleType;

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataFleet(string id)
        {
            var model = new CcfItem();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;

            var DataFletByEmployee = _fleetBLL.GetFleet().Where(x => x.PoliceNumber == id).FirstOrDefault();
            model.VehicleType = DataFletByEmployee.VehicleType;
            model.VehicleUsage = DataFletByEmployee.VehicleUsage;
            model.Manufacturer = DataFletByEmployee.Manufacturer;
            model.Models = DataFletByEmployee.Models;
            model.Series = DataFletByEmployee.Series;
            model.Vendor = DataFletByEmployee.VendorName;
            //model.VStartPeriod = DataFletByEmployee.StartContract.Value.ToString("dd-MMM-yyyy");
            //model.VEndPeriod = DataFletByEmployee.EndContract.Value.ToString("dd-MMM-yyyy");
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateCcf()
        {
            var model = new CcfItem();
            model.MainMenu = _mainMenu;

            try
            {
                model.CurrentLogin = CurrentUser;
                model.EmployeeID = CurrentUser.EMPLOYEE_ID;
                model.EmployeeIdComplaintFor = CurrentUser.EMPLOYEE_ID;
                model.CreatedDate = DateTime.Today;

                var data = _employeeBLL.GetByID(CurrentUser.EMPLOYEE_ID);
                model.EmployeeName = data.FORMAL_NAME;
                model.EmployeeNameComplaintFor = data.FORMAL_NAME;
                model.LocationAddress = data.ADDRESS;
                model.LocationCity = data.CITY;
                model.TitleForm = "Create Car Complaint Form";
                //model.VCreatedDate = null;
                model = listdata(model, model.EmployeeID);

                model.DocumentStatus = Enums.DocumentStatus.Draft;
                model.DocumentStatusDoc = Enums.DocumentStatus.Draft.ToString();
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateCcf(CcfItem Model)
        {
            var a = ModelState;
            try
            {
                Model.CreatedBy = CurrentUser.USER_ID;
                Model.CreatedDate = DateTime.Now;
                Model.DocumentStatus = Enums.DocumentStatus.Draft;
                Model.IsActive = true;
                if (Model.isSubmit == "submit")
                {
                    if (Model.ComplaintCategoryRole == "HR")
                    {
                        Model.DocumentStatus = Enums.DocumentStatus.AssignedForHR;
                    }
                    else if (Model.ComplaintCategoryRole == "Fleet")
                    {
                        Model.DocumentStatus = Enums.DocumentStatus.AssignedForFleet;
                    }
                }
                var Dto = Mapper.Map<TraCcfDto>(Model);
                var CcfData = _ccfBLL.Save(Dto, CurrentUser);

                if (Model.isSubmit == "submit")
                {
                    CcfWorkflow(CcfData.TraCcfId, Enums.ActionType.Submit, null, false);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("DetailsCcf", "TraCcf", new { TraCcfId = CcfData.TraCcfId });
                }

                return RedirectToAction("Index", "TraCcf");
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                Model = listdata(Model, Model.EmployeeID);
                Model.TitleForm = "Car Complaint Form Create";
                Model.ErrorMessage = exception.Message;
                Model.CurrentLogin = CurrentUser;
                return View(Model);
            }
        }
        #endregion

        #region --------- Edit --------------
        public ActionResult EditCcf(int? TraCcfId, bool IsPersonalDashboard)
        {
            var model = new CcfItem();
            model.MainMenu = _mainMenu;

            if (!TraCcfId.HasValue)
            {
                return HttpNotFound();
            }

            var ccfData = _ccfBLL.GetCcf().Where(x => x.TraCcfId == TraCcfId.Value).FirstOrDefault();

            if (ccfData == null)
            {
                return HttpNotFound();
            }

            if (ccfData.DocumentStatus == Enums.DocumentStatus.AssignedForHR)
            {
                return RedirectToAction("ResponseHR", "TraCcf", new { TraCcfId = ccfData.TraCcfId, IsPersonalDashboard = IsPersonalDashboard });
            }
            else if (ccfData.DocumentStatus == Enums.DocumentStatus.AssignedForFleet)
            {
                return RedirectToAction("ResponseFleet", "TraCcf", new { TraCcfId = ccfData.TraCcfId, IsPersonalDashboard = IsPersonalDashboard });
            }
            else 
            {
                if (ccfData.DocumentStatus == Enums.DocumentStatus.Draft)
                {
                    return RedirectToAction("EditCcfUser", "TraCcf", new { TraCcfId = ccfData.TraCcfId, IsPersonalDashboard = IsPersonalDashboard });
                }
                else
                {
                    return RedirectToAction("DetailsCcfUser", "TraCcf", new { TraCcfId = ccfData.TraCcfId, IsPersonalDashboard = IsPersonalDashboard });
                }
            }

            try
            {
                model.CurrentLogin = CurrentUser;
                model = Mapper.Map<CcfItem>(ccfData);
                model.IsPersonalDashboard = IsPersonalDashboard;
                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                //return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
            }
        }
        #endregion

        #region ------- Edit For User ---------
        public ActionResult EditCcfUser(int? TraCcfId, bool IsPersonalDashboard)
        {
            var model = new CcfItem();
            model.MainMenu = _mainMenu;

            if (!TraCcfId.HasValue)
            {
                return HttpNotFound();
            }

            var ccfData = _ccfBLL.GetCcf().Where(x => x.TraCcfId == TraCcfId.Value).FirstOrDefault();

            if (ccfData == null)
            {
                return HttpNotFound();
            }

            try
            {
                model = Mapper.Map<CcfItem>(ccfData);
                model.IsPersonalDashboard = IsPersonalDashboard;
                model.EmployeeID = CurrentUser.EMPLOYEE_ID;
                model = listdata(model, model.EmployeeID);
                model.CurrentLogin = CurrentUser;
                model.TitleForm = "Car Complaint Form Edit";
                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EditCcfUser(CcfItem model)
        {
            var a = ModelState.IsValid;
            try
            {
                var dataToSave = Mapper.Map<TraCcfDto>(model);
                if (model.ComplaintCategoryRole == "HR")
                {
                    dataToSave.DocumentStatus = Enums.DocumentStatus.AssignedForHR;
                }
                else if (model.ComplaintCategoryRole == "Fleet")
                {
                    dataToSave.DocumentStatus = Enums.DocumentStatus.AssignedForFleet;
                }
                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;
                var saveResult = _ccfBLL.Save(dataToSave, CurrentUser);

                bool isSubmit = model.isSubmit == "submit";
                if (isSubmit)
                {
                    CcfWorkflow(model.TraCcfId, Enums.ActionType.Submit, null, false);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("DetailsCcf", "TraCcf", new { @TraCcfId = model.TraCcfId, @IsPersonalDashboard = model.IsPersonalDashboard });
                }
                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                return RedirectToAction(model.IsPersonalDashboard ? "PersonalDashboard" : "Index");

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                //model = initCreate(model, "benefit");
                model.CurrentLogin = CurrentUser;
                return View(model);
            }
        }

        public ActionResult ResponseHR(int? TraCcfId, bool IsPersonalDashboard)
        {
            var model = new CcfItem();
            model.MainMenu = _mainMenu;

            if (!TraCcfId.HasValue)
            {
                return HttpNotFound();
            }

            var ccfData = _ccfBLL.GetCcf().Where(x => x.TraCcfId == TraCcfId.Value).FirstOrDefault();

            if (ccfData == null)
            {
                return HttpNotFound();
            }

            try
            {
                model = Mapper.Map<CcfItem>(ccfData);
                model.IsPersonalDashboard = IsPersonalDashboard;
                model.EmployeeID = CurrentUser.EMPLOYEE_ID;
                model = listdata(model, model.EmployeeID);
                model.CurrentLogin = CurrentUser;
                model = initCreate(model);
                model.TitleForm = "Car Complaint Form Respone HR";
                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ResponseHR(CcfItem model)
        {
            var a = ModelState.IsValid;
            try
            {
                var dataToSave = Mapper.Map<TraCcfDto>(model);
                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;
                dataToSave.DocumentStatus = Enums.DocumentStatus.InProgress;
                var saveResult = _ccfBLL.Save(dataToSave, CurrentUser);

                bool isSubmit = model.isSubmit == "submit";
                if (isSubmit)
                {
                    CcfWorkflow(model.TraCcfId, Enums.ActionType.Submit, null, false);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("DetailsCcf", "TraCcf", new { @TraCcfId = model.TraCcfId, @IsPersonalDashboard = model.IsPersonalDashboard });
                }
                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                return RedirectToAction(model.IsPersonalDashboard ? "PersonalDashboard" : "Index");

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                //model = initCreate(model, "benefit");
                model.CurrentLogin = CurrentUser;
                return View(model);
            }
        }

        public ActionResult ResponseFleet(int? TraCcfId, bool IsPersonalDashboard)
        {
            var model = new CcfItem();
            model.MainMenu = _mainMenu;

            if (!TraCcfId.HasValue)
            {
                return HttpNotFound();
            }

            var ccfData = _ccfBLL.GetCcf().Where(x => x.TraCcfId == TraCcfId.Value).FirstOrDefault();

            if (ccfData == null)
            {
                return HttpNotFound();
            }

            try
            {
                model = Mapper.Map<CcfItem>(ccfData);
                model.IsPersonalDashboard = IsPersonalDashboard;
                model.EmployeeID = CurrentUser.EMPLOYEE_ID;
                model = listdata(model, model.EmployeeID);
                model.CurrentLogin = CurrentUser;
                model = initCreate(model);
                model.TitleForm = "Car Complaint Form Respone Fleet";
                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ResponseFleet(CcfItem model)
        {
            var a = ModelState.IsValid;
            try
            {
                var dataToSave = Mapper.Map<TraCcfDto>(model);
                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;
                dataToSave.DocumentStatus = Enums.DocumentStatus.InProgress;
                var saveResult = _ccfBLL.Save(dataToSave, CurrentUser);

                bool isSubmit = model.isSubmit == "submit";
                if (isSubmit)
                {
                    CcfWorkflow(model.TraCcfId, Enums.ActionType.Submit, null, false);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("DetailsCcf", "TraCcf", new { @TraCcfId = model.TraCcfId, @IsPersonalDashboard = model.IsPersonalDashboard });
                }
                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                return RedirectToAction(model.IsPersonalDashboard ? "PersonalDashboard" : "Index");

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                //model = initCreate(model, "benefit");
                model.CurrentLogin = CurrentUser;
                return View(model);
            }
        }

        #endregion

        #region --------- CCF Workflow --------------
        private void CcfWorkflow(long id, Enums.ActionType actionType, int? comment, bool Endrent)
        {
            var input = new CcfWorkflowDocumentInput
            {
                DocumentId = id,
                UserId = CurrentUser.USER_ID,
                UserRole = CurrentUser.UserRole,
                ActionType = actionType,
                Comment = comment,
                EndRent = Endrent
            };

            _ccfBLL.CcfWorkflow(input);
        }

        #endregion
    }
}
