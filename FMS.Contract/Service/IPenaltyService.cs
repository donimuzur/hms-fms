using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;

namespace FMS.Contract.Service
{
    public interface IPenaltyService
    {
        List<MST_PENALTY> GetPenalty();
        MST_PENALTY GetPenaltyById(int MstPenaltyId);
    }
}
