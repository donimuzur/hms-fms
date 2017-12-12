using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Utils;
using FMS.Website.Models;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;

namespace FMS.Website.Controllers
{
    public class RptFuelController : BaseController
    {
        #region --------- Field and Constructor --------------

        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private IRptFuelBLL _rptFuelBLL;
        private ISettingBLL _settingBLL;
        private IFleetBLL _fleetBLL;

        public RptFuelController(IPageBLL pageBll, IRptFuelBLL rptFuelBLL, ISettingBLL SettingBLL, IFleetBLL fleetBLL) 
            : base(pageBll, Core.Enums.MenuList.RptFuel)
        {
            _pageBLL = pageBll;
            _rptFuelBLL = rptFuelBLL;
            _settingBLL = SettingBLL;
            _fleetBLL = fleetBLL;
            _mainMenu = Enums.MenuList.RptFuel;
        }

        #endregion

        public ActionResult Index()
        {
            try
            {
                var model = new RptFuelModel();
                model.MainMenu = Enums.MenuList.RptExecutiveSummary;
                model.TitleForm = "Fuel Report";
                model.CurrentLogin = CurrentUser;
                var settingData = _settingBLL.GetSetting();
                var listVehType = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.IsActive).Select(x => new { x.SettingValue }).ToList();
                var costCenter = _fleetBLL.GetFleet();

                model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
                return View(model);
            }
            catch (Exception exception)
            {
                var model = new RptFuelModel();
                model.MainMenu = Enums.MenuList.RptExecutiveSummary;
                model.TitleForm = "Fuel Report";
                model.CurrentLogin = CurrentUser;
                model.ErrorMessage = exception.Message;
                return View(model);
            }
        }

    }
}
