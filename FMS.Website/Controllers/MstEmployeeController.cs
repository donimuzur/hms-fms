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
    public class MstEmployeeController : BaseController
    {
        private IEmployeeBLL _employeeBLL;
        private Enums.MenuList _mainMenu;

        //
        // GET: /MstEmployee/
        public MstEmployeeController(IPageBLL pageBll, IEmployeeBLL employeeBLL) : base(pageBll, Enums.MenuList.MasterEmployee)
        {
            _employeeBLL = employeeBLL;
            _mainMenu = Enums.MenuList.MasterData;
        }

        public ActionResult Index()
        {
            var data = _employeeBLL.GetEmployee();

            var model = new EmployeeModel();
            model.Details = Mapper.Map<List<EmployeeItem>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        public EmployeeItem listdata(EmployeeItem model)
        {
            var list_position_title = _employeeBLL.GetEmployee().Select(x => new { x.POSITION_TITLE }).ToList().Distinct().OrderBy(x => x.POSITION_TITLE);
            model.PositionTitleList = new SelectList(list_position_title, "POSITION_TITLE", "POSITION_TITLE");
            var list_divison_title = _employeeBLL.GetEmployee().Select(x => new { x.DIVISON }).ToList().Distinct().OrderBy(x => x.DIVISON);
            model.DivisonList = new SelectList(list_divison_title, "DIVISON", "DIVISON");
            var list_directorate = _employeeBLL.GetEmployee().Select(x => new { x.DIRECTORATE }).ToList().Distinct().OrderBy(x => x.DIRECTORATE);
            model.DirectorateList = new SelectList(list_directorate, "DIRECTORATE", "DIRECTORATE");
            var list_city = _employeeBLL.GetEmployee().Select(x => new { x.CITY }).ToList().Distinct().OrderBy(x => x.CITY);
            model.CityList = new SelectList(list_city, "CITY", "CITY");
            var list_basetown = _employeeBLL.GetEmployee().Select(x => new { x.BASETOWN }).ToList().Distinct().OrderBy(x => x.BASETOWN);
            model.BaseTownList = new SelectList(list_basetown, "BASETOWN", "BASETOWN");
            var list_company = _employeeBLL.GetEmployee().Select(x => new { x.COMPANY }).ToList().Distinct().OrderBy(x => x.COMPANY);
            model.CompanyList = new SelectList(list_company, "COMPANY", "COMPANY");
            var list_group_level = _employeeBLL.GetEmployee().Select(x => new { x.GROUP_LEVEL }).ToList().Distinct().OrderBy(x => x.GROUP_LEVEL);
            model.GroupLevelList = new SelectList(list_group_level, "GROUP_LEVEL", "GROUP_LEVEL");
            var list_flext_point = _employeeBLL.GetEmployee().Select(x => new { x.FLEX_POINT }).ToList().Distinct().OrderBy(x => x.FLEX_POINT);
            model.FlexPointlList = new SelectList(list_flext_point, "FLEX_POINT", "FLEX_POINT");

            return model;
        }

        public ActionResult Create()
        {
            var model = new EmployeeItem();
            model = listdata(model);
            model.EMPLOYEE_CODE = "X";
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(EmployeeItem model)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<EmployeeDto>(model);
                data.CREATED_DATE = DateTime.Now;
                data.CREATED_BY = CurrentUser.USERNAME;
                data.MODIFIED_DATE = null;
                data.EMPLOYEE_ID = model.EMPLOYEE_CODE + model.EMPLOYEE_ID;
                _employeeBLL.Save(data);
            }
            return RedirectToAction("Index", "MstEmployee");
        }

        public ActionResult Edit(string EmployeeId)
        {
            var data = _employeeBLL.GetByID(EmployeeId);
            var model = new EmployeeItem();
            model = Mapper.Map<EmployeeItem>(data);
            model = listdata(model);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EmployeeItem model)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<EmployeeDto>(model);
                data.MODIFIED_DATE = DateTime.Now;
                data.MODIFIED_BY = CurrentUser.USERNAME;

                _employeeBLL.Save(data);
            }
            return RedirectToAction("Index", "MstEmployee");
        }

        public ActionResult Upload()
        {
            var model = new EmployeeModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        [HttpPost]
        public ActionResult Upload(EmployeeModel Model)
        {
            if (ModelState.IsValid)
            {
                foreach (EmployeeItem data in Model.Details)
            {
                try
                {
                    data.CREATED_DATE = DateTime.Now;
                    data.CREATED_BY = CurrentUser.USERNAME;
                    data.IS_ACTIVE = true;
                    var dto = Mapper.Map<EmployeeDto>(data);
                    _employeeBLL.Save(dto);
                    AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                }
                catch (Exception exception)
                {
                    AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                    return View(Model);
                }
            }

            }
            return RedirectToAction("Index", "MstEmployee");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var qtyPacked = string.Empty;
            var qty = string.Empty;

            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<EmployeeItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    var item = new EmployeeItem();
                    item.EMPLOYEE_ID = dataRow[0];
                    item.FORMAL_NAME = dataRow[1].ToString();
                    item.POSITION_TITLE = dataRow[2].ToString();
                    item.DIVISON = dataRow[3].ToString();
                    item.DIRECTORATE = dataRow[4].ToString();
                    if (dataRow[5] != "")
                    {
                        item.ADDRESS = dataRow[5].ToString();
                    }
                    else
                    {
                        item.ADDRESS = null;
                    }
                    item.CITY = dataRow[6].ToString();
                    item.BASETOWN = dataRow[7].ToString();
                    item.COMPANY = dataRow[8].ToString();
                    item.COST_CENTER = dataRow[9].ToString();
                    if (dataRow[10] != "")
                    {
                        item.GROUP_LEVEL = Convert.ToInt32(dataRow[10].ToString());
                    }
                    if (dataRow[11] != "")
                    {
                        item.FLEX_POINT = Convert.ToInt32(dataRow[11].ToString());
                    }
                    if (dataRow[12] != "")
                    {
                        item.EMAIL_ADDRESS = dataRow[12].ToString();
                    }
                    else
                    {
                        item.EMAIL_ADDRESS = null;
                    }
                    item.CREATED_BY =CurrentUser.USERNAME;
                    item.CREATED_DATE = DateTime.Now;
                    item.MODIFIED_DATE = null;
                    item.IS_ACTIVE = true;
                    model.Add(item);
                }
            }
            return Json(model);
        }

        #region export xls
        public void ExportMasterEmployee()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterEmployee();

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

        private string CreateXlsMasterEmployee()
        {
            //get data
            List<EmployeeDto> employee = _employeeBLL.GetEmployee();
            var listData = Mapper.Map<List<EmployeeItem>>(employee);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Employee");
            slDocument.MergeWorksheetCells(1, 1, 1, 18);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterEmployee(slDocument);

            //create data
            slDocument = CreateDataExcelMasterEmployee(slDocument, listData);

            var fileName = "Master Data Employee " + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;
        }

        private SLDocument CreateHeaderExcelMasterEmployee(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Employee ID");
            slDocument.SetCellValue(iRow, 2, "Formal Name");
            slDocument.SetCellValue(iRow, 3, "Position Title");
            slDocument.SetCellValue(iRow, 4, "Division");
            slDocument.SetCellValue(iRow, 5, "Directorate");
            slDocument.SetCellValue(iRow, 6, "Address");
            slDocument.SetCellValue(iRow, 7, "City");
            slDocument.SetCellValue(iRow, 8, "Basetown");
            slDocument.SetCellValue(iRow, 9, "Company");
            slDocument.SetCellValue(iRow, 10, "Cost Center");
            slDocument.SetCellValue(iRow, 11, "Group Level");
            slDocument.SetCellValue(iRow, 12, "Flex Point");
            slDocument.SetCellValue(iRow, 13, "Email Address");
            slDocument.SetCellValue(iRow, 14, "Created By");
            slDocument.SetCellValue(iRow, 15, "Created");
            slDocument.SetCellValue(iRow, 16, "Modified By");
            slDocument.SetCellValue(iRow, 17, "Modified");
            slDocument.SetCellValue(iRow, 18, "Status");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 18, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelMasterEmployee(SLDocument slDocument, List<EmployeeItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.EMPLOYEE_ID);
                slDocument.SetCellValue(iRow, 2, data.FORMAL_NAME);
                slDocument.SetCellValue(iRow, 3, data.POSITION_TITLE);
                slDocument.SetCellValue(iRow, 4, data.DIVISON);
                slDocument.SetCellValue(iRow, 5, data.DIRECTORATE);
                slDocument.SetCellValue(iRow, 6, data.ADDRESS);
                slDocument.SetCellValue(iRow, 7, data.CITY);
                slDocument.SetCellValue(iRow, 8, data.BASETOWN);
                slDocument.SetCellValue(iRow, 9, data.COMPANY);
                slDocument.SetCellValue(iRow, 10, data.COST_CENTER);
                slDocument.SetCellValue(iRow, 11, data.GROUP_LEVEL.ToString());
                slDocument.SetCellValue(iRow, 12, data.FLEX_POINT.ToString());
                slDocument.SetCellValue(iRow, 13, data.EMAIL_ADDRESS);
                slDocument.SetCellValue(iRow, 14, data.CREATED_BY);
                slDocument.SetCellValue(iRow, 15, data.CREATED_DATE.ToString("dd/MM/yyyy hh:mm"));
                slDocument.SetCellValue(iRow, 16, data.MODIFIED_BY);
                slDocument.SetCellValue(iRow, 17, data.MODIFIED_DATE.Value.ToString("dd/MM/yyyy hh:mm"));
                if (data.IS_ACTIVE)
                {
                    slDocument.SetCellValue(iRow, 18, "Active");
                }
                else
                {
                    slDocument.SetCellValue(iRow, 18, "InActive");
                }

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 18);
            slDocument.SetCellStyle(3, 1, iRow - 1, 18, valueStyle);

            return slDocument;
        }

        #endregion
    }
}
