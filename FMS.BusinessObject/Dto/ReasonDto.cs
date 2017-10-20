using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class ReasonDto
    {
        public int MstReasonId { get; set; }
        public int DocumentType {get;set;}
        public string Reason { get; set; }
        public bool IsPenalty { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActie { get; set; }

    }
}
