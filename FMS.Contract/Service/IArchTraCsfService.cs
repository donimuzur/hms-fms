using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface IArchTraCsfService
    {
        void Save(ARCH_TRA_CSF db, Login Login);
        List<ARCH_TRA_CSF> GetCsf(Login userLogin, bool isCompleted, string benefitType, string wtcType);
        ARCH_TRA_CSF GetCsfById(long id);
    }
}
