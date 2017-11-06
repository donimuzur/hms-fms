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
        private Enums.MenuList _mainMenu;

        public TraCarComplaintFormController(IPageBLL pageBll, IEmployeeBLL employeeBLL, IDelegationBLL delegationBLL, ICarComplaintFormBLL CFFBLL, IComplaintCategoryBLL ComplaintCategoryBLL, ISettingBLL SettingBLL) : base(pageBll, Enums.MenuList.TraCcf)
        {
            _CFFBLL = CFFBLL;
            _employeeBLL = employeeBLL;
            _delegationBLL = delegationBLL;
            _complaintcategoryBLL = ComplaintCategoryBLL;
            _settingBLL = SettingBLL;
            _mainMenu = Enums.MenuList.Transaction;
        }

        //
        // GET: /TraCarComplaintForm/

        public ActionResult Index()
        {
            var data = _CFFBLL.GetCCF();

            var model = new CarComplaintFormModel();
            model.Details = Mapper.Map<List<CarComplaintFormItem>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        public CarComplaintFormItem listdata(CarComplaintFormItem model)
        {
            var listemployeefromdelegation = _delegationBLL.GetDelegation().Select(x => new { x.EmployeeFrom,x.NameEmployeeFrom, x.EmployeeTo,x.NameEmployeeTo}).ToList().Where(x => x.EmployeeTo == CurrentUser.EMPLOYEE_ID).OrderBy(x => x.EmployeeFrom);
            model.EmployeeFromDelegationList = new SelectList(listemployeefromdelegation, "EmployeeFrom", "NameEmployeeFrom");

            var listcomplaintcategory = _complaintcategoryBLL.GetComplaints().Select(x => new { x.MstComplaintCategoryId, x.CategoryName}).ToList().OrderBy(x => x.MstComplaintCategoryId);
            model.ComplaintCategoryList = new SelectList(listcomplaintcategory, "MstComplaintCategoryId", "CategoryName");

            var listsettingvtype = _settingBLL.GetSetting().Select(x => new { x.SettingGroup, x.SettingName,x.SettingValue }).ToList().Where(x => x.SettingGroup == "VEHICLE_TYPE").OrderBy(x => x.SettingValue);
            model.SettingListVType = new SelectList(listsettingvtype, "SettingValue", "SettingName");

            var listsettingvusage = _settingBLL.GetSetting().Select(x => new { x.SettingGroup, x.SettingName, x.SettingValue }).ToList().Where(x => x.SettingGroup == "VEHICLE_USAGE").OrderBy(x => x.SettingValue);
            model.SettingListVUsage = new SelectList(listsettingvusage, "SettingValue", "SettingName");

            return model;
        }

        public ActionResult Create()
        {
            var model = new CarComplaintFormItem();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.EmployeeID = CurrentUser.EMPLOYEE_ID;
            model.EmployeeIdComplaintFor = CurrentUser.EMPLOYEE_ID;

            var data = _employeeBLL.GetByID(CurrentUser.EMPLOYEE_ID);
            model.EmployeeName = data.FORMAL_NAME;
            model.LocationAddress = data.ADDRESS;
            model.LocationCity = data.CITY;

            model = listdata(model);
            return View(model);
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
            model = listdata(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
