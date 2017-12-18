using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class RptFuelModel : BaseModel
    {
        public RptFuelModel()
        {
            RptFuelItem = new List<RptFuelItem>();
            SearchViewExport = new RptFuelSearchViewExport();
            SearchView = new RptFuelSearchView();
            SearchView.MonthFrom = DateTime.Now.Month;
            SearchView.YearFrom = DateTime.Now.Year;
            SearchView.MonthTo = DateTime.Now.Month;
            SearchView.YearTo = DateTime.Now.Year;
        }
        public List<RptFuelItem> RptFuelItem { get; set; }
        public RptFuelSearchView SearchView { get; set; }
        public RptFuelSearchViewExport SearchViewExport { get; set; }
        public string TitleForm { get; set; }
        public string TitleExport { get; set; }
    }

    public class RptFuelItem
    {
        public int Id { get; set; }
        public string PoliceNumber { get; set; }
        public int Liter { get; set; }
        public Decimal Odometer { get; set; }
        public Decimal Usage { get; set; }
        public Decimal kmlt { get; set; }
        public Decimal Cost { get; set; }
        public string FuelType { get; set; }
        public string CostCenter { get; set; }
        public string Function { get; set; }
        public string Manufacturer { get; set; }
        public string Models { get; set; }
        public string Series { get; set; }
        public string BodyType { get; set; }
        public string VehicleType { get; set; }
        public string VehicleUsage { get; set; }
        public string Location { get; set; }
        public string Regional { get; set; }
        public int ReportMonth { get; set; }
        public int ReportYear { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class RptFuelSearchView
    {
        public int MonthFrom { get; set; }
        public int YearFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearTo{ get; set; }
        public string VehicleType { get; set; }
        public string CostCenter { get; set; }
        public string Function { get; set; }
        public string Regional { get; set; }
        public string PoliceNumber { get; set; }

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
        public SelectList CostCenterList { get; set; }
        public SelectList FunctionList { get; set; }
        public SelectList RegionalList { get; set; }
    }

    public class RptFuelSearchViewExport
    {
        public int MonthFrom { get; set; }
        public int YearFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearTo { get; set; }
        public string VehicleType { get; set; }
        public string CostCenter { get; set; }
        public string Function { get; set; }
        public string Regional { get; set; }
        public string PoliceNumber { get; set; }
    }
}