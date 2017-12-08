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
            NoVehicleList = new List<NoVehicleData>();
        }

        public string TitleForm { get; set; }
        public string TitleExport { get; set; }
        public List<NoVehicleData> NoVehicleList { get; set; }
    }

    public class NoVehicleData
    {
        public string VehicleFunction { get; set; }
        public decimal? VehicleCost { get; set; }
    }
}