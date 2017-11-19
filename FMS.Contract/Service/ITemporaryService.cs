using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface ITemporaryService
    {
        TRA_TEMPORARY GetTemporaryById(long id);
        void saveTemporary(TRA_TEMPORARY dbTraTemporary, Login userlogin);
        List<TRA_TEMPORARY> GetTemp(Login userLogin, bool isCompleted, string benefitType, string wtcType);
        List<TRA_TEMPORARY> GetAllTemp();
    }
}
