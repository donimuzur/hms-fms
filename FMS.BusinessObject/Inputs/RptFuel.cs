using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Inputs
{
    public class RptFuelByParamInput
    {
        public int MonthFrom { get; set; }
        public int YearFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearTo { get; set; }
        public string VehicleType { get; set; }
        public string CostCenter { get; set; }
        public string Function { get; set; }
        public string Regional { get; set; }
        public string PoliceNumber { get; set; }
    }
}
