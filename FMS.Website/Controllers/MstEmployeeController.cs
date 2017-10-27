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
            model.IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Administrator);
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new EmployeeItem();
            model.MainMenu = _mainMenu;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(EmployeeItem model)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<EmployeeDto>(model);
                data.IS_ACTIVE = true;
                data.CREATED_DATE = DateTime.Now;
                data.CREATED_BY = "User";
                data.MODIFIED_DATE = null;
                _employeeBLL.Save(data);
            }
            return RedirectToAction("Index", "MstEmployee");
        }

        public ActionResult Edit(string EmployeeId)
        {
            var data = _employeeBLL.GetByID(EmployeeId);
            var model = new EmployeeItem();
            model = Mapper.Map<EmployeeItem>(data);
            model.MainMenu = _mainMenu;

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EmployeeItem model)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<EmployeeDto>(model);
                data.IS_ACTIVE = true;
                data.MODIFIED_DATE = DateTime.Now;
                data.MODIFIED_BY = "User";

                _employeeBLL.Save(data);
            }
            return RedirectToAction("Index", "MstEmployee");
        }

        public ActionResult Upload()
        {
            var model = new EmployeeModel();
            model.MainMenu = _mainMenu;
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
                        data.CREATED_BY = "User";
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
            var model = new List<EmployeeUploadItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    var item = new EmployeeUploadItem();
                    item.EMPLOYEE_ID = dataRow[0].ToString();
                    item.FORMAL_NAME = dataRow[1].ToString();
                    item.POSITION_TITLE = dataRow[2].ToString();
                    item.DIVISON = dataRow[3].ToString();
                    item.DIRECTORATE = dataRow[4].ToString();
                    item.ADDRESS = dataRow[5].ToString();
                    item.CITY = dataRow[6].ToString();
                    item.BASETOWN = dataRow[7].ToString();
                    item.COMPANY = dataRow[8].ToString();
                    item.COST_CENTER = dataRow[9].ToString();
                    item.GROUP_LEVEL = Convert.ToInt32(dataRow[10]);
                    item.EMAIL_ADDRESS = dataRow[11].ToString();
                    item.FLEX_POINT = Convert.ToInt32(dataRow[12]);
                    item.CREATED_BY = "User";
                    item.CREATED_DATE = DateTime.Now;
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

            var fileName = "Master_Data_Employee" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
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
            slDocument.SetCellValue(iRow, 12, "Email Address");
            slDocument.SetCellValue(iRow, 13, "Flex Point");
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
                slDocument.SetCellValue(iRow, 11, data.GROUP_LEVEL);
                slDocument.SetCellValue(iRow, 12, data.EMAIL_ADDRESS);
                slDocument.SetCellValue(iRow, 13, data.FLEX_POINT.ToString());
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
