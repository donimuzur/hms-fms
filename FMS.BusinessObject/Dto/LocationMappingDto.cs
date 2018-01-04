using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class LocationMappingDto
    {
        public int MstLocationMappingId { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string Region { get; set; }
        public string Basetown { get; set; }
        public string ZoneSales { get; set; }
        public string ZonePriceList { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
