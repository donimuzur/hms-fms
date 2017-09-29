namespace FMS.Core
{
    public class Constans
    {
        public const string MasterDataHeaderFooterFolder = "~/files_upload/";
        public const string UploadPath = "~/files_upload/";
        public const string Pbck1DecreeDocFolderPath = UploadPath + "pbck1decreedoc/";
        public const string Lack1UploadFolderPath = UploadPath + "Lack1/";
        public const string Lack2FolderPaht = UploadPath + "Lack2/";
        public const string CK5FolderPath = UploadPath + "CK5/";
        public const string Ck4cDecreeDocFolderPath = UploadPath + "ck4cdecreedoc/";
		public const string MLFolderPath = UploadPath + "Manufacture/";
		public const string MonthClosingDocFolderPath = UploadPath + "monthClosing/";

        public const string InList = "In List";
        public static readonly string MenuActiveDashboard = "Dashboard";
        public static string DelimeterSelectItem = " - ";
        public const string PI = "PI";


        public const string WasteFloor = "W.FLOOR";
        public const string WasteDust = "W.DUST";
        public const string WasteStem = "W.STEM";

        public const string LabelDelegatedBy = "Delegated By ";

        /// <summary>
        /// list of SessionKey constanta
        /// </summary>
        public class SessionKey
        {
            /// <summary>
            /// Report Path, ex : "Reports/Employee.rdlc"
            /// </summary>
            public const string ReportPath = "sk_reportpath";
            /// <summary>
            /// List of ReportDataSources
            /// </summary>
            public const string ReportDataSources = "sk_reportdatasources";

            public const string ReportParameters = "sk_reportparameters";

            /// <summary>
            /// Current User session key
            /// </summary>
            public const string CurrentUser = "sk_current_user";

            public const string MainMenu = "sk_main_menu";

            public const string ExcelUploadProdConvPbck1 = "ExcelUploadProdConvertedPbck1";
        }

        public class  SubmitType
        {
            public const string Save = "Save";
            public const string Cancel = "Cancel";
            public const string Update = "Update";
            public const string PrintPreview = "PrintPreview";
            public const string Delete = "Delete";
            public const string DataExist = "DataExist";
            public const string DataExistPlant = "Main Plant With NPPBKC ID is Already Exists";
        }
        public class SubmitMessage
        {
            public const string Saved = "Save Successfully";
            public const string Updated = "Update Successfully";
            public const string Deleted = "Delete Successfully";
            public const string DataExist = "Data Already Exist";
            public const string DataExistPlant = "Main Plant With NPPBKC ID is Already Exists";
        }


    }
}
