using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;
using System.IO;
using FMS.BusinessObject.Dto;
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

        
        private List<SettingDto> _settingList;

        public TraCrfController(IPageBLL pageBll, IEpafBLL epafBll, ITraCrfBLL crfBLL, IRemarkBLL RemarkBLL, IEmployeeBLL EmployeeBLL, IReasonBLL ReasonBLL,
            ISettingBLL SettingBLL, IFleetBLL FleetBLL)
            : base(pageBll, Core.Enums.MenuList.TraCrf)
        {
            _epafBLL = epafBll;
            _CRFBLL = crfBLL;
            _remarkBLL = RemarkBLL;
            _employeeBLL = EmployeeBLL;
            _reasonBLL = ReasonBLL;
            _settingBLL = SettingBLL;
            _mainMenu = Enums.MenuList.TraCrf;
            _fleetBLL = FleetBLL;
            _settingList = _settingBLL.GetSetting();
        }


        public TraCrfItemViewModel InitialModel(TraCrfItemViewModel model)
        {
            var list = _employeeBLL.GetEmployee().Select(x => new { x.EMPLOYEE_ID, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);
            var listReason = _reasonBLL.GetReason().Where(x => x.DocumentType == (int)Enums.DocumentType.CSF).Select(x => new { x.MstReasonId, x.Reason }).ToList().OrderBy(x => x.Reason);
            var listVehType = _settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_TYPE").Select(x => new { x.SettingName, x.SettingValue }).ToList();
            var listVehCat = _settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_CATEGORY").Select(x => new { x.SettingName, x.SettingValue }).ToList();
            var listVehUsage = _settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_USAGE").Select(x => new { x.SettingName, x.SettingValue }).ToList();
            var listSupMethod = _settingBLL.GetSetting().Where(x => x.SettingGroup == "SUPPLY_METHOD").Select(x => new { x.SettingName, x.SettingValue }).ToList();
            var listProject = _settingBLL.GetSetting().Where(x => x.SettingGroup == "PROJECT").Select(x => new { x.SettingName, x.SettingValue }).ToList();
            var listRelocate = _settingBLL.GetSetting().Where(x => x.SettingGroup == "RELOCATION_TYPE").Select(x => new { x.SettingName, x.SettingValue }).ToList();
            var listLocation = _employeeBLL.GetCityLocation();

            model.EmployeeList = new SelectList(list, "EMPLOYEE_ID", "FORMAL_NAME");
            model.LocationList = new SelectList(listLocation, "City", "City");
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
            var data = _CRFBLL.GetList();
            model.Details = Mapper.Map<List<TraCrfItemDetails>>(data);
            return View(model);
        }

        public ActionResult Completed()
        {
            var model = new TraCrfIndexViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            var data = _CRFBLL.GetList().Where(x => x.DOCUMENT_STATUS == (int)Enums.DocumentStatus.Completed || x.DOCUMENT_STATUS == (int)Enums.DocumentStatus.Cancelled);
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
                if (CurrentUser.UserRole == Enums.UserRole.HR)
                {

                    model.Detail.VehicleType = "BENEFIT";
                }
                else if (CurrentUser.UserRole == Enums.UserRole.Fleet)
                {
                    model.Detail.VehicleType = "WTC";
                }

                model.Detail.DocumentStatus = (int)Enums.DocumentStatus.Draft;
                return View(model);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = InitialModel(model);
                model.ErrorMessage = ex.Message;
                return View(model);
            }
            
        }

        public ActionResult AssignEpaf(int MstEpafId)
        {

            try
            {
                var epafData = _epafBLL.GetEpaf().Where(x => x.MstEpafId == MstEpafId).FirstOrDefault();

                if (epafData != null)
                {
                    TraCrfDto item = new TraCrfDto();

                    item = AutoMapper.Mapper.Map<TraCrfDto>(epafData);

                    
                    item.CREATED_BY = CurrentUser.USER_ID;
                    item.CREATED_DATE = DateTime.Now;
                    item.DOCUMENT_STATUS = (int)Enums.DocumentStatus.Draft;
                    item.IS_ACTIVE = true;

                    var csfData = _CRFBLL.SaveCrf(item, CurrentUser);
                }
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
                var dataToSave = Mapper.Map<TraCrfDto>(model.Detail);
                dataToSave.CREATED_BY = CurrentUser.USER_ID;
                dataToSave.CREATED_DATE = DateTime.Now;
                dataToSave.DOCUMENT_STATUS = (int)Enums.DocumentStatus.Draft;
                dataToSave.IS_ACTIVE = true;
                var data = _CRFBLL.SaveCrf(dataToSave, CurrentUser);
                return RedirectToAction("Edit", new { id = data.TRA_CRF_ID });
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = InitialModel(model);
                model.ErrorMessage = ex.Message;
                return View(model);
            }
            
        }

        public ActionResult Edit(long id)
        {
            var model = new TraCrfItemViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;

            model = InitialModel(model);
            model.ChangesLogs = GetChangesHistory((int) Enums.MenuList.TraCrf, id);
            var data = _CRFBLL.GetDataById(id);
            model.Detail = Mapper.Map<TraCrfItemDetails>(data);
            return View(model);
        }

        public ActionResult Details(long id)
        {
            var model = new TraCrfItemViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;

            model = InitialModel(model);
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.TraCrf, id);
            var data = _CRFBLL.GetDataById(id);
            model.Detail = Mapper.Map<TraCrfItemDetails>(data);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(TraCrfItemViewModel model)
        {
            try
            {
                var dataToSave = Mapper.Map<TraCrfDto>(model.Detail);
                dataToSave.MODIFIED_BY = CurrentUser.USER_ID;
                dataToSave.MODIFIED_DATE = DateTime.Now;
                _CRFBLL.SaveCrf(dataToSave, CurrentUser);
                return RedirectToAction("Edit", new { id = model.Detail.TraCrfId });
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = InitialModel(model);
                model.ErrorMessage = ex.Message;
                return View(model);
            }
            
        }

        public ActionResult Submit(long CrfId)
        {
            var model = new TraCrfItemViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;

            model = InitialModel(model);
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.TraCrf, (int)CrfId);
            var data = _CRFBLL.GetDataById((int)CrfId);
            model.Detail = Mapper.Map<TraCrfItemDetails>(data);
            try
            {
                _CRFBLL.SubmitCrf(CrfId, CurrentUser);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = InitialModel(model);
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
            return Json(model);
        }

        [HttpPost]
        public JsonResult GetVehicleData(string vehUsage,string location)
        {
            var modelVehicle = _fleetBLL.GetFleet().Where(x => x.IsActive && x.VehicleStatus == "ACTIVE").ToList();
            var data = modelVehicle;

            if (vehUsage == "CFM")
            {
                var modelCFMIdle = _fleetBLL.GetFleet().Where(x => x.IsActive && x.City == location && x.VehicleStatus == "CFM IDLE").ToList();
                data = modelCFMIdle;

                if (modelCFMIdle.Count == 0)
                {
                    data = modelVehicle;
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
            }).ToList();

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
            slDocument.SetCellValue(iRow, 9, "CRF No");
            slDocument.SetCellValue(iRow, 10, "CRF Status");
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
                //slDocument.SetCellValue(iRow, 9, data.CRf);
                //slDocument.SetCellValue(iRow, 10, data.CRFStatus);
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

        private SLDocument CreateDataExcelCRF(SLDocument slDocument, List<TraCrfItemDetails> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.DocumentNumber);
                //slDocument.SetCellValue(iRow, 2, data.DocumentStatusString);
                slDocument.SetCellValue(iRow, 3, data.EmployeeId);
                slDocument.SetCellValue(iRow, 4, data.EmployeeName);
                //slDocument.SetCellValue(iRow, 5, data.Remark);
                slDocument.SetCellValue(iRow, 6, data.EffectiveDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
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
