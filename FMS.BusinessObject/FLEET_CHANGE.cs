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
    
    public partial class FLEET_CHANGE
    {
        public long FLEET_CHANGE_ID { get; set; }
        public Nullable<long> FLEET_ID { get; set; }
        public string POLICE_NUMBER { get; set; }
        public string CHASIS_NUMBER { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string EMPLOYEE_NAME { get; set; }
        public string FIELD_NAME { get; set; }
        public Nullable<System.DateTime> CHANGE_DATE { get; set; }
        public Nullable<System.DateTime> DATE_SEND { get; set; }
        public string DATA_BEFORE { get; set; }
        public string DATA_AFTER { get; set; }
        public Nullable<System.DateTime> DATE_UPDATE { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
    }
}