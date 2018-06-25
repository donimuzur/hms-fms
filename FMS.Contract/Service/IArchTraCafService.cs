using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface IArchTraCafService
    {
        void Save(ARCH_TRA_CAF db, Login Login);
        List<ARCH_TRA_CAF> GetList();
        ARCH_TRA_CAF GetCafById(long id);
    }
}
