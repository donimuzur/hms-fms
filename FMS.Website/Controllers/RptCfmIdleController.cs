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
        
        private IPageBLL _pageBLL;
        private ISettingBLL _settingBLL;
        private ICfmIdleReportBLL _cfmIdleReportBLL;
        private Enums.MenuList _mainMenu;

        public RptCfmIdleController(IPageBLL pageBll, ICfmIdleReportBLL CfmIdleReportBLL, ISettingBLL SettingBLL)
            : base(pageBll, Core.Enums.MenuList.RptCfmIdle)
        {
            _pageBLL = pageBll;
            _cfmIdleReportBLL = CfmIdleReportBLL;
            _settingBLL = SettingBLL;
            _mainMenu = Enums.MenuList.RptExecutiveSummary;
        }

        public ActionResult Index()
        {
            var model = new CfmIdleReportModel();
            model.MainMenu = _mainMenu;
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
                var CfmIdleVehicle = new CfmIdleVehicle();

                var StartIdle = (decimal)(item.StartIdle.Value.Date - new DateTime(1900, 1, 1)).TotalDays - 1;

                var today = DateTime.Today;
                var EndIdle = (decimal)(today - new DateTime(1900, 1, 1)).TotalDays + 1;

                if (item.EndIdle.HasValue)
                {
                    EndIdle = (decimal)(item.EndIdle.Value.Date - new DateTime(1900, 1, 1)).TotalDays + 1;
                }

                item.IdleDuration = Math.Round((decimal)(EndIdle - StartIdle) / 30, 2);

                item.TotalMonthly = Math.Round((decimal)(item.IdleDuration * item.MonthlyInstallment), 2);

            }

            var GrandTotal = new CfmIdleVehicle
            {
                PoliceNumber = "ZZZZZ",
                Note = "GrandTotal",
                CreatedDate = DateTime.Today,
                IdleDuration = model.ListCfmIdle.Sum(x => x.IdleDuration),
                TotalMonthly = model.ListCfmIdle.Sum(x => x.TotalMonthly)
            };

            if (model.ListCfmIdle != null)
            {
                var result = model.ListCfmIdle.GroupBy(x => x.PoliceNumber).Select(grouping => new CfmIdleVehicle
                {
                    PoliceNumber = grouping.First().PoliceNumber,
                    Note = "SubTotal",
                    CreatedDate = DateTime.Today,
                    IdleDuration = grouping.Sum(x => x.IdleDuration),
                    TotalMonthly = grouping.Sum(x => x.TotalMonthly)
                }).ToList();
                foreach (var itemResult in result)
                {
                    model.ListCfmIdle.Add(itemResult);
                }
            }
            model.ListCfmIdle= model.ListCfmIdle.OrderBy(x => x.PoliceNumber).ToList();

            if (model.ListCfmIdle != null) model.ListCfmIdle.Add(GrandTotal);

            return View(model);
        }

        [HttpPost]
        public PartialViewResult ListCfmIdleVehicle(CfmIdleReportModel model)
        {
            model.ListCfmIdle = GetVehicleData(model.SearchView);
            foreach (var item in model.ListCfmIdle)
            {
                var CfmIdleVehicle = new CfmIdleVehicle();
                
                var StartIdle = (decimal)(item.StartIdle.Value.Date - new DateTime(1900, 1, 1)).TotalDays - 1;

                var today = DateTime.Today;
                var EndIdle = (decimal)(today - new DateTime(1900, 1, 1)).TotalDays + 1;

                if (item.EndIdle.HasValue)
                {
                    EndIdle = (decimal)(item.EndIdle.Value.Date - new DateTime(1900, 1, 1)).TotalDays + 1;
                }

                item.IdleDuration = Math.Round((decimal)(EndIdle - StartIdle) / 30, 2);

                item.TotalMonthly = Math.Round((decimal)(item.IdleDuration * item.MonthlyInstallment), 2);
                
            }

            var GrandTotal = new CfmIdleVehicle
            {
                PoliceNumber = "ZZZZZ",
                Note = "GrandTotal",
                CreatedDate = DateTime.Today,
                IdleDuration = model.ListCfmIdle.Sum(x => x.IdleDuration),
                TotalMonthly = model.ListCfmIdle.Sum(x => x.TotalMonthly)
            };

            if (model.ListCfmIdle != null)
            {
                var result = model.ListCfmIdle.GroupBy(x => x.PoliceNumber).Select(grouping => new CfmIdleVehicle
                {
                    PoliceNumber = grouping.First().PoliceNumber,
                    Note = "SubTotal",
                    CreatedDate = DateTime.Today,
                    IdleDuration = grouping.Sum(x => x.IdleDuration),
                    TotalMonthly = grouping.Sum(x => x.TotalMonthly)
                }).ToList();
                foreach (var itemResult in result)
                {
                    model.ListCfmIdle.Add(itemResult);
                }
            }
            model.ListCfmIdle = model.ListCfmIdle.OrderBy(x => x.PoliceNumber).ToList();

            if (model.ListCfmIdle != null) model.ListCfmIdle.Add(GrandTotal);

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
