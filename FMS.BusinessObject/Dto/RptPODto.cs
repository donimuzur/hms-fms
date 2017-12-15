using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class RptPODto
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
        public String StartContract { get; set; }
        public String EndContract { get; set; }
        public String Vendor { get; set; }
        public Decimal MonthlyInstallment { get; set; }
        public Decimal Gst { get; set; }
        public Decimal TotMonthInstallment { get; set; }
        public long MstFleetId { get; set; }
    }
}
