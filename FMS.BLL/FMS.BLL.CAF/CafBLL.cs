using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Utils;

namespace FMS.BLL.CAF
{
    public class CafBLL : ICafBLL
    {
        private ICAFService _CafService;
        private IUnitOfWork _uow;

        private IDocumentNumberService _docNumberService;
        private IWorkflowHistoryService _workflowService;
        private ISettingService _settingService;
        private IMessageService _messageService;
        private IEmployeeService _employeeService;
        private IEpafService _epafService;
        private IRemarkService _remarkService;
        private ITemporaryService _temporaryService;
        private IVendorService _vendorService;

        public CafBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _CafService = new CafService(_uow);

            _docNumberService = new DocumentNumberService(_uow);
            _workflowService = new WorkflowHistoryService(_uow);
            _settingService = new SettingService(_uow);
            _messageService = new MessageService(_uow);
            _employeeService = new EmployeeService(_uow);
            _epafService = new EpafService(_uow);
            _remarkService = new RemarkService(_uow);
            _temporaryService = new TemporaryService(_uow);
            _vendorService = new VendorService(_uow);
        }

        public void Save(BusinessObject.Dto.TraCafDto data, BusinessObject.Business.Login user)
        {
            throw new NotImplementedException();
        }

        public List<BusinessObject.Dto.TraCafDto> GetCaf()
        {
            var data = _CafService.GetList();

            return Mapper.Map<List<TraCafDto>>(data);
        }

        public BusinessObject.Dto.TraCafDto GetById(long id)
        {
            var data = _CafService.GetCafById(id);

            return Mapper.Map<TraCafDto>(data);
        }

        public void SaveList(List<TraCafDto> data, BusinessObject.Business.Login CurrentUser)
        {
            
            var datatoSave = Mapper.Map<List<TRA_CAF>>(data);
            foreach (var caf in datatoSave)
            {
                TRA_CAF dataCaf = _CafService.GetCafByNumber(caf.SIRS_NUMBER);
                if (!string.IsNullOrEmpty(caf.VENDOR_NAME))
                {
                    var vendorData = _vendorService.GetByShortName(caf.VENDOR_NAME);
                    caf.VENDOR_ID = vendorData.MST_VENDOR_ID;
                }
                else
                {
                    caf.VENDOR_ID = null;
                }
                if (dataCaf == null)
                {
                    caf.REMARK = null;
                    caf.IS_ACTIVE = true;
                    caf.DOCUMENT_NUMBER = _docNumberService.GenerateNumber(new GenerateDocNumberInput()
                    {
                        DocType = (int) Enums.DocumentType.CAF,
                        Month = DateTime.Now.Month,
                        Year = DateTime.Now.Year

                    });
                    caf.DOCUMENT_STATUS = (int) Enums.DocumentStatus.Draft;
                    _CafService.Save(caf, CurrentUser);
                }
                else
                {
                    dataCaf.IS_ACTIVE = true;
                    dataCaf.REMARK = null;
                }
                
                
            }
            _uow.SaveChanges();
        }


        public void ValidateCaf(TraCafDto dataTovalidate, out string message)
        {
            List<string> validation = new List<string>(); 
            message = "";

            if (string.IsNullOrEmpty(dataTovalidate.SirsNumber))
            {
                validation.Add("Sirs number cannot be empty.");
            }

            if (string.IsNullOrEmpty(dataTovalidate.PoliceNumber))
            {
                validation.Add("Police number cannot be empty.");
            }

            var index = 0;
            if (validation.Count > 0)
            {
                foreach (var vld in validation)
                {
                    if (index > 0)
                    {
                        message += ", " + vld;
                    }
                    else
                    {
                        message += vld;
                    }
                    
                }

                return;
            }

            var dbData = _CafService.GetCafByNumber(dataTovalidate.SirsNumber);
            
            if (dbData != null)
            {
                validation.Add("Sirs Number already registered in FMS.");
            }


            var isAnyCaf = _CafService.IsCafExist(dataTovalidate.PoliceNumber, dataTovalidate.IncidentDate);
            if (isAnyCaf)
            {
                validation.Add(string.Format("CAF For police number : {0} and Incident Date : {1} already registered in FMS.",dataTovalidate.PoliceNumber,dataTovalidate.IncidentDate.ToString("dd-MMM-yyyy")));
                
            }

            if (validation.Count > 0)
            {
                foreach (var vld in validation)
                {
                    if (index > 0)
                    {
                        message += ", " + vld;
                    }
                    else
                    {
                        message += vld;
                    }

                }

                
            }
        }


        public TraCafDto GetCafBySirs(string sirsNumber)
        {
            var data = _CafService.GetCafByNumber(sirsNumber);
            return Mapper.Map<TraCafDto>(data);
        }


        public int SaveProgress(TraCafProgressDto traCafProgressDto,string sirsNumber, BusinessObject.Business.Login CurrentUser)
        {
            var data = Mapper.Map<TRA_CAF_PROGRESS>(traCafProgressDto);
            
            data.CREATED_BY = CurrentUser.USER_ID;
            data.CREATED_DATE = DateTime.Now;
            
            var caf = _CafService.GetCafByNumber(sirsNumber);
            var lastStatus = caf.DOCUMENT_STATUS;
            var countDetails = _CafService.SaveProgress(data,sirsNumber,CurrentUser);
            
            if (lastStatus != data.STATUS_ID)
            {
                if (countDetails == 0)
                {
                    _workflowService.Save(new WorkflowHistoryDto()
                    {
                        ACTION = Enums.ActionType.Modified,
                        ACTION_DATE = DateTime.Now,
                        ACTION_BY = CurrentUser.USER_ID,
                        FORM_ID = caf.TRA_CAF_ID,
                        MODUL_ID = Enums.MenuList.TraCaf

                    });
                    _uow.SaveChanges();    
                }
                
                
                caf.DOCUMENT_STATUS = data.STATUS_ID.HasValue ? data.STATUS_ID.Value : caf.DOCUMENT_STATUS;
                
                
                var dataCaf = Mapper.Map<TraCafDto>(caf);
                SendEmailWorkflow(dataCaf, Enums.ActionType.Submit);
            }
            return  lastStatus;
        }


        private void SendEmailWorkflow(TraCafDto crfData, Enums.ActionType action)
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

        private FMSMailNotification ProsesMailNotificationBody(TraCafDto crfData, Enums.ActionType action)
        {
            var bodyMail = new StringBuilder();
            var rc = new FMSMailNotification();

            //var vehTypeBenefit = _settingService.GetSetting().Where(x => x.SETTING_GROUP == "VEHICLE_TYPE" && x.SETTING_NAME == "BENEFIT").FirstOrDefault().MST_SETTING_ID;

            //var isBenefit = crfData.VEHICLE_TYPE.ToUpper().Contains("BENEFIT");
            string creatorDataEmail = "";
            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
            var typeEnv = ConfigurationManager.AppSettings["Environment"];
            var serverIntranet = ConfigurationManager.AppSettings["ServerIntranet"];
            var employeeData = _employeeService.GetEmployeeById(crfData.EmployeeId);

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
            var creatorQuery =
                "SELECT EMAIL from " + serverIntranet + ".[dbo].[tbl_ADSI_User] where FULL_NAME like 'PMI\\" +
                crfData.CreatedBy + "'";
            if (typeEnv == "VTI")
            {
                hrQueryEmail = "SELECT EMAIL FROM EMAIL_FOR_VTI WHERE FULL_NAME IN (" + hrList + ")";
                fleetQueryEmail = "SELECT EMAIL FROM EMAIL_FOR_VTI WHERE FULL_NAME IN (" + fleetList + ")";
                creatorQuery = "SELECT EMAIL FROM LOGIN_FOR_VTI WHERE LOGIN like '" + crfData.CreatedBy + "'";
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

            reader.Close();
            con.Close();

            rc.Subject = "CAF - Car Accident Report Progress";

            bodyMail.Append("Dear " + crfData.EmployeeId + ",<br /><br />");
            bodyMail.AppendLine();
            bodyMail.Append("Your filed Car accident report has  updated.<br />");
            bodyMail.AppendLine();
            
            bodyMail.AppendLine();
            bodyMail.Append("SIRS Number : " + crfData.SirsNumber + "<br />");
            bodyMail.AppendLine();
            bodyMail.Append("Current status of your report : "+ crfData.DocumentStatusString +"<br />");
            bodyMail.AppendLine();
            bodyMail.Append("Send confirmation by clicking below CAF number:<br />");
            bodyMail.AppendLine();
            bodyMail.Append("<a href='" + webRootUrl + "/TraCaf/Details/" + crfData.TraCafId + "?isPersonalDashboard=True'>" +
                            "CAF Number : "+ crfData.DocumentNumber + "</a> requested by " + crfData.EmployeeName +
                            "<br /><br />");
            bodyMail.AppendLine();
            bodyMail.Append("Thanks<br /><br />");
            bodyMail.AppendLine();
            bodyMail.Append("Regards,<br />");
            bodyMail.AppendLine();
            bodyMail.Append("Fleet Team");
            bodyMail.AppendLine();



            
            rc.To.Add(employeeData.EMAIL_ADDRESS);
            rc.CC.Add(creatorDataEmail);
            foreach (var item in fleetEmailList)
            {
                rc.CC.Add(item);
            }
            
            

            rc.Body = bodyMail.ToString();

            if (rc.CC.Count > 0)
            {
                rc.IsCCExist = true;
            }

            return rc;
        }


        public List<TraCafDto> GetCafPersonal(Login CurrentUser)
        {
            var allData = this.GetCaf();
            var data = allData.Where(x => x.IsActive && (x.EmployeeId == CurrentUser.EMPLOYEE_ID
                || x.CreatedBy == CurrentUser.USER_ID)).ToList();

            var dataIds = data.Select(x => x.TraCafId).ToList();
            var dataWorkflow = _workflowService.GetWorkflowHistoryByUser((int)Enums.DocumentType.CAF, CurrentUser.USER_ID);
            var formIdList = dataWorkflow.Where(x => x.FORM_ID != null && !dataIds.Contains(x.FORM_ID.Value)).GroupBy(x => x.FORM_ID).Select(x => x.Key).ToList();


            var myWorkflowData = allData.Where(x => formIdList.Contains(x.TraCafId)).ToList();
            data.AddRange(myWorkflowData);
            return data;
        }


        public void CompleteCaf(int TraCafId, Login CurrentUser)
        {
            var data = _CafService.GetCafById(TraCafId);

            if (data.DOCUMENT_STATUS == (int)Enums.DocumentStatus.Delivery)
            {
                var lastStatus = data.TRA_CAF_PROGRESS.OrderByDescending(x => x.STATUS_ID).Select(x => x.STATUS_ID).FirstOrDefault();
                _CafService.SaveProgress(new TRA_CAF_PROGRESS() { 
                    CREATED_BY = CurrentUser.USER_ID,
                    CREATED_DATE = DateTime.Now,
                    ACTUAL = DateTime.Now,
                    //ESTIMATION = DateTime.Now,
                    MODIFIED_BY = CurrentUser.USER_ID,
                    MODIFIED_DATE = DateTime.Now,
                    PROGRESS_DATE = DateTime.Now,
                    STATUS_ID = (int)Enums.DocumentStatus.Completed,
                    TRA_CAF_ID = data.TRA_CAF_ID
                }, data.SIRS_NUMBER, CurrentUser);
                data.DOCUMENT_STATUS = (int)Enums.DocumentStatus.Completed;

                _workflowService.Save(new WorkflowHistoryDto()
                {
                    ACTION = Enums.ActionType.Modified,
                    ACTION_DATE = DateTime.Now,
                    ACTION_BY = CurrentUser.USER_ID,
                    FORM_ID = data.TRA_CAF_ID,
                    MODUL_ID = Enums.MenuList.TraCaf

                });
                _uow.SaveChanges();

                var dataCaf = Mapper.Map<TraCafDto>(data);
                SendEmailWorkflow(dataCaf, Enums.ActionType.Completed);
            }
        }


        public void CloseCaf(long traCafId)
        {
            var data = _CafService.GetCafById(traCafId);
            data.DOCUMENT_STATUS = (int) Enums.DocumentStatus.Cancelled;
            data.IS_ACTIVE = false;
            _uow.SaveChanges();
        }
    }
}
