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
using FMS.BusinessObject.Inputs;

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
            var model = new EmployeeModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            model.WriteAccess = CurrentPageAccess.WriteAccess == true ? 1 : 0;
            model.ReadAccess = CurrentPageAccess.ReadAccess == true ? 1 : 0;
            return View(model);
        }

        [HttpPost]
        public ActionResult SearchEmployeeAjax(DTParameters<EmployeeModel> param)
        {
            var model = param;

            var data = model != null ? SearchDataEmployee(model) : SearchDataEmployee();
            DTResult<EmployeeItem> result = new DTResult<EmployeeItem>();
            result.draw = param.Draw;
            result.recordsFiltered = data.Count;
            result.recordsTotal = data.Count;
            //param.TotalData = data.Count;
            //if (param != null && param.Start > 0)
            //{
            IEnumerable<EmployeeItem> dataordered;
            dataordered = data;
            if (param.Order.Length > 0)
            {
                foreach (var ordr in param.Order)
                {
                    if (ordr.Column == 0)
                    {
                        continue;
                    }
                    dataordered = EmployeeDataOrder(EmployeeDataOrderByIndex(ordr.Column), ordr.Dir, dataordered);
                }
            }
            data = dataordered.ToList();
            data = data.Skip(param.Start).Take(param.Length).ToList();

            //}
            result.data = data;

            return Json(result);
        }

        private List<EmployeeItem> SearchDataEmployee(DTParameters<EmployeeModel> searchView = null)
        {
            var param = new EmployeeParamInput();
            param.Status = searchView.StatusSource == "False" ? false : true;
            param.EmployeeId = searchView.EmployeeId;
            param.FormalName = searchView.FormalName;
            param.PositionTitle = searchView.PositionTitle;
            param.Division = searchView.Division;
            param.Directorate = searchView.Directorate;
            param.Address = searchView.Address;
            param.City = searchView.City;
            param.BaseTown = searchView.BaseTown;
            param.Company = searchView.Company;
            param.CostCenter = searchView.CostCenter;
            param.GroupLevel = searchView.GroupLevel;
            param.EmailAddress = searchView.EmailAddress;
            param.FlexPoint = searchView.FlexPoint;

            var data = _employeeBLL.GetEmployeeByParam(param);
            return Mapper.Map<List<EmployeeItem>>(data);
        }

        private IEnumerable<EmployeeItem> EmployeeDataOrder(string column, DTOrderDir dir, IEnumerable<EmployeeItem> data)
        {

            switch (column)
            {
                case "EmployeeId": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.EMPLOYEE_ID).ToList() : data.OrderByDescending(x => x.EMPLOYEE_ID).ToList();
                case "FormalName": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.FORMAL_NAME).ToList() : data.OrderByDescending(x => x.FORMAL_NAME).ToList();
                case "PositionTitle": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.POSITION_TITLE).ToList() : data.OrderByDescending(x => x.POSITION_TITLE).ToList();
                case "Division": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.DIVISON).ToList() : data.OrderByDescending(x => x.DIVISON).ToList();
                case "Directorate": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.DIRECTORATE).ToList() : data.OrderByDescending(x => x.DIRECTORATE).ToList();
                case "Address": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.ADDRESS).ToList() : data.OrderByDescending(x => x.ADDRESS).ToList();
                case "City": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.CITY).ToList() : data.OrderByDescending(x => x.CITY).ToList();
                case "BaseTown": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.BASETOWN).ToList() : data.OrderByDescending(x => x.BASETOWN).ToList();
                case "Company": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.COMPANY).ToList() : data.OrderByDescending(x => x.COMPANY).ToList();
                case "CostCenter": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.COST_CENTER).ToList() : data.OrderByDescending(x => x.COST_CENTER).ToList();
                case "GroupLevel": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.GROUP_LEVEL).ToList() : data.OrderByDescending(x => x.GROUP_LEVEL).ToList();
                case "EmailAddress": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.EMAIL_ADDRESS).ToList() : data.OrderByDescending(x => x.EMAIL_ADDRESS).ToList();
                case "FlexPoint": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.FLEX_POINT).ToList() : data.OrderByDescending(x => x.FLEX_POINT).ToList();

            }
            return null;
        }

        private string EmployeeDataOrderByIndex(int index)
        {
            Dictionary<int, string> columnDict = new Dictionary<int, string>();
            columnDict.Add(1, "EmployeeId");
            columnDict.Add(2, "FormalName");
            columnDict.Add(3, "PositionTitle");
            columnDict.Add(4, "Division");
            columnDict.Add(5, "Directorate");
            columnDict.Add(6, "Address");
            columnDict.Add(7, "City");
            columnDict.Add(8, "BaseTown");
            columnDict.Add(9, "Company");
            columnDict.Add(10, "CostCenter");
            columnDict.Add(11, "GroupLevel");
            columnDict.Add(12, "EmailAddress");
            columnDict.Add(13, "FlexPoint");


            return columnDict[index];
        }

        public EmployeeItem listdata2(EmployeeItem model,string id)
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
            var list_address = _employeeBLL.GetEmployee().Select(x => new { x.ADDRESS,x.CITY}).ToList().Distinct().Where(x => x.CITY == id).OrderBy(x => x.ADDRESS);
            model.AddressList = new SelectList(list_address, "ADDRESS", "ADDRESS");
            return model;
        }

        public EmployeeItem listdata(EmployeeItem model, string id)
        {
            var list_position_title = _employeeBLL.GetEmployee().Select(x => new { x.POSITION_TITLE }).ToList().Distinct().OrderBy(x => x.POSITION_TITLE);
            model.PositionTitleList = new SelectList(list_position_title, "POSITION_TITLE", "POSITION_TITLE");
            var list_divison_title = _employeeBLL.GetEmployee().Select(x => new { x.DIVISON }).ToList().Distinct().OrderBy(x => x.DIVISON);
            model.DivisonList = new SelectList(list_divison_title, "DIVISON", "DIVISON");
            var list_directorate = _employeeBLL.GetEmployee().Select(x => new { x.DIRECTORATE }).ToList().Distinct().OrderBy(x => x.DIRECTORATE);
            model.DirectorateList = new SelectList(list_directorate, "DIRECTORATE", "DIRECTORATE");
            var list_city = _employeeBLL.GetEmployee().Select(x => new { x.CITY, x.BASETOWN }).ToList().Distinct().Where(x => x.BASETOWN == id).OrderBy(x => x.CITY);
            model.CityList = new SelectList(list_city, "CITY", "CITY");
            var list_basetown = _employeeBLL.GetEmployee().Select(x => new { x.BASETOWN }).ToList().Distinct().OrderBy(x => x.BASETOWN);
            model.BaseTownList = new SelectList(list_basetown, "BASETOWN", "BASETOWN");
            var list_company = _employeeBLL.GetEmployee().Select(x => new { x.COMPANY }).ToList().Distinct().OrderBy(x => x.COMPANY);
            model.CompanyList = new SelectList(list_company, "COMPANY", "COMPANY");
            var list_group_level = _employeeBLL.GetEmployee().Select(x => new { x.GROUP_LEVEL }).ToList().Distinct().OrderBy(x => x.GROUP_LEVEL);
            model.GroupLevelList = new SelectList(list_group_level, "GROUP_LEVEL", "GROUP_LEVEL");
            var list_flext_point = _employeeBLL.GetEmployee().Select(x => new { x.FLEX_POINT }).ToList().Distinct().OrderBy(x => x.FLEX_POINT);
            model.FlexPointlList = new SelectList(list_flext_point, "FLEX_POINT", "FLEX_POINT");
            var list_address = _employeeBLL.GetEmployee().Select(x => new { x.ADDRESS, x.BASETOWN }).ToList().Distinct().Where(x => x.BASETOWN == id).OrderBy(x => x.ADDRESS);
            model.AddressList = new SelectList(list_address, "ADDRESS", "ADDRESS");
            return model;
        }

        public ActionResult GetDataJson(string id)
        {
            var model = new EmployeeItem();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model = listdata2(model, id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBaseTownDataJson(string id)
        {
            var model = new EmployeeItem();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model = listdata(model, id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            var model = new EmployeeItem();
            model = listdata(model,null);
            model.EMPLOYEE_CODE = "X";
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            string LastEmployeeId = _employeeBLL.GetLastEmployeeId();
            LastEmployeeId = LastEmployeeId.Trim('X');
            int LastEmployeeIdInt = Convert.ToInt32(LastEmployeeId);
            model.EMPLOYEE_ID = (LastEmployeeIdInt + 1).ToString();
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
            model = listdata(model,model.CITY);
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

        public ActionResult Detail(string EmployeeId)
        {
            var data = _employeeBLL.GetByID(EmployeeId);
            var model = new EmployeeItem();
            model = Mapper.Map<EmployeeItem>(data);
            model = listdata(model, model.CITY);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
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
