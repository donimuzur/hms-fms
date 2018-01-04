using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Core;
using FMS.Utils;

namespace FMS.BusinessObject.Dto
{
    public class TraCafDto
    {
        public long TraCafId { get; set; }
        public string DocumentNumber { get; set; }

        public int? DocumentStatus { get; set; }
        public string DocumentStatusString
        {
            get
            {
                return EnumHelper.GetDescription((Enums.DocumentStatus)this.DocumentStatus);
            }
        }
        public string SirsNumber { get; set; }
        public string PoliceNumber { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Supervisor { get; set; }

        public string Area { get; set; }

        public int? VendorId { get; set; }
        public string VendorName { get; set; }

        public string VehicleModel { get; set; }
        public DateTime IncidentDate { get; set; }
        public string IncidentLocation { get; set; }
        public int? RemarkId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public List<TraCafProgressDto> ProgressDetails { get; set; }

        public string IncidentDescription { get; set; }
    }

    public class TraCafProgressDto
    {
        public long TraCafId { get; set; }
        public int? StatusId { get; set; }

        public DateTime? ProgressDate { get; set; }

        public string Remark { get; set; }
        public DateTime? Estimation { get; set; }

        public DateTime? Actual { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

    }
}
