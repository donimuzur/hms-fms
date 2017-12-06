using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.DAL.Services;

namespace FMS.BLL.ExecutiveSummary
{
    public class ExecutiveSummaryBLL : IExecutiveSummaryBLL
    {
        private ICsfService _CsfService;
        private IUnitOfWork _uow;

        public ExecutiveSummaryBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _CsfService = new CsfService(_uow);
        }

        public List<ExecutiveSummaryDto> GetExecutiveSummary()
        {
            throw new NotImplementedException();
        }
    }
}
