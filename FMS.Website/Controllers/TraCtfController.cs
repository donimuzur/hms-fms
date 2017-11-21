﻿using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Core.Exceptions;
using FMS.Utils;
using FMS.Website.Models;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Controllers
{
    public class TraCtfController : BaseController
    {
        #region --------- Field & Constructor--------------
        private IEpafBLL _epafBLL;
        private ITraCtfBLL _ctfBLL;
        private ITraCsfBLL _csfBLL;
        private IRemarkBLL _remarkBLL;
        private IFleetBLL _fleetBLL;
        private IEmployeeBLL _employeeBLL;
        private IReasonBLL _reasonBLL;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private ISettingBLL _settingBLL;
        private ILocationMappingBLL _locationMappingBLL;
        private ICtfExtendBLL _ctfExtendBLL;
        public TraCtfController(IPageBLL pageBll, IEpafBLL epafBll, ITraCtfBLL ctfBll, ITraCsfBLL CsfBll, IRemarkBLL RemarkBLL,ISettingBLL SettingBLL, ICtfExtendBLL CtfExtendBLL,
                                IEmployeeBLL  EmployeeBLL, IReasonBLL ReasonBLL, IFleetBLL FleetBLL, ILocationMappingBLL LocationMappingBLL): base(pageBll, Core.Enums.MenuList.TraCtf)
        {
          
            _epafBLL = epafBll;
            _ctfBLL = ctfBll;
            _csfBLL = CsfBll;
            _employeeBLL = EmployeeBLL;
            _pageBLL = pageBll;
            _remarkBLL = RemarkBLL;
            _reasonBLL = ReasonBLL;
            _fleetBLL = FleetBLL;
            _settingBLL = SettingBLL;
            _locationMappingBLL = LocationMappingBLL;
            _ctfExtendBLL = CtfExtendBLL;
            _mainMenu = Enums.MenuList.Transaction;
        }
        #endregion

        #region --------- Open Document--------------
        public ActionResult Index()
        {
            if (CurrentUser.UserRole == Enums.UserRole.User)
            {
                return RedirectToAction("PersonalDashboard");
            }

            var model = new CtfModel();
            var data = _ctfBLL.GetCtfDashboard(CurrentUser, false);
            model.Details = Mapper.Map<List<CtfItem>>(data);
            foreach (var item in model.Details)
            {
                var ctfExtendDto = _ctfExtendBLL.GetCtfExtend().Where(x => x.TraCtfId == item.TraCtfId).FirstOrDefault();
                if (ctfExtendDto != null)
                {
                    item.CtfExtend = ctfExtendDto;
                }
            }
            model.TitleForm = "CTF Open Document";
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.IsPersonalDashboard = false;
            
            return View(model);
        }

        public CtfModel TerminationBoard( CtfModel model)
        {
            var settingData = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
            var benefitType = settingData.Where(x => x.SettingName.ToUpper() == "BENEFIT").FirstOrDefault().SettingName;
            var wtcType = settingData.Where(x => x.SettingName.ToUpper() == "WTC").FirstOrDefault().SettingName;

            var fleetBenefit = _fleetBLL.GetFleetForEndContractLessThan(60).Where(x => x.VehicleType == benefitType && x.IsActive == true).ToList();
            var fleetWTC = _fleetBLL.GetFleetForEndContractLessThan(90).Where(x => x.VehicleType == wtcType && x.IsActive == true).ToList();

            if (CurrentUser.UserRole == Enums.UserRole.HR)
            {
                if (fleetBenefit != null)
                {
                    foreach (var item in fleetBenefit)
                    {
                        try
                        {
                            var ctfitem = Mapper.Map<CtfItem>(item);
                            var ReasonID = _reasonBLL.GetReason().Where(x => x.Reason.ToLower() == "end rent").FirstOrDefault().MstReasonId;

                            var ctfdata = _ctfBLL.GetCtf().Where(x => x.EmployeeId == ctfitem.EmployeeId && x.EmployeeName == ctfitem.EmployeeName && x.CostCenter == ctfitem.CostCenter
                                            && x.GroupLevel == ctfitem.GroupLevel && x.SupplyMethod == ctfitem.SupplyMethod && x.PoliceNumber == ctfitem.PoliceNumber && x.VehicleYear == ctfitem.VehicleYear 
                                            && x.VehicleType == ctfitem.VehicleType && x.VehicleUsage == ctfitem.VehicleUsage && x.EndRendDate == ctfitem.EndRendDate && x.DocumentStatus != Enums.DocumentStatus.Completed).ToList();
                            var csfdata = _csfBLL.GetList().Where( x => x.EMPLOYEE_ID == ctfitem.EmployeeId && x.EMPLOYEE_NAME == ctfitem.EmployeeName && x.COST_CENTER == ctfitem.CostCenter
                                            && x.GROUP_LEVEL == ctfitem.GroupLevel).ToList();

                            if (ctfdata.Count() > 0 || csfdata.Count() > 0) ctfitem.isSend = true;

                            var days7 = DateTime.Now.AddDays(7);
                            ctfitem.Reason = ReasonID;
                            ctfitem.ReasonS = "End Rent";
                            ctfitem.lessthan2month = true;
                            ctfitem.CreatedBy = "SYSTEM";
                            ctfitem.lessthan7day = ctfitem.EndRendDate <= days7 ? true : false;
                            model.Details.Add(ctfitem);
                        }
                        catch (Exception exp)
                        {
                            AddMessageInfo(exp.Message, Enums.MessageInfoType.Error);
                        }
                    }
                }
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Fleet)
            {
                fleetBenefit = _fleetBLL.GetFleetForEndContractLessThan(7).Where(x => x.VehicleType == benefitType && x.IsActive == true).ToList();
                if (fleetBenefit != null)
                {
                    foreach (var item in fleetBenefit)
                    {
                        try
                        {
                            var ctfitem = Mapper.Map<CtfItem>(item);
                            var ReasonID = _reasonBLL.GetReason().Where(x => x.Reason.ToLower() == "end rent").FirstOrDefault().MstReasonId;
                            var ctfdata = _ctfBLL.GetCtf().Where(x => x.EmployeeId == ctfitem.EmployeeId && x.EmployeeName == ctfitem.EmployeeName && x.CostCenter == ctfitem.CostCenter
                                         && x.GroupLevel == ctfitem.GroupLevel && x.SupplyMethod == ctfitem.SupplyMethod && x.PoliceNumber == ctfitem.PoliceNumber && x.VehicleYear == ctfitem.VehicleYear
                                         && x.VehicleType == ctfitem.VehicleType && x.VehicleUsage == ctfitem.VehicleUsage && x.EndRendDate == ctfitem.EndRendDate && x.DocumentStatus != Enums.DocumentStatus.Completed).ToList();
                            var csfdata = _csfBLL.GetList().Where(x => x.EMPLOYEE_ID == ctfitem.EmployeeId && x.EMPLOYEE_NAME == ctfitem.EmployeeName && x.COST_CENTER == ctfitem.CostCenter
                                           && x.GROUP_LEVEL == ctfitem.GroupLevel).ToList();
                            
                            var days7 = DateTime.Now.AddDays(7);
                            if (ctfdata.Count() > 0 || csfdata.Count() > 0) ctfitem.isSend = true;
                            ctfitem.Reason = ReasonID;
                            ctfitem.ReasonS = "End Rent";
                            ctfitem.lessthan2month = false;
                            ctfitem.lessthan7day = true;
                            ctfitem.CreatedBy = "SYSTEM";

                            model.Details.Add(ctfitem);
                        }
                        catch (Exception exp)
                        {
                            AddMessageInfo(exp.Message, Enums.MessageInfoType.Error);
                        }
                    }
                }
                if (fleetWTC != null)
                {
                    foreach (var item in fleetWTC)
                    {
                        try
                        {
                            var ctfitem = Mapper.Map<CtfItem>(item);
                            var ReasonID = _reasonBLL.GetReason().Where(x => x.Reason.ToLower() == "end rent").FirstOrDefault().MstReasonId;
                            var days7 = DateTime.Now.AddDays(7);
                            var ctfdata = _ctfBLL.GetCtf().Where(x => x.EmployeeId == ctfitem.EmployeeId && x.EmployeeName == ctfitem.EmployeeName && x.CostCenter == ctfitem.CostCenter
                                        && x.GroupLevel == ctfitem.GroupLevel && x.SupplyMethod == ctfitem.SupplyMethod && x.PoliceNumber == ctfitem.PoliceNumber && x.VehicleYear == ctfitem.VehicleYear
                                        && x.VehicleType == ctfitem.VehicleType && x.VehicleUsage == ctfitem.VehicleUsage && x.EndRendDate == ctfitem.EndRendDate && x.DocumentStatus != Enums.DocumentStatus.Completed).ToList();
                            var csfdata = _csfBLL.GetList().Where(x => x.EMPLOYEE_ID == ctfitem.EmployeeId && x.EMPLOYEE_NAME == ctfitem.EmployeeName && x.COST_CENTER == ctfitem.CostCenter
                                           && x.GROUP_LEVEL == ctfitem.GroupLevel).ToList();
                            if (ctfdata.Count() > 0 || csfdata.Count() > 0) ctfitem.isSend = true;
                            ctfitem.Reason = ReasonID;
                            ctfitem.ReasonS = "End Rent";
                            ctfitem.lessthan2month = true;
                            ctfitem.CreatedBy = "SYSTEM";
                            ctfitem.lessthan7day = ctfitem.EndRendDate <= days7 ? true : false;
                            model.Details.Add(ctfitem);
                        }
                        catch (Exception exp)
                        {
                            AddMessageInfo(exp.Message, Enums.MessageInfoType.Error);
                        }
                    }
                }

            }
            return model;
        }
        #endregion
        
        #region --------- Personal Dashboard --------------
        public ActionResult PersonalDashboard()
        {
            var data = _ctfBLL.GetCtfPersonal(CurrentUser);
            var model = new CtfModel();
            model.TitleForm = "CTF Personal Dashboard";
            model.Details = Mapper.Map<List<CtfItem>>(data);
            model.MainMenu = Enums.MenuList.PersonalDashboard;
            model.CurrentLogin = CurrentUser;
            model.IsPersonalDashboard = true;
            return View(model);
        }
        #endregion

        #region --------- Create --------------
        public CtfItem initCreate(CtfItem model)
        {
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE == true).Select(x => new { x.EMPLOYEE_ID, employee = x.EMPLOYEE_ID + " - " + x.FORMAL_NAME }).Distinct().ToList().OrderBy(x => x.EMPLOYEE_ID);
            var ReasonList = _reasonBLL.GetReason().Where(x => x.IsActive == true && x.DocumentType == 6).ToList().OrderBy(x=>x.Reason);
            var VehicleLocationList = _locationMappingBLL.GetLocationMapping().Select(x => new { City = x.Location }).Distinct().OrderBy(x =>x.City);
            var UserDecisionList = new Dictionary<int, string>{ { 1, "Buy" }, { 2, "Refund" }};
            var PoliceNumberList = new List<FleetDto>();
            if (model.EmployeeId != null)
            {
                PoliceNumberList = _fleetBLL.GetFleet().Where(x => x.IsActive == true && x.EmployeeID == model.EmployeeId).OrderBy(x=>x.PoliceNumber).ToList();
            }
            else
            {
                PoliceNumberList = _fleetBLL.GetFleet().Where(x => x.IsActive == true).OrderBy(x=>x.PoliceNumber).ToList();
            }
            
            var ExtendList = new Dictionary<bool, string>{ { false, "No" }, { true, "Yes" }};
            var RemarkList = _remarkBLL.GetRemark().Where(x => x.RoleType == CurrentUser.UserRole.ToString() && x.DocumentType == (int)Enums.DocumentType.CTF).ToList();

            model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");
            model.ExtendList = new SelectList(ExtendList, "Key", "Value");
            model.PoliceNumberList = new SelectList(PoliceNumberList, "PoliceNumber", "PoliceNumber");
            model.UserDecisionList = new SelectList(UserDecisionList, "Key", "Value");
            model.ReasonList = new SelectList(ReasonList, "MstReasonId", "Reason");
            model.EmployeeIdList = new SelectList(EmployeeList, "EMPLOYEE_ID", "employee");
            model.VehicleLocationList = new SelectList(VehicleLocationList, "City", "City");

            model.CurrentLogin = CurrentUser;
            model.MainMenu = model.IsPersonalDashboard ? Enums.MenuList.PersonalDashboard : _mainMenu;

            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.TraCtf, model.TraCtfId);
            model.WorkflowLogs = GetWorkflowHistory((int)Enums.MenuList.TraCtf, model.TraCtfId);
            return model;
        }
        public ActionResult Create()
        {
            if (CurrentUser.UserRole != Enums.UserRole.HR && CurrentUser.UserRole != Enums.UserRole.Fleet)
            {
                return RedirectToAction("Index");
            }

            var model = new CtfItem();
            model = initCreate(model);
            model.CreatedBy = CurrentUser.USER_ID;
            model.CreatedDate = DateTime.Now;
            model.DocumentStatus = Enums.DocumentStatus.Draft;
            model.CurrentLogin = CurrentUser;
            model.TitleForm = "Car Termination Form";
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CtfItem Model)
        {
            var a = ModelState;
            try
            {
                Model.CreatedBy = CurrentUser.USER_ID;
                Model.EmployeeIdCreator = CurrentUser.EMPLOYEE_ID;
                Model.CreatedDate = DateTime.Now;
                Model.DocumentStatus = Enums.DocumentStatus.Draft;
                if (Model.BuyCostTotalStr != null)
                {
                    Model.BuyCostTotal = Convert.ToDecimal(Model.BuyCostTotalStr.Replace(",", ""));
                }
                
                Model.IsActive = true;
                var Dto = Mapper.Map<TraCtfDto>(Model);
                var CtfData = _ctfBLL.Save(Dto, CurrentUser);

                var settingData = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
                var benefitType = settingData.Where(x => x.SettingName.ToUpper() == "BENEFIT").FirstOrDefault().SettingName;
                var wtcType = settingData.Where(x => x.SettingName.ToUpper() == "WTC").FirstOrDefault().SettingName;

                var reasonStr = _reasonBLL.GetReasonById(Model.Reason.Value).Reason;

                var IsBenefit = Model.VehicleType == benefitType;
                var IsEndRent = reasonStr.ToLower() == "end rent";

                if (Model.isSubmit == "submit")
                {
                    if (!IsBenefit && IsEndRent)
                    {
                        CtfWorkflow(CtfData.TraCtfId, Enums.ActionType.Submit, null, true, IsBenefit, Model.DocumentNumber);
                        AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                        return RedirectToAction("Edit", "TraCtf", new { TraCtfId = CtfData.TraCtfId, IsPersonalDashboard = false });
                    }

                    CtfWorkflow(CtfData.TraCtfId, Enums.ActionType.Submit, null,false, IsBenefit,Model.DocumentNumber );
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("Edit", "TraCtf", new { TraCtfId = CtfData.TraCtfId, IsPersonalDashboard = false });
                }
                AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
                CtfWorkflow(CtfData.TraCtfId, Enums.ActionType.Created, null, false,IsBenefit,Model.DocumentNumber);

                return RedirectToAction("Edit", "TraCtf", new { TraCtfId = CtfData.TraCtfId, IsPersonalDashboard = false });
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                Model = initCreate(Model);
                Model.TitleForm = "Car Termination Form";
                Model.ErrorMessage = exception.Message;
                Model.CurrentLogin = CurrentUser;
                return View(Model);
            }
        }
        #endregion

        #region ---------  Details --------------
        public ActionResult Details(int? TraCtfId, bool IsPersonalDashboard)
        {
            if (!TraCtfId.HasValue)
            {
                return HttpNotFound();
            }

            var ctfData = _ctfBLL.GetCtf().Where(x => x.TraCtfId == TraCtfId.Value).FirstOrDefault();

            if (ctfData == null)
            {
                return HttpNotFound();
            }
            var settingData = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
            var benefitType = settingData.Where(x => x.SettingName.ToUpper() == "BENEFIT").FirstOrDefault().SettingName;
            var wtcType = settingData.Where(x => x.SettingName.ToUpper() == "WTC").FirstOrDefault().SettingName;

            if (ctfData.VehicleType == wtcType)
            {
                return RedirectToAction("DetailsWTC", "TraCtf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard = IsPersonalDashboard });
            }
            else
            {
                return RedirectToAction("DetailsBenefit", "TraCtf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard = IsPersonalDashboard });
            }
        }
        public ActionResult DetailsBenefit(int? TraCtfId,bool IsPersonalDashboard)
        {
            if (!TraCtfId.HasValue)
            {
                return HttpNotFound();
            }

            var ctfData = _ctfBLL.GetCtf().Where(x => x.TraCtfId == TraCtfId.Value).FirstOrDefault();

            if (ctfData == null)
            {
                return HttpNotFound();
            }
            try
            {
                var model = new CtfItem();
                model = Mapper.Map<CtfItem>(ctfData);
                model.IsPersonalDashboard = IsPersonalDashboard;
                model = initCreate(model);
                model.CurrentLogin = CurrentUser;
                model.BuyCostTotalStr = model.BuyCostTotal == null ? "" : Convert.ToInt32(model.BuyCostTotal).ToString();
                model.TitleForm = "Car Termination Form";
                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
            }
        }
        public ActionResult DetailsWTC(int? TraCtfId, bool IsPersonalDashboard)
        {
            if (!TraCtfId.HasValue)
            {
                return HttpNotFound();
            }

            var ctfData = _ctfBLL.GetCtf().Where(x => x.TraCtfId == TraCtfId.Value).FirstOrDefault();

            if (ctfData == null)
            {
                return HttpNotFound();
            }
            try
            {
                var model = new CtfItem();
                model = Mapper.Map<CtfItem>(ctfData);
                model.IsPersonalDashboard = IsPersonalDashboard;
                model = initCreate(model);
                model.CurrentLogin = CurrentUser;
                model.TitleForm = "Car Termination Form WTC";
                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
            }
        }

        #endregion

        #region --------- Edit --------------
        public ActionResult Edit(int? TraCtfId, bool IsPersonalDashboard)
        {
            if (!TraCtfId.HasValue)
            {
                return HttpNotFound();
            }

            var ctfData = _ctfBLL.GetCtf().Where(x => x.TraCtfId == TraCtfId.Value).FirstOrDefault();

            if (ctfData == null)
            {
                return HttpNotFound();
            }
            var settingData = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
            var benefitType = settingData.Where(x => x.SettingName.ToUpper() == "BENEFIT").FirstOrDefault().SettingName;
            var wtcType = settingData.Where(x => x.SettingName.ToUpper() == "WTC").FirstOrDefault().SettingName;

            if (ctfData.VehicleType == wtcType)
            {
                return RedirectToAction("EditWTC", "TraCtf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard = IsPersonalDashboard });
            }
            else
            {
                return RedirectToAction("EditBenefit", "TraCtf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard = IsPersonalDashboard });
            }
        }
        public ActionResult EditBenefit(int? TraCtfId, bool IsPersonalDashboard)
        {
            if (!TraCtfId.HasValue)
            {
                return HttpNotFound();
            }

            var ctfData = _ctfBLL.GetCtf().Where(x => x.TraCtfId == TraCtfId.Value).FirstOrDefault();

            if (ctfData == null)
            {
                return HttpNotFound();
            }
            //if user want to edit doc
            if (CurrentUser.EMPLOYEE_ID == ctfData.EmployeeId && ctfData.DocumentStatus== Enums.DocumentStatus.AssignedForUser)
            {
                return RedirectToAction("EditForEmployeeBenefit", "TraCTf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard= IsPersonalDashboard });
            }
            if (CurrentUser.UserRole == Enums.UserRole.Fleet && ctfData.DocumentStatus == Enums.DocumentStatus.WaitingFleetApproval)
            {
                return RedirectToAction("ApprovalFleetBenefit", "TraCtf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard = IsPersonalDashboard });
            }
            //if created by want to edit
            if ((CurrentUser.USER_ID != ctfData.CreatedBy && ctfData.DocumentStatus== Enums.DocumentStatus.AssignedForUser ) || (CurrentUser.USER_ID != ctfData.CreatedBy && ctfData.DocumentStatus == Enums.DocumentStatus.Draft || (CurrentUser.USER_ID != ctfData.CreatedBy && ctfData.DocumentStatus == Enums.DocumentStatus.WaitingFleetApproval)))
            {
                return RedirectToAction("DetailsBenefit", "TraCtf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard = IsPersonalDashboard });
            }
            try
            {
                var model = new CtfItem();
                model = Mapper.Map<CtfItem>(ctfData);
                model.IsPersonalDashboard = IsPersonalDashboard;
                model = initCreate(model);
                model.CurrentLogin = CurrentUser;
                model.BuyCostTotalStr = model.BuyCostTotal == null ? "" : Convert.ToInt32( model.BuyCostTotal).ToString();
                model.TitleForm = "Car Termination Form";
                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
            }
        }
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBenefit(CtfItem model)
        {
            try
            {
                var dataToSave = Mapper.Map<TraCtfDto>(model);
                dataToSave.Penalty = _ctfBLL.PenaltyCost(dataToSave);
                dataToSave.DocumentStatus = Enums.DocumentStatus.Draft;
                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;
                if (model.BuyCostTotalStr != null)
                {
                    dataToSave.BuyCostTotal = Convert.ToDecimal(model.BuyCostTotalStr.Replace(",", ""));
                }
                var saveResult = _ctfBLL.Save(dataToSave, CurrentUser);

                bool isSubmit = model.isSubmit == "submit";  
                if (isSubmit)
                {
                    CtfWorkflow(model.TraCtfId, Enums.ActionType.Submit, null,false,true,model.DocumentNumber);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("DetailsBenefit", "TraCtf", new { @TraCtfId = model.TraCtfId, IsPersonalDashboard = model.IsPersonalDashboard });
                }
                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                return RedirectToAction("EditBenefit", "TraCtf", new { TraCtfId = model.TraCtfId, IsPersonalDashboard = model.IsPersonalDashboard });

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = initCreate(model);
                model.CurrentLogin = CurrentUser;
                model.ErrorMessage = exception.Message;
                return View(model);
            }
        }

        public ActionResult EditWTC(int? TraCtfId, bool IsPersonalDashboard)
        {
            if (!TraCtfId.HasValue)
            {
                return HttpNotFound();
            }

            var ctfData = _ctfBLL.GetCtf().Where(x => x.TraCtfId == TraCtfId.Value).FirstOrDefault();

            if (ctfData == null)
            {
                return HttpNotFound();
            }
            //if user want to edit doc
            if (CurrentUser.EMPLOYEE_ID == ctfData.EmployeeId && ctfData.DocumentStatus == Enums.DocumentStatus.AssignedForUser)
            {
                return RedirectToAction("EditForEmployeeWTC", "TraCTf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard = IsPersonalDashboard });
            }
            if (CurrentUser.UserRole == Enums.UserRole.Fleet && ctfData.DocumentStatus == Enums.DocumentStatus.WaitingFleetApproval)
            {
                return RedirectToAction("ApprovalFleetWTC", "TraCtf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard = IsPersonalDashboard });
            }
            //if created by want to edit
            if ((CurrentUser.USER_ID != ctfData.CreatedBy && ctfData.DocumentStatus == Enums.DocumentStatus.AssignedForUser) || (CurrentUser.USER_ID != ctfData.CreatedBy && ctfData.DocumentStatus == Enums.DocumentStatus.Draft || (CurrentUser.USER_ID != ctfData.CreatedBy && ctfData.DocumentStatus == Enums.DocumentStatus.WaitingFleetApproval)))
            {
                return RedirectToAction("DetailsWTC", "TraCtf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard = IsPersonalDashboard });
            }
            try
            {
                var model = new CtfItem();
                model = Mapper.Map<CtfItem>(ctfData);
                model.IsPersonalDashboard = IsPersonalDashboard;
                model = initCreate(model);
                model.CurrentLogin = CurrentUser;
                model.TitleForm = "Car Termination Form";
                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditWTC(CtfItem model)
        {
            try
            {
                var dataToSave = Mapper.Map<TraCtfDto>(model);

                dataToSave.DocumentStatus = Enums.DocumentStatus.Draft;
                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;

                bool isSubmit = model.isSubmit == "submit";
                var saveResult = _ctfBLL.Save(dataToSave, CurrentUser);

                var settingData = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
                var benefitType = settingData.Where(x => x.SettingName.ToUpper() == "BENEFIT").FirstOrDefault().SettingName;
                var wtcType = settingData.Where(x => x.SettingName.ToUpper() == "WTC").FirstOrDefault().SettingName;

                var reasonStr = _reasonBLL.GetReasonById(model.Reason.Value).Reason;

                var IsBenefit = model.VehicleType == benefitType;
                var IsEndRent = reasonStr.ToLower() == "end rent";

                if (isSubmit)
                {
                    if (IsEndRent)
                    {
                        CtfWorkflow(model.TraCtfId, Enums.ActionType.Submit, null, IsEndRent, IsBenefit, model.DocumentNumber);
                        AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                        return RedirectToAction("DetailsWTC", "TraCtf", new { TraCtfId = model.TraCtfId, IsPersonalDashboard = false });
                    }
                    CtfWorkflow(model.TraCtfId, Enums.ActionType.Submit, null,false,false,model.DocumentNumber);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("DetailsWTC", "TraCtf", new { @TraCtfId = model.TraCtfId, IsPersonalDashboard= model.IsPersonalDashboard });
                }

                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                return RedirectToAction("EditWTC", "TraCtf", new { @TraCtfId = model.TraCtfId, IsPersonalDashboard = model.IsPersonalDashboard });

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = initCreate(model);
                model.CurrentLogin = CurrentUser;
                model.ErrorMessage = exception.Message;
                return View(model);
            }
        }
        #endregion

        #region ------- Edit For Employee ---------
        public ActionResult EditForEmployeeBenefit(int? TraCtfId , bool IsPersonalDashboard)
        {
            if (!TraCtfId.HasValue)
            {
                return HttpNotFound();
            }

            var ctfData = _ctfBLL.GetCtf().Where(x => x.TraCtfId == TraCtfId.Value).FirstOrDefault();

            if (ctfData == null)
            {
                return HttpNotFound();
            }
           
            try
            {
                var model = new CtfItem();
                model = Mapper.Map<CtfItem>(ctfData);
                model.IsPersonalDashboard = IsPersonalDashboard;
                model = initCreate(model);
                model.CurrentLogin = CurrentUser;
                model.BuyCostTotalStr = model.BuyCostTotal == null ? "" : string.Format("{0:n0}", model.BuyCostTotal);
                model.TitleForm = "Car Termination Form";
                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditForEmployeeBenefit(CtfItem model)
        {
            var a = ModelState.IsValid;
            try
            {
                var dataToSave = Mapper.Map<TraCtfDto>(model);

                dataToSave.DocumentStatus = Enums.DocumentStatus.AssignedForUser;
                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;
                var saveResult = _ctfBLL.Save(dataToSave, CurrentUser);

                bool isSubmit = model.isSubmit == "submit";
                if (isSubmit)
                {
                    CtfWorkflow(model.TraCtfId, Enums.ActionType.Submit, null,false,true, model.DocumentNumber);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("DetailsBenefit", "TraCtf", new { @TraCtfId = model.TraCtfId, @IsPersonalDashboard = model.IsPersonalDashboard });
                }
                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                return RedirectToAction("EditForEmployeeBenefit", "TraCtf", new { @TraCtfId = model.TraCtfId, @IsPersonalDashboard = model.IsPersonalDashboard });

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = initCreate(model);
                model.CurrentLogin = CurrentUser;
                return View(model);
            }
        }
        public ActionResult EditForEmployeeWTC(int? TraCtfId, bool IsPersonalDashboard)
        {
            if (!TraCtfId.HasValue)
            {
                return HttpNotFound();
            }

            var ctfData = _ctfBLL.GetCtf().Where(x => x.TraCtfId == TraCtfId.Value).FirstOrDefault();

            if (ctfData == null)
            {
                return HttpNotFound();
            }

            try
            {
                var model = new CtfItem();
                model = Mapper.Map<CtfItem>(ctfData);
                model.IsPersonalDashboard = IsPersonalDashboard;
                model = initCreate(model);
                model.CurrentLogin = CurrentUser;
                model.TitleForm = "Car Termination Form";
                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditForEmployeeWTC(CtfItem model)
        {
            var a = ModelState.IsValid;
            try
            {
                var dataToSave = Mapper.Map<TraCtfDto>(model);

                dataToSave.DocumentStatus = Enums.DocumentStatus.AssignedForUser;
                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;
                var saveResult = _ctfBLL.Save(dataToSave, CurrentUser);

                bool isSubmit = model.isSubmit == "submit";
                if (isSubmit)
                {
                    CtfWorkflow(model.TraCtfId, Enums.ActionType.Submit, null, false,false,dataToSave.DocumentNumber);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("DetailsWTC", "TraCtf", new { @TraCtfId = model.TraCtfId, @IsPersonalDashboard = model.IsPersonalDashboard });
                }
                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                return RedirectToAction("EditForEmployeeWTC", "TraCtf", new { @TraCtfId = model.TraCtfId, @IsPersonalDashboard = model.IsPersonalDashboard });

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = initCreate(model);
                model.CurrentLogin = CurrentUser;
                return View(model);
            }
        }
        #endregion

        #region --------- AprovalFleet --------------
        public ActionResult ApprovalFleetBenefit(int? TraCtfId, bool IsPersonalDashboard)
        {
            if (!TraCtfId.HasValue)
            {
                return HttpNotFound();
            }

            var ctfData = _ctfBLL.GetCtfById(TraCtfId.Value);

            if (ctfData == null)
            {
                return HttpNotFound();
            }

            if (CurrentUser.UserRole != Enums.UserRole.Fleet)
            {
                return RedirectToAction("DetailsBenefit", "TraCtf", new { id = ctfData.TraCtfId });
            }

            try
            {
                var model = new CtfItem();
                model = Mapper.Map<CtfItem>(ctfData);
                model.IsPersonalDashboard = IsPersonalDashboard;
                model = initCreate(model);
                model.CurrentLogin = CurrentUser;
                model.BuyCostTotalStr = model.BuyCostTotal == null ? "" : Convert.ToInt32(model.BuyCostTotal).ToString();

                var RemarkList = _remarkBLL.GetRemark().Where(x => x.RoleType == CurrentUser.UserRole.ToString() && x.DocumentType == (int)Enums.DocumentType.CTF).ToList();
                model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");

                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApprovalFleetBenefit(CtfItem model)
        {
            try
            {
                var dataToSave = Mapper.Map<TraCtfDto>(model);
                
                dataToSave.DocumentStatus = Enums.DocumentStatus.WaitingFleetApproval;
                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;
                dataToSave.EmployeeIdFleetApproval = CurrentUser.EMPLOYEE_ID;
                dataToSave.ApprovedFleet = CurrentUser.USER_ID;
                dataToSave.ApprovedFleetDate = DateTime.Now;

                var Reason = _reasonBLL.GetReasonById(dataToSave.Reason.Value);
                
                var saveResult = _ctfBLL.Save(dataToSave, CurrentUser);
                var reasonStr = _reasonBLL.GetReasonById(model.Reason.Value).Reason;

                bool isSubmit = model.isSubmit == "submit";
                var IsEndRent = reasonStr.ToLower() == "end rent";
                
                if (isSubmit)
                {
                    if (IsEndRent)
                    {
                        CtfWorkflow(model.TraCtfId, Enums.ActionType.Approve, null, IsEndRent, true, model.DocumentNumber);
                        AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                        return RedirectToAction("DetailsBenefit", "TraCtf", new { @TraCtfId = model.TraCtfId, IsPersonalDashboard = model.IsPersonalDashboard });
                    }

                    CtfWorkflow(model.TraCtfId, Enums.ActionType.Approve, null, false,true, model.DocumentNumber);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("DetailsBenefit", "TraCtf", new { @TraCtfId = model.TraCtfId, IsPersonalDashboard = model.IsPersonalDashboard });
                }
                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                return RedirectToAction(model.IsPersonalDashboard ? "PersonalDashboard" : "Index");

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = initCreate(model);
                model.CurrentLogin = CurrentUser;
                model.ErrorMessage = exception.Message;
                return View(model);
            }
        }
        public ActionResult ApprovalFleetWTC(int? TraCtfId, bool IsPersonalDashboard)
        {
            if (!TraCtfId.HasValue)
            {
                return HttpNotFound();
            }

            var ctfData = _ctfBLL.GetCtfById(TraCtfId.Value);

            if (ctfData == null)
            {
                return HttpNotFound();
            }

            if (CurrentUser.UserRole != Enums.UserRole.Fleet)
            {
                return RedirectToAction("DetailsWTC", "TraCtf", new { TraCtfId = ctfData.TraCtfId , IsPersonalDashboard  = IsPersonalDashboard });
            }

            try
            {
                var model = new CtfItem();
                model = Mapper.Map<CtfItem>(ctfData);
                model.IsPersonalDashboard = IsPersonalDashboard;
                model = initCreate(model);

                var RemarkList = _remarkBLL.GetRemark().Where(x => x.RoleType == CurrentUser.UserRole.ToString() && x.DocumentType == (int)Enums.DocumentType.CTF).ToList();
                model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");

                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApprovalFleetWTC(CtfItem model)
        {
            try
            {
                var dataToSave = Mapper.Map<TraCtfDto>(model);

                dataToSave.DocumentStatus = Enums.DocumentStatus.WaitingFleetApproval;
                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;
                dataToSave.EmployeeIdFleetApproval = CurrentUser.EMPLOYEE_ID;
                dataToSave.ApprovedFleet = CurrentUser.USER_ID;
                dataToSave.ApprovedFleetDate = DateTime.Now;

                var saveResult = _ctfBLL.Save(dataToSave, CurrentUser);
                bool isSubmit = model.isSubmit == "submit";
                
                if (isSubmit)
                {
                    CtfWorkflow(model.TraCtfId, Enums.ActionType.Approve, null, false,false,model.DocumentNumber);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);

                    return RedirectToAction("DetailsWTC", "TraCtf", new { @TraCtfId = model.TraCtfId , IsPersonalDashboard =model.IsPersonalDashboard});
                }
                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                return RedirectToAction(model.IsPersonalDashboard ? "PersonalDashboard" : "Index");

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = initCreate(model);
                model.CurrentLogin = CurrentUser;
                model.ErrorMessage = exception.Message;
                return View(model);
            }
        }
        #endregion

        #region --------- RejectFleet --------------
        public ActionResult RejectCtfBenefit(int TraCtfIdReject, int RemarkId, bool IsPersonalDashboard)
        {
            bool isSuccess = false;
            var CtfDoc = _ctfBLL.GetCtfById(TraCtfIdReject);
            try
            {

                CtfDoc.ApprovedFleet = CurrentUser.USER_ID;
                CtfDoc.ApprovedFleetDate= DateTime.Now;
                CtfDoc.EmployeeIdFleetApproval = CurrentUser.EMPLOYEE_ID;
                var saveResult = _ctfBLL.Save(CtfDoc, CurrentUser);
                CtfWorkflow(TraCtfIdReject, Enums.ActionType.Reject, RemarkId,false,true, CtfDoc.DocumentNumber);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("DetailsBenefit", "TraCtf", new { TraCtfId = TraCtfIdReject, IsPersonalDashboard = IsPersonalDashboard });
            AddMessageInfo("Success Reject Document", Enums.MessageInfoType.Success);
            return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
        }
        public ActionResult RejectCtfWTC(int TraCtfIdReject, int RemarkId, bool IsPersonalDashboard)
        {
            bool isSuccess = false;
            var CtfDoc = _ctfBLL.GetCtfById(TraCtfIdReject);
            try
            {
                CtfDoc.ApprovedFleet = CurrentUser.USER_ID;
                CtfDoc.ApprovedFleetDate = DateTime.Now;
                CtfDoc.EmployeeIdFleetApproval = CurrentUser.EMPLOYEE_ID;
                var saveResult = _ctfBLL.Save(CtfDoc, CurrentUser);
                CtfWorkflow(TraCtfIdReject, Enums.ActionType.Reject, RemarkId,false,false,CtfDoc.DocumentNumber);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("DetailsWTC", "TraCtf", new { TraCtfId = TraCtfIdReject,IsPersonalDashboard = IsPersonalDashboard });
            AddMessageInfo("Success Reject Document", Enums.MessageInfoType.Success);
            return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
        }
        #endregion

        #region --------- Cancel Document CTF --------------

        public ActionResult CancelCtf(long TraCtfId, int RemarkId, string type, bool IsPersonalDashboard)
        {
            if (ModelState.IsValid)
            {
                try
                {
                  _ctfBLL.CancelCtf(TraCtfId, RemarkId, CurrentUser.USER_ID);
                    AddMessageInfo("Success Cancelled Document", Enums.MessageInfoType.Success);
                }
                catch (Exception)
                {

                }
            }
             
                return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
            
            
        }
        #endregion

        #region --------- Dashboar Epaf --------------
        public ActionResult DashboardEpaf()
        {
            if (CurrentUser.UserRole != Enums.UserRole.HR && CurrentUser.UserRole != Enums.UserRole.Viewer &&  CurrentUser.UserRole != Enums.UserRole.HR)
            {
                return RedirectToAction("Index", "TraCtf");
            }

            var EpafData = _epafBLL.GetEpafByDocType(Enums.DocumentType.CTF).ToList();
            var RemarkList = _remarkBLL.GetRemark().Where(x => x.RoleType == CurrentUser.UserRole.ToString() && x.DocumentType == (int)Enums.DocumentType.CTF).ToList();

            var model = new CtfModel();
            model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");
            foreach (var data in EpafData)
            {
                var item = new CtfItem();
                item.EPafData = data;
                var traCtf = _ctfBLL.GetCtf().Where(x => x.EpafId == data.MstEpafId).FirstOrDefault();
                if (traCtf != null)
                {
                    item.EPafData.CtfNumber = traCtf == null ? "" : traCtf.DocumentNumber;
                    item.EPafData.CtfStatus = traCtf.DocumentStatus.ToString();
                    item.EPafData.CtfId = traCtf.TraCtfId;
                    item.ModifiedBy = traCtf.ModifiedBy;
                    item.ModifiedDate = traCtf.ModifiedDate;
                }
                model.Details.Add(item);
            }
            model.TitleForm = "CTF Dashboard";
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }
        public ActionResult CloseEpaf(long MstEpafId, int RemarkId)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _epafBLL.DeactivateEpaf(MstEpafId, RemarkId, CurrentUser.USER_ID);
                }
                catch (Exception)
                {

                }

            }
            return RedirectToAction("DashboardEpaf", "TraCtf");
        }
        public ActionResult Assign(long MstEpafId)
        {
            var CtfData = new TraCtfDto();
            if (ModelState.IsValid)
            {
                var data = _epafBLL.GetEpaf().Where(x => x.MstEpafId == MstEpafId).FirstOrDefault();
                
                if (data == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                try
                {
                    TraCtfDto item = new TraCtfDto();

                    item = Mapper.Map<TraCtfDto>(data);

                    var reason = _reasonBLL.GetReason().Where(x => x.DocumentType == (int)Enums.DocumentType.CTF && x.Reason.ToLower() == data.EpafAction.ToLower()).FirstOrDefault();

                    if (reason == null)
                    {
                        AddMessageInfo("Please Add Reason In Master Data", Enums.MessageInfoType.Warning);
                        return RedirectToAction("DashboardEpaf", "TraCtf");
                    }
                    var settingData = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
                    var benefitType = settingData.Where(x => x.SettingName.ToUpper() == "BENEFIT").FirstOrDefault().SettingName;

                    var FleetData = _fleetBLL.GetFleet().Where(x => x.EmployeeID == item.EmployeeId && x.IsActive == true && (x.VehicleType == benefitType )).FirstOrDefault();

                    item.Reason = reason.MstReasonId;
                    item.CreatedBy = CurrentUser.USER_ID;
                    item.CreatedDate = DateTime.Now;
                    item.EmployeeIdCreator = CurrentUser.EMPLOYEE_ID;
                    item.DocumentStatus = Enums.DocumentStatus.Draft;
                    item.EpafId = data.MstEpafId;
                    item.IsActive = true;
                    if (FleetData != null)
                    {
                        item.CostCenter = FleetData.CostCenter;
                        item.PoliceNumber = FleetData.PoliceNumber;
                        item.VehicleYear = FleetData.VehicleYear;
                        item.VehicleUsage= FleetData.VehicleUsage;
                        item.VehicleLocation = FleetData.City;
                        item.VehicleType = FleetData.VehicleType;
                        item.SupplyMethod = FleetData.SupplyMethod;
                        item.EndRendDate = FleetData.EndContract;
                    }
                    CtfData = _ctfBLL.Save(item, CurrentUser);
                    
                    AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
                    CtfWorkflow(CtfData.TraCtfId, Enums.ActionType.Created, null,false,true,CtfData.DocumentNumber);
                }
                catch (Exception exp)
                {  
                    AddMessageInfo(exp.Message, Enums.MessageInfoType.Error);
                    return RedirectToAction("DashboardEpaf", "TraCtf");
                }
            }
            return RedirectToAction("EditBenefit", "TraCtf", new { @TraCtfId = CtfData.TraCtfId, IsPersonalDashboard = false });
        }


        #endregion

        #region --------- Completed Document--------------
        public ActionResult Completed()
        {
            var model = new CtfModel();
            var data = new List<TraCtfDto>();

            data = _ctfBLL.GetCtfDashboard(CurrentUser, true);
            model.TitleForm = "CTF Completed Document";
           
            model.Details = Mapper.Map<List<CtfItem>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }
        #endregion

        #region --------- Json --------------
        [HttpPost]
        public JsonResult GetEmployee(string Id)
        {
            var model = new CtfItem();
            var employee = _employeeBLL.GetByID(Id);
            if (employee != null)
            {
                model.EmployeeName = employee.FORMAL_NAME;
                model.CostCenter = employee.COST_CENTER;
                model.GroupLevel = employee.GROUP_LEVEL;

            }
            var settingData = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
            var benefitType = settingData.Where(x => x.SettingName.ToUpper() == "BENEFIT").FirstOrDefault().SettingName;
            var wtcType = settingData.Where(x => x.SettingName.ToUpper() == "WTC").FirstOrDefault().SettingName;

            var vehicle = new FleetDto();

            if (CurrentUser.UserRole == Enums.UserRole.HR)
            {
                vehicle = _fleetBLL.GetFleet().Where(x => x.IsActive == true && x.VehicleType == benefitType && x.EmployeeID == Id).FirstOrDefault();
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Fleet)
            {
                vehicle = _fleetBLL.GetFleet().Where(x => x.IsActive == true  && x.EmployeeID == Id).FirstOrDefault();
            }
            if (vehicle != null)
            {
                model.PoliceNumber = vehicle.PoliceNumber;
                model.VehicleYear = vehicle.VehicleYear;
                model.VehicleType = vehicle.VehicleType;
                model.VehicleUsage = vehicle.VehicleUsage;
                model.SupplyMethod = vehicle.SupplyMethod;
                model.EndRendDate = vehicle.EndContract;
            }
            return Json(model);
        }


        public JsonResult GetEmployeeList()
        {
            var model = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE && x.GROUP_LEVEL > 0).Select(x => new { x.EMPLOYEE_ID, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetEmployeeList()
        //{
        //    var model = new List<EmployeeDto>();
        //    if (CurrentUser.UserRole == Enums.UserRole.HR)
        //    {
        //        model = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE && x.GROUP_LEVEL > 0).OrderBy(x => x.FORMAL_NAME).ToList();
        //    }
        //    else
        //    {
        //        model = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE ).OrderBy(x => x.FORMAL_NAME).ToList();
        //    }
            
        //    return Json(model, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public JsonResult SetExtendVehicle()
        {
            var model = "";
            return Json(model);
        }
        [HttpPost]
        public JsonResult GetVehicle(string Id)
        {
            var model = new CtfItem();
            var settingData = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
            var benefitType = settingData.Where(x => x.SettingName.ToUpper() == "BENEFIT").FirstOrDefault().SettingName;
            var wtcType = settingData.Where(x => x.SettingName.ToUpper() == "WTC").FirstOrDefault().SettingName;

            var vehicle = new FleetDto();

            if (CurrentUser.UserRole == Enums.UserRole.HR)
            {
                vehicle = _fleetBLL.GetFleet().Where(x => x.IsActive == true && x.VehicleType == benefitType && x.PoliceNumber == Id).FirstOrDefault();
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Fleet)
            {
                vehicle = _fleetBLL.GetFleet().Where(x => x.IsActive == true && x.PoliceNumber == Id).FirstOrDefault();
            }
            if (vehicle != null)
            {
                model.PoliceNumber = vehicle.PoliceNumber;
                model.VehicleYear = vehicle.VehicleYear;
                model.VehicleType = vehicle.VehicleType;
                model.VehicleUsage = vehicle.VehicleUsage;
                model.SupplyMethod = vehicle.SupplyMethod;
                model.EndRendDate = vehicle.EndContract;
            }
            var employee = _employeeBLL.GetByID(vehicle.EmployeeID);
            if (employee != null)
            {
                model.EmployeeId= employee.EMPLOYEE_ID;
                model.EmployeeName = employee.FORMAL_NAME;
                model.CostCenter = employee.COST_CENTER;
                model.GroupLevel = employee.GROUP_LEVEL;

            }
            return Json(model);
        }

        public JsonResult GetPoliceNumberList()
        {
            var settingData = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
            var benefitType = settingData.Where(x => x.SettingName.ToUpper() == "BENEFIT").FirstOrDefault().SettingName;
           var wtcType = settingData.Where(x => x.SettingName.ToUpper() == "WTC").FirstOrDefault().SettingName;

            var model = new List<FleetDto>();
           
            if (CurrentUser.UserRole == Enums.UserRole.HR)
            {
               model = _fleetBLL.GetFleet().Where(x => x.IsActive == true && x.VehicleType == benefitType).ToList();
            }
            else 
            {
                model = _fleetBLL.GetFleet().Where(x => x.IsActive == true && x.VehicleType == wtcType ).ToList();
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region --------- CTF Workflow --------------
        private void CtfWorkflow(long id, Enums.ActionType actionType, int? comment,bool Endrent, bool isBenefit, string DocumentNumber)
        {
            var input = new CtfWorkflowDocumentInput
            {
                DocumentId = id,
                UserId = CurrentUser.USER_ID,
                UserRole = CurrentUser.UserRole,
                EmployeeId =CurrentUser.EMPLOYEE_ID,
                ActionType = actionType,
                Comment = comment,
                EndRent = Endrent,
                isBenefit = isBenefit,
                DocumentNumber = DocumentNumber
            };

            _ctfBLL.CtfWorkflow(input);
        }

        #endregion

        #region --------- Export--------------
        //---------------------------- Viewer --------------------------------------------
        public void ExportCompletedViewer()
        {
            string pathFile = "";

            pathFile = CreateXlsCompletedViewer();

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

        private string CreateXlsCompletedViewer()
        {
            //get data
            var data = _ctfBLL.GetCtf().Where(x =>  x.DocumentStatus == Enums.DocumentStatus.Completed || x.DocumentStatus == Enums.DocumentStatus.Cancelled).ToList();

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Completed CTF");
            slDocument.MergeWorksheetCells(1, 1, 1, 15);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelBenefit(slDocument);

            //create data
            slDocument = CreateDataExcelBenefit(slDocument, data, true);

            var fileName = "Completed_CTF_document" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;
        }

        public void ExportOpenViewer()
        {
            string pathFile = "";

            pathFile = CreateXlsOpenViewer();

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
        private string CreateXlsOpenViewer()
        {
            //get data
            var data = _ctfBLL.GetCtf().Where(x => x.DocumentStatus != Enums.DocumentStatus.Completed && x.DocumentStatus != Enums.DocumentStatus.Cancelled).ToList();

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Open Document CTF");
            slDocument.MergeWorksheetCells(1, 1, 1, 15);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelBenefit(slDocument);

            //create data
            slDocument = CreateDataExcelBenefit(slDocument, data, false);

            var fileName = "Open_CTF_document" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }
        //--------------------------------------------------------------------------------
        public void ExportEpaf()
        {
            string pathFile = "";

            pathFile = CreateXlsEpaf();

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
        private string CreateXlsEpaf()
        {
            //get data
            var data = _epafBLL.GetEpafByDocType(Enums.DocumentType.CTF).ToList();

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Epaf");
            slDocument.MergeWorksheetCells(1, 1, 1, 12);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelEpaf(slDocument);

            //create data
            slDocument = CreateDataExcelEpaf(slDocument, data, true);

            var fileName = "Epaf" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }
        private SLDocument CreateHeaderExcelEpaf(SLDocument slDocument)
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
            slDocument.SetCellValue(iRow, 9, "CSF No");
            slDocument.SetCellValue(iRow, 10, "CSF Status");
            slDocument.SetCellValue(iRow, 11, "Modified By");
            slDocument.SetCellValue(iRow, 12, "Modified Date");

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
        private SLDocument CreateDataExcelEpaf(SLDocument slDocument, List<EpafDto> listData, bool isComplete)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.EfectiveDate == null ? "" : data.EfectiveDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 2, data.ApprovedDate == null ? "" : data.ApprovedDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 3, data.LetterSend);
                slDocument.SetCellValue(iRow, 4, data.EpafAction);
                slDocument.SetCellValue(iRow, 5, data.EmployeeId);
                slDocument.SetCellValue(iRow, 6, data.EmployeeName);
                slDocument.SetCellValue(iRow, 7, data.CostCenter);
                slDocument.SetCellValue(iRow, 8, data.GroupLevel);
                var ctf = new TraCtfDto();
                ctf=_ctfBLL.GetCtf().Where(x=>x.EpafId == data.MstEpafId).FirstOrDefault();
                slDocument.SetCellValue(iRow, 9, ctf == null ? "" :ctf.DocumentNumber);
                slDocument.SetCellValue(iRow, 10, ctf == null ? "": ctf.DocumentStatus.ToString());
                slDocument.SetCellValue(iRow, 11,ctf==null? "" : data.ModifiedBy);
                slDocument.SetCellValue(iRow, 12, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
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

        public void ExportCompleted()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Fleet)
            {
                ExportCompletedWTC();
            }
            else if(CurrentUser.UserRole == Enums.UserRole.HR)
            {
                ExportCompletedBeneift();
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                ExportCompletedViewer();
            }
        }
        public void ExportOpen()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Fleet)
            {
                ExportOpenWTC();
            }
            else if (CurrentUser.UserRole == Enums.UserRole.HR)
            {
                ExportOpenBeneift();
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                ExportOpenViewer();
            }

        }
     
        public void ExportCompletedWTC()
        {
            string pathFile = "";

            pathFile = CreateXlsCompletedWTC();

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
        private string CreateXlsCompletedWTC()
        {
            //get data
            var data = _ctfBLL.GetCtf().Where(x =>  x.VehicleType == "WTC" && (x.DocumentStatus==Enums.DocumentStatus.Completed || x.DocumentStatus== Enums.DocumentStatus.Cancelled)).ToList();

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Completed CTF WTC");
            slDocument.MergeWorksheetCells(1, 1, 1, 14);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelWTC(slDocument);

            //create data
            slDocument = CreateDataExcelWTC(slDocument, data, true);

            var fileName = "Completed_CTF_document_WTC" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }
        private SLDocument CreateHeaderExcelWTC(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "CTF Number");
            slDocument.SetCellValue(iRow, 2, "CTF Status");
            slDocument.SetCellValue(iRow, 3, "Reason Terminate");
            slDocument.SetCellValue(iRow, 4, "Termination Date");
            slDocument.SetCellValue(iRow, 5, "Police Number");
            slDocument.SetCellValue(iRow, 6, "Vehicle Type");
            slDocument.SetCellValue(iRow, 7, "End Rent Date");
            slDocument.SetCellValue(iRow, 8, "Employee ID");
            slDocument.SetCellValue(iRow, 9, "Employee Name");
            slDocument.SetCellValue(iRow, 10, "Vehicle Location");
            slDocument.SetCellValue(iRow, 11, "Cost Center");
            slDocument.SetCellValue(iRow, 12, "Supply Method");
            slDocument.SetCellValue(iRow, 13, "Updated By");
            slDocument.SetCellValue(iRow, 14, "Updated Date");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 14, headerStyle);

            return slDocument;

        }
        private SLDocument CreateDataExcelWTC(SLDocument slDocument, List<TraCtfDto> listData,bool isComplete)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.DocumentNumber);
                slDocument.SetCellValue(iRow, 2, isComplete == true ? Enums.DocumentStatus.Completed.ToString() : "");
                slDocument.SetCellValue(iRow, 3, data.ReasonS);
                slDocument.SetCellValue(iRow, 4, data.EffectiveDate == null ? "" : data.EffectiveDate.Value.ToString("dd MMM yyyy"));
                slDocument.SetCellValue(iRow, 5, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 6, data.VehicleType);
                slDocument.SetCellValue(iRow, 7, data.EndRendDate == null? "" : data.EndRendDate.Value.ToString("dd MMM yyyy"));
                slDocument.SetCellValue(iRow, 8, data.EmployeeId);
                slDocument.SetCellValue(iRow, 9, data.EmployeeName);
                slDocument.SetCellValue(iRow, 10, data.VehicleLocation);
                slDocument.SetCellValue(iRow, 11, data.CostCenter);
                slDocument.SetCellValue(iRow, 12, data.SupplyMethod);
                slDocument.SetCellValue(iRow, 13, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 14, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 14);
            slDocument.SetCellStyle(3, 1, iRow - 1, 14, valueStyle);

            return slDocument;
        }

        public void ExportOpenWTC()
        {
            string pathFile = "";

            pathFile = CreateXlsOpenWTC();

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
        private string CreateXlsOpenWTC()
        {
            //get data
            var data = _ctfBLL.GetCtf().Where(x => x.VehicleType == "WTC" && (x.DocumentStatus != Enums.DocumentStatus.Completed && x.DocumentStatus != Enums.DocumentStatus.Cancelled)).ToList();

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Open CTF WTC");
            slDocument.MergeWorksheetCells(1, 1, 1, 14);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelWTC(slDocument);

            //create data
            slDocument = CreateDataExcelWTC(slDocument, data, false);

            var fileName = "Open_CTF_document_WTC" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }
        //--------------------------------Benefit-------------------------------//
        public void ExportCompletedBeneift()
        {
            string pathFile = "";

            pathFile = CreateXlsCompletedBenefit();

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
        private string CreateXlsCompletedBenefit()
        {
            //get data
            var data = _ctfBLL.GetCtf().Where(x =>( x.VehicleType == "Benefit" || x.VehicleType=="BENEFIT" )&& (x.DocumentStatus == Enums.DocumentStatus.Completed || x.DocumentStatus ==Enums.DocumentStatus.Cancelled)).ToList();

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Completed CTF Benefit");
            slDocument.MergeWorksheetCells(1, 1, 1, 15);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelBenefit(slDocument);

            //create data
            slDocument = CreateDataExcelBenefit(slDocument, data, true);

            var fileName = "Completed_CTF_document_Benefit" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }
        private SLDocument CreateHeaderExcelBenefit(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "CTF Number");
            slDocument.SetCellValue(iRow, 2, "CTF Status");
            slDocument.SetCellValue(iRow, 3, "Reason Terminate");
            slDocument.SetCellValue(iRow, 4, "Termination Date");
            slDocument.SetCellValue(iRow, 5, "Police Number");
            slDocument.SetCellValue(iRow, 6, "Vehicle Type");
            slDocument.SetCellValue(iRow, 7, "Vehicle Usage");
            slDocument.SetCellValue(iRow, 8, "End Rent Date");
            slDocument.SetCellValue(iRow, 9, "Employee ID");
            slDocument.SetCellValue(iRow, 10, "Employee Name");
            slDocument.SetCellValue(iRow, 11, "Vehicle Location");
            slDocument.SetCellValue(iRow, 12, "Cost Center");
            slDocument.SetCellValue(iRow, 13, "Supply Method");
            slDocument.SetCellValue(iRow, 14, "Updated By");
            slDocument.SetCellValue(iRow, 15, "Updated Date");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 15, headerStyle);

            return slDocument;

        }
        private SLDocument CreateDataExcelBenefit(SLDocument slDocument, List<TraCtfDto> listData, bool isComplete)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.DocumentNumber);
                slDocument.SetCellValue(iRow, 2, isComplete == true ? Enums.DocumentStatus.Completed.ToString() : "");
                slDocument.SetCellValue(iRow, 3, data.ReasonS);
                slDocument.SetCellValue(iRow, 4, data.EffectiveDate == null ? "" : data.EffectiveDate.Value.ToString("dd MMM yyyy"));
                slDocument.SetCellValue(iRow, 5, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 6, data.VehicleType);
                slDocument.SetCellValue(iRow, 7, data.VehicleUsage);
                slDocument.SetCellValue(iRow, 8, data.EndRendDate == null ? "" : data.EndRendDate.Value.ToString("dd MMM yyyy"));
                slDocument.SetCellValue(iRow, 9, data.EmployeeId);
                slDocument.SetCellValue(iRow, 10, data.EmployeeName);
                slDocument.SetCellValue(iRow, 11, data.VehicleLocation);
                slDocument.SetCellValue(iRow, 12, data.CostCenter);
                slDocument.SetCellValue(iRow, 13, data.SupplyMethod);
                slDocument.SetCellValue(iRow, 14, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 15, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 15);
            slDocument.SetCellStyle(3, 1, iRow - 1, 15, valueStyle);

            return slDocument;
        }

        public void ExportOpenBeneift()
        {
            string pathFile = "";

            pathFile = CreateXlsOpenBenefit();

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
        private string CreateXlsOpenBenefit()
        {
            //get data
            var data = _ctfBLL.GetCtf().Where(x => (x.VehicleType == "Benefit" || x.VehicleType == "BENEFIT") && (x.DocumentStatus != Enums.DocumentStatus.Completed || x.DocumentStatus != Enums.DocumentStatus.Cancelled)).ToList();

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Open CTF Benefit");
            slDocument.MergeWorksheetCells(1, 1, 1, 15);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelBenefit(slDocument);

            //create data
            slDocument = CreateDataExcelBenefit(slDocument, data, false);

            var fileName = "Open_CTF_document_Benefit" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }
        //----------------------------------------------------------------------//
        #endregion
    }
}
