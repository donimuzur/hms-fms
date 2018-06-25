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
    public interface IArchVehicleSpectService
    {
        List<ARCH_MST_VEHICLE_SPECT> GetVehicleSpect(VehicleSpectParamInput filter);
        void Save(ARCH_MST_VEHICLE_SPECT db, Login userlogin);
    }
}
