using FMS.BusinessObject.Dto;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;
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

        //
        // GET: /TraCaf/

        public ActionResult Index()
        {
            var model = new TraCrfDashboardViewModel();
            return View();
        }

    }
}
