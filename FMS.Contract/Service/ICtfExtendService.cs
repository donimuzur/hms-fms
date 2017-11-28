using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface ICtfExtendService
    {
        void Save(TRA_CTF_EXTEND dbCtfExtend);
        List<TRA_CTF_EXTEND> GetCtfExtend();
        void Save(TRA_CTF_EXTEND dbCtf, Login userlogin);
    }
}
