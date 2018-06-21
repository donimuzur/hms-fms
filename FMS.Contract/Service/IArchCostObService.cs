using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface IArchCostObService
    {
        void Save(ARCH_MST_COST_OB db, Login userlogin);
        List<ARCH_MST_COST_OB> GetCostObByFilter(CostObParamInput filter);
    }
}
