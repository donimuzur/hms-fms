using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;

namespace FMS.Contract.Service
{
    public interface IWorkflowHistoryService
    {
        void Save(WorkflowHistoryDto history);

        List<TRA_WORKFLOW_HISTORY> GetWorkflowHistoryByUser(int modulId, string UserId);
    }
}
