using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.Core;
using FMS.BusinessObject.Dto;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class RptKpiMonitoringModel : BaseModel
    {
        public RptKpiMonitoringModel()
        {
            ListTransaction = new List<KpiMonitoringItem>();
            SearchView = new KpiReportSearchView();
        }
        public KpiReportSearchView SearchView { get; set; }
        public List<KpiMonitoringItem> ListTransaction { get; set; }
    }
    public class KpiMonitoringItem : BaseModel
    {
        public int Id { get; set; }
        public long TraId { get; set; }
        public string FormType { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string CostCenter { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string Reason { get; set; }
        public string Address { get; set; }
        public string PreviousBaseTown { get; set; }
        public string NewBaseTown { get; set; }
        public string VehicleUsage { get; set; }
        public int? VehicleGroup { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public string PoliceNumber { get; set; }
        public string Location { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

        public DateTime? TemporaryRequestDate { get; set; }
        public DateTime? EeReceivedTemp { get; set; }
        public DateTime? SendToEmpDate { get; set; }
        public DateTime? SendBackToHr { get; set; }
        public DateTime? SendToFleetDate { get; set; }
        public DateTime? SendToEmpBenefit { get; set; }
        public DateTime? SendSuratKuasa { get; set; }
        public DateTime? SendAgreement { get; set; }
        public string Remark { get; set; }
        public decimal? Kpi1 { get; set; }
        public decimal? Kpi2 { get; set; }
        public decimal? Kpi3 { get; set; }
        public decimal? Kpi4 { get; set; }
        public decimal? Kpi5 { get; set; }
    }
    public class KpiReportSearchView 
    {
        public string FormType { get; set; }
        public string VehicleUsage { get; set; }
        public DateTime? FormDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Location { get; set; }

        public SelectList FormTypeList { get; set; }
        public SelectList VehicleUsageList { get; set; }
        public SelectList LocationList { get; set; }
    }
}