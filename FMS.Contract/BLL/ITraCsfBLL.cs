using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.BLL
{
    public interface ITraCsfBLL
    {
        List<TraCsfDto> GetCsf();
        TraCsfDto Save(TraCsfDto item, Login userLogin);
        void CsfWorkflow(CsfWorkflowDocumentInput input);
        void CancelCsf(long id, int Remark, string user);
    }
}
