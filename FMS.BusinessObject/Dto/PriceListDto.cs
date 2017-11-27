using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class PriceListDto
    {
        public int MstPriceListId { get; set; }
        public int Year { get; set; }
        public string Manufacture { get; set; }
        public string Model { get; set; }
        public string Series { get; set; }
        public string ZonePriceList { get; set; }
        public decimal Price { get; set; }
        public decimal InstallmenHMS { get; set; }
        public decimal InstallmenEMP { get; set; }
        public int Vendor { get; set; }
        public string VehicleType { get; set; }
        public string VehicleUsage { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
