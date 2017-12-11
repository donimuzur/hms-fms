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
            Details = new List<RptFuelItem>();
        }

        public List<RptFuelItem> Details { get; set; }
    }

    public class RptFuelItem
    {
        public int Id { get; set; }
        public string PoliceNumber { get; set; }
        public int Liter { get; set; }
        public Decimal Odometer { get; set; }
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
        public int ReportMonth { get; set; }
        public int ReportYear { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}