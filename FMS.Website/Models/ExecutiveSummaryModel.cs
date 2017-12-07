using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FMS.Website.Models
{
    public class ExecutiveSummaryModel : BaseModel
    {
        public ExecutiveSummaryModel()
        {
            ExecutiveList = new List<ExecutiveSummaryData>();
        }

        public string TitleForm { get; set; }
        public string TitleExport { get; set; }
        public List<ExecutiveSummaryData> ExecutiveList { get; set; }
    }

    public class ExecutiveSummaryData
    {
        public string VehicleFunction { get; set; }
        public decimal? VehicleCost { get; set; }
    }
}