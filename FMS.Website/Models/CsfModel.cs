using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    }

    public class CsfIndexModel : BaseModel
    {
        public CsfIndexModel()
        {
            CsfList = new List<CsfData>();
        }

        public string TitleForm { get; set; }
        public List<CsfData> CsfList { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class CsfItemViewModel : BaseModel
    {

        public CsfItemViewModel()
        {
            Detail = new CsfData();
        }

        public CsfData Detail { get; set; }
    }

    public class EpafData
    {
        public int MstEpafId { get; set; }
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
        public string Reason { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}