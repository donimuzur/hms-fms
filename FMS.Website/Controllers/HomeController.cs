using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;

namespace FMS.Website.Controllers
{
    public class HomeController : BaseController
    {
        private IPageBLL _pageBLL;
        private Enums.MenuList _mainMenu;

        public HomeController(IPageBLL pageBll)
            : base(pageBll, Core.Enums.MenuList.Home)
        {
            _pageBLL = pageBll;
            _mainMenu = Enums.MenuList.Home;
        }

        public ActionResult Index()
        {
            var model = new BaseModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;

            return View(model);
        }
    }
}
