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
        public bool? PenaltyForEmplloyee { get; set; }
        public bool? PenaltyForFleet { get; set; }
        public string CreatedBy { get; set; }
        public string VehicleType{ get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public string MstDocumentType { get; set; }
        public MST_DOCUMENT_TYPE MstDocType { get; set; }

    }
}
