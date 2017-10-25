using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class FuelOdometerModel : BaseModel
    {
        public List<FuelOdometerItem> Details { get; set; }
        public FuelOdometerModel()
        {
            Details = new List<FuelOdometerItem>();
        }
    }

    public class FuelOdometerItem : BaseModel
    {
        public int MstFuelOdometerId { get; set; }
        public string VehicleType { get; set; }
        public string PoliceNumber { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int EcsRmbTransId { get; set; }
        public int SeqNumber { get; set; }
        public string ClaimType { get; set; }
        public DateTime DateOfCost { get; set; }
        public string CostCenter { get; set; }
        public Decimal FuelAmount { get; set; }
        public Decimal LastKM { get; set; }
        public Decimal Cost { get; set; }
        public string ClaimComment { get; set; }
        public DateTime PostedTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}