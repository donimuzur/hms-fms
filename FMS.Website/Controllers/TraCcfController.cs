using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Core.Exceptions;
using FMS.Website.Models;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Controllers
{
    public class TraCcfController : BaseController
    {

        #region --------- Field & Constructor--------------
        private IEpafBLL _epafBLL;
        private ITraCcfBLL _ccfBLL;
        private IRemarkBLL _remarkBLL;
        private IFleetBLL _fleetBLL;
        private IEmployeeBLL _employeeBLL;
        private IComplaintCategoryBLL _complaintCategoryBLL;
        private IReasonBLL _reasonBLL;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private ILocationMappingBLL _locationMappingBLL;
        public TraCcfController(IPageBLL pageBll, IEpafBLL epafBll, ITraCcfBLL ccfBll, IRemarkBLL RemarkBLL,
                                IEmployeeBLL EmployeeBLL, IReasonBLL ReasonBLL, IFleetBLL FleetBLL, IComplaintCategoryBLL complaintCategoryBLL,
                                ILocationMappingBLL LocationMappingBLL): base(pageBll, Core.Enums.MenuList.TraCtf)
        {

            _epafBLL = epafBll;
            _ccfBLL = ccfBll;
            _employeeBLL = EmployeeBLL;
            _complaintCategoryBLL = complaintCategoryBLL;
            _pageBLL = pageBll;
            _remarkBLL = RemarkBLL;
            _reasonBLL = ReasonBLL;
            _fleetBLL = FleetBLL;
            _locationMappingBLL = LocationMappingBLL;
            _mainMenu = Enums.MenuList.Transaction;
        }
        #endregion

        #region --------- Open Document--------------
        public ActionResult Index()
        {
            var data = _ccfBLL.GetCcf();
            var model = new CcfModel();
            model.Details = Mapper.Map<List<CcfItem>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;

            if (CurrentUser.EMPLOYEE_ID == "")
            {
                return RedirectToAction("Unauthorized", "Error");
            }
            else
            {
                return View(model);
            }
        }

        public ActionResult DashboardHR()
        {
            var model = new CcfModel();
            var data = _ccfBLL.GetCcf();
            if (CurrentUser.UserRole == Enums.UserRole.HR)
            {
                return RedirectToAction("Index", "traCcf");
            }
            model.Details = Mapper.Map<List<CcfItem>>(data.Where(x => x.DocumentStatus != Enums.DocumentStatus.AssignedForHR));
            model.TitleForm = "Open Document HR";
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }
        #endregion

    }
}
