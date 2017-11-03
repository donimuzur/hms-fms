using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Controllers
{
    public class TraCtfController : BaseController
    {
        private IEpafBLL _epafBLL;
        private ITraCtfBLL _ctfBLL;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        public TraCtfController(IPageBLL pageBll, IEpafBLL epafBll, ITraCtfBLL ctfBll): base(pageBll, Core.Enums.MenuList.TraCtf)
        {
            _epafBLL = epafBll;
            _ctfBLL = ctfBll;
            _pageBLL = pageBll;
            _mainMenu = Enums.MenuList.Transaction;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dashoard()
        {
            var EpafData= _epafBLL.GetEpaf().Where(x => x.DocumentType == 6).ToList();
            var model = new CtfModel();
            foreach(var data in EpafData)
            {
            }
            return View(); 
        }
    }
}
