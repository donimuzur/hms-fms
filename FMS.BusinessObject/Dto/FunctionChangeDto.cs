using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class FunctionChangeDto
    {
        public string EmployeeId { get; set; }
        public string FormalName { get; set; }
        public string CostCenter { get; set; }
        public string FunctionOld { get; set; }
        public string FunctionNew { get; set; }
        public DateTime? ChangeDate { get; set; }
        public DateTime? DateSend { get; set; }
        public long FunctionChangeId { get; set; }
    }
}
