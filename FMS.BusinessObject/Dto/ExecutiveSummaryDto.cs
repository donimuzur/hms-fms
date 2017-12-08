using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class ExecutiveSummaryDto
    {
        public string VehicleFunction { get; set; }
        public decimal? VehicleCost { get; set; }
    }

    public class NoVehicleDto
    {
        public string VEHICLE_TYPE { get; set; }
        public string SUPPLY_METHOD { get; set; }
        public string FUNCTION { get; set; }
        public int? NO_OF_VEHICLE { get; set; }
        public int? REPORT_MONTH { get; set; }
        public int? REPORT_YEAR { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }
}
