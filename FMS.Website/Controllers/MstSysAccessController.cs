using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.BLL.Page;
using FMS.Website.Models;
using AutoMapper;
using FMS.BusinessObject.Dto;
using System.IO;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;

namespace FMS.Website.Controllers
{
    public class MstSysAccessController : BaseController
    {
        private ISysAccessBLL _sysAccessBLL;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        
        public MstSysAccessController(IPageBLL pageBll, ISysAccessBLL sysAccessBLL) : base(pageBll, Enums.MenuList.MasterSysAccess)
        {
            _sysAccessBLL = sysAccessBLL;
            _pageBLL = pageBll;
            _mainMenu = Enums.MenuList.MasterData;
        }
        

        public ActionResult Index()
        {
            var model = new SysAccessModel();
            var data = _sysAccessBLL.GetSysAccess();
            model.Details = Mapper.Map<List<SysAccessItem>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View(model);
        }

        
        public ActionResult Create()
        {
            var model = new SysAccessItem();
            var list1= _pageBLL.GetPages();
            model.ModulList = new SelectList(list1, "MST_MODUL_ID", "MODUL_NAME");
            var list2 = _sysAccessBLL.GetSysAccess().Select(x => new { x.RoleName }).ToList().Distinct().OrderBy(x => x.RoleName);
            model.RoleNameList = new SelectList(list2, "RoleName", "RoleName");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(SysAccessItem model)
        {
            if(ModelState.IsValid)
            {
                var data = Mapper.Map<SysAccessDto>(model);
                data.CreatedBy = CurrentUser.USERNAME;
                data.CreatedDate = DateTime.Now;
                data.IsActive = true;
                try
                {
                    _sysAccessBLL.Save(data);
                }
                catch (Exception)
                {
                    var list1 = _pageBLL.GetPages();
                    model.ModulList = new SelectList(list1, "MST_MODUL_ID", "MODUL_NAME");
                    var list2 = _sysAccessBLL.GetSysAccess().Select(x => new { x.RoleName }).ToList().Distinct().OrderBy(x => x.RoleName);
                    model.RoleNameList = new SelectList(list2, "RoleName", "RoleName");
                    model.MainMenu = _mainMenu;
                    model.CurrentLogin = CurrentUser;
                    return View(model);
                    
                }
            }
            return RedirectToAction("Index", "MstSysAccess");
        }

        [HttpPost]
        public JsonResult onChangeRoleName(string RoleName)
        {
            var model = _sysAccessBLL.GetSysAccess().Where(x => x.RoleName == RoleName).FirstOrDefault();
            return Json(model == null ? "": model.RoleNameAlias);
        }

        public ActionResult Edit(int MstSysAccessId)
        {
            var data = _sysAccessBLL.GetSysAccessById(MstSysAccessId);
            var model = Mapper.Map<SysAccessItem>(data);
            var list1 = _pageBLL.GetPages();
            model.ModulList = new SelectList(list1, "MST_MODUL_ID", "MODUL_NAME");
            var list2 = _sysAccessBLL.GetSysAccess().Select(x => new { x.RoleName }).ToList().Distinct().OrderBy(x => x.RoleName);
            model.RoleNameList = new SelectList(list2, "RoleName", "RoleName");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterSysAccess, MstSysAccessId);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(SysAccessItem model)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<SysAccessDto>(model);
                data.ModifiedBy = CurrentUser.USERNAME;
                data.ModifiedDate = DateTime.Now;
                try
                {
                    _sysAccessBLL.Save(data, CurrentUser);
                }
                catch (Exception)
                {
                    var list1 = _pageBLL.GetPages();
                    model.ModulList = new SelectList(list1, "MST_MODUL_ID", "MODUL_NAME");
                    var list2 = _sysAccessBLL.GetSysAccess().Select(x => new { x.RoleName }).ToList().Distinct().OrderBy(x => x.RoleName);
                    model.RoleNameList = new SelectList(list2, "RoleName", "RoleName");
                    model.MainMenu = _mainMenu;
                    model.CurrentLogin = CurrentUser;
                    return View(model);

                }
            }
            return RedirectToAction("Index", "MstSysAccess");
        }

        public ActionResult Detail(int MstSysAccessId)
        {
            var data = _sysAccessBLL.GetSysAccessById(MstSysAccessId);
            var model = Mapper.Map<SysAccessItem>(data);
            var list1 = _pageBLL.GetPages();
            model.ModulList = new SelectList(list1, "MST_MODUL_ID", "MODUL_NAME");
            var list2 = _sysAccessBLL.GetSysAccess().Select(x => new { x.RoleName }).ToList().Distinct().OrderBy(x => x.RoleName);
            model.RoleNameList = new SelectList(list2, "RoleName", "RoleName");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterSysAccess, MstSysAccessId);
            return View(model);
        }


        #region export xls
        public string ExportMasterSysAccess()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterSysAccess();
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
        private string CreateXlsMasterSysAccess()
        {
            //get data
            List<SysAccessDto> SysAccess = _sysAccessBLL.GetSysAccess();
            var listData = Mapper.Map<List<SysAccessItem>>(SysAccess);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master SysAccess");
            slDocument.MergeWorksheetCells(1, 1, 1, 12);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterSysAccess(slDocument);

            //create data
            slDocument = CreateDataExcelMasterSysAccess(slDocument, listData);

            var fileName = "Master_Data_SysAccess" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterSysAccess(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Role Name");
            slDocument.SetCellValue(iRow, 2, "Role Name Alias");
            slDocument.SetCellValue(iRow, 3, "Modul Id");
            slDocument.SetCellValue(iRow, 4, "Modul Name");
            slDocument.SetCellValue(iRow, 5, "Read Access");
            slDocument.SetCellValue(iRow, 6, "Write Access");
            slDocument.SetCellValue(iRow, 7, "Upload Access");
            slDocument.SetCellValue(iRow, 8, "Created Date");
            slDocument.SetCellValue(iRow, 9, "Created By");
            slDocument.SetCellValue(iRow, 10, "Modified Date");
            slDocument.SetCellValue(iRow, 11, "Modified By");
            slDocument.SetCellValue(iRow, 12, "Status");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 12, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelMasterSysAccess(SLDocument slDocument, List<SysAccessItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.RoleName);
                slDocument.SetCellValue(iRow, 2, data.RoleNameAlias);
                slDocument.SetCellValue(iRow, 3, data.ModulId.ToString());
                slDocument.SetCellValue(iRow, 4, data.ModulName);
                slDocument.SetCellValue(iRow, 5, data.ReadAccessData == true ? "Yes" : "No");
                slDocument.SetCellValue(iRow, 6, data.WriteAccessData == true ? "Yes" : "No");
                slDocument.SetCellValue(iRow, 7, data.UploadAccess == true ? "Yes" : "No");
                slDocument.SetCellValue(iRow, 8, data.CreatedDate.ToString("dd-MMM-yyyy HH:mm:ss"));
                slDocument.SetCellValue(iRow, 9, data.CreatedBy);
                slDocument.SetCellValue(iRow, 10, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy HH:mm:ss"));
                slDocument.SetCellValue(iRow, 11, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 12, data.IsActive == true ? "Active" : "InActive");
                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 12);
            slDocument.SetCellStyle(3, 1, iRow - 1, 12, valueStyle);

            return slDocument;
        }

        #endregion
    }
}
