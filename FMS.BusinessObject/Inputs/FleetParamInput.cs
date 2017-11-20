using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Inputs
{
    public class FleetParamInput
    {
        public string EmployeeId { get; set; }
        public string VehicleType { get; set; }
        public string VehicleUsage { get; set; }

        public string VehicleStatus { get; set; }

        public string VehicleCity { get; set; }
    }

    public class FleetSearchInput
    {
        public bool? Status { get; set; }
        public string VehicleType { get; set; }
        public string VehicleUsage { get; set; }
        public string SupplyMethod { get; set; }
        public string BodyType { get; set; }
        public string Vendor { get; set; }
        public string Function { get; set; }
        public DateTime StartRent { get; set; }
        public DateTime EndRent { get; set; }
        public string Regional { get; set; }
        public string City { get; set; }
    }
}