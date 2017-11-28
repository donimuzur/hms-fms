using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
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
        void Save(CtfExtendDto Dto, Login userLogin);
    }
}
