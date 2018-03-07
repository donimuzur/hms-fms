using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class EpafModel : BaseModel
    {
        public EpafModel()
        {
            Details = new List<EpafItem>();
            SearchView = new EpafSearchView();
        }
        public EpafSearchView SearchView { get; set; }
        public List<EpafItem> Details { get; set; }
    }

    public class EpafItem : BaseModel
    {
        public long MstEpafId { get; set; }
        public int DocumentType { get; set; }
        public string EpafAction { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string CostCenter { get; set; }
        public DateTime? EfectiveDate { get; set; }
        public int GroupLevel { get; set; }
        public string City { get; set; }
        public string BaseTown { get; set; }
        public bool Expat { get; set; }
        public bool LetterSend { get; set; }
        public string LastUpdate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        
    }
    public class EpafSearchView
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EpafAction { get; set; }
        public string DocumentType { get; set; }

        public SelectList EmployeeIdList { get; set; }
        public SelectList EmployeeNameList { get; set; }
        public SelectList EpafActionList { get;set;}
        public SelectList DocumentTypeList { get; set; }
    }
}