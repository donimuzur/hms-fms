using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class ComplaintCategoryModel : BaseModel
    {
        public ComplaintCategoryModel()
        {
            Details = new List<ComplaintCategoryItem>();
        }

        public List<ComplaintCategoryItem> Details { get; set; }
    }


    public class ComplaintCategoryItem : BaseModel
    {
        public int MstComplaintCategoryId { get; set; }
        public string CategoryName { get; set; }
        public string RoleType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public SelectList RoleTypeList { get; set; }
    }
}
