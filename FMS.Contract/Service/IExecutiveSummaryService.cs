using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;

namespace FMS.Contract.Service
{
    public interface IExecutiveSummaryService
    {
        List<NO_OF_VEHICLE_REPORT_DATA> GetAllNoVehicle();
    }
}
