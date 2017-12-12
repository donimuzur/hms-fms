using FMS.BusinessObject;
using FMS.BusinessObject.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface ICfmIdleReportService
    {
        List<CFM_IDLE_REPORT_DATA> GetCfmIdle(CfmIdleGetByParamInput filter);
    }
}
