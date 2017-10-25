using System.ComponentModel;

namespace FMS.Core
{
	public class Enums
	{
		public enum MenuList
		{
			MasterEmployee = 1,
            MasterFleet = 2,
			MasterComplaintCategory = 3,
            MasterPriceList = 4,
            MasterVendor = 5,
            MasterSetting = 6,
            MasterEpaf = 7,
            MasterPenalty = 8,
            MasterGroupCostCenter = 9,
            MasterHoliday = 10,
            MasterReason = 11,
            MasterFuelOdoMeter = 12,
            MasterVehicleSpect = 13,
            MasterLocationMapping = 14,
            MasterCostOB = 15,
            MasterGS = 16,
            MasterSalesVolume = 17,
            MasterDelegation = 18,
            MasterRemark = 19,
            MasterSysAccess = 20,
            MasterData = 21,
            MasterPenaltyLogic = 22,
		}
		

        public enum DocumentStatus
        {
            [Description("Draft")]
            Draft = 1,
            [Description("Revised")]
            Revised = 5,
            [Description("Waiting for POA Approval")]
            WaitingForApproval = 10,
            [Description("Waiting for Controller Approval")]
            WaitingForApprovalController = 11,
            [Description("Waiting for POA Approval 2")]
            WaitingForApproval2 = 12,
            [Description("Approved")]
            Approved = 15,
            [Description("Rejected")]
            Rejected = 20,
            
            [Description("Government Approved")]
            GovApproved = 30,
            [Description("Government Rejected")]
            GovRejected = 35,
            [Description("Government Canceled")]
            GovCanceled = 40,
           
            [Description("Cancelled")]
            Cancelled = 100,
            [Description("Completed")]
            Completed = 105,

            [Description("STOB Good Issue Completed")]
            StobGICompleted = 110,

            
        }


        

		public enum FormType
		{
            /*
			[Description("PBCK-1")]
			PBCK1 = 1,
            */
        }

		public enum ActionType
		{
			[Description("Created")]
			Created = 1,
			[Description("Cancel")]
			Cancel = 2,
			[Description("Modified")]
			Modified = 3,
			[Description("Submit")]
			Submit = 4,
			[Description("Waiting for Approval")]
			WaitingForApproval = 5,
			[Description("Approve")]
			Approve = 6,
			[Description("Reject")]
			Reject = 7,
			[Description("Completed")]
			Completed = 8,
			[Description("Cancelled")]
			Cancelled = 9,

		}

		/// <summary>
		/// message popup type
		/// </summary>
		public enum MessageInfoType
		{
			Success,
			Error,
			Warning,
			Info
		}

		public enum UserRole
		{
			User = 1,
			POA = 2,
            Controller = 3,
			Viewer = 4,
			System = 10,
			Administrator = 15,
			SuperAdmin = 16,
            AdminApprover = 17

		}

		public enum FormViewType
		{
			Index = 1,
			Create = 2,
			Edit = 3,
			Detail = 4
		}

		public enum VehicleType
		{
			[Description("Benefit")]
			Benefit = 10,
			[Description("WTC")]
			Wtc = 11,
			

		}


	    public enum BenefitType
	    {
            [Description("Car Ownership Program")]
            Cop = 10,
            [Description("Car For Manager")]
            Cfm = 11,
	    }

		

		

	}
}
