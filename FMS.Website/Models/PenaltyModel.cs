using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class PenaltyModel : BaseModel
    {
        public PenaltyModel()
        {
            Details = new List<PenaltyItem>();
            SearchView = new PenaltySearchView();
        }

        public PenaltySearchView SearchView { get; set; }
        public List<PenaltyItem> Details { get; set; }
    }

    public class PenaltySearchView
    {
        public string Vendor { get; set; }
        public string RequestYear { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string Series { get; set; }
        public string BodyType { get; set; }
        public string VehicleType { get; set; }

        public SelectList VendorList { get; set; }
        public SelectList ManufacturerList { get; set; }
        public SelectList ModelList { get; set; }
        public SelectList SeriesList { get; set; }
        public SelectList BodyTypeList { get; set; }
        public SelectList VehicleTypeList { get; set; }
    }

    public class PenaltyItem : BaseModel
    {
        public int MstPenaltyId { get; set; }
        public int Vendor { get; set; }
        public string VendorName { get; set; }
        public string Manufacturer { get; set; }
        public string Models { get; set; }
        public string Series { get; set; }
        public string BodyType { get; set; }
        public int? Year { get; set; }
        public int MonthStart { get; set; }
        public int MonthEnd { get; set; }
        public string VehicleType { get; set; }
        public int Penalty { get; set; }
        public string PenaltyLogic { get; set; }
        public bool Restitution { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public SelectList VehicleList { get; set; }
        public SelectList RestitutionList { get; set; }
        public SelectList PenaltyList { get; set; }
        public SelectList LogicList { get; set;}
        public SelectList VendorList { get; set; }
    }
}