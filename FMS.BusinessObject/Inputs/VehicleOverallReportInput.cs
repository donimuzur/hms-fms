using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Inputs
{
    public class VehicleOverallReportGetByParamInput
    {
        public bool? VehicleStatus { get; set; }
        public string SupplyMethod { get; set; }
        public string BodyType { get; set; }
        public string VehicleType { get; set; }
        public string Vendor { get; set; }
        public string Function { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Regional { get; set; }
        public string City { get; set; }
        public string PoliceNumber { get; set; }
        public string EmployeeName { get; set; }
        public string EngineNumber { get; set; }
        public string EmployeeID { get; set; }
        public string ChasisNumber { get; set; }
        public string VehicleUsage { get; set; }
        public DateTime? EndContract_FromDate { get; set; }
        public DateTime? EndContract_ToDate { get; set; }
        public DateTime? EndDate_FromDate { get; set; }
        public DateTime? EndDate_ToDate { get; set; }
    }
}
