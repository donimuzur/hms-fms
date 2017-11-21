using AutoMapper;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.Core;
using FMS.DAL.Services;
using FMS.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FMS.BLL.Crf
{
    public class CrfBLL : ITraCrfBLL
    {
        private ICRFService _CrfService;
        private IEpafService _epafService;
        private IDocumentNumberService _docNumberService;
        private IEmployeeService _employeeService;
        private IFleetService _fleetService;
        private IVendorService _vendorService;
        private IWorkflowHistoryService _workflowService;
        private IMessageService _messageService;
        private ISettingService _settingService;

        private IUnitOfWork _uow;
        public CrfBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _CrfService = new CrfService(_uow);
            _epafService = new EpafService(_uow);
            _employeeService = new EmployeeService(_uow);
            _docNumberService = new DocumentNumberService(_uow);
            _workflowService = new WorkflowHistoryService(_uow);
            _vendorService = new VendorService(_uow);
            _fleetService = new FleetService(_uow);
            _messageService = new MessageService(_uow);
            _settingService = new SettingService(_uow);
        }


        public List<TraCrfDto> GetList(Login currentUser)
        {
            var data = _CrfService.GetList();
            List<TRA_CRF> crfList = new List<TRA_CRF>();
            if (currentUser.UserRole == Enums.UserRole.User || currentUser.UserRole == 0)
            {
                crfList.AddRange(data.Where(x => x.EMPLOYEE_ID == currentUser.EMPLOYEE_ID).ToList());
                
            }

            if (currentUser.UserRole == Enums.UserRole.Fleet || currentUser.UserRole == Enums.UserRole.HR)
            {
                data = data.Where(x => x.EMPLOYEE_ID != currentUser.EMPLOYEE_ID).ToList();
                if (currentUser.UserRole == Enums.UserRole.Fleet)
                {
                    crfList.AddRange(data.Where(x => x.VEHICLE_TYPE == "WTC" 
                        || x.DOCUMENT_STATUS == (int) Enums.DocumentStatus.WaitingFleetApproval 
                        || x.DOCUMENT_STATUS == (int) Enums.DocumentStatus.AssignedForFleet));
                    
                }

                if (currentUser.UserRole == Enums.UserRole.HR)
                {
                    crfList.AddRange(data.Where(x => x.VEHICLE_TYPE == "BENEFIT"));
                }
                    
                
            }
            
            return Mapper.Map<List<TraCrfDto>>(crfList);
        }

        public List<TraCrfDto> GetList()
        {
            var data = _CrfService.GetList();


            return Mapper.Map<List<TraCrfDto>>(data);
        }

        public TraCrfDto GetDataById(long id)
        {
            var data = _CrfService.GetById((int)id);

            return Mapper.Map<TraCrfDto>(data);
        }

        

        

        

        public TraCrfDto AssignCrfFromEpaf(long epafId, Login CurrentUser)
        {

            var epafData = _epafService.GetEpafById(epafId);
            
            if (epafData != null)
            {
                var existingData = _CrfService.GetByEpafId(epafId);
                if (existingData != null)
                {
                    throw new Exception("Epaf Already asigned.");
                }
                
                TraCrfDto item = new TraCrfDto();
                var employeeData = _employeeService.GetEmployeeById(epafData.EMPLOYEE_ID);
                var fleetData = _fleetService.GetFleetByParam(new FleetParamInput()
                {
                    EmployeeId = employeeData.EMPLOYEE_ID

                }).FirstOrDefault();
                var fleetDo = Mapper.Map<FleetDto>(fleetData);
                var employeeDto = Mapper.Map<EmployeeDto>(employeeData);
                employeeDto.EmployeeVehicle = fleetDo;
                item = Mapper.Map<TraCrfDto>(epafData);
                if (CurrentUser.UserRole == Enums.UserRole.HR)
                {
                    item.VEHICLE_TYPE = "BENEFIT";
                    item.COST_CENTER = employeeDto.COST_CENTER;
                    
                }
                else if (CurrentUser.UserRole == Enums.UserRole.Fleet)
                {
                    item.VEHICLE_TYPE = "WTC";
                }

                if (employeeData == null)
                {
                    throw new Exception(string.Format("Employee Data {0} not found.", epafData.EMPLOYEE_ID));
                }

                item.LOCATION_CITY = employeeData.CITY;
                item.LOCATION_OFFICE = employeeData.BASETOWN;

                item.CREATED_BY = CurrentUser.USER_ID;
                item.CREATED_DATE = DateTime.Now;
                item.DOCUMENT_STATUS = (int) Enums.DocumentStatus.Draft;
                item.IS_ACTIVE = true;

                var vehicleData = _fleetService.GetFleetByParam(new FleetParamInput()
                {
                    EmployeeId = epafData.EMPLOYEE_ID,
                    //VehicleType = item.VEHICLE_TYPE,

                }).FirstOrDefault();

                if (vehicleData != null)
                {
                    var vendorData =
                        _vendorService.GetVendor().FirstOrDefault(x => x.SHORT_NAME == vehicleData.VENDOR_NAME);
                    item.POLICE_NUMBER = vehicleData.POLICE_NUMBER;
                    item.MANUFACTURER = vehicleData.MANUFACTURER;
                    item.MODEL = vehicleData.MODEL;
                    item.SERIES = vehicleData.SERIES;
                    item.BodyType = vehicleData.BODY_TYPE;
                    item.VENDOR_NAME = vendorData != null ? vendorData.SHORT_NAME : null;
                    item.VENDOR_ID = vendorData != null ? vendorData.MST_VENDOR_ID : (int?) null;
                    item.START_PERIOD = vehicleData.START_DATE;
                    item.END_PERIOD = vehicleData.END_DATE;
                    item.VEHICLE_TYPE = vehicleData.VEHICLE_TYPE;
                    item.VEHICLE_USAGE = vehicleData.VEHICLE_USAGE;
                    
                }
                else
                {
                    throw new Exception("Vehicle for this employee not found on FMS.");
                }


                var returnData = this.SaveCrf(item, CurrentUser);

                //AddWorkflowHistory(returnData,CurrentUser,Enums.ActionType.Created, null);

                return returnData;
            }
            else
            {
                throw new Exception("Please select Epaf document first.");
            }
        }

        public TraCrfDto SaveCrf(TraCrfDto data,Login userLogin)
        {
            
            var datatosave = Mapper.Map<TRA_CRF>(data);
            datatosave.BODY_TYPE = data.BodyType;
            if (datatosave.TRA_CRF_ID > 0)
            {

            }
            else
            {
                //datatosave.role_type
                    

                datatosave.DOCUMENT_NUMBER = _docNumberService.GenerateNumber(new GenerateDocNumberInput() { 
                    Month = DateTime.Now.Month,
                    Year = DateTime.Now.Year,
                    DocType = (int) Enums.DocumentType.CRF
                });
                    
               
            }
                
                
            datatosave.MST_REMARK = null;
            datatosave.REMARK = null;
            data.TRA_CRF_ID = _CrfService.SaveCrf(datatosave, userLogin);
                
            AddWorkflowHistory(data,userLogin,Enums.ActionType.Created, null);
            _uow.SaveChanges();
            return data;
            
            
            
        }

        public bool IsAllowedEdit(Login currentUser, TraCrfDto data)
        {
            
            if (currentUser.EMPLOYEE_ID == data.EMPLOYEE_ID 
                && data.DOCUMENT_STATUS == (int) Enums.DocumentStatus.AssignedForUser) 
                return true;

            if (currentUser.UserRole == Enums.UserRole.HR 
                && data.DOCUMENT_STATUS == (int) Enums.DocumentStatus.Draft)
                return true;
            if (currentUser.UserRole == Enums.UserRole.Fleet 
                && data.DOCUMENT_STATUS == (int)Enums.DocumentStatus.AssignedForFleet)
                return true;
            if (currentUser.UserRole == Enums.UserRole.Fleet
                && data.DOCUMENT_STATUS == (int) Enums.DocumentStatus.Draft
                && data.VEHICLE_TYPE == "WTC")
                return true;
            return false;
        }

        public bool IsAllowedApprove(Login currentUser, TraCrfDto data)
        {
            bool isAllowed = false;

            if (currentUser.UserRole == Enums.UserRole.HR
                && data.DOCUMENT_STATUS == (int)Enums.DocumentStatus.WaitingHRApproval)
                return true;
            if (currentUser.UserRole == Enums.UserRole.Fleet
                && data.DOCUMENT_STATUS == (int)Enums.DocumentStatus.WaitingFleetApproval)
                return true;

            return isAllowed;
        }

        public void Approve(long TraCrfId,Login currentUser)
        {
            var data = _CrfService.GetById((int) TraCrfId);
            var dataToSave = Mapper.Map<TraCrfDto>(data);
            if (data.VEHICLE_TYPE == "BENEFIT")
            {
                if (currentUser.UserRole == Enums.UserRole.HR)
                {
                    data.DOCUMENT_STATUS = (int) Enums.DocumentStatus.WaitingFleetApproval;
                    AddWorkflowHistory(dataToSave, currentUser, Enums.ActionType.Approve, null);

                }

                if (currentUser.UserRole == Enums.UserRole.Fleet)
                {
                    data.DOCUMENT_STATUS = (int)Enums.DocumentStatus.AssignedForFleet;
                    AddWorkflowHistory(dataToSave, currentUser, Enums.ActionType.Approve, null);

                }
            }
            else
            {
                if (currentUser.UserRole == Enums.UserRole.Fleet)
                {
                    data.DOCUMENT_STATUS = (int)Enums.DocumentStatus.AssignedForFleet;
                    AddWorkflowHistory(dataToSave, currentUser, Enums.ActionType.Approve, null);

                }
            }
            

            

            _uow.SaveChanges();
        }

        public void Reject(long TraCrfId, int? remark, Login currentUser)
        {
            var data = _CrfService.GetById((int)TraCrfId);
            var dataToSave = Mapper.Map<TraCrfDto>(data);
            if (data.VEHICLE_TYPE == "BENEFIT")
            {
                if (currentUser.UserRole == Enums.UserRole.HR)
                {
                    data.DOCUMENT_STATUS = (int) Enums.DocumentStatus.AssignedForUser;
                    AddWorkflowHistory(dataToSave, currentUser, Enums.ActionType.Reject, remark);

                }

                if (currentUser.UserRole == Enums.UserRole.Fleet)
                {
                    data.DOCUMENT_STATUS = (int) Enums.DocumentStatus.WaitingHRApproval;
                    AddWorkflowHistory(dataToSave, currentUser, Enums.ActionType.Reject, remark);

                }
            }
            else
            {
                if (currentUser.UserRole == Enums.UserRole.Fleet)
                {
                    data.DOCUMENT_STATUS = (int)Enums.DocumentStatus.AssignedForUser;
                    AddWorkflowHistory(dataToSave, currentUser, Enums.ActionType.Reject, remark);

                }    
            }
            

            

            _uow.SaveChanges();
        }

        public void SubmitCrf(long crfId,Login currentUser)
        {
            var data = _CrfService.GetById((int)crfId);
            var currentDocStatus = data.DOCUMENT_STATUS;
            if (currentUser.UserRole == Enums.UserRole.HR && data.VEHICLE_TYPE.ToUpper() == "BENEFIT")
            {
                data.DOCUMENT_STATUS = (int) Enums.DocumentStatus.AssignedForUser;

            }
            
            if (currentUser.UserRole == Enums.UserRole.Fleet 
                && data.VEHICLE_TYPE.ToUpper() == "WTC" 
                && data.DOCUMENT_STATUS == (int)Enums.DocumentStatus.Draft)
            {
                data.DOCUMENT_STATUS = (int) Enums.DocumentStatus.AssignedForUser;
            }

            

            if (currentUser.EMPLOYEE_ID == data.EMPLOYEE_ID
                && data.DOCUMENT_STATUS == (int)Enums.DocumentStatus.AssignedForUser)
            {
                data.DOCUMENT_STATUS = (int) (data.VEHICLE_TYPE.ToUpper() == "WTC"
                    ? Enums.DocumentStatus.WaitingFleetApproval : Enums.DocumentStatus.WaitingHRApproval);
            }

            
            data.IS_ACTIVE = true;
            _CrfService.SaveCrf(data, currentUser);
            var crfDto = Mapper.Map<TraCrfDto>(data);

            if (currentDocStatus != data.DOCUMENT_STATUS)
            {
                AddWorkflowHistory(crfDto, currentUser, Enums.ActionType.Submit, null);
                _uow.SaveChanges();
            }
            
            
        }

        public List<TraCrfDto> GetCrfByParam(TraCrfEpafParamInput input)
        {
            var data = _CrfService.GetList(input);

            return Mapper.Map<List<TraCrfDto>>(data);
        }

        

        private void AddWorkflowHistory(TraCrfDto input,Login currentUserLogin,Enums.ActionType action, int? RemarkId)
        {
            var dbData = new WorkflowHistoryDto();
            dbData.ACTION_BY = currentUserLogin.USER_ID;
            dbData.FORM_ID = input.TRA_CRF_ID;
            dbData.MODUL_ID = Enums.MenuList.TraCrf;
            dbData.REMARK_ID = RemarkId;
            dbData.ACTION_DATE = DateTime.Now;
            dbData.ACTION = action;
            dbData.REMARK_ID = RemarkId;

            switch (input.DOCUMENT_STATUS)
            {
                case (int)Enums.DocumentStatus.AssignedForUser:
                    SendEmailWorkflow(input, Enums.ActionType.Submit);
                    break;
                case (int)Enums.DocumentStatus.WaitingHRApproval:
                    SendEmailWorkflow(input, Enums.ActionType.Approve);
                    break;
                case (int)Enums.DocumentStatus.WaitingFleetApproval:
                    SendEmailWorkflow(input, Enums.ActionType.Approve);
                    break;
                case (int)Enums.DocumentStatus.AssignedForFleet:
                    break;
            }

            _workflowService.Save(dbData);

        }

        private void SendEmailWorkflow(TraCrfDto crfData,Enums.ActionType action)
        {
            //var csfData = Mapper.Map<TraCsfDto>(_CsfService.GetCsfById(input.DocumentId));

            var mailProcess = ProsesMailNotificationBody(crfData, action);

            //distinct double To email
            List<string> ListTo = mailProcess.To.Distinct().ToList();

            if (mailProcess.IsCCExist)
                //Send email with CC
                _messageService.SendEmailToListWithCC(ListTo, mailProcess.CC, mailProcess.Subject, mailProcess.Body, true);
            else
                _messageService.SendEmailToList(ListTo, mailProcess.Subject, mailProcess.Body, true);

        }

        private FMSMailNotification ProsesMailNotificationBody(TraCrfDto crfData, Enums.ActionType action)
        {
            var bodyMail = new StringBuilder();
            var rc = new FMSMailNotification();

            //var vehTypeBenefit = _settingService.GetSetting().Where(x => x.SETTING_GROUP == "VEHICLE_TYPE" && x.SETTING_NAME == "BENEFIT").FirstOrDefault().MST_SETTING_ID;

            var isBenefit = crfData.VEHICLE_TYPE.ToUpper().Contains("BENEFIT");

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
            var typeEnv = ConfigurationManager.AppSettings["Environment"];
            var employeeData = _employeeService.GetEmployeeById(crfData.EMPLOYEE_ID);

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
                var hrEmail = _employeeService.GetEmployeeById(crfData.EMPLOYEE_ID).EMAIL_ADDRESS;
                hrList.Add(hrEmail);
            }

            query = new SqlCommand(fleetQuery, con);
            reader = query.ExecuteReader();
            while (reader.Read())
            {
                var fleetEmail = _employeeService.GetEmployeeById(crfData.EMPLOYEE_ID).EMAIL_ADDRESS;
                fleetList.Add(fleetEmail);
            }

            reader.Close();
            con.Close();

            switch (action)
            {
                case Enums.ActionType.Submit:
                    //if submit from HR to EMPLOYEE
                    if (isBenefit)
                    {
                        rc.Subject = crfData.DOCUMENT_NUMBER + " - Benefit Car Relocation";

                        bodyMail.Append("Dear " + crfData.EMPLOYEE_NAME + ",<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Please be advised that due to your Benefit Car entitlement and refering to “HMS 351 - Car For Manager” Principle & Practices, please select Car Model and Types by click in HERE<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("As per your entitlement, we kindly ask you to complete the form within 14 calendar days to ensure your car will be ready on time and to avoid the consequence as stated in the P&P Car For Manager.<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Important Information:");
                        bodyMail.AppendLine();
                        bodyMail.Append("To support you in understanding benefit car (COP/CFM) scheme, the circumstances, and other the terms and conditions, we advise you to read following HR Documents before selecting car scheme and type.<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("- P&P Car For Manager along with the attachments >> click Car for Manager, Affiliate Practices (link)<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("- Car types, models, contribution and early termination terms and conditions >> click Car Types and Models, Communication (link)<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("- Draft of COP / CFM Agreement (attached)<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("The procurement process will start after receiving the signed forms with approximately 2-3 months lead time, and may be longer depending on the car availability in vendor. Thus, during lead time of procurement, you will be using temporary car.<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("If you are interested to modify your CAR current entitlement, we encourage you to read following HR Documents regarding flexible benefits.<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("- P&P Flexible Benefit>> click Flexible Benefits Practices (link)<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("- Flexible Benefit Design >> click Flexible Benefit Design (link)<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("- Core Benefits & Allocated Flex Points Communication >> click Core Benefits & Allocated Flex Points Communication (link)<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("- Coverage Selection Communication >> click Coverage Selection Communication (link)<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Should you need any help or have any questions, please do not hesitate to contact the HR Services team:<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("- Car for Manager : Rizal Setiansyah (ext. 21539) or Astrid Meirina (ext.67165)<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("- Flexible Benefits : HR Services at YOURHR.ASIA@PMI.COM or ext. 900<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("- Thank you for your kind attention and cooperation.<br />");
                        bodyMail.AppendLine();

                        rc.To.Add(employeeData.EMAIL_ADDRESS);

                        foreach (var item in hrList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    //if submit from FLEET to EMPLOYEE
                    else if (!isBenefit)
                    {
                        rc.Subject = crfData.DOCUMENT_NUMBER + " - Operational Car Relocation";

                        bodyMail.Append("Dear " + crfData.EMPLOYEE_NAME + ",<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("new operational car has been recorded as " + crfData.DOCUMENT_NUMBER + "<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Please submit detail vehicle information <a href='" + webRootUrl + "/TraCrf/Edit/" + crfData.TRA_CRF_ID + "?isPersonalDashboard=True" + "'>HERE</a><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("We kindly ask you to complete the form back to within 7 calendar days<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("For any assistance please contact Fleet Name<br />");
                        bodyMail.AppendLine();

                        rc.To.Add(employeeData.EMAIL_ADDRESS);

                        foreach (var item in fleetList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    break;
                case Enums.ActionType.Approve:
                    break;
                case Enums.ActionType.Reject:
                    break;
            }

            rc.Body = bodyMail.ToString();
            return rc;
        }

        public List<EpafDto> GetCrfEpaf(bool isActive = true)
        {
            var data = _epafService.GetEpafByDocumentType(Enums.DocumentType.CRF);
            var dataEpaf = Mapper.Map<List<EpafDto>>(data);

            

            var dataCrf = GetList();

            var dataJoin = (from ep in dataEpaf
                             join crf in dataCrf on ep.MstEpafId equals crf.EPAF_ID
                             select new EpafDto() {
                                 CrfNumber = crf.DOCUMENT_NUMBER,
                                MstEpafId = ep.MstEpafId,
                                 CrfStatus = Utils.EnumHelper.GetDescription((Enums.DocumentStatus)crf.DOCUMENT_STATUS),
                                EmployeeId = crf.EMPLOYEE_ID,
                                EmployeeName = crf.EMPLOYEE_NAME,
                                CityNew = ep.City,
                                BaseTownNew = ep.BaseTown,
                                BaseTown = crf.LOCATION_OFFICE,
                                City = crf.LOCATION_CITY,
                                CrfId = crf.TRA_CRF_ID
                             }).ToList();

            var dataEmployee = _employeeService.GetEmployee();
            

            foreach (var dtEp in dataEpaf)
            {
                var dataCrfJoin = dataJoin.Where(x=> x.MstEpafId == dtEp.MstEpafId).FirstOrDefault();
                if (dataCrfJoin != null)
                {
                    dtEp.CrfId = dataCrfJoin.CrfId;
                    dtEp.CrfNumber = dataCrfJoin.CrfNumber;
                    dtEp.CrfStatus = dataCrfJoin.CrfStatus;
                    dtEp.BaseTownNew = dataCrfJoin.BaseTownNew;
                    dtEp.CityNew = dataCrfJoin.CityNew;
                    dtEp.City = dataCrfJoin.City;
                    dtEp.BaseTown = dataCrfJoin.BaseTown;
                }
                else
                {
                    var employee = dataEmployee.Where(x=>x.EMPLOYEE_ID == dtEp.EmployeeId).FirstOrDefault();
                    var baseTownNew = dtEp.BaseTown;
                    var cityNew = dtEp.City;
                    dtEp.BaseTown = employee.BASETOWN;
                    dtEp.City = employee.CITY;
                    dtEp.BaseTownNew = baseTownNew;
                    dtEp.CityNew = cityNew;
                }
                
            }

            return dataEpaf;
        }
    }
}
