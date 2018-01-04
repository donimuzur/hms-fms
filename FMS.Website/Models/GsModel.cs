using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Parser.SyntaxTree;

namespace FMS.Website.Models
{
    public class GsModel : BaseModel
    {
        public List<GsItem> Details { get; set; }

        public ReportFilter FilterReport { get; set; }

        public GsModel()
        {
            Details = new List<GsItem>();
            FilterReport = new ReportFilter();
        }

        public SelectList VehicleUsageList { get; set; }
    }
    public class GsItem : BaseModel
    {
        public int MstGsId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeId { get; set; }
        public string VehicleUsage { get; set; }
        public string Manufacturer { get; set; }
        public string Transmission { get; set; }
        public string Models { get; set; }
        public string Series { get; set; }
        public string PoliceNumber { get; set; }
        public int? GroupLevel { get; set; }
        public string GroupLevels { get; set; }
        public string Location { get; set; }
        public DateTime? GsRequestDate { get; set; }

        public string GsRequestDates
        {
            get
            {
                return GsRequestDate.HasValue ? GsRequestDate.Value.ToString("dd-MMM-yyyy") : "";
            }
        }

        public DateTime? GsFullfillmentDate { get; set; }

        public string GsFullfillmentDates
        {
            get
            {
                return GsFullfillmentDate.HasValue ? GsFullfillmentDate.Value.ToString("dd-MMM-yyyy") : "";
            }
        }

        public string GsManufacturer { get; set; }
        public string GsModel { get; set; }
        public string GsSeries { get; set; }
        public string GsTransmission { get; set; }
        public string GsUnitType { get; set; }
        public string GsPoliceNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public string StartDates {
            get
            {
                return StartDate.HasValue ? StartDate.Value.ToString("dd-MMM-yyyy") : "";

            }
        }
        public DateTime? EndDate { get; set; }

        public DateTime? LeadTime
        {
            get
            {
                var span = this.EndDate - this.StartDate;
                
                return span.HasValue ? new DateTime(span.Value.Ticks) : (DateTime?)null;
            }
        }

        public string LeadTimeS {
            get
            {
                var span = this.EndDate - this.StartDate;
                return span.HasValue ? span.Value.Days + " day(s)" : "";
                //LeadTime == null ? "" :

                //    LeadTime.Value.DayOfYear + " day(s) ";
            }
        }
        public string EndDates
        {
            get
            {
                return EndDate.HasValue ? EndDate.Value.ToString("dd-MMM-yyyy") : "";

            }
        }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public string KpiFulfillment
        {
            get
            {
                var span = this.GsFullfillmentDate - this.GsRequestDate;
                //var kpi = new DateTime(span.Value.Ticks);

                return span.HasValue ? span.Value.Days + " day(s) " : "";
            }
        }

        public string RentTime
        {
            get
            {
                var span = this.EndDate - this.GsFullfillmentDate;
                //var kpi = new DateTime(span.Value.Ticks);
                //span.Value.Days
                return span.HasValue ? span.Value.Days + " day(s) " : "";
            }
        }

        public SelectList EmployeeList { get; set; }
        public SelectList PoliceNumberList { get; set; }
        public SelectList RemarkList { get; set; }
        public SelectList LocationList { get; set; }
    }


    public class ReportFilter
    {
        public DateTime? StartDateBegin { get; set; }

        public DateTime? StartDateEnd { get; set; }

        public DateTime? EndDateBegin { get; set; }

        public DateTime? EndDateEnd { get; set; }

        public string VehicleUsage { get; set; }

        public string Location { get; set; }
    }
}