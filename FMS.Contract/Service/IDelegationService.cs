using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Business;

namespace FMS.Contract.Service
{
    public interface IDelegationService
    {
        List<MST_DELEGATION> GetDelegation();
        MST_DELEGATION GetDelegationById(int MstDelegationId);
        void save(MST_DELEGATION dbDelegation);
        void save(MST_DELEGATION dbDelegation, Login userLogin);
    }
}
