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
    public interface IFleetService
    {
        List<MST_FLEET> GetFleet();
        List<MST_FLEET> GetFleet(int pageNumber, int dataPerPage);
        MST_FLEET GetFleetById(int MstFleetId);
        void save(MST_FLEET  dbFleet);
        void save(MST_FLEET dbFleet, Login userLogin);
        List<MST_FLEET> GetFleetForEndContractLessThan(int days);
        List<MST_FLEET> GetFleetByParam(FleetParamInput fleetParamInput);
        List<FLEET_CHANGE> GetFleetChange();
        List<FLEET_CHANGE> GetFleetChangeByParam(FleetChangeParamInput fleetChangeParamInput);
        void saveFleetChange(FLEET_CHANGE dbFleetChange, Login userLogin);
    }
}
