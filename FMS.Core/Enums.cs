using System.ComponentModel;

namespace FMS.Core
{
	public class Enums
	{
		public enum MenuList
		{
			MasterEmployee = 2,
			MasterComplaintCategory = 3
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
			[Description("PBCK-1")]
			PBCK1 = 1,
			[Description("CK-5")]
			CK5 = 2,
			[Description("PBCK-4")]
			PBCK4 = 3,
			[Description("PBCK-3")]
			PBCK3 = 4,
			[Description("LACK-1")]
			LACK1 = 5,
			[Description("LACK-2")]
			LACK2 = 6,
			[Description("CK-4C")]
			CK4C = 7,
			[Description("PBCK-7")]
			PBCK7 = 8,
			[Description("CK-5 Market Return")]
			CK5MarketReturn = 9,			
			[Description("CK-5 Market Return")]
            MasterDataApproval = 10,
			[Description("Product Development")]
			ProductDevelopment = 51,
			[Description("Brand Registration")]
			BrandRegistrationReq = 52,
			[Description("Penetapan SKEP")]
			PenetapanSKEP = 53,
			[Description("Financial Ratio")]
			FinanceRatio = 40,
			[Description("Tariff")]
			Tariff = 43,
			[Description("Supporting Document")]
			SupportingDocument = 42,
			[Description("Product Type")]
			ProductType = 37,
			[Description("Excise Credit")]
			ExciseCredit = 45,

            [Description("Interview Request Workflow")]
            InterviewRequestWorkflow = 48,
            [Description("License Request Workflow")]
            LicenseRequestWorkflow = 49,
            [Description("Change Request Workflow")]
            ChangeRequestWorkflow = 50,

            [Description("Interview Request")]
            InterviewRequest = 81,
            [Description("License Request")]
            LicenseRequest = 82,
            [Description("Change Request")]
            ChangeRequest = 83,
            [Description("Brand Registration Transaction")]
            BrandRegistrationTrans = 84,
            [Description("Brand Registration Penetapan SKEP")]
            BrandRegistrationSKEP = 85,

            [Description("LACK-10")]
            LACK10 = 61
        }

		public enum ActionType
		{
			[Description("Created")]
			Created = 1,
			[Description("Cancel")]
			Cancel = 2,
			[Description("CK5 Cancel")]
			CancelSAP = 3,
			[Description("CK5 Cancel")]
			CancelSTOCreated = 4,
			[Description("Modified")]
			Modified = 5,
			[Description("Submit")]
			Submit = 10,
			[Description("Waiting for Approval")]
			WaitingForApproval = 11,
            [Description("Waiting for Approval 2")]
            WaitingForApproval2 = 12,
			[Description("Approve")]
			Approve = 15,
			[Description("Reject")]
			Reject = 20,
			[Description("Sealed")]
			Sealed = 21,
			[Description("UnSealed")]
			UnSealed = 22,
			[Description("Good Issue")]
			GoodIssue = 23,
			[Description("Good Receive")]
			GoodReceive = 24,
			[Description("Gov Approve")]
			GovApprove = 25,
			[Description("Gov Partial Approve")]
			GovPartialApprove = 26,
			[Description("Gov Reject")]
			GovReject = 30,
			[Description("GovCancel")]
			GovCancel = 35,
			[Description("Completed")]
			Completed = 40,
			[Description("Sto Created")]
			STOCreated = 45,
			[Description("STO Failed")]
			STOFailed = 50,
			[Description("Outbound Delivery Created")]
			ODCreated = 55,
			[Description("Good Received Created")]
			GRCreated = 60,
			[Description("Good Received Partial")]
			GRPartial = 65,
			[Description("Good Received Completed")]
			GRCompleted = 70,
			[Description("Good Received Reversal")]
			GRReversal = 75,
			[Description("Good Issue Created")]
			GICreated = 80,
			[Description("Good Issue Partial")]
			GIPartial = 85,
			[Description("Good Issue Completed")]
			GICompleted = 90,
			[Description("Good Issue Reversal")]
			GIReversal = 95,
			[Description("Waiting For Disposal")]
			WaitingForWasteDisposal = 96,
			[Description("Waiting For Waste Approval")]
			WaitingForWasteApproval = 97,
			[Description("Disposal Rejected")]
			WasteDisposalRejected = 98,
			[Description("Disposal Uploaded")]
			WasteDisposalUploaded = 99,
			[Description("Cancelled")]
			Cancelled = 100,
			[Description("Waste Approved")]
			WasteApproved = 101,
			[Description("STOB Good Issue Completed")]
			StobGICompleted = 105,
			[Description("REC STO Created")]
			StoRecCreated = 200,
			[Description("REC GR Completed")]
			StobGRCompleted = 205,

			[Description("STOB GI Partial")]
			StobGIPartial = 210,

			[Description("Rec STO GI Partial")]
			StoRecGIPartial = 215,

			[Description("Rec STO GI Completed")]
			StoRecGICompleted = 220,

			[Description("Rec STO GR Partial")]
			StoRecGRPartial = 225,


			[Description("Rec STO GR Completed")]
			StoRecGRCompleted = 230,


			[Description("STOB Good Issue Reversal")]
			STOBGIReversal = 235,

			[Description("STOB Good Receive Reversal")]
			STOBGRReversal = 240,

			[Description("STOB Good Issue Partial")]
			STOBGIPartial = 245,

			[Description("STOB Good Receive Partial")]
			STOBGRPartial = 250,
			[Description("TF Posted")]
			TFPosted = 300,

			[Description("TF Reversal")]
			TFReversed = 310,

			[Description("TF Partial")]
			TFPartial = 315,

			[Description("STO Cancelled")]
			STOCancelled = 320,

			[Description("Back to Gov Approval")]
			BackToGovApprovalAfterCompleted,

			[Description("Purchase Order Created")]
			POCreated = 325,
			
			[Description("Revise")]
			Revise = 326,
			
			[Description("Waiting For POA SKEP Approval")]
			WaitingForPOASKEPApproval = 27,
			
			[Description("Withdraw")]
			Withdraw = 28,

			[Description("Awaiting Government Approval")]
			SubmitSkep = 401
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

		public enum ExciseSettlement
		{
			[Description("Pembayaran")]
			Pembayaran = 1,
			[Description("Pelekatan Pita Cukai")]
			PitaCukai = 2,
			[Description("Pembubuhan Tanda LunasLainnya")]
			PembubuhanTandaLunasLainnya = 3
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
