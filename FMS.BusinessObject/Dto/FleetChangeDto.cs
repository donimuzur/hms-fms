using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class FleetChangeDto
    {
        public long FleetChangeId { get; set; }
        public long FleetId { get; set; }
        public string PoliceNumber { get; set; }
        public string ChasisNumber { get; set; }
        public string EmployeeId{get;set;}
        public string EmployeeName { get; set; }
        public string FieldName { get; set; }
        public string DataBefore { get; set; }
        public string DataAfter { get; set;}
        public string ModifiedBy { get; set; }
        public DateTime? ChangeDate { get; set; }
        public DateTime? DateSend { get; set; }
        public DateTime? DateUpdate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
