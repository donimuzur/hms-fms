using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface IArchTraCtfExtendService
    {
        void Save(ARCH_TRA_CTF_EXTEND db, Login Login);
        ARCH_TRA_CTF_EXTEND GetCtfExtendByCtfId(long? TraCtfId);
    }
}
