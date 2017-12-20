using System.ComponentModel;

namespace FMS.Core
{
	public class Enums
	{
		public enum MenuList
		{
            Home = 1,

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

            TraCrf = 24,
            TraCsf = 25,
            TraCtf = 26,
            TraTmp = 27,
            TraCaf = 28,
            TraCcf = 29,
            Transaction = 30,

            Login = 31,
            PersonalDashboard = 32,

            RptCfmIdle = 33,
            RptKpiMonitoring = 34,
            RptVehicle = 35,
            RptFuel = 36,
            RptPo = 37,
            RptAutoGr = 38,
            RptGs = 39,
            RptExecutiveSummary = 40,
            RptCcf = 41
		}

        public enum DocumentType
        {
            CSF = 1,
            TMP = 2,
            CRF = 3,
            CAF = 4,
            CCF = 5,
            CTF = 6,
            GS = 7
        }

        public enum DocumentStatus
        {
            [Description("Draft")]
            Draft = 1,
            [Description("Assigned For User")]
            AssignedForUser = 2,
            [Description("Assigned For HR")]
            AssignedForHR = 3,
            [Description("Assigned For Fleet")]
            AssignedForFleet = 4,
            [Description("Waiting HR Approval")]
            WaitingHRApproval = 5,
            [Description("Waiting Fleet Approval")]
            WaitingFleetApproval = 6,
            [Description("Rejected")]
            Rejected = 7,
            [Description("Cancelled")]
            Cancelled = 8,
            [Description("Extended")]
            Extended = 9,
            [Description("In Progress")]
            InProgress = 10,
            [Description("Completed")]
            Completed = 11,
            [Description("Reporting")]
            Reporting = 20,
            [Description("Administrative")]
            Administrative = 21,
            [Description("Repairing Process")]
            RepairingProcess = 22,
            [Description("Awaiting For Sparepart")]
            AwaitingForSparepart = 23,
            [Description("Delivery")]
            Delivery = 24,
            [Description("Rejected By User")]
            RejectedByUser = 25
            
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
            [Description("Extend")]
            Extend = 10,
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
            Viewer = 1,
            HR = 2,
            Fleet = 3,
            IsSupport = 4,
            User = 5,
            Administrator = 15
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

        public enum SettingGroup
        {
            [Description("VEHICLE_TYPE")]
            VehicleType = 1,
            [Description("VEHICLE_USAGE_BENEFIT")]
            VehicleUsageBenefit = 2,
            [Description("VEHICLE_USAGE_WTC")]
            VehicleUsageWtc = 3,
            [Description("VEHICLE_CATEGORY")]
            VehicleCategory = 4,
            [Description("PROJECT")]
            Project = 5,
            [Description("SUPPLY_METHOD")]
            SupplyMethod = 6,
            [Description("STATUS_VEHICLE")]
            StatusVehicle = 7,
            [Description("USER_ROLE")]
            UserRole = 8,
            [Description("BODY_TYPE")]
            BodyType = 9,
            [Description("BODY_MAIL_CSF")]
            BodyMailCsf = 10,
        }

		

	}
}
