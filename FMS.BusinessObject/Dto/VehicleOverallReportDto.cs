using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class VehicleOverallReportDto
    {
        public int Id { get; set; }
        public string PoliceNumber { get; set; }
        public string Manufacture { get; set; }
        public string Models { get; set; }
        public string Series { get; set; }
        public string BodyType { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string VehicleType { get; set; }
        public string CostCenter { get; set; }
        public DateTime? StartContract { get; set; }
        public DateTime? EndContract { get; set; }
        public string SupplyMethod { get; set; }
        public string Vendor { get; set; }
        public string Function { get; set; }
        public string Regional { get; set; }
        public string City { get; set; }
        public string Transmission { get; set; }
        public string FuelType { get; set; }
        public string Branding { get; set; }
        public string Colour { get; set; }
        public bool Airbag { get; set; }
        public bool Abs { get; set; }
        public string ChasisNumber { get; set; }
        public string EngineNumber { get; set; }
        public bool VehicleStatus { get; set; }
        public string AssetsNumber { get; set; }
        public DateTime? TerminationDate { get; set; }
        public bool Restitution { get; set; }
        public decimal? MonthlyInstallment { get; set; }
        public decimal? Vat { get; set; }
        public decimal? TotalMonthlyInstallment { get; set; }
        public string PoNumber { get; set; }
        public string PoLine { get; set; }
        public int ReportMonth { get; set; }
        public int ReportYear { get; set; }
        public DateTime CreatedDate { get; set; }
        public long MstFleetId { get; set; }
        public int? carGrouplevel { get; set; }
        public int? VehicleYear { get; set; }
        public string VehicleUsage { get; set; }
        public bool Project { get; set; }
        public string ProjectName { get; set; }
        public int? EmployeeGroupLevel { get; set; }
        public string AssignedTo { get; set; }
        public string Address { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CertificateOfOwnership { get; set; }
        public string Comments { get; set; }
    }
}
