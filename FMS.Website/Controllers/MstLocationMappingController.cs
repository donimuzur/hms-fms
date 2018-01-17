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
using System.Globalization;

namespace FMS.Website.Controllers
{
    public class MstLocationMappingController : BaseController
    {
        private ILocationMappingBLL _locationMappingBLL;
        private IPageBLL _pageBLL;
        private Enums.MenuList _mainMenu;
        private IEmployeeBLL _employeeBLL;
        public MstLocationMappingController(IPageBLL PageBll, ILocationMappingBLL LocationMappingBLL, IEmployeeBLL EmployeeBLL) : base(PageBll, Enums.MenuList.MasterLocationMapping)
        {
            _locationMappingBLL = LocationMappingBLL;
            _pageBLL = PageBll;
            _employeeBLL = EmployeeBLL;
            _mainMenu = Enums.MenuList.MasterData;
        }
        public ActionResult Index()
        {
            var dto = new LocationMappingDto();
            var data = _locationMappingBLL.GetLocationMapping();
            var model = new LocationMappingModel();
            model.Details = Mapper.Map<List<LocationMappingItem>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View(model);
        }
        public ActionResult Create()
        {
            var model = new LocationMappingItem();
            var Locationlist = _employeeBLL.GetEmployee().Select(x => new { x.CITY }).ToList().Distinct().OrderBy(x => x.CITY);
            var BasetownList = _employeeBLL.GetEmployee().Select(x => new { x.BASETOWN }).Distinct().ToList().OrderBy(x => x.BASETOWN);
            model.LocationList = new SelectList(Locationlist, "CITY", "CITY");
            model.BasetownList = new SelectList(BasetownList, "BASETOWN", "BASETOWN");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(LocationMappingItem model)
        {
            if(ModelState.IsValid)
            {
             
                var data = Mapper.Map<LocationMappingDto>(model);

                data.CreatedDate = DateTime.Now;
                data.CreatedBy = CurrentUser.USER_ID;
                data.IsActive = true;

                var exist = _locationMappingBLL.GetLocationMapping().Where(x => (x.Address == null ? "" : x.Address.ToUpper()) == (data.Address==null ? "" :data.Address.ToUpper())
                    && (x.Basetown == null ? "" : x.Basetown.ToLower()) == (data.Basetown == null ? "" : data.Basetown.ToLower())
                    && (x.Location == null ? "" : x.Location.ToLower()) == (data.Location == null ? "" : data.Location.ToLower())
                    && (x.Region == null ? "" : x.Region.ToLower()) == (data.Region == null ? "" : data.Region.ToLower())
                    && (x.ZoneSales == null ? "" : x.ZoneSales.ToLower()) == (data.ZoneSales == null ? "" : data.ZoneSales.ToLower())
                    && (x.ZonePriceList == null ? "" : x.ZonePriceList.ToLower()) == (data.ZonePriceList == null ? "" : data.ZonePriceList.ToLower())
                    && x.IsActive).FirstOrDefault();

                if (exist != null)
                {
                    model.ErrorMessage = "Data Already Exsit";

                    var Locationlist = _employeeBLL.GetEmployee().Select(x => new { x.CITY }).ToList().Distinct().OrderBy(x => x.CITY);
                    var BasetownList = _employeeBLL.GetEmployee().Select(x => new { x.BASETOWN }).Distinct().ToList().OrderBy(x => x.BASETOWN);
                    model.LocationList = new SelectList(Locationlist, "CITY", "CITY");
                    model.BasetownList = new SelectList(BasetownList, "BASETOWN", "BASETOWN");

                    model.MainMenu = _mainMenu;
                    model.CurrentLogin = CurrentUser;
                    return View(model);
                }
                try
                {
                    _locationMappingBLL.Save(data);
                        
                }
                catch (Exception exception)
                {
                    model.ErrorMessage = exception.Message;

                    var Locationlist = _employeeBLL.GetEmployee().Select(x => new { x.CITY }).ToList().Distinct().OrderBy(x => x.CITY);
                    var BasetownList = _employeeBLL.GetEmployee().Select(x => new { x.BASETOWN }).Distinct().ToList().OrderBy(x => x.BASETOWN);
                    model.LocationList = new SelectList(Locationlist, "CITY", "CITY");
                    model.BasetownList = new SelectList(BasetownList, "BASETOWN", "BASETOWN");

                    model.MainMenu = _mainMenu;
                    model.CurrentLogin = CurrentUser;
                    return View(model);
                }
                try
                {
                    _locationMappingBLL.SaveChanges();
                }
                catch (Exception exception)
                {
                    var msg = exception.Message;
                    model.MainMenu = _mainMenu;
                    model.CurrentLogin = CurrentUser;
                    var Locationlist = _employeeBLL.GetEmployee().Select(x => new { x.CITY }).ToList().Distinct().OrderBy(x => x.CITY);
                    var BasetownList = _employeeBLL.GetEmployee().Select(x => new { x.BASETOWN }).Distinct().ToList().OrderBy(x => x.BASETOWN);
                    model.LocationList = new SelectList(Locationlist, "CITY", "CITY");
                    model.BasetownList= new SelectList(BasetownList, "BASETOWN", "BASETOWN");
                    return View(model);
                }
            }

            return RedirectToAction("Index", "MstLocationMapping");
        }
        public ActionResult Edit(int MstLocationMappingId)
        {
            var data = _locationMappingBLL.GetLocationMappingById(MstLocationMappingId);
            var model = Mapper.Map<LocationMappingItem>(data);
            var Locationlist = _employeeBLL.GetEmployee().Select(x => new { x.CITY }).ToList().Distinct().OrderBy(x => x.CITY);
            var BasetownList = _employeeBLL.GetEmployee().Select(x => new { x.BASETOWN }).Distinct().ToList().OrderBy(x => x.BASETOWN);
            model.LocationList = new SelectList(Locationlist, "CITY", "CITY");
            model.BasetownList = new SelectList(BasetownList, "BASETOWN", "BASETOWN");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterLocationMapping, MstLocationMappingId);
            return View(model);
        }
            
        [HttpPost]
        public ActionResult Edit(LocationMappingItem model)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<LocationMappingDto>(model);
                data.ModifiedDate = DateTime.Now;
                data.ModifiedBy = CurrentUser.USER_ID;
                try
                {
                    var exist = _locationMappingBLL.GetLocationMapping().Where(x => (x.Address == null ? "" : x.Address.ToUpper()) == (data.Address == null ? "" : data.Address.ToUpper())
                      && (x.Basetown == null ? "" : x.Basetown.ToLower()) == (data.Basetown == null ? "" : data.Basetown.ToLower())
                      && (x.Location == null ? "" : x.Location.ToLower()) == (data.Location == null ? "" : data.Location.ToLower())
                      && (x.Region == null ? "" : x.Region.ToLower()) == (data.Region == null ? "" : data.Region.ToLower())
                      && (x.ZoneSales == null ? "" : x.ZoneSales.ToLower()) == (data.ZoneSales == null ? "" : data.ZoneSales.ToLower())
                      && (x.ZonePriceList == null ? "" : x.ZonePriceList.ToLower()) == (data.ZonePriceList == null ? "" : data.ZonePriceList.ToLower())
                      && data.IsActive).FirstOrDefault();

                    if (exist != null)
                    {
                        model.ErrorMessage = "Data Already Exsit";

                        var Locationlist = _employeeBLL.GetEmployee().Select(x => new { x.CITY }).ToList().Distinct().OrderBy(x => x.CITY);
                        var BasetownList = _employeeBLL.GetEmployee().Select(x => new { x.BASETOWN }).Distinct().ToList().OrderBy(x => x.BASETOWN);
                        model.LocationList = new SelectList(Locationlist, "CITY", "CITY");
                        model.BasetownList = new SelectList(BasetownList, "BASETOWN", "BASETOWN");

                        model.MainMenu = _mainMenu;
                        model.CurrentLogin = CurrentUser;
                        return View(model);
                    }
                    _locationMappingBLL.Save(data, CurrentUser);
                    _locationMappingBLL.SaveChanges();

                }
                catch (Exception exp)
                {
                    model.ErrorMessage= exp.Message;

                    var Locationlist = _employeeBLL.GetEmployee().Select(x => new { x.CITY }).ToList().Distinct().OrderBy(x => x.CITY);
                    var BasetownList = _employeeBLL.GetEmployee().Select(x => new { x.BASETOWN }).Distinct().ToList().OrderBy(x => x.BASETOWN);
                    model.LocationList = new SelectList(Locationlist, "CITY", "CITY");
                    model.BasetownList = new SelectList(BasetownList, "BASETOWN", "BASETOWN");

                    model.MainMenu = _mainMenu;
                    model.CurrentLogin = CurrentUser;
                    return View(model);
                }
            }
            return RedirectToAction("Index", "MstLocationMapping");
        }

        public ActionResult Detail(int MstLocationMappingId)
        {
            var data = _locationMappingBLL.GetLocationMappingById(MstLocationMappingId);
            var model = Mapper.Map<LocationMappingItem>(data);
            var list = _employeeBLL.GetEmployee().Select(x => new { x.CITY }).ToList().Distinct().OrderBy(x => x.CITY);
            model.LocationList = new SelectList(list, "City", "City");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterLocationMapping, MstLocationMappingId);
            return View(model);
        }

        public ActionResult Upload()
        {
            var model = new LocationMappingModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }


        [HttpPost]
        public ActionResult Upload(LocationMappingModel Model)
        {
            if (ModelState.IsValid)
            {
                foreach (LocationMappingItem data in Model.Details)
                {
                    try
                    {
                        data.CreatedDate = DateTime.Now;
                        data.CreatedBy = CurrentUser.USERNAME;
                        data.ModifiedDate = null;
                        data.IsActive = true;
                        
//                        data.ValidFrom = Convert.ToDateTime(data.ValidFromS);
                        var dto = Mapper.Map<LocationMappingDto>(data);
                      
                        if (data.ErrorMessage == "" | data.ErrorMessage == null)
                        {
                            _locationMappingBLL.Save(dto); ;
                        }

                    }
                    catch (Exception exception)
                    {
                        var msg = exception.Message;
                        Model.MainMenu = _mainMenu;
                        Model.CurrentLogin = CurrentUser;
                        return View(Model);
                    }
                }
                try
                {
                    _locationMappingBLL.SaveChanges();
                }
                catch (Exception exception)
                {
                    var msg = exception.Message;
                    Model.MainMenu = _mainMenu;
                    Model.CurrentLogin = CurrentUser;
                    return View(Model);
                }
            }
            return RedirectToAction("Index", "MstLocationMapping");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<LocationMappingItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if(dataRow.Count <= 0)
                    {
                        continue;
                    }
                    if (dataRow[0] == "")
                    {
                        continue;
                    }

                    var item = new LocationMappingItem();
                    item.ErrorMessage = "";

                    item.Location = dataRow[0].ToString();
                    if (item.Location == "")
                    {
                        item.ErrorMessage = "Location list Can't be empty";
                    }

                    item.Address = dataRow[1].ToString();
                    if (item.Address == "")
                    {
                        item.ErrorMessage = "Address list Can't be empty";
                    }

                    item.Basetown = dataRow[2].ToString();
                    if (item.Basetown== "")
                    {
                        item.ErrorMessage = "Basetown list Can't be empty";
                    }

                    item.Region = dataRow[3].ToString();
                    item.ZoneSales = dataRow[4].ToString();

                    item.ZonePriceList = dataRow[5].ToString();
                    if (item.ZonePriceList == "")
                    {
                        item.ErrorMessage = "Zone Price list Can't be empty";
                    }

                    try
                    {
                        double dValidfrom = double.Parse(dataRow[6].ToString());
                        DateTime dtValidfrom = DateTime.FromOADate(dValidfrom);
                        item.ValidFrom = dtValidfrom;
                        item.ErrorMessage = "";
                    }
                    catch (Exception)
                    {
                        item.ErrorMessage = "Format Date is not Valid";
                        
                    }
                    var exist = _locationMappingBLL.GetLocationMapping().Where(x => x.Address == item.Address
                           && (x.Basetown == null ? "" : x.Basetown.ToLower()) == (item.Basetown == null ? "" : item.Basetown.ToLower())
                           && (x.Location == null ? "" : x.Location.ToLower()) == (item.Location == null ? "" : item.Location.ToLower())
                           && (x.Region == null ? "" : x.Region.ToLower()) == (item.Region == null ? "" : item.Region.ToLower())
                           && (x.ZoneSales == null ? "" : x.ZoneSales.ToLower()) == (item.ZoneSales == null ? "" : item.ZoneSales.ToLower())
                           && (x.ZonePriceList == null ? "" : x.ZonePriceList.ToLower()) == (item.ZonePriceList == null ? "" : item.ZonePriceList.ToLower())
                           && x.IsActive).FirstOrDefault();

                    if (exist != null) item.ErrorMessage = "Data Already Exist";
                    model.Add(item);
                }
            }
            return Json(model);
        }

        #region -----------------Json Result ----------------
        [HttpPost]
        public JsonResult GetAddress(string Location, string Basetown)
        {
            var GetAddress= _employeeBLL.GetEmployee().Where(x => (x.CITY == null ? "" : x.CITY.ToUpper()) == (Location == null ? "" : Location.ToUpper())
                                                                  && (x.BASETOWN == null ? "" : x.BASETOWN.ToUpper()) == (Basetown == null ? "" : Basetown.ToUpper())).FirstOrDefault();
            var Address = "";
            if (GetAddress != null) Address = GetAddress.ADDRESS;
            return Json(Address);
        }
        [HttpPost]
        public JsonResult GetBasetownList(string Location)
        {
            var BasetownList = _employeeBLL.GetEmployee().Where(x => (x.CITY == null ? "" : x.CITY.ToUpper()) == (Location == null ? "" : Location.ToUpper())).Select(x => new { x.BASETOWN }).Distinct().OrderBy(x => x.BASETOWN).ToList();
            return Json(BasetownList);
        }
        #endregion

        #region export xls
        public void ExportMasterLocationMapping()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterLocationMapping();

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

        private string CreateXlsMasterLocationMapping()
        {
            //get data
            List<LocationMappingDto> LocationMapping = _locationMappingBLL.GetLocationMapping();
            var listData = Mapper.Map<List<LocationMappingItem>>(LocationMapping);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Location Mapping");
            slDocument.MergeWorksheetCells(1, 1, 1,12);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterLocationMapping(slDocument);

            //create data
            slDocument = CreateDataExcelMasterLocationMapping(slDocument, listData);

            var fileName = "Master_Data_Location_Mapping" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterLocationMapping(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Location");
            slDocument.SetCellValue(iRow, 2, "Address");
            slDocument.SetCellValue(iRow, 3, "Basetown");
            slDocument.SetCellValue(iRow, 4, "Region");
            slDocument.SetCellValue(iRow, 5, "Zone Sales");
            slDocument.SetCellValue(iRow, 6, "Zone Price List");
            slDocument.SetCellValue(iRow, 7, "Validity From");
            slDocument.SetCellValue(iRow, 8, "Created Date");
            slDocument.SetCellValue(iRow, 9, "Created By");
            slDocument.SetCellValue(iRow, 10, "Modified Date");
            slDocument.SetCellValue(iRow, 11, "Modified By");
            slDocument.SetCellValue(iRow, 12, "Status");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 12, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelMasterLocationMapping(SLDocument slDocument, List<LocationMappingItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.Location );
                slDocument.SetCellValue(iRow, 2, data.Address);
                slDocument.SetCellValue(iRow, 3, data.Basetown);
                slDocument.SetCellValue(iRow, 4, data.Region );
                slDocument.SetCellValue(iRow, 5, data.ZoneSales);
                slDocument.SetCellValue(iRow, 6, data.ZonePriceList);
                slDocument.SetCellValue(iRow, 7, data.ValidFrom.Value.ToString("dd-MMM-yyyy hh:mm"));
                slDocument.SetCellValue(iRow, 8, data.CreatedDate.Value.ToString("dd-MMM-yyyy hh:mm"));
                slDocument.SetCellValue(iRow, 9, data.CreatedBy);
                slDocument.SetCellValue(iRow, 10, data.ModifiedDate == null ? data.CreatedDate.Value.ToString("dd-MMM-yyyy hh:mm") : data.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 11, data.ModifiedBy == null ? data.CreatedBy : data.ModifiedBy);
                if (data.IsActive)
                {
                    slDocument.SetCellValue(iRow, 12, "Active");
                }
                else
                {
                    slDocument.SetCellValue(iRow, 12, "InActive");
                }

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 12);
            slDocument.SetCellStyle(3, 1, iRow - 1, 12, valueStyle);

            return slDocument;
        }

        #endregion
    }
}
