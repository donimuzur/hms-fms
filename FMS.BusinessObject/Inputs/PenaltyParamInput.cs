using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Inputs
{
    public class PenaltyParamInput
    {
        public string Vendor { get; set; }
        public string RequestYear { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string Series { get; set; }
        public string BodyType { get; set; }
        public string VehicleType { get; set; }
        public string Table { get; set; }
    }
}
