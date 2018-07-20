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
        private IArchTraCcfService _archCcfService;
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
        private ILocationMappingService _locationMappingService;

        public CcfBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _ccfService = new CcfService(uow);
            _archCcfService = new ArchTraCcfService(uow);
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
            _locationMappingService = new LocationMappingService(_uow);
        }

        public List<TraCcfDto> GetCcf()
        {
            var data = _ccfService.GetCcf();
            //var locationMapping = _locationMappingService.GetLocationMapping().Where(x => x.IS_ACTIVE).OrderByDescending(x => x.VALIDITY_FROM).ToList();
            var redata = Mapper.Map<List<TraCcfDto>>(data);
            //foreach (var item in redata)
            //{
            //    var region = locationMapping.Where(x => x.LOCATION.ToUpper() == item.LocationCity.ToUpper()).FirstOrDefault();

            //    item.Region = region == null ? string.Empty : region.REGION;
            //}

            return redata;
        }

        public void SaveDetails(TraCcfDetailDto details, Login userLogin)
        {
            TRA_CCF_DETAIL detailCCF = Mapper.Map<TRA_CCF_DETAIL>(details);

            _ccfService.Save_d1(detailCCF);
        }

        public string GetNumber()
        {
            var inputDoc = new GenerateDocNumberInput();
            inputDoc.Month = DateTime.Now.Month;
            inputDoc.Year = DateTime.Now.Year;
            inputDoc.DocType = (int)Enums.DocumentType.CCF;
            var number = _docNumberService.GenerateNumber(inputDoc);
            return number;
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
                
                if (Dto.TraCcfId > 0)
                {
                    //update
                    var Exist = _ccfService.GetCcf().Where(c => c.TRA_CCF_ID == Dto.TraCcfId).FirstOrDefault();

                    if (Exist == null)
                        throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
                    dbTraCcf = Mapper.Map<TRA_CCF>(Dto);
                    dbTraCcfD1 = Mapper.Map<TRA_CCF_DETAIL>(Dto.DetailSave);

                    //If Status Completed
                    if (Dto.DocumentStatus == Enums.DocumentStatus.Completed)
                    {
                        FMSEntities context = new FMSEntities();
                        var query = from p in context.TRA_CCF
                                    where p.TRA_CCF_ID == dbTraCcf.TRA_CCF_ID
                                    select p;
                        foreach (TRA_CCF dt in query)
                        {
                            dt.DOCUMENT_STATUS = dbTraCcf.DOCUMENT_STATUS;
                        }
                        context.SaveChanges();
                    }

                    //If Status In Progress
                    if (dbTraCcf.POLICE_NUMBER != null || dbTraCcf.POLICE_NUMBER_GS != null)
                    {
                        _ccfService.Save(dbTraCcf, userLogin);
                        if (dbTraCcfD1.COMPLAINT_URL != null && dbTraCcfD1.COMPLAINT_ATT != null)
                        {
                            dbTraCcfD1.TRA_CCF_ID = dbTraCcf.TRA_CCF_ID;
                            _ccfService.Save_d1(dbTraCcfD1);
                        }
                        else
                        {
                            var data_d1 = _ccfService.GetCcfD1().Where(c => c.TRA_CCF_DETAIL_ID == Dto.DetailSave.TraCcfDetilId).FirstOrDefault();
                            if (data_d1 != null)
                            {
                                dbTraCcfD1.TRA_CCF_ID = data_d1.TRA_CCF_ID;
                                dbTraCcfD1.COMPLAINT_URL = data_d1.COMPLAINT_URL;
                                dbTraCcfD1.COMPLAINT_ATT = data_d1.COMPLAINT_ATT;
                                _ccfService.Save_d1(dbTraCcfD1);
                            }
                        }
                    }
                    if (dbTraCcfD1.COORDINATOR_NOTE != null && dbTraCcfD1.VENDOR_NOTE == null)
                    {
                        var data_d1 = _ccfService.GetCcfD1().Where(c => c.TRA_CCF_DETAIL_ID == Dto.DetailSave.TraCcfDetilId).FirstOrDefault();
                        if (data_d1 == null)
                        {
                            _ccfService.Save_d1(dbTraCcfD1);
                        }
                        else
                        {
                            dbTraCcfD1.COMPLAINT_DATE = data_d1.COMPLAINT_DATE;
                            dbTraCcfD1.COMPLAINT_NOTE = data_d1.COMPLAINT_NOTE;
                            dbTraCcfD1.COMPLAINT_URL = data_d1.COMPLAINT_URL;
                            dbTraCcfD1.COMPLAINT_ATT = data_d1.COMPLAINT_ATT;
                            dbTraCcfD1.VENDOR_RESPONSE_DATE = null;
                            if (dbTraCcfD1.COORDINATOR_ATT == null) { dbTraCcfD1.COORDINATOR_ATT = data_d1.COORDINATOR_ATT; }
                            _ccfService.Save_d1(dbTraCcfD1);
                        }
                        FMSEntities context = new FMSEntities();
                        var query = from p in context.TRA_CCF
                                    where p.TRA_CCF_ID == dbTraCcf.TRA_CCF_ID
                                    select p;
                        foreach (TRA_CCF dt in query)
                        {
                            dt.DOCUMENT_STATUS = dbTraCcf.DOCUMENT_STATUS;
                            dt.COORDINATOR_NAME = userLogin.USER_ID;
                        }
                        context.SaveChanges();
                    }
                    else if (dbTraCcfD1.COORDINATOR_NOTE == null && dbTraCcfD1.VENDOR_NOTE != null)
                    {
                        var data_d1 = _ccfService.GetCcfD1().Where(c => c.TRA_CCF_DETAIL_ID == Dto.DetailSave.TraCcfDetilId).FirstOrDefault();
                        if (data_d1 == null)
                        {
                            _ccfService.Save_d1(dbTraCcfD1);
                        }
                        else
                        {
                            dbTraCcfD1.COMPLAINT_DATE = data_d1.COMPLAINT_DATE;
                            dbTraCcfD1.COMPLAINT_NOTE = data_d1.COMPLAINT_NOTE;
                            dbTraCcfD1.COMPLAINT_URL = data_d1.COMPLAINT_URL;
                            dbTraCcfD1.COMPLAINT_ATT = data_d1.COMPLAINT_ATT;
                            dbTraCcfD1.COORDINATOR_RESPONSE_DATE = data_d1.COORDINATOR_RESPONSE_DATE;
                            dbTraCcfD1.COORDINATOR_NOTE = data_d1.COORDINATOR_NOTE;
                            dbTraCcfD1.COORDINATOR_PROMISED_DATE = data_d1.COORDINATOR_PROMISED_DATE;
                            dbTraCcfD1.COORDINATOR_URL = data_d1.COORDINATOR_URL;
                            dbTraCcfD1.COORDINATOR_ATT = data_d1.COORDINATOR_ATT;
                            _ccfService.Save_d1(dbTraCcfD1);
                        }
                        FMSEntities context = new FMSEntities();
                        var query = from p in context.TRA_CCF
                                    where p.TRA_CCF_ID == dbTraCcf.TRA_CCF_ID
                                    select p;
                        foreach (TRA_CCF dt in query)
                        {
                            dt.DOCUMENT_STATUS = dbTraCcf.DOCUMENT_STATUS;
                        }
                        context.SaveChanges();
                    }
                    else if (dbTraCcfD1.COORDINATOR_NOTE != null && dbTraCcfD1.VENDOR_NOTE != null)
                    {
                        var data_d1 = _ccfService.GetCcfD1().Where(c => c.TRA_CCF_DETAIL_ID == Dto.DetailSave.TraCcfDetilId).FirstOrDefault();
                        if (data_d1 == null)
                        {
                            _ccfService.Save_d1(dbTraCcfD1);
                        }
                        else
                        {
                            dbTraCcfD1.COMPLAINT_DATE = data_d1.COMPLAINT_DATE;
                            dbTraCcfD1.COMPLAINT_NOTE = data_d1.COMPLAINT_NOTE;
                            dbTraCcfD1.COMPLAINT_URL = data_d1.COMPLAINT_URL;
                            dbTraCcfD1.COMPLAINT_ATT = data_d1.COMPLAINT_ATT;
                            if (dbTraCcfD1.COORDINATOR_ATT == null) { dbTraCcfD1.COORDINATOR_ATT = data_d1.COORDINATOR_ATT; }
                            if (dbTraCcfD1.VENDOR_ATT == null) { dbTraCcfD1.VENDOR_ATT = data_d1.VENDOR_ATT; }
                            _ccfService.Save_d1(dbTraCcfD1);
                        }
                        FMSEntities context = new FMSEntities();
                        var query = from p in context.TRA_CCF
                                    where p.TRA_CCF_ID == dbTraCcf.TRA_CCF_ID
                                    select p;
                        foreach (TRA_CCF dt in query)
                        {
                            dt.DOCUMENT_STATUS = dbTraCcf.DOCUMENT_STATUS;
                        }
                        context.SaveChanges();
                    }
                    else if (dbTraCcfD1.COORDINATOR_NOTE == null && dbTraCcfD1.VENDOR_NOTE == null && dbTraCcf.DOCUMENT_STATUS == (int)Enums.DocumentStatus.InProgress)
                    {
                        // _ccfService.Save_d1(dbTraCcfD1);
                        FMSEntities context3 = new FMSEntities();
                        context3.TRA_CCF_DETAIL.Add(new TRA_CCF_DETAIL()
                        {
                            TRA_CCF_ID = dbTraCcf.TRA_CCF_ID,
                            COMPLAINT_DATE = dbTraCcfD1.COMPLAINT_DATE,
                            COMPLAINT_NOTE = dbTraCcfD1.COMPLAINT_NOTE,
                            COMPLAINT_URL = dbTraCcfD1.COMPLAINT_URL,
                            COMPLAINT_ATT = dbTraCcfD1.COMPLAINT_ATT
                        });
                        context3.SaveChanges();
                    }
                    FMSEntities context2 = new FMSEntities();
                    context2.TRA_CHANGES_HISTORY.Add(new TRA_CHANGES_HISTORY()
                    {
                        MODUL_ID = (int)Enums.MenuList.TraCcf,
                        FORM_ID = dbTraCcf.TRA_CCF_ID,
                        MODIFIED_BY = userLogin.USER_ID,
                        MODIFIED_DATE = DateTime.Now,
                        ACTION = "Modified"
                    });
                    context2.SaveChanges();
                }
                else
                {
                    dbTraCcf = Mapper.Map<TRA_CCF>(Dto);
                    dbTraCcfD1 = Mapper.Map<TRA_CCF_DETAIL>(Dto.DetailSave);
                   _ccfService.Save(dbTraCcf, userLogin);
                    var dataCCF = _ccfService.GetCcf().Where(x => x.DOCUMENT_NUMBER == Dto.DocumentNumber).FirstOrDefault();
                    dbTraCcfD1.TRA_CCF_ID = dataCCF.TRA_CCF_ID;
                    _ccfService.Save_d1(dbTraCcfD1);

                }

                //Exec Prosedure KPI
                EntityConnectionStringBuilder e = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["FMSEntities"].ConnectionString);
                string connectionString = e.ProviderConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                SqlCommand query1 = new SqlCommand("EXEC KPICoordinator @TraCCFId = " + dbTraCcf.TRA_CCF_ID + "", con);
                query1.ExecuteNonQuery();
                SqlCommand query2 = new SqlCommand("EXEC KPIVendor @TraCCFId = " + dbTraCcf.TRA_CCF_ID + "", con);
                query2.ExecuteNonQuery();
                con.Close();

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

        public void CcfWorkflow(CcfWorkflowDocumentInput input)
        {
            var isNeedSendNotif = true;
            switch (input.ActionType)
            {
                case Enums.ActionType.Created:
                    CreateDocument(input);
                    isNeedSendNotif = false;
                    break;
                case Enums.ActionType.Modified:
                    ModifiedDocument(input);
                    isNeedSendNotif = false;
                    break;
                case Enums.ActionType.Submit:
                    SubmitDocument(input);
                    break;
                case Enums.ActionType.Completed:
                    CompleteDocument(input);
                    break;
                case Enums.ActionType.Cancel:
                    CancelDocument(input);
                    isNeedSendNotif = false;
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
            
            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
            var typeEnv = ConfigurationManager.AppSettings["Environment"];
            var serverIntranet = ConfigurationManager.AppSettings["ServerIntranet"];
            var creatorDataComplaintFor = _employeeService.GetEmployeeById(ccfData.EmployeeIdComplaintFor);
           
            var fleetApprovalData = _employeeService.GetEmployeeById(ccfData.EmployeeID);
            var complaintCategory = _complaintCategory.GetComplaintById(ccfData.ComplaintCategory);
            var creatorDataEmailComplaintFor = creatorDataComplaintFor == null ? string.Empty : creatorDataComplaintFor.EMAIL_ADDRESS;
            
            var hrList = string.Empty;
            var fleetList = string.Empty;

            var hrEmailList = new List<string>();
            var fleetEmailList = new List<string>();

            var hrRole = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                && x.SETTING_VALUE.Contains("HR")).FirstOrDefault().SETTING_VALUE;
            var fleetRole = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                && x.SETTING_VALUE.Contains("FLEET")).FirstOrDefault().SETTING_VALUE;
            
            var hrQuery = "SELECT 'PMI\\' + sAMAccountName AS sAMAccountName FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + hrRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') WHERE sAMAccountName <> '" + ccfData.CreatedBy + "'";
            var fleetQuery = "SELECT 'PMI\\' + sAMAccountName AS sAMAccountName FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + fleetRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') WHERE sAMAccountName <> '" + ccfData.CreatedBy + "'";

            if (typeEnv == "VTI")
            {
                hrQuery = "SELECT 'PMI\\' + LOGIN AS LOGIN FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + hrRole + "' AND LOGIN <> '" + ccfData.CreatedBy + "'";
                fleetQuery = "SELECT 'PMI\\' + LOGIN AS LOGIN FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + fleetRole + "' AND LOGIN <> '"+ ccfData.CreatedBy + "'";
            }

            EntityConnectionStringBuilder e = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["FMSEntities"].ConnectionString);
            string connectionString = e.ProviderConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            //----------UPDATE CREATOR EMAIL FROM ADSI------------//
            var creatorDataEmail = string.Empty;
            var creatorDataName = string.Empty;

            var CreatorQuery = "SELECT EMAIL, INTERNAL_EMAIL FROM " + serverIntranet + ".[dbo].[tbl_ADSI_User] WHERE FULL_NAME = 'PMI\\" + ccfData.CreatedBy + "'";
            if (typeEnv == "VTI")
            {
                CreatorQuery = "SELECT EMAIL, FULL_NAME FROM EMAIL_FOR_VTI WHERE FULL_NAME = 'PMI\\" + ccfData.CreatedBy + "'";
            }

            SqlCommand query = new SqlCommand(CreatorQuery, con);
            SqlDataReader reader = query.ExecuteReader();
            while (reader.Read())
            {
                creatorDataEmail = reader[0].ToString();
                creatorDataName = reader[1].ToString();
            }
            ///////////////////////////////////////////////////////

            query = new SqlCommand(hrQuery, con);
            reader = query.ExecuteReader();
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

            var hrQueryEmail = "SELECT EMAIL FROM " + serverIntranet + ".[dbo].[tbl_ADSI_User] WHERE FULL_NAME IN (" + hrList + ")";
            var fleetQueryEmail = "SELECT EMAIL FROM " + serverIntranet + ".[dbo].[tbl_ADSI_User] WHERE FULL_NAME IN (" + fleetList + ")";
            var creatorQuery =
                "SELECT EMAIL from " + serverIntranet + ".[dbo].[tbl_ADSI_User] where FULL_NAME like 'PMI\\" +
                ccfData.CreatedBy + "'";
            var employeeForQuery =
               "SELECT EMAIL from " + serverIntranet + ".[dbo].[tbl_ADSI_User] where ID like 'ID" +
               ccfData.EmployeeIdComplaintFor + "'";

            if (typeEnv == "VTI")
            {
                hrQueryEmail = "SELECT EMAIL FROM EMAIL_FOR_VTI WHERE FULL_NAME IN (" + hrList + ")";
                fleetQueryEmail = "SELECT EMAIL FROM EMAIL_FOR_VTI WHERE FULL_NAME IN (" + fleetList + ")";
                creatorQuery = "SELECT EMAIL FROM EMAIL_FOR_VTI WHERE FULL_NAME like 'PMI\\" + ccfData.CreatedBy + "'";
                employeeForQuery = "SELECT EMAIL FROM EMAIL_FOR_VTI WHERE ID like 'ID" + ccfData.EmployeeIdComplaintFor + "'";
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

            query = new SqlCommand(creatorQuery, con);
            reader = query.ExecuteReader();
            while (reader.Read())
            {
                creatorDataEmail = reader["EMAIL"].ToString();
            }

            query = new SqlCommand(employeeForQuery, con);
            reader = query.ExecuteReader();
            while (reader.Read())
            {
                creatorDataEmailComplaintFor = reader["EMAIL"].ToString();
            }

            reader.Close();
            con.Close();
            //Email Employee to Fleet / HR
            if (ccfData.EmployeeID == input.EmployeeId)
            {
                if (complaintCategory.ROLE_TYPE.ToUpper() == "FLEET")
                {
                    //rc.Subject = ccfData.DocumentNumber + " has been submitted by " + creatorDataName;
                    rc.Subject = ccfData.DocumentNumber + " - User Complaint";

                    bodyMail.Append("Dear Fleet,<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("You Have Received New Car Complaint Form Document Number : "+ ccfData.DocumentNumber + "<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.AppendLine();
                    bodyMail.Append("Please Respon <a href='" + webRootUrl + "/TraCcf/ResponseCoordinator?TraCcfId=" + ccfData.TraCcfId + "&isPersonalDashboard=False" + "'> HERE </a><br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.AppendLine();
                    bodyMail.Append("Thanks<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Regards,<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append(creatorDataName);
                    bodyMail.AppendLine();

                    foreach (var item in fleetEmailList)
                    {
                        rc.To.Add(item);
                    }

                    //rc.CC.Add(creatorDataEmail);
                    if (creatorDataEmailComplaintFor != creatorDataEmail)
                    {
                        rc.CC.Add(creatorDataEmailComplaintFor);
                        rc.CC.Add(creatorDataEmail);
                    }
                    else
                    {
                        rc.CC.Add(creatorDataEmail);
                    }
                }
                else if (complaintCategory.ROLE_TYPE.ToUpper() == "HR")
                {
                    //rc.Subject = ccfData.DocumentNumber + " has been submitted by " + creatorDataName;
                    rc.Subject = ccfData.DocumentNumber + " - User Complaint";

                    bodyMail.Append("Dear HR,<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("You Have Received New Car Complaint Form Document Number : "+ ccfData.DocumentNumber  + "<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.AppendLine();
                    bodyMail.Append("Please Respon <a href='" + webRootUrl + "/TraCcf/ResponseCoordinator?TraCcfId=" + ccfData.TraCcfId + "&isPersonalDashboard=False" + "'> HERE </a><br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.AppendLine();
                    bodyMail.Append("Thanks<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Regards,<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append(creatorDataName);
                    bodyMail.AppendLine();

                    foreach (var item in hrEmailList)
                    {
                        rc.To.Add(item);
                    }

                    //rc.CC.Add(creatorDataEmail);
                    if (creatorDataEmailComplaintFor != creatorDataEmail)
                    {
                        rc.CC.Add(creatorDataEmailComplaintFor);
                        rc.CC.Add(creatorDataEmail);
                    }
                    else
                    {
                        rc.CC.Add(creatorDataEmail);
                    }
                }
            }
            else
            //Email InProgress & Complete From Fleet/HR to Employee
            {
                switch (input.ActionType)
                {
                    case Enums.ActionType.Submit:
                        if (complaintCategory.ROLE_TYPE.ToUpper() == "FLEET")
                        {
                            //rc.Subject = ccfData.DocumentNumber + " In Progress by Fleet";
                            rc.Subject = ccfData.DocumentNumber + " - Complaint Response";

                            bodyMail.Append("Dear " + ccfData.EmployeeName + ",<br /><br />");
                            bodyMail.AppendLine();
                            bodyMail.Append("Your Car Complaint Form " + ccfData.DocumentNumber + " Has Been Response By "+ input.UserId +" <br /><br />");
                            bodyMail.AppendLine();
                            bodyMail.AppendLine();
                            bodyMail.Append("Click Detail <a href='" + webRootUrl + "/TraCcf/DetailsCcf/DetailsCcf?TraCcfId=" + ccfData.TraCcfId + "&isPersonalDashboard=True" + "'> HERE </a><br /><br />");
                            bodyMail.AppendLine();
                            bodyMail.AppendLine();
                            bodyMail.Append("Thanks<br /><br />");
                            bodyMail.AppendLine();
                            bodyMail.Append("Regards,<br />");
                            bodyMail.AppendLine();
                            bodyMail.Append("Fleet Team");
                            bodyMail.AppendLine();

                            rc.To.Add(creatorDataEmail);

                            foreach (var item in fleetEmailList)
                            {
                                rc.CC.Add(item);
                            }
                            if (creatorDataEmailComplaintFor != creatorDataEmail)
                            {
                                rc.CC.Add(creatorDataEmailComplaintFor);
                            }
                        }
                        else if (complaintCategory.ROLE_TYPE.ToUpper() == "HR")
                        {
                            //rc.Subject = ccfData.DocumentNumber + " In Progress by HR";
                            rc.Subject = ccfData.DocumentNumber + " - Complaint Response";

                            bodyMail.Append("Dear " + ccfData.EmployeeName + ",<br /><br />");
                            bodyMail.AppendLine();
                            bodyMail.Append("Your Car Complaint Form "+ ccfData.DocumentNumber + " Has Been Response By "+ input.UserId +" <br /><br />");
                            bodyMail.AppendLine();
                            bodyMail.AppendLine();
                            bodyMail.Append("Click Detail <a href='" + webRootUrl + "/TraCcf/DetailsCcf/DetailsCcf?TraCcfId=" + ccfData.TraCcfId + "&isPersonalDashboard=True" + "'> HERE </a><br /><br />");
                            bodyMail.AppendLine();
                            bodyMail.AppendLine();
                            bodyMail.Append("Thanks<br /><br />");
                            bodyMail.AppendLine();
                            bodyMail.Append("Regards,<br />");
                            bodyMail.AppendLine();
                            bodyMail.Append("HR Team");
                            bodyMail.AppendLine();

                            rc.To.Add(creatorDataEmail);

                            foreach (var item in hrEmailList)
                            {
                                rc.CC.Add(item);
                            }
                            if (creatorDataEmailComplaintFor != creatorDataEmail)
                            {
                                rc.CC.Add(creatorDataEmailComplaintFor);
                            }
                        }
                        break;
                    case Enums.ActionType.Completed:
                        if (complaintCategory.ROLE_TYPE.ToUpper() == "FLEET")
                        {
                            //rc.Subject = ccfData.DocumentNumber + " has been completed by Fleet";
                            rc.Subject = ccfData.DocumentNumber + " - Completed Document";

                            bodyMail.Append("Dear " + ccfData.EmployeeName + ",<br /><br />");
                            bodyMail.AppendLine();
                            bodyMail.Append("Your Car Complaint Form " + ccfData.DocumentNumber + " Has Been Completed By " + input.UserId + "<br /><br />");
                            bodyMail.AppendLine();
                            bodyMail.AppendLine();
                            bodyMail.Append("Click Detail <a href='" + webRootUrl + "/TraCcf/DetailsCcf/DetailsCcf?TraCcfId=" + ccfData.TraCcfId + "&isPersonalDashboard=True" + "'> HERE </a><br /><br />");
                            bodyMail.AppendLine();
                            bodyMail.AppendLine();
                            bodyMail.Append("Thanks<br /><br />");
                            bodyMail.AppendLine();
                            bodyMail.Append("Regards,<br />");
                            bodyMail.AppendLine();
                            bodyMail.Append("Fleet Team");
                            bodyMail.AppendLine();

                            rc.To.Add(creatorDataEmail);

                            foreach (var item in fleetEmailList)
                            {
                                rc.CC.Add(item);
                            }
                            if (creatorDataEmailComplaintFor != creatorDataEmail)
                            {
                                rc.CC.Add(creatorDataEmailComplaintFor);
                            }
                        }
                        else if (complaintCategory.ROLE_TYPE == "HR")
                        {
                            //rc.Subject = ccfData.DocumentNumber + " has been completed by HR";
                            rc.Subject = ccfData.DocumentNumber + " - Completed Document";

                            bodyMail.Append("Dear " + ccfData.EmployeeName + ",<br /><br />");
                            bodyMail.AppendLine();
                            bodyMail.Append("Your Car Complaint Form " + ccfData.DocumentNumber + " Has Been Completed By " + input.UserId + "<br /><br />");
                            bodyMail.AppendLine();
                            bodyMail.AppendLine();
                            bodyMail.Append("Click Detail <a href='" + webRootUrl + "/TraCcf/DetailsCcf/DetailsCcf?TraCcfId=" + ccfData.TraCcfId + "&isPersonalDashboard=True" + "'> HERE </a><br /><br />");
                            bodyMail.AppendLine();
                            bodyMail.AppendLine();
                            bodyMail.Append("Thanks<br /><br />");
                            bodyMail.AppendLine();
                            bodyMail.Append("Regards,<br />");
                            bodyMail.AppendLine();
                            bodyMail.Append("HR Team");
                            bodyMail.AppendLine();

                            rc.To.Add(creatorDataEmail);

                            foreach (var item in hrEmailList)
                            {
                                rc.CC.Add(item);
                            }
                            if (creatorDataEmailComplaintFor != creatorDataEmail)
                            {
                                rc.CC.Add(creatorDataEmailComplaintFor);
                            }
                        }
                        break;
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

        private void ModifiedDocument(CcfWorkflowDocumentInput input)
        {
            var dbData = _ccfService.GetCcf().Where(x => x.TRA_CCF_ID == input.DocumentId).FirstOrDefault();

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);
        }

        private void SubmitDocument(CcfWorkflowDocumentInput input)
        {
            var dbData = _ccfService.GetCcfById(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //if (dbData.DOCUMENT_STATUS == (int) Enums.DocumentStatus.AssignedForUser)
            //{
            //    dbData.DOCUMENT_STATUS = (int)Enums.DocumentStatus.InProgress;
            //}
            //else if (dbData.DOCUMENT_STATUS == (int)Enums.DocumentStatus.AssignedForFleet)
            //{
            //    dbData.DOCUMENT_STATUS = (int)Enums.DocumentStatus.InProgress;

            //}

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }

        private void CancelDocument(CcfWorkflowDocumentInput input)
        {
            var dbData = _ccfService.GetCcfById(input.DocumentId);

            dbData.MODIFIED_BY = input.UserId;
            dbData.MODIFIED_DATE = DateTime.Now;

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }

        public TraCcfDto GetCcfById(long id, bool? Archive = null)
        {
            if (Archive.HasValue)
            {
                var archData = _archCcfService.GetCcfById(id);
                return Mapper.Map<TraCcfDto>(archData);
            }
            var data = _ccfService.GetCcfById(id);
            var retData = Mapper.Map<TraCcfDto>(data);
            return retData;
        }

        public List<TraCcfDto> GetCcfByParam(CcfParamInput param)
        {
            if (param.Table == "2")
            {
                var archData = _archCcfService.GetCcf();
                return Mapper.Map<List<TraCcfDto>>(archData);
            }
            var data = _ccfService.GetCcf();
            var retData = Mapper.Map<List<TraCcfDto>>(data);
            return retData;
        }

        public void CancelCcf(long id, int Remark, string user)
        {
            _ccfService.CancelCcf(id, Remark, user);
        }

        public List<TraCcfDto> GetCcfPersonal(Login userLogin)
        {
            var data = _ccfService.GetCcf().Where(x => (x.EMPLOYEE_ID == userLogin.EMPLOYEE_ID || x.EMPLOYEE_ID_COMPLAINT_FOR == userLogin.EMPLOYEE_ID)).ToList();
            var retData = Mapper.Map<List<TraCcfDto>>(data);
            return retData;
        }

        private void InProgress(CcfWorkflowDocumentInput input)
        {
            var dbData = _ccfService.GetCcfById(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.DOCUMENT_STATUS == (int)Enums.DocumentStatus.AssignedForHR)
            {
                dbData.DOCUMENT_STATUS = (int)Enums.DocumentStatus.InProgress;
            }
            else if (dbData.DOCUMENT_STATUS == (int)Enums.DocumentStatus.AssignedForFleet)
            {
                dbData.DOCUMENT_STATUS = (int)Enums.DocumentStatus.InProgress;
            }

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }

        private void CompleteDocument(CcfWorkflowDocumentInput input)
        {
            var dbData = _ccfService.GetCcfById(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.DOCUMENT_STATUS == (int)Enums.DocumentStatus.Completed)
            {
                dbData.DOCUMENT_STATUS = (int)Enums.DocumentStatus.Completed;
            }

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }

        public List<TraCcfDto> GetCcfD1(int traCCFid, bool? Archive = null)
        {
            var dataCcf = new TRA_CCF();
            var archDataCcf = new ARCH_TRA_CCF();
            var data = new List<TRA_CCF_DETAIL>();
            var archData = new List<ARCH_TRA_CCF_DETAIL>();
            var redata = new List<TraCcfDto>();
            if (Archive.HasValue)
            {
                archDataCcf = _archCcfService.GetCcfById(traCCFid);
                archData = _archCcfService.GetCcfD1().Where(x => x.TRA_CCF_ID == traCCFid).ToList();
                redata = Mapper.Map<List<TraCcfDto>>(data);
            }
            dataCcf = _ccfService.GetCcfById(traCCFid);
            data = _ccfService.GetCcfD1().Where(x => x.TRA_CCF_ID == traCCFid).ToList();
            redata = Mapper.Map<List<TraCcfDto>>(data);
            return redata;
        }

        public List<TraCcfDto> GetCcfDetil()
        {
            var data = _ccfService.GetCcfDetil();
            var redata = Mapper.Map<List<TraCcfDto>>(data);
            return redata;
        }

        public string GetCcfDetil(long traCcfId)
        {
            throw new NotImplementedException();
        }

        public void NotifEmail()
        {
            var GetCCFDetail = _ccfService.GetCcfDetil().Where(x => x.COORDINATOR_RESPONSE_DATE == null && (x.COMPLAINT_DATE == null  ? false : x.COMPLAINT_DATE.Value.AddDays(2) <= DateTime.Today)).ToList();
            var SendNotifCCFScheduler = ConfigurationManager.AppSettings["CCF_Notif_scheduler"];
            var ComplaintCategory = _complaintCategory.GetComplaintCategories().Where(x => x.IS_ACTIVE).ToList();
            var ComplaintFleet = ComplaintCategory.Where(x => (x.ROLE_TYPE == null ? "" : x.ROLE_TYPE.ToUpper()) == "FLEET").Select(x => new{ x.MST_COMPLAINT_CATEGORY_ID}).ToList();
            var ComplaintHR = ComplaintCategory.Where(x => (x.ROLE_TYPE == null ? "" : x.ROLE_TYPE.ToUpper()) == "HR").Select(x => new { x.MST_COMPLAINT_CATEGORY_ID }).ToList();

            var CCFListForFleet = new List<long>();
            var CCFListForHR = new List<long>();

            foreach (var item in GetCCFDetail)
            {
                var diffDays = (DateTime.Today.Year - item.COMPLAINT_DATE.Value.Year)*12 + (DateTime.Today.Month - item.COMPLAINT_DATE.Value.Month)*30 + (DateTime.Today.Day - item.COMPLAINT_DATE.Value.Day);

                if (diffDays % Convert.ToInt32(SendNotifCCFScheduler) == 0)
                {
                    var GetCCF = _ccfService.GetCcfById(item.TRA_CCF_ID);
                    if(ComplaintFleet.Where(x => x.MST_COMPLAINT_CATEGORY_ID == (GetCCF == null ? 0 : GetCCF.COMPLAINT_CATEGORY)).Count() > 0)
                    {
                        CCFListForFleet.Add(item.TRA_CCF_ID);
                    }
                    if (ComplaintHR.Where(x => x.MST_COMPLAINT_CATEGORY_ID == (GetCCF == null ? 0 : GetCCF.COMPLAINT_CATEGORY)).Count() > 0)
                    {
                        CCFListForHR.Add(item.TRA_CCF_ID);
                    }
                }
            }

            bool isSend;
            if (CCFListForHR.Count > 0)
            {
                isSend = DoEmailCCFNotif("HR", CCFListForHR);
            }

            if (CCFListForFleet.Count > 0)
            {
                isSend = DoEmailCCFNotif("FLEET", CCFListForFleet);
            }

        }
        public bool DoEmailCCFNotif(string CCFFor, List<long> CCFList)
        {
            var rc = new CcfMailNotification();
            var bodyMail = new StringBuilder();
            var CC = ConfigurationManager.AppSettings["CC_MAIL"];
            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
            var typeEnv = ConfigurationManager.AppSettings["Environment"];
            var serverIntranet = ConfigurationManager.AppSettings["ServerIntranet"];
            bool isSend = false;

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

            var hrQueryEmail = "SELECT EMAIL FROM " + serverIntranet + ".[dbo].[tbl_ADSI_User] WHERE FULL_NAME IN (" + hrList + ")";
            var fleetQueryEmail = "SELECT EMAIL FROM " + serverIntranet + ".[dbo].[tbl_ADSI_User] WHERE FULL_NAME IN (" + fleetList + ")";

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

            rc.Subject = "CCF - Waiting For Response";
            if(CCFFor == "FLEET")
            {
                bodyMail.Append("Dear FLEET, <br /><br />");
            }
            else if (CCFFor == "HR")
            {
                bodyMail.Append("Dear HR, <br /><br />");
            }
            bodyMail.AppendLine();
            bodyMail.Append("Bellow are the list of CCF That waiting for Response :<br />");
            bodyMail.AppendLine();
            bodyMail.Append("<table>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td style = 'border: 1px solid black; padding : 5px' >Doc No</td><td style = 'border: 1px solid black; padding : 5px' >URL</td></tr>");
            bodyMail.AppendLine();

            foreach (var CcfDoc in CCFList)
            {
                var GetCCFDoc = _ccfService.GetCcfById(CcfDoc);

                bodyMail.Append("<tr><td style = 'border: 1px solid black; padding : 5px' >" + (GetCCFDoc == null ? "" : GetCCFDoc.DOCUMENT_NUMBER )+ "</td><td style = 'border: 1px solid black; padding : 5px' ><a href='" + webRootUrl + "/TraCcf/ResponseCoordinator?TraCcfId=" + CcfDoc + "&isPersonalDashboard=False" + "'> HERE </a></td></tr>");
                bodyMail.AppendLine();
            }

            bodyMail.Append("</table>");
            bodyMail.AppendLine();
            bodyMail.Append("<br /><br />Thank you <br />");
            bodyMail.AppendLine();
            bodyMail.Append("Best Regards,<br />");
            bodyMail.AppendLine();

            rc.IsCCExist = false;
            rc.Body = bodyMail.ToString();

            
            if (rc.CC.Count > 0) rc.IsCCExist = true;

            if (CCFFor == "FLEET")
            {
                foreach (var item in fleetEmailList)
                {
                    rc.To.Add(item);
                }
            }

            if (CCFFor == "HR")
            {
                foreach (var item in hrEmailList)
                {
                    rc.To.Add(item);
                }
            }
            rc.IsCCExist = false;

            if (rc.IsCCExist)
                //Send email with CC
                isSend = _messageService.SendEmailToListWithCC(rc.To, rc.CC, rc.Subject, rc.Body, true);
            else
                isSend = _messageService.SendEmailToList(rc.To, rc.Subject, rc.Body, true);

            return isSend;

        }

    }
}
