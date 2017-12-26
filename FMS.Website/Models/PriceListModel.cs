using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
 
namespace FMS.Website.Models
{
    public class PriceListModel : BaseModel
    {
        public PriceListModel ()
        {
            Details =new  List<PriceListItem>();
        }

        public List<PriceListItem> Details { get; set; }
    }

    public class PriceListItem : BaseModel
    {
        public int MstPriceListId { get; set; }
        public int Year { get; set; }
        [Required]
        public string Manufacture { get; set; }
        public string Model { get; set; }
        public string Series { get; set; }
        public string ZonePriceList { get; set; }
        public decimal Price { get; set; }
        public decimal InstallmenHMS { get; set; }
        public decimal InstallmenEMP { get; set; }
        public int Vendor { get; set; }
        public string VendorName { get; set; }
        public string VehicleType { get; set; }
        public string VehicleUsage { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveS { get; set; }

        public SelectList ZoneList { get; set; }
        public SelectList VendorList { get; set; }
        public SelectList VehicleTypeList { get; set; }
        public SelectList VehicleUsageList { get; set; }

    }

}