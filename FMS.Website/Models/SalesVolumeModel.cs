using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class SalesVolumeModel : BaseModel
    {
        public SalesVolumeModel()
        {
            Details = new List<SalesVolumeItem>();
            SearchViewExport = new SearchViewExport();
            SearchView = new SearchView();
            SearchView.YearFrom = DateTime.Now.Year;
            SearchView.YearTo = DateTime.Now.Year;
        }

        public List<SalesVolumeItem> Details { get; set; }
        public SearchView SearchView { get; set; }
        public SearchViewExport SearchViewExport { get; set; }
    }
    public class SalesVolumeItem : BaseModel
    {
        public int MstSalesVolumeId { get; set; }
        public string Type { get; set; }
        public string Region { get; set; }
        public int Month { get; set; }
        public string MonthS { get; set; }
        public int Year { get; set; }
        public Decimal Value { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveS { get; set; }
        public string ModulName { get; set; }
        public SelectList ModulList { get; set; }
        public SelectList RoleNameList { get; set; }
    }

    public class SalesVolumeUpload : BaseModel
    {
        public string Type { get; set; }
        public string Region { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Value { get; set; }

    }

    public class SearchView
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string Type { get; set; }
        public string Regional { get; set; }
        public string Table { get; set; }

        public SelectList MonthList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "ALL", Value = "0" },
                    new SelectListItem() {Text = "January", Value = "1" },
                    new SelectListItem() {Text = "February", Value = "2" },
                    new SelectListItem() {Text = "March", Value = "3" },
                    new SelectListItem() {Text = "April", Value = "4" },
                    new SelectListItem() {Text = "May", Value = "5" },
                    new SelectListItem() {Text = "June", Value = "6" },
                    new SelectListItem() {Text = "July", Value = "7" },
                    new SelectListItem() {Text = "August", Value = "8" },
                    new SelectListItem() {Text = "September", Value = "9" },
                    new SelectListItem() {Text = "October", Value = "10" },
                    new SelectListItem() {Text = "November", Value = "11" },
                    new SelectListItem() {Text = "December", Value = "12" }
                };
                return new SelectList(items, "Value", "Text");
            }

        }
        public SelectList TableList { get; set; }
        public SelectList TypeList { get; set; }
        public SelectList RegionalList { get; set; }
    }

    public class SearchViewExport
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string Type { get; set; }
        public string Regional { get; set; }
        public string Table { get; set; }
    }
}