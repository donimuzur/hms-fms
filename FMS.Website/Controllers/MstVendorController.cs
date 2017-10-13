
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

namespace FMS.Website.Controllers
{
    public class MstVendorController : BaseController
    {
        private IVendorBLL _vendorBLL;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;

        public MstVendorController(IPageBLL PageBll, IVendorBLL  VendorBLL) : base(PageBll, Enums.MenuList.MasterVendor )
        {
            _vendorBLL = VendorBLL ;
            _pageBLL = PageBll;
            _mainMenu = Enums.MenuList.MasterVendor;
        }
        public ActionResult Index()
        {
            var data = _vendorBLL.GetVendor ();
            var model = new VendorModel();
            model.Details  = Mapper.Map<List<VendorItem>>(data);
            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Create(VendorItem model)
        {
            if (ModelState.IsValid)
            {
                var dataexist = _vendorBLL.GetExist(model.VendorName);
                if (dataexist != null)
                {
                    AddMessageInfo("Data Already Exist", Enums.MessageInfoType.Warning);
                    
                    return RedirectToAction("Create", "MstVendor", new
                    {
                        VendorName = model.VendorName ,
                        ShortName = model.ShortName,
                        EmailAddress = model.EmailAddress

                    });
                }
                else
                {

                    var data = Mapper.Map<VendorDto>(model);
                    data.CreatedBy = "Doni";
                    data.CreatedDate = DateTime.Today;
                    data.IsActive = true;
                    data.ModifiedDate = null;
                    try
                    {

                        _vendorBLL.Save(data);

                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);

                    }
                    catch (Exception exception)
                    {
                        AddMessageInfo(exception.Message, Enums.MessageInfoType.Error
                                );
                        return RedirectToAction("Create", "MstVendor", new
                        {
                            VendorName = model.VendorName,
                            ShortName = model.ShortName,
                            EmailAddress = model.EmailAddress

                        });
                    }

                }
            }
            return RedirectToAction("Index", "MstVendor");
        }
        public ActionResult Edit(int MstVendorid)
        {
            var data = _vendorBLL.GetByID(MstVendorid);
            var model = new VendorItem();
            model = Mapper.Map<VendorItem>(data);
            
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(VendorItem model)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<VendorDto>(model);
                data.IsActive = true;
                data.ModifiedDate = DateTime.Now;
                data.ModifiedBy = "User";

                var dataexist = _vendorBLL.GetExist(data.VendorName);
                if (dataexist != null)
                {
                    AddMessageInfo("Data Already Exist", Enums.MessageInfoType.Warning);

                    return View(model);
                    
                }
                else
                {
                    try
                    {

                        _vendorBLL.Save(data);

                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);

                    }
                    catch (Exception exception)
                    {
                        AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                        return View(model);
                    }
                }
            }
             return RedirectToAction("Index", "MstVendor");
        }

        public ActionResult Upload()
        {
            var model = new VendorModel();
            return View(model);
        }


        [HttpPost]
       
        public ActionResult Upload(HttpPostedFileBase upload, string value, VendorModel Model)
        {
            if (ModelState.IsValid)
            {
                if (value == "generate")
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                        // to get started. This is how we avoid dependencies on ACE or Interop:
                        Stream stream = upload.InputStream;

                        // We return the interface, so that
                        IExcelDataReader reader = null;


                        if (upload.FileName.EndsWith(".xls"))
                        {
                            reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }
                        else if (upload.FileName.EndsWith(".xlsx"))
                        {
                            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }
                        else
                        {
                            ModelState.AddModelError("File", "This file format is not supported");
                            return View();
                        }

                        DataSet result = reader.AsDataSet();
                        List<DataRow> List = result.Tables[0].AsEnumerable().ToList();


                        var data = new List<VendorUploadItem>();

                        foreach (DataRow dr in List)
                        {
                            if (List.IndexOf(dr) == 0) { continue; }
                            var item = new VendorUploadItem();
                            item.VendorName = dr.ItemArray[0].ToString();
                            item.ShortName = dr.ItemArray[1].ToString();
                            item.EmailAddress = dr.ItemArray[2].ToString();
                            item.CreatedBy = "doni";
                            item.CreatedDate = DateTime.Today;
                            item.ModifiedDate = null;
                            item.IsActive = true;
                            data.Add(item);

                        }

                        reader.Close();
                        var ModelExcel = new VendorModel();
                        ModelExcel.Details = Mapper.Map<List<VendorItem>>(data);
                        return View(ModelExcel);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "Please Upload Your file");
                    }
                }
                if (value == "Save")
                {
                    foreach (VendorItem data in Model.Details)
                    {
                        try
                        {
                            var dto = Mapper.Map<VendorDto>(data);
                            _vendorBLL.Save(dto);

                            AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);

                        }
                        catch (Exception exception)
                        {
                            AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                            return View(Model);
                        }
                    }
                    return RedirectToAction("Index", "MstVendor");

                }

            }
            return RedirectToAction("Index", "MstVendor");
        }
    }
        
}