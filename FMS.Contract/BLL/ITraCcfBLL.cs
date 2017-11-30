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
    public interface ITraCcfBLL
    {
        List<TraCcfDto> GetCcf();
        List<TraCcfDto> GetCcfD1(int TraCCFId);
        TraCcfDto Save(TraCcfDto Dto, Login userLogin);
        void CcfWorkflow(CcfWorkflowDocumentInput param);
        TraCcfDto GetCcfById(long id);
        void CancelCcf(long id, int Remark, string user);
        List<TraCcfDto> GetCcfPersonal(Login userLogin);

        void SaveDetails(TraCcfDetailDto details, Login userLogin);
    }
}
