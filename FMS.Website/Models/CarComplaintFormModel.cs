using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class CarComplaintFormModel : BaseModel
    {
        public CarComplaintFormModel()
        {
            Details = new List<CarComplaintFormItem>();
        }

        public List<CarComplaintFormItem> Details { get; set; }
    }

    public class CarComplaintFormItem : BaseModel
    {
        public int TraCcfId { get; set; }
        public string DocumentNumber { get; set; }
        public int DocumentStatus { get; set; }
        public int ComplaintCategory { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeIdComplaintFor { get; set; }
        public string EmployeeNameComplaintFor { get; set; }
        public string PoliceNumber { get; set; }
        public string PoliceNumberGS { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}