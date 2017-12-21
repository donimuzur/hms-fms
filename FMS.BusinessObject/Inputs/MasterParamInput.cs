using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Inputs
{
    public class SalesVolumeParamInput
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string Type { get; set; }
        public string Regional { get; set; }
    }
}
