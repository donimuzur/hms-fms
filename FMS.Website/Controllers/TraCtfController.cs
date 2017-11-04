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
            var data = _epafBLL.GetEpaf().Where(x => x.DocumentType == 1);
            var model = new CsfIndexModel();
            //model.TitleForm = "CSF Open Document";
            //model.EpafList = Mapper.Map<List<EpafData>>(data);
            model.MainMenu = _mainMenu;
            return View(model);
        }

        public ActionResult Dashboard()
        {
            var EpafData = _epafBLL.GetEpafByDocType(Enums.DocumentType.CTF).ToList();
            var model = new CtfModel();
            foreach (var data in EpafData)
            {
                var item = new CtfItem();
                item.EPafData = data;
                model.Details.Add(item);
            }
            model.TitleForm = "CTF Dashboard"; 
            model.MainMenu = _mainMenu;
            return View(model);
        }

        public ActionResult Completed()
        {
            var data = _epafBLL.GetEpaf().Where(x => x.DocumentType == 1);
            var model = new CsfIndexModel();
            //model.TitleForm = "CSF Completed Document";
            //model.EpafList = Mapper.Map<List<EpafData>>(data);
            model.MainMenu = _mainMenu;
            return View("Index", model);
        }

    }
}
