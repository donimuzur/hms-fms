using FMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Inputs
{
    public class CtfWorkflowDocumentInput
    {
        public long DocumentId { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public string Comment { get; set; }
        public Enums.ActionType ActionType { get; set; }
        public string DocumentNumber { get; set; }
    }
}
