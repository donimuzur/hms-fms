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
    public class ChangesLogController : BaseController
    {
        
        //
        // POST: /Changes/
        public ChangesLogController(IPageBLL pageBll, Enums.MenuList menuID) : base(pageBll, menuID)
        {

        }

        [HttpPost]
        public ActionResult Get(int ModulId, long FormId)
        {
            var data = GetChangesHistory(ModulId, FormId);
            var model = new BaseModel();
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = Mapper.Map<List<ChangesLogs>>(data);
            return View("_ChangesLog", model);
        }

    }
}
