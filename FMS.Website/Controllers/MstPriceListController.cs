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
using FMS.DAL;
using FMS.BusinessObject;

namespace FMS.Website.Controllers
{
    public class MstPriceListController : BaseController
    {
        private IPriceListBLL _priceListBLL;
        private IVehicleSpectBLL _vehicleSpect;
        private ILocationMappingBLL _locationMapping;
        private IPageBLL _pageBLL;
        private IVendorBLL _vendorBLL;
        private ISettingBLL _settingBLL;
        private Enums.MenuList _mainMenu;

        public MstPriceListController(IPageBLL PageBll, IPriceListBLL PriceListBLL, IVehicleSpectBLL VehicleSpect, ILocationMappingBLL LocationMapping, IVendorBLL VendorBLL, ISettingBLL SettingBLL) : base(PageBll, Enums.MenuList.MasterPriceList)
        {
            _priceListBLL = PriceListBLL;
            _vehicleSpect = VehicleSpect;
            _locationMapping = LocationMapping;
            _vendorBLL = VendorBLL;
            _settingBLL = SettingBLL;
            _pageBLL = PageBll;
            _mainMenu = Enums.MenuList.MasterData;
  
        }

        public PriceListItem listdata(PriceListItem model)
        {
            var VehicleSpect = _vehicleSpect.GetVehicleSpect().Where(x => x.IsActive).ToList();

            var ManufacturerList = VehicleSpect.Select(x =>  new { x.Manufacturer }).Distinct().OrderBy(x => x.Manufacturer).ToList();
            model.ManufactureList = new SelectList(ManufacturerList, "Manufacturer", "Manufacturer");

            var ModelList = VehicleSpect.Select(x => new { x.Models }).Distinct().OrderBy(x => x.Models).ToList();
            model.ModelList = new SelectList(ModelList, "Models", "Models");

            var SeriesList = VehicleSpect.Select(x => new { x.Series }).Distinct().OrderBy(x => x.Series).ToList();
            model.SeriesList = new SelectList(SeriesList, "Series", "Series");

            var VendorList = _vendorBLL.GetVendor().Where(x => x.IsActive).Select(x => new { x.VendorName, x.MstVendorId }).Distinct().OrderBy(x=> x.VendorName).ToList();
            model.VendorList = new SelectList(VendorList, "MstVendorId", "VendorName");

            var listvehicleType = _settingBLL.GetSetting().Where(x=> x.IsActive).Select(x => new { x.SettingGroup, x.SettingName, x.SettingValue }).ToList().Where(x => x.SettingGroup == "VEHICLE_TYPE").Distinct().OrderBy(x => x.SettingName);
            model.VehicleTypeList = new SelectList(listvehicleType, "SettingName", "SettingName");

            var listvehicleUsage = _settingBLL.GetSetting().Where(x => x.IsActive).Select(x => new { x.SettingGroup, x.SettingName, x.SettingValue }).ToList().Where(x => x.SettingGroup == "VEHICLE_USAGE_BENEFIT").Distinct().OrderBy(x => x.SettingName);
            model.VehicleUsageList = new SelectList(listvehicleUsage, "SettingName", "SettingName");

            var ZoneList = _locationMapping.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.ZonePriceList }).Distinct().OrderBy(x => x.ZonePriceList).ToList();
            model.ZoneList = new SelectList(ZoneList, "ZonePriceList", "ZonePriceList");

            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterPriceList, model.MstPriceListId);

            return model;
        }
        
        public ActionResult Index()
        {
            var data = _priceListBLL.GetPriceList();
            var model = new PriceListModel();
            model.Details = Mapper.Map<List<PriceListItem>>(data);
            foreach(PriceListItem detail in model.Details)
            {
                detail.VendorName = _vendorBLL.GetByID(detail.Vendor) == null ? string.Empty : _vendorBLL.GetByID(detail.Vendor).VendorName;
            }
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                model.IsShowNewButton = false;
                model.IsNotViewer = false;
            }
            else
            {
                model.IsShowNewButton = true;
                model.IsNotViewer = true;
            }
            return View(model);
        }

       
        public ActionResult Create()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                return RedirectToAction("Index");
            }
            var model = new PriceListItem();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model = listdata(model);

            return View(model);
        }

        
        [HttpPost]
        public ActionResult Create(PriceListItem item)
        {
            if (ModelState.IsValid)
            {
                var dataexist = _priceListBLL.GetPriceList().Where(x => ((x.Manufacture == null  ? "" : x.Manufacture.ToUpper())== (item.Manufacture == null  ? "" : item.Manufacture.ToUpper()))
                                                                && (x.Model == null ? "" : x.Model.ToUpper()) == (item.Models == null ? "" : item.Models.ToUpper())
                                                                && (x.Series == null ? "" : x.Series.ToUpper()) == (item.Series == null ? "" : item.Series.ToUpper())
                                                                && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (item.VehicleType == null ? "" : item.VehicleType.ToUpper())
                                                                && (x.VehicleUsage == null ? "" : x.VehicleUsage.ToUpper()) == (item.VehicleUsage == null ? "" : item.VehicleUsage.ToUpper())
                                                                && (x.ZonePriceList == null? "" : x.ZonePriceList.ToUpper()) == (item.ZonePriceList == null ? "" : item.ZonePriceList.ToUpper())
                                                                && x.Year == item.Year && x.Vendor == item.Vendor && x.IsActive).FirstOrDefault();
                if (dataexist != null)
                {
                    item.ErrorMessage = "Data already exist";
                    item.MainMenu = _mainMenu;
                    item.CurrentLogin = CurrentUser;
                    item = listdata(item);
                    return View(item);
                }
                else
                {
                    if (item.InstallmenEMPStr != null)
                    {
                        item.InstallmenEMP = Convert.ToDecimal(item.InstallmenEMPStr.Replace(",", ""));
                    }
                    if (item.InstallmenHMSStr != null)
                    {
                        item.InstallmenHMS = Convert.ToDecimal(item.InstallmenHMSStr.Replace(",", ""));
                    }
                    if (item.PriceStr != null)
                    {
                        item.Price = Convert.ToDecimal(item.PriceStr.Replace(",", ""));
                    }

                    var data = Mapper.Map<PriceListDto>(item);
                    
                    data.CreatedBy = CurrentUser.USER_ID;
                    data.CreatedDate = DateTime.Today;
                    data.IsActive = true;
                    try
                    {

                        _priceListBLL.Save(data);
                        _priceListBLL.SaveChanges();
                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);

                    }
                    catch (Exception exception)
                    {
                        AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                        item.ErrorMessage = exception.Message;
                        item.MainMenu = _mainMenu;
                        item.CurrentLogin = CurrentUser;
                        item = listdata(item);
                        return View(item);
                    }

                }
            }
            return RedirectToAction("Index", "MstPriceList");
        }

        public ActionResult View(int? MstPriceListid)
        {
            if (!MstPriceListid.HasValue)
            {
                return HttpNotFound();
            }
            var data = _priceListBLL.GetByID(MstPriceListid.Value);
            var model = new PriceListItem();
            model = Mapper.Map<PriceListItem>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model = listdata(model);
            
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterPriceList, MstPriceListid.Value);

            return View(model);
        }

        public ActionResult Edit(int? MstPriceListid)
        {
            if (!MstPriceListid.HasValue)
            {
                return HttpNotFound();
            }
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                return RedirectToAction("Index");
            }
            var data = _priceListBLL.GetByID(MstPriceListid.Value);
            var model = new PriceListItem();
            model = Mapper.Map<PriceListItem>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model = listdata(model);
          
          

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(PriceListItem item)
        {
            if (ModelState.IsValid)
            {
                var dataexist = _priceListBLL.GetPriceList().Where(x => ((x.Manufacture == null ? "" : x.Manufacture.ToUpper()) == (item.Manufacture == null ? "" : item.Manufacture.ToUpper()))
                                                            && (x.Model == null ? "" : x.Model.ToUpper()) == (item.Models == null ? "" : item.Models.ToUpper())
                                                            && (x.Series == null ? "" : x.Series.ToUpper()) == (item.Series == null ? "" : item.Series.ToUpper())
                                                            && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (item.VehicleType == null ? "" : item.VehicleType.ToUpper())
                                                            && (x.VehicleUsage == null ? "" : x.VehicleUsage.ToUpper()) == (item.VehicleUsage == null ? "" : item.VehicleUsage.ToUpper())
                                                            && (x.ZonePriceList == null ? "" : x.ZonePriceList.ToUpper()) == (item.ZonePriceList == null ? "" : item.ZonePriceList.ToUpper())
                                                            && x.Year == item.Year && x.Vendor == item.Vendor && x.IsActive && x.MstPriceListId != item.MstPriceListId).FirstOrDefault();

                if (dataexist != null)
                {
                    item.ErrorMessage = "Data already exist";
                    item.MainMenu = _mainMenu;
                    item.CurrentLogin = CurrentUser;
                    item = listdata(item);
                    return View(item);
                }
                else
                {
                    if (item.InstallmenEMPStr != null)
                    {
                        item.InstallmenEMP = Convert.ToDecimal(item.InstallmenEMPStr.Replace(",", ""));
                    }
                    if (item.InstallmenHMSStr != null)
                    {
                        item.InstallmenHMS = Convert.ToDecimal(item.InstallmenHMSStr.Replace(",", ""));
                    }
                    if (item.PriceStr != null)
                    {
                        item.Price = Convert.ToDecimal(item.PriceStr.Replace(",", ""));
                    }

                    var data = Mapper.Map<PriceListDto>(item);
                    data.ModifiedBy = CurrentUser.USER_ID;
                    data.ModifiedDate = DateTime.Today;
                    data.IsActive = true;
                    try
                    {
                        _priceListBLL.Save(data);
                        _priceListBLL.SaveChanges();
                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);

                    }
                    catch (Exception exception)
                    {
                        AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                        item.ErrorMessage = exception.Message;
                        item.MainMenu = _mainMenu;
                        item.CurrentLogin = CurrentUser;
                        item = listdata(item);
                        return View(item);
                    }
                }
            }
            return RedirectToAction("Index", "MstPriceList");
        }

        public ActionResult Upload()
        {
            var model = new PriceListModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }


        [HttpPost]
        public ActionResult Upload(PriceListModel Model)
        {
            if (ModelState.IsValid)
            {
                foreach (PriceListItem data in Model.Details)
                {
                    try
                    {
                        var dataexist = _priceListBLL.GetPriceList().Where(x => ((x.Manufacture == null ? "" : x.Manufacture.ToUpper()) == (data.Manufacture == null ? "" : data.Manufacture.ToUpper()))
                                                           && (x.Model == null ? "" : x.Model.ToUpper()) == (data.Models == null ? "" : data.Models.ToUpper())
                                                           && (x.Series == null ? "" : x.Series.ToUpper()) == (data.Series == null ? "" : data.Series.ToUpper())
                                                           && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (data.VehicleType == null ? "" : data.VehicleType.ToUpper())
                                                           && (x.VehicleUsage == null ? "" : x.VehicleUsage.ToUpper()) == (data.VehicleUsage == null ? "" : data.VehicleUsage.ToUpper())
                                                           && (x.ZonePriceList == null ? "" : x.ZonePriceList.ToUpper()) == (data.ZonePriceList == null ? "" : data.ZonePriceList.ToUpper())
                                                           && x.Year == data.Year && x.Vendor == data.Vendor && x.IsActive ).FirstOrDefault();

                        if (dataexist != null)
                        {
                            dataexist.IsActive = false;
                            dataexist.ModifiedBy = "SYSTEM";
                            dataexist.ModifiedDate = DateTime.Now;
                            _priceListBLL.Save(dataexist);
                        }

                        data.CreatedDate = DateTime.Now;
                        data.CreatedBy = CurrentUser.USER_ID;
                        data.ModifiedDate = null;
                        data.IsActive = true;
                        if (data.ErrorMessage == "" | data.ErrorMessage == null)
                        {
                            var dto = Mapper.Map<PriceListDto>(data);

                            _priceListBLL.Save(dto);
                        }

                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                    }
                    catch (Exception exception)
                    {
                        AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                        Model.ErrorMessage = exception.Message;
                        Model.MainMenu = _mainMenu;
                        Model.CurrentLogin = CurrentUser;
                        return View(Model);
                    }
                }

                _priceListBLL.SaveChanges();
            }
            return RedirectToAction("Index", "MstPriceList");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<PriceListItem>();
            if (data != null)
            {
                var Vendor = _vendorBLL.GetVendor().Where(x => x.IsActive);
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    var item = new PriceListItem();
                    item.ErrorMessage = "";
                    try
                    {
                        item.VendorName = dataRow[0];
                        if(item.VendorName == "" )
                        {
                            item.ErrorMessage = "Vendor Can't be Empty";
                        }
                        var VendorId = Vendor.Where(x => (x.VendorName == null ? "" : x.VendorName.ToUpper()) == (item.VendorName == null ? "" : item.VendorName.ToUpper())).FirstOrDefault();

                        if(VendorId == null )
                        {
                            item.ErrorMessage = "Vendor is not in Master Vendor";
                        }
                        else
                        {
                            item.Vendor = VendorId.MstVendorId;
                        }

                        item.VehicleType = dataRow[1] == null ? "" : dataRow[1].ToUpper();
                        if (item.VehicleType =="")
                        {
                            item.ErrorMessage = "Vehicle Type Can't be empty"; 
                        }
                        if((item.VehicleType == null  ? "" : item.VehicleType.ToUpper()) == "BENEFIT")
                        {
                            item.VehicleUsage = dataRow[2] == null ? "" : dataRow[2].ToUpper();
                            if (item.VehicleUsage == "")
                            {
                                item.ErrorMessage = "Vehicle Usage Can't be empty";
                            }
                        }
                        else
                        {
                            item.VehicleUsage = dataRow[2] == null ? "" : dataRow[2].ToUpper();
                        }

                        item.ZonePriceList = dataRow[3];

                        item.Manufacture = dataRow[4];
                        if(item.Manufacture == "")
                        {
                            item.ErrorMessage = "Manufacture Can't be empty";
                        }

                        item.Models = dataRow[5];
                        if(item.Models == "")
                        {
                            item.ErrorMessage = "Model Can't be empty";
                        }

                        item.Series = dataRow[6];
                        if (item.Series == "")
                        {
                            item.ErrorMessage = "Series Can't be empty";
                        }

                        try
                        {
                            item.Year = Convert.ToInt32(dataRow[7]);
                        }
                        catch (Exception)
                        {

                            item.ErrorMessage = "Request Year is not valid";
                        }

                        try
                        {
                            item.InstallmenHMS = Convert.ToInt32(dataRow[8]);
                        }
                        catch (Exception)
                        {

                            item.ErrorMessage = "Installment HMS  is not valid";
                        }

                        try
                        {
                            item.InstallmenEMP = Convert.ToInt32(dataRow[9]);
                        }
                        catch (Exception)
                        {

                            item.ErrorMessage = "Installment Employee is not valid";
                        }
                        var VehicleSpect = _vehicleSpect.GetVehicleSpect().Where(x => (x.Manufacturer == null ? "" : x.Manufacturer.ToUpper()) == (item.Manufacture == null ? "" : item.Manufacture.ToUpper())
                                                                                && (x.Models == null ? "" : x.Models.ToUpper()) == (item.Models == null ? "" : item.Models.ToUpper())
                                                                                && (x.Series == null ? "" : x.Series.ToUpper()) == (item.Series == null ? "" : item.Series.ToUpper())
                                                                                && x.IsActive).FirstOrDefault();
                        if (VehicleSpect == null) item.ErrorMessage = "There is no description spect in Master Vehicle Spect";
                        model.Add(item);
                    }
                    catch (Exception ex)
                    {
                        item.ErrorMessage = ex.Message;
                    }
                }
            }
            return Json(model);
        }

        #region --------------- Get Json ----------------
        public JsonResult GetModelList(string Manufacture)
        {
            var ModelList= _vehicleSpect.GetVehicleSpect().Where(x => (x.Manufacturer == null ? "" : x.Manufacturer.ToUpper()) == (Manufacture == null ? "" : Manufacture.ToUpper())
                                                                     && x.IsActive).Select(x=> new {x.Models}).Distinct().OrderBy(x => x.Models).ToList();
            return Json(ModelList);
        }
        public JsonResult GetSeriesList(string Manufacture, string Models)
        {
            var SeriesList = _vehicleSpect.GetVehicleSpect().Where(x => (x.Manufacturer == null ? "" : x.Manufacturer.ToUpper()) == (Manufacture == null ? "" : Manufacture.ToUpper())
                                                                        && (x.Models == null ? "" : x.Models.ToUpper()) == (Models == null ? "" : Models.ToUpper())
                                                                        && x.IsActive).Select(x => new { x.Series}).Distinct().OrderBy(x => x.Series).ToList();
            
            return Json(SeriesList);
        }
        #endregion

        #region export xls
        public void ExportMasterPriceList()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterPriceList();

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

        private string CreateXlsMasterPriceList()
        {
            //get data
            List<PriceListDto> priceList = _priceListBLL.GetPriceList();
            var listData = Mapper.Map<List<PriceListItem>>(priceList);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Price List");
            slDocument.MergeWorksheetCells(1, 1, 1, 15);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterPriceList(slDocument);

            //create data
            slDocument = CreateDataExcelMasterPriceList(slDocument, listData);

            var fileName = "Master_Data_PriceList" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterPriceList(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Vendor Name");
            slDocument.SetCellValue(iRow, 2, "Vehicle Type");
            slDocument.SetCellValue(iRow, 3, "Vehicle Usage");
            slDocument.SetCellValue(iRow, 4, "Zone Price List");
            slDocument.SetCellValue(iRow, 5, "Manufacture");
            slDocument.SetCellValue(iRow, 6, "Model");
            slDocument.SetCellValue(iRow, 7, "Series");
            slDocument.SetCellValue(iRow, 8, "Request Year");
            slDocument.SetCellValue(iRow, 9, "Installment HMS");
            slDocument.SetCellValue(iRow, 10, "Installment EMP");
            slDocument.SetCellValue(iRow, 11, "Created Date");
            slDocument.SetCellValue(iRow, 12, "Created By");
            slDocument.SetCellValue(iRow, 13, "Modified Date");
            slDocument.SetCellValue(iRow, 14, "Modified By");
            slDocument.SetCellValue(iRow, 15, "Status");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 15, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelMasterPriceList(SLDocument slDocument, List<PriceListItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                var vendor = _vendorBLL.GetByID(data.Vendor);
                slDocument.SetCellValue(iRow, 1, vendor == null ? "" : vendor.VendorName);
                slDocument.SetCellValue(iRow, 2, data.VehicleType);
                slDocument.SetCellValue(iRow, 3, data.VehicleUsage);
                slDocument.SetCellValue(iRow, 4, data.ZonePriceList);
                slDocument.SetCellValue(iRow, 5, data.Manufacture);
                slDocument.SetCellValue(iRow, 6, data.Models);
                slDocument.SetCellValue(iRow, 7, data.Series);
                slDocument.SetCellValue(iRow, 8, data.Year);
                slDocument.SetCellValue(iRow, 9, data.InstallmenHMS);
                slDocument.SetCellValue(iRow, 10, data.InstallmenEMP);
                slDocument.SetCellValue(iRow, 11, data.CreatedDate.ToString("dd-MMM-yyyy HH:mm:ss"));
                slDocument.SetCellValue(iRow, 12, data.CreatedBy);
                slDocument.SetCellValue(iRow, 13, data.ModifiedDate.Value.ToString("dd-MMM-yyyy HH:mm:ss"));
                slDocument.SetCellValue(iRow, 14, data.ModifiedBy);
                if (data.IsActive)
                {
                    slDocument.SetCellValue(iRow, 15, "Active");
                }
                else
                {
                    slDocument.SetCellValue(iRow, 15, "InActive");
                }

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 15);
            slDocument.SetCellStyle(3, 1, iRow - 1, 15, valueStyle);

            return slDocument;
        }

        #endregion
    }
      

}  