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
    
    public partial class MST_REMARK
    {
        public MST_REMARK()
        {
            this.TRA_CAF = new HashSet<TRA_CAF>();
            this.TRA_CRF = new HashSet<TRA_CRF>();
            this.TRA_CSF = new HashSet<TRA_CSF>();
            this.TRA_CTF = new HashSet<TRA_CTF>();
            this.TRA_WORKFLOW_HISTORY = new HashSet<TRA_WORKFLOW_HISTORY>();
        }
    
        public int MST_REMARK_ID { get; set; }
        public int DOCUMENT_TYPE { get; set; }
        public string REMARK { get; set; }
        public string ROLE_TYPE { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public bool IS_ACTIVE { get; set; }
    
        public virtual MST_DOCUMENT_TYPE MST_DOCUMENT_TYPE { get; set; }
        public virtual ICollection<TRA_CAF> TRA_CAF { get; set; }
        public virtual ICollection<TRA_CRF> TRA_CRF { get; set; }
        public virtual ICollection<TRA_CSF> TRA_CSF { get; set; }
        public virtual ICollection<TRA_CTF> TRA_CTF { get; set; }
        public virtual ICollection<TRA_WORKFLOW_HISTORY> TRA_WORKFLOW_HISTORY { get; set; }
    }
}
