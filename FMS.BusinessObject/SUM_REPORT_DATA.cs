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
    
    public partial class SUM_REPORT_DATA
    {
        public int ID { get; set; }
        public Nullable<int> TOTAL_VEHICLE { get; set; }
        public Nullable<decimal> TOTAL_VEHICLE_COST { get; set; }
        public Nullable<decimal> TOTAL_FUEL_AMOUNT { get; set; }
        public Nullable<int> TOTAL_FUEL_COST { get; set; }
        public Nullable<decimal> TOTAL_KM { get; set; }
        public Nullable<decimal> TOTAL_OPERATIONAL_COST { get; set; }
        public Nullable<int> ACCIDENT_COUNT { get; set; }
        public string VEHICLE_TYPE { get; set; }
        public string FUNCTION { get; set; }
        public string REGION { get; set; }
        public Nullable<int> REPORT_MONTH { get; set; }
        public Nullable<int> REPORT_YEAR { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
    }
}
