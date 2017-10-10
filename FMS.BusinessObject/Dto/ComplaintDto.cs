using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class ComplaintDto
    {
        public int CompCatId { get; set; }

        public int RoleId { get; set; }

        public string ComplaintCategory { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }

        public RoleDto Role { get; set; }
    }
}
