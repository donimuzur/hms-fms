using FMS.Contract.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.Core;
using FMS.Website.Models;
using FMS.BusinessObject.Inputs;
using AutoMapper;
using FMS.Utils;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;

namespace FMS.Website.Controllers
{
    public class RptVehicleOverallController : BaseController
    {
        private IVehicleOverallReportBLL _vehicleOverallReportBLL;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private ISettingBLL _settingBLL;
        private ILocationMappingBLL _locationMappingBLL;
        private IFleetBLL _fleetBLL;
        private IVendorBLL _vendorBLL;
        private IGroupCostCenterBLL _groupCostCenterBLL;

        public RptVehicleOverallController(IPageBLL pageBll,  IVehicleOverallReportBLL VehicleOverallReportBLL, ISettingBLL SettingBLL, IVendorBLL VendorBLL, ILocationMappingBLL LocationMappingBLL, IFleetBLL FleetBLL, IGroupCostCenterBLL GroupCostCenterBLL ) : base(pageBll, Enums.MenuList.RptVehicle)
        {
            _pageBLL = pageBll;
            _vehicleOverallReportBLL = VehicleOverallReportBLL;
            _settingBLL = SettingBLL;
            _fleetBLL = FleetBLL;
            _locationMappingBLL = LocationMappingBLL;                             
            _vendorBLL = VendorBLL;
            _groupCostCenterBLL= GroupCostCenterBLL;
             _mainMenu = Enums.MenuList.RptExecutiveSummary ;
        }

        public VehicleOverallReportModel Initial(VehicleOverallReportModel model)
        {
            var fleetList = _fleetBLL.GetFleet().ToList();

            var settingData = _settingBLL.GetSetting().Where(x => x.IsActive);
            var listSupMethod = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.SupplyMethod)).Select(x => new { x.SettingValue }).OrderBy(x => x.SettingValue).ToList();
            var listBodType = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.BodyType)).Select(x => new { x.SettingValue }).OrderBy(x => x.SettingValue).ToList();
            var listVehType = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType)).Select(x => new { x.SettingValue }).OrderBy(x => x.SettingValue).ToList();
            var locationMappingData = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive == true);
            var groupCostData = _groupCostCenterBLL.GetGroupCenter().Where(x => x.IsActive == true);
            var data = _vendorBLL.GetVendor().Where(x => x.IsActive == true).OrderBy(x => x.VendorName).ToList();
            var listStatus = new Dictionary<bool, string> { { true, "Active" }, { false, "InActive" } };

            model.SearchView.PoliceNumberList = new SelectList(fleetList.Select(x => new { x.PoliceNumber }).Distinct().OrderBy(x => x.PoliceNumber).ToList(), "PoliceNumber", "PoliceNumber");
            model.SearchView.EmployeeNameList = new SelectList(fleetList.Select(x => new { x.EmployeeName }).Distinct().OrderBy(x => x.EmployeeName).ToList(), "EmployeeName", "EmployeeName");
            model.SearchView.ChasisNumberList = new SelectList(fleetList.Select(x => new { x.ChasisNumber }).Distinct().OrderBy(x => x.ChasisNumber).ToList(), "ChasisNumber", "ChasisNumber");
            model.SearchView.EmployeeIDList = new SelectList(fleetList.Select(x => new { x.EmployeeID }).Distinct().OrderBy(x => x.EmployeeID).ToList(), "EmployeeID", "EmployeeID");
            model.SearchView.EngineNumberList = new SelectList(fleetList.Select(x => new { x.EngineNumber }).Distinct().OrderBy(x => x.EngineNumber ).ToList(), "EngineNumber", "EngineNumber");
            model.SearchView.SupplyMethodList = new SelectList(listSupMethod, "SettingValue", "SettingValue");
            model.SearchView.BodyTypeList = new SelectList(listBodType, "SettingValue", "SettingValue");
            model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
            model.SearchView.VehicleUsageList = new SelectList(fleetList.Select(x => new { x.VehicleUsage }).Distinct().OrderBy(x => x.VehicleUsage).ToList(), "VehicleUsage", "VehicleUsage");
            model.SearchView.VendorList = new SelectList(data, "ShortName", "ShortName");
            model.SearchView.FunctionList = new SelectList(groupCostData.Select(x => new { x.FunctionName }).Distinct().OrderBy(x => x.FunctionName).ToList(), "FunctionName", "FunctionName");
            model.SearchView.RegionalList = new SelectList(locationMappingData.Select(x => new { x.Region }).Distinct().OrderBy(x => x.Region).ToList(), "Region", "Region");
            model.SearchView.CityList = new SelectList(locationMappingData.Select(x => new { x.Basetown }).Distinct().OrderBy(x => x.Basetown).ToList(), "Basetown", "Basetown");
            model.SearchView.StatusList = new SelectList(listStatus, "Key", "Value");
            
            model.SearchView.FromDate  = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            model.SearchView.ToDate = DateTime.Today;
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;

            return model;
        }

        public ActionResult Index()
        {
            var model = new VehicleOverallReportModel();
            model = Initial(model);

            var filter = new VehicleOverallReportGetByParamInput();
            filter.FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            filter.ToDate = DateTime.Today;

            var data = _vehicleOverallReportBLL.GetVehicle(filter);

            var ListData = Mapper.Map<List<VehicleOverallItem>>(data);
            model.ListVehicle = ListData;

            return View(model);
        }
        
        public ActionResult DetailsVehicle(string id,VehicleOverallSearchView filter)
        {
            var model = GetVehicleData(filter).Where(x => (x.ChasisNumber == null ? "" : x.ChasisNumber.ToUpper()) == (id == null ? "" : id.ToUpper())).FirstOrDefault();

            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;

            model.MonthlyInstallmentStr = model.MonthlyInstallment == null ? "" : string.Format("{0:n0}", model.MonthlyInstallment);
            model.VatStr = model.Vat == null ? "" : string.Format("{0:n0}", model.Vat);
            model.TotalMonthlyInstallmentStr = model.TotalMonthlyInstallment == null ? "" : string.Format("{0:n0}", model.TotalMonthlyInstallment);


            var History = _fleetBLL.GetFleet().Where(x => (x.ChasisNumber == null ? "" : x.ChasisNumber.ToUpper()) == (model.ChasisNumber == null ? "" : model.ChasisNumber.ToUpper())
                                                           && (x.PoliceNumber == null ? "" : x.PoliceNumber.ToUpper()) == (model.PoliceNumber == null ? "" : model.PoliceNumber.ToUpper())
                                                           && x.StartContract == model.StartContract && x.EndContract == model.EndContract).GroupBy(x => x.CreatedDate)
                                                           .Select(x => new VehicleHistory {
                                                                Employee = x.First().EmployeeName,
                                                                Date = x.First().CreatedDate
                                                                //Description = x.First().VehicleStatus
                                                           }).OrderBy(x=>x.Date).ToList();

            model.DetailsHistory = new List<VehicleHistory>();
            if(History !=null && History.Count >0)model.DetailsHistory = History;

            return View(model);
        }

        [HttpPost]
        public PartialViewResult ListVehicle(VehicleOverallReportModel model)
        {
            model.ListVehicle = GetVehicleData(model.SearchView);
            return PartialView("_ListVehicleOverall", model);
        }
        private List<VehicleOverallItem> GetVehicleData(VehicleOverallSearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _vehicleOverallReportBLL.GetVehicle(new VehicleOverallReportGetByParamInput());
                return Mapper.Map<List<VehicleOverallItem>>(data);
            }

            //getbyparams
            var input = Mapper.Map<VehicleOverallReportGetByParamInput>(filter);

            var dbData = _vehicleOverallReportBLL.GetVehicle(input);
            return Mapper.Map<List<VehicleOverallItem>>(dbData);
        }

        #region ------------Export Report-----------
        public string ExportVehicleReportGenerateReport(VehicleOverallReportModel model = null)
        {
            string pathFile = "";
            pathFile = CreateXlsVehicleReport(model);
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
        private string CreateXlsVehicleReport(VehicleOverallReportModel model)
        {
            //get data
            var data = GetVehicleData(model.SearchView);
          
            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "VEHICLE REPORT");
            slDocument.MergeWorksheetCells(1, 1, 1,43);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderVehicleReport(slDocument);

            //create data
            slDocument = CreateDataExcelVehicleReport(slDocument, data);

            var fileName = "Vehicle_report" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;
        }
        private SLDocument CreateHeaderVehicleReport(SLDocument slDocument)
        {
            int iRow = 2;
            slDocument.SetCellValue(iRow, 1, "Police Number");
            slDocument.SetCellValue(iRow, 2, "Chasis Number");
            slDocument.SetCellValue(iRow, 3, "Engine Number");
            slDocument.SetCellValue(iRow, 4, "Employee Name");
            slDocument.SetCellValue(iRow, 5, "Employee ID");
            slDocument.SetCellValue(iRow, 6, "Cost Center");
            slDocument.SetCellValue(iRow, 7, "Manufacture");
            slDocument.SetCellValue(iRow, 8, "Model");
            slDocument.SetCellValue(iRow, 9, "Series");
            slDocument.SetCellValue(iRow, 10, "Transmission");
            slDocument.SetCellValue(iRow, 11, "Body Type");
            slDocument.SetCellValue(iRow, 12, "Fuel Type");
            slDocument.SetCellValue(iRow, 13, "Branding");
            slDocument.SetCellValue(iRow, 14, "Color");
            slDocument.SetCellValue(iRow, 15, "Airbag");
            slDocument.SetCellValue(iRow, 16, "Request Year");
            slDocument.SetCellValue(iRow, 17, "Vehicle Type");
            slDocument.SetCellValue(iRow, 18, "Vehicle Usage");
            slDocument.SetCellValue(iRow, 19, "Project");
            slDocument.SetCellValue(iRow, 20, "Project Name");
            slDocument.SetCellValue(iRow, 21, "Start Contract");
            slDocument.SetCellValue(iRow, 22, "End Contract");
            slDocument.SetCellValue(iRow, 23, "Vendor Name");
            slDocument.SetCellValue(iRow, 24, "City");
            slDocument.SetCellValue(iRow, 25, "Supply Method");
            slDocument.SetCellValue(iRow, 26, "Restitution");
            slDocument.SetCellValue(iRow, 27, "Monthly HMS Installment");
            slDocument.SetCellValue(iRow, 28, "VAT");
            slDocument.SetCellValue(iRow, 29, "PO Number");
            slDocument.SetCellValue(iRow, 30, "PO Line");
            slDocument.SetCellValue(iRow, 31, "Car Group Level");
            slDocument.SetCellValue(iRow, 32, "Employee Group Level");
            slDocument.SetCellValue(iRow, 33, "Assigned To");
            slDocument.SetCellValue(iRow, 34, "Address");
            slDocument.SetCellValue(iRow, 35, "Start Date");
            slDocument.SetCellValue(iRow, 36, "End Date");
            slDocument.SetCellValue(iRow, 37, "Vehicle Status");
            slDocument.SetCellValue(iRow, 38, "Certificate of Ownership");
            slDocument.SetCellValue(iRow, 39, "Comments");
            slDocument.SetCellValue(iRow, 40, "Assets");
            slDocument.SetCellValue(iRow, 41, "Total Monthly Charge");
            slDocument.SetCellValue(iRow, 42, "Function");
            slDocument.SetCellValue(iRow, 43, "Regional");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 43, headerStyle);

            return slDocument;

        }
        private SLDocument CreateDataExcelVehicleReport(SLDocument slDocument, List<VehicleOverallItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 2, data.ChasisNumber);
                slDocument.SetCellValue(iRow, 3, data.EngineNumber);
                slDocument.SetCellValue(iRow, 4, data.EmployeeName);
                slDocument.SetCellValue(iRow, 5, data.EmployeeId);
                slDocument.SetCellValue(iRow, 6, data.CostCenter);
                slDocument.SetCellValue(iRow, 7, data.Manufacture);
                slDocument.SetCellValue(iRow, 8, data.Models);
                slDocument.SetCellValue(iRow, 9, data.Series);
                slDocument.SetCellValue(iRow, 10, data.Transmission);
                slDocument.SetCellValue(iRow, 11, data.BodyType);
                slDocument.SetCellValue(iRow, 12, data.FuelType);
                slDocument.SetCellValue(iRow, 13, data.Branding);
                slDocument.SetCellValue(iRow, 14, data.Colour);
                slDocument.SetCellValue(iRow, 15, data.Airbag == true? "Yes" : "No");
                slDocument.SetCellValue(iRow, 16, data.VehicleYear == null ? "" : data.VehicleYear.Value.ToString());
                slDocument.SetCellValue(iRow, 17, data.VehicleType);
                slDocument.SetCellValue(iRow, 18, data.VehicleUsage);
                slDocument.SetCellValue(iRow, 19, data.Project == true ? "Yes" : "No");
                slDocument.SetCellValue(iRow, 20, data.ProjectName);
                slDocument.SetCellValue(iRow, 21, data.StartContract.HasValue? data.StartContract.Value.ToString("dd-MMM-yyyy"):"");
                slDocument.SetCellValue(iRow, 22, data.EndContract.HasValue ? data.EndContract.Value.ToString("dd-MMM-yyyy") : "");
                slDocument.SetCellValue(iRow, 23, data.Vendor);
                slDocument.SetCellValue(iRow, 24, data.City);
                slDocument.SetCellValue(iRow, 25, data.SupplyMethod);
                slDocument.SetCellValue(iRow, 26, data.Restitution);
                slDocument.SetCellValueNumeric(iRow, 27, data.MonthlyInstallment.HasValue ? data.MonthlyInstallment.Value.ToString() : "");
                slDocument.SetCellValueNumeric(iRow, 28, data.Vat.HasValue ? data.Vat.Value.ToString() : "");
                slDocument.SetCellValue(iRow, 29, data.PoNumber);
                slDocument.SetCellValue(iRow, 30, data.PoLine);
                slDocument.SetCellValue(iRow, 31, data.carGrouplevel.HasValue ? data.carGrouplevel.Value.ToString() : "");
                slDocument.SetCellValue(iRow, 32, data.EmployeeGroupLevel.HasValue ? data.EmployeeGroupLevel.Value.ToString() : "");
                slDocument.SetCellValue(iRow, 33, data.AssignedTo);
                slDocument.SetCellValue(iRow, 34, data.Address);
                slDocument.SetCellValue(iRow, 35, data.StartDate.HasValue ? data.StartDate.Value.ToString("dd-MMM-yyyy") : "");
                slDocument.SetCellValue(iRow, 36, data.TerminationDate.HasValue ? data.TerminationDate.Value.ToString("yyyy-MMM-dd") : "");
                slDocument.SetCellValue(iRow, 37, data.VehicleStatus == true ? "Active" : "InActive");
                slDocument.SetCellValue(iRow, 38, data.CertificateOfOwnership);
                slDocument.SetCellValue(iRow, 39, data.Comments);
                slDocument.SetCellValue(iRow, 40, data.AssetsNumber);
                slDocument.SetCellValueNumeric(iRow, 41, data.TotalMonthlyInstallment.HasValue ? data.TotalMonthlyInstallment.Value.ToString() : "");
                slDocument.SetCellValue(iRow, 42, data.Function);
                slDocument.SetCellValue(iRow, 43, data.Regional);

                iRow++;
            }
            slDocument.AutoFitColumn(1, 43);
            return slDocument;
        }
        #endregion

        #region ------ Export Details -------
        public void ExportDetailsVehicle(VehicleOverallItem model)
        {
            string pathFile = "";

            pathFile = CreateXlsDetailsVehicle(model);

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
        private string CreateXlsDetailsVehicle(VehicleOverallItem model)
        {
            //get data
            var filter = new VehicleOverallSearchView();
            var data = GetVehicleData(filter).Where(x => (x.ChasisNumber == null ? "" : x.ChasisNumber.ToUpper()) == (model.ChasisNumber == null ? "" : model.ChasisNumber.ToUpper())).FirstOrDefault();

            data.MainMenu = _mainMenu;
            data.CurrentLogin = CurrentUser;

            var History = _fleetBLL.GetFleet().Where(x => (x.ChasisNumber == null ? "" : x.ChasisNumber.ToUpper()) == (data.ChasisNumber == null ? "" : data.ChasisNumber.ToUpper())
                                                           && (x.PoliceNumber == null ? "" : x.PoliceNumber.ToUpper()) == (data.PoliceNumber == null ? "" : data.PoliceNumber.ToUpper())
                                                           && x.StartContract == data.StartContract && x.EndContract == data.EndContract).GroupBy(x => x.CreatedDate)
                                                           .Select(x => new VehicleHistory
                                                           {
                                                               Employee = x.First().EmployeeName,
                                                               Date = x.First().CreatedDate
                                                               //Description = x.First().VehicleStatus
                                                           }).OrderBy(x => x.Date).ToList();
            
            data.DetailsHistory = new List<VehicleHistory>();
            if (History != null && History.Count > 0) data.DetailsHistory = History;

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "DETAILS VEHICLE REPORT");
            slDocument.MergeWorksheetCells(1, 1, 1, 5);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderDetailsVehicle(slDocument);

            //create data
            slDocument = CreateDataExcelDetailsVehicle(slDocument, data);

            var fileName = "Vehicle_report" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;
        }
        private SLDocument CreateHeaderDetailsVehicle(SLDocument slDocument)
        {
            int iRow = 5;
            slDocument.SetCellValue(iRow, 1, "Date");
            slDocument.SetCellValue(iRow, 2, "Employee");
            slDocument.SetCellValue(iRow, 3, "Description");
            //slDocument.SetCellValue(iRow, 4, "Employee ID");
            //slDocument.SetCellValue(iRow, 5, "Employee Name");
            //slDocument.SetCellValue(iRow, 6, "Cost Center");
            //slDocument.SetCellValue(iRow, 7, "Manufacture");
            //slDocument.SetCellValue(iRow, 8, "Model");
            //slDocument.SetCellValue(iRow, 9, "Series");
            //slDocument.SetCellValue(iRow, 10, "Transmission");
            //slDocument.SetCellValue(iRow, 11, "Body Type");
            //slDocument.SetCellValue(iRow, 12, "Fuel");
            //slDocument.SetCellValue(iRow, 13, "Branding");
            //slDocument.SetCellValue(iRow, 14, "Color");
            //slDocument.SetCellValue(iRow, 15, "Airbag");
            //slDocument.SetCellValue(iRow, 16, "ABS");
            //slDocument.SetCellValue(iRow, 17, "Vehicle Type");
            //slDocument.SetCellValue(iRow, 18, "Start Rent");
            //slDocument.SetCellValue(iRow, 19, "End Rent");
            //slDocument.SetCellValue(iRow, 20, "Vendor");
            //slDocument.SetCellValue(iRow, 21, "Asset Number");
            //slDocument.SetCellValue(iRow, 22, "Current Location");
            //slDocument.SetCellValue(iRow, 23, "Supply Method");
            //slDocument.SetCellValue(iRow, 24, "Termination Date");
            //slDocument.SetCellValue(iRow, 25, "Monthly Installment");
            //slDocument.SetCellValue(iRow, 26, "VAT");
            //slDocument.SetCellValue(iRow, 27, "Total Monthly");
            //slDocument.SetCellValue(iRow, 28, "PO Number");
            //slDocument.SetCellValue(iRow, 29, "PO Line");
            //slDocument.SetCellValue(iRow, 30, "Function");
            //slDocument.SetCellValue(iRow, 31, "Regional");
            //slDocument.SetCellValue(iRow, 32, "Vehicle Status");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 5, headerStyle);

            return slDocument;

        }
        private SLDocument CreateDataExcelDetailsVehicle(SLDocument slDocument, VehicleOverallItem Data)
        {

            slDocument.SetCellValue(2, 1, "Police Number    :");
            slDocument.SetCellValue(2, 2, Data.PoliceNumber);

            slDocument.SetCellValue(3, 1, "Chasis Number    :");
            slDocument.SetCellValue(3, 2, Data.ChasisNumber);

            slDocument.SetCellValue(2, 4, "Start Contract   :");
            slDocument.SetCellValue(2, 5, Data.StartContract.HasValue ? Data.StartContract.Value.ToString("dd-MMM-yyyy") : "");

            slDocument.SetCellValue(3, 4, "End Contract     :");
            slDocument.SetCellValue(3, 5, Data.EndContract.HasValue ? Data.EndContract.Value.ToString("dd-MMM-yyyy"):"");

            int iRow = 6; //starting row data
            foreach (var data in Data.DetailsHistory)
            {
                slDocument.SetCellValue(iRow, 1, data.Date.HasValue ? data.Date.Value.ToString("dd-MMM-yyyy") : "");
                slDocument.SetCellValue(iRow, 2, data.Employee);
                slDocument.SetCellValue(iRow, 3, data.Description);
                //slDocument.SetCellValue(iRow, 4, data.EmployeeId);
                //slDocument.SetCellValue(iRow, 5, data.EmployeeName);
                //slDocument.SetCellValue(iRow, 6, data.CostCenter);
                //slDocument.SetCellValue(iRow, 7, data.Manufacture);
                //slDocument.SetCellValue(iRow, 8, data.Models);
                //slDocument.SetCellValue(iRow, 9, data.Series);
                //slDocument.SetCellValue(iRow, 10, data.Transmission);
                //slDocument.SetCellValue(iRow, 11, data.BodyType);
                //slDocument.SetCellValue(iRow, 12, data.FuelType);
                //slDocument.SetCellValue(iRow, 13, data.Branding);
                //slDocument.SetCellValue(iRow, 14, data.Colour);
                //slDocument.SetCellValue(iRow, 15, data.Airbag);
                //slDocument.SetCellValue(iRow, 16, data.Abs);
                //slDocument.SetCellValue(iRow, 17, data.VehicleType);
                //slDocument.SetCellValue(iRow, 18, data.StartContract.HasValue ? data.StartContract.Value.ToString("dd-MMM-yyyy") : "");
                //slDocument.SetCellValue(iRow, 19, data.EndContract.HasValue ? data.EndContract.Value.ToString("dd-MMM-yyyy") : "");
                //slDocument.SetCellValue(iRow, 20, data.Vendor);
                //slDocument.SetCellValue(iRow, 21, data.AssetsNumber);
                //slDocument.SetCellValue(iRow, 22, data.City);
                //slDocument.SetCellValue(iRow, 23, data.SupplyMethod);
                //slDocument.SetCellValue(iRow, 24, data.TerminationDate.HasValue ? data.TerminationDate.Value.ToString("yyyy-MMM-dd") : "");
                //slDocument.SetCellValueNumeric(iRow, 25, data.MonthlyInstallment.HasValue ? data.MonthlyInstallment.Value.ToString() : "");
                //slDocument.SetCellValueNumeric(iRow, 26, data.Vat.HasValue ? data.Vat.Value.ToString() : "");
                //slDocument.SetCellValueNumeric(iRow, 27, data.TotalMonthlyInstallment.HasValue ? data.TotalMonthlyInstallment.Value.ToString() : "");
                //slDocument.SetCellValue(iRow, 28, data.PoNumber);
                //slDocument.SetCellValue(iRow, 29, data.PoLine);
                //slDocument.SetCellValue(iRow, 30, data.Function);
                //slDocument.SetCellValue(iRow, 31, data.Regional);
                //slDocument.SetCellValue(iRow, 32, data.VehicleStatus == true ? "Active" : "InActive");
                iRow++;
            }
            slDocument.AutoFitColumn(1, 5);

            return slDocument;
        }
        #endregion
    }
}
