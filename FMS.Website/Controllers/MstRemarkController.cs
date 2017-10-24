using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;
using FMS.Website.Utility;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Controllers
{
    public class MstRemarkController : BaseController
    {
        private IPageBLL _pageBLL;
        private IRemarkBLL _remarkBLL;
        private IDocumentTypeBLL _documentTypeBLL;
        private Enums.MenuList _mainMenu;
           
        // GET: /MstRemark/
        public MstRemarkController(IPageBLL PageBll, IRemarkBLL RemarkBll, IDocumentTypeBLL DocTypeBLL ) : base(PageBll, Enums.MenuList.MasterVendor)
        {
            _pageBLL = PageBll;
            _remarkBLL = RemarkBll;
            _documentTypeBLL = DocTypeBLL;
            _mainMenu = Enums.MenuList.MasterData;
        }

        public ActionResult Index()
        {
            var data = _remarkBLL.GetRemark();
            var model = new RemarkModel();
            model.Details= Mapper.Map<List<RemarkItem>>(data);
            model.MainMenu = _mainMenu;
            return View(model);
        }
        

        public ActionResult Create()
        {

            var model = new RemarkItem();
            var list1 = _documentTypeBLL.GetDocumentType();
            
            model.DocumentTypeList = new SelectList(list1, "MstDocumentTypeId", "DocumentType");

            var list2 = new List<SelectListItem>
            {
                new SelectListItem { Text = "HR", Value = "HR"},
                new SelectListItem { Text = "Fleet", Value = "Fleet" },
                new SelectListItem { Text = "Admin", Value = "Admin" },
            };
            model.RoleTypeList = new SelectList(list2, "Value", "Text");
            model.MainMenu = _mainMenu;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(RemarkItem model)
        {
            if (ModelState.IsValid)
            {
                var dto = Mapper.Map<RemarkDto>(model);
                dto.CreatedBy = "Doni";
                dto.createdDate = DateTime.Now;
                dto.IsActive = true;
                try
                {
                    _remarkBLL.Save(dto);
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
            }
            
            return RedirectToAction("Index", "MstRemark");
        }
        public ActionResult Edit(int MstRemarkId)
        {
            var data = _remarkBLL.GetRemarkById(MstRemarkId);
            var model = Mapper.Map<RemarkItem>(data);

            
            var list1 = _documentTypeBLL.GetDocumentType();
            
            model.DocumentTypeList = new SelectList(list1, "MstDocumentTypeId", "DocumentType");

            var list2 = new List<SelectListItem>
            {
                new SelectListItem { Text = "HR", Value = "HR"},
                new SelectListItem { Text = "Fleet", Value = "Fleet" },
                new SelectListItem { Text = "Admin", Value = "Admin" },
            };
            model.RoleTypeList = new SelectList(list2, "Value", "Text");
            model.MainMenu = _mainMenu;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(RemarkItem model)
        {
            if (ModelState.IsValid)
            {
                var dto = Mapper.Map<RemarkDto>(model);
                dto.ModifiedBy = "User";
                dto.ModifiedDate = DateTime.Now;
               
                try
                {
                    _remarkBLL.Save(dto);
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
            }

            return RedirectToAction("Index", "MstRemark");
        }

        public ActionResult Upload()
        {
            var model = new RemarkModel();
            model.MainMenu = _mainMenu;
            return View(model);
        }

        [HttpPost]
        public ActionResult Upload(RemarkModel Model)
        {
            if (ModelState.IsValid)
            {
                foreach (var data in Model.Details)
                {
                    try
                    {
                        data.CreatedDate = DateTime.Now;
                        data.CreatedBy = "doni";
                        data.ModifiedDate = null;
                        data.IsActive = true;
                        if (data.ErrorMessage == "" | data.ErrorMessage == null)
                        {
                            var dto = Mapper.Map<RemarkDto>(data);
                        
                         _remarkBLL.Save(dto);
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
            return RedirectToAction("Index", "MstRemark");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var qtyPacked = string.Empty;
            var qty = string.Empty;

            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<RemarkItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    var item = new RemarkItem();
                    try
                    {
                        var getdto = _documentTypeBLL.GetDocumentType().Where(x => x.DocumentType == dataRow[0]).FirstOrDefault();
                        item.MstDocumentType = dataRow[0];
                        if (getdto == null)
                        {
                            item.ErrorMessage = "Document " + dataRow[0] + " tidak ada di database";
                        }
                        else if (getdto != null)
                        {
                            item.DocumentType = getdto.MstDocumentTypeId;
                            item.ErrorMessage = "";
                        }

                        item.Remark= dataRow[1].ToString();
                        item.RoleType = dataRow[2].ToString();

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
        public void ExportMasterRemark()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterRemark();

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

        private string CreateXlsMasterRemark()
        {
            //get data
            List<RemarkDto> remark= _remarkBLL.GetRemark();
            var listData = Mapper.Map<List<RemarkItem>>(remark);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Remark");
            slDocument.MergeWorksheetCells(1, 1, 1, 8);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterRemark(slDocument);

            //create data
            slDocument = CreateDataExcelMasterRemark(slDocument, listData);

            var fileName = "Master_Data_Remark" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterRemark(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Document Type");
            slDocument.SetCellValue(iRow, 2, "Remark");
            slDocument.SetCellValue(iRow, 3, "RoleType");
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

        private SLDocument CreateDataExcelMasterRemark(SLDocument slDocument, List<RemarkItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.MstDocumentType);
                slDocument.SetCellValue(iRow, 2, data.Remark);
                slDocument.SetCellValue(iRow, 3, data.RoleType);
                slDocument.SetCellValue(iRow, 4, data.CreatedDate.ToString("dd - MM - yyyy hh: mm"));
                slDocument.SetCellValue(iRow, 5, data.CreatedBy);
                slDocument.SetCellValue(iRow, 6, data.ModifiedDate == null ?  "" : data.ModifiedDate.Value.ToString("dd - MM - yyyy hh: mm"));
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
