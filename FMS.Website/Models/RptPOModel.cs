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
            SearchView.PeriodFrom = DateTime.Today;
            SearchView.PeriodTo = DateTime.Today;
        }
        public List<RptPOItem> RptPOItem { get; set; }
        public RptPOSearchView SearchView { get; set; }
        public RptPOSearchViewExport SearchViewExport { get; set; }
        public string TitleForm { get; set; }
        public string TitleExport { get; set; }

        public int startMonth { get; set;}
        public int startYear { get; set; }
        public int toMonth { get; set; }
        public int toYear { get; set; }
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
    }

    public class RptPOSearchView
    {
        public DateTime? PeriodFrom { get; set; }
        public DateTime? PeriodTo { get; set; }
        public string EmployeeName { get; set; }
        public string CostCenter { get; set; }
        public string SupplyMethod { get; set; }
        public SelectList SupplyMethodList { get; set; }
        public SelectList EmployeeNameList { get; set; }
        public SelectList CostCenterList { get; set; }
    }

    public class RptPOSearchViewExport
    {
        public DateTime? PeriodFrom { get; set; }
        public DateTime? PeriodTo { get; set; }
        public string EmployeeName { get; set; }
        public string CostCenter { get; set; }
        public string SupplyMethod { get; set; }
    }
}