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
    
    public partial class vehicle_type
    {
        public int vehicle_type_id { get; set; }
        public string vehicle_type1 { get; set; }
        public string vehicle_usage { get; set; }
        public System.DateTime last_modified { get; set; }
        public string modified_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string created_by { get; set; }
        public Nullable<bool> is_active { get; set; }
    }
}