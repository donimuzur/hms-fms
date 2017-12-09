using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class ExecutiveSummaryModel : BaseModel
    {
        public ExecutiveSummaryModel()
        {
            NoVehicleList = new List<NoVehicleData>();
            SearchViewExport = new VehicleSearchViewExport();
            SearchView = new VehicleSearchView();
            SearchView.MonthFrom = DateTime.Now.Month;
            SearchView.MonthTo = DateTime.Now.Month;
            SearchView.YearFrom = DateTime.Now.Year;
            SearchView.YearTo = DateTime.Now.Year;
        }

        public string TitleForm { get; set; }
        public string TitleExport { get; set; }
        public List<NoVehicleData> NoVehicleList { get; set; }
        public VehicleSearchView SearchView { get; set; }
        public VehicleSearchViewExport SearchViewExport { get; set; }
    }

    public class NoVehicleData
    {
        public int Id { get; set; }
        public string VehicleType { get; set; }
        public string SupplyMethod { get; set; }
        public string Function { get; set; }
        public int? NoOfVehicle { get; set; }
        public int? ReportMonth { get; set; }
        public string Month { get; set; }
        public int? ReportYear { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class VehicleSearchView
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string VehicleType { get; set; }
        public string SupplyMethod { get; set; }
        public string Function { get; set; }

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

        public SelectList VehicleTypeList { get; set; }
        public SelectList SupplyMethodList { get; set; }
    }

    public class VehicleSearchViewExport
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string VehicleType { get; set; }
        public string SupplyMethod { get; set; }
        public string Function { get; set; }
    }
}