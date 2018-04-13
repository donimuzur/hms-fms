using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;
using System.IO;

namespace FMS.Website.Controllers
{
    public class HelpController : BaseController
    {
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        public HelpController(IPageBLL pageBll) : base(pageBll, Core.Enums.MenuList.Help)
        {
            _pageBLL = pageBll;
            _mainMenu = Enums.MenuList.Help;
        }

        //
        // GET: /Help/
        public ActionResult Index(string Type)
        {
            var model = new BaseModel();

            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            if(Type == "BENEFIT" )
            {
                model.SuccesMessage = "Help_Files/User Guideline/BENEFIT.pdf";
                model.MessageTitle = "User Guideline Benefit";
            }
            else if(Type == "WTC")
            {
                model.SuccesMessage = "Help_Files/User Guideline/WTC.pdf";
                model.MessageTitle = "User Guideline WTC";
            }
                
            return View(model);
        }
        public ActionResult VehicleInformation(string Type)
        {
            var model = new BaseModel();

            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            if (Type == "BENEFIT")
            {
                model.SuccesMessage = "Help_Files/Vehicle Information/BENEFIT.pdf";
                model.MessageTitle = "Vehicle Information Benefit";
            }
            else if (Type == "WTC")
            {
                model.SuccesMessage = "Help_Files/Vehicle Information/WTC.pdf";
                model.MessageTitle = "Vehicle Information WTC";
            }

            return View(model);
        }
        public ActionResult VendorSLA(string Type)
        {
            var model = new BaseModel();

            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            if (Type == "TRAC")
            {
                model.SuccesMessage = "Help_Files/Vendor SLA/TRAC.pdf";
                model.MessageTitle = "Vendor SLA TRAC";
            }
            else if (Type == "ASSA")
            {
                model.SuccesMessage = "Help_Files/Vendor SLA/ASSA.pdf";
                model.MessageTitle = "Vendor SLA ASSA";
            }

            return View(model);
        }
        public ActionResult VendorInformation(string Type)
        {
            var model = new BaseModel();

            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            if (Type == "TRAC")
            {
                model.SuccesMessage = "Help_Files/Vendor Information/TRAC.pdf";
                model.MessageTitle = "Vendor Information TRAC";
            }
            else if (Type == "ASSA")
            {
                model.SuccesMessage = "Help_Files/Vendor Information/ASSA.pdf";
                model.MessageTitle = "Vendor Information ASSA";
            }

            return View(model);
        }

        public void PdfDownload(string FilePath)
        {
            //string TempFilePath = FilePath;

            //if (System.IO.File.Exists(TempFilePath))
            //{
            //    var cd = new System.Net.Mime.ContentDisposition
            //    {
            //        FileName = "testPDF.pdf",
            //        Inline = false,
            //    };
            //    Response.AppendHeader("Content-Disposition", cd.ToString());
            //    return File(TempFilePath, "application/pdf");
            //}
            //else
            //{
            //    return null;
            //}
            FilePath = AppDomain.CurrentDomain.BaseDirectory + FilePath;
            var newFile = new FileInfo(FilePath);
            var fileName = Path.GetFileName(FilePath);
            string attachment = string.Format("attachment; filename={0}", fileName);
            Response.Clear();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/pdf";
            Response.WriteFile(newFile.FullName);
            Response.Flush();
            Response.End();
        }
    }
}
