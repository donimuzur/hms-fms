using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.Service
{
    public interface IAutoGrService
    {
        List<AUTO_GR> GetAutoGr();
        List<AUTO_GR> GetAutoGr(RptAutoGrInput rptAutoGrInput);

        List<AUTO_GR_DETAIL> GetAutoGrDetails(List<int> autoGrIds);
    }
}
