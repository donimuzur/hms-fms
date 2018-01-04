using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class PenaltyDto
    {
        public int MstPenaltyId { get; set; }
        public int Vendor { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string Series { get; set; }
        public string BodyType { get; set; }
        public int Year { get; set; }
        public int MonthStart { get; set; }
        public int MonthEnd { get; set; }
        public string VehicleType { get; set; }
        public int Penalty { get; set; }
        public bool Restitution { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
