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
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.Office.Interop.Excel;

namespace FMS.BLL.Csf
{
    public class CsfBLL : ITraCsfBLL
    {
        private ICsfService _CsfService;
        private IUnitOfWork _uow;

        private IDocumentNumberService _docNumberService;
        private IWorkflowHistoryService _workflowService;
        private ISettingService _settingService;
        private IMessageService _messageService;
        private IEmployeeService _employeeService;
        private IEpafService _epafService;
        private IRemarkService _remarkService;
        private ITemporaryService _temporaryService;
        private IFleetService _fleetService;
        private IPriceListService _priceListService;
        private ILocationMappingService _locationMappingService;
        private IVehicleSpectService _vehicleSpectService;
        private IVendorService _vendorService;

        public CsfBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _CsfService = new CsfService(_uow);

            _docNumberService = new DocumentNumberService(_uow);
            _workflowService = new WorkflowHistoryService(_uow);
            _settingService = new SettingService(_uow);
            _messageService = new MessageService(_uow);
            _employeeService = new EmployeeService(_uow);
            _epafService = new EpafService(_uow);
            _remarkService = new RemarkService(_uow);
            _temporaryService = new TemporaryService(_uow);
            _fleetService = new FleetService(_uow);
            _priceListService = new PriceListService(_uow);
            _locationMappingService = new LocationMappingService(_uow);
            _vehicleSpectService = new VehicleSpectService(_uow);
            _vendorService = new VendorService(_uow);
        }

        public List<TraCsfDto> GetCsf(Login userLogin, bool isCompleted)
        {
            var settingData = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
            var benefitType = settingData.Where(x => x.SETTING_NAME.ToUpper() == "BENEFIT").FirstOrDefault().MST_SETTING_ID.ToString();
            var wtcType = settingData.Where(x => x.SETTING_NAME.ToUpper() == "WTC").FirstOrDefault().MST_SETTING_ID.ToString();

            var locationMapping = _locationMappingService.GetLocationMapping().Where(x => x.IS_ACTIVE).OrderByDescending(x => x.VALIDITY_FROM).ToList();

            var data = _CsfService.GetCsf(userLogin, isCompleted, benefitType, wtcType);
            var retData = Mapper.Map<List<TraCsfDto>>(data);

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

        public List<TraCsfDto> GetCsfPersonal(Login userLogin)
        {
            var data = _CsfService.GetAllCsf().Where(x => (x.EMPLOYEE_ID == userLogin.EMPLOYEE_ID && x.DOCUMENT_STATUS != Enums.DocumentStatus.Draft) 
                                                                || x.CREATED_BY == userLogin.USER_ID
                                                                || x.APPROVED_FLEET == userLogin.USER_ID).ToList();
            var retData = Mapper.Map<List<TraCsfDto>>(data);

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

        public TraCsfDto Save(TraCsfDto item, Login userLogin)
        {
            TRA_CSF model;
            if (item == null)
            {
                throw new Exception("Invalid Data Entry");
            }

            try
            {
                if (item.TRA_CSF_ID > 0)
                {
                    //update
                    model = _CsfService.GetCsfById(item.TRA_CSF_ID);

                    if (model == null)
                        throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                    Mapper.Map<TraCsfDto, TRA_CSF>(item, model);
                }
                else
                {
                    var inputDoc = new GenerateDocNumberInput();
                    inputDoc.Month = DateTime.Now.Month;
                    inputDoc.Year = DateTime.Now.Year;
                    inputDoc.DocType = (int)Enums.DocumentType.CSF;

                    item.DOCUMENT_NUMBER = _docNumberService.GenerateNumber(inputDoc);
                    item.IS_ACTIVE = true;
                    if (string.IsNullOrEmpty(item.EMPLOYEE_ID_CREATOR))
                    {
                        item.EMPLOYEE_ID_CREATOR = userLogin.EMPLOYEE_ID;
                    }

                    var locationByUser = _employeeService.GetEmployeeById(item.EMPLOYEE_ID);
                    item.LOCATION_CITY = locationByUser.CITY;
                    item.LOCATION_ADDRESS = locationByUser.ADDRESS;

                    model = Mapper.Map<TRA_CSF>(item);
                }

                _CsfService.saveCsf(model, userLogin);
                _uow.SaveChanges();
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return Mapper.Map<TraCsfDto>(model);
        }

        public TemporaryDto SaveTemp(TemporaryDto item, Login userLogin)
        {
            TRA_TEMPORARY model;
            if (item == null)
            {
                throw new Exception("Invalid Data Entry");
            }

            try
            {
                if (item.TRA_TEMPORARY_ID > 0)
                {
                    //update
                    model = _temporaryService.GetTemporaryById(item.TRA_TEMPORARY_ID);

                    if (model == null)
                        throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                    Mapper.Map<TemporaryDto, TRA_TEMPORARY>(item, model);
                }
                else
                {
                    var inputDoc = new GenerateDocNumberInput();
                    inputDoc.Month = DateTime.Now.Month;
                    inputDoc.Year = DateTime.Now.Year;
                    inputDoc.DocType = (int)Enums.DocumentType.TMP;

                    item.DOCUMENT_NUMBER_TEMP = _docNumberService.GenerateNumber(inputDoc);
                    item.IS_ACTIVE = true;

                    model = Mapper.Map<TRA_TEMPORARY>(item);
                }

                _temporaryService.saveTemporary(model, userLogin);
                _uow.SaveChanges();
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return Mapper.Map<TemporaryDto>(model);
        }

        public void CsfWorkflow(CsfWorkflowDocumentInput input)
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
                    var _checkDocDraft = _workflowService.GetWorkflowHistoryByUser((int)Enums.MenuList.TraCsf, input.UserId)
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

        private void SendEmailWorkflow(CsfWorkflowDocumentInput input)
        {
            var csfData = Mapper.Map<TraCsfDto>(_CsfService.GetCsfById(input.DocumentId));

            var mailProcess = ProsesMailNotificationBody(csfData, input);

            //distinct double To email
            List<string> ListTo = mailProcess.To.Distinct().ToList();

            if (mailProcess.IsCCExist)
                //Send email with CC
                _messageService.SendEmailToListWithCC(ListTo, mailProcess.CC, mailProcess.Subject, mailProcess.Body, true, mailProcess.Attachments);
            else
                _messageService.SendEmailToList(ListTo, mailProcess.Subject, mailProcess.Body, true);

        }

        private class CsfMailNotification
        {
            public CsfMailNotification()
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

        private CsfMailNotification ProsesMailNotificationBody(TraCsfDto csfData, CsfWorkflowDocumentInput input)
        {
            var bodyMail = new StringBuilder();
            var rc = new CsfMailNotification();
            var settingData = _settingService.GetSetting().ToList();

            var vehTypeBenefit = settingData.Where(x => x.SETTING_GROUP == "VEHICLE_TYPE" && x.SETTING_NAME == "BENEFIT").FirstOrDefault().MST_SETTING_ID;
            var vehCatNoCar = settingData.Where(x => x.SETTING_GROUP == "VEHICLE_CATEGORY" && x.SETTING_NAME == "NO_CAR").FirstOrDefault().MST_SETTING_ID;
            var vendorData = _vendorService.GetByShortName(csfData.VENDOR_NAME);
            var vendorEmail = vendorData == null ? string.Empty : vendorData.EMAIL_ADDRESS;

            var isBenefit = csfData.VEHICLE_TYPE == vehTypeBenefit.ToString() ? true : false;
            var isNoCar = csfData.VEHICLE_CATEGORY == vehCatNoCar.ToString() ? true : false;

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
            var typeEnv = ConfigurationManager.AppSettings["Environment"];
            var serverIntranet = ConfigurationManager.AppSettings["ServerIntranet"];
            var employeeData = _employeeService.GetEmployeeById(csfData.EMPLOYEE_ID);
            var creatorData = _employeeService.GetEmployeeById(csfData.EMPLOYEE_ID_CREATOR);
            var fleetApprovalData = _employeeService.GetEmployeeById(csfData.EMPLOYEE_ID_FLEET_APPROVAL);

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
                    //if submit from HR to EMPLOYEE
                    if (csfData.CREATED_BY == input.UserId && isBenefit) {
                        var bodyMailCsf = settingData.Where(x => x.IS_ACTIVE && x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.BodyMailCsf)).ToList();
                        
                        var cfmUrlData = bodyMailCsf.Where(x => x.SETTING_NAME == "CAR_FOR_MANAGER_URL").FirstOrDefault();
                        var ctmUrlData = bodyMailCsf.Where(x => x.SETTING_NAME == "CAR_TYPE_MODEL_URL").FirstOrDefault();
                        var fbpUrlData = bodyMailCsf.Where(x => x.SETTING_NAME == "FLEXIBLE_BENEFIT_PRACTICE_URL").FirstOrDefault();
                        var fbdUrlData = bodyMailCsf.Where(x => x.SETTING_NAME == "FLEXIBLE_BENEFIT_DESIGN_URL").FirstOrDefault();
                        var cbUrlData = bodyMailCsf.Where(x => x.SETTING_NAME == "CORE_BENEFIT_URL").FirstOrDefault();
                        var csUrlData = bodyMailCsf.Where(x => x.SETTING_NAME == "COVERAGE_SELECTION_URL").FirstOrDefault();
                        var cfmTextData = bodyMailCsf.Where(x => x.SETTING_NAME == "CAR_FOR_MANAGER_TEXT").FirstOrDefault();
                        var fbTextData = bodyMailCsf.Where(x => x.SETTING_NAME == "FLEXIBLE_BENEFIT_TEXT").FirstOrDefault();

                        var cfmUrl = cfmUrlData == null ? string.Empty : cfmUrlData.SETTING_VALUE;
                        var ctmUrl = ctmUrlData == null ? string.Empty : ctmUrlData.SETTING_VALUE;
                        var fbpUrl = fbpUrlData == null ? string.Empty : fbpUrlData.SETTING_VALUE;
                        var fbdUrl = fbdUrlData == null ? string.Empty : fbdUrlData.SETTING_VALUE;
                        var cbUrl = cbUrlData == null ? string.Empty : cbUrlData.SETTING_VALUE;
                        var csUrl = csUrlData == null ? string.Empty : csUrlData.SETTING_VALUE;
                        var cfmText = cfmTextData == null ? string.Empty : cfmTextData.SETTING_VALUE;
                        var fbText = fbTextData == null ? string.Empty : fbTextData.SETTING_VALUE;

                        rc.Subject = csfData.DOCUMENT_NUMBER + " - Benefit Car Request";

                        bodyMail.Append("Dear " + csfData.EMPLOYEE_NAME + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Please be advised that due to your Benefit Car entitlement and refering to “HMS 351 - Car For Manager” Principle & Practices, please select Car Model and Types by click in <a href='" + webRootUrl + "/TraCsf/EditForEmployee/" + csfData.TRA_CSF_ID + "?isPersonalDashboard=True" + "'>HERE</a><br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("As per your entitlement, we kindly ask you to complete the form within 14 calendar days to ensure your car will be ready on time and to avoid the consequence as stated in the P&P Car For Manager.<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Important Information:<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("To support you in understanding benefit car (COP/CFM) scheme, the circumstances, and other the terms and conditions, we advise you to read following HR Documents before selecting car scheme and type.<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("- P&P Car For Manager along with the attachments >> click Car for Manager, Affiliate Practices (<a href='" + cfmUrl + "'>link</a>)<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("- Car types, models, contribution and early termination terms and conditions >> click Car Types and Models, Communication (<a href='" + ctmUrl + "'>link</a>)<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("The procurement process will start after receiving the signed forms with approximately 2-3 months lead time, and may be longer depending on the car availability in vendor. Thus, during lead time of procurement, you will be using temporary car.<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("If you are interested to modify your CAR current entitlement, we encourage you to read following HR Documents regarding flexible benefits.<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("- P&P Flexible Benefit >> click Flexible Benefits Practices (<a href='" + fbpUrl + "'>link</a>)<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("- Flexible Benefit Design >> click Flexible Benefit Design (<a href='" + fbdUrl + "'>link</a>)<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("- Core Benefits & Allocated Flex Points Communication >> click Core Benefits & Allocated Flex Points Communication (<a href='" + cbUrl + "'>link</a>)<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("- Coverage Selection Communication >> click Coverage Selection Communication (<a href='" + csUrl + "'>link</a>)<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Should you need any help or have any questions, please do not hesitate to contact the HR Services team:<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("- Car for Manager : " + cfmText + "<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("- Flexible Benefits : " + fbText + "<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("- Thank you for your kind attention and cooperation.<br /><br />");
                        bodyMail.AppendLine();

                        rc.To.Add(employeeDataEmail);

                        foreach (var item in hrEmailList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    //if submit from FLEET to EMPLOYEE
                    else if (csfData.CREATED_BY == input.UserId && !isBenefit) {
                        rc.Subject = csfData.DOCUMENT_NUMBER + " - Operational Car Request";

                        bodyMail.Append("Dear " + csfData.EMPLOYEE_NAME + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("new operational car has been recorded as " + csfData.DOCUMENT_NUMBER + "<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Please submit detail vehicle information <a href='" + webRootUrl + "/TraCsf/EditForEmployee/" + csfData.TRA_CSF_ID + "?isPersonalDashboard=True" + "'>HERE</a><br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("We kindly ask you to complete the form back to within 7 calendar days<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("For any assistance please contact " + creatorDataName + "<br />");
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
                    //if submit from EMPLOYEE to HR
                    else if (csfData.EMPLOYEE_ID == input.EmployeeId && isBenefit) {
                        rc.Subject = "CSF - Request Confirmation";

                        bodyMail.Append("Dear " + creatorDataName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("You have received new car request<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Send confirmation by clicking below CSF number:<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("<a href='" + webRootUrl + "/TraCsf/Edit/" + csfData.TRA_CSF_ID + "?isPersonalDashboard=False" + "'>" + csfData.DOCUMENT_NUMBER + "</a> requested by "+ csfData.EMPLOYEE_NAME +"<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team");
                        bodyMail.AppendLine();

                        rc.To.Add(creatorDataEmail);

                        foreach (var item in hrEmailList)
                        {
                            rc.CC.Add(item);
                        }

                        if (isNoCar) { 
                            foreach (var item in fleetEmailList)
                            {
                                rc.CC.Add(item);
                            }
                        }
                    }
                    //if submit from EMPLOYEE to Fleet
                    else if (csfData.EMPLOYEE_ID == input.EmployeeId && !isBenefit) {
                        rc.Subject = "CSF - Request Confirmation";

                        bodyMail.Append("Dear " + creatorDataName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("You have received new car request<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Send confirmation by clicking below CSF number:<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("<a href='" + webRootUrl + "/TraCsf/Edit/" + csfData.TRA_CSF_ID + "?isPersonalDashboard=False" + "'>" + csfData.DOCUMENT_NUMBER + "</a> requested by " + csfData.EMPLOYEE_NAME + "<br /><br />");
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
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Approve:
                    //if HR Approve
                    if (input.UserRole == Enums.UserRole.HR)
                    {
                        rc.Subject = csfData.DOCUMENT_NUMBER + " - Employee Submission";

                        bodyMail.Append("Dear Fleet Team,<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("New car request " + csfData.DOCUMENT_NUMBER + " has been approved by " + creatorDataName + "<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Click <a href='" + webRootUrl + "/TraCsf/Edit/" + csfData.TRA_CSF_ID + "?isPersonalDashboard=False" + "'>HERE</a> to monitor your request<br />");
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

                        rc.CC.Add(employeeDataEmail);
                        rc.CC.Add(creatorDataEmail);
                    }
                    //if Fleet Approve for benefit
                    else if (input.UserRole == Enums.UserRole.Fleet && isBenefit)
                    {
                        rc.Subject = csfData.DOCUMENT_NUMBER + " - Employee Submission";

                        bodyMail.Append("Dear " + creatorDataName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Your new car request " + csfData.DOCUMENT_NUMBER + " has been approved by " + fleetApprovalDataName + "<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Click <a href='" + webRootUrl + "/TraCsf/Detail/" + csfData.TRA_CSF_ID + "?isPersonalDashboard=False" + "'>HERE</a> to monitor your request<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team");
                        bodyMail.AppendLine();

                        rc.To.Add(creatorDataEmail);

                        rc.CC.Add(employeeDataEmail);

                        foreach (var item in fleetEmailList)
                        {
                            rc.CC.Add(item);
                        }

                        //if vendor exists
                        if (!string.IsNullOrEmpty(vendorEmail))
                        {
                            var attDoc = CreateDocAttachmentForVendor(csfData.TRA_CSF_ID);
                            rc.Attachments.Add(attDoc);

                            rc.CC.Add(vendorEmail);
                        }
                    }
                    //if Fleet Approve for wtc
                    else if (input.UserRole == Enums.UserRole.Fleet && !isBenefit)
                    {
                        rc.Subject = csfData.DOCUMENT_NUMBER + " - Employee Submission";

                        bodyMail.Append("Dear " + csfData.EMPLOYEE_NAME + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Your new car request " + csfData.DOCUMENT_NUMBER + " has been approved by " + fleetApprovalDataName + "<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Click <a href='" + webRootUrl + "/TraCsf/Detail/" + csfData.TRA_CSF_ID + "?isPersonalDashboard=True" + "'>HERE</a> to monitor your request<br />");
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

                        //if vendor exists
                        if (!string.IsNullOrEmpty(vendorEmail))
                        {
                            var attDoc = CreateDocAttachmentForVendor(csfData.TRA_CSF_ID);
                            rc.Attachments.Add(attDoc);

                            rc.CC.Add(vendorEmail);
                        }
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Reject:
                    //if HR Reject
                    if (input.UserRole == Enums.UserRole.HR)
                    {
                        rc.Subject = csfData.DOCUMENT_NUMBER + " - Employee Submission";

                        bodyMail.Append("Dear " + csfData.EMPLOYEE_NAME + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Your new car request " + csfData.DOCUMENT_NUMBER + " has been rejected by " + creatorDataName + " for below reason : "+ _remarkService.GetRemarkById(input.Comment.Value).REMARK +"<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Please revised and re-submit your request <a href='" + webRootUrl + "/TraCsf/EditForEmployee/" + csfData.TRA_CSF_ID + "?isPersonalDashboard=True" + "'>HERE</a><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("HR Team");
                        bodyMail.AppendLine();

                        rc.To.Add(employeeDataEmail);

                        foreach (var item in hrEmailList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    //if Fleet Reject Benefit
                    else if (input.UserRole == Enums.UserRole.Fleet && isBenefit)
                    {
                        rc.Subject = csfData.DOCUMENT_NUMBER + " - Employee Submission";

                        bodyMail.Append("Dear " + creatorDataName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Your new car request " + csfData.DOCUMENT_NUMBER + " has been rejected by " + fleetApprovalDataName + " for below reason : " + _remarkService.GetRemarkById(input.Comment.Value).REMARK + "<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Please revised and re-submit your request <a href='" + webRootUrl + "/TraCsf/Edit/" + csfData.TRA_CSF_ID + "?isPersonalDashboard=False" + "'>HERE</a><br />");
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
                    }
                    //if Fleet Reject wtc
                    else if (input.UserRole == Enums.UserRole.Fleet && !isBenefit)
                    {
                        rc.Subject = csfData.DOCUMENT_NUMBER + " - Employee Submission";

                        bodyMail.Append("Dear " + csfData.EMPLOYEE_NAME + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Your new car request " + csfData.DOCUMENT_NUMBER + " has been rejected by " + creatorDataName + " for below reason : " + _remarkService.GetRemarkById(input.Comment.Value).REMARK + "<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Please revised and re-submit your request <a href='" + webRootUrl + "/TraCsf/EditForEmployee/" + csfData.TRA_CSF_ID + "?isPersonalDashboard=True" + "'>HERE</a><br />");
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
                    rc.Subject = csfData.DOCUMENT_NUMBER + " - Completed Document";

                        bodyMail.Append("Dear " + creatorDataName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Your new car request " + csfData.DOCUMENT_NUMBER + " has been completed by system<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Click <a href='" + webRootUrl + "/TraCsf/Detail/" + csfData.TRA_CSF_ID + "?isPersonalDashboard=True" + "'>HERE</a> to monitor your request<br />");
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

                        if (isBenefit) { 
                            var attDoc = UpdateDocAttachment(csfData.TRA_CSF_ID);
                            rc.Attachments.Add(attDoc);
                        }

                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Cancel:
                    rc.Subject = csfData.DOCUMENT_NUMBER + " - Cancelled Document";

                    bodyMail.Append("Dear " + employeeDataName + ",<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Your new car request " + csfData.DOCUMENT_NUMBER + " has been cancelled by " + creatorDataName + "<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Click <a href='" + webRootUrl + "/TraCsf/Detail/" + csfData.TRA_CSF_ID + "?isPersonalDashboard=True" + "'>HERE</a> to monitor your request<br />");
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

        private string UpdateDocAttachment(long id)
        {
            var csfData = _CsfService.GetCsfById(id);

            var employeeData = _employeeService.GetEmployeeById(csfData.EMPLOYEE_ID);

            var vehUsageCfm = _settingService.GetSettingById(Convert.ToInt32(csfData.VEHICLE_USAGE)).SETTING_VALUE.ToUpper() == "CFM" ? true : false;

            var typeDoc = "CopAgreement.docx";

            if (vehUsageCfm)
            {
                typeDoc = "CfmAgreement.docx";
            }

            var attDoc = System.Web.HttpContext.Current.Server.MapPath("~/files_upload/" + typeDoc);

            byte[] byteArray = System.IO.File.ReadAllBytes(attDoc);
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(byteArray, 0, (int)byteArray.Length);
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, true))
                {
                    string documentText;

                    using (StreamReader reader = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                    {
                        documentText = reader.ReadToEnd();
                    }


                    documentText = documentText.Replace("CSFEMP1", csfData.EMPLOYEE_NAME);
                    documentText = documentText.Replace("CSFLOC2", csfData.LOCATION_ADDRESS);
                    documentText = documentText.Replace("CSFLOC3", csfData.LOCATION_CITY);
                    documentText = documentText.Replace("CSFNUM4", csfData.DOCUMENT_NUMBER);
                    documentText = documentText.Replace("CSFEMP5", csfData.EMPLOYEE_ID);
                    documentText = documentText.Replace("CSFEMP6", employeeData.POSITION_TITLE);
                    documentText = documentText.Replace("CSFEMP7", employeeData.DIVISON);
                    documentText = documentText.Replace("CSFMAN8", csfData.VENDOR_MANUFACTURER);
                    documentText = documentText.Replace("CSFVEH9", "Benefit");
                    documentText = documentText.Replace("CSFVEH10", csfData.CREATED_DATE.Year.ToString());
                    documentText = documentText.Replace("CSFVEH11", csfData.VENDOR_COLOUR);
                    documentText = documentText.Replace("CSFCHAS12", csfData.VENDOR_CHASIS_NUMBER);
                    documentText = documentText.Replace("CSFENGI13", csfData.VENDOR_ENGINE_NUMBER);
                    documentText = documentText.Replace("CSFPOLI14", csfData.VENDOR_POLICE_NUMBER);
                    documentText = documentText.Replace("CSFSTART15", csfData.VENDOR_CONTRACT_START_DATE == null ? "-" :
                                                                                    csfData.VENDOR_CONTRACT_START_DATE.Value.ToString("dd-MMM-yyyy"));
                    documentText = documentText.Replace("CSFENDCO16", csfData.VENDOR_CONTRACT_END_DATE == null ? "-" :
                                                                                    csfData.VENDOR_CONTRACT_END_DATE.Value.ToString("dd-MMM-yyyy"));
                    documentText = documentText.Replace("CSFBASE17", employeeData.BASETOWN);
                    documentText = documentText.Replace("CSFCREA18", csfData.CREATED_DATE.ToString("dd-MMM-yyyy"));

                    using (StreamWriter writer = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                    {
                        writer.Write(documentText);
                    }
                }

                attDoc = System.Web.HttpContext.Current.Server.MapPath("~/files_upload/" + csfData.EMPLOYEE_ID + DateTime.Now.ToString("_yyyyMMddHHmmss") + "_" + typeDoc);

                // Save the file with the new name
                System.IO.File.WriteAllBytes(attDoc, stream.ToArray());
            }

            return attDoc;
        }

        private string CreateDocAttachmentForVendor(long id)
        {
            var csfData = _CsfService.GetCsfById(id);

            var typeDoc = "excelVendorFor.xlsx";

            var attDoc = System.Web.HttpContext.Current.Server.MapPath("~/files_upload/" + typeDoc);

            byte[] byteArray = System.IO.File.ReadAllBytes(attDoc);
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(byteArray, 0, (int)byteArray.Length);

                //using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, true))
                //{
                //    string documentText;

                //    using (StreamReader reader = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                //    {
                //        documentText = reader.ReadToEnd();
                //    }


                //    documentText = documentText.Replace("CSFNUMB", csfData.DOCUMENT_NUMBER);
                //    documentText = documentText.Replace("EMPNAME", csfData.EMPLOYEE_NAME);
                //    documentText = documentText.Replace("VENDORNAME", csfData.VENDOR_NAME);
                //    documentText = documentText.Replace("STRDT", csfData.EFFECTIVE_DATE.ToString("dd/mm/yyyy"));
                //    documentText = documentText.Replace("ENDDT", csfData.EFFECTIVE_DATE.ToString("dd/mm/yyyy"));
                //    documentText = documentText.Replace("MAKE", csfData.MANUFACTURER);
                //    documentText = documentText.Replace("MODEL", csfData.MODEL);
                //    documentText = documentText.Replace("SERIES", csfData.SERIES);
                //    documentText = documentText.Replace("COLOR", csfData.COLOUR);
                //    documentText = documentText.Replace("BODYTYP", csfData.BODY_TYPE);
                //    documentText = documentText.Replace("YEAR", csfData.CREATED_DATE.Year.ToString());

                //    using (StreamWriter writer = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                //    {
                //        writer.Write(documentText);
                //    }
                //}

                attDoc = System.Web.HttpContext.Current.Server.MapPath("~/files_upload/" + csfData.EMPLOYEE_ID + DateTime.Now.ToString("_yyyyMMddHHmmss") + "_" + typeDoc);

                // Save the file with the new name
                System.IO.File.WriteAllBytes(attDoc, stream.ToArray());
            }

            return attDoc;
        }

        private void CreateDocument(CsfWorkflowDocumentInput input)
        {
            var dbData = _CsfService.GetCsfById(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);
        }

        private void AddWorkflowHistory(CsfWorkflowDocumentInput input)
        {
            var dbData = Mapper.Map<WorkflowHistoryDto>(input);

            dbData.ACTION_DATE = DateTime.Now;
            dbData.MODUL_ID = Enums.MenuList.TraCsf;
            dbData.REMARK_ID = input.Comment;

            _workflowService.Save(dbData);

        }

        public void CancelCsf(long id, int Remark, string user)
        {
            _CsfService.CancelCsf(id, Remark, user);
        }

        private void SubmitDocument(CsfWorkflowDocumentInput input)
        {
            var dbData = _CsfService.GetCsfById(input.DocumentId);

            dbData.MODIFIED_BY = input.UserId;
            dbData.MODIFIED_DATE = DateTime.Now;

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.Draft) { 
                dbData.DOCUMENT_STATUS = Enums.DocumentStatus.AssignedForUser;
            }
            else if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.AssignedForUser)
            {
                var vehTypeBenefit = _settingService.GetSetting().Where(x => x.SETTING_GROUP == "VEHICLE_TYPE" && x.SETTING_NAME == "BENEFIT").FirstOrDefault().MST_SETTING_ID;

                var isBenefit = dbData.VEHICLE_TYPE == vehTypeBenefit.ToString() ? true : false;

                dbData.DOCUMENT_STATUS = Enums.DocumentStatus.WaitingHRApproval;

                if (!isBenefit) {
                    dbData.DOCUMENT_STATUS = Enums.DocumentStatus.WaitingFleetApproval;
                }

                var vehUsageNoCar = _settingService.GetSetting().Where(x => x.SETTING_GROUP == "VEHICLE_CATEGORY" && x.SETTING_NAME == "NO_CAR").FirstOrDefault().MST_SETTING_ID;
                if (vehUsageNoCar.ToString() == dbData.VEHICLE_CATEGORY)
                {
                    dbData.DOCUMENT_STATUS = Enums.DocumentStatus.Completed;
                }
            }

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }

        private void CancelDocument(CsfWorkflowDocumentInput input)
        {
            var dbData = _CsfService.GetCsfById(input.DocumentId);

            dbData.MODIFIED_BY = input.UserId;
            dbData.MODIFIED_DATE = DateTime.Now;

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }

        private void CompleteDocument(CsfWorkflowDocumentInput input)
        {
            var dbData = _CsfService.GetCsfById(input.DocumentId);

            dbData.MODIFIED_BY = input.UserId;
            dbData.MODIFIED_DATE = DateTime.Now;

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            dbData.DOCUMENT_STATUS = Enums.DocumentStatus.Completed;
            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }


        public TraCsfDto GetCsfById(long id)
        {
            var data = _CsfService.GetCsfById(id);
            var retData = Mapper.Map<TraCsfDto>(data);
            return retData;
        }

        private void ApproveDocument(CsfWorkflowDocumentInput input)
        {
            var dbData = _CsfService.GetCsfById(input.DocumentId);

            dbData.MODIFIED_BY = input.UserId;
            dbData.MODIFIED_DATE = DateTime.Now;

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingHRApproval) 
            { 
                dbData.DOCUMENT_STATUS = Enums.DocumentStatus.WaitingFleetApproval;
            }
            else if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingFleetApproval)
            {
                dbData.DOCUMENT_STATUS = Enums.DocumentStatus.InProgress;
                dbData.APPROVED_FLEET = input.UserId;
                dbData.APPROVED_FLEET_DATE = DateTime.Now;
                dbData.EMPLOYEE_ID_FLEET_APPROVAL = input.EmployeeId;

                _uow.SaveChanges();
            }

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }

        private void RejectDocument(CsfWorkflowDocumentInput input)
        {
            var dbData = _CsfService.GetCsfById(input.DocumentId);

            dbData.MODIFIED_BY = input.UserId;
            dbData.MODIFIED_DATE = DateTime.Now;

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingHRApproval)
            {
                dbData.DOCUMENT_STATUS = Enums.DocumentStatus.AssignedForUser;
            }
            else if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingFleetApproval)
            {
                dbData.DOCUMENT_STATUS = Enums.DocumentStatus.WaitingHRApproval;

                var vehTypeBenefit = _settingService.GetSetting().Where(x => x.SETTING_GROUP == "VEHICLE_TYPE" && x.SETTING_NAME == "BENEFIT").FirstOrDefault().MST_SETTING_ID;

                var isBenefit = dbData.VEHICLE_TYPE == vehTypeBenefit.ToString() ? true : false;

                if (!isBenefit)
                {
                    dbData.DOCUMENT_STATUS = Enums.DocumentStatus.AssignedForUser;
                }

                dbData.APPROVED_FLEET = input.UserId;
                dbData.APPROVED_FLEET_DATE = DateTime.Now;
                dbData.EMPLOYEE_ID_FLEET_APPROVAL = input.EmployeeId;

                _uow.SaveChanges();
            }

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }


        public List<EpafDto> GetCsfEpaf(bool isActive = true)
        {
            var data = _epafService.GetEpafByDocumentType(Enums.DocumentType.CSF);
            var dataEpaf = Mapper.Map<List<EpafDto>>(data);

            var dataCsf = GetList();

            var dataJoin = (from ep in dataEpaf
                            join csf in dataCsf on ep.MstEpafId equals csf.EPAF_ID
                            select new EpafDto()
                            {
                                EfectiveDate = ep.EfectiveDate,
                                LetterSend = ep.LetterSend,
                                EpafAction = ep.EpafAction,
                                CsfNumber = csf.DOCUMENT_NUMBER,
                                MstEpafId = ep.MstEpafId,
                                CsfStatus = EnumHelper.GetDescription(csf.DOCUMENT_STATUS),
                                EmployeeId = csf.EMPLOYEE_ID,
                                EmployeeName = csf.EMPLOYEE_NAME,
                                CostCenter = ep.CostCenter,
                                GroupLevel = ep.GroupLevel,
                                CsfId = csf.TRA_CSF_ID,
                                ModifiedBy = csf.MODIFIED_BY == null ? csf.CREATED_BY : csf.MODIFIED_BY,
                                ModifiedDate = csf.MODIFIED_DATE == null ? csf.CREATED_DATE : csf.MODIFIED_DATE
                            }).ToList();

            var epafCsfList = new List<EpafDto>();

            foreach (var dtEp in dataEpaf)
            {
                var dataCsfJoin = dataJoin.Where(x => x.MstEpafId == dtEp.MstEpafId).FirstOrDefault();
                if (dataCsfJoin != null)
                {
                    dtEp.CsfId = dataCsfJoin.CsfId;
                    dtEp.CsfNumber = dataCsfJoin.CsfNumber;
                    dtEp.CsfStatus = dataCsfJoin.CsfStatus;
                    dtEp.EfectiveDate = dataCsfJoin.EfectiveDate;
                    dtEp.LetterSend = dataCsfJoin.LetterSend;
                    dtEp.EpafAction = dataCsfJoin.EpafAction;
                    dtEp.MstEpafId = dataCsfJoin.MstEpafId;
                    dtEp.EmployeeId = dataCsfJoin.EmployeeId;
                    dtEp.EmployeeName = dataCsfJoin.EmployeeName;
                    dtEp.CostCenter = dataCsfJoin.CostCenter;
                    dtEp.GroupLevel = dataCsfJoin.GroupLevel;
                    dtEp.ModifiedBy = dataCsfJoin.ModifiedBy;
                    dtEp.ModifiedDate = dataCsfJoin.ModifiedDate;
                }
                else
                {
                    dtEp.ModifiedBy = dtEp.ModifiedBy == null ? dtEp.CreatedBy : dtEp.ModifiedBy;
                    dtEp.ModifiedDate = dtEp.ModifiedDate == null ? dtEp.CreatedDate : dtEp.ModifiedDate;

                    epafCsfList.Add(dtEp);
                }

            }

            return epafCsfList;
        }

        public List<TraCsfDto> GetList()
        {
            var data = _CsfService.GetAllCsf();

            return Mapper.Map<List<TraCsfDto>>(data);
        }


        public List<TemporaryDto> GetTempByCsf(string csfNumber)
        {
            var tempData = _temporaryService.GetAllTemp().Where(x => x.DOCUMENT_NUMBER_RELATED == csfNumber).ToList();

            return Mapper.Map<List<TemporaryDto>>(tempData);
        }


        public List<VehicleFromVendorUpload> ValidationUploadDocumentProcess(List<VehicleFromVendorUpload> inputs, int id)
        {
            var messageList = new List<string>();
            var messageListStopper = new List<string>();
            var outputList = new List<VehicleFromVendorUpload>();

            var dataCsf = _CsfService.GetCsfById(id);

            var policeNumberActive = _fleetService.GetFleet().Where(x => x.IS_ACTIVE && !string.IsNullOrEmpty(x.POLICE_NUMBER)
                                                                            && x.MST_FLEET_ID != dataCsf.CFM_IDLE_ID).ToList();

            foreach (var inputItem in inputs)
            {
                messageList.Clear();

                //check police number active in mst_fleet
                if (policeNumberActive.Where(x => x.POLICE_NUMBER.ToLower() == inputItem.PoliceNumber.ToLower()).FirstOrDefault() != null)
                {
                    messageList.Add("Police number already exists in master fleet");
                    messageListStopper.Add("Police number already exists in master fleet");
                }

                //check csf number
                if (dataCsf.DOCUMENT_NUMBER.ToLower() != inputItem.CsfNumber.ToLower())
                {
                    messageList.Add("CSF Number not valid");
                    messageListStopper.Add("CSF Number not valid");
                }

                //check employee name
                if (dataCsf.EMPLOYEE_NAME.ToLower() != inputItem.EmployeeName.ToLower())
                {
                    messageList.Add("Employee name not same as employee name request");
                    messageListStopper.Add("Employee name not same as employee name request");
                }

                //check manufacturer
                if (dataCsf.MANUFACTURER.ToLower() != inputItem.Manufacturer.ToLower())
                {
                    messageList.Add("Manufacturer not same as employee request");
                    messageListStopper.Add("Manufacturer not same as employee request");
                }

                //check models
                if (dataCsf.MODEL.ToLower() != inputItem.Models.ToLower())
                {
                    messageList.Add("Models not same as employee request");
                    messageListStopper.Add("Models not same as employee request");
                }

                //check series
                if (dataCsf.SERIES.ToLower() != inputItem.Series.ToLower())
                {
                    messageList.Add("Series not same as employee request");
                }

                //check body type
                if (dataCsf.BODY_TYPE.ToLower() != inputItem.BodyType.ToLower())
                {
                    messageList.Add("Body Type not same as employee request");
                }

                //check color
                if (dataCsf.COLOUR.ToLower() != inputItem.Color.ToLower())
                {
                    messageList.Add("Colour not same as employee request");
                }

                //check start contract
                if (dataCsf.EFFECTIVE_DATE > inputItem.StartPeriod)
                {
                    messageList.Add("Start contract less than effective date");
                    messageListStopper.Add("Start contract less than effective date");
                }

                //check end contract
                if (inputItem.StartPeriod > inputItem.EndPeriod)
                {
                    messageList.Add("End contract less than Start contract");
                    messageListStopper.Add("End contract less Start contract");
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

        public List<VehicleFromUserUpload> ValidationUploadVehicleProcess(List<VehicleFromUserUpload> inputs, int id)
        {
            var messageList = new List<string>();
            var outputList = new List<VehicleFromUserUpload>();

            var dataCsf = _CsfService.GetCsfById(id);

            var dataVehicle = _vehicleSpectService.GetVehicleSpect().Where(x => x.IS_ACTIVE && x.GROUP_LEVEL == 0).ToList();
            var dataAllPricelist = _priceListService.GetPriceList().Where(x => x.IS_ACTIVE).ToList();

            var zonePriceList = _locationMappingService.GetLocationMapping().Where(x => x.IS_ACTIVE && x.LOCATION == dataCsf.LOCATION_CITY)
                                                                                        .OrderByDescending(x => x.VALIDITY_FROM).FirstOrDefault();

            var zonePriceListByUserCsf = zonePriceList == null ? string.Empty : zonePriceList.ZONE_PRICE_LIST;

            var allVendor = _vendorService.GetVendor().Where(x => x.IS_ACTIVE).ToList();

            foreach (var inputItem in inputs)
            {
                messageList.Clear();

                var checkSpect = dataVehicle.Where(x => x.MANUFACTURER.ToLower() == inputItem.Manufacturer.ToLower()
                                                        && x.MODEL.ToLower() == inputItem.Models.ToLower()
                                                        && x.SERIES.ToLower() == inputItem.Series.ToLower()
                                                        && x.BODY_TYPE.ToLower() == inputItem.BodyType.ToLower()
                                                        && x.COLOUR.ToLower() == inputItem.Color.ToLower()
                                                        && x.YEAR == dataCsf.CREATED_DATE.Year).ToList();

                //check exist
                if (checkSpect.Count == 0)
                {
                    messageList.Add("Vehicle Spect not exists in master vehicle spect");
                }
                else
                {
                    dataAllPricelist = dataAllPricelist.Where(x => x.ZONE_PRICE_LIST != null).ToList();

                    //select vendor from pricelist
                    var dataVendor = dataAllPricelist.Where(x => x.MANUFACTURER.ToLower() == inputItem.Manufacturer.ToLower()
                                                            && x.MODEL.ToLower() == inputItem.Models.ToLower()
                                                            && x.SERIES.ToLower() == inputItem.Series.ToLower()
                                                            && x.YEAR == dataCsf.CREATED_DATE.Year
                                                            && x.ZONE_PRICE_LIST.ToLower() == zonePriceListByUserCsf.ToLower()).FirstOrDefault();

                    var vendorId = dataVendor == null ? 0 : dataVendor.VENDOR;

                    var dataVendorDetail = allVendor.Where(x => x.MST_VENDOR_ID == vendorId).FirstOrDefault();

                    inputItem.Vendor = dataVendor == null ? string.Empty : (dataVendorDetail == null ? string.Empty : dataVendorDetail.SHORT_NAME);
                }

                //check vendor
                if (string.IsNullOrEmpty(inputItem.Vendor))
                {
                    messageList.Add("Vendor not exists in master price list");
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

                outputList.Add(inputItem);
            }

            return outputList;
        }

        public void CheckCsfInProgress()
        {
            var dateMinus1 = DateTime.Now.AddDays(-1);

            var listCsfInProgress = _CsfService.GetAllCsf().Where(x => x.VENDOR_CONTRACT_START_DATE != null).ToList();

            listCsfInProgress = listCsfInProgress.Where(x => x.DOCUMENT_STATUS == Enums.DocumentStatus.InProgress
                                                                        && x.VENDOR_CONTRACT_START_DATE.Value.Day == dateMinus1.Day
                                                                        && x.VENDOR_CONTRACT_START_DATE.Value.Month == dateMinus1.Month
                                                                        && x.VENDOR_CONTRACT_START_DATE.Value.Year == dateMinus1.Year
                                                                        && !string.IsNullOrEmpty(x.VENDOR_PO_NUMBER)).ToList();

            foreach (var item in listCsfInProgress)
            {
                //change status completed
                var input = new CsfWorkflowDocumentInput();
                input.ActionType = Enums.ActionType.Completed;
                input.UserId = "SYSTEM";
                input.DocumentId = item.TRA_CSF_ID;
                input.DocumentNumber = item.DOCUMENT_NUMBER;

                CsfWorkflow(input);

                //inactive cfm idle
                var cfmidleData = _fleetService.GetFleet().Where(x => x.IS_ACTIVE && x.POLICE_NUMBER == item.VENDOR_POLICE_NUMBER
                                                                          && x.VEHICLE_USAGE == "CFM IDLE").FirstOrDefault();

                if (cfmidleData != null) {
                    var endDateCfm = item.VENDOR_CONTRACT_START_DATE.Value.AddDays(-1);

                    cfmidleData.END_DATE = endDateCfm;
                    cfmidleData.IS_ACTIVE = false;
                    _fleetService.save(cfmidleData);
                }

                
                //add new master fleet
                MST_FLEET dbFleet;

                var getZonePriceList = _locationMappingService.GetLocationMapping().Where(x => x.LOCATION == item.LOCATION_CITY
                                                                                                 && x.IS_ACTIVE).FirstOrDefault();

                var zonePrice = getZonePriceList == null ? "" : getZonePriceList.ZONE_PRICE_LIST;

                var priceList = _priceListService.GetPriceList().Where(x => x.YEAR == item.CREATED_DATE.Year
                                                                        && x.MANUFACTURER == item.VENDOR_MANUFACTURER
                                                                        && x.MODEL == item.VENDOR_MODEL
                                                                        && x.SERIES == item.VENDOR_SERIES
                                                                        && x.IS_ACTIVE
                                                                        && x.ZONE_PRICE_LIST == zonePrice).FirstOrDefault();

                var vehType = string.Empty;
                var vehUsage = string.Empty;
                var suppMethod = string.Empty;

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

                if (!string.IsNullOrEmpty(item.SUPPLY_METHOD))
                {
                    var suppMethodData = _settingService.GetSettingById(Convert.ToInt32(item.VEHICLE_USAGE));
                    if (suppMethodData != null)
                    {
                        suppMethod = suppMethodData.SETTING_VALUE.ToUpper();
                    }
                }

                dbFleet = Mapper.Map<MST_FLEET>(item);
                dbFleet.IS_ACTIVE = true;
                dbFleet.CREATED_DATE = DateTime.Now;
                dbFleet.VEHICLE_TYPE = vehType;
                dbFleet.VEHICLE_USAGE = vehUsage;
                dbFleet.SUPPLY_METHOD = suppMethod;
                dbFleet.MONTHLY_HMS_INSTALLMENT = priceList == null ? 0 : priceList.PRICE;
                dbFleet.FUEL_TYPE = string.Empty;

                _fleetService.save(dbFleet);

                _uow.SaveChanges();
            }
        }


        public bool CheckCsfExists(TraCsfDto item)
        {
            var isExist = false;

            var vehicleType = _settingService.GetSettingById(Convert.ToInt32(item.VEHICLE_TYPE)).SETTING_VALUE.ToUpper();

            var existData = _fleetService.GetFleet().Where(x => x.IS_ACTIVE && x.EMPLOYEE_ID == item.EMPLOYEE_ID
                                                                && x.VEHICLE_TYPE == vehicleType
                                                                && item.EFFECTIVE_DATE <= x.END_CONTRACT
                                                                && x.SUPPLY_METHOD != "TEMPORARY").ToList();

            if (existData.Count > 0)
            {
                isExist = true;
            }

            return isExist;
        }

        public bool CheckCsfOpenExists(TraCsfDto item)
        {
            var isExist = false;

            var benefitType = _settingService.GetSetting().Where(x => x.SETTING_NAME.ToUpper() == "BENEFIT").FirstOrDefault().MST_SETTING_ID.ToString();

            var existDataOpen = _CsfService.GetAllCsf().Where(x => x.EMPLOYEE_ID == item.EMPLOYEE_ID && x.DOCUMENT_STATUS != Enums.DocumentStatus.Cancelled
                                                                       && x.DOCUMENT_STATUS != Enums.DocumentStatus.Completed
                                                                       && x.VEHICLE_TYPE == benefitType).ToList();

            if (existDataOpen.Count > 0)
            {
                isExist = true;
            }

            return isExist;
        }
    }
}
