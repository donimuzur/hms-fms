using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FMS.Core;
using FMS.Core.Exceptions;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.DAL.Services;
using FMS.Utils;
using AutoMapper;
using System.Data.SqlClient;
using System.Data.Entity.Core.EntityClient;

namespace FMS.BLL.Temporary
{
    public class TemporaryBLL : ITraTemporaryBLL
    {
        private ITemporaryService _TemporaryService;
        private IUnitOfWork _uow;

        private ISettingService _settingService;
        private IDocumentNumberService _docNumberService;
        private IWorkflowHistoryService _workflowService;
        private IMessageService _messageService;
        private IEmployeeService _employeeService;
        private IRemarkService _remarkService;
        private ILocationMappingService _locationMappingService;
        private IFleetService _fleetService;
        private IPriceListService _priceListService;
        private IVendorService _vendorService;
        private IGroupCostCenterService _groupCostService;
        private IVehicleSpectService _vehicleSpectService;

        public TemporaryBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _TemporaryService = new TemporaryService(_uow);

            _settingService = new SettingService(_uow);
            _docNumberService = new DocumentNumberService(_uow);
            _workflowService = new WorkflowHistoryService(_uow);
            _messageService = new MessageService(_uow);
            _employeeService = new EmployeeService(_uow);
            _remarkService = new RemarkService(_uow);
            _locationMappingService = new LocationMappingService(_uow);
            _fleetService = new FleetService(_uow);
            _priceListService = new PriceListService(_uow);
            _vendorService = new VendorService(_uow);
            _groupCostService = new GroupCostCenterService(_uow);
            _vehicleSpectService = new VehicleSpectService(_uow);
        }

        public List<TemporaryDto> GetTemporary(Login userLogin, bool isCompleted)
        {
            var settingData = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
            var benefitType = settingData.Where(x => x.SETTING_NAME.ToUpper() == "BENEFIT").FirstOrDefault().MST_SETTING_ID.ToString();
            var wtcType = settingData.Where(x => x.SETTING_NAME.ToUpper() == "WTC").FirstOrDefault().MST_SETTING_ID.ToString();

            var locationMapping = _locationMappingService.GetLocationMapping().Where(x => x.IS_ACTIVE).OrderByDescending(x => x.VALIDITY_FROM).ToList();

            var data = _TemporaryService.GetTemp(userLogin, isCompleted, benefitType, wtcType);
            var retData = Mapper.Map<List<TemporaryDto>>(data);

            foreach (var item in retData)
            {
                var region = locationMapping.Where(x => x.LOCATION.ToUpper() == item.LOCATION_CITY.ToUpper()).FirstOrDefault();

                item.REGIONAL = region == null ? string.Empty : region.REGION;

                item.VEHICLE_TYPE_NAME = "BENEFIT";

                if (item.VEHICLE_TYPE == wtcType)
                {
                    item.VEHICLE_TYPE_NAME = "WTC";
                }
            }

            return retData;
        }

        public List<TemporaryDto> GetTempPersonal(Login userLogin)
        {
            var data = _TemporaryService.GetAllTemp().Where(x => (x.EMPLOYEE_ID == userLogin.EMPLOYEE_ID && x.DOCUMENT_STATUS != Enums.DocumentStatus.Draft)
                                                                || x.CREATED_BY == userLogin.USER_ID
                                                                || x.APPROVED_FLEET == userLogin.USER_ID).ToList();
            var retData = Mapper.Map<List<TemporaryDto>>(data);

            var settingData = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
            var benefitType = settingData.Where(x => x.SETTING_NAME.ToUpper() == "BENEFIT").FirstOrDefault().MST_SETTING_ID.ToString();
            var wtcType = settingData.Where(x => x.SETTING_NAME.ToUpper() == "WTC").FirstOrDefault().MST_SETTING_ID.ToString();

            var locationMapping = _locationMappingService.GetLocationMapping().Where(x => x.IS_ACTIVE).OrderByDescending(x => x.VALIDITY_FROM).ToList();

            foreach (var item in retData)
            {
                var region = locationMapping.Where(x => x.LOCATION.ToUpper() == item.LOCATION_CITY.ToUpper()).FirstOrDefault();

                item.REGIONAL = region == null ? string.Empty : region.REGION;

                item.VEHICLE_TYPE_NAME = "BENEFIT";
                if (item.VEHICLE_TYPE == wtcType)
                {
                    item.VEHICLE_TYPE_NAME = "WTC";
                }
            }

            return retData;
        }

        public TemporaryDto Save(TemporaryDto item, Login userLogin)
        {
            TRA_TEMPORARY model;
            if (item == null)
            {
                throw new Exception("Invalid Data Entry");
            }

            try
            {
                bool changed = false;

                if (item.TRA_TEMPORARY_ID > 0)
                {
                    //update
                    model = _TemporaryService.GetTemporaryById(item.TRA_TEMPORARY_ID);

                    if (model == null)
                        throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                    Mapper.Map<TemporaryDto, TRA_TEMPORARY>(item, model);

                    changed = true;
                }
                else
                {
                    var inputDoc = new GenerateDocNumberInput();
                    inputDoc.Month = DateTime.Now.Month;
                    inputDoc.Year = DateTime.Now.Year;
                    inputDoc.DocType = (int)Enums.DocumentType.TMP;

                    item.DOCUMENT_NUMBER_TEMP = _docNumberService.GenerateNumber(inputDoc);
                    item.IS_ACTIVE = true;
                    item.EMPLOYEE_ID_CREATOR = userLogin.EMPLOYEE_ID;

                    model = Mapper.Map<TRA_TEMPORARY>(item);
                }

                _TemporaryService.saveTemporary(model, userLogin);
                _uow.SaveChanges();

                //set workflow history
                var input = new TempWorkflowDocumentInput()
                {
                    DocumentId = model.TRA_TEMPORARY_ID,
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

            return Mapper.Map<TemporaryDto>(model);
        }

        private void AddWorkflowHistory(TempWorkflowDocumentInput input)
        {
            var dbData = Mapper.Map<WorkflowHistoryDto>(input);

            dbData.ACTION_DATE = DateTime.Now;
            dbData.MODUL_ID = Enums.MenuList.TraTmp;
            dbData.REMARK_ID = null;

            _workflowService.Save(dbData);

        }

        public void TempWorkflow(TempWorkflowDocumentInput input)
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
                case Enums.ActionType.Cancel:
                    CancelDocument(input);
                    var _checkDocDraft = _workflowService.GetWorkflowHistoryByUser((int)Enums.MenuList.TraTmp, input.UserId)
                        .Where(x => x.ACTION == (int)Enums.ActionType.Submit && x.FORM_ID == input.DocumentId).FirstOrDefault();

                    if (_checkDocDraft == null)
                    {
                        isNeedSendNotif = false;
                    }

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
            }

            //todo sent mail
            if (isNeedSendNotif) SendEmailWorkflow(input);

            _uow.SaveChanges();
        }

        private void CancelDocument(TempWorkflowDocumentInput input)
        {
            var dbData = _TemporaryService.GetTemporaryById(input.DocumentId);

            dbData.MODIFIED_BY = input.UserId;
            dbData.MODIFIED_DATE = DateTime.Now;

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }

        private void CompleteDocument(TempWorkflowDocumentInput input)
        {
            var dbData = _TemporaryService.GetTemporaryById(input.DocumentId);

            dbData.MODIFIED_BY = input.UserId;
            dbData.MODIFIED_DATE = DateTime.Now;

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            dbData.DOCUMENT_STATUS = Enums.DocumentStatus.Completed;
            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }

        private void CreateDocument(TempWorkflowDocumentInput input)
        {
            var dbData = _TemporaryService.GetTemporaryById(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);
        }

        private void SubmitDocument(TempWorkflowDocumentInput input)
        {
            var dbData = _TemporaryService.GetTemporaryById(input.DocumentId);

            dbData.MODIFIED_BY = input.UserId;
            dbData.MODIFIED_DATE = DateTime.Now;

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            var vehTypeBenefit = _settingService.GetSetting().Where(x => x.SETTING_GROUP == "VEHICLE_TYPE" && x.SETTING_NAME == "BENEFIT").FirstOrDefault().MST_SETTING_ID;

            var isBenefit = dbData.VEHICLE_TYPE == vehTypeBenefit.ToString() ? true : false;

            dbData.DOCUMENT_STATUS = Enums.DocumentStatus.WaitingFleetApproval;

            if (!isBenefit)
            {
                dbData.DOCUMENT_STATUS = Enums.DocumentStatus.InProgress;
            }

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }

        private void ApproveDocument(TempWorkflowDocumentInput input)
        {
            var dbData = _TemporaryService.GetTemporaryById(input.DocumentId);

            dbData.MODIFIED_BY = input.UserId;
            dbData.MODIFIED_DATE = DateTime.Now;

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            dbData.DOCUMENT_STATUS = Enums.DocumentStatus.InProgress;
            dbData.APPROVED_FLEET = input.UserId;
            dbData.APPROVED_FLEET_DATE = DateTime.Now;
            dbData.EMPLOYEE_ID_FLEET_APPROVAL = input.EmployeeId;

            _uow.SaveChanges();

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }

        private void RejectDocument(TempWorkflowDocumentInput input)
        {
            var dbData = _TemporaryService.GetTemporaryById(input.DocumentId);

            dbData.MODIFIED_BY = input.UserId;
            dbData.MODIFIED_DATE = DateTime.Now;

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            dbData.DOCUMENT_STATUS = Enums.DocumentStatus.Draft;

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }

        private void SendEmailWorkflow(TempWorkflowDocumentInput input)
        {
            var tempData = Mapper.Map<TemporaryDto>(_TemporaryService.GetTemporaryById(input.DocumentId));

            var mailProcess = ProsesMailNotificationBody(tempData, input);

            //distinct double To email
            List<string> ListTo = mailProcess.To.Distinct().ToList();

            if (mailProcess.IsCCExist)
                //Send email with CC
                _messageService.SendEmailToListWithCC(ListTo, mailProcess.CC, mailProcess.Subject, mailProcess.Body, true, mailProcess.Attachments);
            else
                _messageService.SendEmailToList(ListTo, mailProcess.Subject, mailProcess.Body, true);

        }

        private class TempMailNotification
        {
            public TempMailNotification()
            {
                To = new List<string>();
                CC = new List<string>();
                IsCCExist = false;
                Attachments = new List<string>();
            }
            public string Subject { get; set; }
            public string Body { get; set; }
            public List<string> To { get; set; }
            public List<string> CC { get; set; }
            public List<string> Attachments { get; set; }
            public bool IsCCExist { get; set; }
        }

        private TempMailNotification ProsesMailNotificationBody(TemporaryDto tempData, TempWorkflowDocumentInput input)
        {
            var bodyMail = new StringBuilder();
            var rc = new TempMailNotification();
            var settingData = _settingService.GetSetting().ToList();

            var vehTypeBenefit = settingData.Where(x => x.SETTING_GROUP == "VEHICLE_TYPE" && x.SETTING_NAME == "BENEFIT").FirstOrDefault().MST_SETTING_ID;
            var vendorData = _vendorService.GetByShortName(tempData.VENDOR_NAME);
            var vendorEmail = vendorData == null ? string.Empty : vendorData.EMAIL_ADDRESS;
            var vendorName = vendorData == null ? string.Empty : vendorData.VENDOR_NAME;

            var isBenefit = tempData.VEHICLE_TYPE == vehTypeBenefit.ToString() ? true : false;

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
            var typeEnv = ConfigurationManager.AppSettings["Environment"];
            var serverIntranet = ConfigurationManager.AppSettings["ServerIntranet"];
            var employeeData = _employeeService.GetEmployeeById(tempData.EMPLOYEE_ID);
            var creatorData = _employeeService.GetEmployeeById(tempData.EMPLOYEE_ID_CREATOR);
            var fleetApprovalData = _employeeService.GetEmployeeById(tempData.EMPLOYEE_ID_FLEET_APPROVAL);

            var employeeDataEmail = employeeData == null ? string.Empty : employeeData.EMAIL_ADDRESS;
            var creatorDataEmail = creatorData == null ? string.Empty : creatorData.EMAIL_ADDRESS;
            var fleetApprovalDataEmail = fleetApprovalData == null ? string.Empty : fleetApprovalData.EMAIL_ADDRESS;

            var employeeDataName = employeeData == null ? string.Empty : employeeData.FORMAL_NAME;
            var creatorDataName = creatorData == null ? string.Empty : creatorData.FORMAL_NAME;
            var fleetApprovalDataName = fleetApprovalData == null ? string.Empty : fleetApprovalData.FORMAL_NAME;

            var hrList = string.Empty;
            var fleetList = string.Empty;

            var hrEmailList = new List<string>();
            var fleetEmailList = new List<string>();

            var hrRole = settingData.Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                && x.SETTING_VALUE.Contains("HR")).FirstOrDefault().SETTING_VALUE;
            var fleetRole = settingData.Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                && x.SETTING_VALUE.Contains("FLEET")).FirstOrDefault().SETTING_VALUE;

            var hrQuery = "SELECT 'PMI\\' + sAMAccountName AS sAMAccountName FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + hrRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";
            var fleetQuery = "SELECT 'PMI\\' + sAMAccountName AS sAMAccountName FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + fleetRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";

            if (typeEnv == "VTI")
            {
                hrQuery = "SELECT 'PMI\\' + LOGIN AS LOGIN FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + hrRole + "'";
                fleetQuery = "SELECT 'PMI\\' + LOGIN AS LOGIN FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + fleetRole + "'";
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

            switch (input.ActionType)
            {
                case Enums.ActionType.Submit:
                    //if submit from FLEET for wtc
                    if (tempData.CREATED_BY == input.UserId && !isBenefit)
                    {
                        rc.Subject = tempData.DOCUMENT_NUMBER_TEMP + " - Vendor Information Temporary Car";

                        bodyMail.Append("Dear Vendor " + vendorName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("You have new car request. Please check attached file<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team");
                        bodyMail.AppendLine();

                        //if vendor exists
                        if (!string.IsNullOrEmpty(vendorEmail))
                        {
                            foreach (var item in input.Attachments)
                            {
                                rc.Attachments.Add(item);
                            }

                            rc.To.Add(vendorEmail);
                        }

                        foreach (var item in fleetEmailList)
                        {
                            rc.CC.Add(item);
                        }

                        rc.CC.Add(employeeDataEmail);

                        
                    }
                    //if submit from HR for benefit
                    if (tempData.CREATED_BY == input.UserId && isBenefit)
                    {
                        rc.Subject = tempData.DOCUMENT_NUMBER_TEMP + " - Temporary Car Request";

                        bodyMail.Append("Dear Fleet Team,<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("new temporary car has been recorded as " + tempData.DOCUMENT_NUMBER_TEMP + "<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Please submit detail vehicle information <a href='" + webRootUrl + "/TraTemporary/Edit/" + tempData.TRA_TEMPORARY_ID + "?isPersonalDashboard=False" + "'>HERE</a><br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("For any assistance please contact " + creatorDataName + "<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("HR Team");
                        bodyMail.AppendLine();

                        foreach (var item in fleetEmailList)
                        {
                            rc.To.Add(item);
                        }

                        foreach (var item in hrEmailList)
                        {
                            rc.CC.Add(item);
                        }

                        rc.CC.Add(employeeDataEmail);
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Approve:
                    //if Fleet Approve for benefit
                    if (input.UserRole == Enums.UserRole.Fleet && isBenefit)
                    {
                        rc.Subject = tempData.DOCUMENT_NUMBER_TEMP + " - Vendor Information Temporary Car";

                        bodyMail.Append("Dear Vendor " + vendorName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("You have new car request. Please check attached file<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team");
                        bodyMail.AppendLine();

                        //if vendor exists
                        if (!string.IsNullOrEmpty(vendorEmail))
                        {
                            foreach (var item in input.Attachments)
                            {
                                rc.Attachments.Add(item);
                            }

                            rc.To.Add(vendorEmail);
                        }

                        rc.To.Add(creatorDataEmail);

                        foreach (var item in fleetEmailList)
                        {
                            rc.CC.Add(item);
                        }

                        rc.CC.Add(employeeDataEmail);
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Reject:
                    //if Fleet Reject Benefit
                    if (input.UserRole == Enums.UserRole.Fleet && isBenefit)
                    {
                        rc.Subject = tempData.DOCUMENT_NUMBER_TEMP + " - Temporary Car Request";

                        bodyMail.Append("Dear " + creatorDataName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Your temporary car request " + tempData.DOCUMENT_NUMBER_TEMP + " has been rejected by " + fleetApprovalDataName + " for below reason : " + _remarkService.GetRemarkById(input.Comment.Value).REMARK + "<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Please revised and re-submit your request <a href='" + webRootUrl + "/TraTemporary/Edit/" + tempData.TRA_TEMPORARY_ID + "?isPersonalDashboard=False" + "'>HERE</a><br />");
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

                        rc.CC.Add(employeeDataEmail);
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Completed:
                    rc.Subject = tempData.DOCUMENT_NUMBER_TEMP + " - Completed Document";

                    bodyMail.Append("Dear " + creatorDataName + ",<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Your temporary car request " + tempData.DOCUMENT_NUMBER_TEMP + " has been completed by system<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Click <a href='" + webRootUrl + "/TraTemporary/Detail/" + tempData.TRA_TEMPORARY_ID + "?isPersonalDashboard=True" + "'>HERE</a> to monitor your request<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Thanks<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Regards,<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Fleet Team");
                    bodyMail.AppendLine();

                    rc.To.Add(creatorDataEmail);
                    rc.CC.Add(employeeDataEmail);
                    rc.CC.Add(fleetApprovalDataEmail);
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Cancel:
                    rc.Subject = tempData.DOCUMENT_NUMBER_TEMP + " - Cancelled Document";

                    bodyMail.Append("Dear " + employeeDataName + ",<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Your temporary car request " + tempData.DOCUMENT_NUMBER_TEMP + " has been cancelled by " + creatorDataName + "<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Click <a href='" + webRootUrl + "/TraTemporary/Detail/" + tempData.TRA_TEMPORARY_ID + "?isPersonalDashboard=True" + "'>HERE</a> to monitor your request<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Thanks<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Regards,<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Fleet Team");
                    bodyMail.AppendLine();

                    rc.To.Add(employeeDataEmail);
                    rc.CC.Add(creatorDataEmail);

                    foreach (var item in fleetEmailList)
                    {
                        rc.CC.Add(item);
                    }

                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.InProgress:
                    rc.Subject = tempData.DOCUMENT_NUMBER_TEMP + " - Document In Progress";

                    bodyMail.Append("Dear " + employeeDataName + ",<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Your temporary car request " + tempData.DOCUMENT_NUMBER_TEMP + " will be arrived at " + tempData.VENDOR_CONTRACT_START_DATE.Value.ToString("dd-MMM-yyyy") + "<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Click <a href='" + webRootUrl + "/TraTemporary/Detail/" + tempData.TRA_TEMPORARY_ID + "?isPersonalDashboard=True" + "'>HERE</a> to monitor your request<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Thanks<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Regards,<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Fleet Team");
                    bodyMail.AppendLine();

                    rc.To.Add(employeeDataEmail);
                    rc.CC.Add(creatorDataEmail);

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

        public TemporaryDto GetTempById(long id)
        {
            var data = _TemporaryService.GetTemporaryById(id);
            var retData = Mapper.Map<TemporaryDto>(data);
            return retData;
        }

        public List<VehicleFromVendorUpload> ValidationUploadDocumentProcess(List<VehicleFromVendorUpload> inputs, int id)
        {
            var messageList = new List<string>();
            var messageListStopper = new List<string>();
            var outputList = new List<VehicleFromVendorUpload>();

            var dataTemp = _TemporaryService.GetTemporaryById(id);

            var policeNumberActive = _fleetService.GetFleet().Where(x => x.IS_ACTIVE && !string.IsNullOrEmpty(x.POLICE_NUMBER)
                                                                            && x.MST_FLEET_ID != dataTemp.CFM_IDLE_ID).ToList();

            foreach (var inputItem in inputs)
            {
                messageList.Clear();

                //check police number active in mst_fleet
                if (policeNumberActive.Where(x => x.POLICE_NUMBER.ToLower() == inputItem.PoliceNumber.ToLower()).FirstOrDefault() != null)
                {
                    messageList.Add("Police number already exists in master fleet");
                    messageListStopper.Add("Police number already exists in master fleet");
                }

                //check temp number
                if (dataTemp.DOCUMENT_NUMBER.ToLower() != inputItem.CsfNumber.ToLower())
                {
                    messageList.Add("Temporary Number not valid");
                    messageListStopper.Add("Temporary Number not valid");
                }

                //check employee name
                if (dataTemp.EMPLOYEE_NAME.ToLower() != inputItem.EmployeeName.ToLower())
                {
                    messageList.Add("Employee name not same as employee name request");
                    messageListStopper.Add("Employee name not same as employee name request");
                }

                //check manufacturer
                if (dataTemp.MANUFACTURER.ToLower() != inputItem.Manufacturer.ToLower())
                {
                    messageList.Add("Manufacturer not same as employee request");
                }

                //check models
                if (dataTemp.MODEL.ToLower() != inputItem.Models.ToLower())
                {
                    messageList.Add("Models not same as employee request");
                }

                //check series
                if (dataTemp.SERIES.ToLower() != inputItem.Series.ToLower())
                {
                    messageList.Add("Series not same as employee request");
                }

                //check body type
                if (dataTemp.BODY_TYPE.ToLower() != inputItem.BodyType.ToLower())
                {
                    messageList.Add("Body Type not same as employee request");
                }

                if (dataTemp.COLOR != null)
                {
                    //check color
                    if (dataTemp.COLOR.ToLower() != inputItem.Color.ToLower())
                    {
                        messageList.Add("Colour not same as employee request");
                    }
                }

                #region -------------- Set Message Info if exists ---------------

                if (messageList.Count > 0)
                {
                    inputItem.MessageError = "";
                    foreach (var message in messageList)
                    {
                        inputItem.MessageError += message + ";";
                    }
                }

                else
                {
                    inputItem.MessageError = string.Empty;
                }

                #endregion

                #region -------------- Set Message Stopper Info if exists ---------------

                if (messageListStopper.Count > 0)
                {
                    inputItem.MessageErrorStopper = "";
                    foreach (var message in messageListStopper)
                    {
                        inputItem.MessageErrorStopper += message + ";";
                    }
                }

                else
                {
                    inputItem.MessageErrorStopper = string.Empty;
                }

                #endregion

                outputList.Add(inputItem);
            }

            return outputList;
        }

        public List<VehicleFromVendorUpload> ValidationUploadDocumentProcessMassUpload(List<VehicleFromVendorUpload> inputs)
        {
            var messageList = new List<string>();
            var messageListStopper = new List<string>();
            var outputList = new List<VehicleFromVendorUpload>();

            var dataAllTemp = _TemporaryService.GetAllTemp().Where(x => x.DOCUMENT_STATUS == Enums.DocumentStatus.InProgress);

            var policeNumberActive = _fleetService.GetFleet().Where(x => x.IS_ACTIVE && !string.IsNullOrEmpty(x.POLICE_NUMBER)).ToList();

            foreach (var inputItem in inputs)
            {
                messageList.Clear();

                var dataTemp = dataAllTemp.Where(x => x.DOCUMENT_NUMBER.ToLower() == inputItem.CsfNumber.ToLower()).FirstOrDefault();

                //check temp number
                if (dataTemp == null)
                {
                    messageList.Add("Temporary Number not valid");
                    messageListStopper.Add("Temporary Number not valid");
                }
                else
                {
                    //check police number active in mst_fleet
                    if (policeNumberActive.Where(x => x.POLICE_NUMBER.ToLower() == inputItem.PoliceNumber.ToLower()
                                                            && x.MST_FLEET_ID != dataTemp.CFM_IDLE_ID).FirstOrDefault() != null)
                    {
                        messageList.Add("Police number already exists in master fleet");
                        messageListStopper.Add("Police number already exists in master fleet");
                    }

                    //check employee name
                    if (dataTemp.EMPLOYEE_NAME.ToLower() != inputItem.EmployeeName.ToLower())
                    {
                        messageList.Add("Employee name not same as employee name request");
                        messageListStopper.Add("Employee name not same as employee name request");
                    }

                    //check manufacturer
                    if (dataTemp.MANUFACTURER.ToLower() != inputItem.Manufacturer.ToLower())
                    {
                        messageList.Add("Manufacturer not same as employee request");
                    }

                    //check models
                    if (dataTemp.MODEL.ToLower() != inputItem.Models.ToLower())
                    {
                        messageList.Add("Models not same as employee request");
                    }

                    //check series
                    if (dataTemp.SERIES.ToLower() != inputItem.Series.ToLower())
                    {
                        messageList.Add("Series not same as employee request");
                    }

                    //check body type
                    if (dataTemp.BODY_TYPE.ToLower() != inputItem.BodyType.ToLower())
                    {
                        messageList.Add("Body Type not same as employee request");
                    }

                    if (dataTemp.COLOR != null)
                    {
                        //check color
                        if (dataTemp.COLOR.ToLower() != inputItem.Color.ToLower())
                        {
                            messageList.Add("Colour not same as employee request");
                        }
                    }
                }

                #region -------------- Set Message Info if exists ---------------

                if (messageList.Count > 0)
                {
                    inputItem.MessageError = "";
                    foreach (var message in messageList)
                    {
                        inputItem.MessageError += message + ";";
                    }
                }

                else
                {
                    inputItem.MessageError = string.Empty;
                }

                #endregion

                #region -------------- Set Message Stopper Info if exists ---------------

                if (messageListStopper.Count > 0)
                {
                    inputItem.MessageErrorStopper = "";
                    foreach (var message in messageListStopper)
                    {
                        inputItem.MessageErrorStopper += message + ";";
                    }
                }

                else
                {
                    inputItem.MessageErrorStopper = string.Empty;
                }

                #endregion

                outputList.Add(inputItem);
            }

            return outputList;
        }

        public void CheckTempInProgress()
        {
            var allTemp = _TemporaryService.GetAllTemp().Where(x => x.VENDOR_CONTRACT_START_DATE != null).ToList();

            var listTempInProgress = allTemp.Where(x => x.DOCUMENT_STATUS == Enums.DocumentStatus.InProgress
                                                                        && x.VENDOR_CONTRACT_START_DATE.Value < DateTime.Now
                                                                        && !string.IsNullOrEmpty(x.VENDOR_PO_NUMBER)).ToList();

            foreach (var item in listTempInProgress)
            {
                //change status completed
                var input = new TempWorkflowDocumentInput();
                input.ActionType = Enums.ActionType.Completed;
                input.UserId = "SYSTEM";
                input.DocumentId = item.TRA_TEMPORARY_ID;
                input.DocumentNumber = item.DOCUMENT_NUMBER;

                TempWorkflow(input);

                //inactive cfm idle
                var cfmidleData = _fleetService.GetFleet().Where(x => x.IS_ACTIVE && x.POLICE_NUMBER == item.VENDOR_POLICE_NUMBER
                                                                          && x.VEHICLE_USAGE == "CFM IDLE").FirstOrDefault();

                if (cfmidleData != null)
                {
                    var endDateCfm = item.VENDOR_CONTRACT_START_DATE.Value.AddDays(-1);

                    cfmidleData.DOCUMENT_NUMBER = item.DOCUMENT_NUMBER;
                    cfmidleData.END_DATE = endDateCfm;
                    cfmidleData.IS_ACTIVE = false;
                    _fleetService.save(cfmidleData);
                }

                //add new master fleet
                MST_FLEET dbFleet;

                var getZonePriceList = _locationMappingService.GetLocationMapping().Where(x => x.BASETOWN == item.LOCATION_CITY
                                                                                                 && x.IS_ACTIVE).FirstOrDefault();

                var vSpecList = _vehicleSpectService.GetVehicleSpect().Where(x => x.MANUFACTURER == item.MANUFACTURER
                                                                        && x.MODEL == item.MODEL
                                                                        && x.SERIES == item.SERIES
                                                                        && x.BODY_TYPE == item.BODY_TYPE
                                                                        && x.IS_ACTIVE).FirstOrDefault();

                var functionList = _groupCostService.GetGroupCostCenter().Where(x => x.COST_CENTER == item.COST_CENTER).FirstOrDefault();

                var vehType = string.Empty;
                var vehUsage = string.Empty;
                var projectName = string.Empty;
                var isProject = false;
                var hmsPrice = item.PRICE == null ? 0 : item.PRICE;
                var address = getZonePriceList == null ? "" : getZonePriceList.ADDRESS;
                var regional = getZonePriceList == null ? "" : getZonePriceList.REGION;
                var function = functionList == null ? "" : functionList.FUNCTION_NAME;
                var fuelType = vSpecList == null ? string.Empty : vSpecList.FUEL_TYPE;
                var transmission = vSpecList == null ? string.Empty : vSpecList.TRANSMISSION;

                if (!string.IsNullOrEmpty(item.VEHICLE_TYPE))
                {
                    var vehTypeData = _settingService.GetSettingById(Convert.ToInt32(item.VEHICLE_TYPE));
                    if (vehTypeData != null)
                    {
                        vehType = vehTypeData.SETTING_VALUE.ToUpper();
                    }
                }

                if (!string.IsNullOrEmpty(item.VEHICLE_USAGE))
                {
                    var vehUsageData = _settingService.GetSettingById(Convert.ToInt32(item.VEHICLE_USAGE));
                    if (vehUsageData != null)
                    {
                        vehUsage = vehUsageData.SETTING_VALUE.ToUpper();
                    }
                }

                if (!string.IsNullOrEmpty(item.PROJECT_NAME))
                {
                    var projectNameData = _settingService.GetSettingById(Convert.ToInt32(item.PROJECT_NAME));
                    if (projectNameData != null)
                    {
                        projectName = projectNameData.SETTING_VALUE.ToUpper();
                        isProject = true;
                        if (projectName == "NO PROJECT")
                        {
                            isProject = false;
                        }
                    }
                }

                dbFleet = Mapper.Map<MST_FLEET>(item);
                dbFleet.IS_ACTIVE = true;
                dbFleet.CREATED_DATE = DateTime.Now;
                dbFleet.VEHICLE_TYPE = vehType;
                dbFleet.VEHICLE_USAGE = vehUsage;
                dbFleet.SUPPLY_METHOD = "TEMPORARY";
                dbFleet.PROJECT = isProject;
                dbFleet.PROJECT_NAME = projectName;
                dbFleet.MONTHLY_HMS_INSTALLMENT = hmsPrice;
                dbFleet.TOTAL_MONTHLY_CHARGE = hmsPrice + (item.VAT_DECIMAL == null ? 0 : item.VAT_DECIMAL.Value);
                dbFleet.FUEL_TYPE = fuelType;
                dbFleet.ADDRESS = address;
                dbFleet.REGIONAL = regional;
                dbFleet.VEHICLE_FUNCTION = function;
                dbFleet.TRANSMISSION = transmission;

                _fleetService.save(dbFleet);

                _uow.SaveChanges();
            }
        }


        public bool CheckTempExistsInFleet(TemporaryDto item)
        {
            var isExist = false;

            var vehicleType = _settingService.GetSettingById(Convert.ToInt32(item.VEHICLE_TYPE)).SETTING_VALUE.ToUpper();

            var existData = _fleetService.GetFleet().Where(x => x.IS_ACTIVE && x.EMPLOYEE_ID == item.EMPLOYEE_ID
                                                                && x.VEHICLE_TYPE == vehicleType
                                                                && item.START_DATE <= x.END_CONTRACT
                                                                && x.SUPPLY_METHOD == "TEMPORARY").ToList();

            if (existData.Count > 0)
            {
                isExist = true;
            }

            return isExist;
        }


        public bool CheckTempOpenExists(TemporaryDto item)
        {
            var isExist = false;

            var benefitType = _settingService.GetSetting().Where(x => x.SETTING_NAME.ToUpper() == "BENEFIT").FirstOrDefault().MST_SETTING_ID.ToString();

            var existDataOpen = _TemporaryService.GetAllTemp().Where(x => x.EMPLOYEE_ID == item.EMPLOYEE_ID && x.DOCUMENT_STATUS != Enums.DocumentStatus.Cancelled
                                                                       && x.DOCUMENT_STATUS != Enums.DocumentStatus.Completed
                                                                       && x.VEHICLE_TYPE == benefitType).ToList();

            if (existDataOpen.Count > 0)
            {
                isExist = true;
            }

            return isExist;
        }


        public List<TemporaryDto> GetList()
        {
            var data = _TemporaryService.GetAllTemp();

            return Mapper.Map<List<TemporaryDto>>(data);
        }

        public void CancelTemp(long id, int Remark, string user)
        {
            _TemporaryService.CancelTemp(id, Remark, user);
        }
    }
}
