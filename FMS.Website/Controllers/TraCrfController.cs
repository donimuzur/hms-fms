using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.Contract.BLL;
using FMS.Core;

namespace FMS.Website.Controllers
{
    public class TraCrfController : BaseController
    {
        //
        // GET: /TraCrf/
        
        private IEpafBLL _epafBLL;

        public TraCrfController(IPageBLL pageBll,IEpafBLL epafBll) : base(pageBll, Core.Enums.MenuList.TraCrf)
        {
            _epafBLL = epafBll;
            
        }

        public ActionResult Index()
        {
            return View();
        }

        

    }
}
