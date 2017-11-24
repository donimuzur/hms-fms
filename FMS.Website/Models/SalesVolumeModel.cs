using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public string MonthS { get; set; }
        public int Year { get; set; }
        public Decimal Value { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}