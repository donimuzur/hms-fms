using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;
using AutoMapper;

namespace FMS.Website.Controllers
{
    public class MstGsController : BaseController
    {
        private IGsBLL _gsBLL;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        public MstGsController(IPageBLL pageBll, IGsBLL gsBLL) : base(pageBll, Enums.MenuList.MasterGS)
        {
            _gsBLL = gsBLL;
            _pageBLL = pageBll;
            _mainMenu = Enums.MenuList.MasterData;
        }

        //
        // GET: /MstGs/

        public ActionResult Index()
        {
            var model = new GsModel();
            var data  = _gsBLL.GetGs();
            model.Details = Mapper.Map<List<GsItem>>(data);
            return View(model);
        }

    }
}
