using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class DelegationModel : BaseModel
    {
        public DelegationModel()
        {
            Details = new List<DelegationItem>();
        }

        public List<DelegationItem> Details { get; set; }
    }

    public class DelegationItem : BaseModel
    {
        public int MstDelegationID { get; set; }
        public String EmployeeFrom { get; set; }
        public String EmployeeTo { get; set; }
        public String EmployeeFromS { get; set; }
        public String EmployeeToS { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool IsComplaintFrom { get; set; }
        public String Attachment { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public string NameEmployeeFrom { get; set; }
        public string NameEmployeeTo { get; set; }

        public SelectList EmployeeListFrom { get; set; }
        public SelectList EmployeeListTo { get; set; }
    }

    public class DelegationUploadItem : BaseModel
    {
        public string NameEmployeeFrom { get; set; }
        public string NameEmployeeTo { get; set; }
        public string EmployeeFrom { get; set; }
        public string EmployeeTo { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string IsComplaintForm { get; set; }
    }
}