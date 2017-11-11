using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class PenaltyLogicDto
    {
        public int MstPenaltyLogicId { get; set; }
        public int? MstVendorId { get; set; }
        public int Year { get; set; }
        public string PenaltyLogic { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string VendorName { get; set; }
        public bool IsActive { get; set; }
        public string VehicleType { get; set; }
        public MST_VENDOR MstVendor { get; set; }
    }
}
