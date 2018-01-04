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
        public FuelOdometerSearchView SearchView { get; set; }
        public FuelOdometerModel()
        {
            Details = new List<FuelOdometerItem>();
            SearchView = new FuelOdometerSearchView();
        }

        public int TotalData { get; set; }
        public int TotalDataPerPage { get; set; }

        public int CurrentPage { get; set; }
    }

    public class FuelOdometerSearchView
    {
        public string VehicleType { get; set; }
        public string PoliceNumber { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EcsRmbTransId { get; set; }
        public string SeqNumber { get; set; }
        public string ClaimType { get; set; }
        public string DateOfCost { get; set; }
        public string CostCenter { get; set; }
        public string FuelAmount { get; set; }
        public string LastKM { get; set; }
        public string Cost { get; set; }
        public string ClaimComment { get; set; }
        public string PostedTime { get; set; }
        public string Status { get; set; }


        public SelectList PoliceNumberList { get; set; }

        public SelectList EmployeeIDList { get; set; }

        public SelectList EmployeeNameList { get; set; }

        public SelectList VehicleTypeList { get; set; }

        public SelectList CostCenterList { get; set; }

        public SelectList ClaimTypeList { get; set; }

        public SelectList EcsRmbTransIdList { get; set; }

        public SelectList StatusList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem(){ Text = "Active", Value = "True"},
                    new SelectListItem(){ Text = "InActive", Value = "False"}
                };
                return new SelectList(items, "Value", "Text");
            }

        }
    }

    public class FuelOdometerItem : BaseModel
    {
        public long MstFuelOdometerId { get; set; }
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