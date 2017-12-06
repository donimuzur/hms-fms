
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
    public class MstCostObController : BaseController
    {
        private ICostObBLL _costObBLL;
        private IPageBLL _pageBLL;
        private IRemarkBLL _remarkBLL;
        private ILocationMappingBLL _locationMappingBLL;
        private IVehicleSpectBLL _vehicleSpectBll;
        private Enums.MenuList _mainMenu;

        public MstCostObController(IPageBLL PageBll, ICostObBLL CostObBLL, IRemarkBLL RemarkBLL, ILocationMappingBLL LocationMappingBLL, IVehicleSpectBLL VehicleSpectBll)
            : base(PageBll, Enums.MenuList.MasterCostOB)
        {
            _costObBLL = CostObBLL;
            _remarkBLL = RemarkBLL;
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
            var data = _costObBLL.GetCostOb();
            var model = new CostObModel();
            model.Details = Mapper.Map<List<CostObItem>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            model.CurrentPage = 0;
            foreach (CostObItem item in model.Details)
            {
                Decimal ObCostNotNull = (Decimal)item.ObCost;
                item.ObCostS = ObCostNotNull.ToString("0,000.00");
                item.MonthS = this.SetMonthToString(item.Month);
            }
            return View(model);
        }

        public string SetMonthToString(int Month)
        {
            if (Month == 0)
            {
                return "Month 0 is not exist";
            }
            else if (Month == 1)
            {
                return "Jan";
            }
            else if (Month == 2)
            {
                return "Feb";
            }
            else if (Month == 3)
            {
                return "Mar";
            }
            else if (Month == 4)
            {
                return "Apr";
            }
            else if (Month == 5)
            {
                return "May";
            }
            else if (Month == 6)
            {
                return "Jun";
            }
            else if (Month == 7)
            {
                return "Jul";
            }
            else if (Month == 8)
            {
                return "Aug";
            }
            else if (Month == 9)
            {
                return "Sep";
            }
            else if (Month == 10)
            {
                return "Nov";
            }
            else if (Month == 11)
            {
                return "Oct";
            }
            else if (Month == 12)
            {
                return "Dec";
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
            foreach (CostOBUpload data in Model.UploadedData)
            {
                try
                {
                    CostObItem item = new CostObItem();
                    item.CostCenter = data.CostCenter;
                    item.Model = data.BodyType;
                    item.Type = data.VehicleType;
                    item.ObCost = data.SumOfJan;
                    item.Qty = data.SumOfQtyJan;
                    item.Year = DateTime.Now.Year + 1;
                    item.Month = 1;
                    item.CreatedDate = DateTime.Now;
                    item.CreatedBy = CurrentUser.USER_ID;
                    item.IsActive = true;

                    if (item.ErrorMessage == "" | item.ErrorMessage == null)
                    {
                        var dto = Mapper.Map<CostObDto>(item);

                        _costObBLL.Save(dto);
                    }

                    item = new CostObItem();
                    item.CostCenter = data.CostCenter;
                    item.Model = data.BodyType;
                    item.Type = data.VehicleType;
                    item.ObCost = data.SumOfFeb;
                    item.Qty = data.SumOfQtyFeb;
                    item.Year = DateTime.Now.Year + 1;
                    item.Month = 2;
                    item.CreatedDate = DateTime.Now;
                    item.CreatedBy = CurrentUser.USER_ID;
                    item.IsActive = true;

                    if (item.ErrorMessage == "" | item.ErrorMessage == null)
                    {
                        var dto = Mapper.Map<CostObDto>(item);

                        _costObBLL.Save(dto);
                    }

                    item = new CostObItem();
                    item.CostCenter = data.CostCenter;
                    item.Model = data.BodyType;
                    item.Type = data.VehicleType;
                    item.ObCost = data.SumOfMar;
                    item.Qty = data.SumOfQtyMar;
                    item.Year = DateTime.Now.Year + 1;
                    item.Month = 3;
                    item.CreatedDate = DateTime.Now;
                    item.CreatedBy = CurrentUser.USER_ID;
                    item.IsActive = true;

                    if (item.ErrorMessage == "" | item.ErrorMessage == null)
                    {
                        var dto = Mapper.Map<CostObDto>(item);

                        _costObBLL.Save(dto);
                    }

                    item = new CostObItem();
                    item.CostCenter = data.CostCenter;
                    item.Model = data.BodyType;
                    item.Type = data.VehicleType;
                    item.ObCost = data.SumOfApr;
                    item.Qty = data.SumOfQtyApr;
                    item.Year = DateTime.Now.Year + 1;
                    item.Month = 4;
                    item.CreatedDate = DateTime.Now;
                    item.CreatedBy = CurrentUser.USER_ID;
                    item.IsActive = true;

                    if (item.ErrorMessage == "" | item.ErrorMessage == null)
                    {
                        var dto = Mapper.Map<CostObDto>(item);

                        _costObBLL.Save(dto);
                    }

                    item = new CostObItem();
                    item.CostCenter = data.CostCenter;
                    item.Model = data.BodyType;
                    item.Type = data.VehicleType;
                    item.ObCost = data.SumOfMay;
                    item.Qty = data.SumOfQtyMay;
                    item.Year = DateTime.Now.Year + 1;
                    item.Month = 5;
                    item.CreatedDate = DateTime.Now;
                    item.CreatedBy = CurrentUser.USER_ID;
                    item.IsActive = true;

                    if (item.ErrorMessage == "" | item.ErrorMessage == null)
                    {
                        var dto = Mapper.Map<CostObDto>(item);

                        _costObBLL.Save(dto);
                    }

                    item = new CostObItem();
                    item.CostCenter = data.CostCenter;
                    item.Model = data.BodyType;
                    item.Type = data.VehicleType;
                    item.ObCost = data.SumOfJun;
                    item.Qty = data.SumOfQtyJun;
                    item.Year = DateTime.Now.Year + 1;
                    item.Month = 6;
                    item.CreatedDate = DateTime.Now;
                    item.CreatedBy = CurrentUser.USER_ID;
                    item.IsActive = true;

                    if (item.ErrorMessage == "" | item.ErrorMessage == null)
                    {
                        var dto = Mapper.Map<CostObDto>(item);

                        _costObBLL.Save(dto);
                    }

                    item = new CostObItem();
                    item.CostCenter = data.CostCenter;
                    item.Model = data.BodyType;
                    item.Type = data.VehicleType;
                    item.ObCost = data.SumOfJul;
                    item.Qty = data.SumOfQtyJul;
                    item.Year = DateTime.Now.Year + 1;
                    item.Month = 7;
                    item.CreatedDate = DateTime.Now;
                    item.CreatedBy = CurrentUser.USER_ID;
                    item.IsActive = true;

                    if (item.ErrorMessage == "" | item.ErrorMessage == null)
                    {
                        var dto = Mapper.Map<CostObDto>(item);

                        _costObBLL.Save(dto);
                    }

                    item = new CostObItem();
                    item.CostCenter = data.CostCenter;
                    item.Model = data.BodyType;
                    item.Type = data.VehicleType;
                    item.ObCost = data.SumOfAug;
                    item.Qty = data.SumOfQtyAug;
                    item.Year = DateTime.Now.Year + 1;
                    item.Month = 8;
                    item.CreatedDate = DateTime.Now;
                    item.CreatedBy = CurrentUser.USER_ID;
                    item.IsActive = true;

                    if (item.ErrorMessage == "" | item.ErrorMessage == null)
                    {
                        var dto = Mapper.Map<CostObDto>(item);

                        _costObBLL.Save(dto);
                    }

                    item = new CostObItem();
                    item.CostCenter = data.CostCenter;
                    item.Model = data.BodyType;
                    item.Type = data.VehicleType;
                    item.ObCost = data.SumOfSep;
                    item.Qty = data.SumOfQtySep;
                    item.Year = DateTime.Now.Year + 1;
                    item.Month = 9;
                    item.CreatedDate = DateTime.Now;
                    item.CreatedBy = CurrentUser.USER_ID;
                    item.IsActive = true;

                    if (item.ErrorMessage == "" | item.ErrorMessage == null)
                    {
                        var dto = Mapper.Map<CostObDto>(item);

                        _costObBLL.Save(dto);
                    }

                    item = new CostObItem();
                    item.CostCenter = data.CostCenter;
                    item.Model = data.BodyType;
                    item.Type = data.VehicleType;
                    item.ObCost = data.SumOfOct;
                    item.Qty = data.SumOfQtyOct;
                    item.Year = DateTime.Now.Year + 1;
                    item.Month = 10;
                    item.CreatedDate = DateTime.Now;
                    item.CreatedBy = CurrentUser.USER_ID;
                    item.IsActive = true;

                    if (item.ErrorMessage == "" | item.ErrorMessage == null)
                    {
                        var dto = Mapper.Map<CostObDto>(item);

                        _costObBLL.Save(dto);
                    }

                    item = new CostObItem();
                    item.CostCenter = data.CostCenter;
                    item.Model = data.BodyType;
                    item.Type = data.VehicleType;
                    item.ObCost = data.SumOfNov;
                    item.Qty = data.SumOfQtyNov;
                    item.Year = DateTime.Now.Year + 1;
                    item.Month = 11;
                    item.CreatedDate = DateTime.Now;
                    item.CreatedBy = CurrentUser.USER_ID;
                    item.IsActive = true;

                    if (item.ErrorMessage == "" | item.ErrorMessage == null)
                    {
                        var dto = Mapper.Map<CostObDto>(item);

                        _costObBLL.Save(dto);
                    }

                    item = new CostObItem();
                    item.CostCenter = data.CostCenter;
                    item.Model = data.BodyType;
                    item.Type = data.VehicleType;
                    item.ObCost = data.SumOfDec;
                    item.Qty = data.SumOfQtyDec;
                    item.Year = DateTime.Now.Year + 1;
                    item.Month = 12;
                    item.CreatedDate = DateTime.Now;
                    item.CreatedBy = CurrentUser.USER_ID;
                    item.IsActive = true;

                    if (item.ErrorMessage == "" | item.ErrorMessage == null)
                    {
                        var dto = Mapper.Map<CostObDto>(item);

                        _costObBLL.Save(dto);
                    }

                    AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                }
                catch (Exception exception)
                {
                    AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                    return View(Model);
                }
            }
            return RedirectToAction("Index", "MstCostOb");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var qtyPacked = string.Empty;
            var qty = string.Empty;

            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<CostOBUpload>();
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
                    var item = new CostOBUpload();
                    try
                    {
                        item.CostCenter = dataRow[5].ToString();
                        item.BodyType = dataRow[13].ToString();
                        item.VehicleType = dataRow[14].ToString();
                        item.SumOfJan = decimal.Parse(dataRow[28 + (13 - DateTime.Now.Month)].ToString());
                        item.SumOfFeb = decimal.Parse(dataRow[29 + (13 - DateTime.Now.Month)].ToString());
                        item.SumOfMar = decimal.Parse(dataRow[30 + (13 - DateTime.Now.Month)].ToString());
                        item.SumOfApr = decimal.Parse(dataRow[31 + (13 - DateTime.Now.Month)].ToString());
                        item.SumOfMay = decimal.Parse(dataRow[32 + (13 - DateTime.Now.Month)].ToString());
                        item.SumOfJun = decimal.Parse(dataRow[33 + (13 - DateTime.Now.Month)].ToString());
                        item.SumOfJul = decimal.Parse(dataRow[34 + (13 - DateTime.Now.Month)].ToString());
                        item.SumOfAug = decimal.Parse(dataRow[35 + (13 - DateTime.Now.Month)].ToString());
                        item.SumOfSep = decimal.Parse(dataRow[36 + (13 - DateTime.Now.Month)].ToString());
                        item.SumOfOct = decimal.Parse(dataRow[37 + (13 - DateTime.Now.Month)].ToString());
                        item.SumOfNov = decimal.Parse(dataRow[38 + (13 - DateTime.Now.Month)].ToString());
                        item.SumOfDec = decimal.Parse(dataRow[39 + (13 - DateTime.Now.Month)].ToString());
                        item.SumOfQtyJan = Convert.ToInt32(dataRow[(80 - DateTime.Now.Month) + (13 - DateTime.Now.Month)].ToString());
                        item.SumOfQtyFeb = Convert.ToInt32(dataRow[(81 - DateTime.Now.Month) + (13 - DateTime.Now.Month)].ToString());
                        item.SumOfQtyMar = Convert.ToInt32(dataRow[(82 - DateTime.Now.Month) + (13 - DateTime.Now.Month)].ToString());
                        item.SumOfQtyApr = Convert.ToInt32(dataRow[(83 - DateTime.Now.Month) + (13 - DateTime.Now.Month)].ToString());
                        item.SumOfQtyMay = Convert.ToInt32(dataRow[(84 - DateTime.Now.Month) + (13 - DateTime.Now.Month)].ToString());
                        item.SumOfQtyJun = Convert.ToInt32(dataRow[(85 - DateTime.Now.Month) + (13 - DateTime.Now.Month)].ToString());
                        item.SumOfQtyJul = Convert.ToInt32(dataRow[(86 - DateTime.Now.Month) + (13 - DateTime.Now.Month)].ToString());
                        item.SumOfQtyAug = Convert.ToInt32(dataRow[(87 - DateTime.Now.Month) + (13 - DateTime.Now.Month)].ToString());
                        item.SumOfQtySep = Convert.ToInt32(dataRow[(88 - DateTime.Now.Month) + (13 - DateTime.Now.Month)].ToString());
                        item.SumOfQtyOct = Convert.ToInt32(dataRow[(89 - DateTime.Now.Month) + (13 - DateTime.Now.Month)].ToString());
                        item.SumOfQtyNov = Convert.ToInt32(dataRow[(90 - DateTime.Now.Month) + (13 - DateTime.Now.Month)].ToString());
                        item.SumOfQtyDec = Convert.ToInt32(dataRow[(91 - DateTime.Now.Month) + (13 - DateTime.Now.Month)].ToString());

                        model.Add(item);
                    }
                    catch (Exception ex)
                    {
                        var a = ex.Message;
                    }
                }
            }
            return Json(model);
        }

        #region export xls
        public void ExportMasterCostOb()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterCostOb();

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

        private string CreateXlsMasterCostOb()
        {
            //get data
            List<CostObDto> costOb = _costObBLL.GetCostOb();
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
            slDocument.SetCellValue(iRow, 2, "Zone");
            slDocument.SetCellValue(iRow, 3, "Model");
            slDocument.SetCellValue(iRow, 4, "Type");
            slDocument.SetCellValue(iRow, 5, "Cost OB");
            slDocument.SetCellValue(iRow, 6, "Remark");
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

        private SLDocument CreateDataExcelMasterCostOb(SLDocument slDocument, List<CostObItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, (int)data.Year);
                slDocument.SetCellValue(iRow, 2, data.Zone);
                slDocument.SetCellValue(iRow, 3, data.Model);
                slDocument.SetCellValue(iRow, 4, data.Type);
                slDocument.SetCellValue(iRow, 5, (decimal)data.ObCost);
                slDocument.SetCellValue(iRow, 6, data.Remark);
                slDocument.SetCellValue(iRow, 7, data.CreatedDate.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 8, data.CreatedBy);
                slDocument.SetCellValue(iRow, 9, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
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

            slDocument.AutoFitColumn(1, 12);
            slDocument.SetCellStyle(3, 1, iRow - 1, 11, valueStyle);

            return slDocument;
        }

        #endregion
    }


}