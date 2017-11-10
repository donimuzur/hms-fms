using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Core;
using FMS.Core.Exceptions;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.DAL.Services;
using AutoMapper;

namespace FMS.BLL.Csf
{
    public class CsfBLL : ITraCsfBLL
    {
        //private ILogger _logger;
        private ICsfService _CsfService;
        private IUnitOfWork _uow;

        private IDocumentNumberService _docNumberService;
        private IWorkflowHistoryService _workflowService;

        public CsfBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _CsfService = new CsfService(_uow);

            _docNumberService = new DocumentNumberService(_uow);
            _workflowService = new WorkflowHistoryService(_uow);
        }

        public List<TraCsfDto> GetCsf()
        {
            var data = _CsfService.GetCsf();
            var retData = Mapper.Map<List<TraCsfDto>>(data);
            return retData;
        }

        public TraCsfDto Save(TraCsfDto item, string userId)
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
                    model = _CsfService.GetCsf().Where(c => c.TRA_CSF_ID == item.TRA_CSF_ID).FirstOrDefault();

                    if (model == null)
                        throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                    //changed = SetChangesHistory(model, item, userId);

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
                    _CsfService.save(model);
                }

                _uow.SaveChanges();

                //set workflow history
                var input = new CsfWorkflowDocumentInput()
                {
                    DocumentId = model.TRA_CSF_ID,
                    ActionType = Enums.ActionType.Modified,
                    UserId = userId
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
            //var isNeedSendNotif = true;
            switch (input.ActionType)
            {
                case Enums.ActionType.Created:
                    CreateDocument(input);
                    //isNeedSendNotif = false;
                    break;
                //case Enums.ActionType.Submit:
                //    SubmitDocument(input);
                //    break;
                //case Enums.ActionType.Approve:
                //    ApproveDocument(input);
                //    break;
                //case Enums.ActionType.Reject:
                //    RejectDocument(input);
                //    break;
            }

            //todo sent mail
            //if (isNeedSendNotif) SendEmailWorkflow(input);

            _uow.SaveChanges();
        }

        private void CreateDocument(CsfWorkflowDocumentInput input)
        {
            var dbData = _CsfService.GetCsf().Where(x => x.TRA_CSF_ID == input.DocumentId).FirstOrDefault();

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
            dbData.ACTION = input.ActionType;
            dbData.REMARK_ID = null;

            _workflowService.Save(dbData);

        }
    }
}
