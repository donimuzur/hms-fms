using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Inputs
{
    public class RptPoByParamInput
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public string EmployeeName { get; set; }
        public string CostCenter { get; set; }
        public string SupplyMethod { get; set; }
        public string PoliceNumber { get; set; }
        public int? GroupLevel { get; set; }
    }
}
