using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class CtfExtendDto
    {
        public long CtfExtendId { get; set; }
        public long? TraCtfId { get; set; }
        public string ExtendPoliceNumber { get; set; }
        public DateTime? NewProposedDate { get; set; }
        public string ExtendPoNumber { get; set; }
        public string ExtedPoLine { get; set; }
        public decimal? ExtendPrice { get; set; }
        public string ExtendPriceStr { get; set; }
        public int? Reason { get; set; }
        public MST_REASON MstReason { get; set; }
    }
}
