using FMS.BusinessObject.Dto;
using FMS.Contract.BLL;
using FMS.Core;
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
        private IEmployeeBLL _employeeBLL;
        private IReasonBLL _reasonBLL;
        private ISettingBLL _settingBLL;
        private IFleetBLL _fleetBLL;
        private ICafBLL _cafBLL;
        //private IVendorBLL _vendorBLL;


        private List<SettingDto> _settingList;


        public TraCafController(IPageBLL pageBll,ICafBLL cafBLL, IRemarkBLL RemarkBLL, IEmployeeBLL EmployeeBLL, IReasonBLL ReasonBLL,
            ISettingBLL SettingBLL, IFleetBLL FleetBLL,IVendorBLL vendorBLL)
            : base(pageBll, Core.Enums.MenuList.TraCaf)
        {
            //_epafBLL = epafBll;
            _cafBLL = cafBLL;
            _remarkBLL = RemarkBLL;
            _employeeBLL = EmployeeBLL;
            _reasonBLL = ReasonBLL;
            _settingBLL = SettingBLL;
            _mainMenu = Enums.MenuList.Transaction;
            _fleetBLL = FleetBLL;
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
            List<TraCafDto> data = _cafBLL.GetCaf();
            model.Details = AutoMapper.Mapper.Map<List<TraCafItemDetails>>(data);
            return View(model);
        }


        public ActionResult Details(long id)
        {
            var model = new TraCafItemViewModel();
            model = InitialItemModel(model);
            var data = _cafBLL.GetById(id);
            model.Detail = AutoMapper.Mapper.Map<TraCafItemDetails>(data);
            return View(model);
        }

        public ActionResult Upload()
        {
            var model = new TraCafIndexViewModel();
            model.MainMenu = _mainMenu;

            model.CurrentLogin = CurrentUser;

            return View(model);
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
        public JsonResult UploadFileAjax(HttpPostedFileBase upload)
        {
            var qtyPacked = string.Empty;
            var qty = string.Empty;

            var data = (new ExcelReader()).ReadExcel(upload);
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
                    item.IncidentDate = DateTime.Parse(dataRow[5].ToString());
                    item.IncidentLocation = dataRow[6].ToString();
                    item.IncidentDescription = dataRow[7].ToString();
                    
                    model.Add(item);
                }
            }
            return Json(model);
        }

    }
}
