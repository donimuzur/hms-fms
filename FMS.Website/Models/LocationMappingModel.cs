using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class LocationMappingModel : BaseModel
    {
        public List<LocationMappingItem> Details { get; set; }
        public LocationMappingModel()
        {
            Details = new List<LocationMappingItem>();
        }

    }

    public class LocationMappingItem : BaseModel
    {
        public int MstLocationMappingId { get; set; }
        public string Location { get; set; }
        public string Basetown { get; set; }
        public string Address { get; set; }
        public string Region { get; set; }
        public string ZoneSales { get; set; }
        public string ZonePriceList { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public string ValidFromS { get; set; }
        public SelectList LocationList { get; set; }
        public SelectList AddressList { get; set; }
    }
}