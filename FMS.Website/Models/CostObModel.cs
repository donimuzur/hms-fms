using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class CostObModel : BaseModel
    {
        public CostObModel()
        {
            Details = new List<CostObItem>();
        }

        public List<CostObItem> Details { get; set; }
        public List<CostOBUpload> UploadedData { get; set; }

        public int CurrentPage { get; set; }
    }

    public class CostObItem : BaseModel
    {
        public long MstCostObId { get; set; }
        [Required]
        public int? Year { get; set; }
        public string CostCenter { get; set; }
        public int Qty { get; set; }
        public int Month { get; set; }
        public string MonthS { get; set; }
        public string Zone { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public decimal? ObCost { get; set; }
        public string ObCostS { get; set; }
        [Required]
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public SelectList ZoneList { get; set; }
        public SelectList RemarkList { get; set; }
        public SelectList ModelList { get; set; }
        public SelectList MonthList { get; set; }
        public SelectList TypeList { get; set; }
        public bool IsActive { get; set; }

    }

    public class CostOBUpload
    {
        public string CostCenter { get; set; }
        public string BodyType { get; set; }
        public string VehicleType { get; set; }
        public decimal SumOfJan { get; set; }
        public decimal SumOfFeb { get; set; }
        public decimal SumOfMar { get; set; }
        public decimal SumOfApr { get; set; }
        public decimal SumOfMay { get; set; }
        public decimal SumOfJun { get; set; }
        public decimal SumOfJul { get; set; }
        public decimal SumOfAug { get; set; }
        public decimal SumOfSep { get; set; }
        public decimal SumOfOct { get; set; }
        public decimal SumOfNov { get; set; }
        public decimal SumOfDec { get; set; }
        public int SumOfQtyJan { get; set; }
        public int SumOfQtyFeb { get; set; }
        public int SumOfQtyMar { get; set; }
        public int SumOfQtyApr { get; set; }
        public int SumOfQtyMay { get; set; }
        public int SumOfQtyJun { get; set; }
        public int SumOfQtyJul { get; set; }
        public int SumOfQtyAug { get; set; }
        public int SumOfQtySep { get; set; }
        public int SumOfQtyOct { get; set; }
        public int SumOfQtyNov { get; set; }
        public int SumOfQtyDec { get; set; }
    }

}