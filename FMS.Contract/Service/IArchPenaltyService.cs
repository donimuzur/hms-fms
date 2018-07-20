using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Inputs;
using FMS.BusinessObject.Dto;

namespace FMS.Contract.Service
{
    public interface IArchPenaltyService
    {
        void Save(ARCH_MST_PENALTY db, Login userlogin);
        List<ARCH_MST_PENALTY> GetPenalty(PenaltyParamInput filter);
        ARCH_MST_PENALTY GetPenaltyById(int id);
    }
}
