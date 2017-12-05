using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class ReasonModel : BaseModel
    {
        public List<ReasonItem> Details { get; set; }
        public ReasonModel()
        {
            Details = new List<ReasonItem>();
        }

    }

    public class ReasonItem : BaseModel
    {
        public int MstReasonId { get; set; }
        public int DocumentType { get; set; }
        public string VehicleType { get; set; }
        public string Reason { get; set; }
        public bool IsPenalty { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool PenaltyForEmplloyee { get; set; }
        public bool PenaltyForFleet { get; set; }
        public bool IsActive { get; set; }
        public string MstDocumentType { get; set; }
        public SelectList DocumentTypeList { get; set; }
        public SelectList VehicleTypeList { get; set; }
    }

}