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
        public List<CarComplaintFormItemDetil> Details_D1 { get; set; }
    }

    public class CarComplaintFormItem : BaseModel
    {
        public int TraCcfId { get; set; }
        public string DocumentNumber { get; set; }
        public int DocumentStatus { get; set; }
        public string DocumentStatusDoc { get; set; }
        public int ComplaintCategory { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeAddress { get; set; }
        public string EmployeeCity { get; set; }
        public string EmployeeIdComplaintFor { get; set; }
        public string EmployeeNameComplaintFor { get; set; }
        public string PoliceNumber { get; set; }
        public string PoliceNumberGS { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string VCreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public string LocationCity { get; set; }
        public string LocationAddress { get; set; }
        public string VehicleType { get; set; }
        public string VehicleUsage { get; set; }
        public string Manufacturer { get; set; }
        public string Models { get; set; }
        public string Series { get; set; }
        public string Vendor { get; set; }
        public DateTime? StartPeriod { get; set; }
        public DateTime? EndPeriod { get; set; }
        public string VStartPeriod { get; set; }
        public string VEndPeriod { get; set; }
        public SelectList EmployeeFromDelegationList { get; set; }
        public SelectList ComplaintCategoryList { get; set; }
        public SelectList SettingListVType { get; set; }
        public SelectList SettingListVUsage { get; set; }
        public SelectList SettingListFleet { get; set; }
        
        public List<CarComplaintFormItem> Details { get; set; }
        public string status_user { get; set; }
        public DateTime ComplaintDate { get; set; }
        public string ComplaintNote { get; set; }
        public string ComplaintAtt { get; set; }
    }

    public class CarComplaintFormItemDetil : BaseModel
    {
        public int TraCcfDetailId { get; set; }
        public int TraCcfId { get; set; }
        public DateTime ComplaintDate { get; set; }
        public string ComplaintNote { get; set; }
        public string ComplaintAtt { get; set; }
        public DateTime CoorResponseDate { get; set; }
        public string CoorNote { get; set; }
        public DateTime CoorPromiseDate { get; set; }
        public string CoorAtt { get; set; }
        public DateTime VendorResponseDate { get; set; }
        public string VendorNote { get; set; }
        public DateTime VendorPromiseDate { get; set; }
        public string VendorAtt { get; set; }
    }
}