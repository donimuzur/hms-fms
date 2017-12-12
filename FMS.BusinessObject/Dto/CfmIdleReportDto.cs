using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class CfmIdleReportDto
    {
        public int CfmIdleId { get; set; }
        public string PoliceNumber { get; set; }
        public string Manufacture { get; set; }
        public string Models { get; set; }
        public string Series { get; set; }
        public string BodyType { get; set; }
        public string Color { get; set; }
        public int? GroupLevel { get; set; }
        public DateTime? StartContract { get; set; }
        public DateTime? EndContract { get; set; }
        public string SupplyMethod { get; set; }
        public string Vendor { get; set; }
        public string CostCenter { get; set; }
        public string Transmission { get; set; }
        public string FuelType { get; set; }
        public DateTime? StartIdle { get; set; }
        public DateTime? EndIdle { get; set; }
        public decimal? MonthlyInstallment { get; set; }
        public int? ReportMonth { get; set; }
        public int? ReportYear { get; set; }
        public DateTime CreatedDate{get;set;}
    }
}
