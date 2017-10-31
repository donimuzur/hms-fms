using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class RemarkDto
    {
        public int MstRemarkId { get; set; }
        public int DocumentType { get; set; }
        public string Remark { get; set; }
        public string RoleType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime createdDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set;} 
        public bool IsActive { get; set; }
        public string MstDocumentType { get; set; }
        public MST_DOCUMENT_TYPE MstDocType { get; set; }
    }
}
