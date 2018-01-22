using FMS.BusinessObject.Dto;
using System.Collections.Generic;

namespace FMS.Website.Models
{
    public class RptVehicleMakeDataGrouped
    {
        public RptVehicleMakeFirstGroupedData FirstData { get; set; }
        public int? TotalAll { get; set; }
        public List<NoVehicleMakeDto> GroupedData { get; set; }
    }
    public class RptVehicleMakeFirstGroupedData
    {
        public string Manufacturer { get; set; }
        public int? NoOfVehicle { get; set; }
    }
}