using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface IFleetChangeService
    {
        List<FLEET_CHANGE> GetListFleetChange();
        void Save(FLEET_CHANGE dbFleetChange);
    }
}
