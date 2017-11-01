using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class CostObDto
    {
        public int MstCostObId { get; set; }
        public int Year { get; set; }
        public string Zone { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public decimal ObCost { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
