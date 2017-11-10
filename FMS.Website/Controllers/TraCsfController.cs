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

        public TraCsfController(IPageBLL pageBll, IEpafBLL epafBll, ITraCsfBLL csfBll, IRemarkBLL RemarkBLL, IEmployeeBLL EmployeeBLL, IReasonBLL ReasonBLL)
            : base(pageBll, Core.Enums.MenuList.TraCsf)
        {
            _epafBLL = epafBll;
            _csfBLL = csfBll;
            _pageBLL = pageBll;
            _remarkBLL = RemarkBLL;
            _employeeBLL = EmployeeBLL;
            _reasonBLL = ReasonBLL;
            _mainMenu = Enums.MenuList.Transaction;
        }

        #endregion

        #region --------- Open Document --------------

        public ActionResult Index()
        {
            var data = _csfBLL.GetCsf().Where(x => x.DOCUMENT_STATUS != (int)Enums.DocumentStatus.Completed);
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
            var data = _epafBLL.GetEpafByDocType(Enums.DocumentType.CSF);
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
            var data = _csfBLL.GetCsf().Where(x => x.DOCUMENT_STATUS == (int)Enums.DocumentStatus.Completed); ;
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
            var model = new CsfItemModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;

            model = InitialModel(model);

            return View(model);
        }

        public CsfItemModel InitialModel(CsfItemModel model)
        {
            var list = _employeeBLL.GetEmployee().Select(x => new { x.EMPLOYEE_ID, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);
            var listReason = _reasonBLL.GetReason().Where(x => x.DocumentType == (int)Enums.DocumentType.CSF).Select(x => new { x.MstReasonId, x.Reason }).ToList().OrderBy(x => x.Reason);

            model.Detail.EmployeeList = new SelectList(list, "EMPLOYEE_ID", "FORMAL_NAME");
            model.Detail.ReasonList = new SelectList(listReason, "MstReasonId", "Reason");
            model.Detail.CreateDate = DateTime.Now;
            model.Detail.EffectiveDate = DateTime.Now;
            model.Detail.CreateBy = CurrentUser.USERNAME;

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
                item.DOCUMENT_STATUS = (int)Enums.DocumentStatus.Draft;
                item.IS_ACTIVE = true;

                var csfData = _csfBLL.Save(item, CurrentUser.USER_ID);
                AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
                CsfWorkflow(csfData.TRA_CSF_ID, Enums.ActionType.Created, string.Empty);
                return RedirectToAction("DocumentList");
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = InitialModel(model);
                return View(model);
            }
        }

        #endregion

        #region --------- Workflow --------------

        private void CsfWorkflow(long id, Enums.ActionType actionType, string comment)
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

        #endregion

        #region --------- Close EPAF --------------

        public ActionResult CloseEpaf(int EpafId, int RemarkId)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _epafBLL.DeactivateEpaf(EpafId, RemarkId, CurrentUser.USERNAME);
                }
                catch (Exception)
                {

                }

            }
            return RedirectToAction("Dashboard", "TraCsf");
        }

        #endregion

        #region --------- Assign EPAF --------------

        public ActionResult AssignEpaf(int MstEpafId)
        {

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
                    item.DOCUMENT_STATUS = (int)Enums.DocumentStatus.Draft;
                    item.IS_ACTIVE = true;

                    var csfData = _csfBLL.Save(item, CurrentUser.USER_ID);
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
            List<TraCsfDto> csf = _csfBLL.GetCsf();
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
                slDocument.SetCellValue(iRow, 2, data.CsfStatus);
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
