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

        public string PoliceNumber { get; set; }
    }
}