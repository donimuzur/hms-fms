using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FMS.Website.Models
{
    public class RptAutoGrModel : BaseModel
    {
        public RptAutoGrModel()
        {
            Details = new List<RptAutoGrItem>();
        }
        public List<RptAutoGrItem> Details { get; set; }

        public DateTime? PeriodStart { get; set; }

        public DateTime? PeriodEnd { get; set; }

        public string PONumber { get; set; }

        public int POLine { get; set; }
    }

    public class RptAutoGrItem
    {
        public string PoNumber { get; set; }

        public string PoLine { get; set; }

        public DateTime GrDate { get; set; }

        public string PoliceNumber { get; set; }

        public DateTime? StartContract { get; set; }

        public DateTime? EndContract { get; set; }


        public DateTime? TerminationDate { get; set; }

        public decimal QtyAutoGr { get; set; }

        public decimal QtyCalculated { get; set; }

        public decimal QtyRemaining { get; set; }

        public string GrDateString {
            get
            {
                return GrDate.ToString("dd-MMM-yyyy");
            } 
             
        }

        public string StartContractString {
            get
            {
                return StartContract.HasValue? StartContract.Value.ToString("dd-MMM-yyyy") : "";
            } 
        }

        public string EndContractString
        {
            get
            {
                return EndContract.HasValue ? EndContract.Value.ToString("dd-MMM-yyyy") : "";
            }
        }

        public string TerminationDateString {
            get
            {
                return TerminationDate.HasValue ? TerminationDate.Value.ToString("dd-MMM-yyyy") : "";
            }

        }
    }
}