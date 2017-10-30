using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class SysAccessDto
    {
        public int MstSysAccessId { get; set; }
        public string RoleName { get; set; }
        public string RoleNameAlias { get; set; }
        public int ModulId { get; set; }
        public bool ReadAccess { get; set; }
        public bool WriteAccess { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public MST_MODUL MstModul { get; set; }
        public string ModulName { get; set; }
    }
}
