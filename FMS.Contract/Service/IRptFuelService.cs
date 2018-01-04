using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.Service
{
    public interface IRptFuelService
    {
        List<FUEL_REPORT_DATA> GetRptFuel(RptFuelByParamInput filter);
        List<FUEL_REPORT_DATA> GetRptFuelData();
    }
}
