using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface IPenaltyLogicService
    {
        MST_PENALTY_LOGIC GetPenaltyLogicByID(int MstPenaltyLogicId);
        List<MST_PENALTY_LOGIC> GetPenaltyLogic();
        void Save(MST_PENALTY_LOGIC dbPenaltyLogic);
    }
}
