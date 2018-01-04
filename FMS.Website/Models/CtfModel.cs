﻿using FMS.BusinessObject.Dto;
using FMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class CtfModel : BaseModel
    {
        public List<CtfItem> Details { get; set; }
        public string TitleForm { get; set; }
        public SelectList RemarkList { get; set; }

        public bool IsPersonalDashboard { get; set; }
        public CtfModel()
        {
            Details = new List<CtfItem>();
        }
    }
    public class CtfItem : BaseModel
    {
        public EpafDto EPafData { get; set; }
        public CtfExtendDto CtfExtend { get; set; }
        public long TraCtfId { get; set; }
        public string DocumentNumber { get; set; }

        public Enums.DocumentStatus DocumentStatus { get; set; }
        public string DocumentStatusS { get; set; }

        public long? EpafId { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string CostCenter { get; set; }
        public string CostCenterFleet { get; set; }
        public int? GroupLevel { get; set; }
        public int? Reason { get; set; }
        public string ReasonS { get; set; }
        public string SupplyMethod { get; set; }
        public string PoliceNumber { get; set; }
        public int? VehicleYear { get; set; }
        public string VehicleType { get; set; }
        public string VehicleUsage { get; set; }
        public DateTime? EndRendDate { get; set; }
        public string EndRendDateS { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool IsTransferToIdle { get; set; }
        public decimal? BuyCost { get; set; }
        public string BuyCostStr { get; set; }
        public bool ExtendVehicle { get; set; }
        public string WithdPic { get; set; }
        public string WithdPhone { get; set; }
        public DateTime? WithdDate { get; set; }
        public string WithdCity { get; set; }
        public string WithdAddress { get; set; }
        public decimal? EmployeeContribution { get; set; }
        public string EmployeeContributionStr { get; set; }
        public decimal? Penalty { get; set; }
        public string PenaltyStr { get; set; }
        public decimal? RefundCost { get; set; }
        public string RefundCostStr { get; set; }
        public decimal? BuyCostTotal { get; set; }
        public string BuyCostTotalStr { get; set; }
        public int? UserDecision { get; set; }
        public string PenaltyPoNumber { get; set; }
        public string PenaltyPoLine { get; set; }
        public decimal? PenaltyPrice { get; set; }
        public string PenaltyPriceStr { get; set; }
        public int? Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedDateS { get; set; }
        public string ModifiedBy { get; set; }
        public string VehicleLocation { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public bool IsPenalty { get; set; }
        public string Region { get; set; }
        public string EmployeeIdCreator { get; set; }
        public string EmployeeIdFleetApproval { get; set; }
        public string ApprovedFleet { get; set; }
        public DateTime? ApprovedFleetDate { get; set; }


        public string TitleForm { get; set; }
        public string isSubmit { get; set; }
        public bool IsPersonalDashboard { get; set; }
        public bool lessthan2month { get; set; }
        public bool lessthan3month { get; set; }
        public bool lessthan7day { get; set; }
        public long? MstFleetId { get;  set; }
        public bool? isSend { get; set; }

        public SelectList ExtendList { get; set; }
        public SelectList EmployeeIdList { get; set; }
        public SelectList ReasonList { get; set; }
        public SelectList RemarkList{ get; set; }
        public SelectList PoliceNumberList { get; set; }
        public SelectList UserDecisionList { get; set; }
        public SelectList VehicleLocationList { get; set; }

    }

    public class CtfUploadModel : BaseModel
    {
        public List<CtfItem> Details { get; set; }
        public string TitleForm { get; set; }
        public SelectList RemarkList { get; set; }

        public bool IsPersonalDashboard { get; set; }

        public SelectList ReasonList { get; set; }
        public string Reason { get; set; }

        public bool ExtendVehicle { get; set; }
        public CtfExtendDto CtfExtend { get; set; }
        public SelectList ExtendList { get; set; }

        public CtfUploadModel()
        {
            Details = new List<CtfItem>();
        }
    }
    public class CtfDashboardItem : BaseModel
    {
      

    }
}