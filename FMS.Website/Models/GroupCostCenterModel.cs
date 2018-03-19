using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class GroupCostCenterModel: BaseModel
    {
        public List<GroupCostCenterItem> Details { get; set; }
        public GroupCostCenterModel()
        {
            Details = new List<GroupCostCenterItem>();
            SearchView = new GroupCostCenterSearchView();
        }
        public GroupCostCenterSearchView SearchView { get; set; }
    }

    public class GroupCostCenterSearchView
    {
        public string Function { get; set; }
        public string CostCenter { get; set; }

        public SelectList FunctionList { get; set; }
        public SelectList CostCenterList { get; set; }
    }

    public class GroupCostCenterItem : BaseModel
    {
        public int MstFunctionGroupId { get; set; }
        public string FunctionName { get; set; }
        public string CostCenter { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}