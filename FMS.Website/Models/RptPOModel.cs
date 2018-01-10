using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class RptPOModel : BaseModel
    {
        public RptPOModel()
        {
            RptPOItem = new List<RptPOItem>();
            SearchViewExport = new RptPOSearchViewExport();
            SearchView = new RptPOSearchView();
            //SearchView.PeriodFrom = DateTime.Today;
            //SearchView.PeriodTo = DateTime.Today;
            SearchView.MonthFrom = DateTime.Now.Month;
            SearchView.MonthFrom = SearchView.MonthFrom;
            SearchView.YearFrom = DateTime.Now.Year;
            SearchView.MonthTo = DateTime.Now.Month;
            SearchView.YearTo = DateTime.Now.Year;
            SearchView.SupplyMethod = "TEMPORARY";
            //SearchView.GroupLevel = 1;
        }
        public List<RptPOItem> RptPOItem { get; set; }
        public RptPOSearchView SearchView { get; set; }
        public RptPOSearchViewExport SearchViewExport { get; set; }
        public string TitleForm { get; set; }
        public string TitleExport { get; set; }

        //public int startMonth { get; set;}
        //public int startYear { get; set; }
        //public int toMonth { get; set; }
        //public int toYear { get; set; }
    }

    public class RptPOItem
    {

        public int ID { get; set; }
        public String PoliceNumber { get; set; }
        public String SupplyMethod { get; set; }
        public String EmployeeName { get; set; }
        public String CostCenter { get; set; }
        public String Manufacturer { get; set; }
        public String Models { get; set; }
        public String Series { get; set; }
        public String BodyType { get; set; }
        public String Color { get; set; }
        public String ChasisNumber { get; set; }
        public String EngineNumber { get; set; }
        public String VehicleType { get; set; }
        public String VehicleUsage { get; set; }
        public String PoNumber { get; set; }
        public String PoLine { get; set; }
        public int ReportMonth { get; set; }
        public int ReportYear { get; set; }
        public String CreatedDate { get; set; }
        public DateTime StartContract { get; set; }
        public DateTime EndContract { get; set; }
        public String Vendor { get; set; }
        public Decimal MonthlyInstallment { get; set; }
        public Decimal Gst { get; set; }
        public Decimal TotMonthInstallment { get; set; }
        public long MstFleetId { get; set; }
        public string GroupLevel { get; set; }
        public String VehicleFunction { get; set; }

        public Decimal JanAmount { get; set; }
        public Decimal JanPPN { get; set; }
        public Decimal JanTotal { get; set; }

        public Decimal PebAmount { get; set; }
        public Decimal PebPPN { get; set; }
        public Decimal PebTotal { get; set; }

        public Decimal MarAmount { get; set; }
        public Decimal MarPPN { get; set; }
        public Decimal MarTotal { get; set; }

        public Decimal AprAmount { get; set; }
        public Decimal AprPPN { get; set; }
        public Decimal AprTotal { get; set; }

        public Decimal MeiAmount { get; set; }
        public Decimal MeiPPN { get; set; }
        public Decimal MeiTotal { get; set; }

        public Decimal JunAmount { get; set; }
        public Decimal JunPPN { get; set; }
        public Decimal JunTotal { get; set; }

        public Decimal JulAmount { get; set; }
        public Decimal JulPPN { get; set; }
        public Decimal JulTotal { get; set; }

        public Decimal AgusAmount { get; set; }
        public Decimal AgusPPN { get; set; }
        public Decimal AgusTotal { get; set; }

        public Decimal SepAmount { get; set; }
        public Decimal SepPPN { get; set; }
        public Decimal SepTotal { get; set; }

        public Decimal OktAmount { get; set; }
        public Decimal OktPPN { get; set; }
        public Decimal OktTotal { get; set; }

        public Decimal NopAmount { get; set; }
        public Decimal NopPPN { get; set; }
        public Decimal NopTotal { get; set; }

        public Decimal DesAmount { get; set; }
        public Decimal DesPPN { get; set; }
        public Decimal DesTotal { get; set; }
    }

    public class RptPOSearchView
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public DateTime? PeriodFrom { get; set; }
        public DateTime? PeriodTo { get; set; }
        public string EmployeeName { get; set; }
        public string CostCenter { get; set; }
        public string SupplyMethod { get; set; }
        public string PoliceNumber { get; set; }
        public int? GroupLevel { get; set; }
        public SelectList SupplyMethodList { get; set; }
        public SelectList EmployeeNameList { get; set; }
        public SelectList CostCenterList { get; set; }

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
    }

    public class RptPOSearchViewExport
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public DateTime? PeriodFrom { get; set; }
        public DateTime? PeriodTo { get; set; }
        public string EmployeeName { get; set; }
        public string CostCenter { get; set; }
        public string SupplyMethod { get; set; }
        public string PoliceNumber { get; set; }
        public int GroupLevel { get; set; }
    }
}