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
        public int Id { get; set; }
        public string VehicleType { get; set; }
        public string SupplyMethod { get; set; }
        public string Function { get; set; }
        public int? NoOfVehicle { get; set; }
        public int? ReportMonth { get; set; }
        public int? ReportYear { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}