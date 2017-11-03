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

        public ActionResult Index()
        {
            var data = _epafBLL.GetEpaf().Where(x => x.DocumentType == 1);
            var model = new CsfModel();
            model.TitleForm = "CSF Open Document";
            model.EpafList = Mapper.Map<List<EpafData>>(data);
            model.MainMenu = _mainMenu;
            return View(model);
        }

        public ActionResult Dashboard()
        {
            var data = _epafBLL.GetEpafByDocType(Enums.DocumentType.CSF);
            var model = new CsfModel();
            model.TitleForm = "CSF Dashboard";
            model.EpafList = Mapper.Map<List<EpafData>>(data);
            model.MainMenu = _mainMenu;
            return View(model);
        }

        public ActionResult Completed()
        {
            var data = _epafBLL.GetEpaf().Where(x => x.DocumentType == 1);
            var model = new CsfModel();
            model.TitleForm = "CSF Completed Document";
            model.EpafList = Mapper.Map<List<EpafData>>(data);
            model.MainMenu = _mainMenu;
            return View("Index", model);
        }

    }
}
