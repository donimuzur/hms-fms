using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class RptFuelDto
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
        public string Regional { get; set; }
        public int ReportMonth { get; set; }
        public int ReportYear { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
