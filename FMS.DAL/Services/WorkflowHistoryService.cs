using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.Contract.Service;

namespace FMS.DAL.Services
{
    public class WorkflowHistoryService : IWorkflowHistoryService
    {
        private IUnitOfWork _uow;

        private IGenericRepository<TRA_WORKFLOW_HISTORY> _workflowRepository;

        public WorkflowHistoryService(IUnitOfWork uow)
        {
            _uow = uow;
            _workflowRepository = _uow.GetGenericRepository<TRA_WORKFLOW_HISTORY>();
        }

        public void Save(WorkflowHistoryDto history)
        {
            TRA_WORKFLOW_HISTORY dbData = null;
            if (history.WORKFLOW_HISTORY_ID > 0)
            {
                dbData = _workflowRepository.GetByID(history.WORKFLOW_HISTORY_ID);
                Mapper.Map<WorkflowHistoryDto, TRA_WORKFLOW_HISTORY>(history, dbData);
            }
            else
            {
                dbData = Mapper.Map<TRA_WORKFLOW_HISTORY>(history);
                _workflowRepository.Insert(dbData);
            }
        }
    }
}
