using AutoMapper;
using FMS.BusinessObject;
using FMS.Contract.BLL;
using FMS.BusinessObject.Dto;
using FMS.Core;
using FMS.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;
using FMS.Website.Utility;

namespace FMS.Website.Controllers
{
    public class MstHolidayCalenderController : BaseController
    {
        private IHolidayCalenderBLL _HolidayCalenderBLL;
        private Enums.MenuList _mainMenu;

        public MstHolidayCalenderController(IPageBLL PageBll, IHolidayCalenderBLL HolidayCalenderBLL) : base(PageBll, Enums.MenuList.MasterHoliday)
        {
            _HolidayCalenderBLL = HolidayCalenderBLL;
            _mainMenu = Enums.MenuList.MasterData;
        }

        //
        // GET: /MstHolidayCalender/

        public ActionResult Index()
        {
            var data = _HolidayCalenderBLL.GetHolidayCalender();
            var model = new HolidayCalenderModel();
            model.Details = Mapper.Map<List<HolidayCalenderItem>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new HolidayCalenderItem();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
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
                _HolidayCalenderBLL.Save(data);
            }
            return RedirectToAction("Index", "MstHolidayCalender");
        }

        public ActionResult Edit(DateTime MstHolidayCalenderId)
        {
            var data = _HolidayCalenderBLL.GetholidayCalenderById(MstHolidayCalenderId);
            var model = new HolidayCalenderItem();
            model = Mapper.Map<HolidayCalenderItem>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
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

                _HolidayCalenderBLL.Save(data);
            }
            return RedirectToAction("Index", "MstHolidayCalender");
        }

        public ActionResult Detail(DateTime MstHolidayCalenderId)
        {
            var data = _HolidayCalenderBLL.GetholidayCalenderById(MstHolidayCalenderId);
            var model = new HolidayCalenderItem();
            model = Mapper.Map<HolidayCalenderItem>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        #region ExportXLS
        public void ExportMasterHolidayCalender()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterHolidayCalender();

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
        private string CreateXlsMasterHolidayCalender()
        {
            //get data
            List<HolidayCalenderDto> penalty = _HolidayCalenderBLL.GetHolidayCalender();
            var listData = Mapper.Map<List<HolidayCalenderItem>>(penalty);

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

            var fileName = "Master Data Holiday Calender" + DateTime.Now.ToString(" yyyyMMddHHmmss") + ".xlsx";
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
                slDocument.SetCellValue(iRow, 1, data.MstHolidayDate.ToString("dd/MM/yyyy"));
                slDocument.SetCellValue(iRow, 2, data.Description);
                slDocument.SetCellValue(iRow, 3, data.CreatedBy);
                slDocument.SetCellValue(iRow, 4, data.CreatedDate.ToString("dd/MM/yyyy hh:mm"));
                slDocument.SetCellValue(iRow, 5, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 6, data.ModifiedDate.Value.ToString("dd/MM/yyyy hh:mm"));
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
                        data.IsActive = true;
                        var dto = Mapper.Map<HolidayCalenderDto>(data);
                        _HolidayCalenderBLL.Save(dto);
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
            var model = new List<HolidayCalenderUpload>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    var item = new HolidayCalenderUpload();
                    item.MstHolidayDate = dataRow[0].ToString();
                    item.Description = dataRow[1].ToString();
                    item.ErrorMessage = "";
                    model.Add(item);
                }
            }
            return Json(model);
        }
        #endregion ImportXLS
    }
}
