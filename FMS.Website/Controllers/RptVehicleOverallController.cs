using FMS.Contract.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.Core;
using FMS.Website.Models;
using FMS.BusinessObject.Inputs;
using AutoMapper;
using FMS.Utils;

namespace FMS.Website.Controllers
{
    public class RptVehicleOverallController : BaseController
    {
        private IVehicleOverallReportBLL _vehicleOverallReportBLL;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private ISettingBLL _settingBLL;
        private ILocationMappingBLL _locationMappingBLL;
        private IVendorBLL _vendorBLL;

        public RptVehicleOverallController(IPageBLL pageBll,  IVehicleOverallReportBLL VehicleOverallReportBLL, ISettingBLL SettingBLL, IVendorBLL VendorBLL, ILocationMappingBLL LocationMappingBLL) : base(pageBll, Enums.MenuList.RptVehicle)
        {
            _pageBLL = pageBll;
            _vehicleOverallReportBLL = VehicleOverallReportBLL;
            _settingBLL = SettingBLL;
            _locationMappingBLL = LocationMappingBLL;
            _vendorBLL = VendorBLL;
            _mainMenu = Enums.MenuList.RptExecutiveSummary ;
        }

        public VehicleOverallReportModel Initial(VehicleOverallReportModel model)
        {
            var settingData = _settingBLL.GetSetting();
            var listStatus = new Dictionary<bool, string> { { true ,"Active" }, {false,"InActive" } };
            var listVehType = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.IsActive).Select(x => new { x.SettingValue }).ToList();
            var listSupMethod = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.SupplyMethod) && x.IsActive).Select(x => new { x.SettingValue }).ToList();
            var listRegional = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();
            var listBodyType = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.BodyType) && x.IsActive).Select(x => new { x.SettingValue }).ToList();
            var listVendor = _vendorBLL.GetVendor().Where(x => x.IsActive).Select(x => new { x.VendorName }).Distinct().ToList();
            var listCity = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Location }).Distinct().ToList();

            model.SearchView.StatusList = new SelectList(listStatus, "Key", "Value");
            model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
            model.SearchView.SupplyMethodList = new SelectList(listSupMethod, "SettingValue", "SettingValue");
            model.SearchView.RegionalList = new SelectList(listRegional, "Region", "Region");
            model.SearchView.BodyTypeList = new SelectList(listBodyType, "SettingValue", "SettingValue"); ;
            model.SearchView.VendorList = new SelectList(listVendor, "VendorName", "VendorName");
            model.SearchView.CityList = new SelectList(listCity, "Location", "Location");
            model.SearchView.FromDate  = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            model.SearchView.ToDate = DateTime.Today;
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;

            return model;
        }

        public ActionResult Index()
        {
            var model = new VehicleOverallReportModel();
            model = Initial(model);

            var filter = new VehicleOverallReportGetByParamInput();
            filter.FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            filter.ToDate = DateTime.Today;

            var data = _vehicleOverallReportBLL.GetVehicle(filter);
            var ListData = Mapper.Map<List<VehicleOverallItem>>(data);
            model.ListVehicle = ListData;
            return View(model);
        }
        
        public ActionResult DetailsVehicle(string id,VehicleOverallSearchView filter)
        {
            var model = GetVehicleData(filter).Where(x => x.ChasisNumber ==id).FirstOrDefault();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        [HttpPost]
        public PartialViewResult ListVehicle(VehicleOverallReportModel model)
        {
            model.ListVehicle = GetVehicleData(model.SearchView);
            return PartialView("_ListVehicleOverall", model);
        }
        private List<VehicleOverallItem> GetVehicleData(VehicleOverallSearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _vehicleOverallReportBLL.GetVehicle(new VehicleOverallReportGetByParamInput());
                return Mapper.Map<List<VehicleOverallItem>>(data);
            }

            //getbyparams
            var input = Mapper.Map<VehicleOverallReportGetByParamInput>(filter);

            var dbData = _vehicleOverallReportBLL.GetVehicle(input);
            return Mapper.Map<List<VehicleOverallItem>>(dbData);
        }
    }
}
