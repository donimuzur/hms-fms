using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class RemarkModel : BaseModel
    {
        public RemarkModel()
        {
            Details = new List<RemarkItem>();
        }

        public List<RemarkItem> Details { get; set; }
    }

    public class RemarkItem : BaseModel
    {
        public int MstRemarkId { get; set; }
        public int DocumentType { get; set; }
        public string Remark { get; set; }
        public string RoleType { get; set; }
        public string VehicleType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public string MstDocumentType { get; set; }
        public SelectList DocumentTypeList { get; set; }
        public SelectList RoleTypeList { get; set; }
        public SelectList VehicleTypeList { get; set; }

    }

}