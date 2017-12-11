using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.Contract.BLL;
using FMS.Core;

namespace FMS.Website.Controllers
{
    public class RptCfmIdleController :BaseController
    {
        public RptCfmIdleController(IPageBLL pageBll, Enums.MenuList menuID) : base(pageBll, menuID)
        {

        }

        //
        // GET: /RptCfmIdle/

        public ActionResult Index()
        {
            return View();
        }

    }
}
