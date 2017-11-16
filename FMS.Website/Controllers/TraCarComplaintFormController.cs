using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FMS.Website.Models;
using FMS.Contract.BLL;
using FMS.Core;
using AutoMapper;
using FMS.BusinessObject.Dto;
using System.Web;
using System.IO;
using ExcelDataReader;
using System.Data;
using FMS.Website.Utility;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;

namespace FMS.Website.Controllers
{
    public class TraCarComplaintFormController : BaseController
    {
        private ICarComplaintFormBLL _CFFBLL;
        private IEmployeeBLL _employeeBLL;
        private IDelegationBLL _delegationBLL;
        private IComplaintCategoryBLL _complaintcategoryBLL;
        private ISettingBLL _settingBLL;
        private IFleetBLL _fleetBLL;
        private Enums.MenuList _mainMenu;

        public TraCarComplaintFormController(IPageBLL pageBll, IEmployeeBLL employeeBLL, IDelegationBLL delegationBLL, ICarComplaintFormBLL CFFBLL, IComplaintCategoryBLL ComplaintCategoryBLL, ISettingBLL SettingBLL, IFleetBLL FleetBLL) : base(pageBll, Enums.MenuList.TraCcf)
        {
            _CFFBLL = CFFBLL;
            _employeeBLL = employeeBLL;
            _delegationBLL = delegationBLL;
            _complaintcategoryBLL = ComplaintCategoryBLL;
            _settingBLL = SettingBLL;
            _fleetBLL = FleetBLL;
            _mainMenu = Enums.MenuList.Transaction;
        }

        //
        // GET: /TraCarComplaintForm/

        public ActionResult Index()
        {
            var data = _CFFBLL.GetCCF();
            var model = new CarComplaintFormModel();
            try
            {
                model.Details = Mapper.Map<List<CarComplaintFormItem>>(data);
                model.MainMenu = _mainMenu;
                model.CurrentLogin = CurrentUser;
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
            }
            return View(model);
        }

        public CarComplaintFormItem listdata(CarComplaintFormItem model, string IdEmployee)
        {
            var listemployeefromdelegation = _delegationBLL.GetDelegation().Select(x => new { dataemployeefrom = x.EmployeeFrom + x.NameEmployeeFrom, x.EmployeeFrom,x.NameEmployeeFrom, x.EmployeeTo,x.NameEmployeeTo, x.DateTo}).ToList().Where(x => x.EmployeeTo == CurrentUser.EMPLOYEE_ID && x.DateTo >= DateTime.Today).OrderBy(x => x.EmployeeFrom);
            model.EmployeeFromDelegationList = new SelectList(listemployeefromdelegation, "EmployeeFrom", "dataemployeefrom");

            var listcomplaintcategory = _complaintcategoryBLL.GetComplaints().Select(x => new { x.MstComplaintCategoryId, x.CategoryName}).ToList().OrderBy(x => x.MstComplaintCategoryId);
            model.ComplaintCategoryList = new SelectList(listcomplaintcategory, "MstComplaintCategoryId", "CategoryName");

            var listsettingvtype = _settingBLL.GetSetting().Select(x => new { x.SettingGroup, x.SettingName,x.SettingValue }).ToList().Where(x => x.SettingGroup == "VEHICLE_TYPE").OrderBy(x => x.SettingValue);
            model.SettingListVType = new SelectList(listsettingvtype, "SettingValue", "SettingName");

            var listsettingvusage = _settingBLL.GetSetting().Select(x => new { x.SettingGroup, x.SettingName, x.SettingValue }).ToList().Where(x => x.SettingGroup == "VEHICLE_USAGE").OrderBy(x => x.SettingValue);
            model.SettingListVUsage = new SelectList(listsettingvusage, "SettingValue", "SettingName");

            var listsettingfleet = _fleetBLL.GetFleet().Select(x => new {x.MstFleetId,x.VehicleType,x.VehicleUsage,x.PoliceNumber,x.EmployeeID,x.EmployeeName,x.Manufacturer,x.Models,x.Series,x.VendorName,x.StartContract,x.EndContract}).ToList().Where(x => x.EmployeeID == IdEmployee).OrderBy(x => x.EmployeeName);
            model.SettingListFleet = new SelectList(listsettingfleet, "PoliceNumber", "PoliceNumber");

            return model;
        }

        public ActionResult GetData(string id)
        {
            var model = new CarComplaintFormItem();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;

            var data = _employeeBLL.GetByID(id);
            model.EmployeeID = data.EMPLOYEE_ID;
            model.EmployeeName = data.FORMAL_NAME;
            model.EmployeeAddress = data.ADDRESS;
            model.EmployeeCity = data.CITY;
            model = listdata(model, id);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataFleet(string id)
        {
            var model = new CarComplaintFormItem();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;

            var DataFletByEmployee = _fleetBLL.GetFleet().Where(x => x.PoliceNumber == id).FirstOrDefault();
            model.VehicleType = DataFletByEmployee.VehicleType;
            model.VehicleUsage = DataFletByEmployee.VehicleUsage;
            model.Manufacturer = DataFletByEmployee.Manufacturer;
            model.Models = DataFletByEmployee.Models;
            model.Series = DataFletByEmployee.Series;
            model.Vendor = DataFletByEmployee.VendorName;
            model.VStartPeriod = DataFletByEmployee.StartContract.Value.ToString("dd-MMM-yyyy");
            model.VEndPeriod = DataFletByEmployee.EndContract.Value.ToString("dd-MMM-yyyy");
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            var model = new CarComplaintFormItem();
            model.MainMenu = _mainMenu;

            try
            {
                model.CurrentLogin = CurrentUser;
                model.EmployeeID = CurrentUser.EMPLOYEE_ID;
                model.EmployeeIdComplaintFor = CurrentUser.EMPLOYEE_ID;
                model.CreatedDate = DateTime.Today;

                var data = _employeeBLL.GetByID(CurrentUser.EMPLOYEE_ID);
                model.EmployeeName = data.FORMAL_NAME;
                model.EmployeeNameComplaintFor = data.FORMAL_NAME;
                model.LocationAddress = data.ADDRESS;
                model.LocationCity = data.CITY;
                model.VCreatedDate = null;
                model = listdata(model, model.EmployeeID);

                model.DocumentStatus = Enums.DocumentStatus.Draft.GetHashCode();
                model.DocumentStatusDoc = Enums.DocumentStatus.Draft.ToString();
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(CarComplaintFormItem model)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<CarComplaintFormDto>(model);
                //data.TraCcfId = Convert.ToInt32(model.TraCcfId);
                data.CreatedBy = CurrentUser.USERNAME;
                data.CreatedDate = DateTime.Today;
                data.ModifiedDate = null;
                _CFFBLL.Save(data);
            }
            return RedirectToAction("Index", "TraCarComplaintForm");
        }

        public ActionResult Edit(int TraCcfId)
        {
            var data = _CFFBLL.GetCCFByID(TraCcfId);
            var model = new CarComplaintFormItem();
            model = Mapper.Map<CarComplaintFormItem>(data);
            model = listdata(model, model.EmployeeID);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.DocumentStatus = Enums.DocumentStatus.Draft.GetHashCode();
            model.DocumentStatusDoc = Enums.DocumentStatus.Draft.ToString();
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CarComplaintFormItem model)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<CarComplaintFormDto>(model);
                data.ModifiedDate = DateTime.Now;
                data.ModifiedBy = CurrentUser.USERNAME;

                _CFFBLL.Save(data);
            }
            return RedirectToAction("Index", "TraCarComplaintForm");
        }


    }
}
