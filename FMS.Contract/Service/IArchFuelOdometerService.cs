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
    public interface IArchFuelOdometerService
    {
        void Save(ARCH_MST_FUEL_ODOMETER db, Login userlogin);
        List<ARCH_MST_FUEL_ODOMETER> GetFuelOdometerByParam(FuelOdometerParamInput param);
    }
}
