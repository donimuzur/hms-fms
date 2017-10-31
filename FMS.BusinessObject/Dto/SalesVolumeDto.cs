using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class SalesVolumeDto
    {
        public int MstSalesVolumeId { get; set; }
        public string Type { get; set; }
        public string Region { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public Decimal Value { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
