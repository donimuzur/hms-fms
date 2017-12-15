using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.Core;
using FMS.BusinessObject.Dto;

namespace FMS.Website.Models
{
    public class RptKpiMonitoringModel : BaseModel
    {
        public List<string> FormTyps { get; set; }
        public List<string> VehicleUsages { get; set; }
        public List<KPI_REPORT_DATA> KpiReportDatas { get; set; }
        public string TitleForm { get; set; }
    }

    public class KpiReportSearchView 
    {

    }
}