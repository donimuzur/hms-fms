using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.BusinessObject.Inputs;
namespace FMS.Contract.Service
{
    public interface IRptPoService
    {
        List<PO_REPORT_DATA> GetRptPo(RptPoByParamInput filter);
        List<PO_REPORT_DATA> GetRptPoData();
    }
}
