using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Business;

namespace FMS.Contract.BLL
{
    public interface ITraCrfBLL
    {
        List<TraCrfDto> GetList();
        TraCrfDto GetDataById(long id);
        TraCrfDto SaveCrf(TraCrfDto data,Login userLogin);
        List<TraCrfDto> GetCrfByParam(TraCrfEpafParamInput input);

        List<EpafDto> GetCrfEpaf(bool isActive = true);
        void SubmitCrf(long crfId,Login currentUser);
    }
}
