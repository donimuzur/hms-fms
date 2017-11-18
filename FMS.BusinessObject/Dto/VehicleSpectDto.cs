using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class VehicleSpectDto
    {
        public int MstVehicleSpectId { get; set; }
        public string Manufacturer { get; set; }
        public string Models { get; set; }
        public string Series { get; set; }
        public string BodyType { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public string Image { get; set; }
        public int GroupLevel { get; set; }
        public int FlexPoint { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
