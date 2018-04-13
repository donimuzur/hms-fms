using FMS.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Controllers
{
    public class EmailApprovalsController : Controller
    {
        //
        // GET: /EmailApproval/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult CTF()
        {
            
            var id = Request.Form["id"];
            var email = Request.Form["email"];

            var Data = new EmailApprovals();
            try
            {
                Data.Status = "Success";
                Data.Messages = string.Empty;
            }
            catch (Exception e)
            {
                Data.Status = "Failed";
                Data.Messages = e.Message;
            }

            return Json(Data, JsonRequestBehavior.AllowGet);

        }
    }
}
