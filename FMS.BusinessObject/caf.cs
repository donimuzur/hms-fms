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
    
    public partial class caf
    {
        public caf()
        {
            this.caf_progress = new HashSet<caf_progress>();
        }
    
        public long accident_id { get; set; }
        public Nullable<int> reason_id { get; set; }
        public Nullable<int> form_type_id { get; set; }
        public string SIRS_number { get; set; }
        public Nullable<int> coordinator { get; set; }
        public Nullable<int> caf_status_id { get; set; }
        public string police_number { get; set; }
        public string employee_name { get; set; }
        public string supervisor { get; set; }
        public string location { get; set; }
        public string model { get; set; }
        public string vendor_name { get; set; }
        public Nullable<System.DateTime> incident_date { get; set; }
        public string incident_location { get; set; }
        public string incident_description { get; set; }
        public string progress_update { get; set; }
        public Nullable<System.DateTime> progress_date { get; set; }
        public string remark { get; set; }
        public Nullable<int> estimation { get; set; }
        public Nullable<int> actual { get; set; }
        public string incident_type { get; set; }
        public string scope { get; set; }
        public Nullable<System.DateTime> last_modified { get; set; }
        public string modified_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> input_month { get; set; }
    
        public virtual ICollection<caf_progress> caf_progress { get; set; }
    }
}
