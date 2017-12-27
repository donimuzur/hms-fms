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
    public class RptCcfService : IRptCCFService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<TRA_CCF> _rptCcfRepository;

        public RptCcfService(IUnitOfWork uow)
        {
            _uow = uow;

            _rptCcfRepository = _uow.GetGenericRepository<TRA_CCF>();
        }

        public List<TRA_CCF> GetRptCcf(RptCCFInput filter)
        {
            Expression<Func<TRA_CCF, bool>> queryFilter = PredicateHelper.True<TRA_CCF>();

            if (filter != null)
            {
                if (filter.PeriodFrom != null)
                {
                    queryFilter = queryFilter.And(c => (c.CREATED_DATE.Day >= filter.PeriodFrom.Day) &&
                                                        (c.CREATED_DATE.Month >= filter.PeriodFrom.Month) &&
                                                        (c.CREATED_DATE.Year >= filter.PeriodFrom.Year));
                }
                if (filter.PeriodTo != null)
                {
                    queryFilter = queryFilter.And(c => (c.CREATED_DATE.Day <= filter.PeriodTo.Day) &&
                                                        (c.CREATED_DATE.Month <= filter.PeriodTo.Month) &&
                                                        (c.CREATED_DATE.Year <= filter.PeriodTo.Year));
                }
                if (filter.Category > 0)
                {
                    queryFilter = queryFilter.And(c => c.COMPLAINT_CATEGORY == filter.Category);
                }
                if (!string.IsNullOrEmpty(filter.Coordinator))
                {
                    queryFilter = queryFilter.And(c => c.COORDINATOR_NAME.ToUpper() == filter.Coordinator.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.Location))
                {
                    queryFilter = queryFilter.And(c => c.LOCATION_CITY.ToUpper() == filter.Location.ToUpper());
                }
                if (filter.CoorKPI >= 0)
                {
                    queryFilter = queryFilter.And(c => c.COORDINATOR_KPI == filter.CoorKPI);
                }
                if (filter.VendorKPI >= 0)
                {
                    queryFilter = queryFilter.And(c => c.VENDOR_KPI == filter.VendorKPI);
                }
            }

            return _rptCcfRepository.Get(queryFilter, null, "").ToList();
        }

        public List<TRA_CCF> GetRptCcfData()
        {
            return _rptCcfRepository.Get().ToList();
        }
    }
}
