using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class ComplaintDto
    {
        public int MstComplaintCategoryId { get; set; }
        public string CategoryName { get; set; }
        public string RoleType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
