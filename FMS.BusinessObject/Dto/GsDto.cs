using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class GsDto
    {
        public int MstGsId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeId { get; set; }
        public string VehicleUsage { get; set; }
        public string PoliceNumber { get; set; }
        public int? GroupLevel { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string Series { get; set; }
        public string Transmission { get; set; }
        public string Location { get; set; }
        public DateTime? GsRequestDate { get; set; }
        public DateTime? GsFullfillmentDate { get; set; }
        public string GsManufacturer { get; set; }
        public string GsModel { get; set; }
        public string GsSeries { get; set; }
        public string GsTransmission { get; set; }
        public string GsUnitType { get; set; }
        public string GsPoliceNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? LeadTime { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
