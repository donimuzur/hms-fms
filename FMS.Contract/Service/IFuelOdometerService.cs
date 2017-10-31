using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface IFuelOdometerService
    {
        List<MST_FUEL_ODOMETER> GetFuelOdometer();
    }
}
