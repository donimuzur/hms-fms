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

namespace FMS.BLL.Ccf
{
    public class CcfBLL : ITraCcfBLL
    {
        private IUnitOfWork _uow;
        private ICcfService _ccfService;
        private IDocumentNumberService _docNumberService;
        private IWorkflowHistoryService _workflowService;
        private ISettingService _settingService;
        private IReasonService _reasonService;
        private IPenaltyLogicService _penaltyLogicService;
        private IPriceListService _pricelistService;
        private IFleetService _fleetService;
        private IMessageService _messageService;
        private IEmployeeService _employeeService;
        private IVendorService _vendorService;
        private IComplaintCategoryService _complaintCategory;

        public CcfBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _ccfService = new CcfService(uow);
            _docNumberService = new DocumentNumberService(uow);
            _workflowService = new WorkflowHistoryService(uow);
            _settingService = new SettingService(uow);
            _reasonService = new ReasonService(uow);
            _penaltyLogicService = new PenaltyLogicService(uow);
            _pricelistService = new PriceListService(uow);
            _fleetService = new FleetService(uow);
            _messageService = new MessageService(_uow);
            _employeeService = new EmployeeService(_uow);
            _vendorService = new VendorService(_uow);
            _complaintCategory = new ComplainCategoryService(_uow);
        }

        public List<TraCcfDto> GetCcf()
        {
            var data = _ccfService.GetCcf();
            var redata = Mapper.Map<List<TraCcfDto>>(data);
            return redata;
        }

        public TraCcfDto Save(TraCcfDto Dto, Login userLogin)
        {
            TRA_CCF dbTraCcf;
            TRA_CCF_DETAIL dbTraCcfD1;
            if (Dto == null)
            {
                throw new Exception("Invalid Data Entry");
            }

            try
            {
                bool changed = false;

                if (Dto.TraCcfId > 0)
                {
                    //update
                    var Exist = _ccfService.GetCcf().Where(c => c.TRA_CCF_ID == Dto.TraCcfId).FirstOrDefault();

                    if (Exist == null)
                        throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
                    dbTraCcf = Mapper.Map<TRA_CCF>(Dto);
                    dbTraCcfD1 = Mapper.Map<TRA_CCF_DETAIL>(Dto);
                    if (dbTraCcf.POLICE_NUMBER != null || dbTraCcf.POLICE_NUMBER_GS != null)
                    {
                        _ccfService.Save(dbTraCcf, userLogin);
                    }

                    
                    //if (dbTraCcfD1.COMPLAINT_NOTE != null || dbTraCcfD1.COORDINATOR_NOTE != null)
                    //{
                    if (dbTraCcf.DOCUMENT_STATUS == Enums.DocumentStatus.AssignedForUser)
                    {
                        var data_d1 = _ccfService.GetCcfD1().Where(c => c.TRA_CCF_DETAIL_ID == Dto.TraCcfDetilId).FirstOrDefault();
                        if (data_d1 != null)
                        {
                            dbTraCcfD1.COMPLAINT_DATE = data_d1.COMPLAINT_DATE;
                            dbTraCcfD1.COMPLAINT_NOTE = data_d1.COMPLAINT_NOTE;
                            dbTraCcfD1.COMPLAINT_ATT = data_d1.COMPLAINT_ATT;
                            dbTraCcfD1.COORDINATOR_RESPONSE_DATE = data_d1.COORDINATOR_RESPONSE_DATE;
                            dbTraCcfD1.COORDINATOR_NOTE = data_d1.COORDINATOR_NOTE;
                            dbTraCcfD1.COORDINATOR_PROMISED_DATE = data_d1.COORDINATOR_PROMISED_DATE;
                            dbTraCcfD1.COORDINATOR_ATT = data_d1.COORDINATOR_ATT;
                            _ccfService.Save_d1(dbTraCcfD1);
                        }
                        if (Dto.ComplaintNote != null)
                        {
                            _ccfService.Save_d1(dbTraCcfD1);
                        }
                        if (Dto.CoodinatorNote != null)
                        {
                            _ccfService.Save_d1(dbTraCcfD1);
                        }
                    }
                    else if (dbTraCcf.DOCUMENT_STATUS == Enums.DocumentStatus.AssignedForFleet || dbTraCcf.DOCUMENT_STATUS == Enums.DocumentStatus.AssignedForHR)
                    {
                        var data_d1 = _ccfService.GetCcfD1().Where(c => c.TRA_CCF_DETAIL_ID == Dto.TraCcfDetilId).FirstOrDefault();
                        if (data_d1 != null)
                        {
                            dbTraCcfD1.COMPLAINT_DATE = data_d1.COMPLAINT_DATE;
                            dbTraCcfD1.COMPLAINT_NOTE = data_d1.COMPLAINT_NOTE;
                            dbTraCcfD1.COMPLAINT_ATT = data_d1.COMPLAINT_ATT;
                            _ccfService.Save_d1(dbTraCcfD1);
                        }
                        if (Dto.CoodinatorNote != null)
                        {
                            _ccfService.Save_d1(dbTraCcfD1);
                        }
                    }
                    //}
                }
                else
                {
                    //add
                    var inputDoc = new GenerateDocNumberInput();
                    inputDoc.Month = DateTime.Now.Month;
                    inputDoc.Year = DateTime.Now.Year;
                    inputDoc.DocType = (int)Enums.DocumentType.CCF;

                    Dto.DocumentNumber = _docNumberService.GenerateNumber(inputDoc);

                    dbTraCcf = Mapper.Map<TRA_CCF>(Dto);
                    dbTraCcfD1 = Mapper.Map<TRA_CCF_DETAIL>(Dto);
                    _ccfService.Save(dbTraCcf, userLogin);
                    var dataCCF = _ccfService.GetCcf().Where(x => x.DOCUMENT_NUMBER == Dto.DocumentNumber).FirstOrDefault();
                    dbTraCcfD1.TRA_CCF_ID = dataCCF.TRA_CCF_ID;
                    _ccfService.Save_d1(dbTraCcfD1);

                }
                var input = new CcfWorkflowDocumentInput()
                {
                    DocumentId = dbTraCcf.TRA_CCF_ID,
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
            var data = _ccfService.GetCcf().Where(x => x.DOCUMENT_NUMBER == Dto.DocumentNumber).FirstOrDefault();
            Dto = Mapper.Map<TraCcfDto>(data);
            return Dto;
        }

        private void AddWorkflowHistory(CcfWorkflowDocumentInput input)
        {
            var dbData = Mapper.Map<WorkflowHistoryDto>(input);

            dbData.ACTION_DATE = DateTime.Now;
            dbData.MODUL_ID = Enums.MenuList.TraCcf;
            dbData.ACTION = input.ActionType;
            dbData.REMARK_ID = null;
            _workflowService.Save(dbData);

        }

        private void SubmitDocument(CcfWorkflowDocumentInput input)
        {
            var dbData = _ccfService.GetCcfById(input.DocumentId);

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

        public void CcfWorkflow(CcfWorkflowDocumentInput input)
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
            }
            //todo sent mail
            if (isNeedSendNotif)
                SendEmailWorkflow(input);

            _uow.SaveChanges();
        }

        private void SendEmailWorkflow(CcfWorkflowDocumentInput input)
        {
            var ccfData = Mapper.Map<TraCcfDto>(_ccfService.GetCcfById(input.DocumentId));

            var mailProcess = ProsesMailNotificationBody(ccfData, input);

            //distinct double To email
            List<string> ListTo = mailProcess.To.Distinct().ToList();

            if (mailProcess.IsCCExist)
                //Send email with CC
                _messageService.SendEmailToListWithCC(ListTo, mailProcess.CC, mailProcess.Subject, mailProcess.Body, true);
            else
                _messageService.SendEmailToList(ListTo, mailProcess.Subject, mailProcess.Body, true);
        }

        private class CcfMailNotification
        {
            public CcfMailNotification()
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

        private CcfMailNotification ProsesMailNotificationBody(TraCcfDto ccfData, CcfWorkflowDocumentInput input)
        {
            var bodyMail = new StringBuilder();
            var rc = new CcfMailNotification();

            var fleetdata = _fleetService.GetFleet().Where(x => x.POLICE_NUMBER == ccfData.PoliceNumber && x.IS_ACTIVE).FirstOrDefault();
            var vendor = _vendorService.GetVendor().Where(x => x.VENDOR_NAME == fleetdata.VENDOR_NAME).FirstOrDefault();
            var vehTypeBenefit = _settingService.GetSetting().Where(x => x.SETTING_GROUP == "VEHICLE_TYPE" && x.SETTING_NAME == "BENEFIT").FirstOrDefault().SETTING_NAME;

            var isBenefit = ccfData.VehicleType == vehTypeBenefit.ToString() ? true : false;

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
            var typeEnv = ConfigurationManager.AppSettings["Environment"];
            var employeeData = _employeeService.GetEmployeeById(ccfData.EmployeeID);
            var creatorData = _employeeService.GetEmployeeById(ccfData.EmployeeID);

            var fleetApprovalData = _employeeService.GetEmployeeById(ccfData.EmployeeID);
            var complaintCategory = _complaintCategory.GetComplaintById(ccfData.ComplaintCategory);

            var employeeDataEmail = employeeData == null ? string.Empty : employeeData.EMAIL_ADDRESS;
            var creatorDataEmail = creatorData == null ? string.Empty : creatorData.EMAIL_ADDRESS;
            var vendorDataEmail = vendor == null ? string.Empty : vendor.EMAIL_ADDRESS;

            var employeeDataName = employeeData == null ? string.Empty : employeeData.FORMAL_NAME;
            var creatorDataName = creatorData == null ? string.Empty : creatorData.FORMAL_NAME;
            var fleetApprovalDataName = fleetApprovalData == null ? string.Empty : fleetApprovalData.FORMAL_NAME;
            var vendorDataName = vendor == null ? string.Empty : vendor.VENDOR_NAME;

            var hrList = new List<string>();
            var fleetList = new List<string>();

            var hrRole = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                && x.SETTING_VALUE.Contains("HR")).FirstOrDefault().SETTING_VALUE;
            var fleetRole = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                && x.SETTING_VALUE.Contains("FLEET")).FirstOrDefault().SETTING_VALUE;

            var hrQuery = "SELECT employeeID FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + hrRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";
            var fleetQuery = "SELECT employeeID FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + fleetRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";

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
                var hrEmail = _employeeService.GetEmployeeById(reader.GetString(0));
                var hrEmailData = hrEmail == null ? string.Empty : hrEmail.EMAIL_ADDRESS;
                hrList.Add(hrEmailData);
            }

            query = new SqlCommand(fleetQuery, con);
            reader = query.ExecuteReader();
            while (reader.Read())
            {
                var fleetEmail = _employeeService.GetEmployeeById(reader.GetString(0));
                var fleetEmailData = fleetEmail == null ? string.Empty : fleetEmail.EMAIL_ADDRESS;
                fleetList.Add(fleetEmailData);
            }

            reader.Close();
            con.Close();

            if (ccfData.EmployeeID == input.EmployeeId)
            {
                if (complaintCategory.ROLE_TYPE == "Fleet")
                {
                    rc.Subject = "CCF -  Car Complaint Form";

                    bodyMail.Append("Dear Fleet,<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("You have received new Car Complaint Form<br />");
                    bodyMail.AppendLine();
                    bodyMail.AppendLine();
                    bodyMail.Append("<a href='" + webRootUrl + "TraCcf/ResponseCoordinator?TraCcfId=" + ccfData.TraCcfId + "&isPersonalDashboard=False" + "'>" + webRootUrl + "TraCcf/ResponseCoordinator?TraCcfId=" + ccfData.TraCcfId + "&isPersonalDashboard=False" + "</a><br />");
                    bodyMail.AppendLine();
                    bodyMail.AppendLine();
                    bodyMail.Append("Thanks<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Regards,<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append(employeeDataName);
                    bodyMail.AppendLine();

                    foreach (var item in fleetList)
                    {
                        rc.To.Add(item);
                    }
                }
                else if (complaintCategory.ROLE_TYPE == "HR")
                {
                    rc.Subject = "CCF -  Car Complaint Form";

                    bodyMail.Append("Dear HR,<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("You have received new Car Complaint Form<br />");
                    bodyMail.AppendLine();
                    bodyMail.AppendLine();
                    bodyMail.Append("<a href='" + webRootUrl + "TraCcf/ResponseCoordinator?TraCcfId=" + ccfData.TraCcfId + "&isPersonalDashboard=False" + "'>" + webRootUrl + "TraCcf/ResponseCoordinator?TraCcfId=" + ccfData.TraCcfId + "&isPersonalDashboard=False" + "</a><br />");
                    bodyMail.AppendLine();
                    bodyMail.AppendLine();
                    bodyMail.Append("Thanks<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Regards,<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append(employeeDataName);
                    bodyMail.AppendLine();

                    foreach (var item in fleetList)
                    {
                        rc.To.Add(item);
                    }
                }
            }
            else
            //Email InProgress From Fleet/HR to Employee
            {
                if (ccfData.DocumentStatus == Enums.DocumentStatus.InProgress)
                {
                    if (complaintCategory.ROLE_TYPE == "Fleet")
                    {
                        rc.Subject = "CCF -  Car Complaint Form";

                        bodyMail.Append("Dear " + ccfData.EmployeeName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("You have received email response complaint <br />");
                        bodyMail.AppendLine();
                        bodyMail.AppendLine();
                        bodyMail.Append("<a href='" + webRootUrl + "DetailsCcf/DetailsCcf?TraCcfId=" + ccfData.TraCcfId + "&isPersonalDashboard=True" + "'>" + webRootUrl + "TraCcf/DetailsCcf?TraCcfId=" + ccfData.TraCcfId + "&isPersonalDashboard=False" + "</a><br />");
                        bodyMail.AppendLine();
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team");
                        bodyMail.AppendLine();

                        rc.To.Add(employeeDataEmail);

                        foreach (var item in fleetList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    else if (complaintCategory.ROLE_TYPE == "HR")
                    {
                        rc.Subject = "CCF -  Car Complaint Form";

                        bodyMail.Append("Dear " + ccfData.EmployeeName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("You have received email response complaint <br />");
                        bodyMail.AppendLine();
                        bodyMail.AppendLine();
                        bodyMail.Append("<a href='" + webRootUrl + "DetailsCcf/DetailsCcf?TraCcfId=" + ccfData.TraCcfId + "&isPersonalDashboard=True" + "'>" + webRootUrl + "TraCcf/DetailsCcf?TraCcfId=" + ccfData.TraCcfId + "&isPersonalDashboard=False" + "</a><br />");
                        bodyMail.AppendLine();
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("HR Team");
                        bodyMail.AppendLine();

                        rc.To.Add(employeeDataEmail);

                        foreach (var item in fleetList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                }
                else if (ccfData.DocumentStatus == Enums.DocumentStatus.Completed)
                {
                    if (complaintCategory.ROLE_TYPE == "Fleet")
                    {
                        rc.Subject = "CCF -  Car Complaint Form";

                        bodyMail.Append("Dear " + ccfData.EmployeeName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("You have received email response complaint status is completed<br />");
                        bodyMail.AppendLine();
                        bodyMail.AppendLine();
                        bodyMail.Append("<a href='" + webRootUrl + "DetailsCcf/DetailsCcf?TraCcfId=" + ccfData.TraCcfId + "&isPersonalDashboard=True" + "'>" + webRootUrl + "TraCcf/DetailsCcf?TraCcfId=" + ccfData.TraCcfId + "&isPersonalDashboard=False" + "</a><br />");
                        bodyMail.AppendLine();
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team");
                        bodyMail.AppendLine();

                        rc.To.Add(employeeDataEmail);

                        foreach (var item in fleetList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    else if (complaintCategory.ROLE_TYPE == "HR")
                    {
                        rc.Subject = "CCF -  Car Complaint Form";

                        bodyMail.Append("Dear " + ccfData.EmployeeName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("You have received email response complaint status is completed<br />");
                        bodyMail.AppendLine();
                        bodyMail.AppendLine();
                        bodyMail.Append("<a href='" + webRootUrl + "DetailsCcf/DetailsCcf?TraCcfId=" + ccfData.TraCcfId + "&isPersonalDashboard=True" + "'>" + webRootUrl + "TraCcf/DetailsCcf?TraCcfId=" + ccfData.TraCcfId + "&isPersonalDashboard=False" + "</a><br />");
                        bodyMail.AppendLine();
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("HR Team");
                        bodyMail.AppendLine();

                        rc.To.Add(employeeDataEmail);

                        foreach (var item in fleetList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                }
            }
            rc.IsCCExist = true;
            rc.Body = bodyMail.ToString();
            return rc;
        }

        private void CreateDocument(CcfWorkflowDocumentInput input)
        {
            var dbData = _ccfService.GetCcf().Where(x => x.TRA_CCF_ID == input.DocumentId).FirstOrDefault();

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);
        }

        public TraCcfDto GetCcfById(long id)
        {
            var data = _ccfService.GetCcfById(id);
            var retData = Mapper.Map<TraCcfDto>(data);
            return retData;
        }

        public void CancelCcf(long id, int Remark, string user)
        {
            _ccfService.CancelCcf(id, Remark, user);
        }

        public List<TraCcfDto> GetCcfPersonal(Login userLogin)
        {
            var data = _ccfService.GetCcf().Where(x => (x.EMPLOYEE_ID == userLogin.EMPLOYEE_ID && x.EMPLOYEE_ID_COMPLAINT_FOR == userLogin.EMPLOYEE_ID)).ToList();
            var retData = Mapper.Map<List<TraCcfDto>>(data);
            return retData;
        }

        private void ApproveDocument(CcfWorkflowDocumentInput input)
        {
            var dbData = _ccfService.GetCcfById(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);


            //if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingFleetApproval && input.EndRent == true)
            //{
            //    dbData.DOCUMENT_STATUS = Enums.DocumentStatus.InProgress;
            //}
            //else if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingFleetApproval && input.EndRent == false)
            //{
            //    dbData.DOCUMENT_STATUS = Enums.DocumentStatus.Completed;
            //}
            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }

        private void RejectDocument(CcfWorkflowDocumentInput input)
        {
            var dbData = _ccfService.GetCcfById(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingFleetApproval)
            //{
            //    dbData.DOCUMENT_STATUS = Enums.DocumentStatus.AssignedForUser;
            //}

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }

        public List<TraCcfDto> GetCcfD1(int traCCFid)
        {
            var data = _ccfService.GetCcfD1().Where(x=>x.TRA_CCF_ID == traCCFid);
            var redata = Mapper.Map<List<TraCcfDto>>(data);
            return redata;
        }
    }
}
