using FMS.BusinessObject.Dto;
using FMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class CcfModel : BaseModel
    {
        public List<CcfItem> Details { get; set; }
        public string TitleForm { get; set; }
        public string DocumentStatus { get; set; }
        public SelectList RemarkList { get; set; }

        public bool IsPersonalDashboard { get; set; }
        public CcfModel()
        {
            Details = new List<CcfItem>();
        }
    }

    public class CcfItem : BaseModel
    {
        public long TraCcfId { get; set; }
        public string DocumentNumber { get; set; }
        public Enums.DocumentStatus DocumentStatus { get; set; }
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
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        public int CoordinatorKPI { get; set; }
        public int VendorKPI { get; set; }
        public string ComplaintCategoryName { get; set; }
        public string ComplaintCategoryRole { get; set; }
        public SelectList EmployeeFromDelegationList { get; set; }
        public SelectList ComplaintCategoryList { get; set; }
        public SelectList SettingListVType { get; set; }
        public SelectList SettingListVUsage { get; set; }
        public SelectList SettingListFleet { get; set; }
        public string TitleForm { get; set; }
        public string isSubmit { get; set; }
        public string VStartPeriod { get; set; }
        public string VEndPeriod { get; set; }
        public bool IsPersonalDashboard { get; set; }
        public string Region { get; set; }

        public long TraCcfDetilId { get; set; }
        public DateTime ComplaintDate { get; set; }
        public string ComplaintNote { get; set; }
        public string ComplaintUrl { get; set; }
        public string ComplaintAtt { get; set; }
        public DateTime CoodinatorResponseDate { get; set; }
        public string CoodinatorNote { get; set; }
        public DateTime CoodinatorPromiseDate { get; set; }
        public string CoodinatorUrl { get; set; }
        public string CoodinatorAtt { get; set; }
        public DateTime VendorResponseDate { get; set; }
        public string VendorNote { get; set; }
        public DateTime VendorPromiseDate { get; set; }
        public string VendorUrl { get; set; }
        public string VendorAtt { get; set; }
        public string ConfigUrl { get; set; }
        public string StsTraCcfId { get; set; }
        public DateTime StsVndrDate { get; set; }
        public List<CcfItemDetil> Details_d1 { get; set; }

        public CcfItemDetil DetailSave { get; set; }
        public CcfItem()
        {
            Details_d1 = new List<CcfItemDetil>();
            DetailSave = new CcfItemDetil();
        }
    }

    public class CcfItemDetil : BaseModel
    {
        public long TraCcfId { get; set; }
        public long TraCcfDetilId { get; set; }
        public DateTime ComplaintDate { get; set; }
        public string ComplaintNote { get; set; }
        public string ComplaintAtt { get; set; }
        public DateTime? CoodinatorResponseDate { get; set; }
        public string CoodinatorNote { get; set; }
        public DateTime? CoodinatorPromiseDate { get; set; }
        public string CoodinatorAtt { get; set; }
        public DateTime? VendorResponseDate { get; set; }
        public string VendorNote { get; set; }
        public DateTime? VendorPromiseDate { get; set; }
        public string VendorAtt { get; set; }
        public string ComplaintUrl { get; set; }
        public string CoordinatorUrl { get; set; }
        public string VendorUrl { get; set; }
    }
}