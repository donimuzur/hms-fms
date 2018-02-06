using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Core.Exceptions;
using FMS.Utils;
using FMS.Website.Models;
using FMS.Website.Utility;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Data;
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
        private IVendorBLL _vendorBLL;

        public TraCtfController(IPageBLL pageBll, IEpafBLL epafBll, ITraCtfBLL ctfBll, ITraCsfBLL CsfBll, IRemarkBLL RemarkBLL, ISettingBLL SettingBLL, ICtfExtendBLL CtfExtendBLL,
                                IEmployeeBLL EmployeeBLL, IReasonBLL ReasonBLL, IFleetBLL FleetBLL, ILocationMappingBLL LocationMappingBLL, IVendorBLL VendorBLL) : base(pageBll, Core.Enums.MenuList.TraCtf)
        {

            _epafBLL = epafBll;
            _ctfBLL = ctfBll;
            _csfBLL = CsfBll;
            _employeeBLL = EmployeeBLL;
            _pageBLL = pageBll;
            _vendorBLL = VendorBLL;
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
            _ctfBLL.CheckCtfInProgress();

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
                var region = _locationMappingBLL.GetLocationMapping().Where(x => x.Location == item.VehicleLocation).FirstOrDefault();
                if (region != null)
                {
                    item.Region = region.Region;
                }

            }
            model.TitleForm = "CTF Open Document";
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.IsPersonalDashboard = false;
            model = TerminationBoard(model);
            model.Details = model.Details.OrderBy(x => x.DocumentNumber).ToList();
            return View(model);
        }
        public CtfModel TerminationBoard(CtfModel model)
        {
            var settingData = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
            var benefitType = settingData.Where(x => x.SettingName.ToUpper() == "BENEFIT").FirstOrDefault().SettingName;
            var wtcType = settingData.Where(x => x.SettingName.ToUpper() == "WTC").FirstOrDefault().SettingName;

            var VehUsage = _settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_USAGE_BENEFIT");
            var CfmUsage = VehUsage.Where(x => x.SettingName.ToUpper() == "CFM").FirstOrDefault().SettingName;

            var fleetBenefit = _fleetBLL.GetFleetForEndContractLessThan(60).Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == benefitType.ToUpper() && x.IsActive == true).ToList();
            var fleetWTC = _fleetBLL.GetFleetForEndContractLessThan(90).Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == wtcType.ToUpper() && x.IsActive == true).ToList();

            bool IsSendCsf = false;
            bool IsTerminate = false;
            var GetSetting = _settingBLL.GetSetting();
            if (CurrentUser.UserRole == Enums.UserRole.HR)
            {
                IsSendCsf = false;
                IsTerminate = false;
                if (fleetBenefit != null)
                {
                    foreach (var item in fleetBenefit)
                    {
                        try
                        {
                            var ctfitem = Mapper.Map<CtfItem>(item);
                            var ReasonID = _reasonBLL.GetReason().Where(x => x.Reason.ToLower() == "end rent").FirstOrDefault().MstReasonId;

                            var VehType = GetSetting.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.SettingName.ToUpper() == (item.VehicleType == null ? "" : item.VehicleType.ToUpper()) && x.IsActive).FirstOrDefault();
                            var csfdata = _csfBLL.GetList().Where(x => x.EMPLOYEE_ID == ctfitem.EmployeeId && (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == (VehType == null ? "" : VehType.MstSettingId.ToString()) && x.DOCUMENT_STATUS != Enums.DocumentStatus.Completed && x.DOCUMENT_STATUS != Enums.DocumentStatus.Cancelled).ToList();

                            var days7 = DateTime.Now.AddDays(7);
                            ctfitem.Reason = ReasonID;
                            ctfitem.ReasonS = "End Rent";
                            ctfitem.lessthan2month = true;
                            ctfitem.CreatedBy = "SYSTEM";

                            if (csfdata.Count() > 0)
                            {
                                ctfitem.lessthan2month = false;
                                IsSendCsf = true;
                            }

                            if (!IsSendCsf)
                            {
                                model.Details.Add(ctfitem);
                            }
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
                fleetBenefit = _fleetBLL.GetFleetForEndContractLessThan(7).Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == benefitType.ToUpper() && (x.VehicleUsage == null ? "" : x.VehicleUsage.ToUpper()) == CfmUsage.ToUpper() && x.IsActive == true).ToList();

                IsSendCsf = false;
                IsTerminate = false;
                if (fleetBenefit != null)
                {
                    foreach (var item in fleetBenefit)
                    {
                        try
                        {
                            var ctfitem = Mapper.Map<CtfItem>(item);
                            var ReasonID = _reasonBLL.GetReason().Where(x => x.Reason.ToLower() == "end rent").FirstOrDefault().MstReasonId;

                            var ctfdata = _ctfBLL.GetCtf().Where(x => x.EmployeeId == ctfitem.EmployeeId && x.PoliceNumber == ctfitem.PoliceNumber && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (ctfitem.VehicleType == null ? "" : ctfitem.VehicleType.ToUpper()) && x.DocumentStatus != Enums.DocumentStatus.Completed && x.DocumentStatus != Enums.DocumentStatus.Cancelled).ToList();

                            var days7 = DateTime.Now.AddDays(7);

                            ctfitem.Reason = ReasonID;
                            ctfitem.ReasonS = "End Rent";
                            ctfitem.lessthan2month = false;
                            ctfitem.lessthan7day = true;
                            ctfitem.CreatedBy = "SYSTEM";

                            if (ctfdata.Count() > 0)
                            {
                                ctfitem.lessthan7day = false;
                                IsTerminate = true;
                            }

                            if (!IsTerminate)
                            {
                                model.Details.Add(ctfitem);
                            }

                        }
                        catch (Exception exp)
                        {
                            AddMessageInfo(exp.Message, Enums.MessageInfoType.Error);
                        }
                    }
                }
                if (fleetWTC != null)
                {
                    IsSendCsf = false;
                    IsTerminate = false;
                    foreach (var item in fleetWTC)
                    {
                        try
                        {
                            var ctfitem = Mapper.Map<CtfItem>(item);
                            var ReasonID = _reasonBLL.GetReason().Where(x => x.Reason.ToLower() == "end rent").FirstOrDefault().MstReasonId;
                            var days7 = DateTime.Now.AddDays(7);

                            var ctfdata = _ctfBLL.GetCtf().Where(x => x.EmployeeId == ctfitem.EmployeeId && x.PoliceNumber == ctfitem.PoliceNumber && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (ctfitem.VehicleType == null ? "" : ctfitem.VehicleType.ToUpper()) && x.DocumentStatus != Enums.DocumentStatus.Completed && x.DocumentStatus != Enums.DocumentStatus.Cancelled).ToList();

                            var VehType = GetSetting.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.SettingName.ToUpper() == (item.VehicleType == null ? "" : item.VehicleType.ToUpper()) && x.IsActive).FirstOrDefault();

                            ctfitem.Reason = ReasonID;
                            ctfitem.ReasonS = "End Rent";
                            ctfitem.lessthan2month = true;
                            ctfitem.CreatedBy = "SYSTEM";
                            ctfitem.lessthan7day = ctfitem.EndRendDate <= days7 ? true : false;

                            if (ctfdata.Count() > 0)
                            {
                                ctfitem.lessthan7day = false;
                                IsTerminate = true;
                            }

                            if (!IsTerminate)
                            {
                                model.Details.Add(ctfitem);
                            }
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
        public ActionResult SendCSF(long MstFleetId)
        {
            var FleetData = _fleetBLL.GetFleetById((int)MstFleetId);
            int ReasonId;
            TraCsfDto item = new TraCsfDto();
            var Reason = _reasonBLL.GetReason().Where(x => x.Reason.ToLower() == "end rent" && x.DocumentType == (int)Enums.DocumentType.CSF && x.IsActive
                                                    && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (FleetData.VehicleType == null ? "" : FleetData.VehicleType.ToUpper())).FirstOrDefault();
            if (Reason == null)
            {
                ReasonId = _reasonBLL.GetReason().Where(x => x.DocumentType == (int)Enums.DocumentType.CSF && x.IsActive).FirstOrDefault().MstReasonId;
            }
            else
            {
                ReasonId = Reason.MstReasonId;
            }

            var VehType = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.SettingName.ToUpper() == (FleetData.VehicleType == null ? "" : FleetData.VehicleType.ToUpper())).FirstOrDefault();

            item.CREATED_DATE = DateTime.Today;
            item.CREATED_BY = CurrentUser.USER_ID;
            item.REASON_ID = ReasonId;
            item.EFFECTIVE_DATE = DateTime.Today;
            item.EMPLOYEE_ID = FleetData.EmployeeID;
            item.EMPLOYEE_NAME = FleetData.EmployeeName;
            item.COST_CENTER = FleetData.CostCenter;
            item.DOCUMENT_STATUS = Enums.DocumentStatus.Draft;
            item.VEHICLE_TYPE = VehType.MstSettingId.ToString();
            item.IS_ACTIVE = true;

            var csfData = _csfBLL.Save(item, CurrentUser);
            var input = new CsfWorkflowDocumentInput
            {
                DocumentId = csfData.TRA_CSF_ID,
                UserId = CurrentUser.USER_ID,
                EmployeeId = CurrentUser.EMPLOYEE_ID,
                UserRole = CurrentUser.UserRole,
                ActionType = Enums.ActionType.Created,
                Comment = null
            };
            AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
            _csfBLL.CsfWorkflow(input);
            return RedirectToAction("Edit", "TraCsf", new { id = csfData.TRA_CSF_ID, isPersonalDashboard = false });
        }
        public ActionResult Terminate(long MstFleetId)
        {
            var FleetData = _fleetBLL.GetFleetById((int)MstFleetId);
            var Employee = _employeeBLL.GetEmployee().Where(x => x.EMPLOYEE_ID == FleetData.EmployeeID && x.IS_ACTIVE).FirstOrDefault();
            int ReasonId;
            var Reason = _reasonBLL.GetReason().Where(x => x.Reason.ToLower() == "end rent" && x.DocumentType == (int)Enums.DocumentType.CTF && x.IsActive
                                                    && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (FleetData.VehicleType == null ? "" : FleetData.VehicleType.ToUpper())).FirstOrDefault();
            TraCtfDto item = new TraCtfDto();
            if (Reason == null)
            {
                ReasonId = _reasonBLL.GetReason().Where(x => x.DocumentType == (int)Enums.DocumentType.CTF && x.IsActive).FirstOrDefault().MstReasonId;
            }
            else
            {
                ReasonId = Reason.MstReasonId;
            }

            var settingData = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
            var benefitType = settingData.Where(x => x.SettingName.ToUpper() == "BENEFIT").FirstOrDefault().SettingName;

            var IsBenefit = FleetData.VehicleType == benefitType;

            item.CreatedDate = DateTime.Now;
            item.CreatedBy = CurrentUser.USER_ID;
            item.EmployeeIdCreator = CurrentUser.EMPLOYEE_ID;
            item.DocumentStatus = Enums.DocumentStatus.Draft;

            item.Reason = ReasonId;
            item.EffectiveDate = DateTime.Today;
            item.EmployeeId = Employee.EMPLOYEE_ID;
            item.EmployeeName = Employee.FORMAL_NAME;
            item.CostCenter = Employee.COST_CENTER;
            item.GroupLevel = Employee.GROUP_LEVEL;
            item.PoliceNumber = FleetData.PoliceNumber;
            item.VehicleYear = FleetData.VehicleYear;
            item.VehicleType = FleetData.VehicleType;
            item.VehicleUsage = FleetData.VehicleUsage;
            item.SupplyMethod = FleetData.SupplyMethod;
            item.EndRendDate = FleetData.EndContract;
            item.EffectiveDate = FleetData.EndContract;
            item.IsActive = true;

            var CtfData = _ctfBLL.Save(item, CurrentUser);

            AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
            CtfWorkflow(CtfData.TraCtfId, Enums.ActionType.Created, null, false, IsBenefit, CtfData.DocumentNumber);

            return RedirectToAction("Edit", "TraCtf", new { TraCtfId = CtfData.TraCtfId, IsPersonalDashboard = false });
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
            var ReasonList = _reasonBLL.GetReason().Where(x => x.IsActive == true && x.DocumentType == 6).OrderBy(x => x.Reason).ToList();
            var ReasonExtendList = _reasonBLL.GetReason().Where(x => x.IsActive == true && x.DocumentType == 8).OrderBy(x => x.Reason).ToList();
            if (!string.IsNullOrEmpty(model.VehicleType))
            {
                ReasonList = ReasonList.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == model.VehicleType.ToUpper()).ToList();
            }

            var VehicleLocationList = _locationMappingBLL.GetLocationMapping().Select(x => new { City = x.Location }).Distinct().OrderBy(x => x.City);
            var UserDecisionList = new Dictionary<int, string> { { 1, "Buy" }, { 2, "Refund" } };
            var PoliceNumberList = new List<FleetDto>();
            if (model.EmployeeId != null)
            {
                PoliceNumberList = _fleetBLL.GetFleet().Where(x => x.IsActive == true && x.EmployeeID == model.EmployeeId).OrderBy(x => x.PoliceNumber).ToList();
            }
            else
            {
                PoliceNumberList = _fleetBLL.GetFleet().Where(x => x.IsActive == true).OrderBy(x => x.PoliceNumber).ToList();
            }

            var ExtendList = new Dictionary<bool, string> { { false, "No" }, { true, "Yes" } };

            var RemarkList = _remarkBLL.GetRemark().Where(x => (x.RoleType == null ? "" : x.RoleType.ToUpper()) == CurrentUser.UserRole.ToString().ToUpper() && x.DocumentType == (int)Enums.DocumentType.CTF && x.IsActive).ToList();

            model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");
            model.ExtendList = new SelectList(ExtendList, "Key", "Value");
            model.ReasonExtendList = new SelectList(ReasonExtendList, "MstReasonId", "Reason");
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
                
                Model.EmployeeId = Model.EmployeeId.Split('-')[0].Trim();
                Model.CreatedBy = CurrentUser.USER_ID;
                Model.EmployeeIdCreator = CurrentUser.EMPLOYEE_ID;
                Model.CreatedDate = DateTime.Now;
                Model.DocumentStatus = Enums.DocumentStatus.Draft;
                Model.ErrorMessage = "";
                Model.ExtendVehicle = false;
                if (Model.BuyCostTotalStr != null)
                {
                    Model.BuyCostTotal = Convert.ToDecimal(Model.BuyCostTotalStr.Replace(",", ""));
                }
                if (Model.BuyCostStr != null)
                {
                    Model.BuyCost = Convert.ToDecimal(Model.BuyCostStr.Replace(",", ""));
                }
                if (Model.EmployeeContributionStr != null)
                {
                    Model.EmployeeContribution = Convert.ToDecimal(Model.EmployeeContributionStr.Replace(",", ""));
                }
                if (Model.PenaltyPriceStr != null)
                {
                    Model.PenaltyPrice = Convert.ToDecimal(Model.PenaltyPriceStr.Replace(",", ""));
                }
                if (Model.RefundCostStr != null)
                {
                    Model.RefundCost = Convert.ToDecimal(Model.RefundCostStr.Replace(",", ""));
                }
                if (Model.PenaltyStr != null)
                {
                    Model.Penalty = Convert.ToDecimal(Model.PenaltyStr.Replace(",", ""));
                }
                Model.IsActive = true;
                var settingData = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
                var benefitType = settingData.Where(x => x.SettingName.ToUpper() == "BENEFIT").FirstOrDefault().SettingName;
                var wtcType = settingData.Where(x => x.SettingName.ToUpper() == "WTC").FirstOrDefault().SettingName;

                settingData = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleUsageBenefit));
                var CopUsage = settingData.Where(x => x.SettingName.ToUpper() == "COP").FirstOrDefault().SettingName;

                var ReasonData = _reasonBLL.GetReasonById(Model.Reason.Value);

                var reasonStr = ReasonData.Reason;
                var IsPenalty = ReasonData.IsPenalty;
                var PenaltyForFleet = ReasonData.PenaltyForFleet;
                var PenaltyForEmployee = ReasonData.PenaltyForEmplloyee;

                var IsBenefit = Model.VehicleType == benefitType;
                var IsEndRent = reasonStr.ToLower() == "end rent";

                //only check for benefit
                var CtfDto = Mapper.Map<TraCtfDto>(Model);
                var checkExistCtf = _ctfBLL.CheckCtfExists(CtfDto);
                if (checkExistCtf)
                {
                    Model = initCreate(Model);
                    Model.CurrentLogin = CurrentUser;
                    Model.TitleForm = "Car Termination Form";
                    Model.ErrorMessage = "Data already exists";
                    return View(Model);
                }
                if (!Model.IsTransferToIdle)
                {
                    if (IsPenalty)
                    {
                        if (PenaltyForEmployee == true) CtfDto.Penalty = _ctfBLL.PenaltyCost(CtfDto);
                        if (PenaltyForFleet == true) CtfDto.PenaltyPrice = _ctfBLL.PenaltyCost(CtfDto);
                    }
                    if ((Model.VehicleUsage == null ? "" : Model.VehicleUsage.ToUpper()) == (CopUsage == null ? "" : CopUsage.ToUpper()))
                    {
                        CtfDto.RefundCost = _ctfBLL.RefundCost(CtfDto);
                        CtfDto.BuyCostTotal = CtfDto.BuyCostTotal;
                        CtfDto.BuyCost = CtfDto.BuyCostTotal;
                        CtfDto.EmployeeContribution = _ctfBLL.EmployeeContribution(CtfDto);
                    }
                }
                var CtfData = _ctfBLL.Save(CtfDto, CurrentUser);
                if (Model.isSubmit == "submit")
                {
                    if (!IsBenefit && IsEndRent)
                    {
                        CtfWorkflow(CtfData.TraCtfId, Enums.ActionType.Approve, null, true, false, Model.DocumentNumber);
                        AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                        return RedirectToAction("Edit", "TraCtf", new { TraCtfId = CtfData.TraCtfId, IsPersonalDashboard = false });
                    }

                    CtfWorkflow(CtfData.TraCtfId, Enums.ActionType.Submit, null, false, IsBenefit, Model.DocumentNumber);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("Edit", "TraCtf", new { TraCtfId = CtfData.TraCtfId, IsPersonalDashboard = false });
                }
                AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
                CtfWorkflow(CtfData.TraCtfId, Enums.ActionType.Created, null, false, IsBenefit, Model.DocumentNumber);

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
        public ActionResult DetailsBenefit(int? TraCtfId, bool IsPersonalDashboard)
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

                if (model.ExtendVehicle)
                {
                    var ctfExtend = _ctfExtendBLL.GetCtfExtend().Where(x => x.TraCtfId == model.TraCtfId).FirstOrDefault();
                    model.CtfExtend = ctfExtend;
                    if (model.CtfExtend != null)
                    {
                        model.CtfExtend.ExtendPriceStr = model.CtfExtend == null ? "" : string.Format("{0:n0}", model.CtfExtend.ExtendPrice);
                    }
                }

                model.IsPersonalDashboard = IsPersonalDashboard;
                model = initCreate(model);
                model.CurrentLogin = CurrentUser;

                model.BuyCostTotalStr = model.BuyCostTotal == null ? "" : string.Format("{0:n0}", model.BuyCostTotal);
                model.BuyCostStr = model.BuyCost == null ? "" : string.Format("{0:n0}", model.BuyCost);
                model.EmployeeContributionStr = model.EmployeeContribution == null ? "" : string.Format("{0:n0}", model.EmployeeContribution);
                model.PenaltyPriceStr = model.PenaltyPrice == null ? "" : string.Format("{0:n0}", model.PenaltyPrice);
                model.PenaltyStr = model.Penalty == null ? "" : string.Format("{0:n0}", model.Penalty);
                model.RefundCostStr = model.RefundCost == null ? "" : string.Format("{0:n0}", model.RefundCost);

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

                if (model.ExtendVehicle)
                {
                    var ctfExtend = _ctfExtendBLL.GetCtfExtend().Where(x => x.TraCtfId == model.TraCtfId).FirstOrDefault();
                    model.CtfExtend = ctfExtend;
                    if (model.CtfExtend != null)
                    {
                        model.CtfExtend.ExtendPriceStr = model.CtfExtend == null ? "" : string.Format("{0:n0}", model.CtfExtend.ExtendPrice);
                    }
                }

                model.IsPersonalDashboard = IsPersonalDashboard;
                model = initCreate(model);
                model.CurrentLogin = CurrentUser;


                model.BuyCostTotalStr = model.BuyCostTotal == null ? "" : string.Format("{0:n0}", model.BuyCostTotal);
                model.BuyCostStr = model.BuyCost == null ? "" : string.Format("{0:n0}", model.BuyCost);
                model.EmployeeContributionStr = model.EmployeeContribution == null ? "" : string.Format("{0:n0}", model.EmployeeContribution);
                model.PenaltyPriceStr = model.PenaltyPrice == null ? "" : string.Format("{0:n0}", model.PenaltyPrice);
                model.PenaltyStr = model.Penalty == null ? "" : string.Format("{0:n0}", model.Penalty);
                model.RefundCostStr = model.RefundCost == null ? "" : string.Format("{0:n0}", model.RefundCost);

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
            if (ctfData.DocumentStatus == Enums.DocumentStatus.Completed || ctfData.DocumentStatus == Enums.DocumentStatus.Extended)
            {
                return RedirectToAction("DetailsBenefit", "TraCtf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard = IsPersonalDashboard });
            }
            if ((CurrentUser.EMPLOYEE_ID == ctfData.EmployeeId || ctfData.EmployeeIdCreator == CurrentUser.EMPLOYEE_ID) && ctfData.DocumentStatus == Enums.DocumentStatus.AssignedForUser)
            {
                return RedirectToAction("EditForEmployeeBenefit", "TraCTf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard = IsPersonalDashboard });
            }
            if (CurrentUser.EMPLOYEE_ID == ctfData.EmployeeId && ctfData.DocumentStatus != Enums.DocumentStatus.AssignedForUser)
            {
                return RedirectToAction("DetailsBenefit", "TraCTf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard = IsPersonalDashboard });
            }
            if (ctfData.EmployeeIdFleetApproval == CurrentUser.EMPLOYEE_ID && ctfData.DocumentStatus == Enums.DocumentStatus.InProgress)
            {
                return RedirectToAction("InProgressBenefit", "TraCTf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard = IsPersonalDashboard });
            }
            if (CurrentUser.UserRole == Enums.UserRole.Fleet && ctfData.DocumentStatus == Enums.DocumentStatus.WaitingFleetApproval)
            {
                return RedirectToAction("ApprovalFleetBenefit", "TraCtf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard = IsPersonalDashboard });
            }
            if (CurrentUser.UserRole == Enums.UserRole.HR && ctfData.DocumentStatus == Enums.DocumentStatus.WaitingHRApproval)
            {
                return RedirectToAction("ApprovalHR", "TraCtf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard = IsPersonalDashboard });
            }
            if ((CurrentUser.USER_ID != ctfData.CreatedBy && ctfData.DocumentStatus == Enums.DocumentStatus.AssignedForUser) || (CurrentUser.USER_ID != ctfData.CreatedBy && ctfData.DocumentStatus == Enums.DocumentStatus.Draft || (CurrentUser.USER_ID != ctfData.CreatedBy && ctfData.DocumentStatus == Enums.DocumentStatus.WaitingFleetApproval) || (CurrentUser.EMPLOYEE_ID != ctfData.EmployeeIdCreator && ctfData.DocumentStatus == Enums.DocumentStatus.WaitingHRApproval) || (CurrentUser.EMPLOYEE_ID != ctfData.EmployeeIdCreator && ctfData.DocumentStatus == Enums.DocumentStatus.InProgress)))
            {
                return RedirectToAction("DetailsBenefit", "TraCtf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard = IsPersonalDashboard });
            }
            try
            {
                var model = new CtfItem();
                model = Mapper.Map<CtfItem>(ctfData);

                if (model.ExtendVehicle)
                {
                    var ctfExtend = _ctfExtendBLL.GetCtfExtend().Where(x => x.TraCtfId == model.TraCtfId).FirstOrDefault();
                    model.CtfExtend = ctfExtend;
                    if (model.CtfExtend != null)
                    {
                        model.CtfExtend.ExtendPriceStr = model.CtfExtend == null ? "" : string.Format("{0:n0}", model.CtfExtend.ExtendPrice);
                    }
                }

                model.IsPersonalDashboard = IsPersonalDashboard;
                model = initCreate(model);
                model.CurrentLogin = CurrentUser;

                model.BuyCostTotalStr = model.BuyCostTotal == null ? "" : string.Format("{0:n0}", model.BuyCostTotal);
                model.BuyCostStr = model.BuyCost == null ? "" : string.Format("{0:n0}", model.BuyCost);
                model.EmployeeContributionStr = model.EmployeeContribution == null ? "" : string.Format("{0:n0}", model.EmployeeContribution);
                model.PenaltyPriceStr = model.PenaltyPrice == null ? "" : string.Format("{0:n0}", model.PenaltyPrice);
                model.PenaltyStr = model.Penalty == null ? "" : string.Format("{0:n0}", model.Penalty);
                model.RefundCostStr = model.RefundCost == null ? "" : string.Format("{0:n0}", model.RefundCost);


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
                model.EmployeeId = model.EmployeeId.Split('-')[0].Trim();
                if (model.BuyCostTotalStr != null)
                {
                    model.BuyCostTotal = Convert.ToDecimal(model.BuyCostTotalStr.Replace(",", ""));
                }
                if (model.BuyCostStr != null)
                {
                    model.BuyCost = Convert.ToDecimal(model.BuyCostStr.Replace(",", ""));
                }
                if (model.EmployeeContributionStr != null)
                {
                    model.EmployeeContribution = Convert.ToDecimal(model.EmployeeContributionStr.Replace(",", ""));
                }
                if (model.PenaltyPriceStr != null)
                {
                    model.PenaltyPrice = Convert.ToDecimal(model.PenaltyPriceStr.Replace(",", ""));
                }
                if (model.RefundCostStr != null)
                {
                    model.RefundCost = Convert.ToDecimal(model.RefundCostStr.Replace(",", ""));
                }
                if (model.PenaltyStr != null)
                {
                    model.Penalty = Convert.ToDecimal(model.PenaltyStr.Replace(",", ""));
                }

                var dataToSave = Mapper.Map<TraCtfDto>(model);
                dataToSave.DocumentStatus = Enums.DocumentStatus.Draft;
                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;
                dataToSave.ExtendVehicle = false;
                var saveResult = _ctfBLL.Save(dataToSave, CurrentUser);

                bool isSubmit = model.isSubmit == "submit";

                var reasonStr = _reasonBLL.GetReasonById(model.Reason.Value).Reason;

                var IsEndRent = reasonStr.ToLower() == "end rent";

                if (isSubmit)
                {
                    if (IsEndRent && (model.VehicleUsage == null ? "" : model.VehicleUsage.ToUpper()) == "COP")
                    {
                        CtfWorkflow(model.TraCtfId, Enums.ActionType.Approve, null, IsEndRent, true, model.DocumentNumber);
                        AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                        return RedirectToAction("DetailsBenefit", "TraCtf", new { TraCtfId = model.TraCtfId, IsPersonalDashboard = model.IsPersonalDashboard });
                    }
                    CtfWorkflow(model.TraCtfId, Enums.ActionType.Submit, null, false, true, model.DocumentNumber);
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
            if (ctfData.DocumentStatus == Enums.DocumentStatus.Completed || ctfData.DocumentStatus == Enums.DocumentStatus.Extended)
            {
                return RedirectToAction("DetailsWTC", "TraCtf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard = IsPersonalDashboard });
            }
            //if user want to edit doc
            if ((CurrentUser.EMPLOYEE_ID == ctfData.EmployeeId || ctfData.EmployeeIdCreator == CurrentUser.EMPLOYEE_ID) && ctfData.DocumentStatus == Enums.DocumentStatus.AssignedForUser)
            {
                return RedirectToAction("EditForEmployeeWTC", "TraCTf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard = IsPersonalDashboard });
            }
            if (ctfData.EmployeeIdFleetApproval == CurrentUser.EMPLOYEE_ID && ctfData.DocumentStatus == Enums.DocumentStatus.InProgress)
            {
                return RedirectToAction("InProgressWTC", "TraCTf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard = IsPersonalDashboard });
            }
            if (CurrentUser.UserRole == Enums.UserRole.Fleet && ctfData.DocumentStatus == Enums.DocumentStatus.WaitingFleetApproval)
            {
                return RedirectToAction("ApprovalFleetWTC", "TraCtf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard = IsPersonalDashboard });
            }
            if (CurrentUser.EMPLOYEE_ID == ctfData.EmployeeId && ctfData.DocumentStatus != Enums.DocumentStatus.AssignedForUser)
            {
                return RedirectToAction("DetailsWTC", "TraCTf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard = IsPersonalDashboard });
            }
            //if created by want to edit
            if ((CurrentUser.USER_ID != ctfData.CreatedBy && ctfData.DocumentStatus == Enums.DocumentStatus.AssignedForUser) || (CurrentUser.USER_ID != ctfData.CreatedBy && ctfData.DocumentStatus == Enums.DocumentStatus.Draft || (CurrentUser.USER_ID != ctfData.CreatedBy && ctfData.DocumentStatus == Enums.DocumentStatus.WaitingFleetApproval) || (CurrentUser.EMPLOYEE_ID != ctfData.EmployeeIdCreator && ctfData.DocumentStatus == Enums.DocumentStatus.InProgress)))
            {
                return RedirectToAction("DetailsWTC", "TraCtf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard = IsPersonalDashboard });
            }
            try
            {
                var model = new CtfItem();
                model = Mapper.Map<CtfItem>(ctfData);

                if (model.ExtendVehicle)
                {
                    var ctfExtend = _ctfExtendBLL.GetCtfExtend().Where(x => x.TraCtfId == model.TraCtfId).FirstOrDefault();
                    model.CtfExtend = ctfExtend;
                    if (model.CtfExtend != null)
                    {
                        model.CtfExtend.ExtendPriceStr = model.CtfExtend == null ? "" : string.Format("{0:n0}", model.CtfExtend.ExtendPrice);
                    }
                }


                model.IsPersonalDashboard = IsPersonalDashboard;
                model = initCreate(model);
                model.CurrentLogin = CurrentUser;

                model.BuyCostTotalStr = model.BuyCostTotal == null ? "" : string.Format("{0:n0}", model.BuyCostTotal);
                model.BuyCostStr = model.BuyCost == null ? "" : string.Format("{0:n0}", model.BuyCost);
                model.EmployeeContributionStr = model.EmployeeContribution == null ? "" : string.Format("{0:n0}", model.EmployeeContribution);
                model.PenaltyPriceStr = model.PenaltyPrice == null ? "" : string.Format("{0:n0}", model.PenaltyPrice);
                model.PenaltyStr = model.Penalty == null ? "" : string.Format("{0:n0}", model.Penalty);
                model.RefundCostStr = model.RefundCost == null ? "" : string.Format("{0:n0}", model.RefundCost);

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
                if (model.BuyCostTotalStr != null)
                {
                    model.BuyCostTotal = Convert.ToDecimal(model.BuyCostTotalStr.Replace(",", ""));
                }
                if (model.BuyCostStr != null)
                {
                    model.BuyCost = Convert.ToDecimal(model.BuyCostStr.Replace(",", ""));
                }
                if (model.EmployeeContributionStr != null)
                {
                    model.EmployeeContribution = Convert.ToDecimal(model.EmployeeContributionStr.Replace(",", ""));
                }
                if (model.PenaltyPriceStr != null)
                {
                    model.PenaltyPrice = Convert.ToDecimal(model.PenaltyPriceStr.Replace(",", ""));
                }
                if (model.RefundCostStr != null)
                {
                    model.RefundCost = Convert.ToDecimal(model.RefundCostStr.Replace(",", ""));
                }
                if (model.PenaltyStr != null)
                {
                    model.Penalty = Convert.ToDecimal(model.PenaltyStr.Replace(",", ""));
                }
                var dataToSave = Mapper.Map<TraCtfDto>(model);

                dataToSave.DocumentStatus = Enums.DocumentStatus.Draft;
                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;
                dataToSave.ExtendVehicle = false;
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
                        CtfWorkflow(model.TraCtfId, Enums.ActionType.Approve, null, IsEndRent, IsBenefit, model.DocumentNumber);
                        AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                        return RedirectToAction("DetailsWTC", "TraCtf", new { TraCtfId = model.TraCtfId, IsPersonalDashboard = model.IsPersonalDashboard });
                    }
                    CtfWorkflow(model.TraCtfId, Enums.ActionType.Submit, null, false, false, model.DocumentNumber);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("DetailsWTC", "TraCtf", new { @TraCtfId = model.TraCtfId, IsPersonalDashboard = model.IsPersonalDashboard });
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
        public ActionResult EditForEmployeeBenefit(int? TraCtfId, bool IsPersonalDashboard)
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

                var vehicle = _fleetBLL.GetFleet().Where(x => x.IsActive && (x.PoliceNumber == null ? "" : x.PoliceNumber.ToUpper()) == (model.PoliceNumber == null ? "" : model.PoliceNumber.ToUpper())
                                                        && (x.EmployeeID == null ? "" : x.EmployeeID) == (model.EmployeeId == null ? "" : model.EmployeeId)).FirstOrDefault();

                if(vehicle != null)
                {
                    model.WithdCity = vehicle.City;
                    model.WithdAddress = vehicle.Address;
                }
                

                model.BuyCostTotalStr = model.BuyCostTotal == null ? "" : string.Format("{0:n0}", model.BuyCostTotal);
                model.BuyCostStr = model.BuyCost == null ? "" : string.Format("{0:n0}", model.BuyCost);
                model.EmployeeContributionStr = model.EmployeeContribution == null ? "" : string.Format("{0:n0}", model.EmployeeContribution);
                model.PenaltyPriceStr = model.PenaltyPrice == null ? "" : string.Format("{0:n0}", model.PenaltyPrice);
                model.PenaltyStr = model.Penalty == null ? "" : string.Format("{0:n0}", model.Penalty);
                model.RefundCostStr = model.RefundCost == null ? "" : string.Format("{0:n0}", model.RefundCost);

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
                if (model.BuyCostTotalStr != null)
                {
                    model.BuyCostTotal = Convert.ToDecimal(model.BuyCostTotalStr.Replace(",", ""));
                }
                if (model.BuyCostStr != null)
                {
                    model.BuyCost = Convert.ToDecimal(model.BuyCostStr.Replace(",", ""));
                }
                if (model.EmployeeContributionStr != null)
                {
                    model.EmployeeContribution = Convert.ToDecimal(model.EmployeeContributionStr.Replace(",", ""));
                }
                if (model.PenaltyPriceStr != null)
                {
                    model.PenaltyPrice = Convert.ToDecimal(model.PenaltyPriceStr.Replace(",", ""));
                }
                if (model.RefundCostStr != null)
                {
                    model.RefundCost = Convert.ToDecimal(model.RefundCostStr.Replace(",", ""));
                }
                if (model.PenaltyStr != null)
                {
                    model.Penalty = Convert.ToDecimal(model.PenaltyStr.Replace(",", ""));
                }

                var dataToSave = Mapper.Map<TraCtfDto>(model);

                dataToSave.DocumentStatus = Enums.DocumentStatus.AssignedForUser;
                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;
                var saveResult = _ctfBLL.Save(dataToSave, CurrentUser);

                bool isSubmit = model.isSubmit == "submit";
                if (isSubmit)
                {
                    CtfWorkflow(model.TraCtfId, Enums.ActionType.Submit, null, false, true, model.DocumentNumber);
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
                model.ErrorMessage = exception.Message;
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

                var vehicle = _fleetBLL.GetFleet().Where(x => x.IsActive && (x.PoliceNumber == null ? "" : x.PoliceNumber.ToUpper()) == (model.PoliceNumber == null ? "" : model.PoliceNumber.ToUpper())
                                                && (x.EmployeeID == null ? "" : x.EmployeeID) == (model.EmployeeId == null ? "" : model.EmployeeId)).FirstOrDefault();

                if (vehicle != null)
                {
                    model.WithdCity = vehicle.City;
                    model.WithdAddress = vehicle.Address;
                }

                model.BuyCostTotalStr = model.BuyCostTotal == null ? "" : string.Format("{0:n0}", model.BuyCostTotal);
                model.BuyCostStr = model.BuyCost == null ? "" : string.Format("{0:n0}", model.BuyCost);
                model.EmployeeContributionStr = model.EmployeeContribution == null ? "" : string.Format("{0:n0}", model.EmployeeContribution);
                model.PenaltyPriceStr = model.PenaltyPrice == null ? "" : string.Format("{0:n0}", model.PenaltyPrice);
                model.PenaltyStr = model.Penalty == null ? "" : string.Format("{0:n0}", model.Penalty);
                model.RefundCostStr = model.RefundCost == null ? "" : string.Format("{0:n0}", model.RefundCost);

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

                if (model.BuyCostTotalStr != null)
                {
                    model.BuyCostTotal = Convert.ToDecimal(model.BuyCostTotalStr.Replace(",", ""));
                }
                if (model.BuyCostStr != null)
                {
                    model.BuyCost = Convert.ToDecimal(model.BuyCostStr.Replace(",", ""));
                }
                if (model.EmployeeContributionStr != null)
                {
                    model.EmployeeContribution = Convert.ToDecimal(model.EmployeeContributionStr.Replace(",", ""));
                }
                if (model.PenaltyPriceStr != null)
                {
                    model.PenaltyPrice = Convert.ToDecimal(model.PenaltyPriceStr.Replace(",", ""));
                }
                if (model.RefundCostStr != null)
                {
                    model.RefundCost = Convert.ToDecimal(model.RefundCostStr.Replace(",", ""));
                }
                if (model.PenaltyStr != null)
                {
                    model.Penalty = Convert.ToDecimal(model.PenaltyStr.Replace(",", ""));
                }

                var dataToSave = Mapper.Map<TraCtfDto>(model);

                dataToSave.DocumentStatus = Enums.DocumentStatus.AssignedForUser;
                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;
                var saveResult = _ctfBLL.Save(dataToSave, CurrentUser);

                bool isSubmit = model.isSubmit == "submit";
                if (isSubmit)
                {
                    CtfWorkflow(model.TraCtfId, Enums.ActionType.Submit, null, false, false, dataToSave.DocumentNumber);
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
                model.ErrorMessage = exception.Message;
                return View(model);
            }
        }
        #endregion

        #region--------- HR Approval ----------
        public ActionResult ApprovalHR(int? TraCtfId, bool IsPersonalDashboard)
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
                model.BuyCostStr = model.BuyCost == null ? "" : string.Format("{0:n0}", model.BuyCost);
                model.EmployeeContributionStr = model.EmployeeContribution == null ? "" : string.Format("{0:n0}", model.EmployeeContribution);
                model.PenaltyPriceStr = model.PenaltyPrice == null ? "" : string.Format("{0:n0}", model.PenaltyPrice);
                model.PenaltyStr = model.Penalty == null ? "" : string.Format("{0:n0}", model.Penalty);
                model.RefundCostStr = model.RefundCost == null ? "" : string.Format("{0:n0}", model.RefundCost);

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
        public ActionResult ApprovalHR(CtfItem model)
        {
            try
            {
                if (model.BuyCostTotalStr != null)
                {
                    model.BuyCostTotal = Convert.ToDecimal(model.BuyCostTotalStr.Replace(",", ""));
                }
                if (model.BuyCostStr != null)
                {
                    model.BuyCost = Convert.ToDecimal(model.BuyCostStr.Replace(",", ""));
                }
                if (model.EmployeeContributionStr != null)
                {
                    model.EmployeeContribution = Convert.ToDecimal(model.EmployeeContributionStr.Replace(",", ""));
                }
                if (model.PenaltyPriceStr != null)
                {
                    model.PenaltyPrice = Convert.ToDecimal(model.PenaltyPriceStr.Replace(",", ""));
                }
                if (model.RefundCostStr != null)
                {
                    model.RefundCost = Convert.ToDecimal(model.RefundCostStr.Replace(",", ""));
                }
                if (model.PenaltyStr != null)
                {
                    model.Penalty = Convert.ToDecimal(model.PenaltyStr.Replace(",", ""));
                }
                var dataToSave = Mapper.Map<TraCtfDto>(model);

                dataToSave.DocumentStatus = Enums.DocumentStatus.WaitingHRApproval;
                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;
                dataToSave.EmployeeIdFleetApproval = CurrentUser.EMPLOYEE_ID;
                dataToSave.ApprovedFleet = CurrentUser.USER_ID;
                dataToSave.ApprovedFleetDate = DateTime.Now;

                var Reason = _reasonBLL.GetReasonById(dataToSave.Reason.Value);

                var saveResult = _ctfBLL.Save(dataToSave, CurrentUser);
                var reasonStr = _reasonBLL.GetReasonById(model.Reason.Value).Reason;

                bool isSubmit = model.isSubmit == "submit";

                if (isSubmit)
                {
                    CtfWorkflow(model.TraCtfId, Enums.ActionType.Approve, null, false, true, model.DocumentNumber);
                    AddMessageInfo("Success Approve Document", Enums.MessageInfoType.Success);
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

                model.BuyCostTotalStr = model.BuyCostTotal == null ? "" : string.Format("{0:n0}", model.BuyCostTotal);
                model.BuyCostStr = model.BuyCost == null ? "" : string.Format("{0:n0}", model.BuyCost);
                model.EmployeeContributionStr = model.EmployeeContribution == null ? "" : string.Format("{0:n0}", model.EmployeeContribution);
                model.PenaltyPriceStr = model.PenaltyPrice == null ? "" : string.Format("{0:n0}", model.PenaltyPrice);
                model.PenaltyStr = model.Penalty == null ? "" : string.Format("{0:n0}", model.Penalty);
                model.RefundCostStr = model.RefundCost == null ? "" : string.Format("{0:n0}", model.RefundCost);


                var RemarkList = _remarkBLL.GetRemark().Where(x => (x.RoleType == null ? "" : x.RoleType.ToUpper()) == CurrentUser.UserRole.ToString().ToUpper() && x.DocumentType == (int)Enums.DocumentType.CTF && x.IsActive).ToList();

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
                if (model.BuyCostTotalStr != null)
                {
                    model.BuyCostTotal = Convert.ToDecimal(model.BuyCostTotalStr.Replace(",", ""));
                }
                if (model.BuyCostStr != null)
                {
                    model.BuyCost = Convert.ToDecimal(model.BuyCostStr.Replace(",", ""));
                }
                if (model.EmployeeContributionStr != null)
                {
                    model.EmployeeContribution = Convert.ToDecimal(model.EmployeeContributionStr.Replace(",", ""));
                }
                if (model.PenaltyPriceStr != null)
                {
                    model.PenaltyPrice = Convert.ToDecimal(model.PenaltyPriceStr.Replace(",", ""));
                }
                if (model.RefundCostStr != null)
                {
                    model.RefundCost = Convert.ToDecimal(model.RefundCostStr.Replace(",", ""));
                }
                if (model.PenaltyStr != null)
                {
                    model.Penalty = Convert.ToDecimal(model.PenaltyStr.Replace(",", ""));
                }
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

                    CtfWorkflow(model.TraCtfId, Enums.ActionType.Approve, null, false, true, model.DocumentNumber);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("InProgressBenefit", "TraCtf", new { @TraCtfId = model.TraCtfId, IsPersonalDashboard = model.IsPersonalDashboard });
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
                return RedirectToAction("DetailsWTC", "TraCtf", new { TraCtfId = ctfData.TraCtfId, IsPersonalDashboard = IsPersonalDashboard });
            }

            try
            {
                var model = new CtfItem();
                model = Mapper.Map<CtfItem>(ctfData);
                model.IsPersonalDashboard = IsPersonalDashboard;
                model = initCreate(model);

                model.BuyCostTotalStr = model.BuyCostTotal == null ? "" : string.Format("{0:n0}", model.BuyCostTotal);
                model.BuyCostStr = model.BuyCost == null ? "" : string.Format("{0:n0}", model.BuyCost);
                model.EmployeeContributionStr = model.EmployeeContribution == null ? "" : string.Format("{0:n0}", model.EmployeeContribution);
                model.PenaltyPriceStr = model.PenaltyPrice == null ? "" : string.Format("{0:n0}", model.PenaltyPrice);
                model.PenaltyStr = model.Penalty == null ? "" : string.Format("{0:n0}", model.Penalty);
                model.RefundCostStr = model.RefundCost == null ? "" : string.Format("{0:n0}", model.RefundCost);


                var RemarkList = _remarkBLL.GetRemark().Where(x => (x.RoleType == null ? "" : x.RoleType.ToUpper()) == CurrentUser.UserRole.ToString().ToUpper() && x.DocumentType == (int)Enums.DocumentType.CTF && x.IsActive).ToList();

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
                if (model.BuyCostTotalStr != null)
                {
                    model.BuyCostTotal = Convert.ToDecimal(model.BuyCostTotalStr.Replace(",", ""));
                }
                if (model.BuyCostStr != null)
                {
                    model.BuyCost = Convert.ToDecimal(model.BuyCostStr.Replace(",", ""));
                }
                if (model.EmployeeContributionStr != null)
                {
                    model.EmployeeContribution = Convert.ToDecimal(model.EmployeeContributionStr.Replace(",", ""));
                }
                if (model.PenaltyPriceStr != null)
                {
                    model.PenaltyPrice = Convert.ToDecimal(model.PenaltyPriceStr.Replace(",", ""));
                }
                if (model.RefundCostStr != null)
                {
                    model.RefundCost = Convert.ToDecimal(model.RefundCostStr.Replace(",", ""));
                }
                if (model.PenaltyStr != null)
                {
                    model.Penalty = Convert.ToDecimal(model.PenaltyStr.Replace(",", ""));
                }
                var a = ModelState;
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
                    CtfWorkflow(model.TraCtfId, Enums.ActionType.Approve, null, false, false, model.DocumentNumber);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("InProgressWTC", "TraCtf", new { @TraCtfId = model.TraCtfId, IsPersonalDashboard = model.IsPersonalDashboard });
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

        #region -------- In Progress -----------
        public ActionResult InProgressBenefit(int? TraCtfId, bool IsPersonalDashboard)
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
                model.BuyCostStr = model.BuyCost == null ? "" : string.Format("{0:n0}", model.BuyCost);
                model.EmployeeContributionStr = model.EmployeeContribution == null ? "" : string.Format("{0:n0}", model.EmployeeContribution);
                model.PenaltyPriceStr = model.PenaltyPrice == null ? "" : string.Format("{0:n0}", model.PenaltyPrice);
                model.PenaltyStr = model.Penalty == null ? "" : string.Format("{0:n0}", model.Penalty);
                model.RefundCostStr = model.RefundCost == null ? "" : string.Format("{0:n0}", model.RefundCost);


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
        public ActionResult InProgressBenefit(CtfItem model)
        {
            var a = ModelState.IsValid;
            try
            {
                if (model.BuyCostTotalStr != null)
                {
                    model.BuyCostTotal = Convert.ToDecimal(model.BuyCostTotalStr.Replace(",", ""));
                }
                if (model.BuyCostStr != null)
                {
                    model.BuyCost = Convert.ToDecimal(model.BuyCostStr.Replace(",", ""));
                }
                if (model.EmployeeContributionStr != null)
                {
                    model.EmployeeContribution = Convert.ToDecimal(model.EmployeeContributionStr.Replace(",", ""));
                }
                if (model.PenaltyPriceStr != null)
                {
                    model.PenaltyPrice = Convert.ToDecimal(model.PenaltyPriceStr.Replace(",", ""));
                }
                if (model.RefundCostStr != null)
                {
                    model.RefundCost = Convert.ToDecimal(model.RefundCostStr.Replace(",", ""));
                }
                if (model.PenaltyStr != null)
                {
                    model.Penalty = Convert.ToDecimal(model.PenaltyStr.Replace(",", ""));
                }

                var dataToSave = Mapper.Map<TraCtfDto>(model);

                dataToSave.DocumentStatus = Enums.DocumentStatus.InProgress;
                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;
                var saveResult = _ctfBLL.Save(dataToSave, CurrentUser);

                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                return RedirectToAction("DetailsBenefit", "TraCtf", new { @TraCtfId = model.TraCtfId, @IsPersonalDashboard = model.IsPersonalDashboard });

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = initCreate(model);
                model.CurrentLogin = CurrentUser;
                return View(model);
            }
        }
        public ActionResult InProgressWTC(int? TraCtfId, bool IsPersonalDashboard)
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
                model.BuyCostStr = model.BuyCost == null ? "" : string.Format("{0:n0}", model.BuyCost);
                model.EmployeeContributionStr = model.EmployeeContribution == null ? "" : string.Format("{0:n0}", model.EmployeeContribution);
                model.PenaltyPriceStr = model.PenaltyPrice == null ? "" : string.Format("{0:n0}", model.PenaltyPrice);
                model.PenaltyStr = model.Penalty == null ? "" : string.Format("{0:n0}", model.Penalty);
                model.RefundCostStr = model.RefundCost == null ? "" : string.Format("{0:n0}", model.RefundCost);

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
        public ActionResult InProgressWTC(CtfItem model)
        {
            var a = ModelState.IsValid;
            try
            {
                if (model.BuyCostTotalStr != null)
                {
                    model.BuyCostTotal = Convert.ToDecimal(model.BuyCostTotalStr.Replace(",", ""));
                }
                if (model.BuyCostStr != null)
                {
                    model.BuyCost = Convert.ToDecimal(model.BuyCostStr.Replace(",", ""));
                }
                if (model.EmployeeContributionStr != null)
                {
                    model.EmployeeContribution = Convert.ToDecimal(model.EmployeeContributionStr.Replace(",", ""));
                }
                if (model.PenaltyPriceStr != null)
                {
                    model.PenaltyPrice = Convert.ToDecimal(model.PenaltyPriceStr.Replace(",", ""));
                }
                if (model.RefundCostStr != null)
                {
                    model.RefundCost = Convert.ToDecimal(model.RefundCostStr.Replace(",", ""));
                }
                if (model.PenaltyStr != null)
                {
                    model.Penalty = Convert.ToDecimal(model.PenaltyStr.Replace(",", ""));
                }

                var dataToSave = Mapper.Map<TraCtfDto>(model);

                dataToSave.DocumentStatus = Enums.DocumentStatus.InProgress;
                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;
                var saveResult = _ctfBLL.Save(dataToSave, CurrentUser);

                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                return RedirectToAction("DetailsWTC", "TraCtf", new { @TraCtfId = model.TraCtfId, @IsPersonalDashboard = model.IsPersonalDashboard });

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

        #region --------- RejectFleet --------------
        public ActionResult RejectCtfBenefit(int TraCtfIdReject, int RemarkId, bool IsPersonalDashboard)
        {
            bool isSuccess = false;
            var CtfDoc = _ctfBLL.GetCtfById(TraCtfIdReject);
            try
            {

                CtfDoc.ApprovedFleet = CurrentUser.USER_ID;
                CtfDoc.ApprovedFleetDate = DateTime.Now;
                CtfDoc.EmployeeIdFleetApproval = CurrentUser.EMPLOYEE_ID;
                var saveResult = _ctfBLL.Save(CtfDoc, CurrentUser);
                CtfWorkflow(TraCtfIdReject, Enums.ActionType.Reject, RemarkId, false, true, CtfDoc.DocumentNumber);
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
                CtfWorkflow(TraCtfIdReject, Enums.ActionType.Reject, RemarkId, false, false, CtfDoc.DocumentNumber);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("DetailsWTC", "TraCtf", new { TraCtfId = TraCtfIdReject, IsPersonalDashboard = IsPersonalDashboard });
            AddMessageInfo("Success Reject Document", Enums.MessageInfoType.Success);
            return RedirectToAction(IsPersonalDashboard ? "PersonalDashboard" : "Index");
        }
        #endregion

        #region ---------- Extend CTF --------
        public ActionResult CtfExtend(CtfItem Model)
        {
            var settingData = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));

            var benefitType = settingData.Where(x => x.SettingName.ToUpper() == "BENEFIT").FirstOrDefault().SettingName;
            var wtcType = settingData.Where(x => x.SettingName.ToUpper() == "WTC").FirstOrDefault().SettingName;

            settingData = _settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_USAGE_BENEFIT");
            var CopUsage = settingData.Where(x => x.SettingName.ToUpper() == "COP").FirstOrDefault().SettingName;

            var TraCtfId = Model.TraCtfId;

            if (TraCtfId == 0)
            {

                var employee = _employeeBLL.GetEmployee().Where(x => x.EMPLOYEE_ID == Model.EmployeeId).FirstOrDefault();
                var vehicle = _fleetBLL.GetFleet().Where(x => x.PoliceNumber == Model.PoliceNumber && x.EmployeeID == Model.EmployeeId && x.IsActive).FirstOrDefault();

                Model.CreatedBy = CurrentUser.USER_ID;
                Model.EmployeeIdCreator = CurrentUser.EMPLOYEE_ID;
                Model.CreatedDate = DateTime.Now;
                Model.DocumentStatus = Enums.DocumentStatus.Draft;

                Model.EmployeeName = employee == null ? "" : employee.FORMAL_NAME;
                Model.CostCenter = employee == null ? "" : employee.COST_CENTER;
                Model.GroupLevel = employee == null ? 0 : employee.GROUP_LEVEL;

                Model.VehicleYear = vehicle.VehicleYear;
                Model.VehicleType = vehicle.VehicleType;
                Model.VehicleUsage = vehicle.VehicleUsage;
                Model.SupplyMethod = vehicle.SupplyMethod;

                Model.EndRendDate = vehicle.EndContract.Value;

                var IsBenefit = Model.VehicleType == benefitType;

                var TraCtfDto = Mapper.Map<TraCtfDto>(Model);
                var checkExistCtf = _ctfBLL.CheckCtfExists(TraCtfDto);

                if (checkExistCtf)
                {
                    Model = initCreate(Model);
                    Model.CurrentLogin = CurrentUser;
                    Model.TitleForm = "Car Termination Form";
                    Model.ErrorMessage = "Data already exists";
                    return View("Create", Model);
                }

                var CtfData = _ctfBLL.Save(TraCtfDto, CurrentUser);

                AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
                CtfWorkflow(CtfData.TraCtfId, Enums.ActionType.Created, null, false, IsBenefit, Model.DocumentNumber);

                TraCtfId = CtfData.TraCtfId;
            }

            var CtfDto = _ctfBLL.GetCtfById(TraCtfId);

            CtfDto.IsActive = true;
            CtfDto.EffectiveDate = Model.EndRendDate.Value;
            CtfDto.DocumentStatus = Enums.DocumentStatus.Extended;
            CtfDto.Reason = null;
            CtfDto.ModifiedBy = CurrentUser.USER_ID;
            CtfDto.ModifiedDate = DateTime.Now;
            CtfDto.ExtendVehicle = true;

            var IsBenefitExtend = CtfDto.VehicleType == benefitType;
            CtfDto = _ctfBLL.Save(CtfDto, CurrentUser);

            if (Model.CtfExtend.ExtendPriceStr != null)
            {
                Model.CtfExtend.ExtendPrice = Convert.ToDecimal(Model.CtfExtend.ExtendPriceStr.Replace(",", ""));
            }
            if (Model.BuyCostTotalStr != null)
            {
                Model.BuyCostTotal = Convert.ToDecimal(Model.BuyCostTotalStr.Replace(",", ""));
            }
            if (Model.BuyCostStr != null)
            {
                Model.BuyCost = Convert.ToDecimal(Model.BuyCostStr.Replace(",", ""));
            }
            if (Model.EmployeeContributionStr != null)
            {
                Model.EmployeeContribution = Convert.ToDecimal(Model.EmployeeContributionStr.Replace(",", ""));
            }
            if (Model.PenaltyPriceStr != null)
            {
                Model.PenaltyPrice = Convert.ToDecimal(Model.PenaltyPriceStr.Replace(",", ""));
            }
            if (Model.RefundCostStr != null)
            {
                Model.RefundCost = Convert.ToDecimal(Model.RefundCostStr.Replace(",", ""));
            }
            if (Model.PenaltyStr != null)
            {
                Model.Penalty = Convert.ToDecimal(Model.PenaltyStr.Replace(",", ""));
            }

            var TraCtfDtoExtend = new CtfExtendDto();
            TraCtfDtoExtend.ExtendPoliceNumber = Model.CtfExtend.ExtendPoliceNumber;
            TraCtfDtoExtend.ExtedPoLine = Model.CtfExtend.ExtedPoLine;
            TraCtfDtoExtend.ExtendPoNumber = Model.CtfExtend.ExtendPoNumber;
            TraCtfDtoExtend.ExtendPrice = Model.CtfExtend.ExtendPrice;
            TraCtfDtoExtend.NewProposedDate = Model.CtfExtend.NewProposedDate;
            TraCtfDtoExtend.Reason = Model.CtfExtend.Reason;
            TraCtfDtoExtend.TraCtfId = CtfDto.TraCtfId;

            _ctfExtendBLL.Save(TraCtfDtoExtend, CurrentUser);
            AddMessageInfo("Extend Success", Enums.MessageInfoType.Success);
            CtfWorkflow(CtfDto.TraCtfId, Enums.ActionType.Extend, null, false, IsBenefitExtend, Model.DocumentNumber);
            return RedirectToAction("Edit", "TraCtf", new { TraCtfId = CtfDto.TraCtfId, IsPersonalDashboard = false });

        }
        #endregion

        #region --------- Cancel Document CTF --------------

        public ActionResult CancelCtf(long TraCtfId, int RemarkId, string type, bool IsPersonalDashboard)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    CtfWorkflow(TraCtfId, Enums.ActionType.Cancel, RemarkId, false, true, "");
                    _ctfBLL.CancelCtf(TraCtfId, RemarkId, CurrentUser);
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
            if (CurrentUser.UserRole != Enums.UserRole.HR && CurrentUser.UserRole != Enums.UserRole.Viewer && CurrentUser.UserRole != Enums.UserRole.HR)
            {
                return RedirectToAction("Index", "TraCtf");
            }

            var EpafData = _epafBLL.GetEpafByDocType(Enums.DocumentType.CTF).ToList();
            var RemarkList = _remarkBLL.GetRemark().Where(x => (x.RoleType == null ? "" : x.RoleType.ToUpper()) == CurrentUser.UserRole.ToString().ToUpper() && x.DocumentType == (int)Enums.DocumentType.CTF && x.IsActive).ToList();

            var model = new CtfModel();
            model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");
            foreach (var data in EpafData)
            {
                var item = new CtfItem();
                item.EPafData = data;
                var traCtf = _ctfBLL.GetCtf().Where(x => x.EpafId == data.MstEpafId && x.DocumentStatus != Enums.DocumentStatus.Cancelled).FirstOrDefault();
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
                var settingData = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
                var benefitType = settingData.Where(x => x.SettingName.ToUpper() == "BENEFIT").FirstOrDefault().SettingName;
                var allCtfData = _ctfBLL.GetCtf().Where(x => x.VehicleType == benefitType);
                var existsCtf = allCtfData.Where(x => x.EmployeeId == data.EmployeeId && x.DocumentStatus != Enums.DocumentStatus.Cancelled
                                                           && x.EffectiveDate.Value.Day == data.EfectiveDate.Value.Day
                                                           && x.EffectiveDate.Value.Month == data.EfectiveDate.Value.Month
                                                           && x.EffectiveDate.Value.Year == data.EfectiveDate.Value.Year).FirstOrDefault();

                if (existsCtf != null)
                {
                    //update epaf id
                    existsCtf.EpafId = data.MstEpafId;

                    var csfDataExists = _ctfBLL.Save(existsCtf, CurrentUser);
                    AddMessageInfo("Success Update Epaf Data", Enums.MessageInfoType.Success);
                    return RedirectToAction("Dashboard", "TraCtf");
                }

                if (data == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                try
                {
                    TraCtfDto item = new TraCtfDto();

                    item = Mapper.Map<TraCtfDto>(data);

                    var reason = _reasonBLL.GetReason().Where(x => x.DocumentType == (int)Enums.DocumentType.CTF && (x.Reason == null ? "" : x.Reason.ToLower()) == (data.EpafAction == null ? "" : data.EpafAction.ToLower())).FirstOrDefault();

                    if (reason == null)
                    {
                        AddMessageInfo("Please Add Reason In Master Data", Enums.MessageInfoType.Warning);
                        return RedirectToAction("DashboardEpaf", "TraCtf");
                    }

                    var FleetData = _fleetBLL.GetFleet().Where(x => x.EmployeeID == item.EmployeeId && x.IsActive == true
                                                                && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (benefitType == null ? "" : benefitType.ToUpper())
                                                                && (x.SupplyMethod == null ? "" : x.SupplyMethod.ToUpper()) != "TEMPORARY").FirstOrDefault();

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
                        item.VehicleUsage = FleetData.VehicleUsage;
                        item.VehicleLocation = FleetData.City;
                        item.VehicleType = FleetData.VehicleType;
                        item.SupplyMethod = FleetData.SupplyMethod;
                        item.EndRendDate = FleetData.EndContract;
                    }
                    if (reason.IsPenalty)
                    {
                        if (reason.PenaltyForEmplloyee == true) item.Penalty = _ctfBLL.PenaltyCost(item);
                        if (reason.PenaltyForFleet == true) item.PenaltyPrice = _ctfBLL.PenaltyCost(item);
                    }
                    if ((item.VehicleUsage == null ? "" : item.VehicleUsage.ToUpper()) == "COP")
                    {
                        item.RefundCost = _ctfBLL.RefundCost(item);
                        item.EmployeeContribution = _ctfBLL.EmployeeContribution(item);
                    }

                    CtfData = _ctfBLL.Save(item, CurrentUser);

                    AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
                    CtfWorkflow(CtfData.TraCtfId, Enums.ActionType.Created, null, false, true, CtfData.DocumentNumber);
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

            foreach (var item in model.Details)
            {
                var ctfExtendDto = _ctfExtendBLL.GetCtfExtend().Where(x => x.TraCtfId == item.TraCtfId).FirstOrDefault();
                if (ctfExtendDto != null)
                {
                    item.CtfExtend = ctfExtendDto;
                }
            }

            return View(model);
        }
        #endregion

        #region -------- Upload ---------------
        public ActionResult UploadCtf()
        {
            var model = new CtfModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.IsPersonalDashboard = false;
            return View(model);
        }
        [HttpPost]
        public ActionResult UploadCtf(HttpPostedFileBase upload, CtfModel model)
        {
            var data = (new ExcelReader()).ReadExcel(upload);

            var VehicleList = _fleetBLL.GetFleet().Where(x => x.IsActive).ToList();
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
            var ReasonList = _reasonBLL.GetReason().Where(x => x.IsActive).ToList();
            bool isBenefit = false;

            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow.Count <= 0)
                    {
                        continue;
                    }
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    if (dataRow[0] == "EMPLOYEE ID")
                    {
                        continue;
                    }
                    var item = new CtfItem();
                    try
                    {
                        item.EmployeeId = dataRow[0];
                        item.EmployeeName = dataRow[1];
                        double dEffectiveDate = double.Parse(dataRow[3]);
                        DateTime dtEffectiveDate = DateTime.FromOADate(dEffectiveDate);
                        item.EffectiveDate = dtEffectiveDate;
                        item.PoliceNumber = dataRow[4];
                        item.CostCenter = dataRow[5];
                        item.VehicleType = dataRow[6];

                        var Reason = ReasonList.Where(x => (x.Reason == null ? "" : x.Reason.ToUpper()) == dataRow[2].ToUpper()
                                    && x.DocumentType == (int)Enums.DocumentType.CTF
                                    && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (item.VehicleType == null ? "" : item.VehicleType.ToUpper())).FirstOrDefault();

                        item.Reason = Reason.MstReasonId;
                        item.VehicleYear = Convert.ToInt32(dataRow[7]);
                        item.SupplyMethod = dataRow[8];
                        if (dataRow[9].ToUpper() == "YES") item.ExtendVehicle = true;
                        else if (dataRow[9].ToUpper() == "NO") item.ExtendVehicle = false;

                        var Vehicle = VehicleList.Where(x => (x.PoliceNumber == null ? "" : x.PoliceNumber.ToUpper()) == (item.PoliceNumber == null ? "" : item.PoliceNumber.ToUpper())
                                                                   && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (item.VehicleType == null ? "" : item.VehicleType.ToUpper())
                                                                   && x.EmployeeID == item.EmployeeId).FirstOrDefault();



                        item.EndRendDate = Vehicle.EndContract;
                        item.VehicleLocation = Vehicle.City;

                        var Employee = EmployeeList.Where(x => x.EMPLOYEE_ID == item.EmployeeId).FirstOrDefault();

                        if ((item.VehicleType == null ? "" : item.VehicleType.ToUpper()) == "BENEFIT")
                        {
                            item.VehicleUsage = Vehicle.VehicleUsage;
                            item.GroupLevel = Employee.GROUP_LEVEL;
                        }

                        if (item.ExtendVehicle == true)
                        {
                            item.CtfExtend = new CtfExtendDto();

                            double dNewDate = double.Parse(dataRow[10]);
                            DateTime dtNewDate = DateTime.FromOADate(dNewDate);
                            item.CtfExtend.NewProposedDate = dtNewDate;

                            item.CtfExtend.ExtendPoliceNumber = dataRow[11];
                            item.CtfExtend.ExtendPoNumber = dataRow[12];
                            item.CtfExtend.ExtedPoLine = dataRow[13];
                            item.CtfExtend.ExtendPrice = Convert.ToDecimal(dataRow[14]);

                            var ExtendReason = ReasonList.Where(x => x.DocumentType == 8
                                                && (x.Reason == null ? "" : x.Reason.ToUpper()) == (dataRow[15].ToUpper())).FirstOrDefault();

                            item.CtfExtend.Reason = ExtendReason.MstReasonId;
                        }

                    }
                    catch (Exception exp)
                    {
                        item.ErrorMessage = exp.Message;
                    }

                    #region ---- Save Data------
                    if (!item.ExtendVehicle)
                    {
                        item.CreatedBy = CurrentUser.USER_ID;
                        item.EmployeeIdCreator = CurrentUser.EMPLOYEE_ID;
                        item.CreatedDate = DateTime.Now;
                        item.DocumentStatus = Enums.DocumentStatus.Draft;
                        item.ExtendVehicle = false;
                        item.IsActive = true;

                        var settingData = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
                        var wtcType = settingData.Where(x => x.SettingName.ToUpper() == "WTC").FirstOrDefault().SettingName;

                        var ReasonData = _reasonBLL.GetReasonById(item.Reason.Value);

                        var reasonStr = ReasonData.Reason;
                        var IsPenalty = ReasonData.IsPenalty;
                        var PenaltyForFleet = ReasonData.PenaltyForFleet;
                        var PenaltyForEmployee = ReasonData.PenaltyForEmplloyee;

                        isBenefit = item.VehicleType == wtcType ? false : true;

                        var IsEndRent = reasonStr.ToLower() == "end rent";

                        //only check for benefit
                        var CtfDto = Mapper.Map<TraCtfDto>(item);
                        if (IsPenalty)
                        {
                            if (PenaltyForEmployee == true) CtfDto.Penalty = _ctfBLL.PenaltyCost(CtfDto);
                            if (PenaltyForFleet == true) CtfDto.PenaltyPrice = _ctfBLL.PenaltyCost(CtfDto);
                        }
                        try
                        {
                            var CtfData = _ctfBLL.Save(CtfDto, CurrentUser);
                            CtfWorkflow(CtfData.TraCtfId, Enums.ActionType.Created, null, false, isBenefit, item.DocumentNumber);
                            AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
                            if (IsEndRent)
                            {
                                CtfWorkflow(CtfData.TraCtfId, Enums.ActionType.Approve, null, true, isBenefit, item.DocumentNumber);
                                AddMessageInfo("Submit Success", Enums.MessageInfoType.Success);
                            }
                            else
                            {
                                CtfWorkflow(CtfData.TraCtfId, Enums.ActionType.Submit, null, false, isBenefit, item.DocumentNumber);
                                AddMessageInfo("Submit Success", Enums.MessageInfoType.Success);
                            }
                        }
                        catch (Exception exp)
                        {
                            model.ErrorMessage = "Send Failed, Error : " + exp.Message;
                            model.MainMenu = _mainMenu;
                            model.CurrentLogin = CurrentUser;
                            model.IsPersonalDashboard = false;
                            return View(model);
                        }
                    }
                    else
                    {
                        try
                        {
                            var settingData = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));

                            var benefitType = settingData.Where(x => x.SettingName.ToUpper() == "BENEFIT").FirstOrDefault().SettingName;
                            var wtcType = settingData.Where(x => x.SettingName.ToUpper() == "WTC").FirstOrDefault().SettingName;

                            settingData = _settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_USAGE_BENEFIT");
                            var CopUsage = settingData.Where(x => x.SettingName.ToUpper() == "COP").FirstOrDefault().SettingName;

                            item.CreatedBy = CurrentUser.USER_ID;
                            item.EmployeeIdCreator = CurrentUser.EMPLOYEE_ID;
                            item.CreatedDate = DateTime.Now;
                            item.DocumentStatus = Enums.DocumentStatus.Extended;
                            item.Reason = null;
                            item.ModifiedBy = CurrentUser.USER_ID;
                            item.ModifiedDate = DateTime.Now;
                            item.ExtendVehicle = true;

                            var TraCtfDto = Mapper.Map<TraCtfDto>(item);
                            var CtfData = _ctfBLL.Save(TraCtfDto, CurrentUser);

                            isBenefit = item.VehicleType == wtcType ? false : true;

                            AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
                            CtfWorkflow(CtfData.TraCtfId, Enums.ActionType.Created, null, false, isBenefit, item.DocumentNumber);

                            var TraCtfDtoExtend = new CtfExtendDto();
                            TraCtfDtoExtend.ExtendPoliceNumber = item.CtfExtend.ExtendPoliceNumber;
                            TraCtfDtoExtend.ExtedPoLine = item.CtfExtend.ExtedPoLine;
                            TraCtfDtoExtend.ExtendPoNumber = item.CtfExtend.ExtendPoNumber;
                            TraCtfDtoExtend.ExtendPrice = item.CtfExtend.ExtendPrice;
                            TraCtfDtoExtend.NewProposedDate = item.CtfExtend.NewProposedDate;
                            TraCtfDtoExtend.Reason = item.CtfExtend.Reason;
                            TraCtfDtoExtend.TraCtfId = CtfData.TraCtfId;

                            _ctfExtendBLL.Save(TraCtfDtoExtend, CurrentUser);
                            AddMessageInfo("Extend Success", Enums.MessageInfoType.Success);
                            CtfWorkflow(CtfData.TraCtfId, Enums.ActionType.Extend, null, false, isBenefit, CtfData.DocumentNumber);
                        }
                        catch (Exception exp)
                        {
                            model.ErrorMessage = "Extend Failed, Error : " + exp.Message;
                            model.MainMenu = _mainMenu;
                            model.CurrentLogin = CurrentUser;
                            model.IsPersonalDashboard = false;
                            return View(model);
                        }
                    }
                    #endregion
                }
            }
            return RedirectToAction("Index", "TraCtf");
        }
        #endregion

        #region --------- Json --------------
        [HttpPost]
        public JsonResult GetEmployee(string Id)
        {
            var data = Id.Split('-')[0].Trim();
            var model = new CtfItem();
            var employee = _employeeBLL.GetByID(data);
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
                vehicle = _fleetBLL.GetFleet().Where(x => x.IsActive == true && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (benefitType == null ? "" : benefitType.ToUpper()) && x.EmployeeID == data).FirstOrDefault();
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Fleet)
            {
                vehicle = _fleetBLL.GetFleet().Where(x => x.IsActive == true && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (wtcType == null ? "" : wtcType.ToUpper()) && x.EmployeeID == data).FirstOrDefault();
            }
            if (vehicle != null)
            {
                model.PoliceNumber = vehicle.PoliceNumber;
                model.VehicleYear = vehicle.VehicleYear;
                model.VehicleType = vehicle.VehicleType;
                model.VehicleUsage = vehicle.VehicleUsage;
                model.VehicleLocation = vehicle.City;
                model.SupplyMethod = vehicle.SupplyMethod;
                model.EndRendDate = vehicle.EndContract;
                model.CostCenterFleet = vehicle.CostCenter;
                model.WithdCity = vehicle.City;

            }
            return Json(model);
        }
        public JsonResult GetEmployeeList()
        {
            var model = _employeeBLL
                .GetEmployee()
                .Where(x => x.IS_ACTIVE)
                .Select(x
                    => new
                    {
                        DATA = string.Concat(x.EMPLOYEE_ID, " - ", x.FORMAL_NAME)
                    })
                    .OrderBy(X => X.DATA)
                .ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
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
                vehicle = _fleetBLL.GetFleet().Where(x => x.IsActive == true && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (benefitType == null ? "" : benefitType.ToUpper()) && (x.PoliceNumber == null ? "" : x.PoliceNumber.ToUpper()) == (Id == null ? "" : Id.ToUpper())).FirstOrDefault();
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Fleet)
            {
                vehicle = _fleetBLL.GetFleet().Where(x => x.IsActive == true && (x.PoliceNumber == null ? "" : x.PoliceNumber.ToUpper() ) == (Id == null ? "" : Id.ToUpper()) && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (wtcType == null ? "" : wtcType.ToUpper())).FirstOrDefault();
            }
            if (vehicle != null)
            {
                model.PoliceNumber = vehicle.PoliceNumber;
                model.VehicleYear = vehicle.VehicleYear;
                model.VehicleType = vehicle.VehicleType;
                model.VehicleUsage = vehicle.VehicleUsage;
                model.VehicleLocation = vehicle.City;
                model.SupplyMethod = vehicle.SupplyMethod;
                model.EndRendDate = vehicle.EndContract;
                model.CostCenterFleet = vehicle.CostCenter;
                model.WithdCity = vehicle.City;
            }
            var employee = _employeeBLL.GetByID(vehicle.EmployeeID);
            if (employee != null)
            {
                model.EmployeeId = employee.EMPLOYEE_ID;
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
            if (CurrentUser.UserRole == Enums.UserRole.HR)
            {
                var model = _fleetBLL.GetFleet().Where(x => x.IsActive == true && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (benefitType == null ? "" : benefitType.ToUpper()) && x.PoliceNumber != null).Select(x => new  {  x.PoliceNumber,  x.VehicleType}).ToList();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var model = _fleetBLL.GetFleet().Where(x => x.IsActive == true && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (wtcType == null ? "" : wtcType.ToUpper()) && x.PoliceNumber != null ).Select(x => new {x.PoliceNumber , x.VehicleType}).ToList();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var VehicleList = _fleetBLL.GetFleet().Where(x => x.IsActive).ToList();
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
            var ReasonList = _reasonBLL.GetReason().Where(x => x.IsActive).ToList();

            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<CtfItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow.Count <= 0)
                    {
                        continue;
                    }
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    if (dataRow[0] == "EMPLOYEE ID")
                    {
                        continue;
                    }
                    var item = new CtfItem();
                    item.ErrorMessage = "";
                    try
                    {

                        item.EmployeeId = dataRow[0];
                        if (item.EmployeeId == "")
                        {
                            item.ErrorMessage = "Employee ID can't be empty";
                        }

                        item.EmployeeName = dataRow[1];
                        if (item.EmployeeName == "")
                        {
                            item.ErrorMessage = "Employee Name can't be empty";
                        }

                        if (dataRow[3] == "")
                        {
                            item.ErrorMessage = "Effective Date Can't be empty";
                        }
                        else
                        {
                            try
                            {
                                double dEffectiveDate = double.Parse(dataRow[3]);
                                DateTime dtEffectiveDate = DateTime.FromOADate(dEffectiveDate);
                                item.EffectiveDate = dtEffectiveDate;
                            }
                            catch (Exception)
                            {
                                item.ErrorMessage = "Effective date format is not valid";
                            }
                        }

                        item.PoliceNumber = dataRow[4];
                        if (item.PoliceNumber == "")
                        {
                            item.ErrorMessage = "Police Number can't be empty";
                        }

                        item.CostCenter = dataRow[5];
                        if (item.CostCenter == "")
                        {
                            item.ErrorMessage = "Cost Center can't be empty";
                        }

                        item.VehicleType = dataRow[6];
                        if (item.VehicleType == "")
                        {
                            item.ErrorMessage = "Vehicle Type Can't be empty";
                        }

                        item.ReasonS = dataRow[2];
                        if (item.ReasonS == "")
                        {
                            item.ErrorMessage = "Reason Can't be empty";
                        }
                        else
                        {
                            var Reason = ReasonList.Where(x => (x.Reason == null ? "" : x.Reason.ToUpper()) == item.ReasonS.ToUpper()
                                        && x.DocumentType == (int)Enums.DocumentType.CTF
                                        && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (item.VehicleType == null ? "" : item.VehicleType.ToUpper())).FirstOrDefault();

                            if (Reason == null)
                            {
                                item.ErrorMessage = item.ReasonS + " is not found in master Reason";
                            }
                            else
                            {
                                item.Reason = Reason.MstReasonId;
                            }
                        }

                        if (dataRow[7] == "")
                        {
                            item.ErrorMessage = "Year can't be empty";
                        }
                        else
                        {
                            try
                            {
                                item.VehicleYear = Convert.ToInt32(dataRow[7]);
                            }
                            catch (Exception)
                            {
                                item.ErrorMessage = "Vehicle Year format is not valid";
                            }
                        }

                        item.SupplyMethod = dataRow[8];
                        if (item.SupplyMethod == "")
                        {
                            item.ErrorMessage = "Supply Method can't be empty";
                        }

                        if (dataRow[9] == "")
                        {
                            item.ErrorMessage = "Extend Vehicle can't be empty";
                        }
                        else
                        {
                            if (dataRow[9].ToUpper() == "YES") item.ExtendVehicle = true;
                            else if (dataRow[9].ToUpper() == "NO") item.ExtendVehicle = false;
                            else { item.ErrorMessage = "Extend Vehicle is unknown"; }
                        }

                        #region -- Vehicle & Employee Validation ----

                        var Employee = EmployeeList.Where(x => x.EMPLOYEE_ID == item.EmployeeId).FirstOrDefault();
                        if (Employee == null)
                        {
                            item.ErrorMessage = "Employee is not found in Master Employee";
                        }

                        var Vehicle = VehicleList.Where(x => (x.PoliceNumber == null ? "" : x.PoliceNumber.ToUpper()) == (item.PoliceNumber == null ? "" : item.PoliceNumber.ToUpper())
                                                                    && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (item.VehicleType == null ? "" : item.VehicleType.ToUpper())
                                                                    && x.EmployeeID == item.EmployeeId).FirstOrDefault();
                        if (Vehicle == null)
                        {
                            item.ErrorMessage = "Vehicle is not found in Master Fleet";
                        }

                        #endregion

                        if (item.ExtendVehicle == true)
                        {

                            #region -- Extend --

                            item.CtfExtend = new CtfExtendDto();

                            if (dataRow[10] == "")
                            {
                                item.ErrorMessage = "Extend New Date can't be empty";
                            }
                            else
                            {
                                try
                                {
                                    double dNewDate = double.Parse(dataRow[10]);
                                    DateTime dtNewDate = DateTime.FromOADate(dNewDate);
                                    item.CtfExtend.NewProposedDate = dtNewDate;
                                }
                                catch (Exception)
                                {
                                    item.ErrorMessage = "Extend New Date format is not valid";
                                }
                            }

                            item.CtfExtend.ExtendPoliceNumber = dataRow[11];
                            if (dataRow[11] == "")
                            {
                                item.ErrorMessage = "Extend Police Number can't be empty";
                            }

                            item.CtfExtend.ExtendPoNumber = dataRow[12];
                            if (dataRow[12] == "")
                            {
                                item.ErrorMessage = "Extend PO Number can't be empty";
                            }

                            item.CtfExtend.ExtedPoLine = dataRow[13];
                            if (dataRow[13] == "")
                            {
                                item.ErrorMessage = "Extend PO Line can't be empty";
                            }

                            if (dataRow[14] == "")
                            {
                                item.ErrorMessage = "Extend Price can't be empty";
                            }
                            else
                            {
                                try
                                {
                                    item.CtfExtend.ExtendPrice = Convert.ToDecimal(dataRow[14]);
                                }
                                catch (Exception)
                                {
                                    item.ErrorMessage = "Extend Price must be number";
                                }
                            }

                            item.CtfExtend.ReasonStr = dataRow[15];
                            if (dataRow[15] == "")
                            {
                                item.ErrorMessage = "Extend Reason Can't be empty";
                            }
                            else
                            {
                                var ExtendReason = ReasonList.Where(x => x.DocumentType == 8
                                                   && (x.Reason == null ? "" : x.Reason.ToUpper()) == (dataRow[15].ToUpper())).FirstOrDefault();
                                if (ExtendReason == null)
                                {
                                    item.ErrorMessage = "Reason " + dataRow[15] + " is not found in Master Reason";
                                }
                                else
                                {
                                    item.CtfExtend.Reason = ExtendReason.MstReasonId;
                                }
                            }
                            #endregion

                        }

                    }
                    catch (Exception exp)
                    {
                        item.ErrorMessage = exp.Message;
                    }

                    var Dto = Mapper.Map<TraCtfDto>(item);

                    var exist = _ctfBLL.CheckCtfExists(Dto);
                    if (exist)
                    {
                        item.ErrorMessage = "Data Already Exist";
                    }

                    model.Add(item);
                }
            }
            return Json(model);
        }
        [HttpPost]
        public JsonResult GetReasonByVehicleType(string VehicleType)
        {
            var ReasonList = _reasonBLL.GetReason().Where(x => x.IsActive == true && x.DocumentType == 6).OrderBy(x => x.Reason).ToList();

            if (!string.IsNullOrEmpty(VehicleType))
            {
                ReasonList = ReasonList.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == VehicleType.ToUpper()).ToList();
            }
            return Json(ReasonList);
        }
        #endregion

        #region --------- CTF Workflow --------------
        private void CtfWorkflow(long id, Enums.ActionType actionType, int? comment, bool Endrent, bool isBenefit, string DocumentNumber)
        {
            var input = new CtfWorkflowDocumentInput
            {
                DocumentId = id,
                UserId = CurrentUser.USER_ID,
                UserRole = CurrentUser.UserRole,
                EmployeeId = CurrentUser.EMPLOYEE_ID,
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
        //----------------------------- ExportPersonalDashboard --------------------------
        public void ExportPersonalDashboard()
        {
            string pathFile = "";

            pathFile = CreateXlsPersonalDashboard();

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
        private string CreateXlsPersonalDashboard()
        {
            //get data
            var data = _ctfBLL.GetCtfPersonal(CurrentUser);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Personal Dokumen CTF");
            slDocument.MergeWorksheetCells(1, 1, 1, 36);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelFleet(slDocument);

            //create data
            slDocument = CreateDataExcelFleet(slDocument, data, true);

            var fileName = "Personal_dashboard_CTF" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;
        }
        //------------------------------------EPAF----------------------------------------
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
            slDocument.SetCellValue(iRow, 9, "CTF No");
            slDocument.SetCellValue(iRow, 10, "CTF Status");
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
                var traCtf = _ctfBLL.GetCtf().Where(x => x.EpafId == data.MstEpafId).FirstOrDefault();
                if (traCtf == null)
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
                    ctf = _ctfBLL.GetCtf().Where(x => x.EpafId == data.MstEpafId).FirstOrDefault();
                    slDocument.SetCellValue(iRow, 9, ctf == null ? "" : ctf.DocumentNumber);
                    slDocument.SetCellValue(iRow, 10, ctf == null ? "" : ctf.DocumentStatus.ToString());
                    slDocument.SetCellValue(iRow, 11, data.ModifiedBy);
                    slDocument.SetCellValue(iRow, 12, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
                    iRow++;
                }
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
        //------------------------------------------------------------------------------//
        public void ExportCompleted()
        {
            if (CurrentUser.UserRole == Enums.UserRole.HR)
            {
                ExportHR(true);
            }
            else
            {
                ExportFleet(true);
            }
        }
        public void ExportOpen()
        {
            if (CurrentUser.UserRole == Enums.UserRole.HR)
            {
                ExportHR(false);
            }
            else
            {
                ExportFleet(false);
            }
        }
        //------------------------------------------------------------------------------//     
        public void ExportFleet(bool Completed)
        {
            string pathFile = "";

            pathFile = CreateXlsFleet(Completed);

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
        private string CreateXlsFleet(bool Completed)
        {
            //get data
            var data = _ctfBLL.GetCtfDashboard(CurrentUser, Completed);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, Completed == true ? "CTF Completed Document" : "CTF Open Document");
            slDocument.MergeWorksheetCells(1, 1, 1, 36);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelFleet(slDocument);

            //create data
            slDocument = CreateDataExcelFleet(slDocument, data, true);

            var fileName = (Completed == true ? "CTF_Completed_Document" : "CTF_Open_Document") + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }
        private SLDocument CreateHeaderExcelFleet(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "CTF Number");
            slDocument.SetCellValue(iRow, 2, "CTF Status");
            slDocument.SetCellValue(iRow, 3, "Reason Terminate");
            slDocument.SetCellValue(iRow, 4, "End Rent Date");
            slDocument.SetCellValue(iRow, 5, "Termination Date");
            slDocument.SetCellValue(iRow, 6, "Police Number");
            slDocument.SetCellValue(iRow, 7, "Vehicle Type");
            slDocument.SetCellValue(iRow, 8, "Vehicle Usage");
            slDocument.SetCellValue(iRow, 9, "Vehicle Year");
            slDocument.SetCellValue(iRow, 10, "Vehicle Location");
            slDocument.SetCellValue(iRow, 11, "Region");
            slDocument.SetCellValue(iRow, 12, "Cost Center");
            slDocument.SetCellValue(iRow, 13, "Supply Method");

            slDocument.SetCellValue(iRow, 14, "Coordinator");
            slDocument.SetCellValue(iRow, 15, "Employee ID");
            slDocument.SetCellValue(iRow, 16, "Employee Name");

            slDocument.SetCellValue(iRow, 17, "Witdhrawal PIC");
            slDocument.SetCellValue(iRow, 18, "Witdhrawal Phone");
            slDocument.SetCellValue(iRow, 19, "Witdhrawal Date");
            slDocument.SetCellValue(iRow, 20, "Witdhrawal City");
            slDocument.SetCellValue(iRow, 21, "Witdhrawal Address");

            slDocument.SetCellValue(iRow, 22, "Extend Vehicle");
            slDocument.SetCellValue(iRow, 23, "New Proposed Date");
            slDocument.SetCellValue(iRow, 24, "Extend PO Number");
            slDocument.SetCellValue(iRow, 25, "Extend PO Line");
            slDocument.SetCellValue(iRow, 26, "Extend Price");
            slDocument.SetCellValue(iRow, 27, "Extend Reason");

            slDocument.SetCellValue(iRow, 28, "User Decision");
            slDocument.SetCellValue(iRow, 29, "Buy Cost");
            slDocument.SetCellValue(iRow, 30, "Refund Cost");
            slDocument.SetCellValue(iRow, 31, "Employee Contribution");

            slDocument.SetCellValue(iRow, 32, "Penalty PO Number");
            slDocument.SetCellValue(iRow, 33, "Penalty PO Line");
            slDocument.SetCellValue(iRow, 34, "Penalty Cost");

            slDocument.SetCellValue(iRow, 35, "Updated By");
            slDocument.SetCellValue(iRow, 36, "Updated Date");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 36, headerStyle);

            return slDocument;

        }
        private SLDocument CreateDataExcelFleet(SLDocument slDocument, List<TraCtfDto> listData, bool isComplete)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.DocumentNumber);
                slDocument.SetCellValue(iRow, 2, data.DocumentStatus.ToString());
                slDocument.SetCellValue(iRow, 3, data.ReasonS);
                slDocument.SetCellValue(iRow, 4, data.EndRendDate == null ? "" : data.EndRendDate.Value.ToString("dd MMM yyyy"));
                slDocument.SetCellValue(iRow, 5, data.EffectiveDate == null ? "" : data.EffectiveDate.Value.ToString("dd MMM yyyy"));
                slDocument.SetCellValue(iRow, 6, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 7, data.VehicleType);
                slDocument.SetCellValue(iRow, 8, data.VehicleUsage);
                slDocument.SetCellValue(iRow, 9, data.VehicleYear == null ? 0 : data.VehicleYear.Value);
                slDocument.SetCellValue(iRow, 10, data.VehicleLocation);

                var region = _locationMappingBLL.GetLocationMapping().Where(x => (x.Location == null ? "" : x.Location.ToUpper()) == (data.VehicleLocation == null ? "" : data.VehicleLocation.ToUpper())).FirstOrDefault();
                slDocument.SetCellValue(iRow, 11, region == null ? "" : region.Region);
                slDocument.SetCellValue(iRow, 12, data.CostCenter);
                slDocument.SetCellValue(iRow, 13, data.SupplyMethod);

                slDocument.SetCellValue(iRow, 14, data.CreatedBy);
                slDocument.SetCellValue(iRow, 15, data.EmployeeId);
                slDocument.SetCellValue(iRow, 16, data.EmployeeName);

                slDocument.SetCellValue(iRow, 17, data.WithdPic);
                slDocument.SetCellValue(iRow, 18, data.WithdPhone);
                slDocument.SetCellValue(iRow, 19, data.WithdDate == null ? "" : data.WithdDate.Value.ToString("dd MMM yyyy"));
                slDocument.SetCellValue(iRow, 20, data.WithdCity);
                slDocument.SetCellValue(iRow, 21, data.WithdAddress);


                var extend = _ctfExtendBLL.GetCtfExtend().Where(x => x.TraCtfId == data.TraCtfId).FirstOrDefault();

                slDocument.SetCellValue(iRow, 22, data.ExtendVehicle == true ? "Yes" : "No");
                slDocument.SetCellValue(iRow, 23, extend == null ? "" : extend.NewProposedDate.Value.ToString("dd MMM yyyy"));
                slDocument.SetCellValue(iRow, 24, extend == null ? "" : extend.ExtendPoNumber);
                slDocument.SetCellValue(iRow, 25, extend == null ? "" : extend.ExtedPoLine);
                slDocument.SetCellValue(iRow, 26, extend == null ? 0 : extend.ExtendPrice.Value);
                slDocument.SetCellValue(iRow, 27, extend == null ? "" : extend.MstReason.REASON);

                if (data.UserDecision == 0)
                {
                    slDocument.SetCellValue(iRow, 28, "");
                }
                else
                {
                    slDocument.SetCellValue(iRow, 28, data.UserDecision == 1 ? "Buy" : (data.UserDecision == 2 ? "Refund":""));
                }

                slDocument.SetCellValue(iRow, 29, data.BuyCost.HasValue ? data.BuyCost.Value : 0);
                slDocument.SetCellValue(iRow, 30, data.RefundCost.HasValue ? data.RefundCost.Value : 0);
                slDocument.SetCellValue(iRow, 31, data.EmployeeContribution.HasValue ? data.EmployeeContribution.Value : 0);

                slDocument.SetCellValue(iRow, 32, data.PenaltyPoNumber);
                slDocument.SetCellValue(iRow, 33, data.PenaltyPoLine);
                slDocument.SetCellValue(iRow, 34, data.Penalty.HasValue ? data.Penalty.Value : 0);

                slDocument.SetCellValue(iRow, 35, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 36, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 36);
            slDocument.SetCellStyle(3, 1, iRow - 1, 36, valueStyle);

            return slDocument;
        }
        //--------------------------------HR---------------------------------------//
        public void ExportHR(bool Completed)
        {
            string pathFile = "";

            pathFile = CreateXlsHR(Completed);

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
        private string CreateXlsHR(bool Completed)
        {
            //get data
            var data = _ctfBLL.GetCtfDashboard(CurrentUser, Completed);
            var slDocument = new SLDocument();

            slDocument.SetCellValue(1, 1, Completed == true ? "CTF Completed Document" : "CTF Open Document");
            slDocument.MergeWorksheetCells(1, 1, 1, 31);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelHR(slDocument);

            //create data
            slDocument = CreateDataExcelHR(slDocument, data, Completed);

            var fileName = (Completed == true ? "CTF_Completed_Document" : "CTF_Open_Document") + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }
        private SLDocument CreateHeaderExcelHR(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "CTF Number");
            slDocument.SetCellValue(iRow, 2, "CTF Status");
            slDocument.SetCellValue(iRow, 3, "Reason Terminate");
            slDocument.SetCellValue(iRow, 4, "End Rent Date");
            slDocument.SetCellValue(iRow, 5, "Termination Date");
            slDocument.SetCellValue(iRow, 6, "Police Number");
            slDocument.SetCellValue(iRow, 7, "Vehicle Type");
            slDocument.SetCellValue(iRow, 8, "Vehicle Usage");
            slDocument.SetCellValue(iRow, 9, "Vehicle Year");
            slDocument.SetCellValue(iRow, 10, "Vehicle Location");
            slDocument.SetCellValue(iRow, 11, "Region");
            slDocument.SetCellValue(iRow, 12, "Cost Center");
            slDocument.SetCellValue(iRow, 13, "Supply Method");

            slDocument.SetCellValue(iRow, 14, "Coordinator");
            slDocument.SetCellValue(iRow, 15, "Employee ID");
            slDocument.SetCellValue(iRow, 16, "Employee Name");
            slDocument.SetCellValue(iRow, 17, "Group Level");

            slDocument.SetCellValue(iRow, 18, "Witdhrawal PIC");
            slDocument.SetCellValue(iRow, 19, "Witdhrawal Phone");
            slDocument.SetCellValue(iRow, 20, "Witdhrawal Date");
            slDocument.SetCellValue(iRow, 21, "Witdhrawal City");
            slDocument.SetCellValue(iRow, 22, "Witdhrawal Address");

            slDocument.SetCellValue(iRow, 23, "User Decision");
            slDocument.SetCellValue(iRow, 24, "Buy Cost");
            slDocument.SetCellValue(iRow, 25, "Refund Cost");
            slDocument.SetCellValue(iRow, 26, "Employee Contribution");

            slDocument.SetCellValue(iRow, 27, "Penalty PO Number");
            slDocument.SetCellValue(iRow, 28, "Penalty PO Line");
            slDocument.SetCellValue(iRow, 29, "Penalty Cost");

            slDocument.SetCellValue(iRow, 30, "Updated By");
            slDocument.SetCellValue(iRow, 31, "Updated Date");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 31, headerStyle);

            return slDocument;

        }
        private SLDocument CreateDataExcelHR(SLDocument slDocument, List<TraCtfDto> listData, bool isComplete)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.DocumentNumber);
                slDocument.SetCellValue(iRow, 2, data.DocumentStatus.ToString());
                slDocument.SetCellValue(iRow, 3, data.ReasonS);
                slDocument.SetCellValue(iRow, 4, data.EndRendDate == null ? "" : data.EndRendDate.Value.ToString("dd MMM yyyy"));
                slDocument.SetCellValue(iRow, 5, data.EffectiveDate == null ? "" : data.EffectiveDate.Value.ToString("dd MMM yyyy"));
                slDocument.SetCellValue(iRow, 6, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 7, data.VehicleType);
                slDocument.SetCellValue(iRow, 8, data.VehicleUsage);
                slDocument.SetCellValue(iRow, 9, data.VehicleYear.Value);
                slDocument.SetCellValue(iRow, 10, data.VehicleLocation);
                var region = _locationMappingBLL.GetLocationMapping().Where(x => x.Location == data.VehicleLocation).FirstOrDefault();
                slDocument.SetCellValue(iRow, 11, region == null ? "" : region.Region);
                slDocument.SetCellValue(iRow, 12, data.CostCenter);
                slDocument.SetCellValue(iRow, 13, data.SupplyMethod);

                slDocument.SetCellValue(iRow, 14, data.CreatedBy);
                slDocument.SetCellValue(iRow, 15, data.EmployeeId);
                slDocument.SetCellValue(iRow, 16, data.EmployeeName);
                slDocument.SetCellValue(iRow, 17, data.GroupLevel.HasValue ? 0 : data.GroupLevel.Value);

                slDocument.SetCellValue(iRow, 18, data.WithdPic);
                slDocument.SetCellValue(iRow, 19, data.WithdPhone);
                slDocument.SetCellValue(iRow, 20, data.WithdDate == null ? "" : data.WithdDate.Value.ToString("dd MMM yyyy"));
                slDocument.SetCellValue(iRow, 21, data.WithdCity);
                slDocument.SetCellValue(iRow, 22, data.WithdAddress);

                if (data.UserDecision == 0)
                {
                    slDocument.SetCellValue(iRow, 23, "");
                }
                else
                {
                    slDocument.SetCellValue(iRow, 23, data.UserDecision == 1 ? "Buy" : "Refund");
                }

                slDocument.SetCellValue(iRow, 24, data.BuyCost.HasValue ? data.BuyCost.Value : 0);
                slDocument.SetCellValue(iRow, 25, data.RefundCost.HasValue ? data.RefundCost.Value : 0);
                slDocument.SetCellValue(iRow, 26, data.EmployeeContribution.HasValue ? data.EmployeeContribution.Value : 0);

                slDocument.SetCellValue(iRow, 27, data.PenaltyPoNumber);
                slDocument.SetCellValue(iRow, 28, data.PenaltyPoLine);
                slDocument.SetCellValue(iRow, 29, data.Penalty.HasValue ? data.Penalty.Value : 0);

                slDocument.SetCellValue(iRow, 30, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 31, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 31);
            slDocument.SetCellStyle(3, 1, iRow - 1, 31, valueStyle);

            return slDocument;
        }
        //-----------------------------------------------------------------------------//
        #endregion

        #region  ------- Batch Email Vendor --------
        public void  GetListCtfInProgress ()
        {
            var ListCtf = _ctfBLL.GetCtf().Where(x => x.DocumentStatus == Enums.DocumentStatus.InProgress && x.DateSendVendor == null).ToList();
            var ListVendor = _vendorBLL.GetVendor().Where(x => x.IsActive).Select(x => new VendorItem { VendorName = x.VendorName, MstVendorId = x.MstVendorId, ShortName = x.ShortName }).Distinct().ToList();
            var ListCtfDto = new List<TraCtfDto>();
            var Vendor = new List<String>();
            bool IsSend = false;
            foreach (var CtfData in ListCtf)
            {
                var vehicle = _fleetBLL.GetFleet().Where(x => x.IsActive && x.PoliceNumber == CtfData.PoliceNumber && x.EmployeeID == CtfData.EmployeeId).FirstOrDefault();
                if(vehicle != null)
                {
                    CtfData.Vendor = (vehicle.VendorName == null ? "" : vehicle.VendorName.ToUpper()) ;
                    CtfData.ChasisNumber = vehicle.ChasisNumber;
                    CtfData.EngineNumber = vehicle.EngineNumber;
                    CtfData.Manufacture = vehicle.Manufacturer;
                    CtfData.Models = vehicle.Models;
                    CtfData.Series = vehicle.Series;
                    CtfData.Basetown = vehicle.City;
                }
                ListCtfDto.Add(CtfData);
            }

            Vendor = ListCtfDto.Where(x => x.Vendor != null ).Select(x => x.Vendor ).Distinct().ToList();
            
            foreach(var VendorItem in Vendor)
            {
                
                var reListCtfDto = ListCtfDto.Where(x => (x.Vendor == null ? "" : x.Vendor.ToUpper()) == VendorItem).ToList();

                var WtcListCtf = ListCtfDto.Where(x => (x.Vendor == null ? "" : x.Vendor.ToUpper()) == VendorItem && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) ==  "WTC").ToList();

                var BenefitListCtf = ListCtfDto.Where(x => (x.Vendor == null ? "" : x.Vendor.ToUpper()) == VendorItem && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT").ToList();
                
                string AttacthmentWtc = null;
                string AttacthmentBenefit = null;

                if (WtcListCtf.Count > 0)
                {
                    AttacthmentWtc = CreateExcelForVendor(WtcListCtf, "WTC");
                }

                if (BenefitListCtf.Count > 0)
                {
                    AttacthmentBenefit= CreateExcelForVendor(BenefitListCtf, "BENEFIT");
                }
                reListCtfDto = reListCtfDto.OrderBy(x => x.VehicleType).ToList();
                IsSend = _ctfBLL.BatchEmailCtf(reListCtfDto, VendorItem, AttacthmentWtc, AttacthmentBenefit);
                
                if(IsSend)
                {
                    foreach(var Ctf in reListCtfDto)
                    {
                        Ctf.DateSendVendor = DateTime.Now;

                        var login = new Login();
                        login.USER_ID = "SYSTEM";
                        _ctfBLL.Save(Ctf,login);
                    }
                }

            }
        }
        #endregion

        #region --------- Add Attachment File For Vendor --------------
        private string CreateExcelForVendor(List<TraCtfDto> CtfDto, string VehicleType)
        {
         
            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(2, 2, "Detail Vehicle");
            slDocument.MergeWorksheetCells(2, 2, 2, 15);

            slDocument.SetCellValue(2, 16, "Detail Withdrawal");
            slDocument.MergeWorksheetCells(2, 16, 2, 20);

          
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Font.FontSize = 11;
            slDocument.SetCellStyle(2, 2, 2, 20, valueStyle);

            //create header
            slDocument = CreateHeaderExcelForVendor(slDocument);

            //create data
            slDocument = CreateDataExcelForVendor(slDocument, CtfDto);

            var fileName = "Attachment_CTF_" + VehicleType + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
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
            slDocument.SetCellValue(iRow, 8, "Manufacture");
            slDocument.SetCellValue(iRow, 9, "Model");
            slDocument.SetCellValue(iRow, 10, "Series");
            slDocument.SetCellValue(iRow, 11, "VehicleUsage");
            slDocument.SetCellValue(iRow, 12, "Basetown");
            slDocument.SetCellValue(iRow, 13, "Transfer to CFM Idle");
            slDocument.SetCellValue(iRow, 14, "Contract End Date");
            slDocument.SetCellValue(iRow, 15, "Termination Date");
            slDocument.SetCellValue(iRow, 16, "PIC");
            slDocument.SetCellValue(iRow, 17, "Date & Time");
            slDocument.SetCellValue(iRow, 18, "Phone Number");
            slDocument.SetCellValue(iRow, 19, "City");
            slDocument.SetCellValue(iRow, 20, "Address");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.SetCellStyle(iRow, 2, iRow, 20, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelForVendor(SLDocument slDocument, List<TraCtfDto> ctfData)
        {
            int iRow = 4; //starting row data
            foreach(var data in ctfData)
            {
                slDocument.SetCellValue(iRow, 2, data.DocumentNumber);
                slDocument.SetCellValue(iRow, 3, data.EmployeeName);
                slDocument.SetCellValue(iRow, 4, data.Vendor);
                slDocument.SetCellValue(iRow, 5, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 6, data.ChasisNumber);
                slDocument.SetCellValue(iRow, 7, data.EngineNumber);
                slDocument.SetCellValue(iRow, 8, data.Manufacture);
                slDocument.SetCellValue(iRow, 9, data.Models);
                slDocument.SetCellValue(iRow, 10, data.Series);
                slDocument.SetCellValue(iRow, 11, data.VehicleUsage);
                slDocument.SetCellValue(iRow, 12, data.Basetown);
                slDocument.SetCellValue(iRow, 13, data.IsTransferToIdle == true ? "Yes": "No" );
                if(data.EndRendDate.HasValue)slDocument.SetCellValue(iRow, 14, data.EndRendDate.Value.ToOADate());
                if(data.EffectiveDate.HasValue)slDocument.SetCellValue(iRow, 15, data.EffectiveDate.Value.ToOADate());
                slDocument.SetCellValue(iRow, 16, data.WithdPic);
                if (data.WithdDate.HasValue) slDocument.SetCellValue(iRow, 17, data.WithdDate.Value.ToOADate());
                slDocument.SetCellValue(iRow, 18, data.WithdPhone);
                slDocument.SetCellValue(iRow, 19, data.WithdCity);
                slDocument.SetCellValue(iRow, 20, data.WithdAddress);

                SLStyle dateStyle = slDocument.CreateStyle();
                dateStyle.FormatCode = "dd-MMM-yyyy";

                slDocument.SetCellStyle(iRow, 14, iRow, 14, dateStyle);
                slDocument.SetCellStyle(iRow, 15, iRow, 15, dateStyle);

                dateStyle = slDocument.CreateStyle();
                dateStyle.FormatCode = "dd-MMM-yyyy HH:mm";
                slDocument.SetCellStyle(iRow, 17, iRow, 17, dateStyle);
                SLStyle valueStyle = slDocument.CreateStyle();
                valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

                slDocument.AutoFitColumn(2, 20);
                slDocument.SetCellStyle(iRow, 2, iRow, 20, valueStyle);

                iRow++;

            }
          

            //create style
            return slDocument;
        }

        #endregion

    }
}
