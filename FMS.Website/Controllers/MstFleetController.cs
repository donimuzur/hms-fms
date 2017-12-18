using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using FMS.BLL.Vendor;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Utils;
using FMS.Website.Models;
using FMS.Website.Utility;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Controllers
{
    public class MstFleetController : BaseController
    {
        private IFleetBLL _fleetBLL;
        private IVendorBLL _vendorBLL;
        private IPageBLL _pageBLL;
        private IGroupCostCenterBLL _groupCostCenterBLL;
        private ILocationMappingBLL _locationMappingBLL;
        private ISettingBLL _settingBLL;
        private Enums.MenuList _mainMenu;

        public MstFleetController(IPageBLL PageBll, IFleetBLL  FleetBLL, IVendorBLL VendorBLL, IGroupCostCenterBLL GroupCostCenterBLL
            , ILocationMappingBLL LocationMappingBLL, ISettingBLL SettingBLL)
            : base(PageBll, Enums.MenuList.MasterFleet)
        {
            _fleetBLL = FleetBLL;
            _vendorBLL = VendorBLL;
            _pageBLL = PageBll;
            _groupCostCenterBLL = GroupCostCenterBLL;
            _locationMappingBLL = LocationMappingBLL;
            _settingBLL = SettingBLL;
            _mainMenu = Enums.MenuList.MasterData;
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
                        
            model.SearchView.PoliceNumberList = new SelectList(fleetList.Select(x => new { x.PoliceNumber }).Distinct().ToList(), "PoliceNumber", "PoliceNumber");
            model.SearchView.EmployeeNameList = new SelectList(fleetList.Select(x => new { x.EmployeeName }).Distinct().ToList(), "EmployeeName", "EmployeeName");
            model.SearchView.ChasisNumberList = new SelectList(fleetList.Select(x => new { x.ChasisNumber }).Distinct().ToList(), "ChasisNumber", "ChasisNumber");
            model.SearchView.EmployeeIDList = new SelectList(fleetList.Select(x => new { x.EmployeeID }).Distinct().ToList(), "EmployeeID", "EmployeeID");
            model.SearchView.EngineNumberList = new SelectList(fleetList.Select(x => new { x.EngineNumber }).Distinct().ToList(), "EngineNumber", "EngineNumber");
            model.SearchView.SupplyMethodList = new SelectList(fleetList.Select(x => new { x.SupplyMethod }).Distinct().ToList(), "SupplyMethod", "SupplyMethod");
            model.SearchView.BodyTypeList = new SelectList(fleetList.Select(x => new { x.BodyType }).Distinct().ToList(), "BodyType", "BodyType");
            model.SearchView.VehicleTypeList = new SelectList(fleetList.Select(x => new { x.VehicleType }).Distinct().ToList(), "VehicleType", "VehicleType");
            model.SearchView.VehicleUsageList = new SelectList(fleetList.Select(x => new { x.VehicleUsage }).Distinct().ToList(), "VehicleUsage", "VehicleUsage");
            model.SearchView.VendorList = new SelectList(fleetList.Select(x => new { x.VendorName }).Distinct().ToList(), "VendorName", "VendorName");
            model.SearchView.FunctionList = new SelectList(fleetList.Select(x => new { x.Function }).Distinct().ToList(), "Function", "Function");
            model.SearchView.RegionalList = new SelectList(fleetList.Select(x => new { x.Regional }).Distinct().ToList(), "Regional", "Regional");
            model.SearchView.CityList = new SelectList(fleetList.Select(x => new { x.City }).Distinct().ToList(), "City", "City");

            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            model.WriteAccess = CurrentPageAccess.WriteAccess == true ? 1 : 0;
            model.ReadAccess = CurrentPageAccess.ReadAccess == true ? 1 : 0;

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
            param.EndRent = searchView.EndRent;
            param.Regional = searchView.Regional;
            param.City = searchView.City;
            param.EmployeeId = searchView.EmployeeId;
            param.FormalName = searchView.FormalName;
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

            model.VendorList = new SelectList(data, "VendorName", "VendorName", model.VendorName);

            var list1 = new List<SelectListItem>
            {
                new SelectListItem { Text = "Extend", Value = "Extend" },
                new SelectListItem { Text = "Temporary", Value = "Temporary" },
                new SelectListItem { Text = "Lease", Value = "Lease" },
                new SelectListItem { Text = "Services", Value = "Services" }
            };
            model.SupplyMethodList = new SelectList(list1, "Value", "Text",model.SupplyMethod);

            list1 = new List<SelectListItem>
            {
                new SelectListItem { Text = "Automatic", Value = "Automatic" },
                new SelectListItem { Text = "Manual", Value = "Manual" }
            };
            model.TransmissionList = new SelectList(list1, "Value", "Text", model.Transmission);

            list1 = new List<SelectListItem>
            {
                new SelectListItem { Text = "MPV", Value = "MPV" },
                new SelectListItem { Text = "SUV", Value = "SUV" },
                new SelectListItem { Text = "Forklift", Value = "Forklift" },
                new SelectListItem { Text = "Motorcycle", Value = "Motorcycle" },
                new SelectListItem { Text = "Truck", Value = "Truck" }
            };
            model.BodyTypeList = new SelectList(list1, "Value", "Text",model.BodyType);

            list1 = new List<SelectListItem>
            {
                new SelectListItem { Text = "Gasoline", Value = "Gasoline" },
                new SelectListItem { Text = "Diesel", Value = "Diesel" }
            };
            model.FuelTypeList = new SelectList(list1, "Value", "Text", model.FuelType);

            var groupCostData = _groupCostCenterBLL.GetGroupCenter().Where(x => x.IsActive == true);
            model.FunctionList = new SelectList(groupCostData, "FunctionName", "FunctionName", model.Function);

            var locationMappingData = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive == true);
            model.RegionalList = new SelectList(locationMappingData, "Region", "Region", model.Regional);

            return model;
        }

        public ActionResult Edit(int? MstFleetId)
        {
            if (!MstFleetId.HasValue)
            {
                return HttpNotFound();
            }
            var data = _fleetBLL.GetFleetById(MstFleetId.Value);
            var model = Mapper.Map<FleetItem>(data);
            model = initEdit(model);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterFleet, MstFleetId.Value);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(FleetItem model)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<FleetDto>(model);
                data.ModifiedBy = CurrentUser.USERNAME;
                data.ModifiedDate = DateTime.Now;

                _fleetBLL.Save(data, CurrentUser);
            }
            return RedirectToAction("Index","MstFleet");
        }

        public ActionResult Detail(int MstFleetId)
        {
            var data = _fleetBLL.GetFleetById(MstFleetId);
            var model = Mapper.Map<FleetItem>(data);
            model = initEdit(model);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterFleet, MstFleetId);
            return View(model);
        }


        public ActionResult Upload()
        {
            var model = new FleetModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        [HttpPost]
        public ActionResult Upload(FleetModel model)
        {
            if (ModelState.IsValid)
            {
                foreach(var data in model.Details)
                {
                    try
                    {
                        data.CreatedDate = DateTime.Now;
                        data.CreatedBy = CurrentUser.USER_ID;
                        data.IsActive = true;
                        if (data.EmployeeID == "null" ||  data.EmployeeID == "NULL" || data.EmployeeID == null)
                        {
                            data.EmployeeID = null;
                        }

                        var dto = Mapper.Map<FleetDto>(data);
                        _fleetBLL.Save(dto);
                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                    }
                    catch (Exception ex)
                    {
                        var msg = ex.Message;
                        var index = model.Details.IndexOf(data) + 1;
                        model.ErrorMessage = "Please Check your Data number "+index+"  and Police Number "+data.PoliceNumber+ ". ";
                        model.ErrorMessage = model.ErrorMessage + (ex.InnerException == null ? ex.Message : ex.InnerException.InnerException.Message);
                        model.MainMenu = _mainMenu;
                        model.CurrentLogin = CurrentUser;
                        return View(model);
                    }
                }
            }
            return RedirectToAction("Index", "MstFleet");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var qtyPacked = string.Empty;
            var qty = string.Empty;
            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<FleetItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    try
                    {
                        if (dataRow.Count <= 0)
                        {
                            continue;
                        }
                        else if (dataRow[0] == "Police Number")
                        {
                            continue;
                        }
                        var item = new FleetItem();
                        item.PoliceNumber = dataRow[0];
                        if (dataRow[2] != "NULL" & dataRow[2] != "")
                        {
                            item.EmployeeID = dataRow[2];
                        }
                        item.EmployeeName = dataRow[1];
                        item.CostCenter = dataRow[3];
                        item.Manufacturer = dataRow[4];
                        item.Models = dataRow[5];
                        item.Series = dataRow[6];
                        item.Transmission = dataRow[7];
                        item.BodyType = dataRow[8];
                        item.FuelType = dataRow[9];
                        item.Branding = dataRow[10];
                        item.Color = dataRow[11];
                        item.Airbag = dataRow[12] == "Yes"? true: false;
                        item.AirbagS = dataRow[12];
                        item.ChasisNumber = dataRow[13];
                        item.EngineNumber = dataRow[14];
                        item.VehicleYear = Convert.ToInt32(dataRow[15]);
                        item.VehicleType = dataRow[16];
                        item.VehicleUsage = dataRow[17];
                        item.Project = dataRow[18] == "Yes" ? true : false;
                        item.ProjectS = dataRow[18];
                        item.ProjectName = dataRow[19];
                        if (dataRow[20] != "NULL" && dataRow[20] != "")
                        {
                            double dStartDate = double.Parse(dataRow[21].ToString());
                            DateTime dtStartDate = DateTime.FromOADate(dStartDate);
                            item.StartDate = dtStartDate;
                        }
                        if (dataRow[21] != "NULL" && dataRow[21] != "")
                        {
                            double dEndDate = double.Parse(dataRow[21].ToString());
                            DateTime dtEndDate = DateTime.FromOADate(dEndDate);
                            item.EndDate = dtEndDate;
                        }
                        item.VendorName = dataRow[22];
                        item.City = dataRow[23];
                        item.SupplyMethod = dataRow[24];
                        item.Restitution = dataRow[25] == "Yes" ? true : false;
                        item.RestitutionS = dataRow[25];
                        item.MonthlyHMSInstallment = Convert.ToInt32(dataRow[26]);
                        item.VatDecimal = Convert.ToInt64(dataRow[27]);
                        item.PoNumber = dataRow[28];
                        item.PoLine = dataRow[29];
                        item.CarGroupLevel = Convert.ToInt32(dataRow[30]);
                        if(dataRow[31] != "NULL" && dataRow[31] != "")
                        {
                            item.GroupLevel = Convert.ToInt32(dataRow[31]);
                        }
                        else
                        {
                            item.GroupLevel = 0;
                        }
                        item.AssignedTo = dataRow[32];
                        item.Address = dataRow[33];

                        if (dataRow[34] != "NULL" && dataRow[34] != "")
                        {
                            double dStartContract = double.Parse(dataRow[34].ToString());
                            DateTime dtStartContract = DateTime.FromOADate(dStartContract);
                            item.StartContract = dtStartContract;
                        }
                        if (dataRow[35] != "NULL" && dataRow[35] != "")
                        {
                            double dEndContract = double.Parse(dataRow[35].ToString());
                            DateTime dtEndContract = DateTime.FromOADate(dEndContract);
                            item.EndContract = dtEndContract;
                        }

                        item.VehicleStatus = dataRow[36];
                        item.CertificateOwnership = dataRow[37];
                        item.Comments = dataRow[38];
                        item.Assets = dataRow[39];
                        string TotalMonthlyCharge = dataRow[40];
                        TotalMonthlyCharge = TotalMonthlyCharge.Trim(',');
                        item.TotalMonthlyCharge = Int64.Parse(String.IsNullOrEmpty(TotalMonthlyCharge) ? "0" : TotalMonthlyCharge);
                        item.Function = dataRow[41];
                        if(dataRow.Count<= 42)
                        {
                            item.Regional = "";
                        }
                        else
                        {
                            item.Regional = dataRow[42];
                        }
                        model.Add(item);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return Json(model);
        }

        #region export xls
        public void ExportMasterFleet()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterFleet();

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

        private string CreateXlsMasterFleet()
        {
            //get data
            List<FleetDto> fleet = _fleetBLL.GetFleet();
            var listData = Mapper.Map<List<FleetItem>>(fleet);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Fleet");
            slDocument.MergeWorksheetCells(1, 1, 1,52);
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

            var fileName = "Master_Data_Fleet" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterFleet(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Police Number");
            slDocument.SetCellValue(iRow, 2, "Chasis Number");
            slDocument.SetCellValue(iRow, 3, "Engine Number");
            slDocument.SetCellValue(iRow, 4, "Employee ID");
            slDocument.SetCellValue(iRow, 5, "Employee Name");
            slDocument.SetCellValue(iRow, 6, "Group Level");
            slDocument.SetCellValue(iRow, 7, "Actual Group");
            slDocument.SetCellValue(iRow, 8, "Assigned To");
            slDocument.SetCellValue(iRow, 9, "Cost Center");
            slDocument.SetCellValue(iRow, 10, "Vendor Name");
            slDocument.SetCellValue(iRow, 11, "Manufacturer");
            slDocument.SetCellValue(iRow, 12, "Models");
            slDocument.SetCellValue(iRow, 13, "Series");
            slDocument.SetCellValue(iRow, 14, "Body Type");
            slDocument.SetCellValue(iRow, 15, "Color");
            slDocument.SetCellValue(iRow, 16, "Transmission");
            slDocument.SetCellValue(iRow, 17, "Car Group Level");
            slDocument.SetCellValue(iRow, 18, "Fuel Type");
            slDocument.SetCellValue(iRow, 19, "Branding");
            slDocument.SetCellValue(iRow, 20, "Airbag");
            slDocument.SetCellValue(iRow, 21, "Vehicle Year");
            slDocument.SetCellValue(iRow, 22, "Vehicle Type");
            slDocument.SetCellValue(iRow, 23, "Vehicle Usage");
            slDocument.SetCellValue(iRow, 24, "Supply Method");
            slDocument.SetCellValue(iRow, 25, "City");
            slDocument.SetCellValue(iRow, 26, "Address");
            slDocument.SetCellValue(iRow, 27, "Purpose");
            slDocument.SetCellValue(iRow, 28, "Vat");
            slDocument.SetCellValue(iRow, 29, "Restitution");
            slDocument.SetCellValue(iRow, 30, "Star tDate");
            slDocument.SetCellValue(iRow, 31, "End Date");
            slDocument.SetCellValue(iRow, 32, "Termination Date");
            slDocument.SetCellValue(iRow, 33, "PO Number");
            slDocument.SetCellValue(iRow, 34, "PO Line");
            slDocument.SetCellValue(iRow, 35, "Start Contract");
            slDocument.SetCellValue(iRow, 36, "End Contract");
            slDocument.SetCellValue(iRow, 37, "Price");
            slDocument.SetCellValue(iRow, 38, "Vehicle Status");
            slDocument.SetCellValue(iRow, 39, "Is Taken");
            slDocument.SetCellValue(iRow, 40, "GR Left Qty");
            slDocument.SetCellValue(iRow, 41, "Certificate Of Ownership");
            slDocument.SetCellValue(iRow, 42, "Comments");
            slDocument.SetCellValue(iRow, 43, "Asset");
            slDocument.SetCellValue(iRow, 44, "Total Monthly Charge");
            slDocument.SetCellValue(iRow, 45, "Function");
            slDocument.SetCellValue(iRow, 46, "Regional");
            slDocument.SetCellValue(iRow, 47, "Created By");
            slDocument.SetCellValue(iRow, 48, "Created Date");
            slDocument.SetCellValue(iRow, 49, "Modified By");
            slDocument.SetCellValue(iRow, 50, "Modified Date");
            slDocument.SetCellValue(iRow, 51, "Modified By");
            slDocument.SetCellValue(iRow, 52, "Status");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 52, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelMasterFleet(SLDocument slDocument, List<FleetItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 2, data.ChasisNumber);
                slDocument.SetCellValue(iRow, 3, data.EngineNumber);
                slDocument.SetCellValue(iRow, 4, data.EmployeeID);
                slDocument.SetCellValue(iRow, 5, data.EmployeeName );
                slDocument.SetCellValue(iRow, 6, data.GroupLevel);
                slDocument.SetCellValue(iRow, 7, data.ActualGroup);
                slDocument.SetCellValue(iRow, 8, data.AssignedTo);
                slDocument.SetCellValue(iRow, 9, data.CostCenter);
                slDocument.SetCellValue(iRow, 10, data.VendorName);
                slDocument.SetCellValue(iRow, 11, data.Manufacturer);
                slDocument.SetCellValue(iRow, 12, data.Models);
                slDocument.SetCellValue(iRow, 13, data.Series);
                slDocument.SetCellValue(iRow, 14, data.BodyType);
                slDocument.SetCellValue(iRow, 15, data.Color);
                slDocument.SetCellValue(iRow, 16, data.Transmission);
                slDocument.SetCellValue(iRow, 17, data.CarGroupLevel);
                slDocument.SetCellValue(iRow, 18, data.FuelType);
                slDocument.SetCellValue(iRow, 19, data.Branding);
                slDocument.SetCellValue(iRow, 20, data.Airbag);
                slDocument.SetCellValue(iRow, 21, data.VehicleYear);
                slDocument.SetCellValue(iRow, 22, data.VehicleType);
                slDocument.SetCellValue(iRow, 23, data.VehicleUsage);
                slDocument.SetCellValue(iRow, 24, data.SupplyMethod);
                slDocument.SetCellValue(iRow, 25, data.City);
                slDocument.SetCellValue(iRow, 26, data.Address);
                slDocument.SetCellValue(iRow, 27, data.Purpose);
                slDocument.SetCellValue(iRow, 28, data.Vat);
                slDocument.SetCellValue(iRow, 29, data.Restitution);
                slDocument.SetCellValue(iRow, 30, data.StartDate == null ? "" : data.StartDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 31, data.EndDate == null ? "" :  data.EndDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 32, data.TerminationDate == null ? "" : data.TerminationDate.Value.ToString("dd-MMM-yyyy hh: mm"));
                slDocument.SetCellValue(iRow, 33, data.PoNumber);
                slDocument.SetCellValue(iRow, 34, data.PoLine);
                slDocument.SetCellValue(iRow, 35, data.StartContract== null ? "" : data.StartContract.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 36, data.EndContract == null ? "" : data.EndContract.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 37, data.Price);
                slDocument.SetCellValue(iRow, 38, data.VehicleStatus);
                slDocument.SetCellValue(iRow, 39, data.IsTaken);
                slDocument.SetCellValue(iRow, 40, data.GrLeftQty);
                slDocument.SetCellValue(iRow, 41, data.CertificateOwnership);
                slDocument.SetCellValue(iRow, 42, data.Comments);
                slDocument.SetCellValue(iRow, 43, data.Assets);
                slDocument.SetCellValue(iRow, 44, data.TotalMonthlyCharge == null? 0 : (decimal)data.TotalMonthlyCharge);
                slDocument.SetCellValue(iRow, 45, data.Function);
                slDocument.SetCellValue(iRow, 46, data.Regional);
                slDocument.SetCellValue(iRow, 47, data.CreatedBy);
                slDocument.SetCellValue(iRow, 48, data.CreatedDate.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 49, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 50, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 51, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 52, data.IsActive == true ? "Active" : "InActive");
          

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 52);
            slDocument.SetCellStyle(3, 1, iRow - 1, 52, valueStyle);

            return slDocument;
        }

        #endregion

    }
}
