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
    
    public partial class ARCH_MST_VEHICLE_SPECT
    {
        public int MST_VEHICLE_SPECT_ID { get; set; }
        public string MANUFACTURER { get; set; }
        public string MODEL { get; set; }
        public string SERIES { get; set; }
        public string BODY_TYPE { get; set; }
        public Nullable<int> YEAR { get; set; }
        public string COLOUR { get; set; }
        public string IMAGE { get; set; }
        public Nullable<int> GROUP_LEVEL { get; set; }
        public Nullable<int> FLEX_POINT { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public Nullable<bool> IS_ACTIVE { get; set; }
        public string TRANSMISSION { get; set; }
        public string FUEL_TYPE { get; set; }
        public Nullable<System.DateTime> ARCHIVED_DATE { get; set; }
        public string ARCHIVED_BY { get; set; }
    }
}
