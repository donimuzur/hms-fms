using System.ComponentModel;

namespace FMS.Core
{
	public class Enums
	{
		public enum MenuList
		{
            MasterData = 2,
			MasterEmployee = 3,
            MasterFleet = 4,
			MasterComplaintCategory = 5,
            MasterPriceList = 6,
            MasterVendor = 7,
            MasterSetting = 8,
            MasterEpaf = 9,
            MasterPenalty = 10,
            MasterGroupCostCenter = 11,
            MasterHoliday = 12,
            MasterReason = 13,
            MasterFuelOdoMeter = 14,
            MasterVehicleSpect = 15,
            MasterLocationMapping = 16,
            MasterCostOB = 17,
            MasterGS = 18,
            MasterSalesVolume = 19,
            MasterDelegation = 20,
            MasterRemark = 21,
            MasterSysAccess = 22,
            
            MasterPenaltyLogic = 23,
            TraCrf = 24
		}

	    public enum DocumentType
	    {
	        CSF = 1,
            CRF = 3,
            CTF = 6
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
			[Description("CFM")]
			BenefitCfm = 10,
            [Description("COP")]
            BenefitCop = 11,
			[Description("WTC")]
			Wtc = 12,
			

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
