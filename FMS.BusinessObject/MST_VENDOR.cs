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
    
    public partial class MST_VENDOR
    {
        public MST_VENDOR()
        {
            this.MST_PENALTY_LOGIC = new HashSet<MST_PENALTY_LOGIC>();
            this.TRA_CAF = new HashSet<TRA_CAF>();
            this.TRA_CRF = new HashSet<TRA_CRF>();
        }
    
        public int MST_VENDOR_ID { get; set; }
        public string VENDOR_NAME { get; set; }
        public string SHORT_NAME { get; set; }
        public string EMAIL_ADDRESS { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public bool IS_ACTIVE { get; set; }
    
        public virtual ICollection<MST_PENALTY_LOGIC> MST_PENALTY_LOGIC { get; set; }
        public virtual ICollection<TRA_CAF> TRA_CAF { get; set; }
        public virtual ICollection<TRA_CRF> TRA_CRF { get; set; }
    }
}
