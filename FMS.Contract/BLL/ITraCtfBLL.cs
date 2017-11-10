using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.BLL
{
    public interface ITraCtfBLL
    {
        List<TraCtfDto> GetCtf();
        TraCtfDto Save(TraCtfDto Dto, string userId);
        void CtfWorkflow(CtfWorkflowDocumentInput param);
    }
}
