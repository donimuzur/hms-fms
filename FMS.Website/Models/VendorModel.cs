using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace FMS.Website.Models
{
    public class VendorModel : BaseModel
    {
        public VendorModel ()
        {
            Details =new  List<VendorItem>();
        }

        public List<VendorItem> Details { get; set; }
    }

    public class VendorItem : BaseModel
    {
        public int MstVendorId { get; set; }
        public string VendorName { get; set; }
        public string ShortName { get; set; }
        public string EmailAddress { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime?  ModifiedDate { get; set; }
        public bool IsActive { get; set; }

    }

    public class VendorUploadItem : BaseModel
    {
        public int MstVendorId { get; set; }
        public string VendorName { get; set; }
        public string ShortName { get; set; }
        public string EmailAddress { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }

    }
}