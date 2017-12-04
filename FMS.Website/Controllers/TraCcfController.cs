﻿using System;
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
                    model.Details = Mapper.Map<List<CcfItem>>(data.Where(x => (
                    (x.DocumentStatus == Enums.DocumentStatus.AssignedForHR ||
                    x.DocumentStatus == Enums.DocumentStatus.InProgress)
                    )));
                }
                else if (CurrentUser.UserRole == Enums.UserRole.Fleet)
                {
                    model.Details = Mapper.Map<List<CcfItem>>(data.Where(x => (
                    (x.DocumentStatus == Enums.DocumentStatus.AssignedForFleet ||
                    x.DocumentStatus == Enums.DocumentStatus.InProgress)
                    )));
                }
                else if (CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Administrator)
                {
                    model.Details = Mapper.Map<List<CcfItem>>(data.Where(x => (
                    x.DocumentStatus == Enums.DocumentStatus.InProgress)
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
                    model.Details = Mapper.Map<List<CcfItem>>(data.Where(x => x.DocumentStatus == Enums.DocumentStatus.Completed && x.ComplaintCategoryRole == "HR"));
                }
                else if (CurrentUser.UserRole == Enums.UserRole.Fleet)
                {
                    model.Details = Mapper.Map<List<CcfItem>>(data.Where(x => x.DocumentStatus == Enums.DocumentStatus.Completed && x.ComplaintCategoryRole == "Fleet"));
                }
                else if (CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Administrator)
                {
                    model.Details = Mapper.Map<List<CcfItem>>(data.Where(x => x.DocumentStatus == Enums.DocumentStatus.Completed));
                }
                else
                {
                    model.Details = Mapper.Map<List<CcfItem>>(data.Where(x => x.DocumentStatus == Enums.DocumentStatus.Completed && x.EmployeeID == CurrentUser.EMPLOYEE_ID));
                }
                return View("Index", model);
            }
        }
        #endregion

        #region --------- Personal Dashboard --------------

        public ActionResult PersonalDashboard()
        {
            var data = _ccfBLL.GetCcf().Where(x=>x.CreatedBy == CurrentUser.USER_ID);
            var model = new CcfModel();
            model.TitleForm = "CCF Personal Dashboard";
            model.Details = Mapper.Map<List<CcfItem>>(data);
            model.MainMenu = Enums.MenuList.PersonalDashboard;
            model.CurrentLogin = CurrentUser;
            model.IsPersonalDashboard = true;
            //model.MainMenu = _mainMenu;
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
                model.VStartPeriod = fleetData.StartContract.Value.ToString("dd-MMM-yyyy");
                model.VEndPeriod = fleetData.EndContract.Value.ToString("dd-MMM-yyyy");
                model.IsPersonalDashboard = IsPersonalDashboard;
                model = listdata(model, model.EmployeeID);
                model.CurrentLogin = CurrentUser;
                model = initCreate(model);
                model.TitleForm = "Car Complaint Form Details";
                model.MainMenu = _mainMenu;
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
            //var listemployeefromdelegation = _delegationBLL.GetDelegation().Select(x => new { dataemployeefrom = x.EmployeeFrom + x.NameEmployeeFrom, x.EmployeeFrom, x.NameEmployeeFrom, x.EmployeeTo, x.NameEmployeeTo, x.DateTo }).ToList().Where(x => x.EmployeeTo == CurrentUser.EMPLOYEE_ID && x.DateTo >= DateTime.Today).OrderBy(x => x.EmployeeFrom);
            var listemployeefromdelegation = CurrentUser.LoginFor;
            model.EmployeeFromDelegationList = new SelectList(listemployeefromdelegation, "EMPLOYEE_ID", "EMPLOYEE_NAME");

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
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateCcf(CcfItem Model, HttpPostedFileBase ComplaintAtt)
        {
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

                if (ComplaintAtt != null)
                {
                    string filename = System.IO.Path.GetFileName(ComplaintAtt.FileName);
                    ComplaintAtt.SaveAs(Server.MapPath("~/files_upload/" + filename));
                    string filepathtosave = "files_upload" + filename;
                    Model.ComplaintAtt = filename;
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
                    ComplaintAtt.SaveAs(Server.MapPath("~/files_upload/" + filename));
                    string filepathtosave = "files_upload" + filename;
                    dataToSave.ComplaintAtt = filename;
                }

                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;
                var saveResult = _ccfBLL.Save(dataToSave, CurrentUser);
                
                if (isSubmit)
                {
                    CcfWorkflow(model.TraCcfId, Enums.ActionType.Submit, null, false);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("DetailsCcf", "TraCcf", new { @TraCcfId = model.TraCcfId, @IsPersonalDashboard = model.IsPersonalDashboard });
                }
                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
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
                model.IsPersonalDashboard = IsPersonalDashboard;
                model.EmployeeID = CurrentUser.EMPLOYEE_ID;
                model = listdata(model, model.EmployeeID);
                model.CurrentLogin = CurrentUser;
                model.TitleForm = "Car Complaint Form";
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
                //return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ResponseUser(CcfItem model, HttpPostedFileBase ComplaintAtt)
        {
            try
            {
                var dataToSave = Mapper.Map<TraCcfDto>(model);

                bool isSubmit = model.isSubmit == "submit";
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
                    ComplaintAtt.SaveAs(Server.MapPath("~/files_upload/" + filename));
                    string filepathtosave = "files_upload" + filename;
                    dataToSave.ComplaintAtt = filename;
                }

                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;
                var saveResult = _ccfBLL.Save(dataToSave, CurrentUser);

                if (isSubmit)
                {
                    CcfWorkflow(model.TraCcfId, Enums.ActionType.Submit, null, false);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("DetailsCcf", "TraCcf", new { @TraCcfId = model.TraCcfId, @IsPersonalDashboard = model.IsPersonalDashboard });
                }
                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                return RedirectToAction("ResponseUser", "TraCcf", new { @TraCcfId = model.TraCcfId, @IsPersonalDashboard = model.IsPersonalDashboard });

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
                model.VStartPeriod = fleetData.StartContract.Value.ToString("dd-MMM-yyyy");
                model.VEndPeriod = fleetData.EndContract.Value.ToString("dd-MMM-yyyy");
                model.IsPersonalDashboard = IsPersonalDashboard;
                model = listdata(model, model.EmployeeID);
                model.CurrentLogin = CurrentUser;
                model = initCreate(model);
                model.TitleForm = "Car Complaint Form Respone Coordinator";
                model.MainMenu = _mainMenu;
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
        public ActionResult ResponseCoordinator(CcfItem model, HttpPostedFileBase CoodinatorAtt, HttpPostedFileBase VendorAtt)
        {
            var a = ModelState.IsValid;
            try
            {
                var dataToSave = Mapper.Map<TraCcfDto>(model);
                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;
                if (model.isSubmit == "submit")
                {
                    dataToSave.DocumentStatus = Enums.DocumentStatus.InProgress;
                }
                else if (model.isSubmit == "complete")
                {
                    dataToSave.DocumentStatus = Enums.DocumentStatus.Completed;
                }

                if (CoodinatorAtt != null)
                {
                    string filename = System.IO.Path.GetFileName(CoodinatorAtt.FileName);
                    CoodinatorAtt.SaveAs(Server.MapPath("~/files_upload/" + filename));
                    string filepathtosave = "files_upload" + filename;
                    dataToSave.CoodinatorAtt = filename;
                }

                if (VendorAtt != null)
                {
                    string filename = System.IO.Path.GetFileName(VendorAtt.FileName);
                    VendorAtt.SaveAs(Server.MapPath("~/files_upload/" + filename));
                    string filepathtosave = "files_upload" + filename;
                    dataToSave.VendorAtt = filename;
                }
                var saveResult = _ccfBLL.Save(dataToSave, CurrentUser);

                if (model.isSubmit == "submit")
                {
                    CcfWorkflow(model.TraCcfId, Enums.ActionType.Submit, null, false);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("DetailsCcf", "TraCcf", new { @TraCcfId = model.TraCcfId, @IsPersonalDashboard = model.IsPersonalDashboard });
                }
                else if (model.isSubmit == "complete")
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
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                //model = initCreate(model, "benefit");
                model.CurrentLogin = CurrentUser;
                return View(model);
            }
        }

        public ActionResult ResponseVendor(int? TraCcfId, bool IsPersonalDashboard)
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
                model.IsPersonalDashboard = IsPersonalDashboard;
                model = listdata(model, model.EmployeeID);
                model.CurrentLogin = CurrentUser;
                model = initCreate(model);
                model.TitleForm = "Car Complaint Form Respone Coordinator";
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
        public ActionResult ResponseVendor(CcfItem model, HttpPostedFileBase VendorAtt)
        {
            var a = ModelState.IsValid;
            try
            {
                var dataToSave = Mapper.Map<TraCcfDto>(model);
                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;
                if (model.isSubmit == "submit")
                {
                    dataToSave.DocumentStatus = Enums.DocumentStatus.AssignedForUser;
                }
                else if (model.isSubmit == "complete")
                {
                    dataToSave.DocumentStatus = Enums.DocumentStatus.Completed;
                }

                if (VendorAtt != null)
                {
                    string filename = System.IO.Path.GetFileName(VendorAtt.FileName);
                    VendorAtt.SaveAs(Server.MapPath("~/files_upload/" + filename));
                    string filepathtosave = "files_upload" + filename;
                    dataToSave.VendorAtt = filename;
                }
                var saveResult = _ccfBLL.Save(dataToSave, CurrentUser);

                if (model.isSubmit == "submit")
                {
                    //Bermasalah merubah status tiba2
                    //CcfWorkflow(model.TraCcfId, Enums.ActionType.Submit, null, false);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("DetailsCcf", "TraCcf", new { @TraCcfId = model.TraCcfId, @IsPersonalDashboard = model.IsPersonalDashboard });
                }
                else if (model.isSubmit == "complete")
                {
                    CcfWorkflow(model.TraCcfId, Enums.ActionType.Completed, null, false);
                    AddMessageInfo("Success Completed Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("DetailsCcf", "TraCcf", new { @TraCcfId = model.TraCcfId, @IsPersonalDashboard = model.IsPersonalDashboard });
                }
                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                return RedirectToAction("Completed", "TraCcf");

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
                EmployeeId = CurrentUser.EMPLOYEE_ID,
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
