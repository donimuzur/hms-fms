using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class RptFuelDto
    {
        public string Id { get; set; }
        public string PoliceNumber { get; set; }
        public string Liter { get; set; }
        public string Odometer { get; set; }
        public string Cost { get; set; }
        public string FuelType { get; set; }
        public string CostCenter { get; set; }
        public string Function { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string Series { get; set; }
        public string BodyType { get; set; }
    }
}
