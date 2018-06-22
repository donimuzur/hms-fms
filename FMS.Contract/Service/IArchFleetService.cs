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
    public interface IArchFleetService
    {
        void Save(ARCH_MST_FLEET db, Login userlogin);
        List<ARCH_MST_FLEET> GetFleet();
        ARCH_MST_FLEET GetFleetById(int MstFleetId);
        List<ARCH_MST_FLEET> GetFleetByParam(FleetParamInput input);
    }
}
