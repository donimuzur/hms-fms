using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.Service
{
    public interface ILocationMappingService
    {
        List<MST_LOCATION_MAPPING> GetLocationMapping();
        List<MST_LOCATION_MAPPING> GetLocationMapping(LocationMappingParamInput input);
        MST_LOCATION_MAPPING GetLocationMappingById(int MstLocationMappingId);
        void Save(MST_LOCATION_MAPPING dbLocationMapping);
        void Save(MST_LOCATION_MAPPING db, Login userLogin);
    }


}
