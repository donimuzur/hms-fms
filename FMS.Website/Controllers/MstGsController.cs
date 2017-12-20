using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.BusinessObject.Inputs;
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
        private ISettingBLL _settingBLL;
        public MstGsController(IPageBLL pageBll, IGsBLL gsBLL,ISettingBLL settingBLL, IRemarkBLL RemarkBLL, IFleetBLL FleetBLL, IEmployeeBLL EmployeeBLL, ILocationMappingBLL LocationMapping) : base(pageBll, Enums.MenuList.MasterGS)
        {
            _gsBLL = gsBLL;
            _pageBLL = pageBll;
            _remarkBLL = RemarkBLL;
            _fleetBLL = FleetBLL;
            _employeeBLL = EmployeeBLL;
            _locationMappingBLL = LocationMapping;
            _settingBLL = settingBLL;
            _mainMenu = Enums.MenuList.MasterData;
        }
        #endregion
        // GET: /MstGs/
        #region -------- list VIew-------------------
        public ActionResult Index()
        {
            var model = new GsModel();
            var data = _gsBLL.GetGs();
            model.Details = Mapper.Map<List<GsItem>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View(model);
        }
        #endregion

        #region ------------- create ---------------
        public GsItem InitialModel(GsItem model)
        {
            var policeList = _fleetBLL.GetFleet().Where(x => x.IsActive == true).ToList();
            model.PoliceNumberList = new SelectList(policeList, "PoliceNumber", "PoliceNumber");
            var RemarkList = _remarkBLL.GetRemark().Where(x => x.IsActive == true).ToList();
            model.RemarkList = new SelectList(RemarkList, "Remark", "Remark");
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE == true).Select(x => new { EmployeeName = "[" + x.EMPLOYEE_ID + "] " + x.FORMAL_NAME }).ToList();
            model.EmployeeList = new SelectList(EmployeeList, "EmployeeName", "EmployeeName");
            var LocationList = _locationMappingBLL.GetLocationMapping().Select(x => new { location = x.Location }).ToList();
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

                String EmployeeName = model.EmployeeName;
                int startIndex = (EmployeeName.IndexOf("] ")) + 2;
                int endIndex = EmployeeName.Length;
                data.EmployeeName = EmployeeName.Substring(startIndex, endIndex - startIndex);

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
            model.EmployeeName = "[" + model.EmployeeId + "] " + model.EmployeeName;
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            
            model = InitialModel(model);
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterGS, MstGsId);
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
                    _gsBLL.Save(data, CurrentUser);
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

        #region -------detail-------
        public ActionResult Detail(int MstGsId)
        {
            var data = _gsBLL.GetGsById(MstGsId);
            var model = Mapper.Map<GsItem>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            
            model = InitialModel(model);
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterGS, MstGsId);
            return View(model);
        }
        #endregion

        public ActionResult ReportGs()
        {
            var model = new GsModel();
            var data = _gsBLL.GetGsReport(new RptGsInput());
            model.Details = Mapper.Map<List<GsItem>>(data);
            model.MainMenu = Enums.MenuList.RptGs;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;


            var settingList = _settingBLL.GetSetting().Where(x => x.SettingGroup.StartsWith("VEHICLE_USAGE")).Select(x => new { x.SettingName,  x.SettingValue }).ToList();
            model.VehicleUsageList = new SelectList(settingList, "SettingName", "SettingValue");
            return View(model);
        }

        [HttpPost]
        public ActionResult ReportGs(GsModel model)
        {
            
            //var data = _gsBLL.GetGs();
            //model.Details = Mapper.Map<List<GsItem>>(data);
            model.MainMenu = Enums.MenuList.RptGs;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;

            List<GsDto> data = _gsBLL.GetGsReport(new RptGsInput()
            {
                StartDateBegin = model.FilterReport.StartDateBegin,
                EndDateBegin = model.FilterReport.EndDateBegin,
                StartDateEnd = model.FilterReport.StartDateEnd,
                EndDateEnd = model.FilterReport.EndDateEnd,
                Location = model.FilterReport.Location,
                VehicleUsage = model.FilterReport.VehicleUsage
            });

            model.Details = Mapper.Map<List<GsItem>>(data);
            var settingList = _settingBLL.GetSetting().Where(x => x.SettingGroup.StartsWith("VEHICLE_USAGE")).Select(x => new { x.SettingName, x.SettingValue }).ToList();
            model.VehicleUsageList = new SelectList(settingList, "SettingName", "SettingValue");
            return View(model);
        }

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
                        var fleetData = _fleetBLL.GetFleetByParam(new BusinessObject.Inputs.FleetParamInput()
                        {
                            PoliceNumber = data.PoliceNumber
                        }).FirstOrDefault();

                        if (fleetData != null)
                        {
                            data.EmployeeId = fleetData.EmployeeID;
                            data.EmployeeName = fleetData.EmployeeName;
                            data.GroupLevel = fleetData.CarGroupLevel;
                            data.Location = fleetData.City;
                            data.Manufacturer = fleetData.Manufacturer;
                            data.VehicleUsage = fleetData.VehicleUsage;
                            data.Transmission = fleetData.Transmission;
                            data.Model = fleetData.Models;
                            data.Series = fleetData.Series;
                        }

                        //var span = data.EndDate - data.StartDate;
                        //data.LeadTime = new DateTime(span.Value.Ticks);
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
                    var GsFullfillmentDateD = dataRow[11].ToString() == null ? 0 : double.Parse(dataRow[11].ToString());
                    var GsRequestDateD = dataRow[10].ToString() == null ? 0 : double.Parse(dataRow[10].ToString());
                    var StartDateD = dataRow[17].ToString() == null ? 0 : double.Parse(dataRow[17].ToString());
                    var EndDateD = dataRow[18].ToString() == null ? 0 : double.Parse(dataRow[18].ToString());
                    item.PoliceNumber = dataRow[3].ToString();
                    item.GsRequestDate = DateTime.FromOADate(GsRequestDateD);
                    item.GsFullfillmentDate = DateTime.FromOADate(GsFullfillmentDateD);
                    item.GsManufacturer = dataRow[12].ToString();
                    item.GsModel = dataRow[13].ToString();
                    item.GsSeries = dataRow[14].ToString();
                    item.GsTransmission = dataRow[15].ToString();
                    item.GsPoliceNumber = dataRow[16].ToString();
                    item.StartDate = DateTime.FromOADate(StartDateD);
                    item.EndDate = DateTime.FromOADate(EndDateD);
                    item.Remark = dataRow[20].ToString();
                    item.ErrorMessage = "";
                    model.Add(item);
                }
            }
            return Json(model);
        }
        #endregion

        #region -------- Json -------------
        public JsonResult GetEmployee(string EmployeeID)
        {
            var Employee = _employeeBLL.GetEmployee().Where(x => x.EMPLOYEE_ID == EmployeeID).FirstOrDefault();
            return Json(Employee);
        }
        public JsonResult FillPoliceNumber(string EmployeeID, int GroupLevel)
        {
            var fleet = _fleetBLL.GetFleet().Where(x => x.EmployeeID == EmployeeID && x.GroupLevel == GroupLevel && x.IsActive == true).ToList();
            return Json(fleet);
        }
        public JsonResult ChangePoliceNumber(string EmployeeID, string PoliceNumber, int GroupLevel)
        {
            var fleet = _fleetBLL.GetFleet().Where(x => x.EmployeeID == EmployeeID && x.PoliceNumber == PoliceNumber && x.GroupLevel == GroupLevel && x.IsActive == true).FirstOrDefault();
            return Json(fleet);
        }
        [HttpPost]
        public JsonResult GetEmployeeData(string employeeId)
        {
            var model = _employeeBLL.GetByID(employeeId);
            FleetDto data = new FleetDto();
            data = _fleetBLL.GetVehicleByEmployeeId(model.EMPLOYEE_ID);
            model.EmployeeVehicle = data;
            return Json(model);
        }

        public JsonResult GetEmployeeList()
        {
            var model = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE && x.GROUP_LEVEL > 0).Select(x => new { x.EMPLOYEE_ID, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region export xls

        public void ExportReportGs(GsModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsReportGs(model.FilterReport);

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

        public void ExportMasterGs(GsModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsReportGs(model.FilterReport);

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

        private string CreateXlsReportGs(ReportFilter filter)
        {
            //get data
            List<GsDto> Gs = _gsBLL.GetGsReport(new RptGsInput()
            {
                EndDateBegin = filter.EndDateBegin,
                EndDateEnd = filter.EndDateEnd,
                StartDateBegin = filter.StartDateBegin,
                StartDateEnd = filter.StartDateEnd,
                Location = filter.Location,
                VehicleUsage = filter.VehicleUsage
            });
            var listData = Mapper.Map<List<GsItem>>(Gs);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Gs Report");
            slDocument.MergeWorksheetCells(1, 1, 1, 13);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelReportGs(slDocument);

            //create data
            slDocument = CreateDataExcelReportGs(slDocument, listData);

            var fileName = "Gs_Report" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateDataExcelReportGs(SLDocument slDocument, List<GsItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.EmployeeName);
                slDocument.SetCellValue(iRow, 2, data.VehicleUsage);
                slDocument.SetCellValue(iRow, 3, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 4, data.GroupLevel == null ? "" : data.GroupLevel.ToString());
                slDocument.SetCellValue(iRow, 5, data.Location);
                slDocument.SetCellValue(iRow, 6, data.GsRequestDate == null ? "" : data.GsRequestDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 7, data.GsFullfillmentDate == null ? "" : data.GsFullfillmentDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 8, data.GsManufacturer);
                slDocument.SetCellValue(iRow, 9, data.GsModel);
                slDocument.SetCellValue(iRow, 10, data.GsSeries);
                slDocument.SetCellValue(iRow, 11, data.GsTransmission);
                slDocument.SetCellValue(iRow, 12, data.GsPoliceNumber);
                slDocument.SetCellValue(iRow, 13, data.StartDate == null ? "" : data.StartDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 14, data.EndDate == null ? "" : data.EndDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 15, data.LeadTimeS);
                slDocument.SetCellValue(iRow, 16, data.KpiFulfillment);
                slDocument.SetCellValue(iRow, 17, data.RentTime);
                slDocument.SetCellValue(iRow, 18, data.Remark);
                
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

        private SLDocument CreateHeaderExcelReportGs(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Employee Name");
            slDocument.SetCellValue(iRow, 2, "Vehicle Usage");
            slDocument.SetCellValue(iRow, 3, "Police Number");
            slDocument.SetCellValue(iRow, 4, "Group Level");
            slDocument.SetCellValue(iRow, 5, "Location");
            slDocument.SetCellValue(iRow, 6, "Gs Request Date");
            slDocument.SetCellValue(iRow, 7, "Gs Fullfillment Date");
            slDocument.SetCellValue(iRow, 8, "Gs Manufacturer");
            slDocument.SetCellValue(iRow, 9, "Gs Model");
            slDocument.SetCellValue(iRow, 10, "Gs Series");
            slDocument.SetCellValue(iRow, 11, "Gs Transmission");
            slDocument.SetCellValue(iRow, 12, "Gs Police Number");
            slDocument.SetCellValue(iRow, 13, "Start Date");
            slDocument.SetCellValue(iRow, 14, "End Date");
            slDocument.SetCellValue(iRow, 15, "Lead Time");
            slDocument.SetCellValue(iRow, 12, "KPI Fulfillment");
            slDocument.SetCellValue(iRow, 12, "Rent Time");
            slDocument.SetCellValue(iRow, 16, "Remark");


            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 16, headerStyle);

            return slDocument;
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

            slDocument.SetCellValue(iRow, 1, "Employee Id");
            slDocument.SetCellValue(iRow, 2, "Employee Name");
            slDocument.SetCellValue(iRow, 3, "Vehicle Usage");
            slDocument.SetCellValue(iRow, 4, "Police Number");
            slDocument.SetCellValue(iRow, 5, "Group Level");
            slDocument.SetCellValue(iRow, 6, "Location");
            slDocument.SetCellValue(iRow, 7, "Gs Request Date");
            slDocument.SetCellValue(iRow, 8, "Gs Fullfillment Date");
            slDocument.SetCellValue(iRow, 9, "Gs Unit Type");
            slDocument.SetCellValue(iRow, 10, "Gs Police Number");
            slDocument.SetCellValue(iRow, 11, "Start Date");
            slDocument.SetCellValue(iRow, 12, "End Date");
            slDocument.SetCellValue(iRow, 13, "Remark");
            slDocument.SetCellValue(iRow, 14, "Created Date");
            slDocument.SetCellValue(iRow, 15, "Created By");
            slDocument.SetCellValue(iRow, 16, "Modified Date");
            slDocument.SetCellValue(iRow, 17, "Modified By");
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

        private SLDocument CreateDataExcelMasterGs(SLDocument slDocument, List<GsItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.EmployeeId);
                slDocument.SetCellValue(iRow, 2, data.EmployeeName);
                slDocument.SetCellValue(iRow, 3, data.VehicleUsage);
                slDocument.SetCellValue(iRow, 4, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 5, data.GroupLevel == null ? "" : data.GroupLevel.ToString());
                slDocument.SetCellValue(iRow, 6, data.Location);
                slDocument.SetCellValue(iRow, 7, data.GsRequestDate == null ? "" : data.GsRequestDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 8, data.GsFullfillmentDate == null ? "" : data.GsFullfillmentDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 9, data.GsUnitType);
                slDocument.SetCellValue(iRow, 10, data.GsPoliceNumber);
                slDocument.SetCellValue(iRow, 11, data.StartDate == null ? "" : data.StartDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 12, data.EndDate == null ? "" : data.EndDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 13, data.Remark);
                slDocument.SetCellValue(iRow, 14, data.CreatedDate.ToString("dd-MMM-yyyy HH:mm:ss"));
                slDocument.SetCellValue(iRow, 15, data.CreatedBy);
                slDocument.SetCellValue(iRow, 16, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy HH:mm:ss"));
                slDocument.SetCellValue(iRow, 17, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 18, data.IsActive == true ? "Active" : "InActive");
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
