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
using FMS.Website.Models;
using AutoMapper;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;

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

        public TraCsfController(IPageBLL pageBll, IEpafBLL epafBll, ITraCsfBLL csfBll, IRemarkBLL RemarkBLL, IEmployeeBLL EmployeeBLL, IReasonBLL ReasonBLL,
            ISettingBLL SettingBLL, IFleetBLL FleetBLL)
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
            _mainMenu = Enums.MenuList.Transaction;
        }

        #endregion

        #region --------- Open Document --------------

        public ActionResult Index()
        {
            var data = _csfBLL.GetCsf(CurrentUser, false);
            var model = new CsfIndexModel();
            model.TitleForm = "CSF Open Document";
            model.TitleExport = "ExportOpen";
            model.CsfList = Mapper.Map<List<CsfData>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
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
            model.CsfList = Mapper.Map<List<CsfData>>(data);
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

            return View(model);
        }

        public CsfItemModel InitialModel(CsfItemModel model)
        {
            var list = _employeeBLL.GetEmployee().Select(x => new { x.EMPLOYEE_ID, employee = x.EMPLOYEE_ID + " - " + x.FORMAL_NAME, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);
            var listReason = _reasonBLL.GetReason().Where(x => x.DocumentType == (int)Enums.DocumentType.CSF).Select(x => new { x.MstReasonId, x.Reason }).ToList().OrderBy(x => x.Reason);
            var listVehType = _settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_TYPE").Select(x => new { x.MstSettingId, x.SettingValue }).ToList();
            var listVehCat = _settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_CATEGORY").Select(x => new { x.MstSettingId, x.SettingValue }).ToList();
            var listVehUsage = _settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_USAGE").Select(x => new { x.MstSettingId, x.SettingValue }).ToList();
            var listSupMethod = _settingBLL.GetSetting().Where(x => x.SettingGroup == "SUPPLY_METHOD").Select(x => new { x.MstSettingId, x.SettingValue }).ToList();
            var listProject = _settingBLL.GetSetting().Where(x => x.SettingGroup == "PROJECT").Select(x => new { x.MstSettingId, x.SettingValue }).ToList();

            model.Detail.EmployeeList = new SelectList(list, "EMPLOYEE_ID", "employee");
            model.Detail.ReasonList = new SelectList(listReason, "MstReasonId", "Reason");
            model.Detail.VehicleTypeList = new SelectList(listVehType, "MstSettingId", "SettingValue");
            model.Detail.VehicleCatList = new SelectList(listVehCat, "MstSettingId", "SettingValue");
            model.Detail.VehicleUsageList = new SelectList(listVehUsage, "MstSettingId", "SettingValue");
            model.Detail.SupplyMethodList = new SelectList(listSupMethod, "MstSettingId", "SettingValue");
            model.Detail.ProjectList = new SelectList(listProject, "MstSettingId", "SettingValue");

            var vehTypeBenefit = _settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_TYPE" && x.SettingName == "BENEFIT").FirstOrDefault().MstSettingId;

            model.Detail.IsBenefit = model.Detail.VehicleType == vehTypeBenefit.ToString() ? true : false;

            model.CurrentLogin = CurrentUser;
            model.MainMenu = _mainMenu;

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

                var csfData = _csfBLL.Save(item, CurrentUser);

                bool isSubmit = model.Detail.IsSaveSubmit == "submit";

                if (isSubmit)
                {
                    CsfWorkflow(csfData.TRA_CSF_ID, Enums.ActionType.Submit, null);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("Edit", "TraCsf", new { id = csfData.TRA_CSF_ID });
                }

                AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
                CsfWorkflow(csfData.TRA_CSF_ID, Enums.ActionType.Created, null);
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

        public ActionResult Detail(int? id)
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
                model.Detail = Mapper.Map<CsfData>(csfData);
                model = InitialModel(model);                

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

        public ActionResult Edit(int? id)
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
                return RedirectToAction("EditForEmployee", "TraCsf", new { id = csfData.TRA_CSF_ID });
            }

            //if created by want to edit
            if (CurrentUser.USER_ID != csfData.CREATED_BY && csfData.DOCUMENT_STATUS == Enums.DocumentStatus.AssignedForUser)
            {
                return RedirectToAction("Detail", "TraCsf", new { id = csfData.TRA_CSF_ID });
            }

            //if hr want to approve / reject
            if (csfData.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingHRApproval)
            {
                return RedirectToAction("ApproveHr", "TraCsf", new { id = csfData.TRA_CSF_ID });
            }

            //if hr want to approve / reject
            if (csfData.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingFleetApproval)
            {
                return RedirectToAction("ApproveFleet", "TraCsf", new { id = csfData.TRA_CSF_ID });
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
                return RedirectToAction("Index");
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
                    return RedirectToAction("Detail", "TraCsf", new { id = model.Detail.TraCsfId });
                }

                //return RedirectToAction("Index");
                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
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

        public ActionResult EditForEmployee(int? id)
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
                return RedirectToAction("Detail", "TraCsf", new { id = csfData.TRA_CSF_ID });
            }

            try
            {
                var model = new CsfItemModel();
                model.Detail = Mapper.Map<CsfData>(csfData);
                model = InitialModel(model);

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
        public ActionResult EditForEmployee(CsfItemModel model)
        {
            try
            {
                var dataToSave = Mapper.Map<TraCsfDto>(model.Detail);

                dataToSave.DOCUMENT_STATUS = Enums.DocumentStatus.AssignedForUser;
                dataToSave.MODIFIED_BY = CurrentUser.USER_ID;
                dataToSave.MODIFIED_DATE = DateTime.Now;

                bool isSubmit = model.Detail.IsSaveSubmit == "submit";

                var saveResult = _csfBLL.Save(dataToSave, CurrentUser);

                if (isSubmit)
                {
                    CsfWorkflow(model.Detail.TraCsfId, Enums.ActionType.Submit, null);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("Detail", "TraCsf", new { id = model.Detail.TraCsfId });
                }

                //return RedirectToAction("Index");
                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
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

        #region --------- Approve / Reject --------------

        public ActionResult ApproveHr(int? id)
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
                return RedirectToAction("Detail", "TraCsf", new { id = csfData.TRA_CSF_ID });
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
                return RedirectToAction("Index");
            }
        }

        public ActionResult ApproveFleet(int? id)
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
                return RedirectToAction("Detail", "TraCsf", new { id = csfData.TRA_CSF_ID });
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
                return RedirectToAction("Index");
            }
        }

        public ActionResult ApproveCsf(long TraCsfId)
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
            if (!isSuccess) return RedirectToAction("Detail", "TraCsf", new { id = TraCsfId });
            AddMessageInfo("Success Approve Document", Enums.MessageInfoType.Success);
            return RedirectToAction("Index");
        }

        public ActionResult RejectCsf(int TraCsfIdReject, int RemarkId)
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

            if (!isSuccess) return RedirectToAction("Detail", "TraCsf", new { id = TraCsfIdReject });
            AddMessageInfo("Success Reject Document", Enums.MessageInfoType.Success);
            return RedirectToAction("Index");
        }

        #endregion

        #region --------- Workflow --------------

        private void CsfWorkflow(long id, Enums.ActionType actionType, int? comment)
        {
            var input = new CsfWorkflowDocumentInput
            {
                DocumentId = id,
                UserId = CurrentUser.USER_ID,
                UserRole = CurrentUser.UserRole,
                ActionType = actionType,
                Comment = comment
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

        [HttpPost]
        public JsonResult GetVehicleData(string vehUsage)
        {
            var modelVehicle = _fleetBLL.GetFleet().Where(x => x.IsActive && x.VehicleStatus == "ACTIVE").ToList();
            var data = modelVehicle;

            if (vehUsage == "CFM")
            {
                var modelCFMIdle = _fleetBLL.GetFleet().Where(x => x.IsActive && x.VehicleStatus == "CFM IDLE").ToList();
                data = modelCFMIdle;

                if (modelCFMIdle.Count == 0)
                {
                    data = modelVehicle;
                }
            }

            return Json(data);
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
                    _epafBLL.DeactivateEpaf(EpafId, RemarkId, CurrentUser.USERNAME);
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

        public ActionResult CancelCsf(int TraCsfId, int RemarkId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _csfBLL.CancelCsf(TraCsfId, RemarkId, CurrentUser.USERNAME);
                    AddMessageInfo("Success Cancelled Document", Enums.MessageInfoType.Success);
                }
                catch (Exception)
                {

                }

            }
            return RedirectToAction("Index");
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

                    var listVehType = _settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_TYPE").Select(x => new { x.MstSettingId, x.SettingValue }).ToList();
                    item.VEHICLE_TYPE = listVehType.Where(x => x.SettingValue.ToLower() == "benefit").FirstOrDefault().MstSettingId.ToString();

                    var csfData = _csfBLL.Save(item, CurrentUser);

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
                slDocument.SetCellValue(iRow, 9, data.CsfNumber);
                slDocument.SetCellValue(iRow, 10, data.CsfStatus);
                slDocument.SetCellValue(iRow, 11, data.ModifiedBy);
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
            slDocument.MergeWorksheetCells(1, 1, 1, 8);
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

            slDocument.SetCellValue(iRow, 1, "CSF No");
            slDocument.SetCellValue(iRow, 2, "CSF Status");
            slDocument.SetCellValue(iRow, 3, "Employee ID");
            slDocument.SetCellValue(iRow, 4, "Employee Name");
            slDocument.SetCellValue(iRow, 5, "Reason");
            slDocument.SetCellValue(iRow, 6, "Effective Date");
            slDocument.SetCellValue(iRow, 7, "Modified By");
            slDocument.SetCellValue(iRow, 8, "Modified Date");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 8, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelCsf(SLDocument slDocument, List<CsfData> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.CsfNumber);
                slDocument.SetCellValue(iRow, 2, data.CsfStatusName);
                slDocument.SetCellValue(iRow, 3, data.EmployeeId);
                slDocument.SetCellValue(iRow, 4, data.EmployeeName);
                slDocument.SetCellValue(iRow, 5, data.Reason);
                slDocument.SetCellValue(iRow, 6, data.EffectiveDate.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 7, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 8, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 8);
            slDocument.SetCellStyle(3, 1, iRow - 1, 8, valueStyle);

            return slDocument;
        }

        #endregion
    }
}
