using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        [Required]
        public int? Year { get; set; }
        [Required]
        public string Zone { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public decimal? ObCost { get; set; }
        public string ObCostS { get; set; }
        [Required]
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public SelectList ZoneList { get; set; }
        public SelectList RemarkList { get; set; }
        public SelectList ModelList { get; set; }
        public bool IsActive { get; set; }

    }

}