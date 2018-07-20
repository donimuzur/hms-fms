using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Inputs
{
    public class PricelistParamInput
    {
        public string vendor { get; set; }
        public string VehicleType { get; set; }
        public string VehicleUsage { get; set; }
        public string ZonePricelist { get; set; }
        public string Table { get; set; }
    }
}
