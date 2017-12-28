using System.IO;
using System.Web.Routing;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using FMS.BusinessObject.Dto;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Utils;
using FMS.Website.Models;
using FMS.Website.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SpreadsheetLight;

namespace FMS.Website.Controllers
{
    public class TraCafController : BaseController
    {

        private Enums.MenuList _mainMenu;

        
        
        private IRemarkBLL _remarkBLL;
        
        
        private ISettingBLL _settingBLL;
        
        private ICafBLL _cafBLL;
        //private IVendorBLL _vendorBLL;


        private List<SettingDto> _settingList;


        public TraCafController(IPageBLL pageBll,ICafBLL cafBLL, IRemarkBLL RemarkBLL,
            ISettingBLL SettingBLL)
            : base(pageBll, Core.Enums.MenuList.TraCaf)
        {
            //_epafBLL = epafBll;
            _cafBLL = cafBLL;
            _remarkBLL = RemarkBLL;
            
            
            _settingBLL = SettingBLL;
            _mainMenu = Enums.MenuList.Transaction;
            
            //_vendorBLL = vendorBLL;
            _settingList = _settingBLL.GetSetting();
            
        }

        private TraCafIndexViewModel InitialIndexModel(TraCafIndexViewModel model)
        {
            var RemarkList = _remarkBLL.GetRemark().Where(x => x.RoleType == CurrentUser.UserRole.ToString()).ToList();
            model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");
            

            model.CurrentLogin = CurrentUser;

            model.MainMenu = _mainMenu;
            model.CurrentPageAccess = CurrentPageAccess;
            return model;
        }

        private TraCafItemViewModel InitialItemModel(TraCafItemViewModel model)
        {
            model.CurrentLogin = CurrentUser;

            model.MainMenu = _mainMenu;
            model.CurrentPageAccess = CurrentPageAccess;
            return model;
        }

        //
        // GET: /TraCaf/

        public ActionResult Index()
        {
            var model = new TraCafIndexViewModel();
           
            //var data = _cafBLL.GetCrfEpaf().Where(x => x.CrfId == null);
            model = InitialIndexModel(model);
            var data = _cafBLL.GetCaf();
            if (CurrentUser.UserRole == Enums.UserRole.Fleet ||
                CurrentUser.UserRole == Enums.UserRole.Viewer ||
                CurrentUser.UserRole == Enums.UserRole.Administrator)
            {
                data = data
                    .Where(x => x.DocumentStatus != (int) Enums.DocumentStatus.Completed
                                && x.DocumentStatus != (int) Enums.DocumentStatus.Cancelled).ToList();
            }
            else
            {
                data = data
                    .Where(x => x.DocumentStatus != (int)Enums.DocumentStatus.Completed
                                && x.DocumentStatus != (int)Enums.DocumentStatus.Cancelled
                                && x.EmployeeId == CurrentUser.EMPLOYEE_ID).ToList();
            }
            
            model.Details = AutoMapper.Mapper.Map<List<TraCafItemDetails>>(data);
            return View(model);
        }

        public ActionResult CloseSirs(long traCafId)
        {
            _cafBLL.CloseCaf(traCafId);

            return RedirectToAction("Index", "TraCaf");
        }

        public ActionResult Details(long id, bool isPersonalDashboard)
        {
            var model = new TraCafItemViewModel();
            model = InitialItemModel(model);
            var data = _cafBLL.GetById(id);
            model.Detail = AutoMapper.Mapper.Map<TraCafItemDetails>(data);
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = GetChangesHistory((int) Enums.MenuList.TraCaf, id);
            model.WorkflowLogs = GetWorkflowHistory((int) Enums.MenuList.TraCaf, id);
            model.IsPersonalDashboard = isPersonalDashboard;
            return View(model);
        }

        public ActionResult Upload()
        {
            var model = new TraCafUploadViewModel();
            model.MainMenu = _mainMenu;

            model.CurrentLogin = CurrentUser;

            return View("UploadDashboard",model);
        }

        public ActionResult PersonalDashboard()
        {
            List<TraCafDto> data = _cafBLL.GetCafPersonal(CurrentUser);
            var model = new TraCafIndexViewModel
            {
                Details =  AutoMapper.Mapper.Map<List<TraCafItemDetails>>(data),
                MainMenu = Enums.MenuList.PersonalDashboard,
                CurrentLogin = CurrentUser,
                CurrentPageAccess = new RoleDto()
                {
                    ReadAccess = true,

                },
                IsPersonalDashboard = true
            };

            var RemarkList = _remarkBLL.GetRemark().Where(x => x.RoleType == CurrentUser.UserRole.ToString()).ToList();
            model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");
            //model.TitleForm = "CRF Personal Dashboard";
            // model.TitleExport = "ExportOpen";
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult Upload(TraCafUploadViewModel model)
        {
            try
            {
                var data = AutoMapper.Mapper.Map<List<TraCafDto>>(model.Details);
                _cafBLL.SaveList(data,CurrentUser);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SaveProgress(TraCafItemViewModel model)
        {
            try
            {
                var data = AutoMapper.Mapper.Map<List<TraCafProgressDto>>(model.Detail.ProgressDetails);
                var lastStatus = 0;
                foreach (var traCafProgressDto in data)
                {
                    lastStatus = _cafBLL.SaveProgress(traCafProgressDto, model.Detail.SirsNumber, CurrentUser);    
                }
                var mainData = _cafBLL.GetCafBySirs(model.Detail.SirsNumber);
                return RedirectToAction("Details", new { id = mainData.TraCafId, isPersonalDashboard = model.IsPersonalDashboard});
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                var cafData = _cafBLL.GetCafBySirs(model.Detail.SirsNumber);
                var modelData = Mapper.Map<TraCafItemDetails>(cafData);
                model = InitialItemModel(model);
                model.WorkflowLogs = GetWorkflowHistory((int) Enums.DocumentType.CAF, cafData.TraCafId);
                model.ChangesLogs = GetChangesHistory((int) Enums.DocumentType.CAF, cafData.TraCafId);
                model.Detail = modelData;
                return View("Details", model);
            }

            
        }

        [HttpPost]
        public PartialViewResult UploadProgress(HttpPostedFileBase itemExcelFile)
        {
            var data = (new ExcelReader()).ReadExcel(itemExcelFile);
            var model = new List<TraCafProgress>();
            var mainData = new TraCafDto();
            var sirsNumber = "";
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    var item = new TraCafProgress();
                    sirsNumber = dataRow[0];
                    mainData = _cafBLL.GetCafBySirs(sirsNumber);

                    if (mainData == null)
                    {
                        item.Message = "SRIS number not found on FMS.";
                    }
                    else
                    {
                        item.TraCafId = mainData.TraCafId;
                        item.CreatedBy = CurrentUser.USER_ID;
                        item.CreatedDate = DateTime.Now;
                        double d = double.Parse(dataRow[2]);
                        DateTime conv = DateTime.FromOADate(d);
                        item.Estimation = conv;
                        item.ProgressDate = DateTime.Now;
                        item.StatusId = (int) EnumHelper.GetValueFromDescription<Enums.DocumentStatus>(dataRow[1]);
                        item.Remark = dataRow[3];
                        if (item.StatusId == 0)
                        {
                            item.Message = "Status Not recognized.";
                        }
                    }

                    model.Add(item);
                    
                }
            }
            TraCafItemViewModel modelData = new TraCafItemViewModel();
            modelData.Detail = new TraCafItemDetails();
            modelData.Detail.SirsNumber = sirsNumber;
            modelData.Detail.ProgressDetails = model;
            return PartialView("_UploadDetailList",modelData);
        }

        public ActionResult Complete(int TraCafId)
        {
            _cafBLL.CompleteCaf(TraCafId, CurrentUser);

            return RedirectToAction("Index", "TraCaf");

        }

        [HttpPost]
        public PartialViewResult UploadFileAjax(HttpPostedFileBase itemExcelFile)
        {
            var qtyPacked = string.Empty;
            var qty = string.Empty;

            var data = (new ExcelReader()).ReadExcel(itemExcelFile);
            var model = new List<TraCafItemDetails>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    var item = new TraCafItemDetails();
                    item.SirsNumber = dataRow[0].ToString();
                    item.CreatedDate = DateTime.Now;
                    item.CreatedBy = CurrentUser.USER_ID;
                    item.PoliceNumber = dataRow[3].ToString();
                    item.Supervisor = dataRow[4].ToString();
                    item.IncidentLocation = dataRow[6].ToString();
                    item.IncidentDescription = dataRow[7].ToString();
                    item.EmployeeName = dataRow[8].ToString();
                    item.EmployeeId = dataRow[9].ToString();
                    item.Area = dataRow[10].ToString();
                    item.VehicleModel = dataRow[11].ToString();
                    item.VendorName = dataRow[12].ToString();
                    double d = double.Parse(dataRow[5].ToString());
                    try
                    {
                        DateTime conv = DateTime.FromOADate(d);
                        item.IncidentDate = conv;
                    }
                    catch (Exception)
                    {
                        item.Message = "Failed to parse Incident Date from excel";
                        model.Add(item);
                        continue;
                    }
                    
                    

                    var dataTovalidate = AutoMapper.Mapper.Map<TraCafDto>(item);
                    string message = "";
                    _cafBLL.ValidateCaf(dataTovalidate, out message);
                    item.Message = message;
                    model.Add(item);
                }

                
                
                
            }
            var modelData = new TraCafUploadViewModel();
            modelData.Details = model;
            return PartialView("_UploadFileDocumentsList", modelData);
        }

        public ActionResult Completed()
        {
            var model = new TraCafIndexViewModel();

            //var data = _cafBLL.GetCrfEpaf().Where(x => x.CrfId == null);
            model = InitialIndexModel(model);
            var data = _cafBLL.GetCaf();
            if (CurrentUser.UserRole != Enums.UserRole.User)
            {
                data = data
                    .Where(x => x.DocumentStatus == (int)Enums.DocumentStatus.Completed
                                || x.DocumentStatus == (int)Enums.DocumentStatus.Cancelled
                                ).ToList();
            }
            else
            {
                data = data
                    .Where(x => (x.DocumentStatus == (int)Enums.DocumentStatus.Completed
                                || x.DocumentStatus == (int)Enums.DocumentStatus.Cancelled)
                                && x.EmployeeId == CurrentUser.EMPLOYEE_ID).ToList();
            }
                
            

            model.Details = AutoMapper.Mapper.Map<List<TraCafItemDetails>>(data);
            return View(model);
        }


        #region --------- Export --------------
        [HttpPost]
        public void ExportMasterEpaf()
        {

        }


        //public void ExportDashboard()
        //{
        //    string pathFile = "";

        //    pathFile = CreateXlsDashboard();

        //    var newFile = new FileInfo(pathFile);

        //    var fileName = Path.GetFileName(pathFile);

        //    string attachment = string.Format("attachment; filename={0}", fileName);
        //    Response.Clear();
        //    Response.AddHeader("content-disposition", attachment);
        //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    Response.WriteFile(newFile.FullName);
        //    Response.Flush();
        //    newFile.Delete();
        //    Response.End();
        //}

        

        

        public void ExportOpen()
        {
            string pathFile = "";

            pathFile = CreateXlsCAF(false);

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

            pathFile = CreateXlsCAF(true);

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

        public void ExportPersonalDashboard()
        {
            string pathFile = "";

            pathFile = CreateXlsCAF(true);

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

        private string CreateXlsPersonal()
        {
            List<TraCafDto> CRF = new List<TraCafDto>(); //_CRFBLL.GetCRF();
            CRF = _cafBLL.GetCafPersonal(CurrentUser);
            var listData = Mapper.Map<List<TraCafItemDetails>>(CRF);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Personal Dashboard CAF");
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

            var fileName = "Data_CAF" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;
        }

        private string CreateXlsCAF(bool isCompleted)
        {
            //get data
            List<TraCafDto> CRF = new List<TraCafDto>(); //_CRFBLL.GetCRF();
            if (isCompleted)
            {
                CRF = _cafBLL.GetCaf().Where(x => x.DocumentStatus == (int)Enums.DocumentStatus.Completed || x.DocumentStatus == (int)Enums.DocumentStatus.Cancelled).ToList();

            }
            else
            {
                CRF = _cafBLL.GetCaf().Where(x => x.DocumentStatus != (int)Enums.DocumentStatus.Completed || x.DocumentStatus != (int)Enums.DocumentStatus.Cancelled).ToList();
            }
            var listData = Mapper.Map<List<TraCafItemDetails>>(CRF);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, isCompleted ? "Completed Document CAF" : "Open Document CAF");
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

            var fileName = "Data_CAF" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelCRF(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "CAF Number");
            slDocument.SetCellValue(iRow, 2, "CAF Status");
            slDocument.SetCellValue(iRow, 3, "Vehicle Type");
            slDocument.SetCellValue(iRow, 4, "Employee ID");
            slDocument.SetCellValue(iRow, 5, "Employee Name");
            slDocument.SetCellValue(iRow, 6, "SIRS Number");
            slDocument.SetCellValue(iRow, 7, "Police Number");
            slDocument.SetCellValue(iRow, 8, "Vehicle Model");
            slDocument.SetCellValue(iRow, 9, "Region");
            slDocument.SetCellValue(iRow, 10, "Vendor Name");
            slDocument.SetCellValue(iRow, 11, "Incident Date");
            slDocument.SetCellValue(iRow, 12, "Coordinator");
            slDocument.SetCellValue(iRow, 13, "Modified By");
            slDocument.SetCellValue(iRow, 14, "Modified Date");

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

        private SLDocument CreateDataExcelCRF(SLDocument slDocument, List<TraCafItemDetails> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.DocumentNumber);
                slDocument.SetCellValue(iRow, 2, data.DocumentStatusString);
                slDocument.SetCellValue(iRow, 3, data.VehicleType);
                slDocument.SetCellValue(iRow, 4, data.EmployeeId);
                slDocument.SetCellValue(iRow, 5, data.EmployeeName);
                slDocument.SetCellValue(iRow, 6, data.SirsNumber);
                slDocument.SetCellValue(iRow, 7, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 8, data.VehicleModel);
                slDocument.SetCellValue(iRow, 9, data.Region);
                slDocument.SetCellValue(iRow, 10, data.VendorName);
                slDocument.SetCellValue(iRow, 11, data.IncidentDateString);
                slDocument.SetCellValue(iRow, 12, data.CreatedBy);
                //slDocument.SetCellValue(iRow, 6, data.VehicleUsage);
                //slDocument.SetCellValue(iRow, 7, data.EffectiveDate.HasValue ? data.EffectiveDate.Value.ToString("dd-MMM-yyyy hh:mm:ss") : "");
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




        #endregion
    }
}
