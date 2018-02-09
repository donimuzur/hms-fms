using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class FleetDashboardModel : BaseModel
    {
        public FleetDashboardModel()
        {
            Details = new List<FleetDashboardItem>();
            SearchView = new FleetDashboardSearchView();
        }
        public List<FleetDashboardItem> Details { get; set; }
        public FleetDashboardSearchView SearchView { get; set; }

        public int TotalData { get; set; }
        public int TotalDataPerPage { get; set; }

        public int CurrentPage { get; set; }
    }

    public class FleetDashboardSearchView
    {
        public string PoliceNumber { get; set; }
        public string ChasisNumber { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        public SelectList PoliceNumberList { get; set; }
        public SelectList ChasisNumberList { get; set; }
        public SelectList EmployeeIDList { get; set; }
        public SelectList EmployeeNameList { get; set; }
    }

    public class FleetDashboardItem : BaseModel
    {
        public long FleetChangeId { get; set; }
        public long FleetId { get; set; }
        public string PoliceNumber { get; set; }
        public string ChasisNumber { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string FieldName { get; set; }
        public string DataBefore { get; set; }
        public string DataAfter { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ChangeDate { get; set; }
        public DateTime? DateSend { get; set; }
        public DateTime? DateUpdate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}