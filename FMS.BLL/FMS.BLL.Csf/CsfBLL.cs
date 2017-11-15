﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace FMS.BLL.Csf
{
    public class CsfBLL : ITraCsfBLL
    {
        //private ILogger _logger;
        private ICsfService _CsfService;
        private IUnitOfWork _uow;

        private IDocumentNumberService _docNumberService;
        private IWorkflowHistoryService _workflowService;
        private ISettingService _settingService;
        private IMessageService _messageService;
        private IEmployeeService _employeeService;
        private IEpafService _epafService;

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
        }

        public List<TraCsfDto> GetCsf(Login userLogin, bool isCompleted)
        {
            var settingData = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
            var benefitType = settingData.Where(x => x.SETTING_NAME.ToUpper() == "BENEFIT").FirstOrDefault().MST_SETTING_ID.ToString();
            var wtcType = settingData.Where(x => x.SETTING_NAME.ToUpper() == "WTC").FirstOrDefault().MST_SETTING_ID.ToString();

            var data = _CsfService.GetCsf(userLogin, isCompleted, benefitType, wtcType);
            var retData = Mapper.Map<List<TraCsfDto>>(data);
            return retData;
        }

        public List<TraCsfDto> GetCsfPersonal(Login userLogin)
        {
            var data = _CsfService.GetAllCsf().Where(x => x.EMPLOYEE_ID == userLogin.EMPLOYEE_ID || x.CREATED_BY == userLogin.USERNAME).ToList();
            var retData = Mapper.Map<List<TraCsfDto>>(data);
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
                bool changed = false;

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

                    model = Mapper.Map<TRA_CSF>(item);
                }

                _CsfService.saveCsf(model, userLogin);
                _uow.SaveChanges();

                //set workflow history
                var input = new CsfWorkflowDocumentInput()
                {
                    DocumentId = model.TRA_CSF_ID,
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

            return Mapper.Map<TraCsfDto>(model);
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
                case Enums.ActionType.Approve:
                    ApproveDocument(input);
                    break;
                case Enums.ActionType.Reject:
                    RejectDocument(input);
                    break;
            }

            //todo sent mail
            if (isNeedSendNotif) //SendEmailWorkflow(input);

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
                _messageService.SendEmailToListWithCC(ListTo, mailProcess.CC, mailProcess.Subject, mailProcess.Body, true);
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
            }
            public string Subject { get; set; }
            public string Body { get; set; }
            public List<string> To { get; set; }
            public List<string> CC { get; set; }
            public bool IsCCExist { get; set; }
        }

        private CsfMailNotification ProsesMailNotificationBody(TraCsfDto csfData, CsfWorkflowDocumentInput input)
        {
            var bodyMail = new StringBuilder();
            var rc = new CsfMailNotification();
            var firstText = input.ActionType == Enums.ActionType.Reject ? " Document" : string.Empty;

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
            var typeEnv = ConfigurationManager.AppSettings["Environment"];
            var employeeData = _employeeService.GetEmployeeById(csfData.EMPLOYEE_ID);

            var hrList = new List<string>();
            var fleetList = new List<string>();

            EntityConnectionStringBuilder e = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["FMSEntities"].ConnectionString);
            string connectionString = e.ProviderConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand query = new SqlCommand("SELECT EMPLOYEE_ID FROM LOGIN_FOR_VTI WHERE AD_GROUP = 'PMI ID UR 3066 OPS FMS HR QA IMDL'", con);
            SqlDataReader reader = query.ExecuteReader();
            while (reader.Read())
            {
                var hrEmail = _employeeService.GetEmployeeById(csfData.EMPLOYEE_ID).EMAIL_ADDRESS;
                hrList.Add(hrEmail);
            }

            query = new SqlCommand("SELECT EMPLOYEE_ID FROM LOGIN_FOR_VTI WHERE AD_GROUP = 'PMI ID UR 3066 OPS FMS FLEET QA IMDL'", con);
            reader = query.ExecuteReader();
            while (reader.Read())
            {
                var fleetEmail = _employeeService.GetEmployeeById(csfData.EMPLOYEE_ID).EMAIL_ADDRESS;
                fleetList.Add(fleetEmail);
            }

            reader.Close();
            con.Close();

            switch (input.ActionType)
            {
                case Enums.ActionType.Submit:
                    rc.Subject = csfData.DOCUMENT_NUMBER + " - Benefit Car Request";

                    bodyMail.Append("Dear " + csfData.EMPLOYEE_NAME);
                    bodyMail.AppendLine();
                    bodyMail.Append("Please be advised that due to your Benefit Car entitlement and refering to “HMS 351 - Car For Manager” Principle & Practices, please select Car Model and Types by click in HERE");
                    bodyMail.AppendLine();
                    bodyMail.Append("As per your entitlement, we kindly ask you to complete the form within 14 calendar days to ensure your car will be ready on time and to avoid the consequence as stated in the P&P Car For Manager.");
                    bodyMail.AppendLine();
                    bodyMail.Append("Important Information:");
                    bodyMail.AppendLine();
                    bodyMail.Append("To support you in understanding benefit car (COP/CFM) scheme, the circumstances, and other the terms and conditions, we advise you to read following HR Documents before selecting car scheme and type.");
                    bodyMail.AppendLine();
                    bodyMail.Append("- P&P Car For Manager along with the attachments >> click Car for Manager, Affiliate Practices (link)");
                    bodyMail.AppendLine();
                    bodyMail.Append("- Car types, models, contribution and early termination terms and conditions >> click Car Types and Models, Communication (link)");
                    bodyMail.AppendLine();
                    bodyMail.Append("- Draft of COP / CFM Agreement (attached)");
                    bodyMail.AppendLine();
                    bodyMail.Append("The procurement process will start after receiving the signed forms with approximately 2-3 months lead time, and may be longer depending on the car availability in vendor. Thus, during lead time of procurement, you will be using temporary car.");
                    bodyMail.AppendLine();
                    bodyMail.Append("If you are interested to modify your CAR current entitlement, we encourage you to read following HR Documents regarding flexible benefits.");
                    bodyMail.AppendLine();
                    bodyMail.Append("- P&P Flexible Benefit>> click Flexible Benefits Practices (link)");
                    bodyMail.AppendLine();
                    bodyMail.Append("- Flexible Benefit Design >> click Flexible Benefit Design (link)");
                    bodyMail.AppendLine();
                    bodyMail.Append("- Core Benefits & Allocated Flex Points Communication >> click Core Benefits & Allocated Flex Points Communication (link)");
                    bodyMail.AppendLine();
                    bodyMail.Append("- Coverage Selection Communication >> click Coverage Selection Communication (link)");
                    bodyMail.AppendLine();
                    bodyMail.Append("Should you need any help or have any questions, please do not hesitate to contact the HR Services team:");
                    bodyMail.AppendLine();
                    bodyMail.Append("- Car for Manager : Rizal Setiansyah (ext. 21539) or Astrid Meirina (ext.67165)");
                    bodyMail.AppendLine();
                    bodyMail.Append("- Flexible Benefits : HR Services at YOURHR.ASIA@PMI.COM or ext. 900");
                    bodyMail.AppendLine();
                    bodyMail.Append("- Thank you for your kind attention and cooperation.");
                    bodyMail.AppendLine();

                    rc.To.Add(employeeData.EMAIL_ADDRESS);

                    if (typeEnv == "VTI") {
                        foreach (var item in hrList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    else
                    {
                        rc.CC.Add("");
                        rc.CC.Add("");
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
            dbData.REMARK_ID = null;

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
            }

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
    }
}
