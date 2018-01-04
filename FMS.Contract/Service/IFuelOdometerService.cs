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
    public interface IFuelOdometerService
    {
        List<MST_FUEL_ODOMETER> GetFuelOdometer();
        List<MST_FUEL_ODOMETER> GetFuelOdometerByParam(FuelOdometerParamInput param);
        MST_FUEL_ODOMETER GetByID(long MstFuelOdometerID);
        void save(MST_FUEL_ODOMETER dbSetting, Login userlogin);
    }
}
