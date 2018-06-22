using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.Service
{
    public interface IArchLocationMappingService
    {
        void Save(ARCH_MST_LOCATION_MAPPING db, Login userlogin);
        List<ARCH_MST_LOCATION_MAPPING> GetLocationMapping(LocationMappingParamInput filter);
        ARCH_MST_LOCATION_MAPPING GetLocationMappingById(int mstLocationMappingId);
    }
}
