using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Inputs
{
    public class EmployeeParamInput
    {
        public bool? Status { get; set; }
        public string EmployeeId { get; set; }
        public string FormalName { get; set; }
        public string PositionTitle { get; set; }
        public string Division { get; set; }
        public string Directorate { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string BaseTown { get; set; }
        public string Company { get; set; }
        public string CostCenter { get; set; }
        public string GroupLevel { get; set; }
        public string EmailAddress { get; set; }
        public string FlexPoint { get; set; }
    }
}
