using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class DelegationDto
    {
        public int MstDelegationID { get; set; }
        public String EmployeeFrom { get; set; }
        public String EmployeeTo { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool IsComplaintFrom { get; set; }
        public String Attachment { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public string NameEmployeeFrom { get; set; }
        public string NameEmployeeTo { get; set; }

        public string LoginFrom { get; set; }

        public string LoginTo { get; set; }
    }
}
