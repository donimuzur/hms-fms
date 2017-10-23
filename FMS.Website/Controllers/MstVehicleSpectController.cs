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
    public class MstVehicleSpectController : BaseController
    {
        private IVehicleSpectBLL _VehicleSpectBLL;
        //private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;

        public MstVehicleSpectController(IPageBLL PageBll, IVehicleSpectBLL VehicleSpectBLL) : base(PageBll, Enums.MenuList.MasterVehicleSpect)
        {
            _VehicleSpectBLL = VehicleSpectBLL;
            _pageBLL = PageBll;
        }

        //
        // GET: /VehicleSpect/

        public ActionResult Index()
        {
            var data = _VehicleSpectBLL.GetVehicleSpect();
            var model = new VehicleSpectModel();
            model.Details = Mapper.Map<List<VehicleSpectItem>>(data);
            return View(model);
        }

        public ActionResult Edit(int? MstVehicleSpectId)
        {
            if (!MstVehicleSpectId.HasValue)
            {
                return HttpNotFound();
            }

            var data = _VehicleSpectBLL.GetVehicleSpectById(MstVehicleSpectId.Value);
            var model = new VehicleSpectItem();
            model = Mapper.Map<VehicleSpectItem>(data);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(VehicleSpectItem model)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<VehicleSpectDto>(model);
                data.IsActive = true;
                data.ModifiedDate = DateTime.Now;
                data.ModifiedBy = "User";

                try
                {
                    _VehicleSpectBLL.Save(data);
                    AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                }
                catch (Exception exception)
                {
                    AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                    return View(model);
                }
            }
            return RedirectToAction("Index", "MstVehicleSpect");
        }

    }
}
