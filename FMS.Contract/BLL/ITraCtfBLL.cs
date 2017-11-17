using FMS.BusinessObject.Business;
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
        TraCtfDto Save(TraCtfDto Dto, Login userLogin);
        void CtfWorkflow(CtfWorkflowDocumentInput param);
        TraCtfDto GetCtfById(long id);
        void CancelCtf(long id, int Remark, string user);
         List<TraCtfDto> GetCtfPersonal(Login userLogin);
    }
}
