using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using FMS.BLL.Vendor;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Utils;
using FMS.Website.Models;
using FMS.Website.Utility;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Controllers
{
    public class MstFleetDashboardController : BaseController
    {
        private IFleetBLL _fleetBLL;
        private IPageBLL _pageBLL;
        private Enums.MenuList _mainMenu;

        public MstFleetDashboardController(IPageBLL PageBll, IFleetBLL FleetBLL)
            : base(PageBll, Enums.MenuList.DashboardFleet)
        {
            _fleetBLL = FleetBLL;
            _pageBLL = PageBll;
            _mainMenu = Enums.MenuList.MasterData;
        }

        #region List View
        public ActionResult Index()
        {
            var model = new FleetDashboardModel();

            model.SearchView = new FleetDashboardSearchView();

            var fleetChangeList = _fleetBLL.GetFleetChange().ToList();

            model.SearchView.PoliceNumberList = new SelectList(fleetChangeList.Select(x => new { x.PoliceNumber }).Distinct().ToList(), "PoliceNumber", "PoliceNumber");
            model.SearchView.EmployeeNameList = new SelectList(fleetChangeList.Select(x => new { x.EmployeeName }).Distinct().ToList(), "EmployeeName", "EmployeeName");
            model.SearchView.ChasisNumberList = new SelectList(fleetChangeList.Select(x => new { x.ChasisNumber }).Distinct().ToList(), "ChasisNumber", "ChasisNumber");
            model.SearchView.EmployeeIDList = new SelectList(fleetChangeList.Select(x => new { x.EmployeeId }).Distinct().ToList(), "EmployeeId", "EmployeeId");
            
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            model.WriteAccess = CurrentPageAccess.WriteAccess == true ? 1 : 0;
            model.ReadAccess = CurrentPageAccess.ReadAccess == true ? 1 : 0;

            return View("Index", model);
        }

        [HttpPost]
        public JsonResult SearchFleetChangeAjax(DTParameters<FleetDashboardModel> param)
        {
            var model = param;

            var data = model != null ? SearchDataFleet(model) : SearchDataFleet();
            DTResult<FleetDashboardItem> result = new DTResult<FleetDashboardItem>();
            result.draw = param.Draw;
            result.recordsFiltered = data.Count;
            result.recordsTotal = data.Count;
            
            result.data = data;

            return Json(result);
        }

        private List<FleetDashboardItem> SearchDataFleet(DTParameters<FleetDashboardModel> searchView = null)
        {
            var param = new FleetChangeParamInput();
            param.EmployeeId = searchView.EmployeeId;
            param.FormalName = searchView.FormalName;
            param.PoliceNumber = searchView.PoliceNumber;

            var data = _fleetBLL.GetFleetChangeByParam(param);
            return Mapper.Map<List<FleetDashboardItem>>(data);
        }

        private List<FleetDashboardItem> SearchDataFleetExport(FleetDashboardSearchView searchView = null)
        {
            var param = new FleetChangeParamInput();
            param.EmployeeId = searchView.EmployeeId;
            param.FormalName = searchView.EmployeeName;
            param.PoliceNumber = searchView.PoliceNumber;

            var data = _fleetBLL.GetFleetChangeByParam(param);
            return Mapper.Map<List<FleetDashboardItem>>(data);
        }

        #endregion
    }
}
