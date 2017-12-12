using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;
using FMS.BusinessObject.Inputs;
using AutoMapper;

namespace FMS.Website.Controllers
{
    public class RptCfmIdleController :BaseController
    {
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private ISettingBLL _settingBLL;
        private ICfmIdleReportBLL _cfmIdleReportBLL;

        public RptCfmIdleController(IPageBLL pageBll, ICfmIdleReportBLL CfmIdleReportBLL, ISettingBLL SettingBLL)
            : base(pageBll, Core.Enums.MenuList.RptCfmIdle)
        {
            _pageBLL = pageBll;
            _cfmIdleReportBLL = CfmIdleReportBLL;
            _settingBLL = SettingBLL;
            _mainMenu = Enums.MenuList.RptCfmIdle;
        }

        public ActionResult Index()
        {
            var model = new CfmIdleReportModel();
            model.MainMenu = Enums.MenuList.RptExecutiveSummary;
            model.CurrentLogin = CurrentUser;
            model.SearchView.FromDate = DateTime.Today;
            model.SearchView.ToDate = DateTime.Today;
            var filter = new CfmIdleGetByParamInput();
            filter.FromDate = null;
            filter.ToDate = DateTime.Today;

            var data = _cfmIdleReportBLL.GetCfmIdle(filter);
            var ListData = Mapper.Map<List<CfmIdleVehicle>>(data);
            model.ListCfmIdle = ListData;
            foreach (var item in model.ListCfmIdle)
            {

                var StartIdle = (decimal)(item.StartIdle.Value.Date - new DateTime(1900, 1, 1)).TotalDays - 1;
                var EndIdle = (decimal)(item.EndIdle.Value.Date - new DateTime(1900, 1, 1)).TotalDays + 1;

                item.IdleDuration = Math.Round((decimal)(EndIdle - StartIdle)/30,2);

                item.TotalMonthly =Math.Round((decimal)(item.IdleDuration * item.MonthlyInstallment), 2);

            }
            return View(model);
        }

        [HttpPost]
        public PartialViewResult ListCfmIdleVehicle(CfmIdleReportModel model)
        {
            model.ListCfmIdle = GetVehicleData(model.SearchView);
            foreach (var item in model.ListCfmIdle)
            {

                var StartIdle = (decimal)(item.StartIdle.Value.Date - new DateTime(1900, 1, 1)).TotalDays - 1;
                
                var EndIdle = (decimal)(item.EndIdle.Value.Date - new DateTime(1900, 1, 1)).TotalDays + 1;

                item.IdleDuration = Math.Round((decimal)(EndIdle - StartIdle) / 30, 2);

                item.TotalMonthly = Math.Round((decimal)(item.IdleDuration * item.MonthlyInstallment), 2);

            }
            return PartialView("_ListCfmIdleVehicle", model);
        }

        private List<CfmIdleVehicle> GetVehicleData(CfmIdleSearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _cfmIdleReportBLL.GetCfmIdle(new CfmIdleGetByParamInput());
                return Mapper.Map<List<CfmIdleVehicle>>(data);
            }

            //getbyparams
            var input = Mapper.Map<CfmIdleGetByParamInput>(filter);

            var dbData = _cfmIdleReportBLL.GetCfmIdle(input);
            return Mapper.Map<List<CfmIdleVehicle>>(dbData);
        }
    }
}
