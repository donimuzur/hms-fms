using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class VendorDto
    {
        public int MstVendorId { get; set; }
        public string VendorName { get; set; }
        public string ShortName { get; set; }
        public string EmailAddress { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime?  ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
