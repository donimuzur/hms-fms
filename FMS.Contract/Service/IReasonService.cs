using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface IReasonService
    {
        List<MST_REASON> GetReason();
        void save(MST_REASON dbReason);
        MST_REASON GetReasonById(int MstReasonId);
    }
}
