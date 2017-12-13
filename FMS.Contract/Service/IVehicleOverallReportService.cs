using FMS.BusinessObject;
using FMS.BusinessObject.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface IVehicleOverallReportService
    {
        List<VEHICLE_REPORT_DATA> GetVehicle(VehicleOverallReportGetByParamInput filter);
    }
}
