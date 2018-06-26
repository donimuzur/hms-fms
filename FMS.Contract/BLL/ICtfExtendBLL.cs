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
    public interface ICtfExtendBLL
    {
        void Save(CtfExtendDto CtfExtendDto);
        List<CtfExtendDto> GetCtfExtend();
        CtfExtendDto GetCtfExtendByCtfId(long? CtfId, CtfParamInput input = null);
        void Save(CtfExtendDto Dto, Login userLogin);
    }
}
