using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.BusinessObject.Dto;
using AutoMapper;
using FMS.Website.Models;
using FMS.Website.Utility;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;

namespace FMS.Website.Controllers
{
    public class MstGroupCostCenterController : BaseController
    {
        private IGroupCostCenterBLL _GroupCostCenterBLL;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        public MstGroupCostCenterController(IPageBLL pageBll, IGroupCostCenterBLL GroupCostCenter) : base(pageBll, Enums.MenuList.MasterGroupCostCenter)
        {
            _GroupCostCenterBLL = GroupCostCenter;
            _pageBLL = pageBll;
            _mainMenu = Enums.MenuList.MasterData;
        }

        //
        // GET: /MstGroupCostCenter/

        public ActionResult Index()
        {
            var data= _GroupCostCenterBLL.GetGroupCenter();
            var model = new GroupCostCenterModel();
            model.Details = Mapper.Map <List<GroupCostCenterItem>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new GroupCostCenterItem();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(GroupCostCenterItem model)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<GroupCostCenterDto>(model);
               
                data.CreatedBy = CurrentUser.USERNAME;
                data.CreatedDate = DateTime.Now;
                data.IsActive = true;
                try
                {
                    _GroupCostCenterBLL.Save(data);
                }
                catch (Exception)
                {
                    model.CurrentLogin = CurrentUser;
                    model.MainMenu = _mainMenu;
                    return View(model);
                }
            }
            return RedirectToAction("Index", "MstGroupCostCenter"); 
        }

        public ActionResult Edit(int MstGroupCostCenterId)
        {
            var data = _GroupCostCenterBLL.GetGroupCenterById(MstGroupCostCenterId);
            var model = Mapper.Map<GroupCostCenterItem>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterGroupCostCenter, MstGroupCostCenterId);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(GroupCostCenterItem model)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<GroupCostCenterDto>(model);

                data.ModifiedBy= CurrentUser.USERNAME;
                data.ModifiedDate = DateTime.Now;
                try
                {
                    _GroupCostCenterBLL.Save(data, CurrentUser);
                }
                catch (Exception)
                {
                    model.MainMenu = _mainMenu;
                    model.CurrentLogin = CurrentUser;
                    return View(model);
                }
            }
            return RedirectToAction("Index", "MstGroupCostCenter");
        }

        public ActionResult Detail(int MstGroupCostCenterId)
        {
            var data = _GroupCostCenterBLL.GetGroupCenterById(MstGroupCostCenterId);
            var model = Mapper.Map<GroupCostCenterItem>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterGroupCostCenter, MstGroupCostCenterId);
            return View(model);
        }

        public ActionResult Upload()
        {
            var model = new GroupCostCenterModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        [HttpPost]
        public ActionResult Upload(GroupCostCenterModel Model)
        {
            if (ModelState.IsValid)
            {
                foreach (GroupCostCenterItem data in Model.Details)
                {
                    try
                    {
                        data.CreatedDate = DateTime.Now;
                        data.CreatedBy = CurrentUser.USERNAME;
                        data.ModifiedDate = null;
                        data.IsActive = true;
                        var lg = data.CostCenter.Length;
                        var dto = Mapper.Map<GroupCostCenterDto>(data);
                        if(data.ErrorMessage == "" | data.ErrorMessage== "null" | data.ErrorMessage == null)
                        {
                            _GroupCostCenterBLL.Save(dto);
                        }
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
            return RedirectToAction("Index", "MstGroupCostCenter");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var qtyPacked = string.Empty;
            var qty = string.Empty;

            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<GroupCostCenterItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    var item = new GroupCostCenterItem();
                    item.FunctionName = dataRow[0];
                    item.CostCenter = dataRow[1];
                    item.ErrorMessage = "";
                    if(dataRow[1].Length > 10)
                    {
                        item.ErrorMessage = "Cost Center tidak boleh lebih dari 10 Karakter";
                    }
                    model.Add(item);
                }
            }
            return Json(model);
        }


        #region export xls
        public void ExportMasterGroupCostCenter()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterGroupCostCenter();

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

        private string CreateXlsMasterGroupCostCenter()
        {
            //get data
            List<GroupCostCenterDto> GroupCostCenter = _GroupCostCenterBLL.GetGroupCenter();
            var listData = Mapper.Map<List<GroupCostCenterItem>>(GroupCostCenter);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master GroupCostCenter");
            slDocument.MergeWorksheetCells(1, 1, 1, 7);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterGroupCostCenter(slDocument);

            //create data
            slDocument = CreateDataExcelMasterGroupCostCenter(slDocument, listData);

            var fileName = "Master_Data_GroupCostCenter" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterGroupCostCenter(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Funtion Name");
            slDocument.SetCellValue(iRow, 2, "Cost Center");
            slDocument.SetCellValue(iRow, 3, "Created Date");
            slDocument.SetCellValue(iRow, 4, "Created By");
            slDocument.SetCellValue(iRow, 5, "Modified Date");
            slDocument.SetCellValue(iRow, 6, "Modified By");
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

        private SLDocument CreateDataExcelMasterGroupCostCenter(SLDocument slDocument, List<GroupCostCenterItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.FunctionName);
                slDocument.SetCellValue(iRow, 2, data.CostCenter);
                slDocument.SetCellValue(iRow, 3, data.CreatedDate.ToString("dd-MMM-yyyy HH:mm:ss"));
                slDocument.SetCellValue(iRow, 4, data.CreatedBy);
                slDocument.SetCellValue(iRow, 5, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy HH:mm:ss"));
                slDocument.SetCellValue(iRow, 6, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 7, data.IsActive == true ? "Active" : "InActive");
                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 7);
            slDocument.SetCellStyle(3, 1, iRow - 1, 7, valueStyle);

            return slDocument;
        }

        #endregion

    }
}
