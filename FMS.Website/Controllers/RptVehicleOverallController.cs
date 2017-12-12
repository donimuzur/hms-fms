﻿using FMS.Contract.BLL;
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
            var filter = new VehicleOverallReportGetByParamInput();

            var data = _vehicleOverallReportBLL.GetVehicle(filter);
            var ListData = Mapper.Map<List<VehicleOverallItem>>(data);
            model.ListVehicle = ListData;
            return View(model);
        }

        public ActionResult DetailsVehicle()
        {
            var model = new VehicleOverallItem();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }
    
    }
}