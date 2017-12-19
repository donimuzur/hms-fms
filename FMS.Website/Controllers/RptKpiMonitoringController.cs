using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Utils;
using FMS.Website.Models;
using AutoMapper;
using FMS.DAL;
using FMS.BusinessObject;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using SpreadsheetLight;
using SpreadsheetLight.Charts;
using System.Web.Helpers;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using FMS.BusinessObject.Business;

namespace FMS.Website.Controllers
{
    public class RptKpiMonitoringController : BaseController
    {
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private IKpiMonitoringBLL _kpiMonitoringBLL;
        private ITraCtfBLL _ctfBLL;
        private ITraCsfBLL _csfBLL;
        private ITraCrfBLL _crfBLL;
        private IEmployeeBLL _employeeBLL;
        private IRemarkBLL _remarkBLL;
        private ISettingBLL _settingBLL;
        private IDocumentTypeBLL _docTypeBLL;

        public RptKpiMonitoringController(IPageBLL pageBll, IKpiMonitoringBLL KpiMonitoringBLL,IRemarkBLL RemarkBLL, 
                                            ITraCtfBLL CtfBLL, ITraCsfBLL CsfBLL, ITraCrfBLL CrfBLL, IDocumentTypeBLL DocTypeBLL,
                                            IEmployeeBLL EmployeeBLL,ISettingBLL SettingBLL)
            : base(pageBll, Core.Enums.MenuList.RptKpiMonitoring)
        {
            _pageBLL = pageBll;
            _kpiMonitoringBLL = KpiMonitoringBLL;
            _remarkBLL = RemarkBLL;
            _settingBLL = SettingBLL;
            _docTypeBLL = DocTypeBLL;
            _employeeBLL = EmployeeBLL;
            _ctfBLL = CtfBLL;
            _csfBLL = CsfBLL;
            _crfBLL = CrfBLL;
            _mainMenu = Enums.MenuList.RptExecutiveSummary;
        }

        public ActionResult Index()
        {
            var model = new RptKpiMonitoringModel();
            var filter = new KpiMonitoringGetByParamInput();
            
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;

            var FormTypeList = new Dictionary<string, string> { { "CSF", "CSF" }, { "CTF", "CTF" }, { "CRF", "CRF" } };
            var VehicleUsageList =_settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_USAGE_BENEFIT").ToList();
            var AddressList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE).Select(x => new { ADDRESS = x.ADDRESS }).Distinct().OrderBy(x => x.ADDRESS).ToList();

            model.SearchView.FormDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            model.SearchView.ToDate = DateTime.Today;

            model.SearchView.FormTypeList = new SelectList(FormTypeList, "Key", "Value");
            model.SearchView.VehicleUsageList = new SelectList(VehicleUsageList, "SettingName", "SettingName");
            model.SearchView.LocationList = new SelectList(AddressList, "ADDRESS", "ADDRESS");

            filter.FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            filter.ToDate = DateTime.Today;

            try
            {
                var ListTransactionDto = _kpiMonitoringBLL.GetTransaction(filter);
                var ListTransaction = Mapper.Map<List<KpiMonitoringItem>>(ListTransactionDto);
                var UserLogin = GetUserLogin();

                foreach (var item in ListTransaction)
                {
                    var data = GetDateworkflow(item, UserLogin);
                    model.ListTransaction.Add(data);
                }

            }
            catch (Exception exp)
            {

                model.ErrorMessage = exp.Message;
            }
           
            return View(model);
        }

        private List<KpiMonitoringItem> GetTransaction(KpiReportSearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _kpiMonitoringBLL.GetTransaction(new KpiMonitoringGetByParamInput());
                return Mapper.Map<List<KpiMonitoringItem>>(data);
            }

            //getbyparams
            var input = Mapper.Map<KpiMonitoringGetByParamInput>(filter);

            var dbData = _kpiMonitoringBLL.GetTransaction(input);
            return Mapper.Map<List<KpiMonitoringItem>>(dbData);
        }

        public KpiMonitoringItem GetDateworkflow(KpiMonitoringItem item, List<Login> UserLogin)
        {
            if (item.FormType.ToUpper() == "CTF")
            {
                var ListWorkflow=  GetWorkflowHistory((int)Enums.MenuList.TraCtf, item.TraId);
                var traCtf = _ctfBLL.GetCtfById(item.TraId);
                
                var Employee = UserLogin.Where(x=> x.EMPLOYEE_ID ==  (traCtf == null ? "" : traCtf.EmployeeId )).FirstOrDefault();
                var Creator = UserLogin.Where(x => x.EMPLOYEE_ID == (traCtf == null ? "" : traCtf.EmployeeIdCreator)).FirstOrDefault();
                var Fleet  = UserLogin.Where(x => x.EMPLOYEE_ID == (traCtf == null ? "" : traCtf.EmployeeIdFleetApproval)).FirstOrDefault();

                var SendToEmp = ListWorkflow.Where(x => x.Action == Enums.ActionType.Submit.ToString() && x.UserId == (Creator == null ? "" : Creator.USER_ID)).FirstOrDefault() ;
                if (SendToEmp != null)
                {
                    item.SendToEmpDate = SendToEmp.ActionDate;
                    var DaysDiff = _kpiMonitoringBLL.GetDifferentDays(item.EffectiveDate, SendToEmp.ActionDate);
                    if (DaysDiff != null) item.Kpi1 = DaysDiff;
                }

                if ((traCtf.VehicleUsage == null ? "" : traCtf.VehicleUsage.ToUpper()) == "COP")
                {
                    var SendBackToHr = ListWorkflow.Where(x => x.Action == Enums.ActionType.Submit.ToString() && x.UserId == (Employee == null ? "" : Employee.USER_ID)).FirstOrDefault();
                    if (SendBackToHr != null) item.SendBackToHr = SendBackToHr.ActionDate;

                    var SendToFleet = ListWorkflow.Where(x => x.Action == Enums.ActionType.Approve.ToString() && x.UserId == (Creator == null ? "" : Creator.USER_ID)).FirstOrDefault();
                    if (SendToFleet != null) item.SendToFleetDate = SendToFleet.ActionDate;
                }
                else if ((traCtf.VehicleUsage == null ? "" : traCtf.VehicleUsage.ToUpper()) == "CFM")
                {
                    var SendToFleet = ListWorkflow.Where(x => x.Action == Enums.ActionType.Submit.ToString() && x.UserId == (Employee == null ? "" : Employee.USER_ID)).FirstOrDefault();
                    if (SendToFleet != null) item.SendToFleetDate = SendToFleet.ActionDate;
                }
            }
            else if (item.FormType.ToUpper() == "CSF")
            {
                var ListWorkflow = GetWorkflowHistory((int)Enums.MenuList.TraCsf, item.TraId);
                var traCsf = _csfBLL.GetCsfById(item.TraId);

                var Employee = UserLogin.Where(x => x.EMPLOYEE_ID == (traCsf == null ? "" : traCsf.EMPLOYEE_ID)).FirstOrDefault();
                var Creator = UserLogin.Where(x => x.EMPLOYEE_ID == (traCsf == null ? "" : traCsf.EMPLOYEE_ID_CREATOR)).FirstOrDefault();
                var Fleet = UserLogin.Where(x => x.EMPLOYEE_ID == (traCsf == null ? "" : traCsf.EMPLOYEE_ID_FLEET_APPROVAL)).FirstOrDefault();

                var SendToEmp = ListWorkflow.Where(x => x.Action == Enums.ActionType.Submit.ToString() && x.UserId == (Creator == null ? "" : Creator.USER_ID)).FirstOrDefault();
                if (SendToEmp != null)
                {
                    item.SendToEmpDate = SendToEmp.ActionDate;
                    var DaysDiff = _kpiMonitoringBLL.GetDifferentDays(item.EffectiveDate, SendToEmp.ActionDate);
                    if (DaysDiff != null) item.Kpi1 = DaysDiff;
                }
             
                var SendBackToHr = ListWorkflow.Where(x => x.Action == Enums.ActionType.Submit.ToString() && x.UserId == (Employee == null ? "" : Employee.USER_ID)).FirstOrDefault();
                if (SendBackToHr != null) item.SendBackToHr = SendBackToHr.ActionDate;

                var SendToFleet = ListWorkflow.Where(x => x.Action == Enums.ActionType.Approve.ToString() && x.UserId == (Creator == null ? "" : Creator.USER_ID)).FirstOrDefault();
                if (SendToFleet != null) item.SendToFleetDate = SendToFleet.ActionDate;

                var SendVehicleDate = traCsf == null ? null : traCsf.VENDOR_CONTRACT_START_DATE;
                if (SendVehicleDate != null) item.SendToEmpBenefit = SendVehicleDate;
              
            }
            else if (item.FormType.ToUpper() == "CRF")
            {
                var ListWorkflow = GetWorkflowHistory((int)Enums.MenuList.TraCtf, item.TraId);
                var TraCrf = _crfBLL.GetDataById(item.TraId);

                var Employee = UserLogin.Where(x => x.EMPLOYEE_ID == (TraCrf == null ? "" : TraCrf.EMPLOYEE_ID)).FirstOrDefault();
                //var Fleet = UserLogin.Where(x => x.EMPLOYEE_ID == (TraCrf == null ? "" : TraCrf.EmployeeIdFleetApproval)).FirstOrDefault();

                var SendToEmp = ListWorkflow.Where(x => x.Action == Enums.ActionType.Submit.ToString() && x.UserId == (TraCrf == null ? "" : TraCrf.CREATED_BY)).FirstOrDefault();
                if (SendToEmp != null)
                {
                    item.SendToEmpDate = SendToEmp.ActionDate;
                    var DaysDiff = _kpiMonitoringBLL.GetDifferentDays(item.EffectiveDate, SendToEmp.ActionDate);
                    if (DaysDiff != null) item.Kpi1 = DaysDiff;
                } 

                var SendBackToHr = ListWorkflow.Where(x => x.Action == Enums.ActionType.Submit.ToString() && x.UserId == (Employee == null ? "" : Employee.USER_ID)).FirstOrDefault();
                if (SendBackToHr != null) item.SendBackToHr = SendBackToHr.ActionDate;

                //var SendToFleet = ListWorkflow.Where(x => x.Action == Enums.ActionType.Approve.ToString() && x.UserId == (Creator == null ? "" : Creator.USER_ID)).FirstOrDefault();
                //if (SendToFleet != null) item.SendToFleetDate = SendToFleet.ActionDate;
                
            }

            return item;
        }

        [HttpPost]
        public PartialViewResult ListTransaction(RptKpiMonitoringModel model)
        {
            try
            {
                var ListTransaction = GetTransaction(model.SearchView);
                var UserLogin =GetUserLogin();
                foreach (var item in ListTransaction)
                {
                    var data = GetDateworkflow(item, UserLogin);
                    model.ListTransaction.Add(data);
                }
            }
            catch (Exception exp)
            {

                model.ErrorMessage = exp.Message;
            }

            return PartialView("_ListTransaction", model);
        }

        private List<Login> GetUserLogin()
        {
            var typeEnv = ConfigurationManager.AppSettings["Environment"];
            var serverIntranet = ConfigurationManager.AppSettings["ServerIntranet"];

            var UserLogin = new List<Login>();
            string LoginQuery = string.Empty;

            LoginQuery = "SELECT ID,FULL_NAME FROM " + serverIntranet + ".[dbo].[tbl_ADSI_User]";

            if (typeEnv == "VTI")
            {
                LoginQuery = "SELECT ID,FULL_NAME FROM EMAIL_FOR_VTI";
            }

            EntityConnectionStringBuilder e = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["FMSEntities"].ConnectionString);
            string connectionString = e.ProviderConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand query = new SqlCommand(LoginQuery, con);
            SqlDataReader reader = query.ExecuteReader();
            while (reader.Read())
            {

                var LoginData = new Login();

                var employeeId = reader[0].ToString();
                if (employeeId != "")
                {
                    employeeId = employeeId.Replace("ID", "");
                    try
                    {
                        LoginData.EMPLOYEE_ID = Convert.ToInt32(employeeId).ToString("00000000");
                    }
                    catch (Exception)
                    {
                        LoginData.EMPLOYEE_ID = employeeId;
                    }
                }

                var UserId = reader[1].ToString();
                if (UserId != "")
                {
                    UserId = UserId.Replace("PMI\\", "");
                    LoginData.USER_ID = UserId;
                }
                UserLogin.Add(LoginData);
            }
            con.Close();

            return UserLogin;
        }

        #region -------------- Export Excel ------------------
        public void ExportKpiMonitoring(RptKpiMonitoringModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsKpiMonitoring(model);

            var newFile = new FileInfo(pathFile);

            var fileName = Path.GetFileName(pathFile);

            string attachment = string.Format("attachment; filename={0}", fileName);
            Response.Clear();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.WriteFile(newFile.FullName);
            Response.Flush();
            newFile.Delete();
            Response.End();
        }
        private string CreateXlsKpiMonitoring(RptKpiMonitoringModel model)
        {

            var ListTransaction = GetTransaction(model.SearchView);
            var UserLogin = GetUserLogin();
            foreach (var item in ListTransaction)
            {
                var data = GetDateworkflow(item, UserLogin);
                model.ListTransaction.Add(data);
            }

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "KPI MONITORING");
            slDocument.MergeWorksheetCells(1, 1, 1, 19);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelKpiMonitoring(slDocument);

            //create data
            slDocument = CreateDataExcelKpiMonitoring(slDocument, model.ListTransaction);

            var fileName = "Kpi_Monitoring" + DateTime.Now.ToString(" yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }
        private SLDocument CreateHeaderExcelKpiMonitoring(SLDocument slDocument)
        {
            int iRow = 2;
            slDocument.SetCellValue(iRow, 1, "FORM TYPE");
            slDocument.SetCellValue(iRow, 2, "EMPLOYEE ID");
            slDocument.SetCellValue(iRow, 3, "EMPLOYEE NAME");
            slDocument.SetCellValue(iRow, 4, "EFFECTIVE DATE");
            slDocument.SetCellValue(iRow, 5, "REASON");
            slDocument.SetCellValue(iRow, 6, "ADDRESS");
            slDocument.SetCellValue(iRow, 7, "PREVIOUS BASE TOWN");
            slDocument.SetCellValue(iRow, 8, "NEW BASE TOWN");
            slDocument.SetCellValue(iRow, 9, "VEHICLE USAGE");
            slDocument.SetCellValue(iRow, 10, "VEHICLE GROUP LEVEL");
            slDocument.SetCellValue(iRow, 11, "VEHICLE MODEL");
            slDocument.SetCellValue(iRow, 12, "COLOR");
            slDocument.SetCellValue(iRow, 13, "POLICE NUMBER");
            slDocument.SetCellValue(iRow, 14, "SEND TO EMP DATE");
            slDocument.SetCellValue(iRow, 15, "SEND BACK TO HR");
            slDocument.SetCellValue(iRow, 16, "DAYS DIFFERENCE");
            slDocument.SetCellValue(iRow, 17, "SEND TO FLEET DATE");
            slDocument.SetCellValue(iRow, 18, "SEND TO EMPLOYEE BENEFIT DATE");
            slDocument.SetCellValue(iRow, 19, "REMARK");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 19, headerStyle);

            return slDocument;

        }
        private SLDocument CreateDataExcelKpiMonitoring(SLDocument slDocument, List<KpiMonitoringItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.FormType);
                slDocument.SetCellValue(iRow, 2, data.EmployeeId);
                slDocument.SetCellValue(iRow, 3, data.EmployeeName);
                slDocument.SetCellValue(iRow, 4, data.EffectiveDate == null ? "":data.EffectiveDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 5, data.Reason);
                slDocument.SetCellValue(iRow, 6, data.Address);
                slDocument.SetCellValue(iRow, 7, data.PreviousBaseTown);
                slDocument.SetCellValue(iRow, 8, data.NewBaseTown);
                slDocument.SetCellValue(iRow, 9, data.VehicleUsage);
                slDocument.SetCellValue(iRow, 10, data.VehicleGroup == null ? "" : data.VehicleGroup.ToString());
                slDocument.SetCellValue(iRow, 11, data.Model);
                slDocument.SetCellValue(iRow, 12, data.Color);
                slDocument.SetCellValue(iRow, 13, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 14, data.SendToEmpDate == null ?"": data.SendToEmpDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 15, data.SendBackToHr == null ? "":data.SendBackToHr.Value.ToString("dd-MMM-yyyy") );
                slDocument.SetCellValue(iRow, 16, data.Kpi1 == null ? "" : data.Kpi1.Value.ToString());
                slDocument.SetCellValue(iRow, 17, data.SendToFleetDate == null ?"": data.SendToFleetDate.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 18, data.SendToEmpBenefit == null ? "":data.SendToEmpBenefit.Value.ToString("dd-MMM-yyyy"));
                slDocument.SetCellValue(iRow, 19, data.Remark);

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 19);
            slDocument.SetCellStyle(3, 1, iRow - 1, 19, valueStyle);

            return slDocument;
        }
        #endregion
    }
}
