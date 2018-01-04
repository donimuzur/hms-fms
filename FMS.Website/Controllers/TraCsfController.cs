﻿using System;
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
using DocumentFormat.OpenXml.Packaging;

namespace FMS.Website.Controllers
{
    public class TraCsfController : BaseController
    {

        #region --------- Field and Constructor --------------

        private IEpafBLL _epafBLL;
        private ITraCsfBLL _csfBLL;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private IRemarkBLL _remarkBLL;
        private IEmployeeBLL _employeeBLL;
        private IReasonBLL _reasonBLL;
        private ISettingBLL _settingBLL;
        private IFleetBLL _fleetBLL;
        private IVehicleSpectBLL _vehicleSpectBLL;
        private ILocationMappingBLL _locationMappingBLL;
        private ITraTemporaryBLL _tempBLL;
        private IPriceListBLL _priceListBLL;
        private IVendorBLL _vendorBLL;
        private ITraCrfBLL _crfBLL;

        public TraCsfController(IPageBLL pageBll, IEpafBLL epafBll, ITraCsfBLL csfBll, IRemarkBLL RemarkBLL, IEmployeeBLL EmployeeBLL, IReasonBLL ReasonBLL,
            ISettingBLL SettingBLL, IFleetBLL FleetBLL, IVehicleSpectBLL VehicleSpectBLL, ILocationMappingBLL LocationMappingBLL, ITraTemporaryBLL TempBLL,
            IPriceListBLL PriceListBLL, IVendorBLL VendorBLL, ITraCrfBLL crfBLL)
            : base(pageBll, Core.Enums.MenuList.TraCsf)
        {
            _epafBLL = epafBll;
            _csfBLL = csfBll;
            _pageBLL = pageBll;
            _remarkBLL = RemarkBLL;
            _employeeBLL = EmployeeBLL;
            _reasonBLL = ReasonBLL;
            _settingBLL = SettingBLL;
            _fleetBLL = FleetBLL;
            _vehicleSpectBLL = VehicleSpectBLL;
            _locationMappingBLL = LocationMappingBLL;
            _tempBLL = TempBLL;
            _priceListBLL = PriceListBLL;
            _vendorBLL = VendorBLL;
            _crfBLL = crfBLL;
            _mainMenu = Enums.MenuList.Transaction;
        }

        #endregion

        #region --------- Open Document --------------

        public ActionResult Index()
        {
            //check csf in progress
            _csfBLL.CheckCsfInProgress();

            var data = _csfBLL.GetCsf(CurrentUser, false);
            var model = new CsfIndexModel();
            model.TitleForm = "CSF Open Document";
            model.TitleExport = "ExportOpen";
            model.CsfList = Mapper.Map<List<CsfData>>(data.OrderByDescending(x => x.CREATED_DATE));
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        #endregion

        #region --------- Personal Dashboard --------------

        public ActionResult PersonalDashboard()
        {
            var data = _csfBLL.GetCsfPersonal(CurrentUser);
            var model = new CsfIndexModel();
            model.TitleForm = "CSF Personal Dashboard";
            model.TitleExport = "ExportPersonal";
            model.CsfList = Mapper.Map<List<CsfData>>(data.OrderByDescending(x => x.CREATED_DATE));
            model.MainMenu = Enums.MenuList.PersonalDashboard;
            model.CurrentLogin = CurrentUser;
            model.IsPersonalDashboard = true;
            return View("Index",model);
        }

        #endregion

        #region --------- Dashboard --------------

        public ActionResult Dashboard()
        {
            if (CurrentUser.UserRole != Enums.UserRole.HR && CurrentUser.UserRole != Enums.UserRole.Administrator && CurrentUser.UserRole != Enums.UserRole.Viewer)
            {
                return RedirectToAction("Index");
            }

            var data = _csfBLL.GetCsfEpaf();
            var RemarkList = _remarkBLL.GetRemark().Where(x => x.RoleType == CurrentUser.UserRole.ToString() && x.DocumentType == (int)Enums.DocumentType.CSF).ToList();
            var model = new CsfDashboardModel();
            model.TitleForm = "CSF Dashboard";
            model.EpafList = Mapper.Map<List<EpafData>>(data);
            model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        #endregion

        #region --------- Completed Document --------------

        public ActionResult Completed()
        {
            var data = _csfBLL.GetCsf(CurrentUser, true);
            var model = new CsfIndexModel();
            model.TitleForm = "CSF Completed Document";
            model.TitleExport = "ExportCompleted";
            model.CsfList = Mapper.Map<List<CsfData>>(data.OrderByDescending(x => x.MODIFIED_DATE));
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.IsCompleted = true;
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

            var model = new CsfItemModel();

            model = InitialModel(model);
            model.Detail.CreateDate = DateTime.Now;
            model.Detail.EffectiveDate = DateTime.Now;
            model.Detail.CreateBy = CurrentUser.USERNAME;
            model.Detail.IsBenefit = CurrentUser.UserRole == Enums.UserRole.HR ? true : false;

            var employeeData = _employeeBLL.GetByID(model.Detail.EmployeeId);
            if (employeeData != null)
            {
                model.Detail.LocationCity = employeeData.BASETOWN;
                model.Detail.LocationAddress = employeeData.ADDRESS;
            }

            return View(model);
        }

        public CsfItemModel InitialModel(CsfItemModel model)
        {
            var settingData = _settingBLL.GetSetting();
            var allEmployee = _employeeBLL.GetEmployee();
            var reasonType = "WTC";

            if (CurrentUser.UserRole == Enums.UserRole.HR)
            {
                allEmployee = allEmployee.Where(x => x.GROUP_LEVEL > 0).ToList();
            }

            var vehTypeBenefit = settingData.Where(x => x.SettingGroup == "VEHICLE_TYPE" && x.SettingName == "BENEFIT").FirstOrDefault().MstSettingId;
            model.Detail.IsBenefit = model.Detail.VehicleType == vehTypeBenefit.ToString() ? true : false;

            var paramVehUsage = EnumHelper.GetDescription(Enums.SettingGroup.VehicleUsageWtc);
            if (model.Detail.IsBenefit || CurrentUser.UserRole == Enums.UserRole.HR)
            {
                paramVehUsage = EnumHelper.GetDescription(Enums.SettingGroup.VehicleUsageBenefit);
                reasonType = "BENEFIT";
            }

            var list = allEmployee.Select(x => new { x.EMPLOYEE_ID, employee = x.EMPLOYEE_ID + " - " + x.FORMAL_NAME, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);
            var listReason = _reasonBLL.GetReason().Where(x => x.DocumentType == (int)Enums.DocumentType.CSF && x.IsActive && x.VehicleType == reasonType).Select(x => new { x.MstReasonId, x.Reason }).ToList().OrderBy(x => x.Reason);
            var listVehType = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.IsActive).Select(x => new { x.MstSettingId, x.SettingValue }).ToList();
            var listVehCat = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleCategory) && x.IsActive).Select(x => new { x.MstSettingId, x.SettingValue }).ToList();
            var listVehUsage = settingData.Where(x => x.SettingGroup == paramVehUsage && x.IsActive).Select(x => new { x.MstSettingId, x.SettingValue }).ToList();
            var listSupMethod = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.SupplyMethod) && x.IsActive).Select(x => new { x.MstSettingId, x.SettingValue }).ToList();
            var listProject = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.Project) && x.IsActive).Select(x => new { x.MstSettingId, x.SettingValue }).ToList();

            model.Detail.EmployeeList = new SelectList(list, "EMPLOYEE_ID", "employee");
            model.Detail.ReasonList = new SelectList(listReason, "MstReasonId", "Reason");
            model.Detail.VehicleTypeList = new SelectList(listVehType, "MstSettingId", "SettingValue");
            model.Detail.VehicleCatList = new SelectList(listVehCat, "MstSettingId", "SettingValue");
            model.Detail.VehicleUsageList = new SelectList(listVehUsage, "MstSettingId", "SettingValue");
            model.Detail.SupplyMethodList = new SelectList(listSupMethod, "MstSettingId", "SettingValue");
            model.Detail.ProjectList = new SelectList(listProject, "MstSettingId", "SettingValue");
            
            model.CurrentLogin = CurrentUser;
            model.MainMenu = model.IsPersonalDashboard ? Enums.MenuList.PersonalDashboard : _mainMenu;

            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.TraCsf, model.Detail.TraCsfId);
            model.WorkflowLogs = GetWorkflowHistory((int)Enums.MenuList.TraCsf, model.Detail.TraCsfId);

            var tempData = _csfBLL.GetTempByCsf(model.Detail.CsfNumber);
            model.TemporaryList = Mapper.Map<List<TemporaryData>>(tempData);
            model.Detail.TemporaryId = tempData.Count();

            var employeeList = _employeeBLL.GetEmployee();
            var CityList = employeeList.Select(x => new { x.BASETOWN }).Distinct().ToList();
            var AddressList = employeeList.Select(x => new { x.ADDRESS }).Distinct().ToList();
            model.Detail.LocationCityList = new SelectList(CityList, "BASETOWN", "BASETOWN");
            model.Detail.LocationAddressList = new SelectList(AddressList, "ADDRESS", "ADDRESS");

            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CsfItemModel model)
        {
            try
            {
                TraCsfDto item = new TraCsfDto();

                item = AutoMapper.Mapper.Map<TraCsfDto>(model.Detail);

                item.EMPLOYEE_ID_CREATOR = CurrentUser.EMPLOYEE_ID;
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

                var checkExistCsfInFleet = _csfBLL.CheckCsfExists(item);
                //only check for benefit in master fleet
                if (checkExistCsfInFleet && CurrentUser.UserRole == Enums.UserRole.HR)
                {
                    model = InitialModel(model);
                    model.ErrorMessage = "Data already exists in master fleet";
                    return View(model);
                }

                var checkExistCsfOpen = _csfBLL.CheckCsfOpenExists(item);
                //only check for benefit in csf
                if (checkExistCsfOpen && CurrentUser.UserRole == Enums.UserRole.HR)
                {
                    model = InitialModel(model);
                    model.ErrorMessage = "Data csf already exists";
                    return View(model);
                }

                var csfData = _csfBLL.Save(item, CurrentUser);

                bool isSubmit = model.Detail.IsSaveSubmit == "submit";

                if (isSubmit)
                {
                    CsfWorkflow(csfData.TRA_CSF_ID, Enums.ActionType.Submit, null);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("Edit", "TraCsf", new { id = csfData.TRA_CSF_ID, isPersonalDashboard = false });
                }

                AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
                CsfWorkflow(csfData.TRA_CSF_ID, Enums.ActionType.Created, null);
                return RedirectToAction("Edit", "TraCsf", new { id = csfData.TRA_CSF_ID, isPersonalDashboard = false });
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

        public ActionResult Detail(int? id, bool isPersonalDashboard)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var csfData = _csfBLL.GetCsfById(id.Value);

            if (csfData == null)
            {
                return HttpNotFound();
            }

            try
            {
                var model = new CsfItemModel();
                model.IsPersonalDashboard = isPersonalDashboard;
                model.Detail = Mapper.Map<CsfData>(csfData);
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

            var csfData = _csfBLL.GetCsfById(id.Value);

            if (csfData == null)
            {
                return HttpNotFound();
            }

            //if user want to edit doc
            if (CurrentUser.EMPLOYEE_ID == csfData.EMPLOYEE_ID && csfData.DOCUMENT_STATUS == Enums.DocumentStatus.AssignedForUser)
            {
                return RedirectToAction("EditForEmployee", "TraCsf", new { id = csfData.TRA_CSF_ID, isPersonalDashboard = isPersonalDashboard });
            }

            //if not created by want to edit
            if (CurrentUser.USER_ID != csfData.CREATED_BY && csfData.DOCUMENT_STATUS == Enums.DocumentStatus.AssignedForUser)
            {
                return RedirectToAction("Detail", "TraCsf", new { id = csfData.TRA_CSF_ID, isPersonalDashboard = isPersonalDashboard });
            }

            //if hr want to approve / reject
            if (csfData.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingHRApproval)
            {
                return RedirectToAction("ApproveHr", "TraCsf", new { id = csfData.TRA_CSF_ID, isPersonalDashboard = isPersonalDashboard });
            }

            //if fleet want to approve / reject
            if (csfData.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingFleetApproval)
            {
                return RedirectToAction("ApproveFleet", "TraCsf", new { id = csfData.TRA_CSF_ID, isPersonalDashboard = isPersonalDashboard });
            }

            //if fleet want to upload
            if (csfData.DOCUMENT_STATUS == Enums.DocumentStatus.InProgress)
            {
                return RedirectToAction("InProgress", "TraCsf", new { id = csfData.TRA_CSF_ID, isPersonalDashboard = isPersonalDashboard });
            }

            try
            {
                var model = new CsfItemModel();
                model.IsPersonalDashboard = isPersonalDashboard;
                model.Detail = Mapper.Map<CsfData>(csfData);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CsfItemModel model)
        {
            try
            {
                var dataToSave = Mapper.Map<TraCsfDto>(model.Detail);

                dataToSave.DOCUMENT_STATUS = Enums.DocumentStatus.Draft;
                dataToSave.MODIFIED_BY = CurrentUser.USER_ID;
                dataToSave.MODIFIED_DATE = DateTime.Now;

                bool isSubmit = model.Detail.IsSaveSubmit == "submit";

                var saveResult = _csfBLL.Save(dataToSave, CurrentUser);

                if (isSubmit)
                {
                    CsfWorkflow(model.Detail.TraCsfId, Enums.ActionType.Submit, null);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("Detail", "TraCsf", new { id = model.Detail.TraCsfId, isPersonalDashboard = model.IsPersonalDashboard });
                }

                //return RedirectToAction("Index");
                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                return RedirectToAction("Edit", "TraCsf", new { id = model.Detail.TraCsfId, isPersonalDashboard = model.IsPersonalDashboard });

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = InitialModel(model);
                model.ErrorMessage = "Error when save data, please contact administrator";
                return View(model);
            }
        }

        public ActionResult EditForEmployee(int? id, bool isPersonalDashboard)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var csfData = _csfBLL.GetCsfById(id.Value);

            if (csfData == null)
            {
                return HttpNotFound();
            }

            if (CurrentUser.EMPLOYEE_ID != csfData.EMPLOYEE_ID)
            {
                return RedirectToAction("Detail", "TraCsf", new { id = csfData.TRA_CSF_ID, isPersonalDashboard = isPersonalDashboard });
            }

            if (Enums.DocumentStatus.AssignedForUser != csfData.DOCUMENT_STATUS)
            {
                return RedirectToAction("Detail", "TraCsf", new { id = csfData.TRA_CSF_ID, isPersonalDashboard = isPersonalDashboard });
            }

            try
            {
                var model = new CsfItemModel();
                model.Detail = Mapper.Map<CsfData>(csfData);
                model.IsPersonalDashboard = isPersonalDashboard;
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
        public ActionResult EditForEmployee(CsfItemModel model)
        {
            try
            {
                //if mass upload wtc
                if(model.VehicleList.Count > 0)
                {
                    var no = 0;
                    foreach (var item in model.VehicleList)
                    {
                        var dataToSave = Mapper.Map<TraCsfDto>(model.Detail);

                        dataToSave.DOCUMENT_STATUS = Enums.DocumentStatus.AssignedForUser;
                        dataToSave.MODIFIED_BY = CurrentUser.USER_ID;
                        dataToSave.MODIFIED_DATE = DateTime.Now;

                        dataToSave.MANUFACTURER = item.Manufacturer;
                        dataToSave.MODEL = item.Models;
                        dataToSave.SERIES = item.Series;
                        dataToSave.BODY_TYPE = item.BodyType;
                        dataToSave.COLOUR = item.Color;
                        dataToSave.VENDOR_NAME = item.Vendor;

                        if (no > 0)
                        {
                            dataToSave.TRA_CSF_ID = 0;
                            dataToSave.EMPLOYEE_ID_CREATOR = model.Detail.EmployeeIdCreator;
                        }

                        bool isSubmit = model.Detail.IsSaveSubmit == "submit";

                        var saveResult = _csfBLL.Save(dataToSave, CurrentUser);

                        if (isSubmit)
                        {
                            CsfWorkflow(saveResult.TRA_CSF_ID, Enums.ActionType.Submit, null);
                        }

                        no++;
                    }

                    AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                    return RedirectToAction(model.IsPersonalDashboard ? "PersonalDashboard" : "Index");
                }
                else
                {
                    var dataToSave = Mapper.Map<TraCsfDto>(model.Detail);

                    dataToSave.DOCUMENT_STATUS = Enums.DocumentStatus.AssignedForUser;
                    dataToSave.MODIFIED_BY = CurrentUser.USER_ID;
                    dataToSave.MODIFIED_DATE = DateTime.Now;

                    bool isSubmit = model.Detail.IsSaveSubmit == "submit";

                    var saveResult = _csfBLL.Save(dataToSave, CurrentUser);

                    if (isSubmit)
                    {
                        //check if wtc not yet select car
                        var vehTypeWtc = _settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_TYPE" && x.SettingName == "WTC").FirstOrDefault().MstSettingId;
                        if (model.Detail.VehicleType == vehTypeWtc.ToString())
                        {
                            if (model.Detail.Manufacturer == null)
                            {
                                model = InitialModel(model);
                                model.ErrorMessage = "Please select vehicle";
                                return View(model);
                            }
                        }

                        //check if benefit not yet select car
                        var vehTypeBen = _settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_TYPE" && x.SettingName == "BENEFIT").FirstOrDefault().MstSettingId;
                        if (model.Detail.VehicleType == vehTypeBen.ToString())
                        {
                            var vehCatNoCar = _settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_CATEGORY" && x.SettingName == "NO_CAR").FirstOrDefault().MstSettingId;
                            if (model.Detail.VehicleCat != vehCatNoCar.ToString())
                            {
                                if (model.Detail.Manufacturer == null)
                                {
                                    model = InitialModel(model);
                                    model.ErrorMessage = "Please select vehicle";
                                    return View(model);
                                }
                            }
                        }

                        CsfWorkflow(model.Detail.TraCsfId, Enums.ActionType.Submit, null);
                        AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                        return RedirectToAction("Detail", "TraCsf", new { id = model.Detail.TraCsfId, isPersonalDashboard = model.IsPersonalDashboard });
                    }

                    //return RedirectToAction("Index");
                    AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                    return RedirectToAction("EditForEmployee", "TraCsf", new { id = model.Detail.TraCsfId, isPersonalDashboard = model.IsPersonalDashboard });
                }
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

        public ActionResult ApproveHr(int? id, bool isPersonalDashboard)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var csfData = _csfBLL.GetCsfById(id.Value);

            if (csfData == null)
            {
                return HttpNotFound();
            }

            if (CurrentUser.UserRole != Enums.UserRole.HR)
            {
                return RedirectToAction("Detail", "TraCsf", new { id = csfData.TRA_CSF_ID, isPersonalDashboard = isPersonalDashboard });
            }

            try
            {
                var model = new CsfItemModel();
                model.Detail = Mapper.Map<CsfData>(csfData);
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

        public ActionResult ApproveFleet(int? id, bool isPersonalDashboard)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var csfData = _csfBLL.GetCsfById(id.Value);

            if (csfData == null)
            {
                return HttpNotFound();
            }

            if (CurrentUser.UserRole != Enums.UserRole.Fleet)
            {
                return RedirectToAction("Detail", "TraCsf", new { id = csfData.TRA_CSF_ID, isPersonalDashboard = isPersonalDashboard });
            }

            try
            {
                var model = new CsfItemModel();
                model.Detail = Mapper.Map<CsfData>(csfData);
                model = InitialModel(model);
                model.Detail.ExpectedDate = model.Detail.EffectiveDate;
                model.Detail.EndRentDate = model.Detail.EffectiveDate;

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

        public ActionResult ApproveCsf(long TraCsfId, bool IsPersonalDashboard)
        {
            bool isSuccess = false;
            try
            {
                CsfWorkflow(TraCsfId, Enums.ActionType.Approve, null);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            if (!isSuccess) return RedirectToAction("Detail", "TraCsf", new { id = TraCsfId, isPersonalDashboard = IsPersonalDashboard });
            AddMessageInfo("Success Approve Document", Enums.MessageInfoType.Success);
            return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
        }

        public ActionResult ApproveCsfFleet(long TraCsfId, bool IsPersonalDashboard, DateTime expectedDateId, DateTime endRentDateId, string supplyMethodId)
        {
            bool isSuccess = false;
            try
            {
                var csfData = _csfBLL.GetCsfById(TraCsfId);
                csfData.EXPECTED_DATE = expectedDateId;
                csfData.END_RENT_DATE = endRentDateId;
                csfData.SUPPLY_METHOD = supplyMethodId;

                var saveResult = _csfBLL.Save(csfData, CurrentUser);

                CsfWorkflow(TraCsfId, Enums.ActionType.Approve, null);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            if (!isSuccess) return RedirectToAction("Detail", "TraCsf", new { id = TraCsfId, isPersonalDashboard = IsPersonalDashboard });
            AddMessageInfo("Success Approve Document", Enums.MessageInfoType.Success);
            return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
        }

        public ActionResult RejectCsf(int TraCsfIdReject, int RemarkId, bool IsPersonalDashboard)
        {
            bool isSuccess = false;
            try
            {
                CsfWorkflow(TraCsfIdReject, Enums.ActionType.Reject, RemarkId);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("Detail", "TraCsf", new { id = TraCsfIdReject, isPersonalDashboard = IsPersonalDashboard });
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

            var csfData = _csfBLL.GetCsfById(id.Value);

            if (csfData == null)
            {
                return HttpNotFound();
            }

            if (CurrentUser.UserRole != Enums.UserRole.Fleet)
            {
                return RedirectToAction("Detail", "TraCsf", new { id = csfData.TRA_CSF_ID, isPersonalDashboard = isPersonalDashboard });
            }

            try
            {
                var model = new CsfItemModel();
                model.Detail = Mapper.Map<CsfData>(csfData);
                model = InitialModel(model);
                if (model.Detail.ExpectedDate == null) { 
                    model.Detail.ExpectedDate = model.Detail.EffectiveDate;
                    model.Detail.EndRentDate = model.Detail.EffectiveDate;
                }
                model.Temporary.StartPeriod = DateTime.Now;
                model.Temporary.EndPeriod = DateTime.Now;

                var listReason = _reasonBLL.GetReason().Where(x => x.DocumentType == (int)Enums.DocumentType.TMP).Select(x => new { x.MstReasonId, x.Reason }).ToList().OrderBy(x => x.Reason);
                model.Temporary.ReasonTempList = new SelectList(listReason, "MstReasonId", "Reason");

                var RemarkList = _remarkBLL.GetRemark().Where(x => x.RoleType == CurrentUser.UserRole.ToString() && x.DocumentType == (int)Enums.DocumentType.CSF).ToList();
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

                        model.Detail.StartPeriodVendor = model.Detail.EffectiveDate;
                        model.Detail.EndRentDate = cfmData.EndContract;
                        model.Detail.EndPeriodVendor = cfmData.EndContract;
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
        public ActionResult InProgress(CsfItemModel model)
        {
            try
            {
                var csfData = _csfBLL.GetCsfById(model.Detail.TraCsfId);
                csfData.VENDOR_POLICE_NUMBER = model.Detail.PoliceNumberVendor;
                csfData.VENDOR_MANUFACTURER = model.Detail.ManufacturerVendor;
                csfData.VENDOR_MODEL = model.Detail.ModelsVendor;
                csfData.VENDOR_SERIES = model.Detail.SeriesVendor;
                csfData.VENDOR_BODY_TYPE = model.Detail.BodyTypeVendor;
                csfData.VENDOR_VENDOR = model.Detail.VendorNameVendor;
                csfData.VENDOR_COLOUR = model.Detail.ColorVendor;
                csfData.VENDOR_CONTRACT_START_DATE = model.Detail.StartPeriodVendor;
                csfData.VENDOR_CONTRACT_END_DATE = model.Detail.EndPeriodVendor;
                csfData.VENDOR_PO_NUMBER = model.Detail.PoNumberVendor;
                csfData.VENDOR_CHASIS_NUMBER = model.Detail.ChasisNumberVendor;
                csfData.VENDOR_ENGINE_NUMBER = model.Detail.EngineNumberVendor;
                csfData.VENDOR_AIR_BAG = model.Detail.IsAirBagVendor;
                csfData.VENDOR_TRANSMISSION = model.Detail.TransmissionVendor;
                csfData.VENDOR_BRANDING = model.Detail.BrandingVendor;
                csfData.VENDOR_PURPOSE = model.Detail.PurposeVendor;
                csfData.VENDOR_PO_LINE = model.Detail.PoLineVendor;
                csfData.VAT_DECIMAL = model.Detail.VatDecimalVendor;
                csfData.VENDOR_RESTITUTION = model.Detail.IsRestitutionVendor;
                csfData.COMMENTS = model.Detail.CommentsVendor;

                csfData.EXPECTED_DATE = model.Detail.ExpectedDate;
                csfData.END_RENT_DATE = model.Detail.EndRentDate;
                csfData.SUPPLY_METHOD = model.Detail.SupplyMethod;
                csfData.PROJECT_NAME = model.Detail.Project;

                var saveResult = _csfBLL.Save(csfData, CurrentUser);
                //send email to user if police number and contract start date is fill

                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                return RedirectToAction("InProgress", "TraCsf", new { id = csfData.TRA_CSF_ID, isPersonalDashboard = model.IsPersonalDashboard });

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return View();
            }
        }

        #endregion

        #region --------- Add Temporary Car --------------

        public ActionResult AddTemporaryCar(CsfItemModel model)
        {
            bool isSuccess = false;
            try
            {
                TemporaryDto item = new TemporaryDto();

                var csfData = _csfBLL.GetCsfById(model.Temporary.TempTraCsfId);

                if (csfData == null)
                {
                    return HttpNotFound();
                }

                item = AutoMapper.Mapper.Map<TemporaryDto>(csfData);
                item.CREATED_BY = CurrentUser.USER_ID;
                item.CREATED_DATE = DateTime.Now;
                item.DOCUMENT_STATUS = Enums.DocumentStatus.Draft;
                item.START_DATE = model.Temporary.StartPeriod;
                item.END_DATE = model.Temporary.EndPeriod;
                item.REASON_ID = model.Temporary.ReasonIdTemp;

                var tempData = _csfBLL.SaveTemp(item, CurrentUser);

                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("Detail", "TraCsf", new { id = model.Temporary.TempTraCsfId, isPersonalDashboard = model.IsPersonalDashboard });
            AddMessageInfo("Success Add Temporary Data", Enums.MessageInfoType.Success);
            return RedirectToAction("InProgress", "TraCsf", new { id = model.Temporary.TempTraCsfId, isPersonalDashboard = model.IsPersonalDashboard });
        }

        #endregion

        #region --------- Workflow --------------

        private void CsfWorkflow(long id, Enums.ActionType actionType, int? comment)
        {
            var attachmentsList = new List<string>();

            //if fleet approve and send csv to vendor
            if (CurrentUser.UserRole == Enums.UserRole.Fleet && actionType == Enums.ActionType.Approve)
            {
                var docForVendor = CreateExcelForVendor(id);
                attachmentsList.Add(docForVendor);
            }

            var input = new CsfWorkflowDocumentInput
            {
                DocumentId = id,
                UserId = CurrentUser.USER_ID,
                EmployeeId = CurrentUser.EMPLOYEE_ID,
                UserRole = CurrentUser.UserRole,
                ActionType = actionType,
                Comment = comment,
                Attachments = attachmentsList
            };

            _csfBLL.CsfWorkflow(input);
        }

        #endregion

        #region --------- Get Data Post JS --------------

        [HttpPost]
        public JsonResult GetEmployee(string Id)
        {
            var model = _employeeBLL.GetByID(Id);
            return Json(model);
        }

        public JsonResult GetEmployeeList()
        {
            var allEmployee = _employeeBLL.GetEmployee().Select(x => new { x.EMPLOYEE_ID, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);

            if (CurrentUser.UserRole == Enums.UserRole.HR)
            {
                allEmployee = _employeeBLL.GetEmployee().Where(x => x.GROUP_LEVEL > 0).ToList().Select(x => new { x.EMPLOYEE_ID, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);
            }

            return Json(allEmployee, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetVehicleData(string vehUsage, string vehType, string vehCat, string groupLevel, DateTime createdDate, string location)
        {
            var vehicleType = _settingBLL.GetByID(Convert.ToInt32(vehType)).SettingName.ToLower();
            var vehicleData = _vehicleSpectBLL.GetVehicleSpect().Where(x => x.IsActive && x.Year == createdDate.Year).ToList();

            var zonePriceList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive && x.Location == location)
                                                                                        .OrderByDescending(x => x.ValidFrom).FirstOrDefault();

            var zonePriceListByUserCsf = zonePriceList == null ? string.Empty : zonePriceList.ZonePriceList;

            var dataAllPricelist = _priceListBLL.GetPriceList().Where(x => x.IsActive).ToList();

            var allVendor = _vendorBLL.GetVendor().Where(x => x.IsActive).ToList();

            foreach (var item in vehicleData)
            {
                dataAllPricelist = dataAllPricelist.Where(x => x.ZonePriceList != null).ToList();

                //select vendor from pricelist
                var dataVendor = dataAllPricelist.Where(x => x.Manufacture.ToLower() == item.Manufacturer.ToLower()
                                                        && x.Model.ToLower() == item.Models.ToLower()
                                                        && x.Series.ToLower() == item.Series.ToLower()
                                                        && x.Year == createdDate.Year
                                                        && x.ZonePriceList.ToLower() == zonePriceListByUserCsf.ToLower()).FirstOrDefault();

                var vendorId = dataVendor == null ? 0 : dataVendor.Vendor;

                var dataVendorDetail = allVendor.Where(x => x.MstVendorId == vendorId).FirstOrDefault();

                item.VendorName = dataVendor == null ? string.Empty : (dataVendorDetail == null ? string.Empty : dataVendorDetail.ShortName);
            }

            var fleetDto = new List<FleetDto>();

            if (vehicleType == "benefit")
            {
                var modelVehicle = vehicleData.Where(x => x.GroupLevel == Convert.ToInt32(groupLevel)).ToList();
                if (vehCat.ToLower() == "flexi benefit")
                {
                    modelVehicle = vehicleData.Where(x => x.GroupLevel < Convert.ToInt32(groupLevel) && x.GroupLevel > 0).ToList();
                }

                if (vehUsage == "CFM")
                {
                    //get selectedCfmIdle temp
                    var cfmIdleListSelected = _tempBLL.GetList().Where(x => x.DOCUMENT_STATUS != Enums.DocumentStatus.Cancelled && x.DOCUMENT_STATUS != Enums.DocumentStatus.Completed
                                                                            && x.CFM_IDLE_ID != null && x.CFM_IDLE_ID.Value > 0).Select(x => x.CFM_IDLE_ID.Value).ToList();

                    //get selectedCfmIdle csf
                    var cfmIdleListSelectedCsf = _csfBLL.GetList().Where(x => x.DOCUMENT_STATUS != Enums.DocumentStatus.Cancelled && x.DOCUMENT_STATUS != Enums.DocumentStatus.Completed
                                                                            && x.CFM_IDLE_ID != null && x.CFM_IDLE_ID.Value > 0).Select(x => x.CFM_IDLE_ID.Value).ToList();

                    //get selectedCfmIdle crf
                    var cfmIdleListSelectedCrf = _crfBLL.GetList().Where(x => x.DOCUMENT_STATUS != (int)Enums.DocumentStatus.Cancelled 
                                                                            && x.MST_FLEET_ID != null && x.MST_FLEET_ID.Value > 0).Select(x => x.MST_FLEET_ID.Value).ToList();

                    var fleetData = _fleetBLL.GetFleet().Where(x => x.VehicleUsage.ToUpper() == "CFM IDLE" 
                                                                    && x.IsActive
                                                                    && !cfmIdleListSelected.Contains(x.MstFleetId)
                                                                    && !cfmIdleListSelectedCsf.Contains(x.MstFleetId)
                                                                    && !cfmIdleListSelectedCrf.Contains(x.MstFleetId)).ToList();

                    var modelCFMIdle = fleetData.Where(x => x.CarGroupLevel == Convert.ToInt32(groupLevel)).ToList();

                    if (vehCat.ToLower() == "flexi benefit")
                    {
                        modelCFMIdle = fleetData.Where(x => x.CarGroupLevel < Convert.ToInt32(groupLevel)).ToList();
                    }

                    fleetDto = modelCFMIdle;

                    if (modelCFMIdle.Count == 0)
                    {
                        return Json(modelVehicle);
                    }

                    return Json(fleetDto);
                }

                return Json(modelVehicle);
            }
            else
            {
                vehicleData = vehicleData.Where(x => x.GroupLevel == 0).ToList();

                return Json(vehicleData);
            }
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase itemExcelFile, int Detail_TraCsfId)
        {
            var data = (new ExcelReader()).ReadExcel(itemExcelFile);
            var model = new List<TemporaryData>();
            if (data != null)
            {
                var csfData = _csfBLL.GetCsfById(Detail_TraCsfId);
                var cfmData = csfData.CFM_IDLE_ID == null ? null : _fleetBLL.GetFleetById((int)csfData.CFM_IDLE_ID.Value);

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
                    item.Comments = dataRow[25];

                    if (cfmData != null)
                    {
                        item.VendorName = cfmData.VendorName;
                        item.PoliceNumber = cfmData.PoliceNumber;
                        item.ChasisNumber = cfmData.ChasisNumber;
                        item.EngineNumber = cfmData.EngineNumber;
                        item.IsAirBag = cfmData.Airbag;
                        item.Manufacturer = cfmData.Manufacturer;
                        item.Models = cfmData.Models;
                        item.Series = cfmData.Series;
                        item.Transmission = cfmData.Transmission;
                        item.Color = cfmData.Color;
                        item.BodyType = cfmData.BodyType;
                        item.Branding = cfmData.Branding;
                        item.Purpose = cfmData.Purpose;
                        item.VatDecimal = cfmData.VatDecimal == null ? 0 : cfmData.VatDecimal.Value;
                        item.IsRestitution = cfmData.Restitution;
                        item.Comments = cfmData.Comments;
                    }

                    model.Add(item);
                }
            }

            var input = Mapper.Map<List<VehicleFromVendorUpload>>(model);
            var outputResult = _csfBLL.ValidationUploadDocumentProcess(input, Detail_TraCsfId);

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
                    item.IsVat = dataRow[22].ToUpper() == "YES" ? true : false;
                    item.IsRestitution = dataRow[23].ToUpper() == "YES" ? true : false;

                    model.Add(item);
                }
            }

            var input = Mapper.Map<List<VehicleFromVendorUpload>>(model);
            var outputResult = _csfBLL.ValidationUploadDocumentProcessMassUpload(input);

            return Json(outputResult);


        }

        [HttpPost]
        public JsonResult GetAddressByCity(string location)
        {
            var data = _locationMappingBLL.GetLocationMapping();

            data = data.Where(x => x.Basetown == location).ToList();

            return Json(data);
        }

        [HttpPost]
        public JsonResult GetFlexBen(string vehCat, int? carGroupLevel, int? empGroupLevel)
        {
            var flexPoint = 0;

            var data = _vehicleSpectBLL.GetVehicleSpect().Where(x => x.IsActive && x.GroupLevel == empGroupLevel && x.GroupLevel > 0).FirstOrDefault();

            if (data != null)
            {
                if (vehCat.ToLower() == "no car") { 
                    flexPoint = data.FlexPoint;
                }
                else if (vehCat.ToLower() == "flexi benefit")
                {
                    var dataFlex = _vehicleSpectBLL.GetVehicleSpect().Where(x => x.IsActive && x.GroupLevel == carGroupLevel && x.GroupLevel > 0).FirstOrDefault();

                    if (dataFlex != null)
                    {
                        flexPoint = data.FlexPoint - dataFlex.FlexPoint;
                    }
                }
            }

            return Json(flexPoint);
        }

        [HttpPost]
        public JsonResult UploadFileVehicle(HttpPostedFileBase itemExcelFile, int Detail_TraCsfId)
        {
            var data = (new ExcelReader()).ReadExcel(itemExcelFile);
            var model = new List<VehicleData>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0].ToLower() == "manufacturer")
                    {
                        continue;
                    }

                    var item = new VehicleData();

                    item.Manufacturer = dataRow[0];
                    item.Models = dataRow[1];
                    item.Series = dataRow[2];
                    item.BodyType = dataRow[3];
                    item.Color = dataRow[4];
                    item.Vendor = string.Empty;

                    model.Add(item);
                }
            }

            var input = Mapper.Map<List<VehicleFromUserUpload>>(model);
            var outputResult = _csfBLL.ValidationUploadVehicleProcess(input, Detail_TraCsfId);

            return Json(outputResult);


        }

        #endregion

        #region --------- Close EPAF --------------

        public ActionResult CloseEpaf(int EpafId, int RemarkId)
        {
            if (CurrentUser.UserRole != Enums.UserRole.HR)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _epafBLL.DeactivateEpaf(EpafId, RemarkId, CurrentUser.USER_ID);
                    AddMessageInfo("Success Close ePAF", Enums.MessageInfoType.Success);
                }
                catch (Exception)
                {

                }

            }
            return RedirectToAction("Dashboard", "TraCsf");
        }

        #endregion

        #region --------- Cancel Document CSF --------------

        public ActionResult CancelCsf(int TraCsfId, int RemarkId, bool IsPersonalDashboard)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _csfBLL.CancelCsf(TraCsfId, RemarkId, CurrentUser.USER_ID);
                    CsfWorkflow(TraCsfId, Enums.ActionType.Cancel, null);
                    AddMessageInfo("Success Cancelled Document", Enums.MessageInfoType.Success);
                }
                catch (Exception)
                {

                }

            }
            return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
        }

        #endregion

        #region --------- Assign EPAF --------------

        public ActionResult AssignEpaf(int MstEpafId)
        {
            if (CurrentUser.UserRole != Enums.UserRole.HR)
            {
                return RedirectToAction("Index");
            }

            try
            {
                var epafData = _epafBLL.GetEpaf().Where(x => x.MstEpafId == MstEpafId).FirstOrDefault();

                if (epafData != null)
                {
                    //check if csf created first
                    var vehTypeBenefit = _settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_TYPE" && x.SettingName == "BENEFIT").FirstOrDefault().MstSettingId;
                    var allCsfData = _csfBLL.GetList().Where(x => x.VEHICLE_TYPE == vehTypeBenefit.ToString());
                    var existsCsf = allCsfData.Where(x => x.EMPLOYEE_ID == epafData.EmployeeId
                                                            && x.EFFECTIVE_DATE.Day == epafData.EfectiveDate.Value.Day
                                                            && x.EFFECTIVE_DATE.Month == epafData.EfectiveDate.Value.Month
                                                            && x.EFFECTIVE_DATE.Year == epafData.EfectiveDate.Value.Year).FirstOrDefault();

                    if (existsCsf != null)
                    {
                        //update epaf id
                        existsCsf.EPAF_ID = epafData.MstEpafId;

                        var csfDataExists = _csfBLL.Save(existsCsf, CurrentUser);
                        AddMessageInfo("Success Update Epaf Data", Enums.MessageInfoType.Success);
                        return RedirectToAction("Dashboard", "TraCsf");
                    }

                    TraCsfDto item = new TraCsfDto();

                    item = AutoMapper.Mapper.Map<TraCsfDto>(epafData);

                    var reason = _reasonBLL.GetReason().Where(x => x.DocumentType == (int)Enums.DocumentType.CSF && x.Reason.ToLower() == epafData.EpafAction.ToLower()).FirstOrDefault();

                    if (reason == null)
                    {
                        AddMessageInfo("Please Add Reason In Master Data", Enums.MessageInfoType.Warning);
                        return RedirectToAction("Dashboard", "TraCsf");
                    }

                    item.REASON_ID = reason.MstReasonId;
                    item.CREATED_BY = CurrentUser.USER_ID;
                    item.CREATED_DATE = DateTime.Now;
                    item.DOCUMENT_STATUS = Enums.DocumentStatus.Draft;
                    item.IS_ACTIVE = true;
                    item.VEHICLE_TYPE = vehTypeBenefit.ToString();
                    item.BODY_TYPE = string.Empty;
                    item.VENDOR_BODY_TYPE = string.Empty;

                    var csfData = _csfBLL.Save(item, CurrentUser);

                    CsfWorkflow(csfData.TRA_CSF_ID, Enums.ActionType.Created, null);
                    CsfWorkflow(csfData.TRA_CSF_ID, Enums.ActionType.Submit, null);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("Dashboard", "TraCsf");
                }
            }
            catch (Exception)
            {

            }

            return RedirectToAction("Dashboard", "TraCsf");
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
                foreach (var data in model.TemporaryList) {

                    var csfData = _csfBLL.GetList().Where(x => x.DOCUMENT_NUMBER.ToLower() == data.CsfNumber.ToLower()).FirstOrDefault();

                    if (csfData != null)
                    {
                        csfData.VENDOR_POLICE_NUMBER = data.PoliceNumber;
                        csfData.VENDOR_MANUFACTURER = data.Manufacturer;
                        csfData.VENDOR_MODEL = data.Models;
                        csfData.VENDOR_SERIES = data.Series;
                        csfData.VENDOR_BODY_TYPE = data.BodyType;
                        csfData.VENDOR_VENDOR = data.VendorName;
                        csfData.VENDOR_COLOUR = data.Color;
                        csfData.VENDOR_CONTRACT_START_DATE = data.StartPeriod;
                        csfData.VENDOR_CONTRACT_END_DATE = data.EndPeriod;
                        csfData.VENDOR_PO_NUMBER = data.PoNumber;
                        csfData.VENDOR_CHASIS_NUMBER = data.ChasisNumber;
                        csfData.VENDOR_ENGINE_NUMBER = data.EngineNumber;
                        csfData.VENDOR_AIR_BAG = data.IsAirBag;
                        csfData.VENDOR_TRANSMISSION = data.Transmission;
                        csfData.VENDOR_BRANDING = data.Branding;
                        csfData.VENDOR_PURPOSE = data.Purpose;
                        csfData.VENDOR_PO_LINE = data.PoLine;
                        csfData.VAT_DECIMAL = data.VatDecimal;
                        csfData.VENDOR_RESTITUTION = data.IsRestitution;
                        csfData.COMMENTS = data.Comments;

                        var saveResult = _csfBLL.Save(csfData, CurrentUser);
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

        private string CreateExcelForVendor(long id)
        {
            //get data
            var csfData = _csfBLL.GetCsfById(id);

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
            slDocument = CreateDataExcelForVendor(slDocument, csfData);

            var fileName = "ExcelForVendor_CSF_" + csfData.TRA_CSF_ID + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
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
            slDocument.SetCellValue(iRow, 20, "Vehicle Year");
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

        private SLDocument CreateDataExcelForVendor(SLDocument slDocument, TraCsfDto csfData)
        {
            int iRow = 4; //starting row data

            slDocument.SetCellValue(iRow, 2, csfData.DOCUMENT_NUMBER);
            slDocument.SetCellValue(iRow, 3, csfData.EMPLOYEE_NAME);
            slDocument.SetCellValue(iRow, 4, csfData.VENDOR_NAME);
            slDocument.SetCellValue(iRow, 5, string.Empty);
            slDocument.SetCellValue(iRow, 6, string.Empty);
            slDocument.SetCellValue(iRow, 7, string.Empty);
            slDocument.SetCellValue(iRow, 8, csfData.EFFECTIVE_DATE.ToOADate());
            slDocument.SetCellValue(iRow, 9, csfData.EFFECTIVE_DATE.ToOADate());
            slDocument.SetCellValue(iRow, 10, "YES");
            slDocument.SetCellValue(iRow, 11, csfData.MANUFACTURER);
            slDocument.SetCellValue(iRow, 12, csfData.MODEL);
            slDocument.SetCellValue(iRow, 13, csfData.SERIES);
            slDocument.SetCellValue(iRow, 14, string.Empty);
            slDocument.SetCellValue(iRow, 15, csfData.COLOUR);
            slDocument.SetCellValue(iRow, 16, csfData.BODY_TYPE);
            slDocument.SetCellValue(iRow, 17, csfData.LOCATION_CITY);
            slDocument.SetCellValue(iRow, 18, string.Empty);
            slDocument.SetCellValue(iRow, 19, string.Empty);
            slDocument.SetCellValue(iRow, 20, csfData.CREATED_DATE.Year.ToString());
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
            List<EpafDto> epaf = _epafBLL.GetEpafByDocType(Enums.DocumentType.CSF);
            var listData = Mapper.Map<List<EpafData>>(epaf);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Dashboard CSF");
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

            var fileName = "Dashboard_CSF" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelDashboard(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "ePAF Effective Date");
            slDocument.SetCellValue(iRow, 2, "eLetter sent(s)");
            slDocument.SetCellValue(iRow, 3, "Action");
            slDocument.SetCellValue(iRow, 4, "Employee ID");
            slDocument.SetCellValue(iRow, 5, "Employee Name");
            slDocument.SetCellValue(iRow, 6, "Cost Centre");
            slDocument.SetCellValue(iRow, 7, "Group Level");
            slDocument.SetCellValue(iRow, 8, "Updated By");
            slDocument.SetCellValue(iRow, 9, "Updated Date");

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

        private SLDocument CreateDataExcelDashboard(SLDocument slDocument, List<EpafData> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.EpafEffectiveDate.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 2, data.LetterSend ? "Yes" : "No");
                slDocument.SetCellValue(iRow, 3, data.Action);
                slDocument.SetCellValue(iRow, 4, data.EmployeeId);
                slDocument.SetCellValue(iRow, 5, data.EmployeeName);
                slDocument.SetCellValue(iRow, 6, data.CostCentre);
                slDocument.SetCellValue(iRow, 7, data.GroupLevel);
                slDocument.SetCellValue(iRow, 8, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 9, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy HH:mm:ss"));

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

        public void ExportPersonal()
        {
            string pathFile = "";

            pathFile = CreateXlsCsfPersonal();

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

        private string CreateXlsCsfPersonal()
        {
            //get data
            List<TraCsfDto> csf = _csfBLL.GetCsfPersonal(CurrentUser);
            var listData = Mapper.Map<List<CsfData>>(csf);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Personal Dashboard CSF");
            slDocument.MergeWorksheetCells(1, 1, 1, 11);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelCsf(slDocument);

            //create data
            slDocument = CreateDataExcelCsf(slDocument, listData);

            var fileName = "Data_CSF" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        public void ExportOpen()
        {
            string pathFile = "";

            pathFile = CreateXlsCsf(false);

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

            pathFile = CreateXlsCsf(true);

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

        private string CreateXlsCsf(bool isCompleted)
        {
            //get data
            List<TraCsfDto> csf = _csfBLL.GetCsf(CurrentUser, isCompleted);
            var listData = Mapper.Map<List<CsfData>>(csf);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, isCompleted ? "Completed Document CSF" : "Open Document CSF");
            slDocument.MergeWorksheetCells(1, 1, 1, 11);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelCsf(slDocument);

            //create data
            slDocument = CreateDataExcelCsf(slDocument, listData);

            var fileName = "Data_CSF" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelCsf(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "CSF Number");
            slDocument.SetCellValue(iRow, 2, "CSF Status");
            slDocument.SetCellValue(iRow, 3, "Vehicle Type");
            slDocument.SetCellValue(iRow, 4, "Employee ID");
            slDocument.SetCellValue(iRow, 5, "Employee Name");
            slDocument.SetCellValue(iRow, 6, "Reason");
            slDocument.SetCellValue(iRow, 7, "Effective Date");
            slDocument.SetCellValue(iRow, 8, "Regional");
            slDocument.SetCellValue(iRow, 9, "Coordinator");
            slDocument.SetCellValue(iRow, 10, "Updated By");
            slDocument.SetCellValue(iRow, 11, "Updated Date");

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

        private SLDocument CreateDataExcelCsf(SLDocument slDocument, List<CsfData> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.CsfNumber);
                slDocument.SetCellValue(iRow, 2, data.CsfStatusName);
                slDocument.SetCellValue(iRow, 3, data.VehicleTypeName);
                slDocument.SetCellValue(iRow, 4, data.EmployeeId);
                slDocument.SetCellValue(iRow, 5, data.EmployeeName);
                slDocument.SetCellValue(iRow, 6, data.Reason);
                slDocument.SetCellValue(iRow, 7, data.EffectiveDate.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 8, data.Regional);
                slDocument.SetCellValue(iRow, 9, data.CreateBy);
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
