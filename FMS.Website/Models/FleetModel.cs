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
            SearchView = new FleetSearchView();
        }
        public List<FleetItem> Details { get; set; }
        public FleetSearchView SearchView { get; set; }


        public int TotalData { get; set; }
        public int TotalDataPerPage { get; set; }

        public int CurrentPage { get; set; }
    }

    public class FleetSearchView
    {
        public string Status { get; set; }
        public string VehicleType { get; set; }
        public string VehicleUsage { get; set; }
        public string SupplyMethod { get; set; }
        public string BodyType { get; set; }
        public string Vendor { get; set; }
        public string Function { get; set; }
        public string StartRent { get; set; }
        public string EndRent { get; set; }
        public string Regional { get; set; }
        public string City { get; set; }

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

        public SelectList SupplyMethodList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "Extend", Value = "E"},
                    new SelectListItem() { Text = "Temporary", Value = "T"},
                    new SelectListItem() { Text = "Lease", Value = "L" },
                    new SelectListItem() { Text = "Services", Value = "S" }
                };
                return new SelectList(items, "Value", "Text");
            }
        }

        public SelectList BodyTypeList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "MPV", Value = "MPV"},
                    new SelectListItem() { Text = "SUV", Value = "SUV"},
                    new SelectListItem() { Text = "Forklift", Value = "Forklift" },
                    new SelectListItem() { Text = "Motorcycle", Value = "Motorcycle" },
                    new SelectListItem() { Text = "Truck", Value = "Truck" }
                };
                return new SelectList(items, "Value", "Text");
            }
        }

        public SelectList VehicleTypeList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "WTC", Value = "WTC"},
                    new SelectListItem() { Text = "Benefit", Value = "Benefit"}
                };
                return new SelectList(items, "Value", "Text");
            }
        }
        public SelectList VehicleUsageList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "COP", Value = "COP"},
                    new SelectListItem() { Text = "CFM", Value = "CFM"}
                };
                return new SelectList(items, "Value", "Text");
            }
        }
        public SelectList VendorList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "ASSA", Value = "ASSA"},
                    new SelectListItem() { Text = "TRAC", Value = "TRAC"}
                };
                return new SelectList(items, "Value", "Text");
            }
        }
        public SelectList FunctionList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "Sales", Value = "Sales"},
                    new SelectListItem() { Text = "Marketing", Value = "Marketing"},
                    new SelectListItem() { Text = "Operations", Value = "Operations"},
                    new SelectListItem() { Text = "IS", Value = "IS"}
                };
                return new SelectList(items, "Value", "Text");
            }
        }
        public SelectList RegionalList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "Jakarta", Value = "Jakarta"},
                    new SelectListItem() { Text = "Sumatra 1", Value = "Sumatra1"},
                    new SelectListItem() { Text = "Sumatra 2", Value = "Sumatra2"}
                };
                return new SelectList(items, "Value", "Text");
            }
        }
        public SelectList CityList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "Surabaya", Value = "Surabaya"},
                    new SelectListItem() { Text = "Malang", Value = "Malang"},
                    new SelectListItem() { Text = "Palembang", Value = "Palembang"}
                };
                return new SelectList(items, "Value", "Text");
            }
        }

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
        public string AirbagS { get; set; }
        public int VehicleYear { get; set; }
        public string VehicleType { get; set; }
        public string VehicleUsage { get; set; }
        public string SupplyMethod { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Purpose { get; set; }
        public bool Project { get; set; }
        public string ProjectS { get; set; }
        public string ProjectName { get; set; }
        public bool Vat { get; set; }
        public bool Restitution { get; set; }
        public string RestitutionS { get; set; }
        public decimal MonthlyHMSInstallment { get; set; }
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

        public Decimal? VatDecimal { get; set; }
        public Decimal? TotalMonthlyCharge { get; set; }
        public string Assets { get; set; }
        public string Function { get; set; }
        public string Regional { get; set; }
        public string Comments { get; set; }
        public string CertificateOwnership { get; set; }

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
        public SelectList FuelTypeList { get; set; }
        public SelectList VehicleTypeList { get; set; }
        public SelectList TransmissionList { get; set; }
        public SelectList RegionalList { get; set; }
        public SelectList FunctionList { get; set; }
    }
}    
