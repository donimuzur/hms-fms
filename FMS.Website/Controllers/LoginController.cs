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
    public class LoginController : BaseController
    {
        private IEmployeeBLL _employeeBLL;
        
        public LoginController(IEmployeeBLL employeeBLL, IPageBLL pageBll)
            : base(pageBll, Enums.MenuList.Login)
        {
            _employeeBLL = employeeBLL;
            
        }

        //
        // GET: /Login/

        public ActionResult Index()
        {
            var model = new LoginFormModel();
            model.Users = new SelectList(_employeeBLL.GetUsers(), "USER_ID", "USER_ID");
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(LoginFormModel model)
        {

            var loginResult = _employeeBLL.VerifyLogin(model.Login.UserId);

            if (loginResult != null)
            {
                CurrentUser = loginResult;
                


                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Unauthorized", "Error");

        }

        //public ActionResult MessageInfo()
        //{
        //    var model = GetListMessageInfo();
        //    return PartialView("_MessageInfo", model);
        //}

    }
}
