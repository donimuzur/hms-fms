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
using FMS.BusinessObject.Dto;
using FMS.Website.Utility;

namespace FMS.Website.Controllers
{
    public class RptVehicleOverallController : BaseController
    {
        private IFleetBLL _fleetBLL;
        private IVendorBLL _vendorBLL;
        private IPageBLL _pageBLL;
        private IGroupCostCenterBLL _groupCostCenterBLL;
        private ILocationMappingBLL _locationMappingBLL;
        private ISettingBLL _settingBLL;
        private IEmployeeBLL _employeeBLL;
        private Enums.MenuList _mainMenu;

        public RptVehicleOverallController(IPageBLL PageBll, IFleetBLL FleetBLL, IVendorBLL VendorBLL, IGroupCostCenterBLL GroupCostCenterBLL
            , ILocationMappingBLL LocationMappingBLL, ISettingBLL SettingBLL, IEmployeeBLL EmployeeBLL)
            : base(PageBll, Enums.MenuList.MasterFleet)
        {
            _fleetBLL = FleetBLL;
            _vendorBLL = VendorBLL;
            _pageBLL = PageBll;
            _groupCostCenterBLL = GroupCostCenterBLL;
            _locationMappingBLL = LocationMappingBLL;
            _settingBLL = SettingBLL;
            _employeeBLL = EmployeeBLL;
            _mainMenu = Enums.MenuList.RptExecutiveSummary;
        }

        #region List View
        // GET: /MstFleet/
        public ActionResult Index()
        {
            //var data = _fleetBLL.GetFleet();
            var model = new FleetModel();

            model.SearchView = new FleetSearchView();
            //model.Details=Mapper.Map<List<FleetItem>>(data);
            //model.Details = new List<FleetItem>();

            var fleetList = _fleetBLL.GetFleet().ToList();
            var settingData = _settingBLL.GetSetting().Where(x => x.IsActive);
            var listSupMethod = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.SupplyMethod)).Select(x => new { x.SettingValue }).ToList();
            var listBodType = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.BodyType)).Select(x => new { x.SettingValue }).ToList();
            var listVehType = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType)).Select(x => new { x.SettingValue }).ToList();
            var locationMappingData = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive == true);
            var groupCostData = _groupCostCenterBLL.GetGroupCenter().Where(x => x.IsActive == true);
            var data = _vendorBLL.GetVendor().Where(x => x.IsActive == true);

            model.SearchView.PoliceNumberList = new SelectList(fleetList.Select(x => new { x.PoliceNumber }).Distinct().ToList(), "PoliceNumber", "PoliceNumber");
            model.SearchView.EmployeeNameList = new SelectList(fleetList.Select(x => new { x.EmployeeName }).Distinct().ToList(), "EmployeeName", "EmployeeName");
            model.SearchView.ChasisNumberList = new SelectList(fleetList.Select(x => new { x.ChasisNumber }).Distinct().ToList(), "ChasisNumber", "ChasisNumber");
            model.SearchView.EmployeeIDList = new SelectList(fleetList.Select(x => new { x.EmployeeID }).Distinct().ToList(), "EmployeeID", "EmployeeID");
            model.SearchView.EngineNumberList = new SelectList(fleetList.Select(x => new { x.EngineNumber }).Distinct().ToList(), "EngineNumber", "EngineNumber");
            model.SearchView.SupplyMethodList = new SelectList(listSupMethod, "SettingValue", "SettingValue");
            model.SearchView.BodyTypeList = new SelectList(listBodType, "SettingValue", "SettingValue");
            model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
            model.SearchView.VehicleUsageList = new SelectList(fleetList.Select(x => new { x.VehicleUsage }).Distinct().ToList(), "VehicleUsage", "VehicleUsage");
            model.SearchView.VendorList = new SelectList(data, "ShortName", "ShortName");
            model.SearchView.FunctionList = new SelectList(groupCostData.Select(x => new { x.FunctionName }).Distinct().ToList(), "FunctionName", "FunctionName");
            model.SearchView.RegionalList = new SelectList(locationMappingData.Select(x => new { x.Region }).Distinct().ToList(), "Region", "Region");
            model.SearchView.CityList = new SelectList(locationMappingData.Select(x => new { x.Basetown }).Distinct().ToList(), "Basetown", "Basetown");

            model.SearchView.StatusSource = "True";

            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;

            return View("Index", model);
        }

        [HttpPost]
        public JsonResult SearchFleetAjax(DTParameters<FleetModel> param)
        {
            var model = param;

            var data = model != null ? SearchDataFleet(model) : SearchDataFleet();
            DTResult<FleetItem> result = new DTResult<FleetItem>();
            result.draw = param.Draw;
            result.recordsFiltered = data.Count;
            result.recordsTotal = data.Count;
            //param.TotalData = data.Count;
            //if (param != null && param.Start > 0)
            //{
            IEnumerable<FleetItem> dataordered;
            dataordered = data;
            if (param.Order.Length > 0)
            {
                foreach (var ordr in param.Order)
                {
                    if (ordr.Column == 0)
                    {
                        continue;
                    }
                    dataordered = FleetDataOrder(FleetDataOrderByIndex(ordr.Column), ordr.Dir, dataordered);
                }
            }
            data = dataordered.ToList();
            data = data.Skip(param.Start).Take(param.Length).ToList();

            //}
            result.data = data;

            return Json(result);
        }

        private List<FleetItem> SearchDataFleet(DTParameters<FleetModel> searchView = null)
        {
            var param = new FleetParamInput();
            param.Status = searchView.StatusSource;
            param.SupplyMethod = searchView.SupplyMethod;
            param.BodyType = searchView.BodyType;
            param.VehicleType = searchView.VehicleType;
            param.VehicleUsage = searchView.VehicleUsage;
            param.Vendor = searchView.Vendor;
            param.Function = searchView.Function;
            param.StartRent = searchView.StartRent;
            param.StartRentTo = searchView.StartRentTo;
            param.EndRent = searchView.EndRent;
            param.EndRentTo = searchView.EndRentTo;
            param.EndDate = searchView.EndDate;
            param.EndDateTo = searchView.EndDateTo;
            param.Regional = searchView.Regional;
            param.City = searchView.City;
            param.EmployeeId = searchView.EmployeeId;
            param.FormalName = searchView.FormalName;
            param.PoliceNumber = searchView.PoliceNumber;

            var data = _fleetBLL.GetFleetByParam(param);
            return Mapper.Map<List<FleetItem>>(data);
        }

        private List<FleetItem> SearchDataFleetExport(FleetSearchView searchView = null)
        {
            var param = new FleetParamInput();
            param.Status = searchView.StatusSource;
            param.SupplyMethod = searchView.SupplyMethod;
            param.BodyType = searchView.BodyType;
            param.VehicleType = searchView.VehicleType;
            param.VehicleUsage = searchView.VehicleUsage;
            param.Vendor = searchView.Vendor;
            param.Function = searchView.Function;
            param.StartRent = searchView.StartRent;
            param.StartRentTo = searchView.StartRentTo;
            param.EndRent = searchView.EndRent;
            param.EndRentTo = searchView.EndRentTo;
            param.EndDate = searchView.EndDate;
            param.EndDateTo = searchView.EndDateTo;
            param.Regional = searchView.Regional;
            param.City = searchView.City;
            param.EmployeeId = searchView.EmployeeID;
            param.FormalName = searchView.EmployeeName;
            param.PoliceNumber = searchView.PoliceNumber;

            var data = _fleetBLL.GetFleetByParam(param);
            return Mapper.Map<List<FleetItem>>(data);
        }

        private IEnumerable<FleetItem> FleetDataOrder(string column, DTOrderDir dir, IEnumerable<FleetItem> data)
        {

            switch (column)
            {
                case "VehicleType": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.VehicleType).ToList() : data.OrderByDescending(x => x.VehicleType).ToList();
                case "VehicleUsage": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.VehicleUsage).ToList() : data.OrderByDescending(x => x.VehicleUsage).ToList();
                case "SupplyMethod": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.SupplyMethod).ToList() : data.OrderByDescending(x => x.SupplyMethod).ToList();
                case "BodyType": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.BodyType).ToList() : data.OrderByDescending(x => x.BodyType).ToList();

            }
            return null;
        }

        private string FleetDataOrderByIndex(int index)
        {
            Dictionary<int, string> columnDict = new Dictionary<int, string>();
            columnDict.Add(17, "VehicleType");
            columnDict.Add(18, "VehicleUsage");
            columnDict.Add(24, "SupplyMethod");
            columnDict.Add(9, "BodyType");


            return columnDict[index];
        }
        #endregion

        #region ----Details-----
        public ActionResult DetailsVehicle(long id)
        {
            var ListVehicle = _fleetBLL.GetFleet();
            var model = new VehicleOverallItem();
            var Getvehicle= ListVehicle.Where(x => (x.MstFleetId == id)).FirstOrDefault();

            try
            {
                model = Mapper.Map<VehicleOverallItem>(Getvehicle);
            }
            catch (Exception exp)
            {
                var msg = exp.Message;   
            }
            

            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;

            model.MonthlyInstallmentStr = model.MonthlyInstallment == null ? "" : string.Format("{0:n0}", model.MonthlyInstallment);
            model.VatStr = model.Vat == null ? "" : string.Format("{0:n0}", model.Vat);
            model.TotalMonthlyInstallmentStr = model.TotalMonthlyInstallment == null ? "" : string.Format("{0:n0}", model.TotalMonthlyInstallment);


            var History = _fleetBLL.GetFleet().Where(x => (x.ChasisNumber == null ? "" : x.ChasisNumber.ToUpper()) == (model.ChasisNumber == null ? "" : model.ChasisNumber.ToUpper())
                                                           && (x.PoliceNumber == null ? "" : x.PoliceNumber.ToUpper()) == (model.PoliceNumber == null ? "" : model.PoliceNumber.ToUpper())
                                                           && x.StartContract == model.StartContract && x.EndContract == model.EndContract).GroupBy(x => x.CreatedDate)
                                                           .Select(x => new VehicleHistory
                                                           {
                                                               Employee = x.First().EmployeeName,
                                                               Date = x.First().CreatedDate
                                                               //Description = x.First().VehicleStatus
                                                           }).OrderBy(x => x.Date).ToList();

            model.DetailsHistory = new List<VehicleHistory>();
            if (History != null && History.Count > 0) model.DetailsHistory = History;

            return View(model);
        }
        #endregion

        #region Create
        public FleetItem initCreate()
        {
            var data = _vendorBLL.GetVendor().Where(x => x.IsActive == true);
            var model = new FleetItem();

            model.VendorList = new SelectList(data, "VendorName", "VendorName");

            var list1 = new List<SelectListItem>
            {
                new SelectListItem { Text = "Extend", Value = "Extend" },
                new SelectListItem { Text = "Temporary", Value = "Temporary" },
                new SelectListItem { Text = "Lease", Value = "Lease" },
                new SelectListItem { Text = "Services", Value = "Services" }
            };
            model.SupplyMethodList = new SelectList(list1, "Value", "Text");

            list1 = new List<SelectListItem>
            {
                new SelectListItem { Text = "Automatic", Value = "Automatic" },
                new SelectListItem { Text = "Manual", Value = "Manual" }
            };
            model.TransmissionList = new SelectList(list1, "Value", "Text");

            list1 = new List<SelectListItem>
            {
                new SelectListItem { Text = "MPV", Value = "MPV" },
                new SelectListItem { Text = "SUV", Value = "SUV" },
                new SelectListItem { Text = "Forklift", Value = "Forklift" },
                new SelectListItem { Text = "Motorcycle", Value = "Motorcycle" },
                new SelectListItem { Text = "Truck", Value = "Truck" }
            };
            model.BodyTypeList = new SelectList(list1, "Value", "Text");

            return model;
        }

        //public ActionResult Create()
        //{
        //    var model = initCreate();
        //    model.MainMenu = _mainMenu;

        //    return View(model);
        //}

        //[HttpPost]
        //public ActionResult Create(FleetItem Model)
        //{
        //    try
        //    {
        //        if(ModelState.IsValid)
        //        {
        //            var dto = Mapper.Map<FleetDto>(Model);
        //            dto.CreatedDate = DateTime.Now;
        //            dto.CreatedBy = "Doni";
        //            dto.IsActive = true;
        //            _fleetBLL.Save(dto);
        //        }
        //    }
        //    catch (Exception )
        //    {
        //        var model = initCreate();
        //        return View(model);
        //    }

        //    return RedirectToAction("Index","MstFleet"); 
        //}
        #endregion

        public FleetItem initEdit(FleetItem model)
        {
            var data = _vendorBLL.GetVendor().Where(x => x.IsActive == true);
            var settingData = _settingBLL.GetSetting().Where(x => x.IsActive);
            var listSupMethod = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.SupplyMethod)).Select(x => new { x.SettingValue }).ToList();
            var groupCostData = _groupCostCenterBLL.GetGroupCenter().Where(x => x.IsActive == true);
            var locationMappingData = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive == true);
            var listBodType = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.BodyType)).Select(x => new { x.SettingValue }).ToList();
            var listFuelType = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.FuelType)).Select(x => new { x.SettingValue }).ToList();
            var listTransmission = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.Transmission)).Select(x => new { x.SettingValue }).ToList();
            var listVehType = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType)).Select(x => new { x.SettingValue }).ToList();
            var listVehUsage = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleUsageBenefit)).Select(x => new { x.SettingValue }).ToList();
            var listProject = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.Project)).Select(x => new { x.SettingValue }).ToList();

            model.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
            model.VehicleUsageList = new SelectList(listVehUsage, "SettingValue", "SettingValue");
            model.ProjectList = new SelectList(listProject, "SettingValue", "SettingValue");
            model.VendorList = new SelectList(data, "VendorName", "VendorName", model.VendorName);
            model.SupplyMethodList = new SelectList(listSupMethod, "SettingValue", "SettingValue");
            model.TransmissionList = new SelectList(listTransmission, "SettingValue", "SettingValue");
            model.BodyTypeList = new SelectList(listBodType, "SettingValue", "SettingValue");
            model.FuelTypeList = new SelectList(listFuelType, "SettingValue", "SettingValue");
            model.FunctionList = new SelectList(groupCostData.Select(x => new { x.FunctionName }).Distinct().ToList(), "FunctionName", "FunctionName", model.Function);
            model.RegionalList = new SelectList(locationMappingData.Select(x => new { x.Region }).Distinct().ToList(), "Region", "Region", model.Regional);

            model.VatDecimalStr = model.VatDecimal == null ? "" : string.Format("{0:n0}", model.VatDecimal);
            model.MonthlyHMSInstallmentStr = string.Format("{0:n0}", model.MonthlyHMSInstallment);
            model.TotalMonthlyChargeStr = model.TotalMonthlyCharge == null ? "" : string.Format("{0:n0}", model.TotalMonthlyCharge);

            return model;
        }
        
        [HttpPost]
        public JsonResult GetVehUsageByVehType(string vehType)
        {
            var paramVehUsage = EnumHelper.GetDescription(Enums.SettingGroup.VehicleUsageWtc);

            if (vehType.ToUpper() == "BENEFIT")
            {
                paramVehUsage = EnumHelper.GetDescription(Enums.SettingGroup.VehicleUsageBenefit);
            }

            var listVehUsage = _settingBLL.GetSetting().Where(x => x.SettingGroup == paramVehUsage && x.IsActive).ToList();

            return Json(listVehUsage);
        }

        #region export xls

        public string ExportMasterFleetGenerateReport(FleetModel model = null)
        {
            string pathFile = "";
            pathFile = CreateXlsMasterFleet(model);
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

        private string CreateXlsMasterFleet(FleetModel model = null)
        {
            //get data
            List<FleetDto> fleet = _fleetBLL.GetFleet();
            var listData = SearchDataFleetExport(model.SearchView);
            //var listData = Mapper.Map<List<FleetItem>>(fleet);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Vehicle Report");
            slDocument.MergeWorksheetCells(1, 1, 1, 43);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterFleet(slDocument);

            //create data
            slDocument = CreateDataExcelMasterFleet(slDocument, listData);

            var fileName = "Vehicle_Report" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterFleet(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Police Number");
            slDocument.SetCellValue(iRow, 2, "Employee Name");
            slDocument.SetCellValue(iRow, 3, "Employee ID");
            slDocument.SetCellValue(iRow, 4, "Cost Center");
            slDocument.SetCellValue(iRow, 5, "Manufacturer");
            slDocument.SetCellValue(iRow, 6, "Model");
            slDocument.SetCellValue(iRow, 7, "Series");
            slDocument.SetCellValue(iRow, 8, "Transmission");
            slDocument.SetCellValue(iRow, 9, "Body Type");
            slDocument.SetCellValue(iRow, 10, "Fuel Type");
            slDocument.SetCellValue(iRow, 11, "Branding");
            slDocument.SetCellValue(iRow, 12, "Color");
            slDocument.SetCellValue(iRow, 13, "Airbag");
            slDocument.SetCellValue(iRow, 14, "Chasis Number");
            slDocument.SetCellValue(iRow, 15, "Engine Number");
            slDocument.SetCellValue(iRow, 16, "Request Year");
            slDocument.SetCellValue(iRow, 17, "Vehicle Type");
            slDocument.SetCellValue(iRow, 18, "Vehicle Usage");
            slDocument.SetCellValue(iRow, 19, "Project");
            slDocument.SetCellValue(iRow, 20, "Project Name");
            slDocument.SetCellValue(iRow, 21, "Start Contract");
            slDocument.SetCellValue(iRow, 22, "End Contract");
            slDocument.SetCellValue(iRow, 23, "Vendor Name");
            slDocument.SetCellValue(iRow, 24, "Basetown");
            slDocument.SetCellValue(iRow, 25, "Supply Method");
            slDocument.SetCellValue(iRow, 26, "Restitution");
            slDocument.SetCellValue(iRow, 27, "Monthly HMS Installment");
            slDocument.SetCellValue(iRow, 28, "Vat");
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

        private SLDocument CreateDataExcelMasterFleet(SLDocument slDocument, List<FleetItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 2, data.EmployeeName);
                slDocument.SetCellValue(iRow, 3, data.EmployeeID);
                slDocument.SetCellValue(iRow, 4, data.CostCenter);
                slDocument.SetCellValue(iRow, 5, data.Manufacturer);
                slDocument.SetCellValue(iRow, 6, data.Models);
                slDocument.SetCellValue(iRow, 7, data.Series);
                slDocument.SetCellValue(iRow, 8, data.Transmission);
                slDocument.SetCellValue(iRow, 9, data.BodyType);
                slDocument.SetCellValue(iRow, 10, data.FuelType);
                slDocument.SetCellValue(iRow, 11, data.Branding);
                slDocument.SetCellValue(iRow, 12, data.Color);
                slDocument.SetCellValue(iRow, 13, data.Airbag == true ? "Yes" : "No");
                slDocument.SetCellValue(iRow, 14, data.ChasisNumber);
                slDocument.SetCellValue(iRow, 15, data.EngineNumber);
                slDocument.SetCellValue(iRow, 16, data.VehicleYear);
                slDocument.SetCellValue(iRow, 17, data.VehicleType);
                slDocument.SetCellValue(iRow, 18, data.VehicleUsage);
                slDocument.SetCellValue(iRow, 19, data.Project == true ? "Yes" : "No");
                slDocument.SetCellValue(iRow, 20, data.ProjectName);
                slDocument.SetCellValue(iRow, 21, data.StartContract == null ? "" : data.StartContract.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 22, data.EndContract == null ? "" : data.EndContract.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 23, data.VendorName);
                slDocument.SetCellValue(iRow, 24, data.City);
                slDocument.SetCellValue(iRow, 25, data.SupplyMethod);
                slDocument.SetCellValue(iRow, 26, data.Restitution == true ? "Yes" : "No");
                slDocument.SetCellValue(iRow, 27, data.MonthlyHMSInstallment);
                slDocument.SetCellValue(iRow, 28, data.VatDecimal == null ? 0 : data.VatDecimal.Value);
                slDocument.SetCellValue(iRow, 29, data.PoNumber);
                slDocument.SetCellValue(iRow, 30, data.PoLine);
                slDocument.SetCellValue(iRow, 31, data.CarGroupLevel);
                slDocument.SetCellValue(iRow, 32, data.GroupLevel);
                slDocument.SetCellValue(iRow, 33, data.AssignedTo);
                slDocument.SetCellValue(iRow, 34, data.Address);
                slDocument.SetCellValue(iRow, 35, data.StartDate == null ? "" : data.StartDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 36, data.EndDate == null ? "" : data.EndDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 37, data.IsActive ? "Active" : "Not Active");
                slDocument.SetCellValue(iRow, 38, data.CertificateOwnership);
                slDocument.SetCellValue(iRow, 39, "'" + data.Comments);
                slDocument.SetCellValue(iRow, 40, data.Assets);
                slDocument.SetCellValue(iRow, 41, data.TotalMonthlyCharge == null ? 0 : data.TotalMonthlyCharge.Value);
                slDocument.SetCellValue(iRow, 42, data.Function);
                slDocument.SetCellValue(iRow, 43, data.Regional);
                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 43);
            slDocument.SetCellStyle(3, 1, iRow - 1, 43, valueStyle);

            return slDocument;
        }

        #endregion

        #region ------ Export Details -------
        public string ExportDetailsVehicle(VehicleOverallItem model = null)
        {
            string pathFile = "";
            pathFile = CreateXlsDetailsVehicle(model);
            return pathFile;

        }
        private string CreateXlsDetailsVehicle(VehicleOverallItem model)
        {
            //get data
            var filter = new VehicleOverallSearchView();
            var data = new VehicleOverallItem();
            var ListVehicle = _fleetBLL.GetFleet();
            var Getvehicle = ListVehicle.Where(x => (x.MstFleetId == model.MstFleetId)).FirstOrDefault();

            try
            {
                data = Mapper.Map<VehicleOverallItem>(Getvehicle);
            }
            catch (Exception exp)
            {
                var msg = exp.Message;
            }
            
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
            slDocument.SetCellValue(3, 5, Data.EndContract.HasValue ? Data.EndContract.Value.ToString("dd-MMM-yyyy") : "");

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
