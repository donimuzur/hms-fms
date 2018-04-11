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
using System.Diagnostics;

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
        private IFleetChangeService _fleetChangeService;
        private IFunctionChangeService _functionChangeService;
        private ILocationChangeService _locationChangeService;
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
            _fleetChangeService = new FleetChangeService(_uow);
            _functionChangeService = new FunctionChangeService(_uow);
            _locationChangeService = new LocationChangeService(_uow);
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
            var data = _ctfService.GetCtf().Where(x => (x.EMPLOYEE_ID == userLogin.EMPLOYEE_ID || x.EMPLOYEE_ID_CREATOR == userLogin.EMPLOYEE_ID || x.EMPLOYEE_ID_FLEET_APPROVAL == userLogin.EMPLOYEE_ID || (userLogin.LoginFor == null ? false : (userLogin.LoginFor.Where( login => login.EMPLOYEE_ID == x.EMPLOYEE_ID_FLEET_APPROVAL || login.EMPLOYEE_ID == x.EMPLOYEE_ID_CREATOR ).Count() > 0 ? true : false)))).ToList();
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

                if (Dto.TraCtfId > 0)
                {
                    //update
                    var Exist = _ctfService.GetCtf().Where(c => c.TRA_CTF_ID == Dto.TraCtfId).FirstOrDefault();

                    if (Exist == null)
                        throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                    //changed = SetChangesHistory(model, item, userId);
                    dbTraCtf = Mapper.Map<TRA_CTF>(Dto);
                    _ctfService.Save(dbTraCtf, userLogin);
                }
                else
                {
                    var inputDoc = new GenerateDocNumberInput();
                    inputDoc.Month = DateTime.Now.Month;
                    inputDoc.Year = DateTime.Now.Year;
                    inputDoc.DocType = (int)Enums.DocumentType.CTF;

                    Dto.DocumentNumber = _docNumberService.GenerateNumber(inputDoc);

                    dbTraCtf = Mapper.Map<TRA_CTF>(Dto);
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
        public TraCtfDto SaveUpload(TraCtfDto Dto, Login userLogin)
        {
            TRA_CTF dbTraCtf;
            if (Dto == null)
            {
                throw new Exception("Invalid Data Entry");
            }
            try
            {
                bool changed = false;

                if (Dto.TraCtfId > 0)
                {
                    //update
                    var Exist = _ctfService.GetCtf().Where(c => c.TRA_CTF_ID == Dto.TraCtfId).FirstOrDefault();

                    if (Exist == null)
                        throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                    //changed = SetChangesHistory(model, item, userId);
                    dbTraCtf = Mapper.Map<TRA_CTF>(Dto);
                    _ctfService.SaveUpload(dbTraCtf, userLogin);
                }
                else
                {
                    var inputDoc = new GenerateDocNumberInput();
                    inputDoc.Month = DateTime.Now.Month;
                    inputDoc.Year = DateTime.Now.Year;
                    inputDoc.DocType = (int)Enums.DocumentType.CTF;

                    Dto.DocumentNumber = _docNumberService.GenerateNumber(inputDoc);

                    dbTraCtf = Mapper.Map<TRA_CTF>(Dto);
                    _ctfService.SaveUpload(dbTraCtf, userLogin);

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
            }
            catch (Exception exception)
            {
                throw exception;
            }
            var data = _ctfService.GetCtf().Where(x => x.DOCUMENT_NUMBER == Dto.DocumentNumber).FirstOrDefault();
            Dto = Mapper.Map<TraCtfDto>(data);
            return Dto;
        }
        private string ReplaceToValue(TraCtfDto CtfDto, MST_FLEET Fleet, string Value)
        {
            if (Value.ToUpper().Contains("MST_FLEET.END_CONTRACT"))
            {
                var EndContract = (int)(Fleet.END_CONTRACT.Value.Date - new DateTime(1900, 1, 1)).TotalDays;
                Value = Value.Replace("MST_FLEET.END_CONTRACT", EndContract.ToString());
            }
            if (Value.ToUpper().Contains("MST_FLEET.MONTHLY_HMS_INSTALLMENT"))
            {
                Value = Value.Replace("MST_FLEET.MONTHLY_HMS_INSTALLMENT", Fleet.MONTHLY_HMS_INSTALLMENT.ToString());
            }
            if (Value.ToUpper().Contains("TRA_CTF.EFFECTIVE_DATE"))
            {
                var EffectiveDate = (int)(CtfDto.EffectiveDate.Value.Date - new DateTime(1900, 1, 1)).TotalDays;
                Value = Value.Replace("TRA_CTF.EFFECTIVE_DATE", EffectiveDate.ToString());
            }
            return Value;
        }
        public decimal? PenaltyCost(TraCtfDto CtfDto)
        {
            decimal? cost = 0;
            if (CtfDto == null)
                return null;
            var fleetData = _fleetService.GetFleet().Where(x => x.POLICE_NUMBER == CtfDto.PoliceNumber && x.EMPLOYEE_ID == CtfDto.EmployeeId && x.IS_ACTIVE
                                                        && (CtfDto.VehicleType == null ? "" : CtfDto.VehicleType.ToUpper()) == (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper())).FirstOrDefault();

            var rentMonth = ((fleetData.END_CONTRACT.Value.Year - CtfDto.EffectiveDate.Value.Year) * 12) + fleetData.END_CONTRACT.Value.Month - CtfDto.EffectiveDate.Value.Month;

            var Vendor = _vendorService.GetVendor().Where(x => (x.VENDOR_NAME == null ? "" : x.VENDOR_NAME.ToUpper()) == (fleetData.VENDOR_NAME == null ? "" : fleetData.VENDOR_NAME.ToUpper()) && x.IS_ACTIVE).FirstOrDefault();

            var penalty = _penaltyService.GetPenalty().Where(x => (x.MANUFACTURER == null ? "" : x.MANUFACTURER.ToUpper()) == (fleetData.MANUFACTURER == null ? "" : fleetData.MANUFACTURER.ToUpper())
                                                                   && (x.MODEL == null ? "" : x.MODEL.ToUpper()) == (fleetData.MODEL == null ? "" : fleetData.MODEL.ToUpper())
                                                                   && (x.SERIES == null ? "" : x.SERIES.ToUpper()) == (fleetData.SERIES == null ? "" : fleetData.SERIES.ToUpper())
                                                                   && (x.BODY_TYPE == null ? "" : x.BODY_TYPE.ToUpper()) == (fleetData.BODY_TYPE == null ? "" : fleetData.BODY_TYPE.ToUpper())
                                                                   && x.YEAR == fleetData.VEHICLE_YEAR
                                                                   && x.VENDOR == Vendor.MST_VENDOR_ID
                                                                   && (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == (fleetData.VEHICLE_TYPE == null ? "" : fleetData.VEHICLE_TYPE.ToUpper())
                                                                   && x.MONTH_START <= rentMonth && x.MONTH_END >= rentMonth && x.IS_ACTIVE).FirstOrDefault();

            if (penalty == null)
            {
                penalty = _penaltyService.GetPenalty().Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == (fleetData.VEHICLE_TYPE == null ? "" : fleetData.VEHICLE_TYPE.ToUpper())
                                                             && x.YEAR == fleetData.VEHICLE_YEAR
                                                             && (x.MANUFACTURER == null ? "" : x.MANUFACTURER.ToUpper()) == (fleetData.MANUFACTURER == null ? "" : fleetData.MANUFACTURER.ToUpper())
                                                             && (x.MODEL == null ? "" : x.MODEL.ToUpper()) == (fleetData.MODEL == null ? "" : fleetData.MODEL.ToUpper())
                                                             && (x.BODY_TYPE == null ? "" : x.BODY_TYPE.ToUpper()) == (fleetData.BODY_TYPE == null ? "" : fleetData.BODY_TYPE.ToUpper())
                                                             && (x.SERIES == null || x.SERIES == "")
                                                             && x.VENDOR == Vendor.MST_VENDOR_ID && x.MONTH_START <= rentMonth && x.MONTH_END >= rentMonth && x.IS_ACTIVE).FirstOrDefault();
            }

            if (penalty == null)
            {
                penalty = _penaltyService.GetPenalty().Where(x => (x.BODY_TYPE == null ? "" : x.BODY_TYPE.ToUpper()) == (fleetData.BODY_TYPE == null ? "" : fleetData.BODY_TYPE.ToUpper())
                                                                    && (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == (fleetData.VEHICLE_TYPE == null ? "" : fleetData.VEHICLE_TYPE.ToUpper())
                                                                    && (x.MANUFACTURER == null || x.MANUFACTURER == "")
                                                                    && (x.MODEL == null || x.MODEL == "")
                                                                    && (x.SERIES == null || x.SERIES == "")
                                                                    && x.YEAR == fleetData.VEHICLE_YEAR
                                                                    && x.VENDOR == Vendor.MST_VENDOR_ID && x.MONTH_START <= rentMonth && x.MONTH_END >= rentMonth && x.IS_ACTIVE).FirstOrDefault();
            }

            if (penalty == null)
            {
                penalty = _penaltyService.GetPenalty().Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == (fleetData.VEHICLE_TYPE == null ? "" : fleetData.VEHICLE_TYPE.ToUpper())
                                                             && x.YEAR == fleetData.VEHICLE_YEAR
                                                             && (x.MANUFACTURER == null || x.MANUFACTURER == "")
                                                             && (x.MODEL == null || x.MODEL == "")
                                                             && (x.SERIES == null || x.SERIES == "")
                                                             && x.VENDOR == Vendor.MST_VENDOR_ID && x.MONTH_START <= rentMonth && x.MONTH_END >= rentMonth && x.IS_ACTIVE).FirstOrDefault();
            }

            if (penalty == null)
            {
                return 0;
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
            var fleet = _fleetService.GetFleet().Where(x => x.EMPLOYEE_ID == CtfDto.EmployeeId && x.POLICE_NUMBER == CtfDto.PoliceNumber && x.IS_ACTIVE
                                                        && (CtfDto.VehicleType == null ? "" : CtfDto.VehicleType.ToUpper()) == (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper())).FirstOrDefault();
            var Vendor = _vendorService.GetVendor().Where(x => (x.VENDOR_NAME == null ? "" : x.VENDOR_NAME.ToUpper()) == (fleet.VENDOR_NAME == null ? "" : fleet.VENDOR_NAME.ToUpper())
                                                                && x.IS_ACTIVE).FirstOrDefault();
            var ReasonData = _reasonService.GetReasonById(CtfDto.Reason.Value);
            var reasonStr = ReasonData.REASON;
            var IsPenalty = ReasonData.IS_PENALTY;
            var PenaltyForFleet = ReasonData.PENALTY_FOR_FLEET;
            var PenaltyForEmployee = ReasonData.PENALTY_FOR_EMPLOYEE;

            if (fleet == null) return 0;

            var installmentEmp = _pricelistService.GetPriceList().Where(x => (x.MANUFACTURER == null ? "" : x.MANUFACTURER.ToUpper()) == (fleet.MANUFACTURER == null ? "" : fleet.MANUFACTURER.ToUpper())
                                                                        && (x.MODEL == null ? "" : x.MODEL.ToUpper()) == (fleet.MODEL == null ? "" : fleet.MODEL.ToUpper())
                                                                        && (x.SERIES == null ? "" : x.SERIES.ToUpper()) == (fleet.SERIES == null ? "" : fleet.SERIES.ToUpper())
                                                                        && (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == (fleet.VEHICLE_TYPE == null ? "" : fleet.VEHICLE_TYPE.ToUpper())
                                                                        && (x.VEHICLE_USAGE == null ? "" : x.VEHICLE_USAGE.ToUpper()) == (fleet.VEHICLE_USAGE == null ? "" : fleet.VEHICLE_USAGE.ToUpper())
                                                                        && x.YEAR == fleet.VEHICLE_YEAR && x.VENDOR == Vendor.MST_VENDOR_ID && x.IS_ACTIVE == true).FirstOrDefault();

            if (installmentEmp == null) return 0;

            var rentMonth = ((CtfDto.EffectiveDate.Value.Year - fleet.START_CONTRACT.Value.Year) * 12) + CtfDto.EffectiveDate.Value.Month - fleet.START_CONTRACT.Value.Month;

            if (PenaltyForEmployee.Value == true)
            {
                cost = ((rentMonth * installmentEmp.INSTALLMEN_EMP) / (decimal)1.1) - CtfDto.Penalty.Value;
            }
            else
            {
                cost = (rentMonth * installmentEmp.INSTALLMEN_EMP) / (decimal)1.1;
            }
            return cost;

        }
        public decimal? EmployeeContribution(TraCtfDto CtfDto)
        {
            decimal? cost = 0;
            var fleet = _fleetService.GetFleet().Where(x => x.EMPLOYEE_ID == CtfDto.EmployeeId && x.POLICE_NUMBER == CtfDto.PoliceNumber && x.IS_ACTIVE
                                                        && (CtfDto.VehicleType == null ? "" : CtfDto.VehicleType.ToUpper()) == (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper())).FirstOrDefault();
            var Vendor = _vendorService.GetVendor().Where(x => (x.VENDOR_NAME == null ? "" : x.VENDOR_NAME.ToUpper()) == (fleet.VENDOR_NAME == null ? "" : fleet.VENDOR_NAME.ToUpper())
                                                          && x.IS_ACTIVE).FirstOrDefault();

            if (fleet == null) return 0;

            var Price = _pricelistService.GetPriceList().Where(x => (x.MANUFACTURER == null ? "" : x.MANUFACTURER.ToUpper()) == (fleet.MANUFACTURER == null ? "" : fleet.MANUFACTURER.ToUpper())
                                                                          && (x.MODEL == null ? "" : x.MODEL.ToUpper()) == (fleet.MODEL == null ? "" : fleet.MODEL.ToUpper())
                                                                          && (x.SERIES == null ? "" : x.SERIES.ToUpper()) == (fleet.SERIES == null ? "" : fleet.SERIES.ToUpper())
                                                                          && (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == (fleet.VEHICLE_TYPE == null ? "" : fleet.VEHICLE_TYPE.ToUpper())
                                                                          && (x.VEHICLE_USAGE == null ? "" : x.VEHICLE_USAGE.ToUpper()) == (fleet.VEHICLE_USAGE == null ? "" : fleet.VEHICLE_USAGE.ToUpper())
                                                                          && x.YEAR == fleet.VEHICLE_YEAR && x.VENDOR == Vendor.MST_VENDOR_ID && x.IS_ACTIVE == true).FirstOrDefault();

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
                    var Ctf = _ctfService.GetCtfById(input.DocumentId);
                    CancelDocument(input);
                    if (Ctf.DOCUMENT_STATUS == Enums.DocumentStatus.Draft) isNeedSendNotif = false;
                    break;
                case Enums.ActionType.Extend:
                    ExtendDocument(input);
                    break;
            }
            //todo sent mail
            if (isNeedSendNotif) SendEmailWorkflow(input);

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
                Attachments = new List<string>();
                IsCCExist = false;
            }
            public string Subject { get; set; }
            public string Body { get; set; }
            public List<string> To { get; set; }
            public List<string> CC { get; set; }
            public List<string> Attachments { get; set; }
            public bool IsCCExist { get; set; }
        }
        private CtfMailNotification ProsesMailNotificationBody(TraCtfDto ctfData, CtfWorkflowDocumentInput input)
        {
            var bodyMail = new StringBuilder();
            var rc = new CtfMailNotification();

            var fleetdata = _fleetService.GetFleet().Where(x => (x.POLICE_NUMBER == null ? "" : x.POLICE_NUMBER.ToUpper())== (ctfData.PoliceNumber == null ? "" : ctfData.PoliceNumber.ToUpper()) 
                                                            && ctfData.EmployeeId== x.EMPLOYEE_ID 
                                                            && (x.SUPPLY_METHOD == null ? "" : x.SUPPLY_METHOD.ToUpper()) == (ctfData.SupplyMethod == null ? "" : ctfData.SupplyMethod.ToUpper())
                                                            && (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == (ctfData.VehicleType == null ? "" : ctfData.VehicleType.ToUpper())).FirstOrDefault();
            if(fleetdata == null)
            {
                return rc;
            }

            var vendor = _vendorService.GetVendor().Where(x => x.VENDOR_NAME == fleetdata.VENDOR_NAME && x.IS_ACTIVE).FirstOrDefault();
            if (vendor == null)
            {
                return rc;
            }

            var vehTypeBenefit = _settingService.GetSetting().Where(x => x.SETTING_GROUP == "VEHICLE_TYPE" && x.SETTING_NAME == "BENEFIT").FirstOrDefault().SETTING_NAME;

            var isBenefit = ctfData.VehicleType == vehTypeBenefit.ToString() ? true : false;

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
            var typeEnv = ConfigurationManager.AppSettings["Environment"];
            var serverIntranet = ConfigurationManager.AppSettings["ServerIntranet"];

            var userData = _employeeService.GetEmployeeById(input.EmployeeId);
            var employeeData = _employeeService.GetEmployeeById(ctfData.EmployeeId);            
            var fleetApprovalData = _employeeService.GetEmployeeById(ctfData.EmployeeIdFleetApproval);

            var employeeDataEmail = employeeData == null ? string.Empty : employeeData.EMAIL_ADDRESS;
            var vendorDataEmail = vendor == null ? string.Empty : vendor.EMAIL_ADDRESS;
            var userDataEmail = userData == null ? string.Empty : userData.EMAIL_ADDRESS;

            var extend = _ctfExtendService.GetCtfExtend().Where(x => x.TRA_CTF_ID == ctfData.TraCtfId).FirstOrDefault();

            var employeeDataName = employeeData == null ? string.Empty : employeeData.FORMAL_NAME;
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

            //----------UPDATE CREATOR EMAIL FROM ADSI------------//  
            var creatorDataName = string.Empty;
            var creatorDataEmail = string.Empty;

            var CreatorQuery = "SELECT EMAIL, INTERNAL_EMAIL FROM " + serverIntranet + ".[dbo].[tbl_ADSI_User] WHERE FULL_NAME = 'PMI\\" + ctfData.CreatedBy + "'";
            if (typeEnv == "VTI")
            {
                CreatorQuery = "SELECT EMAIL, FULL_NAME FROM EMAIL_FOR_VTI WHERE FULL_NAME = 'PMI\\" + ctfData.CreatedBy + "'";
            }

            SqlCommand query = new SqlCommand(CreatorQuery, con);
            SqlDataReader reader = query.ExecuteReader();
            while (reader.Read())
            {
                creatorDataEmail =reader[0].ToString();
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

                    //if submit BENEFIT to EMPLOYEE
                    if ((ctfData.EmployeeIdCreator == input.EmployeeId || ctfData.CreatedBy == input.UserId) && isBenefit)
                    {
                        rc.Subject = ctfData.DocumentNumber + " - Benefit Car Termination";

                        bodyMail.Append("Dear " + ctfData.EmployeeName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("In light with your car termination as of "+(ctfData.EffectiveDate == null ? "" : ctfData.EffectiveDate.Value.ToString("dd-MMM-yyyy"))+ ", kindly fill in the car termination information <a href='" + webRootUrl + "/TraCtf/Edit?TraCtfId=" + ctfData.TraCtfId + "&isPersonalDashboard=True" + "'>HERE</a> in 14 days after you receive this email.<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Also, kindly follow these steps to complete the termination process:<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("<ol>");
                        bodyMail.AppendLine();
                        bodyMail.Append("<li>Fill and Complete Berita Acara Kejadian (BAK) form which vendor will send to you during the withdrawal (if needed)</li>");
                        bodyMail.AppendLine();
                        bodyMail.Append("<li>Prepare a copy of your driving license (SIM A) for the vendor</li>");
                        bodyMail.AppendLine();
                        bodyMail.Append("<li>Fill and complete SIRS via Web Link SIRS (Application Access > Environment, Health, Safety & Security > Sampoerna Incident Reporting System (SIRS))</li>");
                        bodyMail.AppendLine();
                        bodyMail.Append("<li>ill and compelte CIRF (if there is any incident)</li>");
                        bodyMail.AppendLine();
                        bodyMail.Append("</ol> ");
                        bodyMail.AppendLine();
                        bodyMail.Append("<br /><br />");
                        bodyMail.AppendLine(); 
                        bodyMail.Append("Also, kindly send these following documents to complete the termination to " + creatorDataName +" :");
                        bodyMail.AppendLine();
                        bodyMail.Append("<ul>");
                        bodyMail.AppendLine();
                        bodyMail.Append("<li>Copy of KTP</li>");
                        bodyMail.AppendLine();
                        bodyMail.Append("<li>Copy of NPWP</li>");
                        bodyMail.AppendLine();
                        bodyMail.Append("<li>Copy of Bank Account (for COP Scheme)</li>");
                        bodyMail.AppendLine();
                        bodyMail.Append("<li>Your Transfer Receipt (for COP Scheme with outstanding payment)</li>");
                        bodyMail.AppendLine(); 
                        bodyMail.Append("<li>Supporting Letter (for COP Scheme, will be send in separate email)</li>");
                        bodyMail.AppendLine();
                        bodyMail.Append("</ul>");
                        bodyMail.AppendLine();
                        bodyMail.Append("<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Should your car is on the COP Scheme, below are some important information for the termination process:");
                        bodyMail.AppendLine();
                        bodyMail.Append("<ul>");
                        bodyMail.AppendLine();
                        bodyMail.Append("<li>Legal documents such as BPKB will be handled directly by our vendor and be completed within approximately 2 months after termination effective date.</li>");
                        bodyMail.AppendLine();
                        bodyMail.Append("<li>Transfer of Motor Vehicle Title Fee (Biaya Balik Nama) is subjected to your personal account and will be handled directly by our vendor</li>");
                        bodyMail.AppendLine();
                        bodyMail.Append("<li>No insurance and service are covered by Company after the termination.</li>");
                        bodyMail.AppendLine();
                        bodyMail.Append("</ul>");
                        bodyMail.AppendLine();
                        bodyMail.Append("<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("We look forward for your response.<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("For any assistance please contact "+creatorDataName+".<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thank you<br /><br />");
                        bodyMail.AppendLine();

                        rc.To.Add(employeeDataEmail);

                        foreach (var item in hrEmailList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    //if submit WTC to EMPLOYEE
                    else if ((ctfData.EmployeeIdCreator == input.EmployeeId || ctfData.CreatedBy == input.UserId) && !isBenefit && !input.EndRent.Value)
                    {

                        rc.Subject = ctfData.DocumentNumber + " - WTC Car Termination";

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
                    else if ((ctfData.EmployeeIdCreator == input.EmployeeId || ctfData.CreatedBy == input.UserId) && !isBenefit && input.EndRent.Value)
                    {
                        rc.Subject = ctfData.DocumentNumber + " - WTC Car Termination";

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
                        rc.Subject = ctfData.DocumentNumber + " - Terminate Confirmation";

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
                        rc.Subject = ctfData.DocumentNumber + " - Employee Submission";

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
                    if (input.UserRole == Enums.UserRole.HR && isBenefit)
                    {
                        rc.Subject = ctfData.DocumentNumber + " - Employee Submission";

                        bodyMail.Append("Dear Fleet, <br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Your Car Termination Form " + ctfData.DocumentNumber + " has been approved by " + creatorDataName + "<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Click <a href='" + webRootUrl + "/TraCtf/DetailsBenefit?TraCtfId=" + ctfData.TraCtfId + "&isPersonalDashboard=False" + "'>HERE</a> to monitor your request<br />");
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
                        //rc.CC.Add(employeeDataEmail);
                    }
                    else if (input.UserRole == Enums.UserRole.Fleet && isBenefit)
                    {
                        //rc.Subject = ctfData.DocumentNumber + " - Vendor Information";

                        //bodyMail.Append("Dear " + vendorDataName + ",<br /><br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Below are the details of vehicle That will be Terminate :<br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Police Number : " + (fleetdata == null ? "" : fleetdata.POLICE_NUMBER.ToUpper()) + "<br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Manufacture : " + (fleetdata == null ? "" : fleetdata.MANUFACTURER.ToUpper()) + "<br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Model : " + (fleetdata == null ? "" : fleetdata.MODEL.ToUpper()) + "<br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Series : " + (fleetdata == null ? "" : fleetdata.SERIES.ToUpper()) + "<br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Employee Name : " + (ctfData == null ? "" : ctfData.EmployeeName) + "<br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Vehicle Usage : " + (ctfData == null ? "" : ctfData.VehicleUsage.ToUpper()) + "<br />");
                        //bodyMail.AppendLine();
                        //if ((ctfData.VehicleUsage == null ? "" : ctfData.VehicleUsage.ToUpper()) == "CFM")
                        //{
                        //    bodyMail.Append("Transfer To CFM Idle : " + (ctfData == null ? "" : ctfData.IsTransferToIdle.Value == true ? "Yes" : "No") + "<br />");
                        //    bodyMail.AppendLine();
                        //}
                        //bodyMail.Append("Contract End Date : " + fleetdata.END_CONTRACT.Value.ToString("dd-MMM-yyyy") + " <br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Termination Date : " + ctfData.EffectiveDate.Value.ToString("dd-MMM-yyyy") + " <br /><br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Withdrawal Detail : " + " <br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("PIC : " + (ctfData.WithdPic == null ? "" : ctfData.WithdPic.ToUpper()) + " <br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Date : " + (ctfData.WithdDate == null ? "" : ctfData.WithdDate.Value.ToString("dd-MMM-yyyy HH:mm")) + " <br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Phone Number : " + (ctfData.WithdPhone == null ? "" : ctfData.WithdPhone) + " <br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Address : " + (ctfData.WithdAddress == null  ? "" : ctfData.WithdAddress.ToUpper()) + " <br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("City: " + (ctfData.WithdCity == null ? "" : ctfData.WithdCity.ToUpper()) + " <br /><br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("For any assistance please contact " + fleetApprovalDataName + "<br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Thanks <br /><br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Regards,<br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Fleet Team");
                        //bodyMail.AppendLine();

                        //if (!string.IsNullOrEmpty(vendorDataEmail))
                        //{
                        //    rc.To.Add(vendorDataEmail);
                        //}
                        //foreach (var item in fleetEmailList)
                        //{
                        //    rc.CC.Add(item);
                        //}
                        //if (!string.IsNullOrEmpty(employeeDataEmail))
                        //{
                        //    //rc.CC.Add(employeeDataEmail);
                        //}
                        //foreach (var item in hrEmailList)
                        //{
                        //    rc.CC.Add(item);
                        //}
                        return rc;
                    }
                    //if Fleet Approve for wtc
                    else if (input.UserRole == Enums.UserRole.Fleet && !isBenefit)
                    {
                        //rc.Subject = ctfData.DocumentNumber + " - Vendor Information";

                        //bodyMail.Append("Dear " + vendorDataName + ",<br /><br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Below are the details of vehicle That will be Terminate :<br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Police Number : " + (fleetdata == null ? "" : fleetdata.POLICE_NUMBER.ToUpper()) + "<br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Manufacture : " + (fleetdata == null ? "" : fleetdata.MANUFACTURER.ToUpper()) + "<br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Model : " + (fleetdata == null ? "" : fleetdata.MODEL.ToUpper()) + "<br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Series : " + (fleetdata == null ? "" : fleetdata.SERIES.ToUpper()) + "<br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Employee Name : " + (ctfData == null ? "" : ctfData.EmployeeName) + "<br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Contract End Date : " + fleetdata.END_CONTRACT.Value.ToString("dd-MMM-yyyy") + " <br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Termination Date : " + ctfData.EffectiveDate.Value.ToString("dd-MMM-yyyy") + " <br /><br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Withdrawal Detail : " + " <br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("PIC : " + (ctfData.WithdPic == null ? "" : ctfData.WithdPic.ToUpper()) + " <br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Date : " + (ctfData.WithdDate == null ? "" : ctfData.WithdDate.Value.ToString("dd-MMM-yyyy HH:mm")) + " <br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Phone Number : " + (ctfData.WithdPhone == null ? "" : ctfData.WithdPhone) + " <br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Address : " + (ctfData.WithdAddress == null ? "" : ctfData.WithdAddress.ToUpper()) + " <br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("City: " + (ctfData.WithdCity == null ? "" : ctfData.WithdCity.ToUpper()) + " <br /><br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("For any assistance please contact " + creatorDataName + "<br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Thanks <br /><br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Regards,<br />");
                        //bodyMail.AppendLine();
                        //bodyMail.Append("Fleet Team");
                        //bodyMail.AppendLine();

                        //if (!string.IsNullOrEmpty(vendorDataEmail))
                        //{
                        //    rc.To.Add(vendorDataEmail);
                        //}
                        //foreach (var item in fleetEmailList)
                        //{
                        //    rc.CC.Add(item);
                        //}
                        //if (!string.IsNullOrEmpty(employeeDataEmail))
                        //{
                        //    rc.CC.Add(employeeDataEmail);
                        //}
                        return rc;
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Reject:
                    if (input.UserRole == Enums.UserRole.HR && isBenefit)
                    {
                        rc.Subject = ctfData.DocumentNumber + " - Employee Submission";

                        bodyMail.Append("Dear " + ctfData.EmployeeName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Your Document " + ctfData.DocumentNumber + " has been rejected by " + creatorDataName + " for below reason : " + _remarkService.GetRemarkById(input.Comment.Value).REMARK + "<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Please revised and re-submit your request <a href='" + webRootUrl + "/TraCtf/Edit?TraCtfId=" + ctfData.TraCtfId + "&isPersonalDashboard=True" + "'>HERE</a><br />");
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
                        rc.IsCCExist = true;
                    }
                    else if (input.UserRole == Enums.UserRole.Fleet && isBenefit)
                    {
                        rc.Subject = ctfData.DocumentNumber + " - Employee Submission";

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
                        foreach (var item in hrEmailList)
                        {
                            rc.CC.Add(item);
                        }
                        rc.IsCCExist = true;
                    }
                    //if Fleet Reject Benefit
                    else if (input.UserRole == Enums.UserRole.Fleet && !isBenefit)
                    {
                        rc.Subject = ctfData.DocumentNumber + "- Employee Submission";

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
                        rc.Subject = ctfData.DocumentNumber + "- Completed Document";

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
                        bodyMail.Append("Termination Date : " + (ctfData == null ? "" : ctfData.EffectiveDate.Value.ToString("dd-MMM-yyyy")) + " <br />");
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
                        rc.CC.Add(vendorDataEmail);

                        rc.IsCCExist = true;
                    }
                    else if (isBenefit && !ctfData.ExtendVehicle.Value)
                    {
                        rc.Subject = ctfData.DocumentNumber + "- Completed Document";

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
                        bodyMail.Append("Termination Date : " + (ctfData== null ? "" : ctfData.EffectiveDate.Value.ToString("dd-MMM-yyyy")) + " <br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("For any assistance please contact " + fleetApprovalDataName + "<br />");
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
                        rc.CC.Add(vendorDataEmail);

                        rc.IsCCExist = true;
                    }
                    break;
                case Enums.ActionType.Cancel:
                    rc.Subject = ctfData.DocumentNumber + " - Cancelled Document";

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
                    rc.Subject = ctfData.DocumentNumber + " - Extend Vehicle";

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
                    bodyMail.Append("New End Contract Date  : " + (extend == null ? "" : extend.NEW_PROPOSED_DATE.Value.ToString("dd-MMM-yyyy")) + " <br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("For any assistance please contact " + creatorDataName + "<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Thanks <br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Regards,<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Fleet Team");
                    bodyMail.AppendLine();

                    if (!string.IsNullOrEmpty(vendorDataEmail))
                    {
                        rc.To.Add(vendorDataEmail);
                    }

                    //rc.CC.Add(employeeDataEmail);
                   
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
            var dbData = _ctfService.GetCtf().Where(x => x.TRA_CTF_ID == input.DocumentId).FirstOrDefault();

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
            var settingData = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
            var benefitType = settingData.Where(x => x.SETTING_NAME.ToUpper() == "BENEFIT").FirstOrDefault().SETTING_NAME;
            var CopUsage = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.VehicleUsageBenefit) && x.SETTING_NAME.ToUpper() == "COP").FirstOrDefault().SETTING_NAME;

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.Draft)
            {
                dbData.DOCUMENT_STATUS = Enums.DocumentStatus.AssignedForUser;
            }
            else if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.AssignedForUser && dbData.VEHICLE_TYPE == benefitType && dbData.VEHICLE_USAGE == CopUsage)
            {
                dbData.DOCUMENT_STATUS = Enums.DocumentStatus.WaitingHRApproval;
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
            var settingData = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
            var benefitType = settingData.Where(x => x.SETTING_NAME.ToUpper() == "BENEFIT").FirstOrDefault().SETTING_NAME;
            var CopUsage = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.VehicleUsageBenefit) && x.SETTING_NAME.ToUpper() == "COP").FirstOrDefault().SETTING_NAME;

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingFleetApproval && dbData.VEHICLE_TYPE == benefitType && dbData.VEHICLE_USAGE == CopUsage)
            {
                dbData.DOCUMENT_STATUS = Enums.DocumentStatus.WaitingHRApproval;
            }
            else if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingFleetApproval || dbData.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingHRApproval)
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
            else if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingHRApproval)
            {
                dbData.DOCUMENT_STATUS = Enums.DocumentStatus.WaitingFleetApproval;
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

            var CopEndRentList = _fleetService.GetFleet().Where(x => x.IS_ACTIVE
                                && (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT"
                                && (x.VEHICLE_USAGE == null ? "" : x.VEHICLE_USAGE.ToUpper()) == "COP"
                                && x.END_CONTRACT.Value <= dateMinus1).ToList();

            foreach (var copitem in CopEndRentList)
            {
                InActiveCOPEndRent(copitem.MST_FLEET_ID);
            }
            var listCtfInProgress = _ctfService.GetCtf().Where(x => (x.DOCUMENT_STATUS == Enums.DocumentStatus.InProgress || x.DOCUMENT_STATUS == Enums.DocumentStatus.Extended)).ToList();
            listCtfInProgress = listCtfInProgress.Where(x => (x.DOCUMENT_STATUS == Enums.DocumentStatus.InProgress || x.DOCUMENT_STATUS == Enums.DocumentStatus.Extended)
                                                                        && x.EFFECTIVE_DATE.Value <= dateMinus1).ToList();

            foreach (var item in listCtfInProgress)
            {
                UpdateFleet(item.TRA_CTF_ID);

                _uow.SaveChanges();
            }

            EmailNotifChangeCC();
            EmailNotifChangeFleet();
            EmailNotifChangeFunction();
            EmailNotifChangeLocation();
        }
        private void UpdateFleet(long id)
        {
            var CtfData = _ctfService.GetCtfById(id);
            
            var vehicle = _fleetService.GetFleet().Where(x => (x.POLICE_NUMBER == null ? "" : x.POLICE_NUMBER.ToUpper()) == (CtfData.POLICE_NUMBER == null ? "" : CtfData.POLICE_NUMBER.ToUpper())
                                                        && x.IS_ACTIVE && CtfData.EMPLOYEE_ID == x.EMPLOYEE_ID
                                                        && (x.SUPPLY_METHOD == null ? "" : x.SUPPLY_METHOD.ToUpper()) == (CtfData.SUPPLY_METHOD == null ? "" : CtfData.SUPPLY_METHOD.ToUpper())
                                                        && (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == (CtfData.VEHICLE_TYPE == null ? "" : CtfData.VEHICLE_TYPE.ToUpper())).FirstOrDefault();
            if (vehicle == null)
            {
                return ;
            }
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
                        vehicle.END_DATE = CtfData.EFFECTIVE_DATE;
                        vehicle.MODIFIED_BY = "SYSTEM";
                        vehicle.MODIFIED_DATE = DateTime.Now;
                        //vehicle.DOCUMENT_NUMBER = CtfData.DOCUMENT_NUMBER;

                        _fleetService.save(vehicle);

                        var FleetDto = Mapper.Map<FleetDto>(vehicle);

                        FleetDto.MstFleetId = 0;
                        FleetDto.EmployeeID = null;
                        FleetDto.EmployeeName = null;
                        FleetDto.AssignedTo = null;
                        FleetDto.StartDate = DateTime.Now;
                        FleetDto.EndDate = null;
                        FleetDto.VehicleStatus = "LIVE";
                        FleetDto.VehicleUsage = "CFM IDLE";
                        FleetDto.CreatedBy = "SYSTEM";
                        FleetDto.CreatedDate = DateTime.Now;
                        FleetDto.ModifiedBy = null;
                        FleetDto.ModifiedDate = null;
                        FleetDto.IsActive = true;
                        FleetDto.DocumentNumber = CtfData.DOCUMENT_NUMBER;
                        
                        input.ActionType = Enums.ActionType.Completed;
                        CtfWorkflow(input);

                        var IdleCar = Mapper.Map<MST_FLEET>(FleetDto);
                        _fleetService.save(IdleCar);
                    }
                    else
                    {
                        if (!CtfData.MST_REASON.IS_PENALTY || (CtfData.MST_REASON.IS_PENALTY && (CtfData.PENALTY_PRICE == 0 || CtfData.PENALTY_PRICE == null)))
                        {

                            vehicle.IS_ACTIVE = false;
                            vehicle.MODIFIED_BY = "SYSTEM";
                            vehicle.MODIFIED_DATE = DateTime.Now;
                            //vehicle.DOCUMENT_NUMBER = CtfData.DOCUMENT_NUMBER;

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
                            FleetDto.DocumentNumber = CtfData.DOCUMENT_NUMBER;

                           
                            input.ActionType = Enums.ActionType.Completed;
                            CtfWorkflow(input);

                            var TerminateCar = Mapper.Map<MST_FLEET>(FleetDto);
                            _fleetService.save(TerminateCar);

                        }
                        else if (CtfData.MST_REASON.IS_PENALTY && (CtfData.PENALTY_PRICE != 0 && CtfData.PENALTY_PRICE != null) && (CtfData.PENALTY_PO_LINE != "" && CtfData.PENALTY_PO_LINE != null) && (CtfData.PENALTY_PO_NUMBER != "" && CtfData.PENALTY_PO_NUMBER != null))
                        {

                            vehicle.IS_ACTIVE = false;
                            vehicle.MODIFIED_BY = "SYSTEM";
                            vehicle.MODIFIED_DATE = DateTime.Now;
                            //vehicle.DOCUMENT_NUMBER = CtfData.DOCUMENT_NUMBER;

                            _fleetService.save(vehicle);

                            var FleetDto = Mapper.Map<FleetDto>(vehicle);

                            FleetDto.CreatedBy = "SYSTEM";
                            FleetDto.CreatedDate = DateTime.Now;
                            FleetDto.ModifiedBy = null;
                            FleetDto.ModifiedDate = null;
                            FleetDto.VehicleStatus = "TERMINATE";
                            FleetDto.IsActive = false;
                            FleetDto.EndDate = CtfData.EFFECTIVE_DATE;
                            FleetDto.DocumentNumber = CtfData.DOCUMENT_NUMBER;
                            FleetDto.MstFleetId = 0;

                            input.ActionType = Enums.ActionType.Completed;
                            CtfWorkflow(input);

                            var TerminateCar = Mapper.Map<MST_FLEET>(FleetDto);
                            _fleetService.save(TerminateCar);
                            
                        }
                    }
                    var reason = _reasonService.GetReasonById(CtfData.REASON.HasValue ? CtfData.REASON.Value : 0);
                    var GetConfigReason = ConfigurationManager.AppSettings["Reason"].Split(',').ToList();
                    if ((reason == null ? false : GetConfigReason.Where(x => (x == null ? "" : x.ToUpper() )  == reason.REASON.ToUpper()).Count() > 0 ))
                    {
                        var Employee = _employeeService.GetEmployeeById(CtfData.EMPLOYEE_ID);
                        if (Employee != null)
                        {
                            Employee.IS_ACTIVE = false;
                            _employeeService.save(Employee);
                        }
                    }
                }
            }
            else if (CtfData.EXTEND_VEHICLE.Value)
            {
                vehicle.IS_ACTIVE = false;
                vehicle.END_DATE = CtfData.EFFECTIVE_DATE;
                vehicle.MODIFIED_BY = "SYSTEM";
                vehicle.MODIFIED_DATE = DateTime.Now;
                //vehicle.DOCUMENT_NUMBER = CtfData.DOCUMENT_NUMBER;
                _fleetService.save(vehicle);

                var FleetDto = Mapper.Map<FleetDto>(vehicle);
                var extendDto = _ctfExtendService.GetCtfExtend().Where(x => x.TRA_CTF_ID == CtfData.TRA_CTF_ID).FirstOrDefault();

                FleetDto.CreatedBy = "SYSTEM";
                FleetDto.CreatedDate = DateTime.Now;
                FleetDto.EndDate = null;
                FleetDto.ModifiedBy = null;
                FleetDto.ModifiedDate = null;
                FleetDto.VehicleStatus = "LIVE";
                FleetDto.SupplyMethod = "EXTEND";
                FleetDto.IsActive = true;
                FleetDto.PoLine = extendDto.EXTEND_PO_LINE;
                FleetDto.PoNumber = extendDto.EXTEND_PO_NUMBER;
                FleetDto.PoliceNumber = extendDto.EXTEND_POLICE_NUMBER;
                FleetDto.Price = extendDto.EXTEND_PRICE == null ? 0 : extendDto.EXTEND_PRICE.Value;
                FleetDto.StartContract = vehicle.END_CONTRACT.Value.AddDays(1);
                FleetDto.EndContract = extendDto.NEW_PROPOSED_DATE;
                FleetDto.DocumentNumber = CtfData.DOCUMENT_NUMBER;
                FleetDto.MstFleetId = 0;

                input.ActionType = Enums.ActionType.Completed;
                CtfWorkflow(input);

                var ExtendCar = Mapper.Map<MST_FLEET>(FleetDto);
                _fleetService.save(ExtendCar);
                
            }

        }
        public bool CheckCtfExists(TraCtfDto item)
        {
            var isExist = false;
            var exist = _ctfService.GetCtf().Where(x => x.IS_ACTIVE && x.DOCUMENT_STATUS != Enums.DocumentStatus.Completed && x.DOCUMENT_STATUS != Enums.DocumentStatus.Cancelled 
                                                && (x.POLICE_NUMBER == null ? "" : x.POLICE_NUMBER.ToUpper()) == (item.PoliceNumber == null ? ""  : item.PoliceNumber.ToUpper()) 
                                                && x.EMPLOYEE_ID == item.EmployeeId 
                                                && (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == (item.VehicleType == null ? "" : item.VehicleType.ToUpper())).ToList();
            if (exist.Count > 0)
            {
                isExist = true;
            }

            return isExist;
        }

        public void InActiveCOPEndRent(long id)
        {
            var CopVehicle = _fleetService.GetFleetById((int)id);
            CopVehicle.IS_ACTIVE = false;
            CopVehicle.END_DATE = DateTime.Now;
            CopVehicle.MODIFIED_DATE = DateTime.Now;
            CopVehicle.MODIFIED_BY = "SYSTEM";
            CopVehicle.VEHICLE_STATUS = "IN ACTIVE";
            _fleetService.save(CopVehicle);
        }

        #region ----------- Batch Email -----------
        public bool BatchEmailCtf(List<TraCtfDto> ListCtf, string Vendor, string AttachmentWtc, string AttachmentBenefit)
        {

            var rc = new CtfMailNotification();
            var bodyMail = new StringBuilder();
            var CC = ConfigurationManager.AppSettings["CC_MAIL"];
            var GetVendor = _vendorService.GetVendor().Where(x => (x.VENDOR_NAME == null ? "" : x.VENDOR_NAME.ToUpper()) == (Vendor == null ? "" : Vendor.ToUpper()) && x.IS_ACTIVE).FirstOrDefault();
            var EmailVendor = (GetVendor == null ? "" : GetVendor.EMAIL_ADDRESS);
            bool isSend = false;
            rc.Subject = "CTF "+ DateTime.Now.ToString("dd-MMM-yyyy HH:mm");

            bodyMail.Append("Dear Vendor "+ Vendor +",<br /><br />");
            bodyMail.AppendLine();
            bodyMail.Append("Bellow are list of CTF Requests<br />");
            bodyMail.AppendLine();
            bodyMail.Append("Please find the detail in attached document<br />");
            bodyMail.AppendLine();
            bodyMail.Append("<table>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td style = 'border: 1px solid black; padding : 5px' >Doc No</td><td style = 'border: 1px solid black; padding : 5px' >Effective Date</td><td style = 'border: 1px solid black; padding : 5px' >Police Number</td><td style = 'border: 1px solid black; padding : 5px' >Employee Name</td><td style = 'border: 1px solid black; padding : 5px' >Current Basetown</td><td style = 'border: 1px solid black; padding : 5px' >Vehicle Type</td></tr>");
            bodyMail.AppendLine();
            foreach(var CtfDoc in ListCtf)
            {
                bodyMail.Append("<tr><td style = 'border: 1px solid black; padding : 5px' >"+CtfDoc.DocumentNumber+"</td><td style = 'border: 1px solid black; padding : 5px' >"+ (CtfDoc.EffectiveDate == null ? "" : CtfDoc.EffectiveDate.Value.ToString("dd-MMM-yyyy"))+ "</td><td style = 'border: 1px solid black; padding : 5px' >"+ CtfDoc.PoliceNumber+ "</td><td style = 'border: 1px solid black; padding : 5px' >"+ CtfDoc.EmployeeName+ "</td><td style = 'border: 1px solid black; padding : 5px' >"+ CtfDoc.Basetown+ "</td><td style = 'border: 1px solid black; padding : 5px' >"+ CtfDoc.VehicleType+ "</td></tr>");
                bodyMail.AppendLine();
            }
            bodyMail.Append("</table>");
            bodyMail.AppendLine();
            bodyMail.Append("<br /><br />Thank you <br />");
            bodyMail.AppendLine();
            bodyMail.Append("Best Regards,<br />");
            bodyMail.AppendLine();
            bodyMail.Append("Fleet Team");
            bodyMail.AppendLine();
            
            rc.IsCCExist = false;
            rc.Body = bodyMail.ToString();

            rc.To.Add(EmailVendor);
            rc.CC.Add(CC);

            if(rc.CC.Count >0 )rc.IsCCExist = true;

            if (AttachmentWtc != null)
            {
                rc.Attachments.Add(AttachmentWtc);
            }

            if (AttachmentBenefit != null)
            {
                rc.Attachments.Add(AttachmentBenefit);
            }

            if (rc.IsCCExist)
                //Send email with CC
                isSend=_messageService.SendEmailToListWithCC(rc.To, rc.CC, rc.Subject, rc.Body, true, rc.Attachments);
            else
                isSend=_messageService.SendEmailToList(rc.To, rc.Subject, rc.Body, true);

            return isSend;
        }
        public void SendEmailForErrorBatch(string messageError)
        {
            var rc = new CtfMailNotification();
            var bodyMail = new StringBuilder();
            var emailTo = ConfigurationManager.AppSettings["CC_MAIL"];

            rc.Subject = "CTF Error Batch " + DateTime.Today.ToString("dd-MMM-yyyy HH:mm");

            bodyMail.Append("Dear Team,<br /><br />");
            bodyMail.AppendLine();
            bodyMail.Append("Error : " + messageError + ",<br /><br />");
            bodyMail.AppendLine();
            bodyMail.Append("Best Regards,<br />");
            bodyMail.AppendLine();
            bodyMail.Append("Fleet Team");
            bodyMail.AppendLine();

            rc.Body = bodyMail.ToString();
            rc.To.Add(emailTo);

            _messageService.SendEmailToList(rc.To, rc.Subject, rc.Body, true);
        }
        #endregion

        #region --------- Notification Email Data Change ---------
        public void EmailNotifChangeCC()
        {
            try
            {
                var bodyMail = new StringBuilder();
                var rc = new CtfMailNotification();

                var fleetList = string.Empty;
                var fleetEmailList = new List<string>();
                var fleetRole = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                    && x.SETTING_VALUE.Contains("FLEET")).FirstOrDefault().SETTING_VALUE;
                var fleetQuery = "SELECT 'PMI\\' + sAMAccountName AS sAMAccountName FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + fleetRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";

                var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
                var typeEnv = ConfigurationManager.AppSettings["Environment"];
                var serverIntranet = ConfigurationManager.AppSettings["ServerIntranet"];

                if (typeEnv == "VTI")
                {
                    fleetQuery = "SELECT EMPLOYEE_ID FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + fleetRole + "'";
                }

                EntityConnectionStringBuilder e = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["FMSEntities"].ConnectionString);
                string connectionString = e.ProviderConnectionString;
                SqlCommand query = null;
                SqlDataReader reader = null;
                string fleetQueryEmail = string.Empty;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (query = new SqlCommand(fleetQuery, con))
                    {
                        reader = query.ExecuteReader();
                        while (reader.Read())
                        {
                            var fleetLogin = "'" + reader[0].ToString() + "',";
                            fleetList += fleetLogin;
                        }
                    }
                    fleetList = fleetList.TrimEnd(',');

                    fleetQueryEmail = "SELECT EMAIL FROM " + serverIntranet + ".[dbo].[tbl_ADSI_User] WHERE FULL_NAME IN (@fleetList)";


                    if (typeEnv == "VTI")
                    {
                        fleetQueryEmail = "SELECT EMAIL FROM EMAIL_FOR_VTI WHERE FULL_NAME IN (@fleetList)";
                    }
                    using (query = new SqlCommand(fleetQueryEmail, con))
                    {
                        query.Parameters.AddWithValue("@fleetList", fleetList);
                        reader = query.ExecuteReader();
                        while (reader.Read())
                        {
                            fleetEmailList.Add(reader[0].ToString());
                        }
                    }
                    var EmployeeId = new List<string>();
                    var FormalName = new List<string>();
                    var CostCenterOld = new List<string>();
                    var CostCenterNew = new List<string>();
                    var ChangeDate = new List<string>();
                    var Processed = new List<int>();
                    var EmpChangeQuery = "SELECT EMPLOYEE_ID, FORMAL_NAME, COST_CENTER, CC, CHANGE_DATE, PROCESSED FROM EMP_CHANGE WHERE PROCESSED = 0";
                    using (query = new SqlCommand(EmpChangeQuery, con))
                    {
                        reader = query.ExecuteReader();

                        while (reader.Read())
                        {

                            EmployeeId.Add(reader["EMPLOYEE_ID"].ToString());
                            FormalName.Add(reader["FORMAL_NAME"].ToString());
                            CostCenterOld.Add(reader["COST_CENTER"].ToString());
                            CostCenterNew.Add(reader["CC"].ToString());
                            ChangeDate.Add(reader["CHANGE_DATE"].ToString());
                        }

                        rc.Subject = "Cost Center Changes ";

                        bodyMail.Append("Dear Fleet,<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("There are Change Cost Center in Master Employee, Here is the List : <br /><br />");
                        bodyMail.AppendLine();
                        for (int i = 0; i < EmployeeId.Count; i++)
                        {
                            bodyMail.Append("Employee ID = " + EmployeeId[i] + ", New Cost Center = " + CostCenterNew[i] + "   <br /><br />");
                            bodyMail.AppendLine();
                        }
                        bodyMail.Append("<br />Thanks <br /><br />");
                        bodyMail.AppendLine();

                        foreach (var item in fleetEmailList)
                        {
                            rc.To.Add(item);
                        }

                        rc.IsCCExist = false;
                        rc.Body = bodyMail.ToString();


                        //distinct double To email
                        List<string> ListTo = rc.To.Distinct().ToList();

                        if (rc.IsCCExist)
                            //Send email with CC
                            _messageService.SendEmailToListWithCC(ListTo, rc.CC, rc.Subject, rc.Body, true);
                        else
                            _messageService.SendEmailToList(ListTo, rc.Subject, rc.Body, true);

                        for (int i = 0; i < EmployeeId.Count; i++)
                        {
                            //old code
                            //var UpdateProcessed = "UPDATE EMP_CHANGE SET PROCESSED = 1" +
                            //    " WHERE EMPLOYEE_ID ='" + EmployeeId[i] + "' AND COST_CENTER ='" + CostCenterOld[i] + "' AND CC ='" + CostCenterNew[i] + "'";


                            var UpdateProcessed = "UPDATE EMP_CHANGE SET PROCESSED = 1" +
                                " WHERE EMPLOYEE_ID = @employeeID AND COST_CENTER = @costCenter AND CC = @cc";
                            using (query = new SqlCommand(UpdateProcessed, con))
                            {
                                query.Parameters.AddWithValue("@employeeID", EmployeeId[i]);
                                query.Parameters.AddWithValue("@costCenter", CostCenterOld[i]);
                                query.Parameters.AddWithValue("@cc", CostCenterNew[i]);
                                query.ExecuteNonQuery();
                            }
                        }
                    }


                }

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }





            //var bodyMail = new StringBuilder();
            //var rc = new CtfMailNotification();

            //var fleetList = string.Empty;
            //var fleetEmailList = new List<string>();
            //var fleetRole = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
            //                                                    && x.SETTING_VALUE.Contains("FLEET")).FirstOrDefault().SETTING_VALUE;
            //var fleetQuery = "SELECT 'PMI\\' + sAMAccountName AS sAMAccountName FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + fleetRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";

            //var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
            //var typeEnv = ConfigurationManager.AppSettings["Environment"];
            //var serverIntranet = ConfigurationManager.AppSettings["ServerIntranet"];

            //if (typeEnv == "VTI")
            //{
            //    fleetQuery = "SELECT EMPLOYEE_ID FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + fleetRole + "'";
            //}

            //EntityConnectionStringBuilder e = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["FMSEntities"].ConnectionString);
            //string connectionString = e.ProviderConnectionString;
            //SqlConnection con = new SqlConnection(connectionString);
            //con.Open();
            //SqlCommand query = new SqlCommand(fleetQuery, con);
            //SqlDataReader reader = query.ExecuteReader();
            //while (reader.Read())
            //{
            //    var fleetLogin = "'" + reader[0].ToString() + "',";
            //    fleetList += fleetLogin;
            //}

            //fleetList = fleetList.TrimEnd(',');

            //var fleetQueryEmail = "SELECT EMAIL FROM " + serverIntranet + ".[dbo].[tbl_ADSI_User] WHERE FULL_NAME IN (" + fleetList + ")";

            //if (typeEnv == "VTI")
            //{
            //    fleetQueryEmail = "SELECT EMAIL FROM EMAIL_FOR_VTI WHERE FULL_NAME IN (" + fleetList + ")";
            //}

            //query = new SqlCommand(fleetQueryEmail, con);
            //reader = query.ExecuteReader();

            //var EmployeeId = new List<string>();
            //var FormalName = new List<string>();
            //var CostCenterOld = new List<string>();
            //var CostCenterNew = new List<string>();
            //var ChangeDate = new List<string>();
            //var Processed = new List<int>();

            //while (reader.Read())
            //{
            //    fleetEmailList.Add(reader[0].ToString());
            //}

            //var EmpChangeQuery = "SELECT * FROM EMP_CHANGE WHERE PROCESSED = 0";
            //query = new SqlCommand(EmpChangeQuery, con);
            //reader = query.ExecuteReader();

            //while (reader.Read())
            //{

            //    EmployeeId.Add(reader[0].ToString());
            //    FormalName.Add(reader[1].ToString());
            //    CostCenterOld.Add(reader[2].ToString());
            //    CostCenterNew.Add(reader[3].ToString());
            //    ChangeDate.Add(reader[4].ToString());
            //}

            //rc.Subject = "Cost Center Changes ";

            //bodyMail.Append("Dear Fleet,<br /><br />");
            //bodyMail.AppendLine();
            //bodyMail.Append("There are Change Cost Center in Master Employee, Here is the List : <br /><br />");
            //bodyMail.AppendLine();
            //for (int i = 0; i < EmployeeId.Count; i++)
            //{
            //    bodyMail.Append("Employee ID = " + EmployeeId[i] + ", New Cost Center = " + CostCenterNew[i] + "   <br /><br />");
            //    bodyMail.AppendLine();
            //}
            //bodyMail.Append("<br />Thanks <br /><br />");
            //bodyMail.AppendLine();

            //foreach (var item in fleetEmailList)
            //{
            //    rc.To.Add(item);
            //}

            //rc.IsCCExist = false;
            //rc.Body = bodyMail.ToString();


            ////distinct double To email
            //List<string> ListTo = rc.To.Distinct().ToList();

            //if (rc.IsCCExist)
            //    //Send email with CC
            //    _messageService.SendEmailToListWithCC(ListTo, rc.CC, rc.Subject, rc.Body, true);
            //else
            //    _messageService.SendEmailToList(ListTo, rc.Subject, rc.Body, true);

            //for (int i = 0; i < EmployeeId.Count; i++)
            //{
            //    var UpdateProcessed = "UPDATE EMP_CHANGE SET PROCESSED = 1 WHERE EMPLOYEE_ID ='" + EmployeeId[i] + "' AND COST_CENTER ='" + CostCenterOld[i] + "' AND CC ='" + CostCenterNew[i] + "'";
            //    query = new SqlCommand(UpdateProcessed, con);
            //    reader = query.ExecuteReader();
            //}
            //reader.Close();
            //con.Close();
        }
        public void EmailNotifChangeFleet()
        {
            try
            {
                var bodyMail = new StringBuilder();
                var rc = new CtfMailNotification();

                var fleetList = string.Empty;
                var fleetEmailList = new List<string>();
                var fleetRole = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                    && x.SETTING_VALUE.Contains("FLEET")).FirstOrDefault().SETTING_VALUE;
                var fleetQuery = "SELECT 'PMI\\' + sAMAccountName AS sAMAccountName FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + fleetRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";

                var HRList = string.Empty;
                var HrEmailList = new List<string>();
                var HrRole = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                    && x.SETTING_VALUE.Contains("HR")).FirstOrDefault().SETTING_VALUE;
                var HrQuery = "SELECT 'PMI\\' + sAMAccountName AS sAMAccountName FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + HrRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";

                var AdminList = string.Empty;
                var AdminEmailList = new List<string>();
                var AdminRole = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                    && x.SETTING_VALUE.Contains("ADMINISTRATOR")).FirstOrDefault().SETTING_VALUE;
                var AdminQuery = "SELECT 'PMI\\' + sAMAccountName AS sAMAccountName FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + AdminRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";
                
                var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
                var typeEnv = ConfigurationManager.AppSettings["Environment"];
                var serverIntranet = ConfigurationManager.AppSettings["ServerIntranet"];

                if (typeEnv == "VTI")
                {
                    fleetQuery = "SELECT LOGIN FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + fleetRole + "'";
                    HrQuery = "SELECT LOGIN FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + HrRole + "'";
                    AdminQuery = "SELECT LOGIN FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + AdminRole + "'";

                }

                EntityConnectionStringBuilder e = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["FMSEntities"].ConnectionString);
                string connectionString = e.ProviderConnectionString;
                SqlCommand query = null;
                SqlDataReader reader = null;

                string fleetQueryEmail = string.Empty;
                string HrQueryEmail = string.Empty;
                string AdminQueryEmail = string.Empty;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (query = new SqlCommand(fleetQuery, con))
                    {
                        reader = query.ExecuteReader();
                        while (reader.Read())
                        {
                            var fleetLogin = "'" + reader[0].ToString() + "',";
                            fleetList += fleetLogin;
                        }
                    }
                    fleetList = fleetList.TrimEnd(',');

                    fleetQueryEmail = "SELECT EMAIL FROM " + serverIntranet + ".[dbo].[tbl_ADSI_User] WHERE FULL_NAME IN (" + (string.IsNullOrEmpty(fleetList) ? "''" : fleetList) + ")";
                    
                    if (typeEnv == "VTI")
                    {
                        fleetQueryEmail = "SELECT EMAIL FROM EMAIL_FOR_VTI WHERE FULL_NAME IN ("+(string.IsNullOrEmpty(fleetList) ? "''" : fleetList)+")";
                    }
                    using (query = new SqlCommand(fleetQueryEmail, con))
                    {
                        reader = query.ExecuteReader();
                        while (reader.Read())
                        {
                            fleetEmailList.Add(reader[0].ToString());
                        }
                    }

                    using (query = new SqlCommand(HrQuery, con))
                    {
                        reader = query.ExecuteReader();
                        while (reader.Read())
                        {
                            var Hrlogin = "'" + reader[0].ToString() + "',";
                            HRList += Hrlogin;
                        }
                    }
                    HRList = HRList.TrimEnd(',');

                    HrQueryEmail = "SELECT EMAIL FROM " + serverIntranet + ".[dbo].[tbl_ADSI_User] WHERE FULL_NAME IN (" + (string.IsNullOrEmpty(HRList) ? "''" : HRList) + ")";

                    if (typeEnv == "VTI")
                    {
                       HrQueryEmail = "SELECT EMAIL FROM EMAIL_FOR_VTI WHERE FULL_NAME IN ("+(string.IsNullOrEmpty(HRList) ? "''" : HRList)+")";
                    }
                    using (query = new SqlCommand(HrQueryEmail, con))
                    {
                        reader = query.ExecuteReader();
                        while (reader.Read())
                        {
                            HrEmailList.Add(reader[0].ToString());
                        }
                    }

                    using (query = new SqlCommand(AdminQuery, con))
                    {
                        reader = query.ExecuteReader();
                        while (reader.Read())
                        {
                            var AdminLogin = "'" + reader[0].ToString() + "',";
                            AdminList += AdminLogin;
                        }
                    }
                    AdminList =AdminList.TrimEnd(',');

                    AdminQueryEmail = "SELECT EMAIL FROM " + serverIntranet + ".[dbo].[tbl_ADSI_User] WHERE FULL_NAME IN (" + (string.IsNullOrEmpty(AdminList) ? "''" : AdminList) + ")";

                    if (typeEnv == "VTI")
                    {
                        AdminQueryEmail = "SELECT EMAIL FROM EMAIL_FOR_VTI WHERE FULL_NAME IN ("+(string.IsNullOrEmpty(AdminList) ? "''" : AdminList)+")";
                    }
                    using (query = new SqlCommand(AdminQueryEmail, con))
                    {
                        reader = query.ExecuteReader();
                        while (reader.Read())
                        {
                            AdminEmailList.Add(reader[0].ToString());
                        }
                    }
                    var FleetChangeList = _fleetChangeService.GetListFleetChange();
                    if(FleetChangeList.Count> 0)
                    {
                        rc.Subject = "Fleet Changes ";

                        bodyMail.Append("Dear Administrator,<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("There are Change data in Master Fleet, Here is the List : <br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("<table>");
                        bodyMail.AppendLine();
                        bodyMail.Append("<tr><td style = 'border: 1px solid black; padding : 5px' >Police Number</td><td style = 'border: 1px solid black; padding : 5px' >Chasis Number</td><td style = 'border: 1px solid black; padding : 5px' >Employee Id</td><td style = 'border: 1px solid black; padding : 5px' >Employee Name</td><td style = 'border: 1px solid black; padding : 5px' >Field Name</td><td style = 'border: 1px solid black; padding : 5px' >Change Date</td><td style = 'border: 1px solid black; padding : 5px' >Data Before</td><td style = 'border: 1px solid black; padding : 5px' >Data After</td></tr>");
                        bodyMail.AppendLine();
                        foreach (var item in FleetChangeList)
                        {
                            bodyMail.Append("<tr><td style = 'border: 1px solid black; padding : 5px' >" + item.POLICE_NUMBER + "</td><td style = 'border: 1px solid black; padding : 5px' >" + item.CHASIS_NUMBER + "</td><td style = 'border: 1px solid black; padding : 5px' >" + item.EMPLOYEE_ID + "</td><td style = 'border: 1px solid black; padding : 5px' >" + item.EMPLOYEE_NAME + "</td><td style = 'border: 1px solid black; padding : 5px' >" + item.FIELD_NAME + "</td><td style = 'border: 1px solid black; padding : 5px' >" + (item.CHANGE_DATE == null ? "" : item.CHANGE_DATE.Value.ToString("dd-MMM-yyyy")) + "</td><td style = 'border: 1px solid black; padding : 5px' >" + item.DATA_BEFORE + "</td><td style = 'border: 1px solid black; padding : 5px' >" + item.DATA_AFTER + "</td></tr>");
                            bodyMail.AppendLine();
                        }
                        bodyMail.Append("</table>");
                        bodyMail.AppendLine();
                        bodyMail.Append("<br />Thanks <br /><br />");
                        bodyMail.AppendLine();

                        foreach (var item in AdminEmailList)
                        {
                            rc.To.Add(item);
                        }

                        foreach (var item in fleetEmailList)
                        {
                            rc.CC.Add(item);
                        }
                        foreach (var item in HrEmailList)
                        {
                            rc.CC.Add(item);
                        }

                        rc.IsCCExist = true;
                        rc.Body = bodyMail.ToString();

                        //distinct double To email
                        List<string> ListTo = rc.To.Distinct().ToList();
                        List<string> ListCC = rc.CC.Distinct().ToList();
                        bool IsSend;
                        if (rc.IsCCExist)
                            //Send email with CC
                            IsSend = _messageService.SendEmailToListWithCC(ListTo, ListCC, rc.Subject, rc.Body, true);
                        else
                            IsSend = _messageService.SendEmailToList(ListTo, rc.Subject, rc.Body, true);

                        if (IsSend)
                        {
                            var Datesend = DateTime.Now;
                            foreach (var FleetChange in FleetChangeList)
                            {
                                FleetChange.DATE_SEND = Datesend;
                                _fleetChangeService.Save(FleetChange);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }
        public void EmailNotifChangeFunction()
        {
            try
            {
                var bodyMail = new StringBuilder();
                var rc = new CtfMailNotification();

                var fleetList = string.Empty;
                var fleetEmailList = new List<string>();
                var fleetRole = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                    && x.SETTING_VALUE.Contains("FLEET")).FirstOrDefault().SETTING_VALUE;
                var fleetQuery = "SELECT 'PMI\\' + sAMAccountName AS sAMAccountName FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + fleetRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";

                var HRList = string.Empty;
                var HrEmailList = new List<string>();
                var HrRole = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                    && x.SETTING_VALUE.Contains("HR")).FirstOrDefault().SETTING_VALUE;
                var HrQuery = "SELECT 'PMI\\' + sAMAccountName AS sAMAccountName FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + HrRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";

                var AdminList = string.Empty;
                var AdminEmailList = new List<string>();
                var AdminRole = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                    && x.SETTING_VALUE.Contains("ADMINISTRATOR")).FirstOrDefault().SETTING_VALUE;
                var AdminQuery = "SELECT 'PMI\\' + sAMAccountName AS sAMAccountName FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + AdminRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";

                var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
                var typeEnv = ConfigurationManager.AppSettings["Environment"];
                var serverIntranet = ConfigurationManager.AppSettings["ServerIntranet"];

                if (typeEnv == "VTI")
                {
                    fleetQuery = "SELECT LOGIN FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + fleetRole + "'";
                    HrQuery = "SELECT LOGIN FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + HrRole + "'";
                    AdminQuery = "SELECT LOGIN FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + AdminRole + "'";
                }

                EntityConnectionStringBuilder e = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["FMSEntities"].ConnectionString);
                string connectionString = e.ProviderConnectionString;
                SqlCommand query = null;
                SqlDataReader reader = null;

                string fleetQueryEmail = string.Empty;
                string HrQueryEmail = string.Empty;
                string AdminQueryEmail = string.Empty;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (query = new SqlCommand(fleetQuery, con))
                    {
                        reader = query.ExecuteReader();
                        while (reader.Read())
                        {
                            var fleetLogin = "'" + reader[0].ToString() + "',";
                            fleetList += fleetLogin;
                        }
                    }
                    fleetList = fleetList.TrimEnd(',');

                    fleetQueryEmail = "SELECT EMAIL FROM " + serverIntranet + ".[dbo].[tbl_ADSI_User] WHERE FULL_NAME IN (" + (string.IsNullOrEmpty(fleetList ) ? "''" : fleetList) + ")";

                    if (typeEnv == "VTI")
                    {
                        fleetQueryEmail = "SELECT EMAIL FROM EMAIL_FOR_VTI WHERE FULL_NAME IN ("+ (string.IsNullOrEmpty(fleetList) ? "''" : fleetList) + ")";
                    }
                    using (query = new SqlCommand(fleetQueryEmail, con))
                    {
                        query.Parameters.AddWithValue("@fleetList", fleetList);
                        reader = query.ExecuteReader();
                        while (reader.Read())
                        {
                            fleetEmailList.Add(reader[0].ToString());
                        }
                    }

                    using (query = new SqlCommand(HrQuery, con))
                    {
                        reader = query.ExecuteReader();
                        while (reader.Read())
                        {
                            var Hrlogin = "'" + reader[0].ToString() + "',";
                            HRList += Hrlogin;
                        }
                    }
                    HRList = HRList.TrimEnd(',');

                    HrQueryEmail = "SELECT EMAIL FROM " + serverIntranet + ".[dbo].[tbl_ADSI_User] WHERE FULL_NAME IN ("+ (string.IsNullOrEmpty(HRList) ? "''": HRList)+")";

                    if (typeEnv == "VTI")
                    {
                        HrQueryEmail = "SELECT EMAIL FROM EMAIL_FOR_VTI WHERE FULL_NAME IN ("+ (string.IsNullOrEmpty(HRList) ? "''" : HRList) + ")";
                    }
                    using (query = new SqlCommand(HrQueryEmail, con))
                    {
                        reader = query.ExecuteReader();
                        while (reader.Read())
                        {
                            HrEmailList.Add(reader[0].ToString());
                        }
                    }

                    using (query = new SqlCommand(AdminQuery, con))
                    {
                        reader = query.ExecuteReader();
                        while (reader.Read())
                        {
                            var AdminLogin = "'" + reader[0].ToString() + "',";
                            AdminList += AdminLogin;
                        }
                    }
                    AdminList = AdminList.TrimEnd(',');

                    AdminQueryEmail = "SELECT EMAIL FROM " + serverIntranet + ".[dbo].[tbl_ADSI_User] WHERE FULL_NAME IN (" + (string.IsNullOrEmpty(AdminList) ? "''": AdminList) + ")";

                    if (typeEnv == "VTI")
                    {
                        AdminQueryEmail = "SELECT EMAIL FROM EMAIL_FOR_VTI WHERE FULL_NAME IN ("+ (string.IsNullOrEmpty(AdminList) ? "''" : AdminList) + ")";
                    }
                    using (query = new SqlCommand(AdminQueryEmail, con))
                    {
                        reader = query.ExecuteReader();
                        while (reader.Read())
                        {
                            AdminEmailList.Add(reader[0].ToString());
                        }
                    }

                    var FunctionChangeList = _functionChangeService.GetListFunctionChange();
                    if(FunctionChangeList.Count> 0)
                    {
                        rc.Subject = "Function Changes ";

                        bodyMail.Append("Dear Administrator,<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("There are Change data in Master Group Cost Center, Here is the List : <br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("<table>");
                        bodyMail.AppendLine();
                        bodyMail.Append("<tr><td style = 'border: 1px solid black; padding : 5px' >Employee ID</td><td style = 'border: 1px solid black; padding : 5px' >Formal Name</td><td style = 'border: 1px solid black; padding : 5px' >Cost Center</td><td style = 'border: 1px solid black; padding : 5px' >Function Old</td><td style = 'border: 1px solid black; padding : 5px' >Function New</td><td style = 'border: 1px solid black; padding : 5px' >Change Date</td></tr>");
                        bodyMail.AppendLine();
                        foreach (var item in FunctionChangeList)
                        {
                            bodyMail.Append("<tr><td style = 'border: 1px solid black; padding : 5px' >" + item.EMPLOYEE_ID + "</td><td style = 'border: 1px solid black; padding : 5px' >" + item.FORMAL_NAME + "</td><td style = 'border: 1px solid black; padding : 5px' >" + item.COST_CENTER + "</td><td style = 'border: 1px solid black; padding : 5px' >" + item.FUNCTION_OLD + "</td><td style = 'border: 1px solid black; padding : 5px' >" + item.FUNCTION_NEW + "</td><td style = 'border: 1px solid black; padding : 5px' >" + (item.CHANGE_DATE == null ? "" : item.CHANGE_DATE.Value.ToString("dd-MMM-yyyy")) + "</td></tr>");
                            bodyMail.AppendLine();
                        }
                        bodyMail.Append("</table>");
                        bodyMail.AppendLine();
                        bodyMail.Append("<br />Thanks <br /><br />");
                        bodyMail.AppendLine();

                        foreach (var item in AdminEmailList)
                        {
                            rc.To.Add(item);
                        }

                        foreach (var item in fleetEmailList)
                        {
                            rc.CC.Add(item);
                        }
                        foreach (var item in HrEmailList)
                        {
                            rc.CC.Add(item);
                        }

                        rc.IsCCExist = true;
                        rc.Body = bodyMail.ToString();

                        //distinct double To email
                        List<string> ListTo = rc.To.Distinct().ToList();
                        List<string> ListCC = rc.CC.Distinct().ToList();
                        bool IsSend;
                        if (rc.IsCCExist)
                            //Send email with CC
                            IsSend = _messageService.SendEmailToListWithCC(ListTo, ListCC, rc.Subject, rc.Body, true);
                        else
                            IsSend = _messageService.SendEmailToList(ListTo, rc.Subject, rc.Body, true);

                        if (IsSend)
                        {
                            var Datesend = DateTime.Now;
                            foreach (var FunctionChange in FunctionChangeList)
                            {
                                FunctionChange.DATE_SEND = Datesend;
                                _functionChangeService.Save(FunctionChange);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }
        public void EmailNotifChangeLocation()
        {
            try
            {
                var bodyMail = new StringBuilder();
                var rc = new CtfMailNotification();

                var fleetList = string.Empty;
                var fleetEmailList = new List<string>();
                var fleetRole = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                    && x.SETTING_VALUE.Contains("FLEET")).FirstOrDefault().SETTING_VALUE;
                var fleetQuery = "SELECT 'PMI\\' + sAMAccountName AS sAMAccountName FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + fleetRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";

                var HRList = string.Empty;
                var HrEmailList = new List<string>();
                var HrRole = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                    && x.SETTING_VALUE.Contains("HR")).FirstOrDefault().SETTING_VALUE;
                var HrQuery = "SELECT 'PMI\\' + sAMAccountName AS sAMAccountName FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + HrRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";

                var AdminList = string.Empty;
                var AdminEmailList = new List<string>();
                var AdminRole = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                    && x.SETTING_VALUE.Contains("ADMINISTRATOR")).FirstOrDefault().SETTING_VALUE;
                var AdminQuery = "SELECT 'PMI\\' + sAMAccountName AS sAMAccountName FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + AdminRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";

                var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
                var typeEnv = ConfigurationManager.AppSettings["Environment"];
                var serverIntranet = ConfigurationManager.AppSettings["ServerIntranet"];

                if (typeEnv == "VTI")
                {
                    fleetQuery = "SELECT LOGIN FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + fleetRole + "'";
                    HrQuery = "SELECT LOGIN FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + HrRole + "'";
                    AdminQuery = "SELECT LOGIN FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + AdminRole + "'";
                }

                EntityConnectionStringBuilder e = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["FMSEntities"].ConnectionString);
                string connectionString = e.ProviderConnectionString;
                SqlCommand query = null;
                SqlDataReader reader = null;

                string fleetQueryEmail = string.Empty;
                string HrQueryEmail = string.Empty;
                string AdminQueryEmail = string.Empty;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (query = new SqlCommand(fleetQuery, con))
                    {
                        reader = query.ExecuteReader();
                        while (reader.Read())
                        {
                            var fleetLogin = "'" + reader[0].ToString() + "',";
                            fleetList += fleetLogin;
                        }
                    }
                    fleetList = fleetList.TrimEnd(',');

                    fleetQueryEmail = "SELECT EMAIL FROM " + serverIntranet + ".[dbo].[tbl_ADSI_User] WHERE FULL_NAME IN (" + (string.IsNullOrEmpty(fleetList) ? "''" : fleetList ) + ")";

                    if (typeEnv == "VTI")
                    {
                        fleetQueryEmail = "SELECT EMAIL FROM EMAIL_FOR_VTI WHERE FULL_NAME IN ("+ (string.IsNullOrEmpty(fleetList) ? "''" : fleetList) + ")";
                    }
                    using (query = new SqlCommand(fleetQueryEmail, con))
                    {
                        reader = query.ExecuteReader();
                        while (reader.Read())
                        {
                            fleetEmailList.Add(reader[0].ToString());
                        }
                    }

                    using (query = new SqlCommand(HrQuery, con))
                    {
                        reader = query.ExecuteReader();
                        while (reader.Read())
                        {
                            var Hrlogin = "'" + reader[0].ToString() + "',";
                            HRList += Hrlogin;
                        }
                    }
                    HRList = HRList.TrimEnd(',');

                    HrQueryEmail = "SELECT EMAIL FROM " + serverIntranet + ".[dbo].[tbl_ADSI_User] WHERE FULL_NAME IN (" + (string.IsNullOrEmpty(HRList) ? "''" : HRList )+ ")";

                    if (typeEnv == "VTI")
                    {
                        HrQueryEmail = "SELECT EMAIL FROM EMAIL_FOR_VTI WHERE FULL_NAME IN ("+ (string.IsNullOrEmpty(HRList) ? "''" : HRList) + ")";
                    }
                    using (query = new SqlCommand(HrQueryEmail, con))
                    {
                        reader = query.ExecuteReader();
                        while (reader.Read())
                        {
                            HrEmailList.Add(reader[0].ToString());
                        }
                    }

                    using (query = new SqlCommand(AdminQuery, con))
                    {
                        reader = query.ExecuteReader();
                        while (reader.Read())
                        {
                            var AdminLogin = "'" + reader[0].ToString() + "',";
                            AdminList += AdminLogin;
                        }
                    }
                    AdminList = AdminList.TrimEnd(',');

                    AdminQueryEmail = "SELECT EMAIL FROM " + serverIntranet + ".[dbo].[tbl_ADSI_User] WHERE FULL_NAME IN ("+(string.IsNullOrEmpty(AdminList) ? "''" : AdminList)+")";

                    if (typeEnv == "VTI")
                    {
                        AdminQueryEmail = "SELECT EMAIL FROM EMAIL_FOR_VTI WHERE FULL_NAME IN ("+ (string.IsNullOrEmpty(AdminList) ? "''" : AdminList) + ")";
                    }
                    using (query = new SqlCommand(AdminQueryEmail, con))
                    {
                        reader = query.ExecuteReader();
                        while (reader.Read())
                        {
                            AdminEmailList.Add(reader[0].ToString());
                        }
                    }

                    var LocationChangeList = _locationChangeService.GetListLocationChange();
                    if(LocationChangeList.Count > 0)
                    {                    
                        rc.Subject = "Location Changes ";

                        bodyMail.Append("Dear Administrator,<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("There are Change data in Master Location Mapping, Here is the List : <br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("<table>");
                        bodyMail.AppendLine();
                        bodyMail.Append("<tr><td style = 'border: 1px solid black; padding : 5px' >Employee ID</td><td style = 'border: 1px solid black; padding : 5px' >Formal Name</td><td style = 'border: 1px solid black; padding : 5px' >Old Basetown</td><td style = 'border: 1px solid black; padding : 5px' >New Basetown</td><td style = 'border: 1px solid black; padding : 5px' >Old Address</td><td style = 'border: 1px solid black; padding : 5px' >New Address</td><td style = 'border: 1px solid black; padding : 5px' >Old City</td><td style = 'border: 1px solid black; padding : 5px' >New City</td><td style = 'border: 1px solid black; padding : 5px' >Change Date</td></tr>");
                        bodyMail.AppendLine();
                        foreach (var item in LocationChangeList)
                        {
                            bodyMail.Append("<tr><td style = 'border: 1px solid black; padding : 5px' >"+item.EMPLOYEE_ID+"</td><td style = 'border: 1px solid black; padding : 5px' >"+item.FORMAL_NAME+"</td><td style = 'border: 1px solid black; padding : 5px' >"+item.BASETOWN_OLD+"</td><td style = 'border: 1px solid black; padding : 5px' >"+item.BASETOWN_NEW+"</td><td style = 'border: 1px solid black; padding : 5px' >"+item.ADDRESS_OLD+"</td><td style = 'border: 1px solid black; padding : 5px' >"+item.ADDRESS_NEW+"</td><td style = 'border: 1px solid black; padding : 5px' >"+item.CITY_OLD+"</td><td style = 'border: 1px solid black; padding : 5px' >"+item.CITY_NEW+"</td><td style = 'border: 1px solid black; padding : 5px' >"+ (item.CHANGE_DATE == null ? "" : item.CHANGE_DATE.Value.ToString("dd-MMM-yyyy"))+"</td></tr>");
                            bodyMail.AppendLine();
                        }
                        bodyMail.Append("</table>");
                        bodyMail.AppendLine();
                        bodyMail.Append("<br />Thanks <br /><br />");
                        bodyMail.AppendLine();

                        foreach (var item in AdminEmailList)
                        {
                            rc.To.Add(item);
                        }

                        foreach (var item in fleetEmailList)
                        {
                            rc.CC.Add(item);
                        }
                        foreach (var item in HrEmailList)
                        {
                            rc.CC.Add(item);
                        }

                        rc.IsCCExist = true;
                        rc.Body = bodyMail.ToString();

                        //distinct double To email
                        List<string> ListTo = rc.To.Distinct().ToList();
                        List<string> ListCC = rc.CC.Distinct().ToList();
                        bool IsSend;
                        if (rc.IsCCExist)
                            //Send email with CC
                            IsSend = _messageService.SendEmailToListWithCC(ListTo, ListCC, rc.Subject, rc.Body, true);
                        else
                            IsSend = _messageService.SendEmailToList(ListTo, rc.Subject, rc.Body, true);

                        if (IsSend)
                        {
                            var Datesend = DateTime.Now;
                            foreach (var LocationChange in LocationChangeList)
                            {
                                LocationChange.DATE_SEND = Datesend;
                                _locationChangeService.Save(LocationChange);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }
        #endregion  
    }
}

