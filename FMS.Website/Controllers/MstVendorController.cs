
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
    public class MstVendorController : BaseController
    {
        private IVendorBLL _vendorBLL;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;

        public MstVendorController(IPageBLL PageBll, IVendorBLL  VendorBLL) : base(PageBll, Enums.MenuList.MasterVendor )
        {
            _vendorBLL = VendorBLL ;
            _pageBLL = PageBll;
            _mainMenu = Enums.MenuList.MasterData;
        }
        public ActionResult Index()
        {
            var data = _vendorBLL.GetVendor ();
            var model = new VendorModel();
            model.Details  = Mapper.Map<List<VendorItem>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View(model);
        }
       
        public ActionResult Create()
        {
            var model = new VendorItem();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(VendorItem model)
        {
            if (ModelState.IsValid)
            {
                var dataexist = _vendorBLL.GetVendor().Where(x => (x.VendorName == null ? "" : x.VendorName.ToUpper()) == (model.VendorName == null ? "" : model.VendorName.ToUpper()) 
                                                            && x.IsActive).FirstOrDefault();
                if (dataexist != null)
                {
                    model.ErrorMessage = "Data already exist";
                    model.MainMenu = _mainMenu;
                    model.CurrentLogin = CurrentUser;
                    return View(model);
                }
                else
                {
                    var data = Mapper.Map<VendorDto>(model);
                    data.CreatedBy = CurrentUser.USER_ID;
                    data.CreatedDate = DateTime.Today;
                    data.IsActive = true;
                    try
                    {

                        _vendorBLL.Save(data);
                        _vendorBLL.SaveChanges();
                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);

                    }
                    catch (Exception exception)
                    {
                        AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                        model.ErrorMessage = exception.Message;
                        model.MainMenu = _mainMenu;
                        model.CurrentLogin = CurrentUser;
                        return View(model);
                    }

                }
            }
            return RedirectToAction("Index", "MstVendor");
        }
        public ActionResult Edit(int? MstVendorid)
        {
            if (!MstVendorid.HasValue)
            {
                return HttpNotFound();
            }
            var data = _vendorBLL.GetByID(MstVendorid.Value);


            var model = new VendorItem();
            model = Mapper.Map<VendorItem>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterVendor, MstVendorid.Value);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(VendorItem model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dataexist = _vendorBLL.GetVendor().Where(x => (x.VendorName == null ? "" : x.VendorName.ToUpper()) == (model.VendorName == null ? "" : model.VendorName.ToUpper())
                                                          && x.IsActive && x.MstVendorId != model.MstVendorId).FirstOrDefault();
                    if (dataexist != null)
                    {
                        model.ErrorMessage = "Data already exist";
                        model.MainMenu = _mainMenu;
                        model.CurrentLogin = CurrentUser;
                        return View(model);
                    }

                    var data = Mapper.Map<VendorDto>(model);
                    data.ModifiedDate = DateTime.Now;
                    data.ModifiedBy = CurrentUser.USER_ID;

                    _vendorBLL.Save(data, CurrentUser);
                    _vendorBLL.SaveChanges();
                    AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                }
                catch (Exception exception)
                {
                    AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                    model.ErrorMessage = exception.Message;
                    model.MainMenu = _mainMenu;
                    model.CurrentLogin = CurrentUser;
                    return View(model);
                }
            }
             return RedirectToAction("Index", "MstVendor");
        }

        public ActionResult Detail(int MstVendorid)
        {
            var data = _vendorBLL.GetByID(MstVendorid);
            var model = new VendorItem();
            model = Mapper.Map<VendorItem>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterVendor, MstVendorid);
            return View(model);
        }

        public ActionResult Upload()
        {
            var model = new VendorModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }


        [HttpPost]
        public ActionResult Upload(VendorModel Model)
        {
            if (ModelState.IsValid)
            {
                foreach (VendorItem data in Model.Details)
                {
                    try
                    {
                        data.CreatedDate = DateTime.Now;
                        data.CreatedBy = CurrentUser.USER_ID;
                        data.ModifiedDate = null;
                        data.IsActive = true;
                            
                        var Exist = _vendorBLL.GetVendor().Where(x => (x.VendorName == null ? "" : x.VendorName.ToUpper()) == (data.VendorName == null ? "" : data.VendorName.ToUpper())
                                                                        && x.IsActive).FirstOrDefault();
                        if (Exist != null)
                        {
                            Exist.IsActive = false;
                            Exist.ModifiedBy = "SYSTEM";
                            Exist.ModifiedDate = DateTime.Now;
                            _vendorBLL.Save(Exist);
                        }

                        var dto = Mapper.Map<VendorDto>(data);
                        _vendorBLL.Save(dto);

                        _vendorBLL.SaveChanges();
                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                    }
                    catch (Exception exception)
                    {
                        AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                        Model.MainMenu = _mainMenu;
                        Model.CurrentLogin = CurrentUser;

                        return View(Model);
                    }
                }
            }
            return RedirectToAction("Index", "MstVendor");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var qtyPacked = string.Empty;
            var qty = string.Empty;

            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<VendorUploadItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    else if(dataRow[0] == "Vendor Name")
                    {
                        continue;
                    }
                    var item = new VendorUploadItem();
                    item.ErrorMessage = "";
                    try
                    {
                        item.VendorName = dataRow[0].ToString();
                        if (item.VendorName == "")
                        {
                            item.ErrorMessage = "Vendor Name Can't be Empty";
                          
                        }

                        item.ShortName = dataRow[1].ToString();
                        if (item.ShortName == "")
                        {
                            item.ErrorMessage = "Short Name Can't be Empty";
                        }

                        item.EmailAddress = dataRow[2].ToString();
                        if (item.EmailAddress == "")
                        {
                            item.ErrorMessage = "Email Address Can't be Empty";
                        }
                    }
                    catch (Exception exp)
                    {

                        item.ErrorMessage = exp.Message;
                    }
                   
                    model.Add(item);
                }
            }
            return Json(model);
        }

        #region export xls
        public void ExportMasterVendor()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterVendor();

            var newFile = new FileInfo(pathFile);

            var fileName = Path.GetFileName(pathFile);

            string attachment = string.Format("attachment; filename={0}", fileName);
            Response.Clear();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.WriteFile(newFile.FullName);
            Response.Flush();
            newFile.Delete();
            Response.End();
        }

        private string CreateXlsMasterVendor()
        {
            //get data
            List<VendorDto> vendor = _vendorBLL.GetVendor();
            var listData = Mapper.Map<List<VendorItem>>(vendor);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Vendor");
            slDocument.MergeWorksheetCells(1, 1, 1, 8);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterVendor(slDocument);

            //create data
            slDocument = CreateDataExcelMasterVendor(slDocument, listData);

            var fileName = "Master_Data_Vendor" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterVendor(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Vendor Name");
            slDocument.SetCellValue(iRow, 2, "Short Name");
            slDocument.SetCellValue(iRow, 3, "Email Address");
            slDocument.SetCellValue(iRow, 4, "Created Date");
            slDocument.SetCellValue(iRow, 5, "Created By");
            slDocument.SetCellValue(iRow, 6, "Modified Date");
            slDocument.SetCellValue(iRow, 7, "Modified By");
            slDocument.SetCellValue(iRow, 8, "Status");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 8, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelMasterVendor(SLDocument slDocument, List<VendorItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.VendorName );
                slDocument.SetCellValue(iRow, 2, data.ShortName);
                slDocument.SetCellValue(iRow, 3, data.EmailAddress );
                slDocument.SetCellValue(iRow, 4, data.CreatedDate.ToString("dd-MMM-yyyy hh:mm:ss") );
                slDocument.SetCellValue(iRow, 5, data.CreatedBy);
                slDocument.SetCellValue(iRow, 6, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss" ) );
                slDocument.SetCellValue(iRow, 7, data.ModifiedBy);
                if (data.IsActive)
                {
                    slDocument.SetCellValue(iRow, 8, "Active");
                }
                else
                {
                    slDocument.SetCellValue(iRow, 8, "InActive");
                }

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 8);
            slDocument.SetCellStyle(3, 1, iRow - 1, 8, valueStyle);

            return slDocument;
        }

        #endregion
    }

}