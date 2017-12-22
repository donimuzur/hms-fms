using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;

namespace FMS.Contract.Service
{
    public interface IPenaltyService
    {
        List<MST_PENALTY> GetPenalty();
        MST_PENALTY GetPenaltyById(int MstPenaltyId);
        void save(MST_PENALTY dbPenalty);
        void save(MST_PENALTY dbPenalty, Login userLogin);
    }
}
