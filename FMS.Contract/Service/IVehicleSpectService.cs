using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Business;

namespace FMS.Contract.Service
{
    public interface IVehicleSpectService
    {
        List<MST_VEHICLE_SPECT> GetVehicleSpect();
        MST_VEHICLE_SPECT GetVehicleSpectById(int MstVehicleSpectId);
        void save(MST_VEHICLE_SPECT dbVehicleSpect);
        void save(MST_VEHICLE_SPECT dbVehicleSpect, Login userLogin);
    }
}
