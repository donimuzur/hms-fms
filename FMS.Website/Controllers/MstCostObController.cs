
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
using FMS.BusinessObject.Inputs;
using FMS.Utils;

namespace FMS.Website.Controllers
{
    public class MstCostObController : BaseController
    {
        private ICostObBLL _costObBLL;
        private IPageBLL _pageBLL;
        private IRemarkBLL _remarkBLL;
        private ISettingBLL _settingBLL;
        private IGroupCostCenterBLL _functionGroupBll;
        private ILocationMappingBLL _locationMappingBLL;
        private IVehicleSpectBLL _vehicleSpectBll;
        private Enums.MenuList _mainMenu;

        public MstCostObController(IPageBLL PageBll, ICostObBLL CostObBLL, IRemarkBLL RemarkBLL, ILocationMappingBLL LocationMappingBLL, IVehicleSpectBLL VehicleSpectBll ,ISettingBLL SettingBLL, IGroupCostCenterBLL FunctionGroupBLL)
            : base(PageBll, Enums.MenuList.MasterCostOB)
        {
            _costObBLL = CostObBLL;
            _remarkBLL = RemarkBLL;
            _settingBLL = SettingBLL;
            _functionGroupBll = FunctionGroupBLL;
            _locationMappingBLL = LocationMappingBLL;
            _pageBLL = PageBll;
            _vehicleSpectBll = VehicleSpectBll;
            _mainMenu = Enums.MenuList.MasterData;

        }

        public CostObItem InitialModel(CostObItem model)
        {
            var ZoneList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive == true).ToList();
            model.ZoneList = new SelectList(ZoneList, "ZonePriceList", "ZonePriceList");

            var ModelList = new List<SelectListItem>
            {
                new SelectListItem { Text = "MPV", Value = "MPV" },
                new SelectListItem { Text = "SUV", Value = "SUV" },
                new SelectListItem { Text = "Forklift", Value = "Forklift" },
                new SelectListItem { Text = "Motorcycle", Value = "Motorcycle" },
                new SelectListItem { Text = "Truck", Value = "Truck" }
            };
            model.ModelList = new SelectList(ModelList, "Value", "Text");

            var TypeList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Benefit", Value = "Benefit" },
                new SelectListItem { Text = "WTC", Value = "WTC" },
            };
            model.TypeList = new SelectList(TypeList, "Value", "Text");

            var MonthList = new List<SelectListItem>()
            {
                new SelectListItem() {Text = "January", Value = "1" },
                new SelectListItem() {Text = "February", Value = "2" },
                new SelectListItem() {Text = "March", Value = "3" },
                new SelectListItem() {Text = "April", Value = "4" },
                new SelectListItem() {Text = "May", Value = "5" },
                new SelectListItem() {Text = "June", Value = "6" },
                new SelectListItem() {Text = "July", Value = "7" },
                new SelectListItem() {Text = "August", Value = "8" },
                new SelectListItem() {Text = "September", Value = "9" },
                new SelectListItem() {Text = "October", Value = "10" },
                new SelectListItem() {Text = "November", Value = "11" },
                new SelectListItem() {Text = "December", Value = "12" }
            };
            model.MonthList = new SelectList(MonthList, "Value", "Text");

            return model;
        }
        public ActionResult Index()
        {
            var model = new CostObModel();

            model.SearchView.Year = DateTime.Now.Year;
            var VehicleTypeList = _settingBLL.GetSetting().Where(x => x.IsActive && x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType)).Select(x => new { x.SettingName}).Distinct().ToList();
            var FunctionList = _functionGroupBll.GetGroupCenter().Where(x => x.IsActive).Select(x => new { x.FunctionName }).Distinct().ToList();
            var LocationMappingList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).Select(x => new { x.Region }).Distinct().ToList();

            model.SearchView.VehicleTypeList = new SelectList(VehicleTypeList, "SettingName", "SettingName");
            model.SearchView.FunctionList = new SelectList(FunctionList, "FunctionName", "FunctionName");
            model.SearchView.RegionalList = new SelectList(LocationMappingList, "Region", "Region");
            
            var filter = new CostObParamInput();
            filter.Year = DateTime.Now.Year; 

            var data = _costObBLL.GetByFilter(filter);
            
            model.Details = Mapper.Map<List<CostObItem>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            foreach (CostObItem item in model.Details)
            {
                item.MonthS = this.SetMonthToString(item.Month == null ? 0:item.Month.Value);
            }
            return View(model);
        }

        public string SetMonthToString(int Month)
        {
            if (Month == 0 )
            {
                return "Month 0 is not exist";
            }
            else if (Month == 1)
            {
                return "January";
            }
            else if (Month == 2)
            {
                return "February";
            }
            else if (Month == 3)
            {
                return "March";
            }
            else if (Month == 4)
            {
                return "April";
            }
            else if (Month == 5)
            {
                return "May";
            }
            else if (Month == 6)
            {
                return "June";
            }
            else if (Month == 7)
            {
                return "Juli";
            }
            else if (Month == 8)
            {
                return "August";
            }
            else if (Month == 9)
            {
                return "September";
            }
            else if (Month == 10)
            {
                return "November";
            }
            else if (Month == 11)
            {
                return "October";
            }
            else if (Month == 12)
            {
                return "December";
            }

            return "An Error Occurred";
        }


        public ActionResult Create()
        {
            var model = new CostObItem();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model = InitialModel(model);
            model.ObCost = null;
            model.Year = null;
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(CostObItem item)
        {
            string year = Request.Params["Year"];
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<CostObDto>(item);
                data.CreatedBy = CurrentUser.USER_ID;
                data.CreatedDate = DateTime.Now;
                data.IsActive = true;
                try
                {
                    _costObBLL.Save(data);
                }
                catch (Exception ex)
                {

                    item.ErrorMessage = ex.Message;
                    item.MainMenu = _mainMenu;
                    item.CurrentLogin = CurrentUser;
                    item = InitialModel(item);
                    return View(item);
                }

            }
            return RedirectToAction("Index", "MstCostOb");
        }

        public ActionResult Edit(int? MstCostObid)
        {
            if (!MstCostObid.HasValue)
            {
                return HttpNotFound();
            }

            var data = _costObBLL.GetByID(MstCostObid.Value);
            var model = new CostObItem();
            model = Mapper.Map<CostObItem>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model = InitialModel(model);
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterCostOB, MstCostObid.Value);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CostObItem item)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<CostObDto>(item);
                data.ModifiedDate = DateTime.Now;
                data.ModifiedBy = CurrentUser.USER_ID;

                try
                {
                    _costObBLL.Save(data, CurrentUser);
                }
                catch (Exception ex)
                {
                    item.ErrorMessage = ex.Message;
                    item.MainMenu = _mainMenu;
                    item.CurrentLogin = CurrentUser;
                    item = InitialModel(item);
                }
            }
            return RedirectToAction("Index", "MstCostOb");
        }

        public ActionResult Detail(int MstCostObid)
        {
            var data = _costObBLL.GetByID(MstCostObid);
            var model = new CostObItem();
            model = Mapper.Map<CostObItem>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model = InitialModel(model);
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterCostOB, MstCostObid);
            return View(model);
        }

        public ActionResult Upload()
        {
            var model = new CostObModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }


        [HttpPost]
        public ActionResult Upload(CostObModel Model)
        {
            var data = (new ExcelReader()).ReadExcel(Model.upload);
            var model = new List<CostObItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow.Count <= 4)
                    {
                        continue;
                    }
                    if (dataRow[5] == "")
                    {
                        continue;
                    }
                    var item = new CostObItem();
                    item.ErrorMessage = "";
                    try
                    {
                        for (int i = 5; i <= data.Headers.Count() - 1; i++)
                        {

                            item = new CostObItem();
                            item.ErrorMessage = "";

                            item.CostCenter = dataRow[0];
                            if (item.CostCenter == "")
                            {
                                item.ErrorMessage = "Cost Center can't be empty";
                            }

                            item.FunctionName = dataRow[1];
                            if (item.FunctionName == "")
                            {
                                item.ErrorMessage = "Function Can't be empty";
                            }

                            item.Regional = dataRow[2];

                            item.VehicleType = dataRow[3];
                            if (item.VehicleType == "")
                            {
                                item.ErrorMessage = "Vehicle Type Can't be empty";
                            }
                            

                            if (data.Headers[i] == "" || data.Headers[i] == null)
                            {
                                continue;
                            }
                            var Header = data.Headers[i].Split('_');
                            if (Header.Count() > 1)
                            {
                                var Type = Header[0];
                                item.Type = "";
                                item.ObCost = null;
                                item.Qty = null;

                                var Time = Header[1].Split('-');
                                if (Time.Count() > 1)
                                {
                                    if (Type.ToUpper() == "QTY")
                                    {
                                        item.Type = "QTY";
                                        try
                                        {
                                            item.Qty = Convert.ToInt32(dataRow[i]);
                                        }
                                        catch (Exception)
                                        {
                                            item.ErrorMessage = "Qty must be number";
                                        }
                                        item.Month = Convert.ToInt32(Time[0]);
                                        item.Year = Convert.ToInt32(Time[1]);

                                        var exist = _costObBLL.GetCostOb().Where(x => (x.FunctionName == null ? "" : x.FunctionName.ToUpper()) == (item.FunctionName == null ? "" : item.FunctionName.ToUpper())
                                                            && (x.CostCenter == null ? "" : x.CostCenter.ToUpper()) == (item.CostCenter == null ? "" : item.CostCenter.ToUpper())
                                                            && (x.Regional == null ? "" : x.Regional.ToUpper()) == (item.Regional == null ? "" : item.Regional.ToUpper())
                                                            && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (item.VehicleType == null ? "" : item.VehicleType.ToUpper())
                                                            && (x.Type == null ? "" : x.Type.ToUpper()) == (item.Type == null ? "" : item.Type.ToUpper() )
                                                            && x.Month == item.Month && x.Year == item.Year && x.IsActive).FirstOrDefault();
                                        if(exist != null)
                                        {
                                            exist.IsActive = false;
                                            exist.ModifiedBy = "SYSTEM";
                                            exist.ModifiedDate = DateTime.Now;
                                            _costObBLL.Save(exist);
                                        }

                                        var dto = Mapper.Map<CostObDto>(item);
                                        dto.CreatedBy = CurrentUser.USER_ID;
                                        dto.CreatedDate = DateTime.Now;
                                        dto.IsActive = true;
                                        _costObBLL.Save(dto);
                                    }
                                    else
                                    {
                                        item.Type = Type;
                                        try
                                        {
                                            item.ObCost = Convert.ToDecimal(dataRow[i]);
                                        }
                                        catch (Exception)
                                        {
                                            item.ErrorMessage = "Cost OB must be number";
                                        }
                                        item.Month = Convert.ToInt32(Time[0]);
                                        item.Year = Convert.ToInt32(Time[1]);

                                        var exist = _costObBLL.GetCostOb().Where(x => (x.FunctionName == null ? "" : x.FunctionName.ToUpper()) == (item.FunctionName == null ? "" : item.FunctionName.ToUpper())
                                                         && (x.CostCenter == null ? "" : x.CostCenter.ToUpper()) == (item.CostCenter == null ? "" : item.CostCenter.ToUpper())
                                                         && (x.Regional == null ? "" : x.Regional.ToUpper()) == (item.Regional == null ? "" : item.Regional.ToUpper())
                                                         && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (item.VehicleType == null ? "" : item.VehicleType.ToUpper())
                                                         && (x.Type == null ? "" : x.Type.ToUpper()) == (item.Type == null ? "" : item.Type.ToUpper())
                                                         && x.Month == item.Month && x.Year == item.Year && x.IsActive).FirstOrDefault();
                                        if (exist != null)
                                        {
                                            exist.IsActive = false;
                                            exist.ModifiedBy = "SYSTEM";
                                            exist.ModifiedDate = DateTime.Now;
                                            _costObBLL.Save(exist);
                                        }

                                        var dto = Mapper.Map<CostObDto>(item);
                                        dto.CreatedBy = CurrentUser.USER_ID;
                                        dto.CreatedDate = DateTime.Now;
                                        dto.IsActive = true;
                                        _costObBLL.Save(dto);
                                    }
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        item.ErrorMessage = ex.Message;
                    }
                }
                try
                {
                    _costObBLL.SaveChanges();
                }
                catch (Exception exp)
                {
                    Model.ErrorMessage = exp.Message;
                    Model.MainMenu = _mainMenu;
                    Model.CurrentLogin = CurrentUser;
                    return View(Model);
                }
            }
            return RedirectToAction("Index", "MstCostOb");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<CostObItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow.Count <= 4)
                    {
                        continue;
                    }
                    if (dataRow[5] == "")
                    {
                        continue;
                    }
                    var item = new CostObItem();
                    item.ErrorMessage = "";
                    try
                    {
                        for (int i = 5; i <= data.Headers.Count() - 1; i++)
                        {

                            item = new CostObItem();
                            item.ErrorMessage = "";

                            item.CostCenter = dataRow[0];
                            if (item.CostCenter == "")
                            {
                                item.ErrorMessage = "Cost Center can't be empty";
                            }

                            item.FunctionName = dataRow[1];
                            if (item.FunctionName == "")
                            {
                                item.ErrorMessage = "Function Can't be empty";
                            }

                            item.Regional = dataRow[2];

                            item.VehicleType = dataRow[3];
                            if (item.VehicleType == "")
                            {
                                item.ErrorMessage = "Vehicle Type Can't be empty";
                            }

                            if (data.Headers[i] == "" || data.Headers[i] == null)
                            {
                                continue;
                            }
                            var Header = data.Headers[i].Split('_');
                            if (Header.Count() > 1)
                            {
                                var Type = Header[0];
                                item.Type = "";
                                item.ObCost = null;
                                item.Qty = null;

                                var Time = Header[1].Split('-');
                                if (Time.Count() > 1)
                                {
                                    if (Type.ToUpper() == "QTY")
                                    {
                                        item.Type = "QTY";
                                        try
                                        {
                                            item.Qty = Convert.ToInt32(dataRow[i]);
                                        }
                                        catch (Exception)
                                        {
                                            item.ErrorMessage = "Qty must be number";
                                        }
                                        item.Month = Convert.ToInt32(Time[0]);
                                        item.Year = Convert.ToInt32(Time[1]);
                                        model.Add(item);
                                    }
                                    else
                                    {
                                        item.Type = Type;
                                        try
                                        {
                                            item.ObCost = Convert.ToDecimal(dataRow[i]);
                                        }
                                        catch (Exception)
                                        {
                                            item.ErrorMessage = "Cost OB must be number";
                                        }
                                        item.Month = Convert.ToInt32(Time[0]);
                                        item.Year = Convert.ToInt32(Time[1]);
                                        model.Add(item);
                                    }
                                }
                            }
                        }
                       
                    }
                    catch (Exception ex)
                    {
                        item.ErrorMessage = ex.Message;
                    }
                }
            }
            if(model.Where(x => x.ErrorMessage != null && x.ErrorMessage!="").ToList().Count > 0)
            {
                model = model.Where(x => x.ErrorMessage != null && x.ErrorMessage != "").ToList();
                return Json(model);
            }
            else
            {
                return Json(model);
            }
            
        }

        [HttpPost]
        public PartialViewResult ListCostOb(CostObModel model)
        {
            model.Details = new List<CostObItem>();
            model.Details = GetCostOb(model.SearchView);
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            foreach (CostObItem item in model.Details)
            {
                item.MonthS = this.SetMonthToString(item.Month == null ? 0 : item.Month.Value);
            }
            return PartialView("_ListCostOb", model);
        }

        private List<CostObItem> GetCostOb(CostObSearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _costObBLL.GetByFilter(new CostObParamInput());
                return Mapper.Map<List<CostObItem>>(data);
            }

            //getbyparams
            var input = Mapper.Map<CostObParamInput>(filter);

            var dbData = _costObBLL.GetByFilter(input);
            return Mapper.Map<List<CostObItem>>(dbData);
        }


        #region export xls
        public void ExportMasterCostOb(CostObModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsMasterCostOb(model);

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

        private string CreateXlsMasterCostOb(CostObModel model)
        {
            //get data
            var costOb = new List<CostObItem>();
            costOb = GetCostOb(model.SearchView);
            var listData = Mapper.Map<List<CostObItem>>(costOb);
            

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Cost OB");
            slDocument.MergeWorksheetCells(1, 1, 1, 11);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterCostOb(slDocument);

            //create data
            slDocument = CreateDataExcelMasterCostOb(slDocument, listData);

            var fileName = "Master_Data_CostOb" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterCostOb(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Year");
            slDocument.SetCellValue(iRow, 2, "Month");
            slDocument.SetCellValue(iRow, 3, "Cost Center");
            slDocument.SetCellValue(iRow, 4, "Vehicle Type");
            slDocument.SetCellValue(iRow, 5, "Type");
            slDocument.SetCellValue(iRow, 6, "Function Name");
            slDocument.SetCellValue(iRow, 7, "Regional");
            slDocument.SetCellValue(iRow, 8, "Cost OB");
            slDocument.SetCellValue(iRow, 9, "Qty");
            slDocument.SetCellValue(iRow, 10, "Created By");
            slDocument.SetCellValue(iRow, 11, "Created Date");
            slDocument.SetCellValue(iRow, 12, "Modified By");
            slDocument.SetCellValue(iRow, 13, "Modified Date");
            slDocument.SetCellValue(iRow, 14, "Status");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 14, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelMasterCostOb(SLDocument slDocument, List<CostObItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.Year == null ? "" : data.Year.Value.ToString());
                slDocument.SetCellValue(iRow, 2, SetMonthToString(data.Month == null ? 0 : data.Month.Value));
                slDocument.SetCellValue(iRow, 3, data.CostCenter);
                slDocument.SetCellValue(iRow, 4, data.VehicleType);
                slDocument.SetCellValue(iRow, 5, data.Type);
                slDocument.SetCellValue(iRow, 6, data.FunctionName);
                slDocument.SetCellValue(iRow, 7, data.Regional);
                slDocument.SetCellValue(iRow, 8, data.ObCost == null ? "" : string.Format("{0:N0}", data.ObCost));
                slDocument.SetCellValue(iRow, 9, data.Qty == null ? "" : data.Qty.Value.ToString());
                slDocument.SetCellValue(iRow, 10, data.CreatedBy);
                slDocument.SetCellValue(iRow, 11, data.CreatedDate.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 12, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 13, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy"));
                if (data.IsActive)
                {
                    slDocument.SetCellValue(iRow, 14, "Active");
                }
                else
                {
                    slDocument.SetCellValue(iRow, 14, "InActive");
                }

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 14);
            slDocument.SetCellStyle(3, 1, iRow - 1, 14, valueStyle);

            return slDocument;
        }

        #endregion
    }


}