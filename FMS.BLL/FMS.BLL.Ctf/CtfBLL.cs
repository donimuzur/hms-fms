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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private IPenaltyLogicService _penaltyLogicService;
        private IPriceListService _pricelistService;
        private IFleetService _fleetService;

        public CtfBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _ctfService = new CtfService(uow);
            _docNumberService = new DocumentNumberService(uow);
            _workflowService = new WorkflowHistoryService(uow);
            _settingService = new SettingService(uow);
            _reasonService = new ReasonService(uow);
            _penaltyLogicService = new PenaltyLogicService(uow);
            _pricelistService = new PriceListService(uow);
            _fleetService = new FleetService(uow);
        }

        public List<TraCtfDto> GetCtf()
        {
            var data = _ctfService.GetCtf();
            var redata = Mapper.Map<List<TraCtfDto>>(data);
            return redata;
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

        public TraCtfDto PenaltyCost (TraCtfDto CtfDto)
        {
         
            if (CtfDto == null)
                CtfDto.Penalty = null;CtfDto.PenaltyPrice = null;
            var reason = _reasonService.GetReasonById(CtfDto.Reason.Value).REASON;
            if (reason == "RESIGN")
            {
                
                CtfDto.Penalty = 10000;
            }

            return CtfDto;
        }

        public decimal? RefundCost(TraCtfDto CtfDto)
        {
            var fleet = _fleetService.GetFleet().Where(x => x.EMPLOYEE_ID == CtfDto.EmployeeId && x.POLICE_NUMBER == CtfDto.PoliceNumber).FirstOrDefault();
            if (fleet == null) return null;

            var installmentEmp = _pricelistService.GetPriceList().Where(x => x.MANUFACTURER == fleet.MANUFACTURER && x.MODEL == fleet.MODEL && x.SERIES == fleet.SERIES && x.IS_ACTIVE == true).FirstOrDefault().INSTALLMEN_EMP;
            if (installmentEmp == null) return null;


            var rentMonth = ((fleet.END_CONTRACT.Value.Year - fleet.START_CONTRACT.Value.Year) * 12) + fleet.END_CONTRACT.Value.Month - fleet.START_CONTRACT.Value.Month;
                
            if(_reasonService.GetReasonById(CtfDto.Reason.Value).REASON.ToLower() == "RESIGN")
            {
                var cost1 = (rentMonth * installmentEmp);
                return cost1;
            }

            var cost2 = (rentMonth * installmentEmp);
            return cost2;

        }


        public void CtfWorkflow(CtfWorkflowDocumentInput input)
        {
            //var isNeedSendNotif = true;
            switch (input.ActionType)
            {
                case Enums.ActionType.Created:
                    CreateDocument(input);
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
            //if (isNeedSendNotif) SendEmailWorkflow(input);

            _uow.SaveChanges();
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
            dbData.REMARK_ID = null;

            _workflowService.Save(dbData);

        }

        public void CancelCtf(long id, int Remark, string user)
        {
            _ctfService.CancelCtf(id, Remark, user);
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


        public TraCtfDto GetCtfById(long id)
        {
            var data = _ctfService.GetCtfById(id);
            var retData = Mapper.Map<TraCtfDto>(data);
            return retData;
        }
        private void ApproveDocument(CtfWorkflowDocumentInput input)
        {
            var dbData = _ctfService.GetCtfById(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

        
            if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingFleetApproval && input.EndRent == true)
            {
                dbData.DOCUMENT_STATUS = Enums.DocumentStatus.InProgress;
            }
            else if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingFleetApproval && input.EndRent == false)
            {
                dbData.DOCUMENT_STATUS = Enums.DocumentStatus.Completed;
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
    }
}

