using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Business;

namespace FMS.Contract.BLL
{
    public interface ITraCrfBLL
    {
        List<TraCrfDto> GetList();
        TraCrfDto GetDataById(long id);
        TraCrfDto SaveCrf(TraCrfDto data,Login userLogin);
        

        List<EpafDto> GetCrfEpaf(bool isActive = true);
        void SubmitCrf(long crfId,Login currentUser);
        TraCrfDto AssignCrfFromEpaf(long epafId, Login CurrentUser);
        bool IsAllowedEdit(Login currentUser, TraCrfDto data);
        bool IsAllowedApprove(Login currentUser, TraCrfDto data);

        void Approve(long TraCrfId,Login currentUser);

        void Reject(long TraCrfId, int? remark, Login currentUser);
    }
}
