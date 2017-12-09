using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.Contract.BLL;
using FMS.Core;

namespace FMS.Website.Controllers
{
    public class RptAutoGrController : BaseController
    {

        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private IAutoGrBLL _autoGrBLL;
        //
        // GET: /RptAutoGr/

        public RptAutoGrController(IPageBLL pageBll,IAutoGrBLL autoGrBLL) : base(pageBll, Enums.MenuList.RptAutoGr)
        {
            _pageBLL = pageBll;
            _autoGrBLL = autoGrBLL;
            _mainMenu = Enums.MenuList.RptAutoGr;
        }

        public ActionResult Index()
        {
            
            return View();
        }

    }
}
