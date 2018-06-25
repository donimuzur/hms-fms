using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface IArchTraCcfService
    {
        void Save(ARCH_TRA_CCF db, Login Login);
        List<ARCH_TRA_CCF> GetCcf();
        ARCH_TRA_CCF GetCcfById(long id);
        List<ARCH_TRA_CCF_DETAIL> GetCcfD1();
    }
}
