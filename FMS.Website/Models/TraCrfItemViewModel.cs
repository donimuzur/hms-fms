using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.Core;
using FMS.Utils;

namespace FMS.Website.Models
{

    public class TraCrfItemViewModel : BaseModel
    {

        public TraCrfItemViewModel()
        {
            Detail = new TraCrfItemDetails();
            DetailTemporary = new TraCrfTemporary();
        }

        public TraCrfItemDetails Detail { get; set; }

        public SelectList EmployeeList { get; set; }
        public SelectList ReasonList { get; set; }
        public SelectList VehicleTypeList { get; set; }
        public SelectList VehicleCatList { get; set; }
        public SelectList VehicleUsageList { get; set; }
        public SelectList SupplyMethodList { get; set; }
        public SelectList ProjectList { get; set; }

        public SelectList RelocateList { get; set; }
        public SelectList LocationList { get; set; }
        public SelectList LocationNewList { get; set; }

        public SelectList RemarkList { get; set; }

        public bool IsAllowedApprove { get; set; }

        public bool IsApproved { get; set; }

        public TraCrfTemporary DetailTemporary { get; set; }

        public List<TemporaryData> TemporaryList { get; set; }

        public bool IsSend { get; set; }
    }

    public class TraCrfIndexViewModel : BaseModel
    {
        public TraCrfIndexViewModel()
        {
            Details = new List<TraCrfItemDetails>();
        }

        public bool IsCompleted { get; set; }
        public List<TraCrfItemDetails> Details { get; set; }

        public bool IsPersonalDashboard { get; set; }
    }

    public class TraCrfDashboardViewModel : BaseModel
    {
        public TraCrfDashboardViewModel()
        {
            Details = new List<TraCrfEpafItem>();
        }
        public List<TraCrfEpafItem> Details { get; set; }
        public SelectList RemarkList { get; set; }
        public SelectList EmployeeList { get; set; }
    }

    public class TraCrfTemporary
    {
        public int TraCrfId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int? ReasonId { get; set; }
    }

    public class TraCrfEpafItem
    {
        public long EpafId { get; set; }
        public string EpafNumber { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool IsLetterSend { get; set; }
        public string EpafAction { get; set; }
        
        public string VehicleUsage { get; set; }

        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        public string CurrentLocation { get; set; }
        //public string CurrentLocationCrf { get; set; }

        public string RelocateLocation { get; set; }

        public long? CrfId { get; set; }
        public string CrfNumber { get; set; }
        public string CrfModifiedBy { get; set; }
        public DateTime? CrfModifiedDate { get; set; }

        public string CrfReason { get; set; }
        
    }

    public class TraCrfItemDetails
    {
        public long TraCrfId { get; set; }
        public string DocumentNumber { get; set; }
        public int DocumentStatus { get; set; }
        public long? EpafId { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        [Required]
        public string EmployeeName { get; set; }
        public string CostCenter { get; set; }
        public string CostCenterNew { get; set; }
        public string LocationCity { get; set; }
        public string LocationOffice { get; set; }
        public string LocationCityNew { get; set; }
        public string LocationOfficeNew { get; set; }
        public string VehicleType { get; set; }
        public string VehicleUsage { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string RelocationType { get; set; }
        public DateTime? TemporaryDeliverableDate { get; set; }
        public string PoliceNumber { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string SERIES { get; set; }
        public string Body { get; set; }
        public int? VendorId { get; set; }
        public string VendorName { get; set; }
        public DateTime? StartPeriod { get; set; }
        public DateTime? EndPeriod { get; set; }
        public string WithdCity { get; set; }
        [Required]
        public string WithdAddress { get; set; }
        public string WithdPic { get; set; }
        public string WithdPhone { get; set; }
        public DateTime? WithdDateTime { get; set; }
        public string DeliveryCity { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryPic { get; set; }
        public string DeliveryPhone { get; set; }
        public bool ChangePoliceNumber { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string PoNumber { get; set; }
        public string PoLine { get; set; }
        public decimal? Price { get; set; }
        public int? RemarkId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public string DocumentStatusString
        {
            get
            {
                return EnumHelper.GetDescription((Enums.DocumentStatus) this.DocumentStatus);
            }
            
        }


        public string NewPoliceNumber { get; set; }

        public long? MstFleetId { get; set; }

        public List<TemporaryIndexModel> ListTemporary { get; set; } 
    }
}