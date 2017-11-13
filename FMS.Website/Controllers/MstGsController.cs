using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;
using AutoMapper;
using FMS.BusinessObject.Dto;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;
using System.IO;
using FMS.Website.Utility;

namespace FMS.Website.Controllers
{
    public class MstGsController : BaseController
    {
        #region -------------- field and Cunstructor ------------------
        private IGsBLL _gsBLL;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private IRemarkBLL _remarkBLL;
        private IFleetBLL _fleetBLL;
        private IEmployeeBLL _employeeBLL;
        private ILocationMappingBLL _locationMappingBLL;
        public MstGsController(IPageBLL pageBll, IGsBLL gsBLL, IRemarkBLL RemarkBLL, IFleetBLL FleetBLL, IEmployeeBLL EmployeeBLL, ILocationMappingBLL LocationMapping) : base(pageBll, Enums.MenuList.MasterGS)
        {
            _gsBLL = gsBLL;
            _pageBLL = pageBll;
            _remarkBLL = RemarkBLL;
            _fleetBLL = FleetBLL;
            _employeeBLL = EmployeeBLL;
            _locationMappingBLL = LocationMapping;
            _mainMenu = Enums.MenuList.MasterData;
        }
        #endregion
        // GET: /MstGs/
        #region -------- list VIew-------------------
        public ActionResult Index()
        {
            var model = new GsModel();
            var data  = _gsBLL.GetGs();
            model.Details = Mapper.Map<List<GsItem>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }
        #endregion

        #region ------------- create ---------------
        public GsItem InitialModel(GsItem model)
        {
            var policeList = _fleetBLL.GetFleet().Where(x => x.VehicleStatus.ToLower() == "active" || x.IsActive == true).ToList();
            model.PoliceNumberList = new SelectList(policeList, "PoliceNumber", "PoliceNumber");
            var RemarkList = _remarkBLL.GetRemark().Where(x => x.IsActive == true).ToList();
            model.RemarkList = new SelectList(RemarkList, "Remark", "Remark");
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE == true).Select(x => new { EmployeeNme = x.FORMAL_NAME}).ToList();
            model.EmployeeList = new SelectList(EmployeeList, "EmployeeNme", "EmployeeNme");
            var LocationList = _locationMappingBLL.GetLocationMapping().Select(x => new {  location = x.Location }).ToList();
            model.LocationList = new SelectList(LocationList, "location", "location");
            return model;
        }
        public ActionResult Create()
        {
            var model = new GsItem();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model = InitialModel(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(GsItem model)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<GsDto>(model);
                data.CreatedBy = CurrentUser.USER_ID;
                data.CreatedDate = DateTime.Now;
                model = InitialModel(model);
                data.IsActive = true;
                try
                {
                    _gsBLL.Save(data);
                }
                catch (Exception exp)
                {
                    model.MainMenu = _mainMenu;
                    model.CurrentLogin = CurrentUser;
                    model.ErrorMessage = exp.Message;
                    model = InitialModel(model);
                    return View(model);
                }
                
            }
            return RedirectToAction("Index", "MstGs");
        }
        #endregion

        #region ------------- Edit --------------
        public ActionResult Edit(int MstGsId)
        {
            var data = _gsBLL.GetGsById(MstGsId);
            var model = Mapper.Map<GsItem>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model = InitialModel(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(GsItem model)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<GsDto>(model);
                data.ModifiedBy = CurrentUser.USER_ID;
                data.ModifiedDate = DateTime.Now;
                try
                {
                    _gsBLL.Save(data);
                }
                catch (Exception exp)
                {
                    model.MainMenu = _mainMenu;
                    model.CurrentLogin = CurrentUser;
                    model.ErrorMessage = exp.Message;
                    return View(model);
                }

            }
            return RedirectToAction("Index", "MstGs");
        }
        #endregion

        #region --------upload----------
        public ActionResult Upload()
        {
            var model = new GsModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        [HttpPost]
        public ActionResult Upload(GsModel Model)
        {
            if (ModelState.IsValid)
            {
                foreach (GsItem data in Model.Details)
                {
                    try
                    {
                        data.CreatedDate = DateTime.Now;
                        data.CreatedBy = CurrentUser.USERNAME;
                        data.IsActive = true;
                        data.GroupLevel = data.GroupLevels == null ? data.GroupLevel : Convert.ToInt32(data.GroupLevels);
                        data.GsFullfillmentDate = data.GsFullfillmentDates == null ? data.GsFullfillmentDate : Convert.ToDateTime(data.GsFullfillmentDates);
                        data.GsRequestDate = data.GsRequestDates == null ? data.GsRequestDate : Convert.ToDateTime(data.GsRequestDates);
                        data.StartDate = data.StartDates == null ? data.StartDate : Convert.ToDateTime(data.StartDates);
                        data.EndDate = data.EndDates == null ? data.EndDate : Convert.ToDateTime(data.EndDates);
                        var dto = Mapper.Map<GsDto>(data);
                        
                        if (data.ErrorMessage == "" | data.ErrorMessage == null)
                        {
                            _gsBLL.Save(dto); 
                        }

                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                    }
                    catch (Exception exception)
                    {
                        AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                        Model.ErrorMessage = exception.Message;
                        return View(Model);
                    }
                }
            }
            return RedirectToAction("Index", "MstGs");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<GsItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    var item = new GsItem();
                    item.EmployeeName = dataRow[0].ToString();
                    item.VehicleUsage = dataRow[1].ToString();
                    item.PoliceNumber = dataRow[2].ToString();
                    item.GroupLevels = dataRow[3].ToString();
                    item.Location = dataRow[4].ToString();
                    item.GsRequestDates = dataRow[5].ToString();
                    item.GsFullfillmentDates = dataRow[6].ToString();
                    item.GsUnitType = dataRow[7].ToString();
                    item.GsPoliceNumber = dataRow[8].ToString();
                    item.StartDates = dataRow[9].ToString();
                    item.EndDates = dataRow[10].ToString();
                    item.Remark = dataRow[11].ToString();
                    item.ErrorMessage = "";
                    model.Add(item);
                }
            }
            return Json(model);
        }
        #endregion

        #region -------- Json -------------
        public JsonResult GetEmployee(string EmployeeName)
        {
            
            var Employee = _employeeBLL.GetEmployee().Where(x => x.FORMAL_NAME == EmployeeName).FirstOrDefault();
            return Json(Employee);
        }
        public JsonResult FillPoliceNumber(string EmployeeName, int GroupLevel)
        {
            var fleet = _fleetBLL.GetFleet().Where(x => x.EmployeeName == EmployeeName && x.GroupLevel == GroupLevel &&  x.IsActive == true).ToList();
            return Json(fleet);
        }
        public JsonResult ChangePoliceNumber(string EmployeeName, string PoliceNumber, int GroupLevel)
        {
            var fleet = _fleetBLL.GetFleet().Where(x => x.EmployeeName == EmployeeName && x.PoliceNumber == PoliceNumber && x.GroupLevel == GroupLevel && x.IsActive == true ).FirstOrDefault();
            return Json(fleet);
        }
        #endregion

        #region export xls
        public void ExportMasterGs()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterGs();

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

        private string CreateXlsMasterGs()
        {
            //get data
            List<GsDto> Gs = _gsBLL.GetGs();
            var listData = Mapper.Map<List<GsItem>>(Gs);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Gs");
            slDocument.MergeWorksheetCells(1, 1, 1, 17);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterGs(slDocument);

            //create data
            slDocument = CreateDataExcelMasterGs(slDocument, listData);

            var fileName = "Master_Data_Gs" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterGs(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Employee Name");
            slDocument.SetCellValue(iRow, 2, "Vehicle Usage");
            slDocument.SetCellValue(iRow, 3, "Police Number");
            slDocument.SetCellValue(iRow, 4, "Group Level");
            slDocument.SetCellValue(iRow, 5, "Location");
            slDocument.SetCellValue(iRow, 6, "Gs Request Date");
            slDocument.SetCellValue(iRow, 7, "Gs Fullfillment Date");
            slDocument.SetCellValue(iRow, 8, "Gs Unit Type");
            slDocument.SetCellValue(iRow, 9, "Gs Police Number");
            slDocument.SetCellValue(iRow, 10, "Start Date");
            slDocument.SetCellValue(iRow, 11, "End Date");
            slDocument.SetCellValue(iRow, 12, "Remark");
            slDocument.SetCellValue(iRow, 13, "Created Date");
            slDocument.SetCellValue(iRow, 14, "Created By");
            slDocument.SetCellValue(iRow, 15, "Modified Date");
            slDocument.SetCellValue(iRow, 16, "Modified By");
            slDocument.SetCellValue(iRow, 17, "Status");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 17, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelMasterGs(SLDocument slDocument, List<GsItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.EmployeeName);
                slDocument.SetCellValue(iRow, 2, data.VehicleUsage);
                slDocument.SetCellValue(iRow, 3, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 4, data.GroupLevel==null ? "" : data.GroupLevel.ToString());
                slDocument.SetCellValue(iRow, 5, data.Location);
                slDocument.SetCellValue(iRow, 6, data.GsRequestDate == null ? "" : data.GsRequestDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 7, data.GsFullfillmentDate == null ? "": data.GsFullfillmentDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 8, data.GsUnitType);
                slDocument.SetCellValue(iRow, 9, data.GsPoliceNumber);
                slDocument.SetCellValue(iRow, 10, data.StartDate == null ? "" : data.StartDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 11, data.EndDate == null ? "" : data.EndDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 12, data.Remark);
                slDocument.SetCellValue(iRow, 13, data.CreatedDate.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 14, data.CreatedBy);
                slDocument.SetCellValue(iRow, 15, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 16, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 17, data.IsActive == true ? "Active" : "Not Active");
                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 17);
            slDocument.SetCellStyle(3, 1, iRow - 1, 17, valueStyle);

            return slDocument;
        }

        #endregion
    }
}
