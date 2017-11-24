using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class SalesVolumeModel : BaseModel
    {
        public SalesVolumeModel()
        {
            Details = new List<SalesVolumeItem>();
        }

        public List<SalesVolumeItem> Details { get; set; }
    }
    public class SalesVolumeItem : BaseModel
    {
        public int MstSalesVolumeId { get; set; }
        public string Type { get; set; }
        public string Region { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public Decimal Value { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public string ModulName { get; set; }
        public SelectList ModulList { get; set; }
        public SelectList RoleNameList { get; set; }
    }

    public class SalesVolumeUpload : BaseModel
    {
        public string Type { get; set; }
        public string Region { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Value { get; set; }

    }
}