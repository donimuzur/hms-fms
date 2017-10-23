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
    
    public partial class crf
    {
        public long crf_id { get; set; }
        public Nullable<int> epaf_id { get; set; }
        public Nullable<int> form_type_id { get; set; }
        public string request_number { get; set; }
        public Nullable<System.DateTime> request_date { get; set; }
        public Nullable<int> coordinator { get; set; }
        public string new_cost_center { get; set; }
        public string new_location { get; set; }
        public string new_address { get; set; }
        public Nullable<System.DateTime> effective_date { get; set; }
        public Nullable<System.DateTime> temporary_deliverable_date { get; set; }
        public string relocation_type { get; set; }
        public string police_number { get; set; }
        public string manufacturer { get; set; }
        public string model { get; set; }
        public string series { get; set; }
        public string body_type { get; set; }
        public string vendor { get; set; }
        public Nullable<System.DateTime> start_period { get; set; }
        public Nullable<System.DateTime> end_period { get; set; }
        public string withd_city { get; set; }
        public string withd_address { get; set; }
        public string withd_pic { get; set; }
        public string withd_phone { get; set; }
        public Nullable<System.DateTime> withd_datetime { get; set; }
        public string deliv_city { get; set; }
        public string deliv_address { get; set; }
        public string deliv_pic { get; set; }
        public string deliv_phone { get; set; }
        public Nullable<System.DateTime> expected_date { get; set; }
        public Nullable<bool> change_police_number { get; set; }
        public string new_police_number { get; set; }
        public Nullable<int> relocate_po_line { get; set; }
        public Nullable<decimal> relocate_cost { get; set; }
        public Nullable<int> crf_status { get; set; }
        public Nullable<long> fleet_idle { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> modified_date { get; set; }
        public string modified_by { get; set; }
        public string relocate_po_number { get; set; }
        public Nullable<long> fleet_id { get; set; }
    
        public virtual epaf epaf { get; set; }
    }
}