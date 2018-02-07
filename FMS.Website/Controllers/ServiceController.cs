using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.BLL.Crf;
using FMS.BLL.Csf;
using FMS.BLL.Ctf;
using FMS.BLL.Temporary;
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
            ITraCtfBLL ctfBLL = new CtfBLL(uow);
            ITraCsfBLL csfBll = new CsfBLL(uow);
            ITraTemporaryBLL tempBll = new TemporaryBLL(uow);

            //CRF Complete
            var errorMessage = crfBll.CompleteAllDocument();
            
            //CTF Complete
            ctfBLL.CheckCtfInProgress();

            //CSF Complete
            csfBll.CheckCsfInProgress();

            //Temporary Complete
            tempBll.CheckTempInProgress();

            return View();
        }

        public ActionResult EmailBatchToVendor()
        {
            //new TraCsfController().GetListCsfInProgress();
            return View();  
        }
    }
}
