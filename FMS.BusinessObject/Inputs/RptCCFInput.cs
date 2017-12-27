using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Inputs
{
    public class RptCCFInput
    {
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public int Category { get; set; }
        public string Coordinator { get; set; }
        public string Location { get; set; }
        public int? CoorKPI { get; set; }
        public int? VendorKPI { get; set; }
    }
}
