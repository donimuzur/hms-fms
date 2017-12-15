using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Inputs
{
    public class KpiMonitoringGetByParamInput
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string FormType { get; set; }
        public string VehicleUsage { get; set; }
        public string Location { get; set; }
    }
}
