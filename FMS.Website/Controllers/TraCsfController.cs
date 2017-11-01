using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.Contract.BLL;
using FMS.Core;

namespace FMS.Website.Controllers
{
    public class TraCsfController : BaseController
    {
        //
        // GET: /TraCsf/

        private IEpafBLL _epafBLL;

        public TraCsfController(IPageBLL pageBll, IEpafBLL epafBll)
            : base(pageBll, Core.Enums.MenuList.TraCsf)
        {
            _epafBLL = epafBll;

        }

        public ActionResult Index()
        {
            return View();
        }



    }
}
