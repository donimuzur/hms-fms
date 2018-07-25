using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;
using FMS.BusinessObject.Inputs;

namespace FMS.Website.Controllers
{
    public class ArchivedController : BaseController
    {
        private Enums.MenuList _mainMenu;
        private IArchiveBLL _archiveBLL;
        public ArchivedController(IPageBLL pageBll, IArchiveBLL ArchiveBLL) : base(pageBll, Enums.MenuList.MasterData)
        {
            _mainMenu = Enums.MenuList.MasterData;
            _archiveBLL = ArchiveBLL;
        }
        
        public ActionResult Index(string message = null)
        {
            var model = new ArchiveModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            model.Message = message;
            Initial(model);
            return View(model);
        }
        public void Initial(ArchiveModel model)
        {
            model.Details.Add(new ArchiveItem() { Id = 1, Table = "Master Cost OB", Type = "Master" });
            model.Details.Add(new ArchiveItem() { Id = 2, Table = "Master Employee", Type = "Master" });
            model.Details.Add(new ArchiveItem() { Id = 3, Table = "Master Epaf", Type = "Master" });
            model.Details.Add(new ArchiveItem() { Id = 4, Table = "Master Fleet", Type = "Master" });
            model.Details.Add(new ArchiveItem() { Id = 5, Table = "Master Fuel Odometer", Type = "Master" });
            model.Details.Add(new ArchiveItem() { Id = 6, Table = "Master Function Group", Type = "Master" });
            model.Details.Add(new ArchiveItem() { Id = 7, Table = "Master GS", Type = "Master" });
            model.Details.Add(new ArchiveItem() { Id = 8, Table = "Master Holiday Calendar", Type = "Master" });
            model.Details.Add(new ArchiveItem() { Id = 9, Table = "Master Location Mapping", Type = "Master" });
            model.Details.Add(new ArchiveItem() { Id = 10, Table = "Master Penalty", Type = "Master" });
            model.Details.Add(new ArchiveItem() { Id = 11, Table = "Master Pricelist", Type = "Master" });
            model.Details.Add(new ArchiveItem() { Id = 12, Table = "Master Sales Volume", Type = "Master" });
            model.Details.Add(new ArchiveItem() { Id = 13, Table = "Master Vehicle Spect" ,Type="Master"});

            model.Details.Add(new ArchiveItem() { Id = 14, Table = "CSF", Type = "Transaction" });
            model.Details.Add(new ArchiveItem() { Id = 15, Table = "CRF", Type = "Transaction" });
            model.Details.Add(new ArchiveItem() { Id = 16, Table = "CTF", Type = "Transaction" });
            model.Details.Add(new ArchiveItem() { Id = 17, Table = "CCF", Type = "Transaction" });
            model.Details.Add(new ArchiveItem() { Id = 18, Table = "CAF", Type = "Transaction" });
            model.Details.Add(new ArchiveItem() { Id = 19, Table = "TEMPORARY", Type = "Transaction" });

            var OperatorList = new Dictionary<string, string>();
            OperatorList.Add("OR", "OR");
            OperatorList.Add("AND", "AND");
            model.SearchView.OperatorList = new SelectList(OperatorList,"Key","Value");

            var VehicleTypeList = new Dictionary<string, string>();
            VehicleTypeList.Add("BENEFIT", "BENEFIT");
            VehicleTypeList.Add("WTC", "WTC");
            model.SearchView.VehicleTypeList = new SelectList(VehicleTypeList, "Key", "Value");
        }
        public ActionResult ArchiveMaster(string TableArchveId, DateTime? CreatedDate, DateTime? ModifiedDate, string Operator, string VehicleType)
        {
            try
            {
                var input = new ArchiveParamInput();
                input.StartDate = DateTime.Now;
                if(CreatedDate.HasValue)input.CreatedDate = new DateTime(CreatedDate.Value.Year,CreatedDate.Value.Month,CreatedDate.Value.Day,23,59,59);
                if (ModifiedDate.HasValue) input.ModifiedDate = new DateTime(ModifiedDate.Value.Year, ModifiedDate.Value.Month, ModifiedDate.Value.Day, 23, 59, 59); ;
                input.TableId = TableArchveId;
                input.Operator = Operator;
                input.VehicleType = VehicleType;

                var result = _archiveBLL.DoArchive(input, CurrentUser);
                if (result == false) return RedirectToAction("Index", "Archived", new { message = "data is not exist on database/data already archived" });

            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Archived",new { message = "Error On Process" });
            }
            return RedirectToAction("Index", "Archived", new { message = "Archive Successfull" });
        }
    }
}
