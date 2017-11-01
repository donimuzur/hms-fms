using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace FMS.Website.Models
{
    public class CostObModel : BaseModel
    {
        public CostObModel()
        {
            Details = new List<CostObItem>();
        }

        public List<CostObItem> Details { get; set; }
    }

    public class CostObItem : BaseModel
    {
        public int MstCostObId { get; set; }
        public int Year { get; set; }
        public string Zone { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public decimal ObCost { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }

    }

}