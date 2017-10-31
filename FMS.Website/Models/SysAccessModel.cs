using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class SysAccessModel : BaseModel
    {
        public List<SysAccessItem> Details { get; set; }
        public SysAccessModel()
        {
            Details = new List<SysAccessItem>();
        }
    }

    public class SysAccessItem : BaseModel
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
        public string ModulName { get; set; }
        public SelectList ModulList { get; set; }
        public SelectList RoleNameList { get;set;}
    }
}