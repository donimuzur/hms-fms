using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Inputs
{
    public class RptPoByParamInput
    {
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public string EmployeeName { get; set; }
        public string CostCenter { get; set; }
        public string SupplyMethod { get; set; }
    }
}
