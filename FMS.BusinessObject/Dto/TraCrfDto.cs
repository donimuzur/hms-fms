using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class TraCrfDto
    {
        public long TRA_CRF_ID { get; set; }
        public string DOCUMENT_NUMBER { get; set; }
        public int DOCUMENT_STATUS { get; set; }
        public long? EPAF_ID { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string EMPLOYEE_NAME { get; set; }
        public string COST_CENTER { get; set; }
        public string COST_CENTER_NEW { get; set; }
        public string LOCATION_CITY { get; set; }
        public string LOCATION_OFFICE { get; set; }
        public string LOCATION_CITY_NEW { get; set; }
        public string LOCATION_OFFICE_NEW { get; set; }
        public string VEHICLE_TYPE { get; set; }
        public string VEHICLE_USAGE { get; set; }
        public DateTime? EFFECTIVE_DATE { get; set; }
        public string RELOCATION_TYPE { get; set; }
        public DateTime? TEMPORARY_DELIVERABLE_DATE { get; set; }
        public string POLICE_NUMBER { get; set; }
        public string MANUFACTURER { get; set; }
        public string MODEL { get; set; }
        public string SERIES { get; set; }
        public string BODY_TYPE { get; set; }
        public int? VENDOR_ID { get; set; }
        public string VENDOR_NAME { get; set; }
        public DateTime? START_PERIOD { get; set; }
        public DateTime? END_PERIOD { get; set; }
        public string WITHD_CITY { get; set; }
        public string WITHD_ADDRESS { get; set; }
        public string WITHD_PIC { get; set; }
        public string WITHD_PHONE { get; set; }
        public DateTime? WITHD_DATETIME { get; set; }
        public string DELIV_CITY { get; set; }
        public string DELIV_ADDRESS { get; set; }
        public string DELIV_PIC { get; set; }
        public string DELIV_PHONE { get; set; }
        public bool? CHANGE_POLICE_NUMBER { get; set; }
        public DateTime? EXPECTED_DATE { get; set; }
        public string PO_NUMBER { get; set; }
        public string PO_LINE { get; set; }
        public decimal? PRICE { get; set; }
        public int? REMARK { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
        public bool IS_ACTIVE { get; set; }
    }
}
