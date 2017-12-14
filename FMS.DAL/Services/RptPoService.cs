using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;
using NLog;
using FMS.BusinessObject.Inputs;
using System.Linq.Expressions;
using FMS.Utils;


namespace FMS.DAL.Services
{
    public class RptPoService : IRptPoService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<PO_REPORT_DATA> _rptPoRepository;

        public RptPoService(IUnitOfWork uow)
        {
            _uow = uow;

            _rptPoRepository = _uow.GetGenericRepository<PO_REPORT_DATA>();
        }

        public List<PO_REPORT_DATA> GetRptPo(RptPoByParamInput filter)
        {
            Expression<Func<PO_REPORT_DATA, bool>> queryFilter = PredicateHelper.True<PO_REPORT_DATA>();

            if (filter != null)
            {
                if (filter.PeriodFrom != null)
                {
                    queryFilter = queryFilter.And(c => c.CREATED_DATE >= filter.PeriodFrom);
                }
                if (filter.PeriodTo != null)
                {
                    queryFilter = queryFilter.And(c => c.CREATED_DATE <= filter.PeriodTo);
                }
                if (!string.IsNullOrEmpty(filter.EmployeeName))
                {
                    queryFilter = queryFilter.And(c => c.EMPLOYEE_NAME.ToUpper() == filter.EmployeeName.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.CostCenter))
                {
                    queryFilter = queryFilter.And(c => c.COST_CENTER.ToUpper() == filter.CostCenter.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.SupplyMethod))
                {
                    queryFilter = queryFilter.And(c => c.SUPPLY_METHOD.ToUpper() == filter.SupplyMethod.ToUpper());
                }
            }

            return _rptPoRepository.Get(queryFilter, null, "").ToList();
        }
    }
}
