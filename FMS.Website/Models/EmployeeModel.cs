using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class EmployeeModel : BaseModel
    {
        public EmployeeModel()
        {
            Details = new List<EmployeeItem>();
            SearchView = new EmployeeSearchView();
        }

        public List<EmployeeItem> Details { get; set; }
        public EmployeeSearchView SearchView { get; set; }


        public int TotalData { get; set; }
        public int TotalDataPerPage { get; set; }

        public int CurrentPage { get; set; }
    }

    public class EmployeeSearchView
    {
        public bool? Status { get; set; }
        public string EmployeeId { get; set; }
        public string FormalName { get; set; }
        public string PositionTitle { get; set; }
        public string Division { get; set; }
        public string Directorate { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string BaseTown { get; set; }
        public string Company { get; set; }
        public string CostCenter { get; set; }
        public string GroupLevel { get; set; }
        public string EmailAddress { get; set; }
        public string FlexPoint { get; set; }

        public SelectList StatusList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem(){ Text = "Active", Value = "True"},
                    new SelectListItem(){ Text = "InActive", Value = "False"}
                };
                return new SelectList(items, "Value", "Text");
            }

        }

        public SelectList PositionTitleList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "WTC", Value = "WTC"},
                    new SelectListItem() { Text = "Benefit", Value = "Benefit"}
                };
                return new SelectList(items, "Value", "Text");
            }
        }
        public SelectList DivisionList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "ASSA", Value = "ASSA"},
                    new SelectListItem() { Text = "TRAC", Value = "TRAC"}
                };
                return new SelectList(items, "Value", "Text");
            }
        }
        public SelectList DirectorateList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "Sales", Value = "Sales"},
                    new SelectListItem() { Text = "Marketing", Value = "Marketing"},
                    new SelectListItem() { Text = "Operations", Value = "Operations"},
                    new SelectListItem() { Text = "IS", Value = "IS"}
                };
                return new SelectList(items, "Value", "Text");
            }
        }
        public SelectList CompanyList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "Jakarta", Value = "Jakarta"},
                    new SelectListItem() { Text = "Sumatra 1", Value = "Sumatra1"},
                    new SelectListItem() { Text = "Sumatra 2", Value = "Sumatra2"}
                };
                return new SelectList(items, "Value", "Text");
            }
        }
        public SelectList CityList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "Surabaya", Value = "Surabaya"},
                    new SelectListItem() { Text = "Malang", Value = "Malang"},
                    new SelectListItem() { Text = "Palembang", Value = "Palembang"}
                };
                return new SelectList(items, "Value", "Text");
            }
        }
        public SelectList GroupLevelList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "1", Value = "1"},
                    new SelectListItem() { Text = "2", Value = "2"},
                    new SelectListItem() { Text = "3", Value = "3"},
                    new SelectListItem() { Text = "4", Value = "4"}
                };
                return new SelectList(items, "Value", "Text");
            }
        }
        public SelectList AddressList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "COP", Value = "COP"},
                    new SelectListItem() { Text = "CFM", Value = "CFM"}
                };
                return new SelectList(items, "Value", "Text");
            }
        }
        public SelectList FlexPointList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "COP", Value = "COP"},
                    new SelectListItem() { Text = "CFM", Value = "CFM"}
                };
                return new SelectList(items, "Value", "Text");
            }
        }

    }

    public class EmployeeItem : BaseModel
    {
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string FORMAL_NAME { get; set; }
        public string POSITION_TITLE { get; set; }
        public string DIVISON { get; set; }
        public string DIRECTORATE { get; set; }
        public string ADDRESS { get; set; }
        public string CITY { get; set; }
        public string BASETOWN { get; set; }
        public string COMPANY { get; set; }
        public string COST_CENTER { get; set; }
        public int GROUP_LEVEL { get; set; }
        public string EMAIL_ADDRESS { get; set; }
        public int FLEX_POINT { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
        public bool IS_ACTIVE { get; set; }

        public SelectList PositionTitleList { get; set; }
        public SelectList DivisonList { get; set; }
        public SelectList DirectorateList { get; set; }
        public SelectList CityList { get; set; }
        public SelectList BaseTownList { get; set; }
        public SelectList CompanyList { get; set; }
        public SelectList GroupLevelList { get; set; }
        public SelectList FlexPointlList { get; set; }
        public SelectList AddressList { get; set; }
    }


    public class EmployeeUploadItem : BaseModel
    {
        public string EMPLOYEE_ID { get; set; }
        public string FORMAL_NAME { get; set; }
        public string POSITION_TITLE { get; set; }
        public string DIVISON { get; set; }
        public string DIRECTORATE { get; set; }
        public string ADDRESS { get; set; }
        public string CITY { get; set; }
        public string BASETOWN { get; set; }
        public string COMPANY { get; set; }
        public string COST_CENTER { get; set; }
        public string GROUP_LEVEL { get; set; }
        public string EMAIL_ADDRESS { get; set; }
        public string FLEX_POINT { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
        public bool IS_ACTIVE { get; set; }
    }
}