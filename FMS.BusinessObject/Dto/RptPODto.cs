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
        public DateTime StartContract { get; set; }
        public DateTime EndContract { get; set; }
        public String Vendor { get; set; }
        public Decimal MonthlyInstallment { get; set; }
        public Decimal Gst { get; set; }
        public Decimal TotMonthInstallment { get; set; }
        public long MstFleetId { get; set; }


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
}
