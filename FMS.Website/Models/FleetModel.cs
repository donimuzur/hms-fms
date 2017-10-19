using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class FleetModel : BaseModel
    {
        public FleetModel()
        {
            Details = new List<FleetItem>();
        }
        public List<FleetItem> Details { get; set; }
    }

    public class FleetItem : BaseModel
    {
        public long MstFleetId { get; set; }
        public string PoliceNumber { get; set; }
        public string ChasisNumber { get; set; }
        public string EngineNumber { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public int GroupLevel { get; set; }
        public string ActualGroup { get; set; }
        public string AssignedTo { get; set; }
        public string CostCenter { get; set; }
        public string VendorName { get; set; }
        public string Manufacturer { get; set; }
        public string Models { get; set; }
        public string Series { get; set; }
        public string BodyType { get; set; }
        public string Color { get; set; }
        public string Transmission { get; set; }
        public int CarGroupLevel { get; set; }
        public string FuelType { get; set; }
        public string Branding { get; set; }
        public bool Airbag { get; set; }
        public int VehicleYear { get; set; }
        public string VehicleType { get; set; }
        public string VehicleUsage { get; set; }
        public string SupplyMethod { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Purpose { get; set; }
        public bool Vat { get; set; }
        public bool Restitution { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public string PoNumber { get; set; }
        public string PoLine { get; set; }
        public DateTime? StartContract { get; set; }
        public DateTime? EndContract { get; set; }
        public decimal Price { get; set; }
        public string VehicleStatus { get; set; }
        public bool IsTaken { get; set; }
        public decimal GrLeftQty { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public string CreatedDates { get; set; }
        public string ModifiedDates { get; set; }
        public string StartDates { get; set; }
        public string EndDates { get; set; }
        public string TerminationDates { get; set; }
        public string StartContracts { get; set; }
        public string EndContracts { get; set; }

        public SelectList VendorList { get; set; }
        public SelectList SupplyMethodList { get; set; }
        public SelectList BodyTypeList { get; set; }
        public SelectList VehicleTypeList { get; set; }
        public SelectList TransmissionList { get; set; }
    }
}    
