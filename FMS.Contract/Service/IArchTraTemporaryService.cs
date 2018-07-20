using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface IArchTraTemporaryService
    {
        void Save(ARCH_TRA_TEMPORARY db, Login userlogin);
        List<ARCH_TRA_TEMPORARY> GetTemp(Login userLogin, bool isCompleted, string benefitType, string wtcType);
        ARCH_TRA_TEMPORARY GetTempById(long TraTempId);
    }
}
