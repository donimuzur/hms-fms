using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.Contract.BLL;
using FMS.Core;
using AutoMapper;
using FMS.Website.Models;
using FMS.BusinessObject.Dto;
using FMS.Website.Utility;
using System.IO;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;

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
            return View(model);
        }
        public ActionResult Create()
        {
            var model = new LocationMappingItem();
            var list = _employeeBLL.GetEmployee().Select(x => new { x.CITY }).ToList().Distinct().OrderBy(x => x.CITY);
            model.LocationList = new SelectList(list, "City", "City");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(LocationMappingItem model)
        {
            if(ModelState.IsValid)
            {
                var addressList = _employeeBLL.GetEmployee().Where(x => x.CITY == model.Location).Select(x=>x.ADDRESS).Distinct().ToList();
                foreach(var address in addressList )
                { 
                    var data = Mapper.Map<LocationMappingDto>(model);
                    
                    
                    data.ValidFrom = DateTime.Now;
                    data.CreatedDate = DateTime.Now;
                    data.Address = address;
                    data.CreatedBy = CurrentUser.USERNAME;
                    data.IsActive = true;
                    try
                    {
                        _locationMappingBLL.Save(data);
                    }
                    catch (Exception)
                    {
                        return View(model);
                    }
                }
            }

            return RedirectToAction("Index", "MstLocationMapping");
        }
        public ActionResult Edit(int MstLocationMappingId)
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

        [HttpPost]
        public ActionResult Edit(LocationMappingItem model)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<LocationMappingDto>(model);
                data.ValidFrom = DateTime.Now;
                data.ModifiedDate = DateTime.Now;
                data.ModifiedBy = CurrentUser.USERNAME;
                try
                {
                    _locationMappingBLL.Save(data, CurrentUser);

                }
                catch (Exception exp)
                {
                    var error = exp.Message;
                    throw;
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
                        
                        data.ValidFrom = Convert.ToDateTime(data.ValidFromS);
                        var dto = Mapper.Map<LocationMappingDto>(data);
                        if (data.ErrorMessage == "" | data.ErrorMessage == null)
                        {
                            _locationMappingBLL.Save(dto); ;
                        }
                        
                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                    }
                    catch (Exception exception)
                    {
                        AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                        return View(Model);
                    }
                }
            }
            return RedirectToAction("Index", "MstLocationMapping");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var qtyPacked = string.Empty;
            var qty = string.Empty;

            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<LocationMappingItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    var item = new LocationMappingItem();
                    item.Location = dataRow[0].ToString();
                    item.Address = dataRow[1].ToString();
                    item.Region = dataRow[2].ToString();
                    item.ZoneSales = dataRow[3].ToString();
                    item.ZonePriceList = dataRow[4].ToString();
                    try
                    {
                        item.ValidFromS = dataRow[5];
                        item.ErrorMessage = "";
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
            slDocument.MergeWorksheetCells(1, 1, 1,11);
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
            slDocument.SetCellValue(iRow, 3, "Region");
            slDocument.SetCellValue(iRow, 4, "Zone Sales");
            slDocument.SetCellValue(iRow, 5, "Zone Price List");
            slDocument.SetCellValue(iRow, 6, "Validity From");
            slDocument.SetCellValue(iRow, 7, "Created Date");
            slDocument.SetCellValue(iRow, 8, "Created By");
            slDocument.SetCellValue(iRow, 9, "Modified Date");
            slDocument.SetCellValue(iRow, 10, "Modified By");
            slDocument.SetCellValue(iRow, 11, "Status");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 11, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelMasterLocationMapping(SLDocument slDocument, List<LocationMappingItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.Location );
                slDocument.SetCellValue(iRow, 2, data.Address);
                slDocument.SetCellValue(iRow, 3, data.Region );
                slDocument.SetCellValue(iRow, 4, data.ZoneSales);
                slDocument.SetCellValue(iRow, 5, data.ZonePriceList);
                slDocument.SetCellValue(iRow, 6, data.ValidFrom.ToString("dd - MM - yyyy hh: mm") );
                slDocument.SetCellValue(iRow, 7, data.CreatedDate.ToString("dd - MM - yyyy hh: mm"));
                slDocument.SetCellValue(iRow, 8, data.CreatedBy);
                slDocument.SetCellValue(iRow, 9, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd - MM - yyyy hh: mm" ) );
                slDocument.SetCellValue(iRow, 10, data.ModifiedBy);
                if (data.IsActive)
                {
                    slDocument.SetCellValue(iRow, 11, "Active");
                }
                else
                {
                    slDocument.SetCellValue(iRow, 11, "InActive");
                }

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 11);
            slDocument.SetCellStyle(3, 1, iRow - 1, 11, valueStyle);

            return slDocument;
        }

        #endregion
    }
}
