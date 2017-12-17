using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class KpiMonitoringDto
    {
        public int Id { get; set; }
        public long TraId { get; set; }
        public string FormType { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string Reason { get; set; }
        public string Address { get; set; }
        public string PreviousBaseTown { get; set; }
        public string NewBaseTown { get; set; }
        public string VehicleUsage { get; set; }
        public int? VehicleGroup { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public string PoliceNumber { get; set; }
        public string Location { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

        public DateTime? TemporaryRequestDate { get; set; }
        public DateTime? EeReceivedTemp { get; set; }
        public DateTime? SendToEmpDate { get; set; }
        public DateTime? SendBackToHr { get; set; }
        public DateTime? SendToFleetDate { get; set; }
        public DateTime? SendToEmpBenefit { get; set; }
        public DateTime? SendSuratKuasa { get; set; }
        public DateTime? SendAgreement { get; set; }
        public string Remark { get; set; }
        public decimal? Kpi1 { get; set; }
        public decimal? Kpi2 { get; set; }
        public decimal? Kpi3 { get; set; }
        public decimal? Kpi4 { get; set; }
        public decimal? Kpi5 { get; set; }
    }
}
