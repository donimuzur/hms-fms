using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Inputs
{
    public class FuelOdometerParamInput
    {
        public string VehicleType { get; set; }
        public string PoliceNumber { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EcsRmbTransId { get; set; }
        public string SeqNumber { get; set; }
        public string ClaimType { get; set; }
        public string DateOfCost { get; set; }
        public string CostCenter { get; set; }
        public string LastKM { get; set; }
        public string ClaimComment { get; set; }
        public string PostedTime { get; set; }
        public string Status { get; set; }
        public string Table { get; set; }
    }
}
