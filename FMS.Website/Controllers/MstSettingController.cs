
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
    public class MstSettingController : BaseController
    {
        private ISettingBLL _settingBLL;
        private IPageBLL _pageBLL;
        private Enums.MenuList _mainMenu;

        public MstSettingController(IPageBLL PageBll, ISettingBLL SettingBLL)
            : base(PageBll, Enums.MenuList.MasterSetting)
        {
            _settingBLL = SettingBLL;
            _pageBLL = PageBll;
            _mainMenu = Enums.MenuList.MasterData;
  
        }
        public ActionResult Index()
        {
            var data = _settingBLL.GetSetting();
            var model = new SettingModel();
            model.Details = Mapper.Map<List<SettingItem>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

       
        public ActionResult Create()
        {
            var model = new SettingItem();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        
        [HttpPost]
        public ActionResult Create(SettingItem item)
        {
            string year = Request.Params["Year"];
            if (ModelState.IsValid)
            {
                var dataexist = _settingBLL.GetExist(item.SettingGroup);
                if (dataexist != null)
                {
                    AddMessageInfo("Data Already Exist", Enums.MessageInfoType.Warning);
                    return View(item);
                }
                else
                {
                    var data = Mapper.Map<SettingDto>(item);
                    data.CreatedBy = CurrentUser.USERNAME; ;
                    data.CreatedDate = DateTime.Today;
                    data.ModifiedDate = null;
                    try
                    {

                        _settingBLL.Save(data);

                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);

                    }
                    catch (Exception exception)
                    {
                        AddMessageInfo(exception.Message, Enums.MessageInfoType.Error
                                );
                        return View(item);
                    }

                }
            }
            return RedirectToAction("Index", "MstSetting");
        }

        public ActionResult Edit(int? MstSettingid)
        {
            if (!MstSettingid.HasValue)
            {
                return HttpNotFound();
            }

            var data = _settingBLL.GetByID(MstSettingid.Value);
            var model = new SettingItem();
            model = Mapper.Map<SettingItem>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = GetChangesHistory((int) Enums.MenuList.MasterSetting, MstSettingid.Value);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(SettingItem item)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<SettingDto>(item);

                data.ModifiedDate = DateTime.Now;
                data.ModifiedBy = CurrentUser.USERNAME; ;

                try
                {
                    _settingBLL.Save(data,CurrentUser);
                    AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                }
                catch (Exception exception)
                {
                    AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                    return View(item);
                }
            }
            return RedirectToAction("Index", "MstSetting");
        }

        public ActionResult Upload()
        {
            var model = new SettingModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }


        [HttpPost]
        public ActionResult Upload(SettingModel Model)
        {
            if (ModelState.IsValid)
            {
                foreach (SettingItem data in Model.Details)
                {
                    try
                    {
                        data.CreatedDate = DateTime.Now;
                        data.CreatedBy = CurrentUser.USERNAME; ;
                        data.ModifiedDate = null;
                        data.IsActive = true;

                        var dto = Mapper.Map<SettingDto>(data);
                        _settingBLL.Save(dto);
                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                    }
                    catch (Exception exception)
                    {
                        AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                        return View(Model);
                    }
                }
            }
            return RedirectToAction("Index", "MstSetting");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var qtyPacked = string.Empty;
            var qty = string.Empty;

            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<SettingItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    var item = new SettingItem();
                    try
                    {
                        item.SettingGroup = dataRow[0].ToString();
                        item.SettingName = dataRow[1].ToString();
                        item.SettingValue = dataRow[2].ToString();
                        /*
                        item.CreatedBy = "Hardcode User";
                        item.CreatedDate = DateTime.Now;
                        if (dataRow[5].ToString() == "Yes" | dataRow[5].ToString() == "YES" | dataRow[5].ToString() == "true" | dataRow[5].ToString() == "TRUE" | dataRow[5].ToString() == "1")
                        {
                            item.IsActive = true;
                        }
                        else if (dataRow[5].ToString() == "No" | dataRow[5].ToString() == "NO" | dataRow[5].ToString() == "False" | dataRow[5].ToString() == "FALSE" | dataRow[5].ToString() == "0")
                        {
                            item.IsActive = false;
                        }*/
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
        public void ExportMasterSetting()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterSetting();

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

        private string CreateXlsMasterSetting()
        {
            //get data
            List<SettingDto> setting = _settingBLL.GetSetting();
            var listData = Mapper.Map<List<SettingItem>>(setting);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Setting");
            slDocument.MergeWorksheetCells(1, 1, 1, 8);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterSetting(slDocument);

            //create data
            slDocument = CreateDataExcelMasterSetting(slDocument, listData);

            var fileName = "Master_Data_Setting" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterSetting(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Setting Group");
            slDocument.SetCellValue(iRow, 2, "Setting Name");
            slDocument.SetCellValue(iRow, 3, "Setting Value");
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

        private SLDocument CreateDataExcelMasterSetting(SLDocument slDocument, List<SettingItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.SettingGroup);
                slDocument.SetCellValue(iRow, 2, data.SettingName);
                slDocument.SetCellValue(iRow, 3, data.SettingValue);
                slDocument.SetCellValue(iRow, 4, data.CreatedDate.ToString("dd - MM - yyyy hh: mm") );
                slDocument.SetCellValue(iRow, 5, data.CreatedBy);
                slDocument.SetCellValue(iRow, 6, data.ModifiedDate.Value.ToString("dd - MM - yyyy hh: mm"));
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