using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FMS.Website.Models;
using FMS.Contract.BLL;
using FMS.Core;
using AutoMapper;
using FMS.BusinessObject.Dto;
using System.Web;
using System.IO;
using ExcelDataReader;
using System.Data;
using FMS.Website.Utility;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;

namespace FMS.Website.Controllers
{
    public class TraCarComplaintFormController : BaseController
    {
        private ICarComplaintFormBLL _CFFBLL;
        private Enums.MenuList _mainMenu;

        public TraCarComplaintFormController(IPageBLL pageBll, ICarComplaintFormBLL CFFBLL) : base(pageBll, Enums.MenuList.TraCcf)
        {
            _CFFBLL = CFFBLL;
            _mainMenu = Enums.MenuList.Transaction;
        }

        //
        // GET: /TraCarComplaintForm/

        public ActionResult Index()
        {
            var data = _CFFBLL.GetCCF();

            var model = new CarComplaintFormModel();
            model.Details = Mapper.Map<List<CarComplaintFormItem>>(data);
            model.MainMenu = _mainMenu;
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new CarComplaintFormItem();
            model.MainMenu = _mainMenu;
            return View(model);
        }

    }
}
