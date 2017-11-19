using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;
using AutoMapper;

namespace FMS.Website.Controllers
{
    public class TraTemporaryController : BaseController
    {
        #region --------- Field and Constructor --------------

        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private IEmployeeBLL _employeeBLL;
        private IReasonBLL _reasonBLL;
        private ITraTemporaryBLL _tempBLL;

        public TraTemporaryController(IPageBLL pageBll, IEmployeeBLL EmployeeBLL, IReasonBLL ReasonBLL, ITraTemporaryBLL TempBLL)
            : base(pageBll, Core.Enums.MenuList.TraTmp)
        {
            _pageBLL = pageBll;
            _employeeBLL = EmployeeBLL;
            _reasonBLL = ReasonBLL;
            _tempBLL = TempBLL;
            _mainMenu = Enums.MenuList.Transaction;
        }

        #endregion

        #region --------- Open Document --------------

        public ActionResult Index()
        {
            var data = _tempBLL.GetTemporary(CurrentUser, false);
            var model = new TemporaryIndexModel();
            model.TitleForm = "Temporary Open Document";
            model.TitleExport = "ExportOpen";
            model.TempList = Mapper.Map<List<TempData>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        #endregion

        #region --------- Completed Document --------------

        public ActionResult Completed()
        {
            var data = _tempBLL.GetTemporary(CurrentUser, true);
            var model = new TemporaryIndexModel();
            model.TitleForm = "Temporary Completed Document";
            model.TitleExport = "ExportCompleted";
            model.TempList = Mapper.Map<List<TempData>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.IsCompleted = true;
            return View("Index", model);
        }

        #endregion

        #region --------- Personal Dashboard --------------

        public ActionResult PersonalDashboard()
        {
            var data = _tempBLL.GetTempPersonal(CurrentUser);
            var model = new TemporaryIndexModel();
            model.TitleForm = "Temporary Personal Dashboard";
            model.TitleExport = "ExportOpen";
            model.TempList = Mapper.Map<List<TempData>>(data);
            model.MainMenu = Enums.MenuList.PersonalDashboard;
            model.CurrentLogin = CurrentUser;
            model.IsPersonalDashboard = true;
            return View("Index", model);
        }

        #endregion
    }
}
