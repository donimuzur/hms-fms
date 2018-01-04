using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.Core;

namespace FMS.Website.Models
{
    public class TemporaryIndexModel : BaseModel
    {
        public TemporaryIndexModel()
        {
            TempList = new List<TempData>();
        }

        public string TitleForm { get; set; }
        public string TitleExport { get; set; }
        public List<TempData> TempList { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsPersonalDashboard { get; set; }
    }

    public class TempItemModel : BaseModel
    {

        public TempItemModel()
        {
            Detail = new TempData();
        }

        public bool IsPersonalDashboard { get; set; }
        public SelectList RemarkList { get; set; }
        public TempData Detail { get; set; }
    }

    public class TempData
    {
        public long TraTempId { get; set; }
        public string TempNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public Enums.DocumentStatus TempStatus { get; set; }
        public string TempStatusName { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeIdCreator { get; set; }
        public string EmployeeName { get; set; }
        public string CostCenter { get; set; }
        public int ReasonId { get; set; }
        public string Reason { get; set; }
        public string ProjectName { get; set; }
        public int? GroupLevel { get; set; }
        public string VehicleType { get; set; }
        public string VehicleTypeName { get; set; }
        public string VehicleUsage { get; set; }
        public string AssignedTo { get; set; }
        public string VendorName { get; set; }
        public string SupplyMethod { get; set; }
        public string LocationCity { get; set; }
        public string LocationAddress { get; set; }
        public string Manufacturer { get; set; }
        public string Models { get; set; }
        public string Series { get; set; }
        public string BodyType { get; set; }
        public string Color { get; set; }
        public string Regional { get; set; }
        public long? CfmIdleId { get; set; }
        public int? CarGroupLevel { get; set; }

        public string ManufacturerVendor { get; set; }
        public string ModelsVendor { get; set; }
        public string SeriesVendor { get; set; }
        public string BodyTypeVendor { get; set; }
        public string VendorNameVendor { get; set; }
        public string ColorVendor { get; set; }
        public string PoliceNumberVendor { get; set; }
        public string PoNumberVendor { get; set; }
        public string ChasisNumberVendor { get; set; }
        public string EngineNumberVendor { get; set; }
        public string TransmissionVendor { get; set; }
        public string BrandingVendor { get; set; }
        public string PurposeVendor { get; set; }
        public string PoLineVendor { get; set; }
        public string CommentsVendor { get; set; }
        public decimal VatDecimalVendor { get; set; }
        public decimal PriceVendor { get; set; }
        public bool IsAirBagVendor { get; set; }
        public bool IsVatVendor { get; set; }
        public bool IsRestitutionVendor { get; set; }

        public DateTime? StartPeriod { get; set; }
        public DateTime? EndPeriod { get; set; }
        public DateTime? StartPeriodVendor { get; set; }
        public DateTime? EndPeriodVendor { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string IsSaveSubmit { get; set; }

        public bool IsBenefit { get; set; }
        public bool IsActive { get; set; }

        public SelectList EmployeeList { get; set; }
        public SelectList ReasonList { get; set; }
        public SelectList VehicleTypeList { get; set; }
        public SelectList VehicleUsageList { get; set; }
        public SelectList VendorList { get; set; }
        public SelectList SupplyMethodList { get; set; }
        public SelectList LocationCityList { get; set; }
        public SelectList LocationAddressList { get; set; }
        public SelectList ProjectList { get; set; }
    }
}