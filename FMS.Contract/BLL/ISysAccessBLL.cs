using FMS.BusinessObject.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.BLL
{
    public interface ISysAccessBLL
    {
        List<SysAccessDto> GetSysAccess();
        SysAccessDto GetSysAccessById(int MstSysAccessId);
        void Save(SysAccessDto Dto);
    }
}
