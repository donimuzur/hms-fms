using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.Core;

namespace FMS.Website.Models
{
    public class CsfDashboardModel : BaseModel
    {
        public CsfDashboardModel()
        {
            EpafList = new List<EpafData>();
        }

        public string TitleForm { get; set; }
        public List<EpafData> EpafList { get; set; }
        public SelectList RemarkList { get; set; }
    }

    public class CsfIndexModel : BaseModel
    {
        public CsfIndexModel()
        {
            CsfList = new List<CsfData>();
        }

        public string TitleForm { get; set; }
        public string TitleExport { get; set; }
        public List<CsfData> CsfList { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsPersonalDashboard { get; set; }
    }

    public class CsfItemModel : BaseModel
    {

        public CsfItemModel()
        {
            Detail = new CsfData();
            Temporary = new TemporaryData();
            TemporaryList = new List<TemporaryData>();
            VehicleList = new List<VehicleData>();
        }

        public bool IsPersonalDashboard { get; set; }
        public SelectList RemarkList { get; set; }
        public CsfData Detail { get; set; }
        public TemporaryData Temporary { get; set; }
        public List<TemporaryData> TemporaryList { get; set; }
        public List<VehicleData> VehicleList { get; set; }
    }

    public class EpafData
    {
        public long MstEpafId { get; set; }
        public DateTime EpafEffectiveDate { get; set; }
        public DateTime? EpafApprovedDate { get; set; }
        public bool LetterSend { get; set; }
        public string Action { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string CostCentre { get; set; }
        public string GroupLevel { get; set; }
        public long? CsfId { get; set; }
        public string CsfNumber { get; set; }
        public string CsfStatus { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsCop3Years { get; set; }
    }

    public class CsfData
    {
        public long TraCsfId { get; set; }
        public string CsfNumber { get; set; }
        public Enums.DocumentStatus CsfStatus { get; set; }
        public string CsfStatusName { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeIdCreator { get; set; }
        public string EmployeeName { get; set; }
        public string CostCenter { get; set; }
        public int ReasonId { get; set; }
        public string Reason { get; set; }
        public int? GroupLevel { get; set; }
        public string VehicleType { get; set; }
        public string VehicleTypeName { get; set; }
        public string VehicleCat { get; set; }
        public string VehicleUsage { get; set; }
        public string SupplyMethod { get; set; }
        public string Project { get; set; }
        public string ProjectName { get; set; }
        public string LocationCity { get; set; }
        public string LocationAddress { get; set; }
        public string Manufacturer { get; set; }
        public string Models { get; set; }
        public string Series { get; set; }
        public string BodyType { get; set; }
        public string VendorName { get; set; }
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
        public bool IsAirBagVendor { get; set; }
        public bool IsVatVendor { get; set; }
        public bool IsRestitutionVendor { get; set; }

        public string PoliceNumber { get; set; }
        public string PoNumber { get; set; }

        public int RemarkId { get; set; }
        public int TemporaryId { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpectedDate { get; set; }
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        public DateTime StartPeriodVendor { get; set; }
        public DateTime EndPeriodVendor { get; set; }
        public DateTime EndRentDate { get; set; }
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
        public SelectList VehicleCatList { get; set; }
        public SelectList VehicleUsageList { get; set; }
        public SelectList SupplyMethodList { get; set; }
        public SelectList ProjectList { get; set; }
        public SelectList LocationCityList { get; set; }
        public SelectList LocationAddressList { get; set; }
    }

    public class TemporaryData
    {
        public long TraTemporaryId { get; set; }
        public string TemporaryNumber { get; set; }
        public Enums.DocumentStatus TemporaryStatus { get; set; }
        public string TemporaryStatusName { get; set; }
        public string CsfNumber { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string CostCenter { get; set; }
        public string PoNumber { get; set; }
        public string PoliceNumber { get; set; }
        public string ChasisNumber { get; set; }
        public string EngineNumber { get; set; }
        public string Manufacturer { get; set; }
        public string Models { get; set; }
        public string Series { get; set; }
        public string BodyType { get; set; }
        public string Color { get; set; }
        public string VendorName { get; set; }
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        public string StartPeriodName { get; set; }
        public string EndPeriodName { get; set; }
        public string StartPeriodValue { get; set; }
        public string EndPeriodValue { get; set; }
        public bool IsAirBag { get; set; }
        public bool IsVat { get; set; }
        public bool IsRestitution { get; set; }
        public string Transmission { get; set; }
        public string Branding { get; set; }
        public string Purpose { get; set; }
        public int VehicleYear { get; set; }
        public string PoLine { get; set; }

        public int ReasonIdTemp { get; set; }
        public string ReasonTemp { get; set; }
        public string UrlTemp { get; set; }
        public string MessageError { get; set; }
        public string MessageErrorStopper { get; set; }
        public SelectList ReasonTempList { get; set; }
    }

    public class VehicleData
    {
        public string Manufacturer { get; set; }
        public string Models { get; set; }
        public string Series { get; set; }
        public string BodyType { get; set; }
        public string Color { get; set; }
        public string Vendor { get; set; }
        public string MessageError { get; set; }
    }
}