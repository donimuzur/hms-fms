using FMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class RptCCFModel : BaseModel
    {
        public RptCCFModel()
        {
            RptCCFItem = new List<RptCCFItem>();
            SearchViewExport = new RptCCFSearchViewExport();
            SearchView = new RptCCFSearchView();
            SearchView.PeriodFrom = DateTime.Today;
            SearchView.PeriodTo = DateTime.Today;
        }
        public List<RptCCFItem> RptCCFItem { get; set; }
        public RptCCFSearchView SearchView { get; set; }
        public RptCCFSearchViewExport SearchViewExport { get; set; }
        public string TitleForm { get; set; }
        public string TitleExport { get; set; }
    }
    public class RptCCFItem
    {
        public long Id { get; set; }
        public string DocumentNumber { get; set; }
        public Enums.DocumentStatus DocumentStatus { get; set; }
        public int ComplaintCategory { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeIdComplaintFor { get; set; }
        public string EmployeeNameComplaintFor { get; set; }
        public string PoliceNumber { get; set; }
        public string PoliceNumberGS { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public string LocationCity { get; set; }
        public string LocationAddress { get; set; }
        public string VehicleType { get; set; }
        public string VehicleUsage { get; set; }
        public string Manufacturer { get; set; }
        public string Models { get; set; }
        public string Series { get; set; }
        public string Vendor { get; set; }
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        public int CoordinatorKPI { get; set; }
        public int VendorKPI { get; set; }
        public string CoordinatorName { get; set; }
        public string ComplaintCategoryName { get; set; }
        public string ComplaintCategoryRole { get; set; }
    }

    public class RptCCFSearchView
    {
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public int Category { get; set; }
        public string Coordinator { get; set; }
        public string Location { get; set; }
        public int CoorKPI { get; set; }
        public int VendorKPI { get; set; }
        
        public SelectList Categorylist { get; set; }
        public SelectList Coordinatorlist { get; set; }
        public SelectList Locationlist { get; set; }
    }

    public class RptCCFSearchViewExport
    {
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public int Category { get; set; }
        public string Coordinator { get; set; }
        public string Location { get; set; }
        public int CoorKPI { get; set; }
        public int VendorKPI { get; set; }
    }
}