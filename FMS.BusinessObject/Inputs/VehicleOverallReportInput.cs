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
    }
}
