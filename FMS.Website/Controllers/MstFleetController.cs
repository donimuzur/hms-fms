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
        private IEmployeeBLL _employeeBLL;
        private Enums.MenuList _mainMenu;

        public MstFleetController(IPageBLL PageBll, IFleetBLL  FleetBLL, IVendorBLL VendorBLL, IGroupCostCenterBLL GroupCostCenterBLL
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
                var exist = _fleetBLL.GetFleet().Where(x => (x.ChasisNumber == null ? "" : x.ChasisNumber.ToUpper()) == (model.ChasisNumber == null ? "" : model.ChasisNumber.ToUpper())
                           && (x.EngineNumber == null ? "" : x.EngineNumber.ToUpper()) == (model.EngineNumber == null ? "" : model.EngineNumber.ToUpper())
                           && x.IsActive).FirstOrDefault();

                if (exist != null)
                {
                    if (CurrentUser.UserRole == Enums.UserRole.Fleet)
                    {
                        exist.IsActive = false;
                        exist.ModifiedBy = "SYSTEM";
                        exist.ModifiedDate = DateTime.Now;
                        _fleetBLL.Save(exist);

                        model.MstFleetId = 0;
                    }
                }

                if (model.MonthlyHMSInstallmentStr != null)
                {
                    model.MonthlyHMSInstallment = Convert.ToDecimal(model.MonthlyHMSInstallmentStr.Replace(",", ""));
                }
                if (model.VatDecimalStr != null)
                {
                    model.VatDecimal = Convert.ToDecimal(model.VatDecimalStr.Replace(",", ""));
                }

                var data = Mapper.Map<FleetDto>(model);
                data.Project = false;
                data.Restitution = true;
                data.TotalMonthlyCharge = 0;
                if (!string.IsNullOrEmpty(data.ProjectName))
                {
                    data.Project = true;
                    if (data.ProjectName.ToLower() == "no project")
                    {
                        data.Project = false;
                    }
                }
                if (data.VatDecimal.Value > 0)
                {
                    data.Restitution = false;
                }
                if (data.MonthlyHMSInstallment > 0)
                {
                    data.TotalMonthlyCharge = data.MonthlyHMSInstallment;
                    if (data.VatDecimal > 0)
                    {
                        data.TotalMonthlyCharge = data.MonthlyHMSInstallment + data.VatDecimal;
                    }
                }
                data.ModifiedBy = CurrentUser.USERNAME;
                data.ModifiedDate = DateTime.Now;

                _fleetBLL.Save(data, CurrentUser);
            }
            else
            {
                var errors = ModelState.Values.Where(c => c.Errors.Count > 0).ToList();

                if (errors.Count > 0)
                {
                    //get error details
                }

                RedirectToAction("Index", "MstFleet");
            }
            return RedirectToAction("Index","MstFleet");
        }

        public ActionResult Detail(int MstFleetId)
        {
            var data = _fleetBLL.GetFleetById(MstFleetId);
            var model = Mapper.Map<FleetItem>(data);
            model = initEdit(model);
            model.MainMenu = _mainMenu;
            model.VatDecimalStr = model.VatDecimal == null ? "" : string.Format("{0:n0}", model.VatDecimal);
            model.MonthlyHMSInstallmentStr = string.Format("{0:n0}", model.MonthlyHMSInstallment);
            model.TotalMonthlyChargeStr = model.TotalMonthlyCharge == null ? "" : string.Format("{0:n0}", model.TotalMonthlyCharge);
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

                        var exist = _fleetBLL.GetFleet().Where(x => (x.ChasisNumber == null ? "" : x.ChasisNumber.ToUpper()) == (data.ChasisNumber == null ? "" : data.ChasisNumber.ToUpper())
                           && (x.EngineNumber == null ? "" : x.EngineNumber.ToUpper()) == (data.EngineNumber == null ? "" : data.EngineNumber.ToUpper())
                           && x.IsActive).FirstOrDefault();

                        if (exist != null)
                        {
                            if (CurrentUser.UserRole == Enums.UserRole.Fleet)
                            {
                                exist.IsActive = false;
                                exist.ModifiedBy = "SYSTEM";
                                exist.ModifiedDate = DateTime.Now;
                                _fleetBLL.Save(exist);
                            }
                        }

                        var dto = Mapper.Map<FleetDto>(data);

                        if (CurrentUser.UserRole == Enums.UserRole.Administrator)
                        {
                            if (exist != null)
                            {
                                if ((exist.VehicleType == null ? string.Empty : exist.VehicleType.ToUpper()) == "BENEFIT")
                                {
                                    dto = exist;
                                    dto.CostCenter = data.CostCenter;
                                    dto.PoNumber = data.PoNumber;
                                    dto.PoLine = data.PoLine;
                                    dto.PoliceNumber = data.PoliceNumber;
                                    dto.CertificateOwnership = data.CertificateOwnership;
                                    dto.Comments = data.Comments;
                                    dto.Assets = data.Assets;
                                    dto.ChasisNumber = data.ChasisNumber;
                                    dto.EngineNumber = data.EngineNumber;
                                }
                                else
                                {
                                    dto.MstFleetId = exist.MstFleetId;
                                }
                            }
                        }
                        else if (CurrentUser.UserRole == Enums.UserRole.Fleet)
                        {
                            if (exist != null)
                            {
                                if ((exist.VehicleType == null ? string.Empty : exist.VehicleType.ToUpper()) == "BENEFIT")
                                {
                                    dto = exist;
                                    dto.CostCenter = data.CostCenter;
                                    dto.PoNumber = data.PoNumber;
                                    dto.PoLine = data.PoLine;
                                    dto.PoliceNumber = data.PoliceNumber;
                                    dto.CertificateOwnership = data.CertificateOwnership;
                                    dto.Comments = data.Comments;
                                    dto.Assets = data.Assets;
                                    dto.MstFleetId = data.MstFleetId;
                                }
                                else
                                {
                                    dto = exist;
                                    dto.CostCenter = data.CostCenter;
                                    dto.PoNumber = data.PoNumber;
                                    dto.PoLine = data.PoLine;
                                    dto.PoliceNumber = data.PoliceNumber;
                                    dto.CertificateOwnership = data.CertificateOwnership;
                                    dto.Comments = data.Comments;
                                    dto.Assets = data.Assets;
                                    dto.EmployeeID = data.EmployeeID;
                                    dto.EmployeeName = data.EmployeeName;
                                    dto.Branding = data.Branding;
                                    dto.Color = data.Color;
                                    dto.VehicleUsage = data.VehicleUsage;
                                    dto.City = data.City;
                                    dto.AssignedTo = data.AssignedTo;
                                    dto.Project = data.Project;
                                    dto.ProjectName = data.ProjectName;
                                    dto.MstFleetId = data.MstFleetId;
                                }
                            }
                        }

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
                var ListEmployee = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).ToList();
                var ListFunction = _groupCostCenterBLL.GetGroupCenter().Where(x => x.IsActive).ToList();
                var listLocation = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive).ToList();

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

                        var GetEmployee = ListEmployee.Where(x => (x.EMPLOYEE_ID == null ? "" : x.EMPLOYEE_ID.ToUpper()) == (item.EmployeeID == null ? "" : item.EmployeeID.ToUpper())).FirstOrDefault();
                        item.EmployeeName = GetEmployee.FORMAL_NAME ;

                        item.CostCenter = dataRow[3];
                        var GetFunction = ListFunction.Where(x => (x.CostCenter == null ? "" : x.CostCenter.ToUpper()) == (item.CostCenter == null ? "" : item.CostCenter.ToUpper())).FirstOrDefault();
                        item.Function = GetFunction.FunctionName;

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
                        item.VehicleYear = Convert.ToInt32(dataRow[15] == "" ? "0" : dataRow[15]);
                        item.VehicleType = dataRow[16];
                        item.VehicleUsage = dataRow[17];
                        item.Project = dataRow[18] == "Yes" ? true : false;
                        item.ProjectS = dataRow[18];
                        item.ProjectName = dataRow[19];
                        if (dataRow[20] != "NULL" && dataRow[20] != "")
                        {
                            double dStartContract = double.Parse(dataRow[20].ToString());
                            DateTime dtStartContract = DateTime.FromOADate(dStartContract);
                            item.StartContract = dtStartContract;
                        }
                        if (dataRow[21] != "NULL" && dataRow[21] != "")
                        {
                            double dEndContract = double.Parse(dataRow[21].ToString());
                            DateTime dtEndContract = DateTime.FromOADate(dEndContract);
                            item.EndContract = dtEndContract;
                        }
                        item.VendorName = dataRow[22];

                        item.City = dataRow[23];
                        var GetLocation = listLocation.Where(x => (x.Basetown == null ? "" : x.Basetown.ToUpper()) == (item.City == null ? "" : item.City.ToUpper())).FirstOrDefault();
                        item.Address = GetLocation.Address == null ? "" : GetLocation.Address.ToUpper();
                        item.Regional = GetLocation.Region == null ? "" : GetLocation.Region.ToUpper();

                        item.SupplyMethod = dataRow[24];
                        item.Restitution = dataRow[25] == "Yes" ? true : false;
                        item.RestitutionS = dataRow[25];
                        item.MonthlyHMSInstallment = Convert.ToInt32(dataRow[26] == "" ? "0" : dataRow[26]);
                        item.VatDecimal = Convert.ToInt64(dataRow[27] == "" ? "0" : dataRow[27]);
                        item.PoNumber = dataRow[28];
                        item.PoLine = dataRow[29];
                        item.CarGroupLevel = Convert.ToInt32(dataRow[30] == "" ? "0" : dataRow[30]);
                        if(dataRow[31] != "NULL" && dataRow[31] != "")
                        {
                            item.GroupLevel = Convert.ToInt32(dataRow[31]);
                        }
                        else
                        {
                            item.GroupLevel = 0;
                        }
                        item.AssignedTo = dataRow[32];
                        if (dataRow[34] != "NULL" && dataRow[34] != "")
                        {
                            double dStartDate = double.Parse(dataRow[34].ToString());
                            DateTime dtStartDate = DateTime.FromOADate(dStartDate);
                            item.StartDate = dtStartDate;
                        }
                        if (dataRow[35] != "NULL" && dataRow[35] != "")
                        {
                            double dEndDate = double.Parse(dataRow[35].ToString());
                            DateTime dtEndDate = DateTime.FromOADate(dEndDate);
                            item.EndDate = dtEndDate;
                        }
                        item.VehicleStatus = dataRow[36];
                        item.CertificateOwnership = dataRow[37];
                        item.Comments = dataRow[38];
                        item.Assets = dataRow[39];
                        string TotalMonthlyCharge = dataRow[40];
                        TotalMonthlyCharge = TotalMonthlyCharge.Trim(',');
                        item.TotalMonthlyCharge = Int64.Parse(String.IsNullOrEmpty(TotalMonthlyCharge) ? "0" : TotalMonthlyCharge);
                        item.ErrorMessage = string.Empty;

                        if (item.EmployeeID != null) { 
                            var existEmp = _employeeBLL.GetByID(dataRow[2]);

                            if (existEmp == null) { item.ErrorMessage += "Data Employee ID Not Exist in master employee,"; }
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
            slDocument.SetCellValue(1, 1, "Master Fleet");
            slDocument.MergeWorksheetCells(1, 1, 1,47);
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
            slDocument.SetCellValue(iRow, 44, "Created By");
            slDocument.SetCellValue(iRow, 45, "Created Date");
            slDocument.SetCellValue(iRow, 46, "Modified By");
            slDocument.SetCellValue(iRow, 47, "Modified Date");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 47, headerStyle);

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
                slDocument.SetCellValue(iRow, 13, data.Airbag == true ? "Yes": "No");
                slDocument.SetCellValue(iRow, 14, data.ChasisNumber);
                slDocument.SetCellValue(iRow, 15, data.EngineNumber);
                slDocument.SetCellValue(iRow, 16, data.VehicleYear);
                slDocument.SetCellValue(iRow, 17, data.VehicleType);
                slDocument.SetCellValue(iRow, 18, data.VehicleUsage);
                slDocument.SetCellValue(iRow, 19, data.Project == true ?"Yes" : "No");
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
                slDocument.SetCellValue(iRow, 44, data.CreatedBy);
                slDocument.SetCellValue(iRow, 45, data.CreatedDate.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 46, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 47, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
          

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 47);
            slDocument.SetCellStyle(3, 1, iRow - 1, 47, valueStyle);

            return slDocument;
        }

        #endregion

    }
}
