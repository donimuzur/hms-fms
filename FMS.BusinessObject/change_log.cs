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
    
    public partial class change_log
    {
        public long log_id { get; set; }
        public Nullable<int> form_type_id { get; set; }
        public Nullable<long> form_id { get; set; }
        public string status { get; set; }
        public string user_name { get; set; }
        public Nullable<System.DateTime> log_date { get; set; }
        public string role { get; set; }
        public Nullable<int> remark_id { get; set; }
    }
}
