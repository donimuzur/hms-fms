using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class EpafDto
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
        public DateTime? ApprovedDate { get; set; }
        public int? Remark { get;set;}
        public string LastUpdate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public string CsfNumber { get; set; }
        public long? CsfId { get; set; }
        public string CsfStatus { get; set; }


        public string CrfNumber { get; set; }
        public long? CrfId { get; set; }
        public string CrfStatus { get; set; }

        public string CityNew { get; set; }
        public string BaseTownNew { get; set; }
    }
}
