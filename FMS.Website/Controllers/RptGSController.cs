using AutoMapper;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
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
    public class RptGSController : BaseController
    {
        #region -------------- field and Cunstructor ------------------
        private IGsBLL _gsBLL;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private IRemarkBLL _remarkBLL;
        private IFleetBLL _fleetBLL;
        private IEmployeeBLL _employeeBLL;
        private ILocationMappingBLL _locationMappingBLL;
        private ISettingBLL _settingBLL;
        public RptGSController(IPageBLL pageBll, IGsBLL gsBLL, ISettingBLL settingBLL, IRemarkBLL RemarkBLL, IFleetBLL FleetBLL, IEmployeeBLL EmployeeBLL, ILocationMappingBLL LocationMapping) : base(pageBll, Enums.MenuList.MasterGS)
        {
            _gsBLL = gsBLL;
            _pageBLL = pageBll;
            _remarkBLL = RemarkBLL;
            _fleetBLL = FleetBLL;
            _employeeBLL = EmployeeBLL;
            _locationMappingBLL = LocationMapping;
            _settingBLL = settingBLL;
            _mainMenu = Enums.MenuList.RptExecutiveSummary;
        }
        #endregion
        //
        // GET: /RptGS/

        public ActionResult Index()
        {
            var model = new GsModel();
            var data = _gsBLL.GetGsReport(new RptGsInput());
            model.Details = Mapper.Map<List<GsItem>>(data);
            model.MainMenu = Enums.MenuList.RptGs;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;


            var settingList = _settingBLL.GetSetting().Where(x => x.SettingGroup.StartsWith("VEHICLE_USAGE")).Select(x => new { x.SettingName, x.SettingValue }).ToList();
            model.VehicleUsageList = new SelectList(settingList, "SettingName", "SettingValue");
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(GsModel model)
        {
            //var data = _gsBLL.GetGs();
            //model.Details = Mapper.Map<List<GsItem>>(data);
            model.MainMenu = Enums.MenuList.RptGs;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;

            List<GsDto> data = _gsBLL.GetGsReport(new RptGsInput()
            {
                StartDateBegin = model.FilterReport.StartDateBegin,
                EndDateBegin = model.FilterReport.EndDateBegin,
                StartDateEnd = model.FilterReport.StartDateEnd,
                EndDateEnd = model.FilterReport.EndDateEnd,
                Location = model.FilterReport.Location,
                VehicleUsage = model.FilterReport.VehicleUsage
            });

            model.Details = Mapper.Map<List<GsItem>>(data);
            var settingList = _settingBLL.GetSetting().Where(x => x.SettingGroup.StartsWith("VEHICLE_USAGE")).Select(x => new { x.SettingName, x.SettingValue }).ToList();
            model.VehicleUsageList = new SelectList(settingList, "SettingName", "SettingValue");
            return View(model);
        }

    }
}
