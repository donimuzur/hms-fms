using AutoMapper;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.Core;
using FMS.Core.Exceptions;
using FMS.DAL.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.EntityClient;
using System.Threading.Tasks;
using FMS.Utils;
using System.Data;

namespace FMS.BLL.Ctf
{
    public class CtfBLL : ITraCtfBLL
    {
        private IUnitOfWork _uow;
        private ICtfService _ctfService;
        private IDocumentNumberService _docNumberService;
        private IWorkflowHistoryService _workflowService;
        private ISettingService _settingService;
        private IReasonService _reasonService;
        private IPenaltyService _penaltyService;
        private IPenaltyLogicService _penaltyLogicService;
        private IPriceListService _pricelistService;
        private IFleetService _fleetService;
        private IRemarkService _remarkService;
        private IMessageService _messageService;
        private IEmployeeService _employeeService;
        private IVendorService _vendorService;
        private ICtfExtendService _ctfExtendService;

        public CtfBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _ctfService = new CtfService(uow);
            _docNumberService = new DocumentNumberService(uow);
            _workflowService = new WorkflowHistoryService(uow);
            _settingService = new SettingService(uow);
            _reasonService = new ReasonService(uow);
            _remarkService = new RemarkService(uow);
            _penaltyLogicService = new PenaltyLogicService(uow);
            _pricelistService = new PriceListService(uow);
            _fleetService = new FleetService(uow);
            _messageService = new MessageService(_uow);
            _employeeService = new EmployeeService(_uow);
            _vendorService = new VendorService(_uow);
            _penaltyService = new PenaltyService(_uow);
            _ctfExtendService = new CtfExtendService(_uow);
        }
        public List<TraCtfDto> GetCtf()
        {
            var data = _ctfService.GetCtf();
            var redata = Mapper.Map<List<TraCtfDto>>(data);
            return redata;
        }
        public TraCtfDto GetCtfById(long id)
        {
            var data = _ctfService.GetCtfById(id);
            var retData = Mapper.Map<TraCtfDto>(data);
            return retData;
        }
        public List<TraCtfDto> GetCtfDashboard(Login userLogin, bool isCompleted)
        {
            var settingData = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
            var benefitType = settingData.Where(x => x.SETTING_NAME.ToUpper() == "BENEFIT").FirstOrDefault().SETTING_NAME;
            var wtcType = settingData.Where(x => x.SETTING_NAME.ToUpper() == "WTC").FirstOrDefault().SETTING_NAME;

            var data = _ctfService.GetCtfDashboard(userLogin, isCompleted, benefitType, wtcType);
            var retData = Mapper.Map<List<TraCtfDto>>(data);
            return retData;
        }
        public List<TraCtfDto> GetCtfPersonal(Login userLogin)
        {
            var data = _ctfService.GetCtf().Where(x => (x.EMPLOYEE_ID == userLogin.EMPLOYEE_ID || x.EMPLOYEE_ID_CREATOR == userLogin.EMPLOYEE_ID || x.EMPLOYEE_ID_FLEET_APPROVAL == userLogin.EMPLOYEE_ID)).ToList();
            var retData = Mapper.Map<List<TraCtfDto>>(data);
            return retData;
        }
        public TraCtfDto Save(TraCtfDto Dto, Login userLogin)
        {
            TRA_CTF dbTraCtf;
            if (Dto == null)
            {
                throw new Exception("Invalid Data Entry");
            }
            try
            {
                bool changed = false;

                if (Dto.TraCtfId> 0)
                {
                    //update
                   var Exist = _ctfService.GetCtf().Where(c => c.TRA_CTF_ID == Dto.TraCtfId).FirstOrDefault();

                    if (Exist== null)
                        throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                    //changed = SetChangesHistory(model, item, userId);
                    dbTraCtf =Mapper.Map<TRA_CTF>(Dto);
                    _ctfService.Save(dbTraCtf, userLogin);
                }
                else
                {
                    var inputDoc = new GenerateDocNumberInput();
                    inputDoc.Month = DateTime.Now.Month;
                    inputDoc.Year = DateTime.Now.Year;
                    inputDoc.DocType = (int)Enums.DocumentType.CTF;

                    Dto.DocumentNumber = _docNumberService.GenerateNumber(inputDoc);

                   dbTraCtf= Mapper.Map<TRA_CTF>(Dto);
                    _ctfService.Save(dbTraCtf, userLogin);

                }
                var input = new CtfWorkflowDocumentInput()
                {
                    DocumentId = dbTraCtf.TRA_CTF_ID,
                    ActionType = Enums.ActionType.Modified,
                    UserId = userLogin.USER_ID
                };
                if (changed)
                {
                    AddWorkflowHistory(input);
                }
                _uow.SaveChanges();
            }
            catch (Exception exception)
            {
                throw exception;
            }
            var data = _ctfService.GetCtf().Where(x => x.DOCUMENT_NUMBER == Dto.DocumentNumber).FirstOrDefault();
            Dto = Mapper.Map<TraCtfDto>(data);
            return Dto;
        }
        private string ReplaceToValue(TraCtfDto CtfDto,MST_FLEET Fleet, string Value)
        {
            if (Value.ToUpper().Contains("MST_FLEET.END_CONTRACT"))
            {
                var EndContract = (int)(Fleet.END_CONTRACT.Value.Date - new DateTime(1900, 1, 1)).TotalDays + 2;
                Value = Value.Replace("MST_FLEET.END_CONTRACT", EndContract.ToString());
            }
            if (Value.ToUpper().Contains("MST_FLEET.MONTHLY_HMS_INSTALLMENT"))
            {
                Value = Value.Replace("MST_FLEET.MONTHLY_HMS_INSTALLMENT", Fleet.MONTHLY_HMS_INSTALLMENT.ToString());
            }
            if (Value.ToUpper().Contains("TRA_CTF.EFFECTIVE_DATE"))
            {
                var EffectiveDate = (int)(CtfDto.EffectiveDate.Value.Date - new DateTime(1900, 1, 1)).TotalDays + 2;
                Value = Value.Replace("TRA_CTF.EFFECTIVE_DATE", EffectiveDate.ToString());
            }
            return Value;
        }
        public decimal? PenaltyCost (TraCtfDto CtfDto)
        {
            decimal? cost = 0;
            if (CtfDto == null)
               return 0;
            var fleetData = _fleetService.GetFleet().Where(x => x.POLICE_NUMBER == CtfDto.PoliceNumber && x.EMPLOYEE_ID == CtfDto.EmployeeId && x.IS_ACTIVE).FirstOrDefault();

            var rentMonth = ((fleetData.END_CONTRACT.Value.Year - fleetData.START_CONTRACT.Value.Year) * 12) + fleetData.END_CONTRACT.Value.Month - fleetData.START_CONTRACT.Value.Month;

            var Vendor = _vendorService.GetVendor().Where(x => x.VENDOR_NAME == fleetData.VENDOR_NAME && x.IS_ACTIVE).FirstOrDefault();

            var penalty = _penaltyService.GetPenalty().Where(x => x.MANUFACTURER.Contains(fleetData.MANUFACTURER) && x.MODEL.Contains(fleetData.MODEL) && x.SERIES.Contains(fleetData.SERIES)&& x.BODY_TYPE == fleetData.BODY_TYPE && x.YEAR == fleetData.VEHICLE_YEAR
                                                        && x.VENDOR == Vendor.MST_VENDOR_ID && x.VEHICLE_TYPE.Contains(fleetData.VEHICLE_TYPE) && x.MONTH_START <= rentMonth && x.MONTH_END >= rentMonth && x.IS_ACTIVE).FirstOrDefault();
            if (penalty == null)
            {
                penalty = _penaltyService.GetPenalty().Where(x => x.BODY_TYPE.Contains(fleetData.BODY_TYPE)  && x.VEHICLE_TYPE.Contains(fleetData.VEHICLE_TYPE) && x.YEAR == fleetData.VEHICLE_YEAR
                                                        && x.VENDOR == Vendor.MST_VENDOR_ID && x.MONTH_START <= rentMonth && x.MONTH_END >= rentMonth && x.IS_ACTIVE).FirstOrDefault();
            }

            if (penalty == null)
            {
                penalty = _penaltyService.GetPenalty().Where(x => x.VEHICLE_TYPE.Contains(fleetData.VEHICLE_TYPE) && x.YEAR == fleetData.VEHICLE_YEAR
                                                        && x.VENDOR == Vendor.MST_VENDOR_ID && x.MONTH_START <= rentMonth && x.MONTH_END >= rentMonth && x.IS_ACTIVE).FirstOrDefault();
            }

            if (penalty == null)
            {
                return null;
            }

            var PenaltyLogic = _penaltyLogicService.GetPenaltyLogicByID(penalty.PENALTY.Value).PENALTY_LOGIC;

            try
            {
                PenaltyLogic = ReplaceToValue(CtfDto, fleetData, PenaltyLogic);
                string value = new DataTable().Compute(PenaltyLogic, null).ToString();
                cost = Convert.ToDecimal(value);
            }
            catch (Exception exp)
            {
                var msg = exp.Message;
                return 0;
            }
            return cost;
           
        }
        public decimal? RefundCost(TraCtfDto CtfDto)
        {
            decimal? cost = 0;
            var fleet = _fleetService.GetFleet().Where(x => x.EMPLOYEE_ID == CtfDto.EmployeeId && x.POLICE_NUMBER == CtfDto.PoliceNumber && x.IS_ACTIVE).FirstOrDefault();
            var Vendor = _vendorService.GetVendor().Where(x => x.VENDOR_NAME == fleet.VENDOR_NAME && x.IS_ACTIVE).FirstOrDefault();

            if (fleet == null) return 0;

            var installmentEmp = _pricelistService.GetPriceList().Where(x => x.MANUFACTURER.Contains(fleet.MANUFACTURER) && x.MODEL.Contains(fleet.MODEL) && x.SERIES.Contains(fleet.SERIES) 
                                            && x.VEHICLE_TYPE.Contains(fleet.VEHICLE_TYPE) && x.VEHICLE_USAGE.Contains(fleet.VEHICLE_USAGE) && x.YEAR==fleet.VEHICLE_YEAR && x.VENDOR == Vendor.MST_VENDOR_ID  && x.IS_ACTIVE == true).FirstOrDefault();
                
            if (installmentEmp == null) return 0;
            
            var rentMonth = ((fleet.END_CONTRACT.Value.Year - fleet.START_CONTRACT.Value.Year) * 12) + fleet.END_CONTRACT.Value.Month - fleet.START_CONTRACT.Value.Month;

            if (CtfDto.IsPenalty)
            {
                cost = (rentMonth * installmentEmp.INSTALLMEN_EMP) - CtfDto.Penalty.Value;
            }
            else
            {
                cost = (rentMonth * installmentEmp.INSTALLMEN_EMP ) ;
            }
            return cost;

        }
        public decimal? EmployeeContribution(TraCtfDto CtfDto)
        {
            decimal? cost = 0;
            var fleet = _fleetService.GetFleet().Where(x => x.EMPLOYEE_ID == CtfDto.EmployeeId && x.POLICE_NUMBER == CtfDto.PoliceNumber && x.IS_ACTIVE).FirstOrDefault();
            var Vendor = _vendorService.GetVendor().Where(x => x.VENDOR_NAME == fleet.VENDOR_NAME && x.IS_ACTIVE).FirstOrDefault();

            if (fleet == null) return 0;

            var Price = _pricelistService.GetPriceList().Where(x => x.MANUFACTURER.Contains(fleet.MANUFACTURER) && x.MODEL.Contains(fleet.MODEL) && x.SERIES.Contains(fleet.SERIES)
                                            && x.VEHICLE_TYPE.Contains(fleet.VEHICLE_TYPE) && x.VEHICLE_USAGE.Contains(fleet.VEHICLE_USAGE) && x.YEAR == fleet.VEHICLE_YEAR && x.VENDOR == Vendor.MST_VENDOR_ID && x.IS_ACTIVE == true).FirstOrDefault();

            if (Price == null) return 0;
            cost = Price.INSTALLMEN_EMP;
            return cost;
        }
        public void CtfWorkflow(CtfWorkflowDocumentInput input)
        {
            var isNeedSendNotif = true;

            switch (input.ActionType)
            {
                case Enums.ActionType.Created:
                    CreateDocument(input);
                    isNeedSendNotif = false;
                    break;
                case Enums.ActionType.Submit:
                    SubmitDocument(input);
                    break;
                case Enums.ActionType.Approve:
                    ApproveDocument(input);
                    break;
                case Enums.ActionType.Reject:
                    RejectDocument(input);
                    break;
                case Enums.ActionType.Completed:
                    CompleteDocument(input);
                    break;
                case Enums.ActionType.Cancel:
                    var Ctf=_ctfService.GetCtfById(input.DocumentId);
                    CancelDocument(input);
                    if(Ctf.DOCUMENT_STATUS == Enums.DocumentStatus.Draft) isNeedSendNotif = false;
                    break;
                case Enums.ActionType.Extend:
                    ExtendDocument(input);
                    break;
            }
            //todo sent mail
            if (isNeedSendNotif)SendEmailWorkflow(input);

            _uow.SaveChanges();
        }
        private void SendEmailWorkflow(CtfWorkflowDocumentInput input)
        {
            var ctfData = Mapper.Map<TraCtfDto>(_ctfService.GetCtfById(input.DocumentId));

            var mailProcess = ProsesMailNotificationBody(ctfData, input);

            //distinct double To email
            List<string> ListTo = mailProcess.To.Distinct().ToList();

            if (mailProcess.IsCCExist)
                //Send email with CC
                _messageService.SendEmailToListWithCC(ListTo, mailProcess.CC, mailProcess.Subject, mailProcess.Body, true);
            else
                _messageService.SendEmailToList(ListTo, mailProcess.Subject, mailProcess.Body, true);

        }
        private class CtfMailNotification
        {
            public CtfMailNotification()
            {
                To = new List<string>();
                CC = new List<string>();
                IsCCExist = false;
            }
            public string Subject { get; set; }
            public string Body { get; set; }
            public List<string> To { get; set; }
            public List<string> CC { get; set; }
            public bool IsCCExist { get; set; }
        }
        private CtfMailNotification ProsesMailNotificationBody(TraCtfDto ctfData, CtfWorkflowDocumentInput input)
        {
            var bodyMail = new StringBuilder();
            var rc = new CtfMailNotification();

            var fleetdata = _fleetService.GetFleet().Where(x => x.POLICE_NUMBER == ctfData.PoliceNumber).FirstOrDefault();
            var vendor = _vendorService.GetVendor().Where(x => x.VENDOR_NAME == fleetdata.VENDOR_NAME).FirstOrDefault();
            var vehTypeBenefit = _settingService.GetSetting().Where(x => x.SETTING_GROUP == "VEHICLE_TYPE" && x.SETTING_NAME == "BENEFIT").FirstOrDefault().SETTING_NAME;

            var isBenefit = ctfData.VehicleType == vehTypeBenefit.ToString() ? true : false;

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
            var typeEnv = ConfigurationManager.AppSettings["Environment"];
            
            var userData = _employeeService.GetEmployeeById(input.EmployeeId);
            var employeeData = _employeeService.GetEmployeeById(ctfData.EmployeeId);
            var creatorData = _employeeService.GetEmployeeById(ctfData.EmployeeIdCreator);
            var fleetApprovalData = _employeeService.GetEmployeeById(ctfData.EmployeeIdFleetApproval);

            var employeeDataEmail = employeeData == null ? string.Empty : employeeData.EMAIL_ADDRESS;
            var creatorDataEmail = creatorData == null ? string.Empty : creatorData.EMAIL_ADDRESS;
            var vendorDataEmail = vendor == null ? string.Empty : vendor.EMAIL_ADDRESS;
            var userDataEmail = userData == null ? string.Empty : userData.EMAIL_ADDRESS;

            var extend = _ctfExtendService.GetCtfExtend().Where(x => x.TRA_CTF_ID == ctfData.TraCtfId ).FirstOrDefault();

            var employeeDataName = employeeData == null ? string.Empty : employeeData.FORMAL_NAME;
            var creatorDataName = creatorData == null ? string.Empty : creatorData.FORMAL_NAME;
            var fleetApprovalDataName = fleetApprovalData == null ? string.Empty : fleetApprovalData.FORMAL_NAME;
            var vendorDataName = vendor == null ? string.Empty : vendor.VENDOR_NAME;
            var userDataName = userData == null ? string.Empty : userData.FORMAL_NAME;

            var hrList = string.Empty;
            var fleetList = string.Empty;

            var hrEmailList = new List<string>();
            var fleetEmailList = new List<string>();

            var hrRole = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                && x.SETTING_VALUE.Contains("HR")).FirstOrDefault().SETTING_VALUE;
            var fleetRole = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                && x.SETTING_VALUE.Contains("FLEET")).FirstOrDefault().SETTING_VALUE;

            var hrQuery = "SELECT 'PMI\\' + sAMAccountName AS sAMAccountName FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + hrRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";
            var fleetQuery = "SELECT 'PMI\\' + sAMAccountName AS sAMAccountName FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + fleetRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";

            if (typeEnv == "VTI")
            {
                hrQuery = "SELECT EMPLOYEE_ID FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + hrRole + "'";
                fleetQuery = "SELECT EMPLOYEE_ID FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + fleetRole + "'";
            }

            EntityConnectionStringBuilder e = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["FMSEntities"].ConnectionString);
            string connectionString = e.ProviderConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand query = new SqlCommand(hrQuery, con);
            SqlDataReader reader = query.ExecuteReader();
            while (reader.Read())
            {
                var hrLogin = "'" + reader[0].ToString() + "',";
                hrList += hrLogin;
            }

            hrList = hrList.TrimEnd(',');

            query = new SqlCommand(fleetQuery, con);
            reader = query.ExecuteReader();
            while (reader.Read())
            {
                var fleetLogin = "'" + reader[0].ToString() + "',";
                fleetList += fleetLogin;
            }

            fleetList = fleetList.TrimEnd(',');

            var hrQueryEmail = "SELECT EMAIL FROM [HMSSQLFWOPRD.ID.PMI\\PRD03].[db_Intranet_HRDV2].[dbo].[tbl_ADSI_User] WHERE FULL_NAME IN (" + hrList + ")";
            var fleetQueryEmail = "SELECT EMAIL FROM [HMSSQLFWOPRD.ID.PMI\\PRD03].[db_Intranet_HRDV2].[dbo].[tbl_ADSI_User] WHERE FULL_NAME IN (" + fleetList + ")";

            if (typeEnv == "VTI")
            {
                hrQueryEmail = "SELECT EMAIL FROM EMAIL_FOR_VTI WHERE FULL_NAME IN (" + hrList + ")";
                fleetQueryEmail = "SELECT EMAIL FROM EMAIL_FOR_VTI WHERE FULL_NAME IN (" + fleetList + ")";
            }

            query = new SqlCommand(hrQueryEmail, con);
            reader = query.ExecuteReader();
            while (reader.Read())
            {
                hrEmailList.Add(reader[0].ToString());
            }

            query = new SqlCommand(fleetQueryEmail, con);
            reader = query.ExecuteReader();
            while (reader.Read())
            {
                fleetEmailList.Add(reader[0].ToString());
            }

            reader.Close();
            con.Close();

            switch (input.ActionType)
            {
                case Enums.ActionType.Submit:

                    //if submit from HR to EMPLOYEE
                    if (ctfData.EmployeeIdCreator == input.EmployeeId && isBenefit)
                    {
                        rc.Subject = ctfData.DocumentNumber + " - Car Termination";

                        bodyMail.Append("Dear " + ctfData.EmployeeName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Here is vehicle data which terminated for the below reason<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("'" + _reasonService.GetReasonById(ctfData.Reason.Value).REASON + "'<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Please confirm for the vehicle, and fill the information for Withdrawal <a href='" + webRootUrl + "/TraCtf/Edit?TraCtfId=" + ctfData.TraCtfId + "&isPersonalDashboard=True" + "'>HERE</a><br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("For any assistance please contact " + creatorDataName + " <br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("HR Team <br /><br />");
                        bodyMail.AppendLine();

                        rc.To.Add(employeeDataEmail);

                        foreach (var item in hrEmailList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    //if submit from FLEET to EMPLOYEE
                    else if (ctfData.EmployeeIdCreator == input.EmployeeId && !isBenefit && !input.EndRent.Value)
                    {

                        rc.Subject = ctfData.DocumentNumber + " - Car Termination";

                        bodyMail.Append("Dear " + ctfData.EmployeeName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Here is vehicle data which terminated for the below reason<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("'" + _reasonService.GetReasonById(ctfData.Reason.Value).REASON + "'<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Please confirm for the vehicle, and fill the information for Withdrawal <a href='" + webRootUrl + "/TraCtf/Edit?TraCtfId=" + ctfData.TraCtfId + "&isPersonalDashboard=True" + "'>HERE</a><br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("For any assistance please contact " + creatorDataName + " <br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team <br /><br />");
                        bodyMail.AppendLine();
                        
                        rc.To.Add(employeeDataEmail);

                        foreach (var item in fleetEmailList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    //if submit from FLEET to EMPLOYEE WTC END RENT
                    else if (ctfData.EmployeeIdCreator == input.EmployeeId && !isBenefit && input.EndRent.Value)
                    {
                        rc.Subject = ctfData.DocumentNumber + " -  Car Termination";

                        bodyMail.Append("Dear " + ctfData.EmployeeName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("You have received new Car Termination Form<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("See the Details Informations, by clicking below CTF number:<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("<a href='" + webRootUrl + "/TraCtf/Edit?TraCtfId=" + ctfData.TraCtfId + "&isPersonalDashboard=True" + "'>" + ctfData.DocumentNumber + "</a> requested by " + creatorDataName + "<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team");
                        bodyMail.AppendLine();

                        rc.To.Add(employeeDataEmail);
                        foreach (var item in fleetEmailList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    //if submit from EMPLOYEE to Fleet Benefit
                    else if (ctfData.EmployeeId == input.EmployeeId && isBenefit)
                    {
                        rc.Subject = ctfData.DocumentNumber+" -  Car Termination";

                        bodyMail.Append("Dear " + creatorDataName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("You have received new Car Termination Form<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Send confirmation by clicking below CTF number:<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("<a href='" + webRootUrl + "/TraCtf/Edit?TraCtfId=" + ctfData.TraCtfId + "&isPersonalDashboard=False" + "'>" + ctfData.DocumentNumber + "</a> requested by " + ctfData.EmployeeName + "<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();

                        rc.To.Add(creatorDataEmail);

                        foreach (var item in fleetEmailList)
                        {
                            rc.CC.Add(item);
                        }
                        foreach (var item in hrEmailList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    //if submit from EMPLOYEE to Fleet WTC
                    else if (ctfData.EmployeeId == input.EmployeeId && !isBenefit)
                    {
                        rc.Subject = ctfData.DocumentNumber + " -  Car Termination";

                        bodyMail.Append("Dear " + creatorDataName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("You have received new Car Termination Form<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Send confirmation by clicking below CTF number:<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("<a href='" + webRootUrl + "/TraCtf/Edit?TraCtfId=" + ctfData.TraCtfId + "&isPersonalDashboard=False" + "'>" + ctfData.DocumentNumber + "</a> requested by " + ctfData.EmployeeName + "<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();

                        rc.To.Add(creatorDataEmail);
                        foreach (var item in fleetEmailList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Approve:

                    //if Fleet Approve for benefit
                    if (input.UserRole == Enums.UserRole.Fleet && isBenefit)
                    {
                        rc.Subject = ctfData.DocumentNumber + " - Car Termination";

                        bodyMail.Append("Dear " + ctfData.EmployeeName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Your Car Termination Form " + ctfData.DocumentNumber + " has been approved by " + fleetApprovalDataName + "<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Click <a href='" + webRootUrl + "/TraCtf/DetailsBenefit?TraCtfId=" + ctfData.TraCtfId + "&isPersonalDashboard=False" + "'>HERE</a> to monitor your request<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team");
                        bodyMail.AppendLine();

                        rc.To.Add(employeeDataEmail);
                        foreach (var item in fleetEmailList)
                        {
                            rc.CC.Add(item);
                        }
                        foreach (var item in hrEmailList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    //if Fleet Approve for wtc
                    else if (input.UserRole == Enums.UserRole.Fleet && !isBenefit)
                    {
                        rc.Subject = ctfData.DocumentNumber + " - Car Termination";

                        bodyMail.Append("Dear " + ctfData.EmployeeName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Your Car Termination Form " + ctfData.DocumentNumber + " has been approved by " + fleetApprovalDataName + "<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Click <a href='" + webRootUrl + "/TraCtf/DetailsWTC?TraCtfId=" + ctfData.TraCtfId + "&isPersonalDashboard=False" + "'>HERE</a> to monitor your request<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team");
                        bodyMail.AppendLine();

                        rc.To.Add(employeeDataEmail);

                        foreach (var item in fleetEmailList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Reject:

                    if (input.UserRole == Enums.UserRole.Fleet && isBenefit)
                    {
                        rc.Subject = ctfData.DocumentNumber + " - Car Termination";

                        bodyMail.Append("Dear " + ctfData.EmployeeName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Your Document " + ctfData.DocumentNumber + " has been rejected by " + fleetApprovalDataName+ " for below reason : " + _remarkService.GetRemarkById(input.Comment.Value).REMARK + "<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Please revised and re-submit your request <a href='" + webRootUrl + "/TraCtf/Edit?TraCtfId=" + ctfData.TraCtfId + "&isPersonalDashboard=True" + "'>HERE</a><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team");
                        bodyMail.AppendLine();

                        rc.To.Add(employeeDataEmail);

                        foreach (var item in fleetEmailList)
                        {
                            rc.CC.Add(item);
                        }
                        foreach (var item in hrEmailList)
                        {
                            rc.CC.Add(item);
                        }
                        rc.IsCCExist = true;
                    }
                    //if Fleet Reject Benefit
                    else if (input.UserRole == Enums.UserRole.Fleet && !isBenefit)
                    {
                        rc.Subject = ctfData.DocumentNumber + "- Car Termination";

                        bodyMail.Append("Dear " + ctfData.EmployeeName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Your Document " + ctfData.DocumentNumber + " has been rejected by " + fleetApprovalDataName + " for below reason : " + _remarkService.GetRemarkById(input.Comment.Value).REMARK + "<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Please revised and re-submit your request <a href='" + webRootUrl + "/TraCtf/Edit?TraCtfId=" + ctfData.TraCtfId + "&isPersonalDashboard=True" + "'>HERE</a><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team");
                        bodyMail.AppendLine();

                        rc.To.Add(employeeDataEmail);

                        foreach (var item in fleetEmailList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Completed:
                    if (!isBenefit && !ctfData.ExtendVehicle.Value)
                    {
                        rc.Subject = ctfData.DocumentNumber + "- Car Termination";

                        bodyMail.Append("Dear " + ctfData.EmployeeName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("We would like to inform you that the below vehicle was terminated and the status in FMS was INACTIVE <br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("No CTF : <a href='" + webRootUrl + "/TraCtf/Edit?TraCtfId=" + ctfData.TraCtfId + "&isPersonalDashboard=False" + "'>" + ctfData.DocumentNumber + "</a><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Police Number : " + fleetdata.POLICE_NUMBER + "<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Model : " + fleetdata.MODEL + "<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Function : " + fleetdata.VEHICLE_FUNCTION + "<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Reason : " + _reasonService.GetReasonById(ctfData.Reason.Value).REASON + " <br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Termination Date : " + ctfData.EffectiveDate + " <br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("For any assistance please contact " + creatorDataName + "<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks <br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team");
                        bodyMail.AppendLine();

                        rc.To.Add(employeeDataEmail);

                        foreach (var item in fleetEmailList)
                        {
                            rc.CC.Add(item);
                        }
                        rc.IsCCExist = true;
                    }
                    else if (isBenefit && !ctfData.ExtendVehicle.Value)
                    {
                        rc.Subject = ctfData.DocumentNumber + "- Car Termination";

                        bodyMail.Append("Dear " + ctfData.EmployeeName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("We would like to inform you that the below vehicle was terminated and the status in FMS was INACTIVE <br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("No CTF : <a href='" + webRootUrl + "/TraCtf/Edit?TraCtfId=" + ctfData.TraCtfId + "&isPersonalDashboard=False" + "'>" + ctfData.DocumentNumber + "</a><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Police Number : " + fleetdata.POLICE_NUMBER + "<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Model : " + fleetdata.MODEL + "<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Function : " + fleetdata.VEHICLE_FUNCTION + "<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Reason : " + _reasonService.GetReasonById(ctfData.Reason.Value).REASON + " <br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Termination Date : " + ctfData.EffectiveDate + " <br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("For any assistance please contact " + creatorDataName + "<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks <br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team");
                        bodyMail.AppendLine();

                        rc.To.Add(employeeDataEmail);

                        foreach (var item in fleetEmailList)
                        {
                            rc.CC.Add(item);
                        }
                        foreach (var item in hrEmailList)
                        {
                            rc.CC.Add(item);
                        }
                        rc.IsCCExist = true;
                    }
                    break;
                case Enums.ActionType.Cancel:
                    rc.Subject = ctfData.DocumentNumber + " - Car Termination";

                    bodyMail.Append("Dear " + ctfData.EmployeeName + ",<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Your Document " + ctfData.DocumentNumber + " has been Cancelled by " + userDataName + " for below reason : " + _remarkService.GetRemarkById(input.Comment.Value).REMARK + "<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Thanks<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Regards,<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Fleet Team");
                    bodyMail.AppendLine();

                    rc.To.Add(employeeDataEmail);

                    foreach (var item in fleetEmailList)
                    {
                        rc.CC.Add(item);
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Extend:
                    rc.Subject = ctfData.DocumentNumber + " - Car Termination";

                    bodyMail.Append("Dear " + ctfData.EmployeeName + ",<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Here is vehicle data which extended contract period. <br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("No CTF : <a href='" + webRootUrl + "/TraCtf/Edit?TraCtfId=" + ctfData.TraCtfId + "&isPersonalDashboard=False" + "'>" + ctfData.DocumentNumber + "</a><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Police Number : " + fleetdata.POLICE_NUMBER + "<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Model : " + fleetdata.MODEL + "<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Function : " + fleetdata.VEHICLE_FUNCTION + "<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Reason : " + _reasonService.GetReasonById(extend.REASON.Value).REASON + " <br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("New End Contract Date  : " + extend.NEW_PROPOSED_DATE + " <br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("For any assistance please contact " + creatorDataName + "<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Thanks <br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Regards,<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Fleet Team");
                    bodyMail.AppendLine();


                    rc.To.Add(employeeDataEmail);
                    rc.CC.Add(vendorDataEmail);
                    foreach (var item in fleetEmailList)
                    {
                        rc.CC.Add(item);
                    }
                    rc.IsCCExist = true;
                    break;
            }
            rc.Body = bodyMail.ToString();
            return rc;
        }
        private void CreateDocument(CtfWorkflowDocumentInput input)
        {
            var dbData = _ctfService.GetCtf().Where(x => x.TRA_CTF_ID== input.DocumentId).FirstOrDefault();

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);
        }
        private void AddWorkflowHistory(CtfWorkflowDocumentInput input)
        {
            var dbData = Mapper.Map<WorkflowHistoryDto>(input);

            dbData.ACTION_DATE = DateTime.Now;
            dbData.MODUL_ID = Enums.MenuList.TraCtf;
            dbData.ACTION = input.ActionType;
            dbData.REMARK_ID = input.Comment;


            _workflowService.Save(dbData);

        }
        public void CancelCtf(long id, int Remark, Login user)
        {
            _ctfService.CancelCtf(id, Remark, user);
        }
        private void CancelDocument(CtfWorkflowDocumentInput input)
        {
            var dbData = _ctfService.GetCtfById(input.DocumentId);
            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);
        }
        private void SubmitDocument(CtfWorkflowDocumentInput input)
        {
            var dbData = _ctfService.GetCtfById(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.Draft)
            {
                dbData.DOCUMENT_STATUS = Enums.DocumentStatus.AssignedForUser;
            }
            else if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.AssignedForUser)
            {
               dbData.DOCUMENT_STATUS = Enums.DocumentStatus.WaitingFleetApproval;
             
            }

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }
        private void RejectDocument(CtfWorkflowDocumentInput input)
        {
            var dbData = _ctfService.GetCtfById(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingFleetApproval)
            {
                dbData.DOCUMENT_STATUS = Enums.DocumentStatus.AssignedForUser;
            }

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }
        private void ApproveDocument(CtfWorkflowDocumentInput input)
        {
            var dbData = _ctfService.GetCtfById(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingFleetApproval)
            {
                dbData.DOCUMENT_STATUS = Enums.DocumentStatus.InProgress;
            }
            else if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.Draft && input.EndRent.Value)
            {
                input.ActionType = Enums.ActionType.Submit;
                dbData.DOCUMENT_STATUS = Enums.DocumentStatus.InProgress;
            }
            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }
        private void CompleteDocument(CtfWorkflowDocumentInput input)
        {
            var dbData = _ctfService.GetCtfById(input.DocumentId);

            dbData.MODIFIED_BY = input.UserId;
            dbData.MODIFIED_DATE = DateTime.Now;

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            dbData.DOCUMENT_STATUS = Enums.DocumentStatus.Completed;
            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }
        private void ExtendDocument(CtfWorkflowDocumentInput input)
        {
            var dbData = _ctfService.GetCtf().Where(x => x.TRA_CTF_ID == input.DocumentId).FirstOrDefault();

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }
        public void CheckCtfInProgress()
        {
            var dateMinus1 = DateTime.Today.AddDays(-1);

            var listCtfInProgress = _ctfService.GetCtf().Where(x => (x.DOCUMENT_STATUS == Enums.DocumentStatus.InProgress || x.DOCUMENT_STATUS == Enums.DocumentStatus.Extended)
                                                                        && x.EFFECTIVE_DATE.Value.Day <= dateMinus1.Day
                                                                        && x.EFFECTIVE_DATE.Value.Month <= dateMinus1.Month
                                                                        && x.EFFECTIVE_DATE.Value.Year <= dateMinus1.Year).ToList();

            foreach (var item in listCtfInProgress)
            {
                UpdateFleet(item.TRA_CTF_ID);
                
                _uow.SaveChanges();
            }
        }
        private  void UpdateFleet(long id)
        {
            var CtfData = _ctfService.GetCtfById(id);

            var vehicle = _fleetService.GetFleet().Where(x => x.POLICE_NUMBER == CtfData.POLICE_NUMBER && x.IS_ACTIVE && x.EMPLOYEE_ID == CtfData.EMPLOYEE_ID).FirstOrDefault();

            //change status completed
            var input = new CtfWorkflowDocumentInput();
            input.UserId = "SYSTEM";
            input.DocumentId = CtfData.TRA_CTF_ID;
            input.DocumentNumber = CtfData.DOCUMENT_NUMBER;
            input.Comment = null;
           
            //////////////////////////////////


            if (!CtfData.EXTEND_VEHICLE.Value)
            {
                if (vehicle != null)
                {
                    if (CtfData.IS_TRANSFER_TO_IDLE.Value)
                    {
                       
                        vehicle.IS_ACTIVE = false;
                        vehicle.MODIFIED_BY = "SYSTEM";
                        vehicle.MODIFIED_DATE = DateTime.Now;

                        _fleetService.save(vehicle);

                        var FleetDto = Mapper.Map<FleetDto>(vehicle);
                        
                        FleetDto.MstFleetId = 0;
                        FleetDto.EmployeeID = null;
                        FleetDto.EmployeeName = null;
                        FleetDto.AssignedTo = null;
                        FleetDto.EndDate = CtfData.EFFECTIVE_DATE;
                        FleetDto.VehicleStatus = "LIVE";
                        FleetDto.VehicleUsage = "CFM IDLE";
                        FleetDto.CreatedBy ="SYSTEM" ;
                        FleetDto.CreatedDate = DateTime.Now;
                        FleetDto.ModifiedBy = null;
                        FleetDto.ModifiedDate = null;

                        var IdleCar = Mapper.Map<MST_FLEET>(FleetDto);
                        _fleetService.save(IdleCar);
                        
                        input.ActionType = Enums.ActionType.Completed;
                        CtfWorkflow(input);
                    }
                    else
                    {
                        if (!CtfData.MST_REASON.IS_PENALTY || (CtfData.MST_REASON.IS_PENALTY && (CtfData.PENALTY_PRICE == 0 || CtfData.PENALTY_PRICE == null)))
                        {
                           
                            vehicle.IS_ACTIVE = false;
                            vehicle.MODIFIED_BY = "SYSTEM";
                            vehicle.MODIFIED_DATE = DateTime.Now;

                            _fleetService.save(vehicle);

                            var FleetDto = Mapper.Map<FleetDto>(vehicle);

                            FleetDto.CreatedBy = "SYSTEM";
                            FleetDto.CreatedDate = DateTime.Now;
                            FleetDto.ModifiedBy = null;
                            FleetDto.ModifiedDate = null;
                            FleetDto.VehicleStatus = "TERMINATE";
                            FleetDto.IsActive = false;
                            FleetDto.EndDate = CtfData.EFFECTIVE_DATE;
                            FleetDto.MstFleetId = 0;

                            var TerminateCar = Mapper.Map<MST_FLEET>(FleetDto);
                            _fleetService.save(TerminateCar);

                            input.ActionType = Enums.ActionType.Completed;
                            CtfWorkflow(input);

                        }
                        else if (CtfData.MST_REASON.IS_PENALTY  && (CtfData.PENALTY_PRICE != 0 && CtfData.PENALTY_PRICE != null) && (CtfData.PENALTY_PO_LINE != "" && CtfData.PENALTY_PO_LINE != null) && (CtfData.PENALTY_PO_NUMBER != "" && CtfData.PENALTY_PO_NUMBER != null))
                        {
                            
                            vehicle.IS_ACTIVE = false;
                            vehicle.MODIFIED_BY = "SYSTEM";
                            vehicle.MODIFIED_DATE = DateTime.Now;

                            _fleetService.save(vehicle);

                            var FleetDto = Mapper.Map<FleetDto>(vehicle);

                            FleetDto.CreatedBy = "SYSTEM";
                            FleetDto.CreatedDate = DateTime.Now;
                            FleetDto.ModifiedBy = null;
                            FleetDto.ModifiedDate = null;
                            FleetDto.VehicleStatus = "TERMINATE";
                            FleetDto.IsActive = false;
                            FleetDto.EndDate = CtfData.EFFECTIVE_DATE;
                            FleetDto.MstFleetId = 0;

                            var TerminateCar = Mapper.Map<MST_FLEET>(FleetDto);
                            _fleetService.save(TerminateCar);

                            input.ActionType = Enums.ActionType.Completed;
                            CtfWorkflow(input);
                        }
                    }
                }
            }
            else if(CtfData.EXTEND_VEHICLE.Value)
            {
                vehicle.IS_ACTIVE = false;
                vehicle.MODIFIED_BY = "SYSTEM";
                vehicle.MODIFIED_DATE = DateTime.Now;
                _fleetService.save(vehicle);

                var FleetDto = Mapper.Map<FleetDto>(vehicle);
                var extendDto = _ctfExtendService.GetCtfExtend().Where(x => x.TRA_CTF_ID == CtfData.TRA_CTF_ID).FirstOrDefault();

                FleetDto.CreatedBy = "SYSTEM";
                FleetDto.CreatedDate = DateTime.Now;
                FleetDto.ModifiedBy = null;
                FleetDto.ModifiedDate = null;
                FleetDto.VehicleStatus = "LIVE";
                FleetDto.SupplyMethod = "EXTEND";
                FleetDto.IsActive = true;
                FleetDto.StartContract = vehicle.END_CONTRACT.Value.AddDays(1);
                FleetDto.EndContract = extendDto.NEW_PROPOSED_DATE;
                FleetDto.MstFleetId = 0;

                var ExtendCar = Mapper.Map<MST_FLEET>(FleetDto);
                _fleetService.save(ExtendCar);

                input.ActionType = Enums.ActionType.Completed;
                CtfWorkflow(input);
            }
         
        }
        public bool CheckCtfExists(TraCtfDto item)
        {
            var isExist = false;
            var exist = _ctfService.GetCtf().Where(x => x.IS_ACTIVE && x.DOCUMENT_STATUS != Enums.DocumentStatus.Completed && x.DOCUMENT_STATUS != Enums.DocumentStatus.Cancelled && x.POLICE_NUMBER == item.PoliceNumber
                                                                    && x.EMPLOYEE_ID == item.EmployeeId).ToList();
            if (exist.Count > 0)
            {
                isExist = true;
            }

            return isExist;
        }
    }
}

