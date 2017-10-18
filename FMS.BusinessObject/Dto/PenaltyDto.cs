using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class PenaltyDto
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
