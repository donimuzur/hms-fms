using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;

namespace FMS.Contract.BLL
{
    public interface IExecutiveSummaryBLL
    {
        List<ExecutiveSummaryDto> GetExecutiveSummary();
    }
}
