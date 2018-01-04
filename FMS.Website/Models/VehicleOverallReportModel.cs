﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class VehicleOverallReportModel : BaseModel
    {
        public VehicleOverallReportModel()
        {
            SearchViewExport = new VehicleOverallSearchViewExport();
            SearchView = new VehicleOverallSearchView();
            ListVehicle = new List<VehicleOverallItem>();
            
        }
        
        public List<VehicleOverallItem> ListVehicle { get; set; }
        public VehicleOverallSearchView SearchView { get; set; }
        public VehicleOverallSearchViewExport SearchViewExport { get; set; }

    }
    public class VehicleOverallItem : BaseModel
    {
        public VehicleOverallItem()
        {
            DetailsHistory = new List<VehicleHistory>();
        }
        public int Id { get; set; }
        public string PoliceNumber { get; set; }
        public string Manufacture { get; set; }
        public string Models { get; set; }
        public string Series { get; set; }
        public string BodyType { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string VehicleType { get; set; }
        public string CostCenter { get; set; }
        public DateTime? StartContract { get; set; }
        public DateTime? EndContract { get; set; }
        public string SupplyMethod { get; set; }
        public string Vendor { get; set; }
        public string Function { get; set; }
        public string Regional { get; set; }
        public string City { get; set; }
        public string Transmission { get; set; }
        public string FuelType { get; set; }
        public string Branding { get; set; }
        public string Colour { get; set; }
        public bool Airbag { get; set; }
        public bool Abs { get; set; }
        public string ChasisNumber { get; set; }
        public string EngineNumber { get; set; }
        public bool VehicleStatus { get; set; }
        public string AssetsNumber { get; set; }
        public DateTime? TerminationDate { get; set; }
        public bool Restitution { get; set; }
        public decimal? MonthlyInstallment { get; set; }
        public string MonthlyInstallmentStr { get; set; }
        public decimal? Vat { get; set; }
        public string VatStr { get; set; }
        public decimal? TotalMonthlyInstallment { get; set; }
        public string TotalMonthlyInstallmentStr { get; set; }
        public string PoNumber { get; set; }
        public string PoLine { get; set; }
        public int ReportMonth { get; set; }
        public int ReportYear { get; set; }
        public DateTime CreatedDate { get; set; }
        public long MstFleetId { get; set; }
        public List<VehicleHistory> DetailsHistory { get; set; }
    }
    public class VehicleHistory
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string Employee { get; set; }
        public string Description { get; set; }
    }
    public class VehicleOverallSearchView
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool? VehicleStatus { get; set; }
        public string SupplyMethod { get; set; }
        public string BodyType { get; set; }
        public string VehicleType { get; set; }
        public string Vendor { get; set; }
        public string Function { get; set; }
        public string Regional { get; set; }
        public string City { get; set; }

        
        public SelectList StatusList { get; set; }
        public SelectList VehicleTypeList { get; set; }
        public SelectList SupplyMethodList { get; set; }
        public SelectList RegionalList { get; set; }
        public SelectList VendorList { get; set; }
        public SelectList BodyTypeList { get; set; }
        public SelectList CityList { get; set; }

    }

    public class VehicleOverallSearchViewExport
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool? VehicleStatus { get; set; }
        public string SupplyMethod { get; set; }
        public string BodyType { get; set; }
        public string VehicleType { get; set; }
        public string Vendor { get; set; }
        public string Function { get; set; }
        public string Regional { get; set; }
        public string City { get; set; }
    }
}