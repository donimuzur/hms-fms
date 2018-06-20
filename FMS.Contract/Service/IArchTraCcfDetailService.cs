using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface IArchTraCcfDetailService
    {
        void Save(ARCH_TRA_CCF_DETAIL db, Login Login);
    }
}
