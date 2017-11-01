using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class PenaltyLogicModel : BaseModel
    {
        public List<PenaltyLogicItem> Details { get; set; }
        public PenaltyLogicModel()
        {
            Details = new List<PenaltyLogicItem>(); 
        }
    }
    public class PenaltyLogicItem : BaseModel
    {
        public int MstPenaltyLogicId { get; set; }
        public int Year { get; set; }
        public string PenaltyLogic { get; set;}
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public SelectList KolomList { get; set; }
        public string kolom { get; set; }
        public SelectList OperatorList { get; set; }
        public string Operator { get; set; }
        public string CekRumus { get; set; }
        public int? MstVendorId { get; set; }
        public string VendorName { get; set; }
        public SelectList VendorList { get; set; }

        public string VehicleType { get; set; }
        public SelectList VehicleTypeList { get; set; }
 
    }
}