using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface ISysAccessService
    {
        List<MST_SYSACCESS> GetSysAccess();
        MST_SYSACCESS GetSysAccessById(int MstSysAccessId);
        void Save(MST_SYSACCESS dbSysAccess);
    }
}
