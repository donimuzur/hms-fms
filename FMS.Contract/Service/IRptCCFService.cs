using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.Service
{
    public interface IRptCCFService
    {
        List<TRA_CCF> GetRptCcf(RptCCFInput filter);
        List<TRA_CCF> GetRptCcfData();
    }
}
