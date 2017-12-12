using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class CfmIdleReportModel : BaseModel
    {
        public CfmIdleReportModel()
        {
            SearchViewExport = new CfmIdleSearchViewExport();
            SearchView = new CfmIdleSearchView();
            ListCfmIdle = new List<CfmIdleVehicle>();
        }

        public List<CfmIdleVehicle> ListCfmIdle { get; set; }
        public CfmIdleSearchView SearchView { get; set; }
        public CfmIdleSearchViewExport SearchViewExport { get; set; }
    }
    public class CfmIdleVehicle
    {
        public int CfmIdleId { get; set; }
        public string PoliceNumber { get; set; }
        public string Manufacture { get; set; }
        public string Models { get; set; }
        public string Series { get; set; }
        public string BodyType { get; set; }
        public string Color { get; set; }
        public int? GroupLevel { get; set; }
        public DateTime? StartContract { get; set; }
        public DateTime? EndContract { get; set; }
        public string SupplyMethod { get; set; }
        public string Vendor { get; set; }
        public string CostCenter { get; set; }
        public string Transmission { get; set; }
        public string FuelType { get; set; }
        public DateTime? StartIdle { get; set; }
        public DateTime? EndIdle { get; set; }
        public decimal? IdleDuration{ get; set; }
        public decimal? MonthlyInstallment { get; set; }
        public decimal? TotalMonthly { get; set; }
        public int? ReportMonth { get; set; }
        public int? ReportYear { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class CfmIdleSearchView
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string PoliceNumber { get; set; }
        public string CostCenter { get; set; }


        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string VehicleType { get; set; }
        public string SupplyMethod { get; set; }
        public string Function { get; set; }

        public SelectList MonthList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "ALL", Value = "0" },
                    new SelectListItem() {Text = "January", Value = "1" },
                    new SelectListItem() {Text = "February", Value = "2" },
                    new SelectListItem() {Text = "March", Value = "3" },
                    new SelectListItem() {Text = "April", Value = "4" },
                    new SelectListItem() {Text = "May", Value = "5" },
                    new SelectListItem() {Text = "June", Value = "6" },
                    new SelectListItem() {Text = "July", Value = "7" },
                    new SelectListItem() {Text = "August", Value = "8" },
                    new SelectListItem() {Text = "September", Value = "9" },
                    new SelectListItem() {Text = "October", Value = "10" },
                    new SelectListItem() {Text = "November", Value = "11" },
                    new SelectListItem() {Text = "December", Value = "12" }
                };
                return new SelectList(items, "Value", "Text");
            }

        }

        public SelectList VehicleTypeList { get; set; }
        public SelectList SupplyMethodList { get; set; }
    }

    public class CfmIdleSearchViewExport
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string PoliceNumber { get; set; }
        public string CostCenter { get; set; }

        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string VehicleType { get; set; }
        public string SupplyMethod { get; set; }
        public string Function { get; set; }
    }
}