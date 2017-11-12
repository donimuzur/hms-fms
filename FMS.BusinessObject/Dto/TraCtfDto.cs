using FMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class TraCtfDto
    {
        public long TraCtfId { get; set; }
        public string DocumentNumber { get; set; }
        public Enums.DocumentStatus DocumentStatus { get; set; }
        public long? EpafId { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string CostCenter { get; set; }
        public int? GroupLevel { get; set; }
        public int? Reason { get; set; }
        public string ReasonS { get; set; }
        public string SupplyMethod { get; set; }
        public string PoliceNumber { get; set; }
        public int? VehicleYear { get; set; }
        public string VehicleType { get; set; }
        public string VehicleUsage { get; set; }
        public string VehicleLocation{ get; set; }
        public DateTime? EndRendDate { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool? IsTransferToIdle { get; set; }
        public decimal? BuyCost { get; set; }
        public bool? ExtendVehicle { get; set; }
        public string WithdPic { get; set; }
        public string WithdPhone { get; set; }
        public DateTime? WithdDate { get; set; }
        public string WithdCity { get; set; }
        public string WithdAddress { get; set; }
        public decimal? EmployeeContribution { get; set; }
        public decimal? Penalty { get; set; }
        public decimal? RefundCost { get; set; }
        public decimal? BuyCostTotal { get; set; }
        public int? UserDecision { get; set; }
        public string PenaltyPoNumber { get; set; }
        public string PenaltyPoLine { get; set; }
        public decimal? PenaltyPrice { get; set; }
        public int? Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public MST_EPAF MstEpaf { get; set; }
        public MST_EMPLOYEE MstEmployee { get;set; }
        public MST_REASON MstReason { get; set; }
        public MST_REMARK MstRemark { get; set; }

    }
}
