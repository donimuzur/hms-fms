using System;
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
    }

    public class CsfItemModel : BaseModel
    {

        public CsfItemModel()
        {
            Detail = new CsfData();
        }

        public SelectList RemarkList { get; set; }
        public CsfData Detail { get; set; }
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
        public string EmployeeName { get; set; }
        public string CostCenter { get; set; }
        public int ReasonId { get; set; }
        public string Reason { get; set; }
        public int? GroupLevel { get; set; }
        public string VehicleType { get; set; }
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
        public int RemarkId { get; set; }
        public int TemporaryId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpectedDate { get; set; }
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        public DateTime EndRentDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string IsSaveSubmit { get; set; }

        public bool IsBenefit { get; set; }

        public SelectList EmployeeList { get; set; }
        public SelectList ReasonList { get; set; }
        public SelectList VehicleTypeList { get; set; }
        public SelectList VehicleCatList { get; set; }
        public SelectList VehicleUsageList { get; set; }
        public SelectList SupplyMethodList { get; set; }
        public SelectList ProjectList { get; set; }
    }
}