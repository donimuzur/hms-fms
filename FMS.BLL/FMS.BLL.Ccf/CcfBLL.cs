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

                    //changed = SetChangesHistory(model, item, userId);
                    dbTraCcf = Mapper.Map<TRA_CCF>(Dto);
                    _ccfService.Save(dbTraCcf, userLogin);
                }
                else
                {
                    var inputDoc = new GenerateDocNumberInput();
                    inputDoc.Month = DateTime.Now.Month;
                    inputDoc.Year = DateTime.Now.Year;
                    inputDoc.DocType = (int)Enums.DocumentType.CTF;

                    Dto.DocumentNumber = _docNumberService.GenerateNumber(inputDoc);

                    dbTraCcf = Mapper.Map<TRA_CCF>(Dto);
                    _ccfService.Save(dbTraCcf, userLogin);

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
            if (isNeedSendNotif)//SendEmailWorkflow(input);

                _uow.SaveChanges();
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
            throw new NotImplementedException();
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
    }
}
