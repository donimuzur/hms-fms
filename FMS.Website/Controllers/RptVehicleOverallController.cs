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

namespace FMS.Website.Controllers
{
    public class RptVehicleOverallController : BaseController
    {
        private IVehicleOverallReportBLL _vehicleOverallReportBLL;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private ISettingBLL _settingBLL;

        public RptVehicleOverallController(IPageBLL pageBll,  IVehicleOverallReportBLL VehicleOverallReportBLL, ISettingBLL SettingBLL) : base(pageBll, Enums.MenuList.RptVehicle)
        {
            _pageBLL = pageBll;
            _vehicleOverallReportBLL = VehicleOverallReportBLL;
            _settingBLL = SettingBLL;
            _mainMenu = Enums.MenuList.RptExecutiveSummary ;
        }

        public ActionResult Index()
        {
            var model = new VehicleOverallReportModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.SearchView.FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            model.SearchView.ToDate = DateTime.Today;

            var filter = new VehicleOverallReportGetByParamInput();
            filter.FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            filter.ToDate = DateTime.Today;

            var data = _vehicleOverallReportBLL.GetVehicle(filter);
            var ListData = Mapper.Map<List<VehicleOverallItem>>(data);
            model.ListVehicle = ListData;
            return View(model);
        }


        public ActionResult DetailsVehicle(VehicleOverallReportModel filter)
        {
            var model = ListVehicle(filter);
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
