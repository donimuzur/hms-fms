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

    public class CostObParamInput
    {
        public bool? Status { get; set; }
        public string VehicleType { get; set; }
        public string Function { get; set; }
        public string Regional { get; set; }
        public int? Year { get; set; }
    }
    public class EpafParamInput
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EpafAction { get; set; }
        public string DocumentType { get; set; }
    }
}
