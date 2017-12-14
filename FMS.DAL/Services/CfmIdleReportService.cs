using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FMS.DAL.Services
{
    public class CfmIdleReportService : ICfmIdleReportService
    {
        private IGenericRepository<CFM_IDLE_REPORT_DATA> _cfmIdleReportRepository;
        private IUnitOfWork _uow;

        public CfmIdleReportService(IUnitOfWork uow)
        {
            _uow = uow;
            _cfmIdleReportRepository = _uow.GetGenericRepository<CFM_IDLE_REPORT_DATA>();
        }

        public List<CFM_IDLE_REPORT_DATA> GetCfmIdle(CfmIdleGetByParamInput filter)
        {
            Expression<Func<CFM_IDLE_REPORT_DATA, bool>> queryFilter = PredicateHelper.True<CFM_IDLE_REPORT_DATA>();

            if (filter != null)
            {
                if (filter.FromDate.HasValue)
                {
                    queryFilter = queryFilter.And(c => c.START_IDLE >= filter.FromDate);
                }
                if (filter.ToDate.HasValue)
                {
                    queryFilter = queryFilter.And(c => c.END_IDLE <= filter.ToDate);
                }
                if (!string.IsNullOrEmpty(filter.CostCenter))
                {
                    queryFilter = queryFilter.And(c => c.COST_CENTER == filter.CostCenter);
                }
                if (!string.IsNullOrEmpty(filter.PoliceNumber))
                {
                    queryFilter = queryFilter.And(c => c.POLICE_NUMBER.ToUpper() == filter.PoliceNumber.ToUpper());
                }
               
            }
            return _cfmIdleReportRepository.Get(queryFilter, null, "").ToList();
        }
    }
}
