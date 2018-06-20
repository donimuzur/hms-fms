using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface IArchTraCafProgressService
    {
        void Save(ARCH_TRA_CAF_PROGRESS db, Login Login);
    }
}
