using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using FMS.BLL.Reason;
using FMS.BusinessObject.Dto;
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
    public class MstReasonController : BaseController
    {
        private IPageBLL _pageBLL;
        private IReasonBLL _rasonBLL;
        private IDocumentTypeBLL _documentTypeBLL;
        private Enums.MenuList _mainMenu;

        // GET: /MstRemark/
        public MstReasonController(IPageBLL PageBll, IReasonBLL ReasonBLL, IDocumentTypeBLL DocTypeBLL) : base(PageBll, Enums.MenuList.MasterVendor)
        {
            _pageBLL = PageBll;
            _rasonBLL = ReasonBLL;
            _documentTypeBLL = DocTypeBLL;
            _mainMenu = Enums.MenuList.MasterData;
        }

        public ActionResult Index()
        {
            var data = _rasonBLL.GetReason();
            var model = new ReasonModel();
            model.Details = Mapper.Map<List<ReasonItem>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new ReasonItem();
            var list1 = _documentTypeBLL.GetDocumentType();

            var list2 = new List<SelectListItem>()
            {
                new SelectListItem() {Text = "BENEFIT", Value = "BENEFIT" },
                new SelectListItem() {Text = "WTC", Value = "WTC" }
            };

            model.DocumentTypeList = new SelectList(list1, "MstDocumentTypeId", "DocumentType");
            model. VehicleTypeList = new SelectList(list2, "Text", "Value");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ReasonItem model)
        {
            if (ModelState.IsValid)
            {
                var dto = Mapper.Map<ReasonDto>(model);
                dto.CreatedBy = CurrentUser.USER_ID;
                dto.CreatedDate = DateTime.Now;
                dto.IsActive = true;
                try
                {
                    _rasonBLL.save(dto);
                    
                }
                catch (Exception ex)
                {
                    model.ErrorMessage = ex.Message;
                    var list1 = _documentTypeBLL.GetDocumentType();
                    var list2 = new List<SelectListItem>()
                            {
                                new SelectListItem() {Text = "BENEFIT", Value = "BENEFIT" },
                                new SelectListItem() {Text = "WTC", Value = "WTC" }
                            };
                    model.DocumentTypeList = new SelectList(list1, "MstDocumentTypeId", "DocumentType");
                    model.VehicleTypeList = new SelectList(list2, "Text", "Value");
                    model.MainMenu = _mainMenu;
                    model.CurrentLogin = CurrentUser;
                    return View(model);
                }
                _rasonBLL.SaveCanges();
            }

            return RedirectToAction("Index", "MstReason");
        }
    
        public ActionResult Edit(int MstReasonId)
        {
            var data = _rasonBLL.GetReasonById(MstReasonId);
            var model = Mapper.Map<ReasonItem>(data);
            
            var list1 = _documentTypeBLL.GetDocumentType();
            var list2 = new List<SelectListItem>()
            {
                new SelectListItem() {Text = "BENEFIT", Value = "BENEFIT" },
                new SelectListItem() {Text = "WTC", Value = "WTC" }
            };

            model.DocumentTypeList = new SelectList(list1, "MstDocumentTypeId", "DocumentType");
            model.VehicleTypeList = new SelectList(list2, "Text", "Value");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterReason, MstReasonId);
            return View(model);

        }

        [HttpPost]
        public ActionResult Edit(ReasonItem model)
        {
            if (ModelState.IsValid)
            {
                var dto = Mapper.Map<ReasonDto>(model);
                dto.ModifiedBy = CurrentUser.USER_ID;
                dto.ModifiedDate = DateTime.Now;
                try
                {
                    _rasonBLL.save(dto, CurrentUser);
                }
                catch (Exception ex)
                {
                    model.ErrorMessage = ex.Message;
                    var list1 = _documentTypeBLL.GetDocumentType();
                    var list2 = new List<SelectListItem>()
                            {
                                new SelectListItem() {Text = "BENEFIT", Value = "BENEFIT" },
                                new SelectListItem() {Text = "WTC", Value = "WTC" }
                            };

                    model.DocumentTypeList = new SelectList(list1, "MstDocumentTypeId", "DocumentType");
                    model.VehicleTypeList = new SelectList(list2, "Text", "Value");
                    model.MainMenu = _mainMenu;
                    model.CurrentLogin = CurrentUser;
                    return View(model);
                }
                _rasonBLL.SaveCanges();
            }

            return RedirectToAction("Index", "MstReason");
        }

        public ActionResult Detail(int MstReasonId)
        {
            var data = _rasonBLL.GetReasonById(MstReasonId);
            var model = Mapper.Map<ReasonItem>(data);

            var list1 = _documentTypeBLL.GetDocumentType();
            var list2 = new List<SelectListItem>()
            {
                new SelectListItem() {Text = "BENEFIT", Value = "BENEFIT" },
                new SelectListItem() {Text = "WTC", Value = "WTC" }
            };

            model.DocumentTypeList = new SelectList(list1, "MstDocumentTypeId", "DocumentType");
            model.VehicleTypeList = new SelectList(list2, "Text", "Value");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterReason, MstReasonId);
            return View(model);

        }

        public ActionResult Upload()
        {
            var model = new ReasonModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        [HttpPost]
        public ActionResult Upload(ReasonModel Model)
        {
            if (ModelState.IsValid)
            {
                foreach (var data in Model.Details)
                {
                    try
                    {
                        data.CreatedDate = DateTime.Now;
                        data.CreatedBy = CurrentUser.USERNAME;
                        data.ModifiedDate = null;
                        data.IsActive = true;
                        if (data.ErrorMessage == "" | data.ErrorMessage == null)
                        {
                            var dto = Mapper.Map<ReasonDto>(data);

                            _rasonBLL.save(dto);
                        }

                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                    }
                    catch (Exception exception)
                    {
                        AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                        return View(Model);
                    }
                }
                _rasonBLL.SaveCanges();
            }
            return RedirectToAction("Index", "MstReason");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var qtyPacked = string.Empty;
            var qty = string.Empty;

            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<ReasonItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    var item = new ReasonItem();
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
                        item.VehicleType = dataRow[1].ToUpper();
                        item.Reason = dataRow[2].ToString();
                        if (dataRow[3].ToString() == "Yes" | dataRow[3].ToString() == "YES" | dataRow[3].ToString() == "true" | dataRow[3].ToString() == "TRUE" | dataRow[3].ToString() == "1")
                        {
                            item.IsPenalty = true;
                        }
                        else if (dataRow[3].ToString() == "No" | dataRow[3].ToString() == "NO" | dataRow[3].ToString() == "False" | dataRow[3].ToString() == "FALSE" | dataRow[3].ToString() == "0")
                        {
                            item.IsPenalty = false;
                        }
                        if (dataRow[4].ToString() == "Yes" | dataRow[4].ToString() == "YES" | dataRow[4].ToString() == "true" | dataRow[4].ToString() == "TRUE" | dataRow[4].ToString() == "1")
                        {
                            item.PenaltyForFleet= true;
                        }
                        else if (dataRow[4].ToString() == "No" | dataRow[4].ToString() == "NO" | dataRow[4].ToString() == "False" | dataRow[4].ToString() == "FALSE" | dataRow[4].ToString() == "0")
                        {
                            item.PenaltyForFleet = false;
                        }
                        if (dataRow[5].ToString() == "Yes" | dataRow[5].ToString() == "YES" | dataRow[5].ToString() == "true" | dataRow[5].ToString() == "TRUE" | dataRow[5].ToString() == "1")
                        {
                            item.PenaltyForEmplloyee = true;
                        }
                        else if (dataRow[5].ToString() == "No" | dataRow[5].ToString() == "NO" | dataRow[5].ToString() == "False" | dataRow[5].ToString() == "FALSE" | dataRow[5].ToString() == "0")
                        {
                            item.PenaltyForEmplloyee = false;
                        }
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


        #region export xls
        public void ExportMasterReason()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterReason();

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

        private string CreateXlsMasterReason()
        {
            //get data
            List<ReasonDto> Reason = _rasonBLL.GetReason();
            var listData = Mapper.Map<List<ReasonItem>>(Reason);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Reason");
            slDocument.MergeWorksheetCells(1, 1, 1, 8);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterReason(slDocument);

            //create data
            slDocument = CreateDataExcelMasterReason(slDocument, listData);

            var fileName = "Master_Data_Reason" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterReason(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Document Type");
            slDocument.SetCellValue(iRow, 2, "Reason");
            slDocument.SetCellValue(iRow, 3, "Penalty");
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

        private SLDocument CreateDataExcelMasterReason(SLDocument slDocument, List<ReasonItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.MstDocumentType);
                slDocument.SetCellValue(iRow, 2, data.Reason);
                slDocument.SetCellValue(iRow, 3, data.IsPenalty == true ? "Yes" : "No");
                slDocument.SetCellValue(iRow, 4, data.CreatedDate.Value.ToString("dd-MMM-yyyy HH:mm:ss"));
                slDocument.SetCellValue(iRow, 5, data.CreatedBy);
                slDocument.SetCellValue(iRow, 6, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy HH:mm:ss"));
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
