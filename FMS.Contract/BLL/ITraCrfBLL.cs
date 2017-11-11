using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;

namespace FMS.Contract.BLL
{
    public interface ITraCrfBLL
    {
        List<TraCrfDto> GetList();
        TraCrfDto GetDataById(long id);
        TraCrfDto SaveCrf(TraCrfDto data);
        List<TraCrfDto> GetCrfByParam(TraCrfEpafParamInput input);
    }
}
