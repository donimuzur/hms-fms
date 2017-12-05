using FMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class TraCcfDto
    {
        public long TraCcfId { get; set; }
        public string DocumentNumber { get; set; }
        public Enums.DocumentStatus DocumentStatus { get; set; }
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
        public string Region { get; set; }

        public TraCcfDetailDto DetailSave { get; set; }

        public List<TraCcfDetailDto> Details { get; set; } 
    }

    public class TraCcfDetailDto
    {
        public long TraCcfId { get; set; }
        public long TraCcfDetilId { get; set; }
        public DateTime ComplaintDate { get; set; }
        public string ComplaintNote { get; set; }
        public string ComplaintAtt { get; set; }
        public string ComplaintUrl { get; set; }
        public DateTime? CoodinatorResponseDate { get; set; }
        public string CoodinatorNote { get; set; }
        public DateTime? CoodinatorPromiseDate { get; set; }
        public string CoodinatorAtt { get; set; }
        public string CoordinatorUrl { get; set; }
        public DateTime? VendorResponseDate { get; set; }
        public string VendorNote { get; set; }
        public DateTime? VendorPromiseDate { get; set; }
        public string VendorAtt { get; set; }
        public string VendorUrl { get; set; }
    }
}
