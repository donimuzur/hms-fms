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
        }
        public List<RptPOItem> RptPOItem { get; set; }
        public RptPOSearchView SearchView { get; set; }
        public RptPOSearchViewExport SearchViewExport { get; set; }
        public string TitleForm { get; set; }
        public string TitleExport { get; set; }
    }

    public class RptPOItem
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

    public class RptPOSearchView
    {
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public string EmployeeName { get; set; }
        public string CostCenter { get; set; }
        public string SupplyMethod { get; set; }
        public SelectList SupplyMethodList { get; set; }
    }

    public class RptPOSearchViewExport
    {
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public string EmployeeName { get; set; }
        public string CostCenter { get; set; }
        public string SupplyMethod { get; set; }
    }
}