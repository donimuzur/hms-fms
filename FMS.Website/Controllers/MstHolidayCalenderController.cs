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
using FMS.BusinessObject.Inputs;

namespace FMS.Website.Controllers
{
    public class MstHolidayCalenderController : BaseController
    {
        private IHolidayCalenderBLL _HolidayCalenderBLL;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;

        public MstHolidayCalenderController(IPageBLL PageBll, IHolidayCalenderBLL HolidayCalenderBLL) : base(PageBll, Enums.MenuList.MasterHoliday)
        {
            _HolidayCalenderBLL = HolidayCalenderBLL;
            _mainMenu = Enums.MenuList.MasterData;
            _pageBLL = PageBll;
        }

        //
        // GET: /MstHolidayCalender/

        public ActionResult Index()
        {
            var data = _HolidayCalenderBLL.GetHolidayCalender();
            var model = new HolidayCalenderModel();
            model.Details = Mapper.Map<List<HolidayCalenderItem>>(data);
            model.SearchView.DateFrom = DateTime.Today;
            model.SearchView.DateTo = DateTime.Today;
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View(model);
        }

        [HttpPost]
        public PartialViewResult ListHolidayCalender(HolidayCalenderModel model)
        {
            model.Details = new List<HolidayCalenderItem>();
            model.Details = GetHolidayCalender(model.SearchView);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return PartialView("_ListHolidayCalender", model);
        }

        private List<HolidayCalenderItem> GetHolidayCalender(HolidayCalenderSearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _HolidayCalenderBLL.GetHolidayCalender(new HolidayCalenderParamInput());
                return Mapper.Map<List<HolidayCalenderItem>>(data);
            }

            filter.DateFrom = filter.DateFrom.Date;
            filter.DateTo = filter.DateTo.Date;

            //getbyparams
            var input = Mapper.Map<HolidayCalenderParamInput>(filter);

            var dbData = _HolidayCalenderBLL.GetHolidayCalender(input);
            return Mapper.Map<List<HolidayCalenderItem>>(dbData);
        }

        public ActionResult Create()
        {
            var model = new HolidayCalenderItem();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.MstHolidayDate = DateTime.Today;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(HolidayCalenderItem model)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<HolidayCalenderDto>(model);
                data.CreatedBy = CurrentUser.USERNAME;
                data.CreatedDate = DateTime.Now;
                data.IsActive = true;
                try
                {
                    _HolidayCalenderBLL.Save(data);
                }
                catch (Exception)
                {
                    var list1 = _pageBLL.GetPages();
                    model.ModulList = new SelectList(list1, "MST_MODUL_ID", "MODUL_NAME");
                    model.MainMenu = _mainMenu;
                    model.CurrentLogin = CurrentUser;
                    return View(model);

                }
            }
            return RedirectToAction("Index", "MstHolidayCalender");
        }

        public ActionResult Edit(int MstHolidayCalenderId)
        {
            var data = _HolidayCalenderBLL.GetholidayCalenderById(MstHolidayCalenderId);
            var model = Mapper.Map<HolidayCalenderItem>(data);

            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterHoliday, MstHolidayCalenderId);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(HolidayCalenderItem model)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<HolidayCalenderDto>(model);
                data.ModifiedDate = DateTime.Now;
                data.ModifiedBy = CurrentUser.USERNAME;

                _HolidayCalenderBLL.Save(data, CurrentUser);
            }
            return RedirectToAction("Index", "MstHolidayCalender");
        }

        public ActionResult Detail(int MstHolidayCalenderId)
        {
            var data = _HolidayCalenderBLL.GetholidayCalenderById(MstHolidayCalenderId);
            var model = new HolidayCalenderItem();
            model = Mapper.Map<HolidayCalenderItem>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterHoliday, MstHolidayCalenderId);
            return View(model);
        }

        #region ExportXLS
        public string ExportMasterHolidayCalender(HolidayCalenderModel model = null)
        {
            string pathFile = "";
            pathFile = CreateXlsMasterHolidayCalender(model.SearchView);
            return pathFile; 
        }
        public void GetExcelFile(string pathFile)
        {
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
        private string CreateXlsMasterHolidayCalender(HolidayCalenderSearchView filter)
        {
            //get data
            var input = Mapper.Map<HolidayCalenderParamInput>(filter);
            List<HolidayCalenderDto> holidayCalender = _HolidayCalenderBLL.GetHolidayCalender(input);
            var listData = Mapper.Map<List<HolidayCalenderItem>>(holidayCalender);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Holiday Calender");
            slDocument.MergeWorksheetCells(1, 1, 1, 7);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterHolidayCategory(slDocument);

            //create data
            slDocument = CreateDataExcelHolidayCalender(slDocument, listData);

            var fileName = "Master_Data_Holiday_Calender" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";

            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterHolidayCategory(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Holiday Date");
            slDocument.SetCellValue(iRow, 2, "Description");
            slDocument.SetCellValue(iRow, 3, "Created By");
            slDocument.SetCellValue(iRow, 4, "Created Date");
            slDocument.SetCellValue(iRow, 5, "Modified By");
            slDocument.SetCellValue(iRow, 6, "Modified Date");
            slDocument.SetCellValue(iRow, 7, "Status");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 7, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelHolidayCalender(SLDocument slDocument, List<HolidayCalenderItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.MstHolidayDate.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 2, data.Description);
                slDocument.SetCellValue(iRow, 3, data.CreatedBy);
                slDocument.SetCellValue(iRow, 4, data.CreatedDate.ToString("dd-MMM-yyyy HH:mm:ss"));
                slDocument.SetCellValue(iRow, 5, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 6, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy HH:mm:ss"));
                if (data.IsActive)
                {
                    slDocument.SetCellValue(iRow, 7, "Active");
                }
                else
                {
                    slDocument.SetCellValue(iRow, 7, "InActive");
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
            slDocument.SetCellStyle(3, 1, iRow - 1, 7, valueStyle);

            return slDocument;
        }
        #endregion ExportXLS

        #region ImportXLS
        public ActionResult Upload()
        {
            var model = new HolidayCalenderModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }


        [HttpPost]
        public ActionResult Upload(HolidayCalenderModel Model)
        {
            if (ModelState.IsValid)
            {
                foreach (HolidayCalenderItem data in Model.Details)
                {
                    try
                    {
                        data.CreatedDate = DateTime.Now;
                        data.CreatedBy = CurrentUser.USERNAME;

                        if (data.ErrorMessage == "" | data.ErrorMessage == null)
                        {
                            var dto = Mapper.Map<HolidayCalenderDto>(data);
                            _HolidayCalenderBLL.Save(dto);
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
            return RedirectToAction("Index", "MstHolidayCalender");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var qtyPacked = string.Empty;
            var qty = string.Empty;

            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<HolidayCalenderItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    if (dataRow[0] == "Holiday Date")
                    {
                        continue;
                    }
                    var item = new HolidayCalenderItem();
                    try
                    {
                        double MstHolidayDate = double.Parse(dataRow[0].ToString());
                        item.MstHolidayDate = DateTime.FromOADate(MstHolidayDate);
                        item.Description = dataRow[1].ToString();
                        //item.IsActive = dataRow[2].ToString() == "Active"? true : false;
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
        #endregion ImportXLS
    }
}
