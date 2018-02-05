
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
    public class MstComplaintController : BaseController
    {
        private IComplaintCategoryBLL _complaintCategoryBLL;
        private Enums.MenuList _mainMenu;
        private IRoleBLL _roleBLL;
        //
        // GET: /MstComplaint/

        public MstComplaintController(IPageBLL pageBll, IComplaintCategoryBLL complaintCategoryBLL, IRoleBLL RoleBLL) : base(pageBll, Enums.MenuList.MasterComplaintCategory)
        {
            _complaintCategoryBLL = complaintCategoryBLL;
            _roleBLL = RoleBLL;
            _mainMenu = Enums.MenuList.MasterData;
        }

        public ActionResult Index()
        {
            var data = _complaintCategoryBLL.GetComplaints();

            var model = new ComplaintCategoryModel();
            model.Details = Mapper.Map<List<ComplaintCategoryItem>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new ComplaintCategoryItem();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            var RoleTypeList = new Dictionary<string, string> { { "HR", "HR" }, { "FLEET", "FLEET" } };
            model.RoleTypeList = new SelectList(RoleTypeList, "Key", "Value");
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(ComplaintCategoryItem model)
        {
            if (ModelState.IsValid)
            {
                var Exist = _complaintCategoryBLL.GetComplaints().Where(x => (x.CategoryName == null ? "" : x.CategoryName.ToUpper()) == (model.CategoryName == null ? "" : model.CategoryName.ToUpper())
                                                                        && (x.RoleType == null ? "" : x.RoleType.ToUpper()) ==(model.RoleType == null ? "" : model.RoleType.ToUpper())
                                                                        && x.IsActive).FirstOrDefault();
                if (Exist != null)
                {
                    model.ErrorMessage = "Data Already Exist in Master Complaint Category";
                    model.MainMenu = _mainMenu;
                    model.CurrentLogin = CurrentUser;
                    var RoleTypeList = new Dictionary<string, string> { { "HR", "HR" }, { "FLEET", "FLEET" } };
                    model.RoleTypeList = new SelectList(RoleTypeList, "Key", "Value");
                    return View(model);
                }

                var data = Mapper.Map<ComplaintDto>(model);
                data.CreatedBy = CurrentUser.USER_ID;
                data.CreatedDate = DateTime.Now;
                data.ModifiedDate = null;
                try
                {
                    _complaintCategoryBLL.Save(data);
                    _complaintCategoryBLL.SaveChanges();
                }
                catch (Exception exp)
                {
                    model.ErrorMessage = exp.Message;
                    model.MainMenu = _mainMenu;
                    model.CurrentLogin = CurrentUser;
                    var RoleTypeList = new Dictionary<string, string> { { "HR", "HR" }, { "FLEET", "FLEET" } };
                    model.RoleTypeList = new SelectList(RoleTypeList, "Key", "Value");
                    return View(model);
                }
            }
            return RedirectToAction("Index", "MstComplaint");
        }

        public ActionResult Edit(int? MstComplaintId)
        {
            if (!MstComplaintId.HasValue)
            {
                return HttpNotFound();
            }

            var data = _complaintCategoryBLL.GetByID(MstComplaintId.Value);
            var model = new ComplaintCategoryItem();
            model = Mapper.Map<ComplaintCategoryItem>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            var RoleTypeList = new Dictionary<string, string> { { "HR", "HR" }, { "FLEET", "FLEET" } };
            model.RoleTypeList = new SelectList(RoleTypeList, "Key", "Value");
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterComplaintCategory, MstComplaintId.Value);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ComplaintCategoryItem model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var Exist = _complaintCategoryBLL.GetComplaints().Where(x => (x.CategoryName == null ? "" : x.CategoryName.ToUpper()) == (model.CategoryName == null ? "" : model.CategoryName.ToUpper())
                                                                      && (x.RoleType == null ? "" : x.RoleType.ToUpper()) == (model.RoleType == null ? "" : model.RoleType.ToUpper())
                                                                      && x.IsActive && x.MstComplaintCategoryId !=model.MstComplaintCategoryId).FirstOrDefault();
                    if (Exist != null)
                    {
                        model.ErrorMessage = "Data Already Exist in Master Complaint Category";
                        model.MainMenu = _mainMenu;
                        model.CurrentLogin = CurrentUser;
                        var RoleTypeList = new Dictionary<string, string> { { "HR", "HR" }, { "FLEET", "FLEET" } };
                        model.RoleTypeList = new SelectList(RoleTypeList, "Key", "Value");
                        return View(model);
                    }
                    var data = Mapper.Map<ComplaintDto>(model);
                    data.ModifiedDate = DateTime.Now;
                    data.ModifiedBy = CurrentUser.USER_ID;

                    _complaintCategoryBLL.Save(data, CurrentUser);
                    _complaintCategoryBLL.SaveChanges();

                    AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                }
                catch (Exception exception)
                {
                    model.ErrorMessage = exception.Message;
                    model.MainMenu = _mainMenu;
                    model.CurrentLogin = CurrentUser;
                    var RoleTypeList = new Dictionary<string, string> { { "HR", "HR" }, { "FLEET", "FLEET" } };
                    model.RoleTypeList = new SelectList(RoleTypeList, "Key", "Value");
                    return View(model);
                }
            }
            return RedirectToAction("Index", "MstComplaint");
        }

        public ActionResult Detail(int MstComplaintId)
        {
            var data = _complaintCategoryBLL.GetByID(MstComplaintId);
            var model = new ComplaintCategoryItem();
            model = Mapper.Map<ComplaintCategoryItem>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            var RoleTypeList = new Dictionary<string, string> { { "HR", "HR" }, { "FLEET", "FLEET" } };
            model.RoleTypeList = new SelectList(RoleTypeList, "Key", "Value");
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterComplaintCategory, MstComplaintId);
            return View(model);
        }

        public ActionResult Upload()
        {
            var model = new ComplaintCategoryModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }


        [HttpPost]
        public ActionResult Upload(ComplaintCategoryModel Model)
        {
            if (ModelState.IsValid)
            {
                foreach (ComplaintCategoryItem data in Model.Details)
                {
                    try
                    {
                        var exist = _complaintCategoryBLL.GetComplaints().Where(x => (x.RoleType == null ? "" : x.RoleType.ToUpper()) == (data.RoleType == null ? "" : data.RoleType.ToUpper())
                                                                                && (x.CategoryName == null ? "" : x.CategoryName.ToUpper()) == (data.CategoryName == null ? "" : data.CategoryName.ToUpper())
                                                                                && x.IsActive).FirstOrDefault();
                        if (exist != null)
                        {
                            exist.IsActive = false;
                            exist.ModifiedBy = "SYSTEM";
                            exist.ModifiedDate = DateTime.Now;
                            _complaintCategoryBLL.Save(exist, CurrentUser);
                        }

                        data.CreatedDate = DateTime.Now;
                        data.CreatedBy = CurrentUser.USER_ID;
                        data.ModifiedDate = null;
                        data.IsActive = true;

                        var dto = Mapper.Map<ComplaintDto>(data);
                        _complaintCategoryBLL.Save(dto);
                        
                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                    }
                    catch (Exception exception)
                    {
                        Model.ErrorMessage = exception.Message;
                        Model.MainMenu = _mainMenu;
                        Model.CurrentLogin = CurrentUser;
                        return View(Model);
                    }
                }
                try
                {
                    _complaintCategoryBLL.SaveChanges();
                }
                catch (Exception exception)
                {

                    Model.ErrorMessage = exception.Message;
                    Model.MainMenu = _mainMenu;
                    Model.CurrentLogin = CurrentUser;
                    return View(Model);
                }
            }
            return RedirectToAction("Index", "MstComplaint");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<ComplaintCategoryItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    var item = new ComplaintCategoryItem();
                    item.ErrorMessage = "";

                    item.CategoryName = dataRow[0].ToString();
                    if (item.CategoryName == "")
                    {
                        item.ErrorMessage = "Category Name Can't be Empty";
                    }

                    item.RoleType = (dataRow[1] == null ? "" :dataRow[1].ToUpper());
                    if (item.RoleType == "")
                    {
                        item.ErrorMessage = "Role Type Can't be Empty";
                    }
                    else
                    {
                        if (item.RoleType != "HR" && item.RoleType != "FLEET")
                        {
                            item.ErrorMessage = "Role Type is not Valid";
                        }
                    }

                    model.Add(item);
                }
            }
            return Json(model);
        }

        #region export xls
        public string ExportMasterComplaintCategory()
        {
            string pathFile = "";
            pathFile = CreateXlsMasterComplaintCategory();
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

        private string CreateXlsMasterComplaintCategory()
        {
            //get data
            List<ComplaintDto> ComplaintCateggroy = _complaintCategoryBLL.GetComplaints();
            var listData = Mapper.Map<List<ComplaintCategoryItem>>(ComplaintCateggroy);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Complaint Category");
            slDocument.MergeWorksheetCells(1, 1, 1, 7);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterComplaintCategory(slDocument);

            //create data
            slDocument = CreateDataExcelMasterComplaint(slDocument, listData);

            var fileName = "Master Data Complaint Category " + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterComplaintCategory(SLDocument slDocument)
        {
            int iRow = 2;

            //slDocument.SetCellValue(iRow, 1, "Complaint Category ID");
            slDocument.SetCellValue(iRow, 1, "Category Name");
            slDocument.SetCellValue(iRow, 2, "Role Type");
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

        private SLDocument CreateDataExcelMasterComplaint(SLDocument slDocument, List<ComplaintCategoryItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                //slDocument.SetCellValue(iRow, 1, data.MstComplaintCategoryId);
                slDocument.SetCellValue(iRow, 1, data.CategoryName);
                slDocument.SetCellValue(iRow, 2, data.RoleType);
                slDocument.SetCellValue(iRow, 3, data.CreatedDate.ToString("dd-MMM-yyyy HH:mm:ss"));
                slDocument.SetCellValue(iRow, 4, data.CreatedBy);
                slDocument.SetCellValue(iRow, 5, data.ModifiedDate.Value.ToString("dd-MMM-yyyy HH:mm:ss"));
                slDocument.SetCellValue(iRow, 6, data.ModifiedBy);
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

            slDocument.AutoFitColumn(1, 7);
            slDocument.SetCellStyle(3, 1, iRow - 1, 7, valueStyle);

            return slDocument;
        }

        #endregion
    }
}
