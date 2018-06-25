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
        public GSSearchView Initial(GsModel model)
        {
            model.SearchView.VehicleUsageList = new SelectList(model.Details.Select(x => new { x.VehicleUsage }).Distinct().ToList(), "VehicleUsage", "VehicleUsage");
            model.SearchView.PoliceNumberList = new SelectList(model.Details.Select(x => new { x.PoliceNumber }).Distinct().ToList(), "PoliceNumber", "PoliceNumber");
            model.SearchView.EmployeeNameList = new SelectList(model.Details.Select(x => new { x.EmployeeName }).Distinct().ToList(), "EmployeeName", "EmployeeName");
            return model.SearchView;
        }
        public ActionResult Index()
        {
            var model = new GsModel();
            var filter = new GSParamInput();
            var data = _gsBLL.GetGs(filter);
            var TableList = new List<SelectListItem>()
            {
                new SelectListItem() {Text = "Real Data", Value = "1" },
                new SelectListItem() {Text = "Archive Data", Value = "2" }
            };
            model.Details = Mapper.Map<List<GsItem>>(data);
            model.SearchView = Initial(model);
            model.SearchView.TableList = new SelectList(TableList, "Value", "Text");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;

            return View(model);
        }

        [HttpPost]
        public PartialViewResult ListGs(GsModel model)
        {
            model.Details = new List<GsItem>();
            model.Details = GetGs(model.SearchView);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return PartialView("_ListGs", model);
        }
        private List<GsItem> GetGs(GSSearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _gsBLL.GetGs(new GSParamInput());
                return Mapper.Map<List<GsItem>>(data);
            }

            //getbyparams
            var input = Mapper.Map<GSParamInput>(filter);

            var dbData = _gsBLL.GetGs(input);
            return Mapper.Map<List<GsItem>>(dbData);
        }

        #endregion

        #region ------------- create ---------------
        public GsItem InitialModel(GsItem model)
        {
            var policeList = _fleetBLL.GetFleet().Where(x => x.IsActive == true).ToList();
            model.PoliceNumberList = new SelectList(policeList, "PoliceNumber", "PoliceNumber");
            var RemarkList = _remarkBLL.GetRemark().Where(x => x.IsActive == true && x.DocumentType == (int)Enums.DocumentType.GS).ToList();
            model.RemarkList = new SelectList(RemarkList, "Remark", "Remark");
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE == true).Select(x => new { EmployeeName =  x.FORMAL_NAME }).ToList();
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
                try
                {
                    var exist = _gsBLL.GetGs().Where(x => (x.PoliceNumber == null ? "" : x.PoliceNumber.ToUpper()) == (model.PoliceNumber == null ? "" : model.PoliceNumber.ToUpper())
                          && x.EmployeeId == model.EmployeeId && (x.Remark == null ? "" : x.Remark.ToUpper()) == (model.Remark == null ? "" : model.Remark.ToUpper())
                          && (x.EmployeeName == null ? "" : x.EmployeeName.ToUpper()) == (model.EmployeeName == null ? "" : model.EmployeeName.ToUpper()) && x.GsRequestDate == model.GsRequestDate && x.StartDate == model.StartDate
                          && (x.VehicleUsage == null ? "" : x.VehicleUsage.ToUpper()) == (model.VehicleUsage == null ? "" : model.VehicleUsage.ToUpper()) && x.GsFullfillmentDate == model.GsFullfillmentDate && x.EndDate == model.EndDate
                          && (x.Manufacturer == null ? "" : x.Manufacturer.ToUpper()) == (model.Manufacturer == null ? "" : model.Manufacturer.ToUpper()) && (x.GsManufacturer == null ? "" : x.GsManufacturer.ToUpper()) == (model.GsManufacturer == null ? "" : model.GsManufacturer.ToUpper())
                          && (x.Model == null ? "" : x.Model.ToUpper()) == (model.Models == null ? "" : model.Models.ToUpper()) && (x.GsModel == null ? "" : x.GsModel.ToUpper()) == (model.GsModel == null ? "" : model.GsModel.ToUpper())
                          && (x.Series == null ? "" : x.Series.ToUpper()) == (model.Series == null ? "" : model.Series.ToUpper()) && (x.GsSeries == null ? "" : x.GsSeries.ToUpper()) == (model.GsSeries == null ? "" : model.GsSeries.ToUpper())
                          && (x.Transmission == null ? "" : x.Transmission.ToUpper()) == (model.Transmission == null ? "" : model.Transmission.ToUpper()) && (x.GsTransmission == null ? "" : x.GsTransmission.ToUpper()) == (model.GsTransmission == null ? "" : model.GsTransmission.ToUpper())
                          && (x.Location == null ? "" : x.Location.ToUpper()) == (model.Location == null ? "" : model.Location.ToUpper()) && (x.GsPoliceNumber == null ? "" : x.GsPoliceNumber.ToUpper()) == (model.GsPoliceNumber == null ? "" : model.GsPoliceNumber.ToUpper())
                          && x.GroupLevel == model.GroupLevel && x.IsActive).FirstOrDefault();

                    if (exist != null)
                    {
                        model.MainMenu = _mainMenu;
                        model.CurrentLogin = CurrentUser;
                        model.ErrorMessage = "Data Already exist";
                        model = InitialModel(model);
                        return View(model);

                    }

                    var data = Mapper.Map<GsDto>(model);


                    data.CreatedBy = CurrentUser.USER_ID;
                    data.CreatedDate = DateTime.Now;
                    model = InitialModel(model);
                    data.IsActive = true;

                    _gsBLL.Save(data);
                    _gsBLL.SaveChanges();
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
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterGS, MstGsId);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(GsItem model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var exist = _gsBLL.GetGs().Where(x => (x.PoliceNumber == null ? "" : x.PoliceNumber.ToUpper()) == (model.PoliceNumber == null ? "" : model.PoliceNumber.ToUpper())
                         && x.EmployeeId == model.EmployeeId && (x.Remark == null ? "" : x.Remark.ToUpper()) == (model.Remark == null ? "" : model.Remark.ToUpper())
                         && (x.EmployeeName == null ? "" : x.EmployeeName.ToUpper()) == (model.EmployeeName == null ? "" : model.EmployeeName.ToUpper()) && x.GsRequestDate == model.GsRequestDate && x.StartDate == model.StartDate
                         && (x.VehicleUsage == null ? "" : x.VehicleUsage.ToUpper()) == (model.VehicleUsage == null ? "" : model.VehicleUsage.ToUpper()) && x.GsFullfillmentDate == model.GsFullfillmentDate && x.EndDate == model.EndDate
                         && (x.Manufacturer == null ? "" : x.Manufacturer.ToUpper()) == (model.Manufacturer == null ? "" : model.Manufacturer.ToUpper()) && (x.GsManufacturer == null ? "" : x.GsManufacturer.ToUpper()) == (model.GsManufacturer == null ? "" : model.GsManufacturer.ToUpper())
                         && (x.Model == null ? "" : x.Model.ToUpper()) == (model.Models == null ? "" : model.Models.ToUpper()) && (x.GsModel == null ? "" : x.GsModel.ToUpper()) == (model.GsModel == null ? "" : model.GsModel.ToUpper())
                         && (x.Series == null ? "" : x.Series.ToUpper()) == (model.Series == null ? "" : model.Series.ToUpper()) && (x.GsSeries == null ? "" : x.GsSeries.ToUpper()) == (model.GsSeries == null ? "" : model.GsSeries.ToUpper())
                         && (x.Transmission == null ? "" : x.Transmission.ToUpper()) == (model.Transmission == null ? "" : model.Transmission.ToUpper()) && (x.GsTransmission == null ? "" : x.GsTransmission.ToUpper()) == (model.GsTransmission == null ? "" : model.GsTransmission.ToUpper())
                         && (x.Location == null ? "" : x.Location.ToUpper()) == (model.Location == null ? "" : model.Location.ToUpper()) && (x.GsPoliceNumber == null ? "" : x.GsPoliceNumber.ToUpper()) == (model.GsPoliceNumber == null ? "" : model.GsPoliceNumber.ToUpper())
                         && x.GroupLevel == model.GroupLevel && x.IsActive  && x.MstGsId != model.MstGsId).FirstOrDefault();

                    if (exist != null)
                    {
                        model.MainMenu = _mainMenu;
                        model.CurrentLogin = CurrentUser;
                        model.ErrorMessage = "Data Already exist";
                        model = InitialModel(model);
                        return View(model);

                    }
                    var data = Mapper.Map<GsDto>(model);
                    data.ModifiedBy = CurrentUser.USER_ID;
                    data.ModifiedDate = DateTime.Now;
                    _gsBLL.Save(data, CurrentUser);
                    _gsBLL.SaveChanges();
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
                        var fleetData = _fleetBLL.GetFleet().Where(x => (x.PoliceNumber == null ? "" : x.PoliceNumber.ToUpper()) == (data.PoliceNumber == null ?"" : data.PoliceNumber.ToUpper()) 
                                                                    && x.IsActive).FirstOrDefault();

                        if (fleetData != null)
                        {
                            data.EmployeeId = fleetData.EmployeeID;
                            data.EmployeeName = fleetData.EmployeeName;
                            data.GroupLevel = fleetData.CarGroupLevel;
                            data.Location = fleetData.City;
                            data.Manufacturer = fleetData.Manufacturer;
                            data.VehicleUsage = fleetData.VehicleUsage;
                            data.Transmission = fleetData.Transmission;
                            data.Models = fleetData.Models;
                            data.Series = fleetData.Series;
                        }

                        var exist = _gsBLL.GetGs().Where(x => (x.PoliceNumber == null ? "" : x.PoliceNumber.ToUpper()) == (data.PoliceNumber == null ? "" : data.PoliceNumber.ToUpper())
                           && x.EmployeeId == data.EmployeeId && (x.Remark == null ? "" : x.Remark.ToUpper())==(data.Remark == null ? "" : data.Remark.ToUpper()) 
                           && (x.EmployeeName == null ? "" : x.EmployeeName.ToUpper()) == (data.EmployeeName == null ? "" : data.EmployeeName.ToUpper()) && x.GsRequestDate == data.GsRequestDate && x.StartDate == data.StartDate
                           && (x.VehicleUsage == null ? "" : x.VehicleUsage.ToUpper()) == (data.VehicleUsage == null ? "" : data.VehicleUsage.ToUpper()) && x.GsFullfillmentDate == data.GsFullfillmentDate && x.EndDate == data.EndDate
                           && (x.Manufacturer == null ? "" : x.Manufacturer.ToUpper()) == (data.Manufacturer == null ? "" : data.Manufacturer.ToUpper()) && (x.GsManufacturer == null ? "" : x.GsManufacturer.ToUpper()) == (data.GsManufacturer == null ? "" : data.GsManufacturer.ToUpper())
                           && (x.Model == null ? "" : x.Model.ToUpper()) == (data.Models == null ? "" : data.Models.ToUpper()) && (x.GsModel == null ? "" : x.GsModel.ToUpper()) == (data.GsModel == null ? "" : data.GsModel.ToUpper())
                           && (x.Series == null ? "" : x.Series.ToUpper()) == (data.Series == null ? "" : data.Series.ToUpper()) && (x.GsSeries == null ? "" : x.GsSeries.ToUpper()) == (data.GsSeries == null ? "" : data.GsSeries.ToUpper())
                           && (x.Transmission == null ? "" : x.Transmission.ToUpper()) == (data.Transmission == null ? "" : data.Transmission.ToUpper()) && (x.GsTransmission == null ? "" : x.GsTransmission.ToUpper()) == (data.GsTransmission == null ? "" : data.GsTransmission.ToUpper())
                           && (x.Location == null ? "" : x.Location.ToUpper()) == (data.Location == null ? "" : data.Location.ToUpper()) && (x.GsPoliceNumber == null ? "" : x.GsPoliceNumber.ToUpper()) == (data.GsPoliceNumber == null ? "" :data.GsPoliceNumber.ToUpper())
                           && x.GroupLevel == data.GroupLevel && x.IsActive).FirstOrDefault();

                        if (exist != null)
                        {
                            exist.IsActive = false;
                            exist.ModifiedBy = "SYSTEM";
                            exist.ModifiedDate = DateTime.Now;
                            _gsBLL.Save(exist);

                        }

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
                _gsBLL.SaveChanges();
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
                    if (dataRow[0] == "No")
                    {
                        continue;
                    }
                    var item = new GsItem();
                    item.ErrorMessage = "";
                    try
                    {
                        var GsFullfillmentDateD = dataRow[11].ToString() == null ? 0 : double.Parse(dataRow[11].ToString());
                        item.GsFullfillmentDate = DateTime.FromOADate(GsFullfillmentDateD);
                    }
                    catch (Exception)
                    {
                        item.ErrorMessage = "Gs Fullfillment Date is not valid Format";
                    }

                    try
                    {
                        var GsRequestDateD = dataRow[10].ToString() == null ? 0 : double.Parse(dataRow[10].ToString());
                        item.GsRequestDate = DateTime.FromOADate(GsRequestDateD);
                    }
                    catch (Exception)
                    {
                        item.ErrorMessage = "Gs Request Date is not valid Format";

                    }

                    try
                    {
                        var StartDateD = dataRow[17].ToString() == null ? 0 : double.Parse(dataRow[17].ToString());
                        item.StartDate = DateTime.FromOADate(StartDateD);
                    }
                    catch (Exception)
                    {
                        item.ErrorMessage = "Start Date is not valid Format";
                        
                    }

                    try
                    {
                        var EndDateD = dataRow[18].ToString() == null ? 0 : double.Parse(dataRow[18].ToString());
                        item.EndDate = DateTime.FromOADate(EndDateD);
                    }
                    catch (Exception)
                    {
                        item.ErrorMessage = "End Date is not valid Format";
                    }
                    

                    item.PoliceNumber = dataRow[3].ToString();
                    if (item.PoliceNumber == "")
                    {
                        item.ErrorMessage = "Police Number must be filled";
                    }
                    else
                    {
                        var PoliceNumber = _fleetBLL.GetFleet().Where(x => (x.PoliceNumber == null ? "" : x.PoliceNumber.ToUpper()) == (item.PoliceNumber == null ? "" : item.PoliceNumber.ToUpper())
                                                                    && x.IsActive).FirstOrDefault();
                        if (PoliceNumber == null)
                        {
                            item.ErrorMessage = "there is no active Police Number In Master fleet with this Police Number";
                        }
                    }

                   
                    item.GsManufacturer = dataRow[12].ToString();
                    if (item.GsManufacturer == "")
                    {
                        item.ErrorMessage = "GS Manufaturer can't be Empty";
                    }
                    
                    item.GsModel = dataRow[13].ToString();
                    if (item.GsModel == "")
                    {
                        item.ErrorMessage = "GS Model can't be Empty";
                    }

                    item.GsSeries = dataRow[14].ToString();
                    if (item.GsSeries == "")
                    {
                        item.ErrorMessage = "GS Series can't be Empty";
                    }

                    item.GsTransmission = dataRow[15].ToString();
                    if (item.GsTransmission == "")
                    {
                        item.ErrorMessage = "GS Transmission can't be Empty";
                    }

                    item.GsPoliceNumber = dataRow[16].ToString();
                    if (item.GsPoliceNumber == "")
                    {
                        item.ErrorMessage = "GS Police Number can't be Empty";
                    }

                    item.Remark = dataRow[20].ToString();
                    if (item.Remark == "")
                    {
                        item.ErrorMessage = "Remark Can't be empty";
                    }
                    else
                    {
                        var Remark = _remarkBLL.GetRemark().Where(x => x.IsActive == true && x.DocumentType == (int)Enums.DocumentType.GS).FirstOrDefault();
                        if (Remark == null)
                        {
                            item.ErrorMessage = "Remark is not in the Master Remark";
                        }
                    }

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
        public string ExportGsReportGenerateReport(GsModel model = null)
        {
            string pathFile = "";
            pathFile = CreateXlsReportGs(model.FilterReport);
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
            slDocument.MergeWorksheetCells(1, 1, 1, 18);
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
            slDocument.SetCellValue(iRow, 16, "KPI Fulfillment");
            slDocument.SetCellValue(iRow, 17, "Rent Time");
            slDocument.SetCellValue(iRow, 18, "Remark");


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
        public string ExportMasterGs(GsModel model = null)
        {
            string pathFile = "";
            pathFile = CreateXlsMasterGs(model.SearchView);
            return pathFile;

        }
        private string CreateXlsMasterGs(GSSearchView filter)
        {
            //get data
            var input = Mapper.Map<GSParamInput>(filter);
            var dbData = _gsBLL.GetGs(input);
            var listData = Mapper.Map<List<GsItem>>(dbData);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Gs");
            slDocument.MergeWorksheetCells(1, 1, 1, 18);
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
