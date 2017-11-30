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

        public ActionResult Details(long id)
        {
            var model = new TraCafItemViewModel();
            model = InitialItemModel(model);
            var data = _cafBLL.GetById(id);
            model.Detail = AutoMapper.Mapper.Map<TraCafItemDetails>(data);
            model.ChangesLogs = GetChangesHistory((int) Enums.MenuList.TraCaf, id);
            model.WorkflowLogs = GetWorkflowHistory((int) Enums.MenuList.TraCaf, id);
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
                return RedirectToAction("Details", new { id = mainData.TraCafId});
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
                    double d = double.Parse(dataRow[5].ToString());
                    DateTime conv = DateTime.FromOADate(d);
                    item.IncidentDate = conv;
                    item.IncidentLocation = dataRow[6].ToString();
                    item.IncidentDescription = dataRow[7].ToString();
                    item.EmployeeName = dataRow[8].ToString();
                    item.EmployeeId = dataRow[9].ToString();
                    item.Area = dataRow[10].ToString();
                    item.VehicleModel = dataRow[11].ToString();
                    item.VendorName = dataRow[12].ToString();

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
            if (CurrentUser.UserRole == Enums.UserRole.User)
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
    }
}
