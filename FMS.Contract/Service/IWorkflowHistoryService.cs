using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;

namespace FMS.Contract.Service
{
    public interface IWorkflowHistoryService
    {
        void Save(WorkflowHistoryDto history);
    }
}
