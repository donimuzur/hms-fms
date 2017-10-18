using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FMS.Website.Models
{
    public class PenaltyModel : BaseModel
    {
        public PenaltyModel()
        {
            Details = new List<PenaltyItem>();
        }

        public List<PenaltyItem> Details { get; set; }
    }

    public class PenaltyItem : BaseModel
    {
        public int MST_PENALTY_ID { get; set; }
        public string MANUFACTURER { get; set; }
        public string MODEL { get; set; }
        public string SERIES { get; set; }
        public Nullable<int> YEAR { get; set; }
        public Nullable<int> MONTH_START { get; set; }
        public Nullable<int> MONTH_END { get; set; }
        public string VEHICLE_TYPE { get; set; }
        public Nullable<int> PENALTY { get; set; }
        public Nullable<bool> RESTITUTION { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
        public bool IS_ACTIVE { get; set; }
    }

    public class penaltyUploadItem : BaseModel
    {
        public int MST_PENALTY_ID { get; set; }
        public string MANUFACTURER { get; set; }
        public string MODEL { get; set; }
        public string SERIES { get; set; }
        public Nullable<int> YEAR { get; set; }
        public Nullable<int> MONTH_START { get; set; }
        public Nullable<int> MONTH_END { get; set; }
        public string VEHICLE_TYPE { get; set; }
        public Nullable<int> PENALTY { get; set; }
        public Nullable<bool> RESTITUTION { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
        public bool IS_ACTIVE { get; set; }
    }
}