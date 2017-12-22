using System;
using System.Collections.Generic;
using System.Configuration;
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
using FMS.Contract.Service;

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
                                ILocationMappingBLL LocationMappingBLL) : base(pageBll, Core.Enums.MenuList.TraCcf)
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
                    model.Details = Mapper.Map<List<CcfItem>>(data.Where(x => (
                    (x.DocumentStatus == Enums.DocumentStatus.AssignedForHR || (x.DocumentStatus == Enums.DocumentStatus.InProgress && x.ComplaintCategoryRole == "HR")
                    //||(x.DocumentStatus != Enums.DocumentStatus.Completed && x.CreatedBy == CurrentUser.USER_ID)
                    ))).OrderBy(x=>x.DocumentNumber));
                }
                else if (CurrentUser.UserRole == Enums.UserRole.Fleet)
                {
                    model.Details = Mapper.Map<List<CcfItem>>(data.Where(x => (
                    (x.DocumentStatus == Enums.DocumentStatus.AssignedForFleet || (x.DocumentStatus == Enums.DocumentStatus.InProgress && x.ComplaintCategoryRole == "Fleet")
                    //||(x.DocumentStatus != Enums.DocumentStatus.Completed && x.CreatedBy == CurrentUser.USER_ID)
                    ))).OrderBy(x => x.DocumentNumber));
                }
                else if (CurrentUser.UserRole == Enums.UserRole.Viewer)
                {
                    model.Details = Mapper.Map<List<CcfItem>>(data.Where(x => x.DocumentStatus != Enums.DocumentStatus.Completed && x.CreatedBy == CurrentUser.USER_ID).OrderBy(x => x.DocumentNumber));
                    //model.Details = Mapper.Map<List<CcfItem>>(data.Where(x => (
                    //x.DocumentStatus == Enums.DocumentStatus.AssignedForFleet || x.DocumentStatus == Enums.DocumentStatus.AssignedForHR ||
                    //x.DocumentStatus == Enums.DocumentStatus.InProgress)
                    //));
                }
                else
                {
                    model.Details = Mapper.Map<List<CcfItem>>(data.Where(x => x.DocumentStatus != Enums.DocumentStatus.Completed && x.CreatedBy == CurrentUser.USER_ID).OrderBy(x => x.DocumentNumber));
                }
                return View("Index", model);
            }
        }

        public ActionResult Completed()
        {
            var data = _ccfBLL.GetCcf();
            var model = new CcfModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.DocumentStatus = "Completed";
            model.TitleForm = "Car Complaint Form";
            if (CurrentUser.EMPLOYEE_ID == "")
            {
                return RedirectToAction("Unauthorized", "Error");
            }
            else
            {
                if (CurrentUser.UserRole == Enums.UserRole.HR)
                {
                    model.Details = Mapper.Map<List<CcfItem>>(data.Where(
                        x => (x.DocumentStatus == Enums.DocumentStatus.Completed && x.ComplaintCategoryRole == "HR") ||
                        (x.DocumentStatus == Enums.DocumentStatus.Completed && x.CreatedBy == CurrentUser.USER_ID)).OrderBy(x => x.DocumentNumber));
                }
                else if (CurrentUser.UserRole == Enums.UserRole.Fleet)
                {
                    model.Details = Mapper.Map<List<CcfItem>>(data.Where(
                        x => (x.DocumentStatus == Enums.DocumentStatus.Completed && x.ComplaintCategoryRole == "Fleet") ||
                        (x.DocumentStatus == Enums.DocumentStatus.Completed && x.CreatedBy == CurrentUser.USER_ID)).OrderBy(x => x.DocumentNumber));
                }
                else if (CurrentUser.UserRole == Enums.UserRole.Viewer)
                {
                    model.Details = Mapper.Map<List<CcfItem>>(data.Where(x => (x.DocumentStatus == Enums.DocumentStatus.Completed) || (x.DocumentStatus == Enums.DocumentStatus.Completed && x.CreatedBy == CurrentUser.USER_ID)).OrderBy(x => x.DocumentNumber));
                }
                else
                {
                    model.Details = Mapper.Map<List<CcfItem>>(data.Where(x => x.DocumentStatus == Enums.DocumentStatus.Completed && x.CreatedBy == CurrentUser.USER_ID).OrderBy(x => x.DocumentNumber));
                }
                return View("Index", model);
            }
        }
        #endregion

        #region --------- Personal Dashboard --------------

        public ActionResult PersonalDashboard()
        {
            var data = _ccfBLL.GetCcf().Where(x => x.CreatedBy == CurrentUser.USER_ID);

            if (CurrentUser.EMPLOYEE_ID == "")
            {
                return RedirectToAction("Unauthorized", "Error");
            }
            else
            {
                if (CurrentUser.UserRole == Enums.UserRole.HR)
                {
                    data = _ccfBLL.GetCcf().Where(x => x.CreatedBy == CurrentUser.USER_ID || x.DocumentStatus == Enums.DocumentStatus.AssignedForHR || (x.DocumentStatus == Enums.DocumentStatus.InProgress && x.ComplaintCategoryRole == "HR")).OrderBy(x => x.DocumentNumber);
                }
                else if (CurrentUser.UserRole == Enums.UserRole.Fleet)
                {
                    data = _ccfBLL.GetCcf().Where(x => x.CreatedBy == CurrentUser.USER_ID || x.DocumentStatus == Enums.DocumentStatus.AssignedForFleet || (x.DocumentStatus == Enums.DocumentStatus.InProgress && x.ComplaintCategoryRole == "Fleet")).OrderBy(x => x.DocumentNumber);
                }
                else if (CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.User)
                {
                    data = _ccfBLL.GetCcf().Where(x => x.CreatedBy == CurrentUser.USER_ID || x.EmployeeIdComplaintFor == CurrentUser.EMPLOYEE_ID).OrderBy(x => x.DocumentNumber);
                }
            }
            
            var model = new CcfModel();
            model.TitleForm = "CCF Personal Dashboard";
            model.Details = Mapper.Map<List<CcfItem>>(data);
            model.MainMenu = Enums.MenuList.PersonalDashboard;
            model.CurrentLogin = CurrentUser;
            model.IsPersonalDashboard = true;
            var locationMapping = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).OrderByDescending(x => x.ValidFrom).ToList();

            try
            {
                foreach (var item in model.Details)
                {
                    var data_temp1 = _ccfBLL.GetCcfDetil().Where(x => x.TraCcfId == item.TraCcfId && x.VendorPromiseDate != null).Max(x => x.TraCcfDetilId);
                    var data_temp2 = _ccfBLL.GetCcfDetil().Where(x => x.TraCcfDetilId == data_temp1).Select(x => x.VendorPromiseDate).FirstOrDefault();
                    if (data_temp2 != null)
                    {
                        item.StsTraCcfId = data_temp2.ToString();
                        item.StsVndrDate = data_temp2;
                    }

                    if (locationMapping != null)
                    {
                        var region = locationMapping.Where(x => x.Location.ToUpper() == item.LocationCity.ToUpper()).FirstOrDefault();
                        item.Region = region == null ? string.Empty : region.Region;
                    }
                }
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return View(model);
            }
            return View(model);
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
            var ccfDataD1 = _ccfBLL.GetCcfD1(TraCcfId.Value);

            if (ctfData == null)
            {
                return HttpNotFound();
            }
            try
            {
                var model = new CcfItem();
                model = Mapper.Map<CcfItem>(ctfData);
                model.Details_d1 = Mapper.Map<List<CcfItemDetil>>(ccfDataD1);
                var fleetData = _fleetBLL.GetFleet().Where(x => x.PoliceNumber == model.PoliceNumber).FirstOrDefault();
                if (fleetData != null)
                {
                    model.VStartPeriod = fleetData.StartContract.Value.ToString("dd-MMM-yyyy");
                    model.VEndPeriod = fleetData.EndContract.Value.ToString("dd-MMM-yyyy");
                }
                if (model.EmployeeIdComplaintFor != null)
                {
                    model = listdata(model, model.EmployeeIdComplaintFor);
                }
                else
                {
                    model = listdata(model, model.EmployeeID);
                }
                model.IsPersonalDashboard = IsPersonalDashboard;
                model.CurrentLogin = CurrentUser;
                model = initCreate(model);
                model.TitleForm = "Car Complaint Form Details";
                model.MainMenu = _mainMenu;
                return View(model);
            }
            catch (Exception exception)
            {
                var model = new CcfItem();
                model = Mapper.Map<CcfItem>(ctfData);
                model.ErrorMessage = exception.Message;
                model.IsPersonalDashboard = IsPersonalDashboard;
                model = listdata(model, model.EmployeeID);
                model.CurrentLogin = CurrentUser;
                model = initCreate(model);
                model.TitleForm = "Car Complaint Form Details";
                model.MainMenu = _mainMenu;
                return View(model);
            }
        }

        #endregion

        #region --------- Create --------------
        public CcfItem initCreate(CcfItem model)
        {
            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
            model.ConfigUrl = webRootUrl;
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.TraCcf, model.TraCcfId);
            model.WorkflowLogs = GetWorkflowHistory((int)Enums.MenuList.TraCcf, model.TraCcfId);
            return model;
        }

        public CcfItem listdata(CcfItem model, string IdEmployee)
        {
            var listemployeefromdelegation = _delegationBLL.GetDelegation().Select(x => new { dataemployeefrom = x.EmployeeFrom + x.NameEmployeeFrom, x.EmployeeFrom, x.NameEmployeeFrom, x.EmployeeTo, x.NameEmployeeTo, x.DateTo }).ToList().Where(x => x.EmployeeTo == CurrentUser.EMPLOYEE_ID && x.DateTo >= DateTime.Today).OrderBy(x => x.EmployeeFrom);
            model.EmployeeFromDelegationList = new SelectList(listemployeefromdelegation, "EmployeeFrom", "dataemployeefrom");
            //var listemployeefromdelegation = CurrentUser.LoginFor;
            //model.EmployeeFromDelegationList = new SelectList(listemployeefromdelegation, "EMPLOYEE_ID", "EMPLOYEE_NAME");

            var listcomplaintcategory = _complaintCategoryBLL.GetComplaints().Select(x => new { x.MstComplaintCategoryId, x.CategoryName }).ToList().OrderBy(x => x.MstComplaintCategoryId);
            model.ComplaintCategoryList = new SelectList(listcomplaintcategory, "MstComplaintCategoryId", "CategoryName");

            var listsettingvtype = _settingBLL.GetSetting().Select(x => new { x.SettingGroup, x.SettingName, x.SettingValue }).ToList().Where(x => x.SettingGroup == "VEHICLE_TYPE").OrderBy(x => x.SettingValue);
            model.SettingListVType = new SelectList(listsettingvtype, "SettingValue", "SettingName");

            var listsettingvusage = _settingBLL.GetSetting().Select(x => new { x.SettingGroup, x.SettingName, x.SettingValue }).ToList().Where(x => x.SettingGroup == "VEHICLE_USAGE").OrderBy(x => x.SettingValue);
            model.SettingListVUsage = new SelectList(listsettingvusage, "SettingValue", "SettingName");

            var listsettingfleet = _fleetBLL.GetFleet().Select(x => new { x.MstFleetId, x.VehicleType, x.VehicleUsage, x.PoliceNumber, x.EmployeeID, x.EmployeeName, x.Manufacturer, x.Models, x.Series, x.VendorName, x.StartContract, x.EndContract, x.IsActive }).ToList().Where(x => x.EmployeeID == IdEmployee && x.IsActive == true).OrderBy(x => x.EmployeeName);
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
            model.EmployeeAddress = data.ADDRESS;
            model.EmployeeCity = data.CITY;
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
            model.VStartPeriod = DataFletByEmployee.StartContract.Value.ToString("dd-MMM-yyyy");
            model.VEndPeriod = DataFletByEmployee.EndContract.Value.ToString("dd-MMM-yyyy");
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateCcf(bool IsPersonalDashboard)
        {
            var model = new CcfItem();
            model.MainMenu = _mainMenu;

            try
            {
                model.CurrentLogin = CurrentUser;
                model.EmployeeID = CurrentUser.EMPLOYEE_ID;
                model.EmployeeIdComplaintFor = CurrentUser.EMPLOYEE_ID;
                model.CreatedDate = DateTime.Today;
                if (CurrentUser.EMPLOYEE_ID == "")
                {
                    return RedirectToAction("Unauthorized", "Error");
                }
                var data = _employeeBLL.GetByID(CurrentUser.EMPLOYEE_ID);
                model.EmployeeName = data.FORMAL_NAME;
                model.EmployeeNameComplaintFor = data.FORMAL_NAME;
                model.LocationAddress = data.ADDRESS;
                model.LocationCity = data.CITY;
                model.IsPersonalDashboard = IsPersonalDashboard;
                model.TitleForm = "Create Car Complaint Form";
                model = listdata(model, model.EmployeeID);
                model.DocumentStatus = Enums.DocumentStatus.Draft;
                model.DocumentStatusDoc = Enums.DocumentStatus.Draft.ToString();
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = listdata(model, model.EmployeeID);
                model.TitleForm = "Car Complaint Form Create";
                model.ErrorMessage = exception.Message;
                model.CurrentLogin = CurrentUser;
                return View(model);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateCcf(CcfItem Model, HttpPostedFileBase ComplaintAtt)
        {
            try
            {
                Model.CreatedBy = CurrentUser.USER_ID;
                Model.StartPeriod = Convert.ToDateTime(Model.VStartPeriod);
                Model.EndPeriod = Convert.ToDateTime(Model.VEndPeriod) ;

                Model.CreatedDate = DateTime.Now;
                Model.DocumentStatus = Enums.DocumentStatus.Draft;
                Model.IsActive = true;

                Model.DocumentNumber = _ccfBLL.GetNumber();

                string url = Model.DocumentNumber.Replace("/", "_");
                string path = "~/files_upload/CCF/" + Model.DocumentNumber.Replace("/", "_");

                if (!Directory.Exists(path))
                {
                    DirectoryInfo di = Directory.CreateDirectory(Server.MapPath(path));
                }

                if (ComplaintAtt != null)
                {
                    string filename = System.IO.Path.GetFileName(ComplaintAtt.FileName);
                    ComplaintAtt.SaveAs(Server.MapPath("~/files_upload/CCF/" + url + '/' + filename));
                    Model.DetailSave.ComplaintAtt = filename;
                    Model.DetailSave.ComplaintUrl = "/files_upload/CCF/" + url;
                }

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
                    return RedirectToAction("DetailsCcf", "TraCcf", new { TraCcfId = CcfData.TraCcfId, IsPersonalDashboard = Model.IsPersonalDashboard });
                }
                AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
                CcfWorkflow(CcfData.TraCcfId, Enums.ActionType.Created, null, false);
                return RedirectToAction("EditCcfUser", "TraCcf", new { @TraCcfId = CcfData.TraCcfId, @IsPersonalDashboard = Model.IsPersonalDashboard });
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

        #region ------- Edit ---------
        public ActionResult EditCcfUser(int? TraCcfId, bool IsPersonalDashboard)
        {
            var model = new CcfItem();
            

            if (!TraCcfId.HasValue)
            {
                return HttpNotFound();
            }

            var ccfData = _ccfBLL.GetCcf().Where(x => x.TraCcfId == TraCcfId.Value).FirstOrDefault();
            var ccfDataD1 = _ccfBLL.GetCcfD1(TraCcfId.Value);

            if (ccfData == null)
            {
                return HttpNotFound();
            }

            try
            {
                model = Mapper.Map<CcfItem>(ccfData);
                model.Details_d1 = Mapper.Map<List<CcfItemDetil>>(ccfDataD1);
                var fleetData = _fleetBLL.GetFleet().Where(x => x.PoliceNumber == model.PoliceNumber).FirstOrDefault();
                model.IsPersonalDashboard = IsPersonalDashboard;
                model.EmployeeID = CurrentUser.EMPLOYEE_ID;
                if (fleetData != null)
                {
                    model.VStartPeriod = fleetData.StartContract.Value.ToString("dd-MMM-yyyy");
                    model.VEndPeriod = fleetData.EndContract.Value.ToString("dd-MMM-yyyy");
                }
                if (model.EmployeeIdComplaintFor != null)
                {
                    model = listdata(model, model.EmployeeIdComplaintFor);
                }
                else
                {
                    model = listdata(model, model.EmployeeID);
                }
                model = initCreate(model);
                model.CurrentLogin = CurrentUser;
                model.TitleForm = "Car Complaint Form";
                model.MainMenu = _mainMenu;
                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = listdata(model, model.EmployeeID);
                model.IsPersonalDashboard = IsPersonalDashboard;
                model.TitleForm = "Car Complaint Form";
                model.ErrorMessage = exception.Message;
                model.CurrentLogin = CurrentUser;
                model = initCreate(model);
                return View(model);
                //return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EditCcfUser(CcfItem model, HttpPostedFileBase ComplaintAtt)
        {
            try
            {
                var dataToSave = Mapper.Map<TraCcfDto>(model);

                bool isSubmit = model.isSubmit == "submit";
                string url = dataToSave.DocumentNumber.Replace("/", "_");
                string path = "~/files_upload/CCF/" + dataToSave.DocumentNumber.Replace("/", "_");

                if (!Directory.Exists(path))
                {
                    DirectoryInfo di = Directory.CreateDirectory(Server.MapPath(path));
                }

                if (isSubmit)
                {
                    if (model.ComplaintCategoryRole == "HR")
                    {
                        dataToSave.DocumentStatus = Enums.DocumentStatus.AssignedForHR;
                    }
                    else if (model.ComplaintCategoryRole == "Fleet")
                    {
                        dataToSave.DocumentStatus = Enums.DocumentStatus.AssignedForFleet;
                    }
                }

                if (ComplaintAtt != null)
                {
                    string filename = System.IO.Path.GetFileName(ComplaintAtt.FileName);
                    ComplaintAtt.SaveAs(Server.MapPath("~/files_upload/CCF/" + url + '/' + filename));
                    dataToSave.DetailSave.ComplaintAtt = filename;
                    dataToSave.DetailSave.ComplaintUrl = "/files_upload/CCF/" + url;
                }

                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;
                dataToSave.StartPeriod = Convert.ToDateTime(model.VStartPeriod);
                dataToSave.EndPeriod = Convert.ToDateTime(model.VEndPeriod);
                var saveResult = _ccfBLL.Save(dataToSave, CurrentUser);
                
                if (isSubmit)
                {
                    CcfWorkflow(model.TraCcfId, Enums.ActionType.Submit, null, false);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("DetailsCcf", "TraCcf", new { @TraCcfId = model.TraCcfId, @IsPersonalDashboard = model.IsPersonalDashboard });
                }
                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                CcfWorkflow(model.TraCcfId, Enums.ActionType.Modified, null, false);
                return RedirectToAction("EditCcfUser", "TraCcf", new { @TraCcfId = model.TraCcfId, @IsPersonalDashboard = model.IsPersonalDashboard });

            }
            catch (Exception exception)
            {
                model = listdata(model, model.EmployeeID);
                //model.IsPersonalDashboard = IsPersonalDashboard;
                model.TitleForm = "Car Complaint Form";
                model.ErrorMessage = exception.Message;
                model.CurrentLogin = CurrentUser;
                return View(model);
            }
        }

        public ActionResult ResponseUser(int? TraCcfId, bool IsPersonalDashboard)
        {
            var model = new CcfItem();
            model.MainMenu = _mainMenu;

            if (!TraCcfId.HasValue)
            {
                return HttpNotFound();
            }

            var ccfData = _ccfBLL.GetCcf().Where(x => x.TraCcfId == TraCcfId.Value).FirstOrDefault();
            var ccfDataD1 = _ccfBLL.GetCcfD1(TraCcfId.Value);

            if (ccfData == null)
            {
                return HttpNotFound();
            }

            try
            {
                model = Mapper.Map<CcfItem>(ccfData);
                model.Details_d1 = Mapper.Map<List<CcfItemDetil>>(ccfDataD1);
                var fleetData = _fleetBLL.GetFleet().Where(x => x.PoliceNumber == model.PoliceNumber).FirstOrDefault();
                model.IsPersonalDashboard = IsPersonalDashboard;
                model.EmployeeID = CurrentUser.EMPLOYEE_ID;
                if (fleetData != null)
                {
                    model.VStartPeriod = fleetData.StartContract.Value.ToString("dd-MMM-yyyy");
                    model.VEndPeriod = fleetData.EndContract.Value.ToString("dd-MMM-yyyy");
                }
                if (model.EmployeeIdComplaintFor != null)
                {
                    model = listdata(model, model.EmployeeIdComplaintFor);
                }
                else
                {
                    model = listdata(model, model.EmployeeID);
                }
                model = initCreate(model);
                model.CurrentLogin = CurrentUser;
                model.TitleForm = "Car Complaint Form";
                model.MainMenu = _mainMenu;
                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = listdata(model, model.EmployeeID);
                model.IsPersonalDashboard = IsPersonalDashboard;
                model.TitleForm = "Car Complaint Form";
                model.ErrorMessage = exception.Message;
                model.CurrentLogin = CurrentUser;
                return View(model);
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ResponseUser(CcfItem Model, HttpPostedFileBase ComplaintAtt)
        {
            try
            {
                string url = Model.DocumentNumber.Replace("/", "_");
                string path = "~/files_upload/CCF/" + Model.DocumentNumber.Replace("/", "_");

                if (!Directory.Exists(path))
                {
                    DirectoryInfo di = Directory.CreateDirectory(Server.MapPath(path));
                }

                if (ComplaintAtt != null)
                {
                    string filename = System.IO.Path.GetFileName(ComplaintAtt.FileName);
                    ComplaintAtt.SaveAs(Server.MapPath("~/files_upload/CCF/" + url + '/' + filename));
                    Model.DetailSave.ComplaintAtt = filename;
                    Model.DetailSave.ComplaintUrl = "/files_upload/CCF/" + url;
                }

                var Dto = Mapper.Map<TraCcfDto>(Model);
                var CcfData = _ccfBLL.Save(Dto, CurrentUser);
                if (Model.isSubmit == "submit")
                {
                    CcfWorkflow(CcfData.TraCcfId, Enums.ActionType.Submit, null, false);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                }
                return RedirectToAction("DetailsCcf", "TraCcf", new { TraCcfId = CcfData.TraCcfId, IsPersonalDashboard = Model.IsPersonalDashboard });
            }
            catch (Exception exception)
            {
                Model = listdata(Model, Model.EmployeeID);
                //model.IsPersonalDashboard = IsPersonalDashboard;
                Model.TitleForm = "Car Complaint Form";
                Model.ErrorMessage = exception.Message;
                Model.CurrentLogin = CurrentUser;
                return View(Model);
            }
        }

        public ActionResult ResponseCoordinator(int? TraCcfId, bool IsPersonalDashboard)
        {
            var model = new CcfItem();
            model.MainMenu = _mainMenu;

            if (!TraCcfId.HasValue)
            {
                return HttpNotFound();
            }

            var ccfData = _ccfBLL.GetCcf().Where(x => x.TraCcfId == TraCcfId.Value).FirstOrDefault();
            var ccfDataD1 = _ccfBLL.GetCcfD1(TraCcfId.Value);

            if (ccfData == null)
            {
                return HttpNotFound();
            }

            try
            {
                model = Mapper.Map<CcfItem>(ccfData);
                model.Details_d1 = Mapper.Map<List<CcfItemDetil>>(ccfDataD1);
                var fleetData = _fleetBLL.GetFleet().Where(x => x.PoliceNumber == model.PoliceNumber).FirstOrDefault();
                if (fleetData != null)
                {
                    model.VStartPeriod = fleetData.StartContract.Value.ToString("dd-MMM-yyyy");
                    model.VEndPeriod = fleetData.EndContract.Value.ToString("dd-MMM-yyyy");
                }
                if (model.EmployeeIdComplaintFor != null)
                {
                    model = listdata(model, model.EmployeeIdComplaintFor);
                }
                else
                {
                    model = listdata(model, model.EmployeeID);
                }
                model.IsPersonalDashboard = IsPersonalDashboard;
                model.CurrentLogin = CurrentUser;
                model = initCreate(model);
                model.TitleForm = "Car Complaint Form Respone Coordinator";
                model.MainMenu = _mainMenu;
                return View(model);
            }
            catch (Exception exception)
            {
                model.ErrorMessage = exception.Message;
                model.IsPersonalDashboard = IsPersonalDashboard;
                model.CurrentLogin = CurrentUser;
                model = initCreate(model);
                model.TitleForm = "Car Complaint Form Respone Coordinator";
                model.MainMenu = _mainMenu;
                return View(model);
            }
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ResponseCoordinator(CcfItem model, HttpPostedFileBase CoodinatorAtt, HttpPostedFileBase VendorAtt)
        {
            var a = ModelState.IsValid;
            try
            {
                var dataToSave = Mapper.Map<TraCcfDto>(model);
                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;
                dataToSave.DetailSave.TraCcfId = model.TraCcfId;

                string url = model.DocumentNumber.Replace("/", "_");
                string path = "~/files_upload/CCF/" + model.DocumentNumber.Replace("/", "_");
                dataToSave.DetailSave.CoordinatorUrl = "/files_upload/CCF/" + url;
                dataToSave.DetailSave.VendorUrl = "/files_upload/CCF/" + url;

                if (!Directory.Exists(path))
                {
                    DirectoryInfo di = Directory.CreateDirectory(Server.MapPath(path));
                }

                if (model.DetailSave.CoodinatorNote != null || model.DetailSave.VendorNote != null)
                {
                    dataToSave.DocumentStatus = Enums.DocumentStatus.InProgress;
                    model.DocumentStatus = Enums.DocumentStatus.InProgress;
                }

                if (model.isSubmit == "complete")
                {
                    dataToSave.DocumentStatus = Enums.DocumentStatus.Completed;
                    model.DocumentStatus = Enums.DocumentStatus.Completed;
                }

                if (CoodinatorAtt != null)
                {
                    string filename = System.IO.Path.GetFileName(CoodinatorAtt.FileName);
                    CoodinatorAtt.SaveAs(Server.MapPath("~/files_upload/CCF/"+ url +'/' + filename));
                    dataToSave.DetailSave.CoodinatorAtt = filename;
                }

                if (VendorAtt != null)
                {
                    string filename = System.IO.Path.GetFileName(VendorAtt.FileName);
                    VendorAtt.SaveAs(Server.MapPath("~/files_upload/CCF/" + url + '/' + filename));
                    dataToSave.DetailSave.VendorAtt = filename;
                }
                var saveResult = _ccfBLL.Save(dataToSave, CurrentUser);

                if (model.DetailSave.CoodinatorNote != null || model.DetailSave.VendorNote != null)
                {
                    CcfWorkflow(model.TraCcfId, Enums.ActionType.Submit, null, false);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("ResponseCoordinator", "TraCcf", new { @TraCcfId = model.TraCcfId, @IsPersonalDashboard = model.IsPersonalDashboard });
                }
                if (model.isSubmit == "complete")
                {
                    CcfWorkflow(model.TraCcfId, Enums.ActionType.Completed, null, false);
                    AddMessageInfo("Success Completed Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("DetailsCcf", "TraCcf", new { @TraCcfId = model.TraCcfId, @IsPersonalDashboard = model.IsPersonalDashboard });
                }
                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                return RedirectToAction("ResponseCoordinator", "TraCcf", new { @TraCcfId = model.TraCcfId, @IsPersonalDashboard = model.IsPersonalDashboard });

            }
            catch (Exception exception)
            {
                model.ErrorMessage = exception.Message;
                model.CurrentLogin = CurrentUser;
                model = initCreate(model);
                model.TitleForm = "Car Complaint Form Respone Coordinator";
                model.MainMenu = _mainMenu;
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
                EmployeeId = CurrentUser.EMPLOYEE_ID,
                UserRole = CurrentUser.UserRole,
                ActionType = actionType,
                Comment = comment,
                EndRent = Endrent
            };

            _ccfBLL.CcfWorkflow(input);
        }

        #endregion

        #region --------- Export Excel--------------
        public void ExportCCF(bool IsPersonalDashboard, bool IsCompleted)
        {
            string pathFile = "";

            pathFile = CreateXlsTraCCF(IsPersonalDashboard, IsCompleted);

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

        private string CreateXlsTraCCF(bool IsPersonalDashboard, bool IsCompleted)
        {
            List<TraCcfDto> ccf = _ccfBLL.GetCcf();
            var listData = Mapper.Map<List<CcfItem>>(ccf);

            if (CurrentUser.UserRole == Enums.UserRole.HR && IsPersonalDashboard == false)
            {
                if (IsCompleted == true)
                {
                    listData = Mapper.Map<List<CcfItem>>(ccf.Where(x => (x.ComplaintCategoryRole == "HR" && x.DocumentStatus == Enums.DocumentStatus.Completed) ||
                   (x.DocumentStatus == Enums.DocumentStatus.Completed && x.CreatedBy == CurrentUser.USER_ID)
                   ));
                }
                else
                {
                    listData = Mapper.Map<List<CcfItem>>(ccf.Where(x => (
                    (x.DocumentStatus == Enums.DocumentStatus.AssignedForHR ||
                    x.DocumentStatus == Enums.DocumentStatus.InProgress ||
                    x.CreatedBy == CurrentUser.USER_ID)
                    )));
                }
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Fleet && IsPersonalDashboard == false)
            {
                if (IsCompleted == true)
                {
                    listData = Mapper.Map<List<CcfItem>>(ccf.Where(x => ((x.ComplaintCategoryRole == "Fleet" && x.DocumentStatus == Enums.DocumentStatus.Completed) ||
                   (x.DocumentStatus == Enums.DocumentStatus.Completed &&x.CreatedBy == CurrentUser.USER_ID)
                   )));
                }
                else
                {
                    listData = Mapper.Map<List<CcfItem>>(ccf.Where(x => (
                    (x.DocumentStatus == Enums.DocumentStatus.AssignedForFleet ||
                    x.DocumentStatus == Enums.DocumentStatus.InProgress ||
                    x.CreatedBy == CurrentUser.USER_ID)
                    )));
                }
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Viewer && IsPersonalDashboard == false)
            {
                if (IsCompleted == true)
                {
                    listData = Mapper.Map<List<CcfItem>>(ccf.Where(x => (
                    x.DocumentStatus == Enums.DocumentStatus.Completed ||
                    (x.DocumentStatus == Enums.DocumentStatus.Completed && x.CreatedBy == CurrentUser.USER_ID))
                    ));
                }
                else
                {
                    listData = Mapper.Map<List<CcfItem>>(ccf.Where(x => (
                    x.DocumentStatus != Enums.DocumentStatus.Completed &&
                    x.CreatedBy == CurrentUser.USER_ID)
                    ));
                }
            }
            else if (CurrentUser.UserRole == Enums.UserRole.User && IsPersonalDashboard == false)
            {
                if (IsCompleted == true)
                {
                    listData = Mapper.Map<List<CcfItem>>(ccf.Where(x => (
                    x.DocumentStatus == Enums.DocumentStatus.Completed ||
                    (x.DocumentStatus == Enums.DocumentStatus.Completed  && x.CreatedBy == CurrentUser.USER_ID))
                    ));
                }
                else
                {
                    listData = Mapper.Map<List<CcfItem>>(ccf.Where(x => (
                    x.DocumentStatus != Enums.DocumentStatus.Completed &&
                    x.CreatedBy == CurrentUser.USER_ID)
                    ));
                }
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Viewer && IsPersonalDashboard == true)
            {
                listData = Mapper.Map<List<CcfItem>>(ccf.Where(x => (
                x.CreatedBy == CurrentUser.USER_ID)
                ));
            }
            else if (CurrentUser.UserRole == Enums.UserRole.User && IsPersonalDashboard == true)
            {
                listData = Mapper.Map<List<CcfItem>>(ccf.Where(x => (
                x.CreatedBy == CurrentUser.USER_ID)
                ));
            }
            else if(IsPersonalDashboard == true)
            {
                if (IsCompleted == true)
                {
                    listData = Mapper.Map<List<CcfItem>>(ccf.Where(x => (
                    x.DocumentStatus == Enums.DocumentStatus.Completed)
                    ));
                }
                else
                {
                    if (CurrentUser.UserRole == Enums.UserRole.HR)
                    {
                        listData = Mapper.Map<List<CcfItem>>(ccf.Where(x => (
                        x.DocumentStatus == Enums.DocumentStatus.AssignedForHR ||
                        x.DocumentStatus == Enums.DocumentStatus.InProgress ||
                        x.CreatedBy == CurrentUser.USER_ID)
                        ));
                    }
                    else if (CurrentUser.UserRole == Enums.UserRole.Fleet)
                    {
                        listData = Mapper.Map<List<CcfItem>>(ccf.Where(x => (
                        x.DocumentStatus == Enums.DocumentStatus.AssignedForFleet ||
                        x.DocumentStatus == Enums.DocumentStatus.InProgress ||
                        x.CreatedBy == CurrentUser.USER_ID)
                        ));
                    }
                }
                //listData = Mapper.Map<List<CcfItem>>(ccf.Where(x => x.CreatedBy == CurrentUser.USER_ID));
            }
            
            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Data Car Complaint Form (CCF)");
            slDocument.MergeWorksheetCells(1, 1, 1, 25);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelTraCCF(slDocument);

            //create data
            slDocument = CreateDataExcelTraCCF(slDocument, listData);

            var fileName = "CCF_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;
        }

        private SLDocument CreateHeaderExcelTraCCF(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Document Number");
            slDocument.SetCellValue(iRow, 2, "Complaint Category");
            slDocument.SetCellValue(iRow, 3, "Employee ID");
            slDocument.SetCellValue(iRow, 4, "Employee Name");
            slDocument.SetCellValue(iRow, 5, "Employee ID Complaint For");
            slDocument.SetCellValue(iRow, 6, "Employee Name Complaint For");
            slDocument.SetCellValue(iRow, 7, "Police Number");
            slDocument.SetCellValue(iRow, 8, "Police Number GS");
            slDocument.SetCellValue(iRow, 9, "Location City");
            slDocument.SetCellValue(iRow, 10, "Location Address");
            slDocument.SetCellValue(iRow, 11, "Vehicle Type");
            slDocument.SetCellValue(iRow, 12, "Vehicle Usage");
            slDocument.SetCellValue(iRow, 13, "Manufacture");
            slDocument.SetCellValue(iRow, 14, "Model");
            slDocument.SetCellValue(iRow, 15, "Series");
            slDocument.SetCellValue(iRow, 16, "Vendor");
            slDocument.SetCellValue(iRow, 17, "Start Contract");
            slDocument.SetCellValue(iRow, 18, "End Contract");
            slDocument.SetCellValue(iRow, 19, "Coordinator KPI");
            slDocument.SetCellValue(iRow, 20, "Vendor KPI");
            slDocument.SetCellValue(iRow, 21, "Created By");
            slDocument.SetCellValue(iRow, 22, "Created Date");
            slDocument.SetCellValue(iRow, 23, "Modified By");
            slDocument.SetCellValue(iRow, 24, "Modified Date");
            slDocument.SetCellValue(iRow, 25, "Status");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 25, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelTraCCF(SLDocument slDocument, List<CcfItem> listData)
        {
            int iRow = 3; //starting row data
            if (listData != null)
            {
                foreach (var data in listData)
                {
                    slDocument.SetCellValue(iRow, 1, data.DocumentNumber);
                    slDocument.SetCellValue(iRow, 2, data.ComplaintCategoryName);
                    slDocument.SetCellValue(iRow, 3, data.EmployeeID);
                    slDocument.SetCellValue(iRow, 4, data.EmployeeName);
                    slDocument.SetCellValue(iRow, 5, data.EmployeeIdComplaintFor);
                    slDocument.SetCellValue(iRow, 6, data.EmployeeNameComplaintFor);
                    slDocument.SetCellValue(iRow, 7, data.PoliceNumber);
                    slDocument.SetCellValue(iRow, 8, data.PoliceNumberGS);
                    slDocument.SetCellValue(iRow, 9, data.LocationCity);
                    slDocument.SetCellValue(iRow, 10, data.LocationAddress);
                    slDocument.SetCellValue(iRow, 11, data.VehicleType);
                    slDocument.SetCellValue(iRow, 12, data.VehicleUsage);
                    slDocument.SetCellValue(iRow, 13, data.Manufacturer);
                    slDocument.SetCellValue(iRow, 14, data.Models);
                    slDocument.SetCellValue(iRow, 15, data.Series);
                    slDocument.SetCellValue(iRow, 16, data.Vendor);
                    slDocument.SetCellValue(iRow, 17, data.StartPeriod.ToString("dd-MM-yyyy"));
                    slDocument.SetCellValue(iRow, 18, data.EndPeriod.ToString("dd-MMM-yyyy"));
                    slDocument.SetCellValue(iRow, 19, data.CoordinatorKPI);
                    slDocument.SetCellValue(iRow, 20, data.VendorKPI);
                    slDocument.SetCellValue(iRow, 21, data.CreatedBy);
                    slDocument.SetCellValue(iRow, 22, data.CreatedDate.ToString("dd-MM-yyyy"));
                    slDocument.SetCellValue(iRow, 23, data.ModifiedBy);
                    slDocument.SetCellValue(iRow, 24, data.ModifiedDate.Value.ToString("dd-MM-yyyy"));
                    slDocument.SetCellValue(iRow, 25, data.DocumentStatus.ToString());
                    iRow++;
                }


            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 24);
            slDocument.SetCellStyle(3, 1, iRow - 1, 25, valueStyle);

            return slDocument;
        }
        #endregion
    }
}
