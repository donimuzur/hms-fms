using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;

namespace FMS.Website.Controllers
{
    public class TraCrfController : BaseController
    {
        //
        // GET: /TraCrf/
        private Enums.MenuList _mainMenu;
        
        private IEpafBLL _epafBLL;

        public TraCrfController(IPageBLL pageBll,IEpafBLL epafBll) : base(pageBll, Core.Enums.MenuList.TraCrf)
        {
            _epafBLL = epafBll;
            _mainMenu = Enums.MenuList.TraCrf;
        }

        public ActionResult Index()
        {
            var model = new TraCrfIndexViewModel();
            model.MainMenu = _mainMenu;
            
            return View(model);
        }

        public ActionResult Dashboard()
        {
            var model = new EpafModel();
            model.MainMenu = _mainMenu;
            var data = _epafBLL.GetEpaf();
            model.Details = Mapper.Map<List<EpafItem>>(data);
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new TraCrfItemViewModel();
            model.MainMenu = _mainMenu;

            return View(model);
        }
    }
}
