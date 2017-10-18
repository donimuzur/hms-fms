using AutoMapper;
using FMS.BusinessObject;
using FMS.Contract.BLL;
using FMS.BusinessObject.Dto;
using FMS.Core;
using FMS.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;
using FMS.Website.Utility;

namespace FMS.Website.Controllers
{
    public class MstPenaltyController : BaseController
    {
        private IPenaltyBLL _penaltyBLL;
        private Enums.MenuList _mainMenu;
        //
        // GET: /MstPenalty/

        public MstPenaltyController(IPageBLL pageBll, IPenaltyBLL penaltyBLL) : base(pageBll, Enums.MenuList.MasterPenalty)
        {
            _penaltyBLL = penaltyBLL;
            _mainMenu = Enums.MenuList.MasterPenalty;
        }
        public ActionResult Index()
        {
            var data = _penaltyBLL.GetPenalty();

            var model = new PenaltyModel();
            model.Details = Mapper.Map<List<PenaltyItem>>(data);
            return View(model);
        }

    }
}
