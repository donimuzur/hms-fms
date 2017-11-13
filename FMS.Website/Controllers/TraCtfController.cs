using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Core.Exceptions;
using FMS.Website.Models;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
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
        private IRemarkBLL _remarkBLL;
        private IFleetBLL _fleetBLL;
        private IEmployeeBLL _employeeBLL;
        private IReasonBLL _reasonBLL;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private ILocationMappingBLL _locationMappingBLL;
        public TraCtfController(IPageBLL pageBll, IEpafBLL epafBll, ITraCtfBLL ctfBll, IRemarkBLL RemarkBLL, 
                                IEmployeeBLL  EmployeeBLL, IReasonBLL ReasonBLL, IFleetBLL FleetBLL, ILocationMappingBLL LocationMappingBLL): base(pageBll, Core.Enums.MenuList.TraCtf)
        {
          
            _epafBLL = epafBll;
            _ctfBLL = ctfBll;
            _employeeBLL = EmployeeBLL;
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
            var model = new CtfModel();
            var data = _ctfBLL.GetCtf();
            if (CurrentUser.UserRole == Enums.UserRole.HR)
            {
                model.Details = Mapper.Map<List<CtfItem>>(data.Where(x => x.DocumentStatus != Enums.DocumentStatus.Completed && x.DocumentStatus != Enums.DocumentStatus.Cancelled && (x.VehicleType == "Benefit" || x.VehicleType == null)));
                model.TitleForm = "CTF Open Document Benefit";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Fleet)
            {
                model.Details = Mapper.Map<List<CtfItem>>(data.Where(x => x.DocumentStatus != Enums.DocumentStatus.Completed && x.DocumentStatus != Enums.DocumentStatus.Cancelled && (x.VehicleType == "WTC" || x.VehicleType == null)));
                model.TitleForm = "CTF Open Document WTC";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                model.Details = Mapper.Map<List<CtfItem>>(data.Where(x => x.DocumentStatus != Enums.DocumentStatus.Completed && x.DocumentStatus != Enums.DocumentStatus.Cancelled ));
                model.TitleForm = "CTF Open Document";
            }

            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }
        #endregion

        #region --------- Create --------------
        public CtfItem initCreate(CtfItem model, string type)
        {
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE == true).Select(x => new { x.EMPLOYEE_ID, employee = x.EMPLOYEE_ID + " == " + x.FORMAL_NAME }).Distinct().ToList();
            var ReasonList = _reasonBLL.GetReason().Where(x => x.IsActive == true && x.DocumentType == 6).ToList();
            var VehicleLocationList = _locationMappingBLL.GetLocationMapping().Select(x => new { City = x.Location }).Distinct();
            var UserDecisionList = new Dictionary<int, string>{ { 1, "Buy" }, { 2, "Refund" }};
            var PoliceNumberList = type.ToLower() == "wtc"? _fleetBLL.GetFleet().Where(x => x.VehicleType.ToLower() == "wtc" && x.IsActive == true).ToList() : _fleetBLL.GetFleet().Where(x => x.VehicleType.ToLower() == "benefit" && x.IsActive == true).ToList();
            var ExtendList = new Dictionary<bool, string>{ { false, "No" }, { true, "Yes" }};

            model.ExtendList = new SelectList(ExtendList, "Key", "Value");
            model.PoliceNumberList = new SelectList(PoliceNumberList, "PoliceNumber", "PoliceNumber");
            model.UserDecisionList = new SelectList(ExtendList, "Key", "Value");
            model.ReasonList = new SelectList(ReasonList, "MstReasonId", "Reason");
            model.EmployeeIdList = new SelectList(EmployeeList, "EMPLOYEE_ID", "employee");
            model.VehicleLocationList = new SelectList(VehicleLocationList, "City", "City");
            return model;
        }
        public ActionResult CreateFormWtc()
        {
            if (CurrentUser.UserRole != Enums.UserRole.HR && CurrentUser.UserRole != Enums.UserRole.Fleet)
            {
                return RedirectToAction("Index");
            }

            var model = new CtfItem();
            model = initCreate(model,"wtc");
            model.CreatedBy = CurrentUser.USERNAME;
            model.MainMenu = _mainMenu;
            model.CreatedDate = DateTime.Now;
            model.CreatedDateS = model.CreatedDate.ToString("dd MMM yyyy");
            model.DocumentStatus = Enums.DocumentStatus.Draft;
            model.DocumentStatusS = Enums.DocumentStatus.Draft.ToString();
            model.CurrentLogin = CurrentUser;
            model.TitleForm = "Car Termination Form WTC";
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFormWtc(CtfItem Model)
        {
            var a = ModelState;
            try
            {
                Model.CreatedBy = CurrentUser.USER_ID;
                Model.CreatedDate = DateTime.Now;
                Model.DocumentStatus = Enums.DocumentStatus.Draft;
                Model.EndRendDate = Model.EndRendDateS == "" ? Model.EndRendDate = null : Convert.ToDateTime(Model.EndRendDateS);
                Model.IsActive = true;
                var Dto = Mapper.Map<TraCtfDto>(Model);
                var CtfData = _ctfBLL.Save(Dto, CurrentUser);
                AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
                CtfWorkflow(CtfData.TraCtfId, Enums.ActionType.Created, string.Empty);
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                Model = initCreate(Model, "wtc");
                Model.TitleForm = "Car Termination Form WTC";
                Model.ErrorMessage = exception.Message;
                Model.CurrentLogin = CurrentUser;
                Model.MainMenu = _mainMenu;
                return View(Model);
            }
        }

        public ActionResult CreateFormBenefit()
        {
            if (CurrentUser.UserRole != Enums.UserRole.HR && CurrentUser.UserRole != Enums.UserRole.Fleet)
            {
                return RedirectToAction("Index");
            }

            var model = new CtfItem();
            model = initCreate(model,"benefit");
            model.CreatedBy = CurrentUser.USERNAME;
            model.CreatedDate = DateTime.Now;
            model.CreatedDateS = model.CreatedDate.ToString("dd MMM yyyy");
            model.DocumentStatus = Enums.DocumentStatus.Draft;
            model.DocumentStatusS = Enums.DocumentStatus.Draft.ToString();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.TitleForm = "Car Termination Form Benefit";
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFormBenefit(CtfItem Model)
        {
            var a = ModelState;
            try
            {
                Model.CreatedBy = CurrentUser.USER_ID;
                Model.CreatedDate = DateTime.Now;
                Model.DocumentStatus = Enums.DocumentStatus.Draft;
                Model.EndRendDate = Model.EndRendDateS == "" ? Model.EndRendDate = null : Convert.ToDateTime(Model.EndRendDateS);
                Model.IsActive = true;
                var Dto = Mapper.Map<TraCtfDto>(Model);
                var CtfData = _ctfBLL.Save(Dto, CurrentUser);
                AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
                CtfWorkflow(CtfData.TraCtfId, Enums.ActionType.Created, string.Empty);
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                Model = initCreate(Model,"benefit");
                Model.TitleForm = "Car Termination Form Benefit";
                Model.ErrorMessage = exception.Message;
                Model.CurrentLogin = CurrentUser;
                Model.MainMenu = _mainMenu;
                return View(Model);
            }
                
            
        }
        #endregion

        #region ---------  Details --------------
        public ActionResult DetailsBenefit(int? TraCtfId)
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
                model = initCreate(model,"benefit");
                model.MainMenu = _mainMenu;
                model.CurrentLogin = CurrentUser;
                model.TitleForm = "Car Termination Form Benefit";
                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
        }
        public ActionResult DetailsWTC(int? TraCtfId)
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
                model = initCreate(model, "wtc");
                model.MainMenu = _mainMenu;
                model.CurrentLogin = CurrentUser;
                model.TitleForm = "Car Termination Form WTC";
                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region --------- Edit --------------
        public ActionResult EditBenefit(int? TraCtfId)
        {
            if (!TraCtfId.HasValue)
            {
                return HttpNotFound();
            }

            var ctfData = _ctfBLL.GetCtf().Where(x => x.TraCtfId == TraCtfId.Value).FirstOrDefault();
            
            //if user want to edit doc
            if (CurrentUser.EMPLOYEE_ID == ctfData.EmployeeId&& ctfData.DocumentStatus== Enums.DocumentStatus.AssignedForUser)
            {
                return RedirectToAction("EditForEmployeeBenefit", "TraCsf", new { id = ctfData.TraCtfId });
            }

            //if created by want to edit
            if (CurrentUser.USER_ID != ctfData.CreatedBy && ctfData.DocumentStatus== Enums.DocumentStatus.AssignedForUser)
            {
                if (ctfData.VehicleType == "BENEFIT" || ctfData.VehicleType == "Benefit")
                {
                    return RedirectToAction("DetailsBenefit", "TraCtf", new { id = ctfData.TraCtfId });
                }
                if (ctfData.VehicleType == "WTC" || ctfData.VehicleType == "wtc")
                {
                    return RedirectToAction("DetailsWTC", "TraCtf", new { id = ctfData.TraCtfId });
                }
            }

            //if hr want to approve / reject
            //if (ctfData.DocumentStatus == Enums.DocumentStatus.WaitingHRApproval)
            //{
            //    return RedirectToAction("ApproveHr", "TraCsf", new { id = csfData.TRA_CSF_ID });
            //}

            //if hr want to approve / reject
            if (ctfData.DocumentStatus== Enums.DocumentStatus.WaitingFleetApproval)
            {
                if (ctfData.VehicleType == "BENEFIT" || ctfData.VehicleType == "Benefit")
                {
                    return RedirectToAction("ApproveFleetBenefit", "TraCtf", new { id = ctfData.TraCtfId });
                }

                if (ctfData.VehicleType == "WTC" || ctfData.VehicleType == "wtc")
                {
                    return RedirectToAction("ApproveFleetWTC", "TraCtf", new { id = ctfData.TraCtfId });
                }
            }
            
            if (ctfData == null)
            {
                return HttpNotFound();
            }
            try
            {
                var model = new CtfItem();
                model = Mapper.Map<CtfItem>(ctfData);
                model = initCreate(model, "benefit");
                var RemarkList = _remarkBLL.GetRemark().Where(x => x.RoleType == CurrentUser.UserRole.ToString() && x.DocumentType == (int)Enums.DocumentType.CTF).ToList();
                model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");
                model.MainMenu = _mainMenu;
                model.CurrentLogin = CurrentUser;
                model.TitleForm = "Car Termination Form Benefit";
                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
        }
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBenefit(CtfItem model)
        {
            try
            {
                var dataToSave = Mapper.Map<TraCtfDto>(model);

                dataToSave.DocumentStatus = Enums.DocumentStatus.Draft;
                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;

                bool isSubmit = model.isSubmit == "submit";
                var saveResult = _ctfBLL.Save(dataToSave, CurrentUser);

                if (isSubmit)
                {
                    CtfWorkflow(model.TraCtfId, Enums.ActionType.Submit, string.Empty);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("DetailsBenefit", "TraCtf", new { @TraCtfId = model.TraCtfId });
                }
                
                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                return RedirectToAction("Index");

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = initCreate(model,"benefit");
                model.MainMenu = _mainMenu;
                model.CurrentLogin = CurrentUser;
                model.ErrorMessage = exception.Message;
                return View(model);
            }
        }

        public ActionResult EditWTC(int? TraCtfId)
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
                model = initCreate(model, "wtc");
                var RemarkList = _remarkBLL.GetRemark().Where(x => x.RoleType == CurrentUser.UserRole.ToString() && x.DocumentType == (int)Enums.DocumentType.CTF).ToList();
                model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");
                model.MainMenu = _mainMenu;
                model.CurrentLogin = CurrentUser;
                model.TitleForm = "Car Termination Form WTC";
                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
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

                if (isSubmit)
                {
                    CtfWorkflow(model.TraCtfId, Enums.ActionType.Submit, string.Empty);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("DetailsWTC", "TraCtf", new { @TraCtfId = model.TraCtfId });
                }

                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                return RedirectToAction("Index");

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = initCreate(model, "wtc");
                model.MainMenu = _mainMenu;
                model.CurrentLogin = CurrentUser;
                model.ErrorMessage = exception.Message;
                return View(model);
            }
        }
        #endregion

        #region --------- AprovalFleet --------------
        public ActionResult ApproveFleetBenefit(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var ctfData = _ctfBLL.GetCtfById(id.Value);

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
                model = initCreate(model,"benefit");

                var RemarkList = _remarkBLL.GetRemark().Where(x => x.RoleType == CurrentUser.UserRole.ToString() && x.DocumentType == (int)Enums.DocumentType.CTF).ToList();
                model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");

                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
        }

        public ActionResult ApproveFleetWTC(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var ctfData = _ctfBLL.GetCtfById(id.Value);

            if (ctfData == null)
            {
                return HttpNotFound();
            }

            if (CurrentUser.UserRole != Enums.UserRole.Fleet)
            {
                return RedirectToAction("DetailsWTC", "TraCtf", new { id = ctfData.TraCtfId });
            }

            try
            {
                var model = new CtfItem();
                model = Mapper.Map<CtfItem>(ctfData);
                model = initCreate(model, "wtc");

                var RemarkList = _remarkBLL.GetRemark().Where(x => x.RoleType == CurrentUser.UserRole.ToString() && x.DocumentType == (int)Enums.DocumentType.CTF).ToList();
                model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");

                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region --------- RejectFleet --------------
        public ActionResult RejectCtfBenefit(int TraCtfIdReject, int RemarkId)
        {
            bool isSuccess = false;
            try
            {
                var remarks = _remarkBLL.GetRemarkById(RemarkId).Remark;
                CtfWorkflow(TraCtfIdReject, Enums.ActionType.Reject, remarks);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("DetailsBenefit", "TraCtf", new { id = TraCtfIdReject });
            AddMessageInfo("Success Reject Document", Enums.MessageInfoType.Success);
            return RedirectToAction("Index");
        }
        public ActionResult RejectCtfWTC(int TraCtfIdReject, int RemarkId)
        {
            bool isSuccess = false;
            try
            {
                var remarks = _remarkBLL.GetRemarkById(RemarkId).Remark;
                CtfWorkflow(TraCtfIdReject, Enums.ActionType.Reject, remarks);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("DetailsWTC", "TraCtf", new { id = TraCtfIdReject });
            AddMessageInfo("Success Reject Document", Enums.MessageInfoType.Success);
            return RedirectToAction("Index");
        }
        #endregion

        #region --------- Cancel Document CTF --------------

        public ActionResult CancelCtf(long TraCtfId, int RemarkId)
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
            return RedirectToAction("Index");
        }

        #endregion

        #region --------- Dashboar Epaf --------------
        public ActionResult DashboardEpaf()
        {
            if (CurrentUser.UserRole != Enums.UserRole.HR)
            {
                return RedirectToAction("Index");
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
                    item.DocumentNumber = traCtf == null ? "" : traCtf.DocumentNumber;
                    item.DocumentStatus = traCtf.DocumentStatus;
                    item.CreatedBy = traCtf.CreatedBy;
                    item.CreatedDate = traCtf.CreatedDate;
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
                    _epafBLL.DeactivateEpaf(MstEpafId, RemarkId, CurrentUser.USERNAME);
                }
                catch (Exception)
                {

                }

            }
            return RedirectToAction("DashboardEpaf", "TraCtf");
        }
        public ActionResult Assign(long MstEpafId)
        {
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

                    item.Reason = reason.MstReasonId;
                    item.CreatedBy = CurrentUser.USER_ID;
                    item.CreatedDate = DateTime.Now;
                    item.DocumentStatus = Enums.DocumentStatus.Draft;
                    item.IsActive = true;

                    var CtfData = _ctfBLL.Save(item, CurrentUser);
                    AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
                    CtfWorkflow(CtfData.TraCtfId, Enums.ActionType.Created, string.Empty);
                }
                catch (Exception exp)
                {
                   
                    AddMessageInfo(exp.Message, Enums.MessageInfoType.Error);
                    return RedirectToAction("DashboardEpaf", "TraCtf");
                }

            }
            return RedirectToAction("DashboardEpaf", "TraCtf");
        }
        #endregion

        #region --------- Completed Document--------------
        public ActionResult Completed()
        {
            var model = new CtfModel();
            //model.TitleForm = "CSF Open Document";
            //model.EpafList = Mapper.Map<List<EpafData>>(data);
            var data = new List<TraCtfDto>();
            if (CurrentUser.UserRole == Enums.UserRole.Fleet)
            {
                data = _ctfBLL.GetCtf().Where(x => (x.DocumentStatus == Enums.DocumentStatus.Completed || x.DocumentStatus == Enums.DocumentStatus.Cancelled )&& (x.VehicleType == "WTC" || x.VehicleType == null)).ToList();

                model.TitleForm = "CTF Completed Document WTC";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.HR)
            {
                data = _ctfBLL.GetCtf().Where(x =>( x.DocumentStatus == Enums.DocumentStatus.Completed || x.DocumentStatus == Enums.DocumentStatus.Cancelled) && (x.VehicleType == "Benefit" || x.VehicleType == null)).ToList();

                model.TitleForm = "CTF Completed Document Benefit";
            }
            else if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                data = _ctfBLL.GetCtf().Where(x => (x.DocumentStatus == Enums.DocumentStatus.Completed || x.DocumentStatus == Enums.DocumentStatus.Cancelled) ).ToList();

                model.TitleForm = "CTF Completed Document";
            }
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
            var model = _employeeBLL.GetByID(Id);
            return Json(model);
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
            var model = _fleetBLL.GetFleet().Where(x=>x.PoliceNumber==Id).FirstOrDefault();
            var data = Mapper.Map<FleetItem>(model);
            data.EndContracts = data.EndContract == null ? "" : data.EndContract.Value.ToString("dd MMM yyyy");
            return Json(data);
        }
        [HttpPost]
        public JsonResult GetPoliceNumberList(string Id, string Type)
        {
            var model = _fleetBLL.GetFleet().Where(x => x.EmployeeID == Id & x.VehicleType == Type).ToList();
            return Json(model);
        }

        #endregion

        #region --------- CTF Workflow --------------
        private void CtfWorkflow(long id, Enums.ActionType actionType, string comment)
        {
            var input = new CtfWorkflowDocumentInput
            {
                DocumentId = id,
                UserId = CurrentUser.USER_ID,
                UserRole = CurrentUser.UserRole,
                ActionType = actionType,
                Comment = comment
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
