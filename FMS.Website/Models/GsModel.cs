using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class GsModel : BaseModel
    {
        public List<GsItem> Details { get; set; }

        public GsModel()
        {
            Details = new List<GsItem>();
        }
    }
    public class GsItem : BaseModel
    {
        public int MstGsId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeId { get; set; }
        public string VehicleUsage { get; set; }
        public string Manufacturer { get; set; }
        public string Transmission { get; set; }
        public string Model { get; set; }
        public string Series { get; set; }
        public string PoliceNumber { get; set; }
        public int? GroupLevel { get; set; }
        public string GroupLevels { get; set; }
        public string Location { get; set; }
        public DateTime? GsRequestDate { get; set; }
        public string GsRequestDates { get; set; }
        public DateTime? GsFullfillmentDate { get; set; }
        public string GsFullfillmentDates { get; set; }
        public string GsManufacturer { get; set; }
        public string GsModel { get; set; }
        public string GsSeries { get; set; }
        public string GsTransmission { get; set; }
        public string GsUnitType { get; set; }
        public string GsPoliceNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public string StartDates { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? LeadTime { get; set; }
        public string LeadTimeS { get; set; }
        public string EndDates { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }


        public SelectList EmployeeList { get; set; }
        public SelectList PoliceNumberList { get; set; }
        public SelectList RemarkList { get; set; }
        public SelectList LocationList { get; set; }
    }
}