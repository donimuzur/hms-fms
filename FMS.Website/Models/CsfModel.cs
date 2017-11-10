using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        public SelectList VehicleTypeList { get; set; }
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
        public string CsfStatus { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string CostCenter { get; set; }
        public int ReasonId { get; set; }
        public string Reason { get; set; }
        public string GroupLevel { get; set; }
        public int VehicleType { get; set; }
        public string BodyType { get; set; }
        public int RemarkId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }

        public SelectList EmployeeList { get; set; }
        public SelectList ReasonList { get; set; }
    }
}