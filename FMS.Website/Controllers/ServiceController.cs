using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.BLL.Crf;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.DAL;

namespace FMS.Website.Controllers
{
    public class ServiceController : Controller
    {
        //
        // GET: /Service/

        public ActionResult CompleteTransaction()
        {
            IUnitOfWork uow = new SqlUnitOfWork();
            ITraCrfBLL crfBll = new CrfBLL(uow);

            //CRF Complete
            crfBll.CompleteAllDocument();
            
            return View();
        }

    }
}
