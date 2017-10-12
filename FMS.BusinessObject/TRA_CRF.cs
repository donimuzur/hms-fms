//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FMS.BusinessObject
{
    using System;
    using System.Collections.Generic;
    
    public partial class TRA_CRF
    {
        public long TRA_CRF_ID { get; set; }
        public string DOCUMENT_NUMBER { get; set; }
        public int DOCUMENT_STATUS { get; set; }
        public Nullable<long> EPAF_ID { get; set; }
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
        public Nullable<System.DateTime> EFFECTIVE_DATE { get; set; }
        public string RELOCATION_TYPE { get; set; }
        public Nullable<System.DateTime> TEMPORARY_DELIVERABLE_DATE { get; set; }
        public string POLICE_NUMBER { get; set; }
        public string MANUFACTURER { get; set; }
        public string MODEL { get; set; }
        public string SERIES { get; set; }
        public string BODY_TYPE { get; set; }
        public Nullable<int> VENDOR_ID { get; set; }
        public string VENDOR_NAME { get; set; }
        public Nullable<System.DateTime> START_PERIOD { get; set; }
        public Nullable<System.DateTime> END_PERIOD { get; set; }
        public string WITHD_CITY { get; set; }
        public string WITHD_ADDRESS { get; set; }
        public string WITHD_PIC { get; set; }
        public string WITHD_PHONE { get; set; }
        public Nullable<System.DateTime> WITHD_DATETIME { get; set; }
        public string DELIV_CITY { get; set; }
        public string DELIV_ADDRESS { get; set; }
        public string DELIV_PIC { get; set; }
        public string DELIV_PHONE { get; set; }
        public Nullable<bool> CHANGE_POLICE_NUMBER { get; set; }
        public Nullable<System.DateTime> EXPECTED_DATE { get; set; }
        public string PO_NUMBER { get; set; }
        public string PO_LINE { get; set; }
        public Nullable<decimal> PRICE { get; set; }
        public Nullable<int> REMARK { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public bool IS_ACTIVE { get; set; }
    
        public virtual MST_EMPLOYEE MST_EMPLOYEE { get; set; }
        public virtual MST_EPAF MST_EPAF { get; set; }
        public virtual MST_REMARK MST_REMARK { get; set; }
        public virtual MST_VENDOR MST_VENDOR { get; set; }
    }
}
