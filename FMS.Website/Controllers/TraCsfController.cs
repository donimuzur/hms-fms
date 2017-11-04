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
    public class TraCsfController : BaseController
    {

        #region --------- Field and Constructor --------------

        private IEpafBLL _epafBLL;
        private ITraCsfBLL _csfBLL;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;

        public TraCsfController(IPageBLL pageBll, IEpafBLL epafBll, ITraCsfBLL csfBll)
            : base(pageBll, Core.Enums.MenuList.TraCsf)
        {
            _epafBLL = epafBll;
            _csfBLL = csfBll;
            _pageBLL = pageBll;
            _mainMenu = Enums.MenuList.Transaction;
        }

        #endregion

        #region --------- Open Document --------------

        public ActionResult Index()
        {
            var data = _csfBLL.GetCsf();
            var model = new CsfIndexModel();
            model.TitleForm = "CSF Open Document";
            model.CsfList = Mapper.Map<List<CsfData>>(data);
            model.MainMenu = _mainMenu;
            return View(model);
        }

        #endregion

        #region --------- Dashboard --------------

        public ActionResult Dashboard()
        {
            var data = _epafBLL.GetEpafByDocType(Enums.DocumentType.CSF);
            var model = new CsfDashboardModel();
            model.TitleForm = "CSF Dashboard";
            model.EpafList = Mapper.Map<List<EpafData>>(data);
            model.MainMenu = _mainMenu;
            return View(model);
        }

        #endregion

        #region --------- Completed Document --------------

        public ActionResult Completed()
        {
            var data = _csfBLL.GetCsf();
            var model = new CsfIndexModel();
            model.TitleForm = "CSF Completed Document";
            model.CsfList = Mapper.Map<List<CsfData>>(data);
            model.MainMenu = _mainMenu;
            model.IsCompleted = true;
            return View("Index", model);
        }

        #endregion

        #region --------- Create --------------

        public ActionResult Create()
        {
            var model = new CsfItemViewModel();
            model.MainMenu = _mainMenu;

            return View(model);
        }

        #endregion
    }
}
