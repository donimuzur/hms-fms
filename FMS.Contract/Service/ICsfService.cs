using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface ICsfService
    {
        List<TRA_CSF> GetCsf(Login userLogin, bool isCompleted, string benefitType, string wtcType);
        void saveCsf(TRA_CSF dbTraCsf, Login userlogin);
        void CancelCsf(long id, int Remark, string user);
        TRA_CSF GetCsfById(long id);
        List<TRA_CSF> GetAllCsf();
    }
}
