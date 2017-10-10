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


    public class ComplaintCategoryItem
    {
        public int ComplaintId { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }

        public string ComplaintCategory { get; set; }
    }
}
