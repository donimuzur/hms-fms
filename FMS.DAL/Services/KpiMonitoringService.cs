using FMS.BusinessObject;
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
    public class KpiMonitoringService : IKpiMonitoringService
    {

        private IGenericRepository<KPI_REPORT_DATA> _kpiMonitoringRepository;
        private IUnitOfWork _uow;

        public KpiMonitoringService(IUnitOfWork uow)
        {
            _uow = uow;
            _kpiMonitoringRepository = _uow.GetGenericRepository<KPI_REPORT_DATA>();
        }

        public List<KPI_REPORT_DATA> GetTransaction(KpiMonitoringGetByParamInput filter)
        {
            Expression<Func<KPI_REPORT_DATA, bool>> queryFilter = PredicateHelper.True<KPI_REPORT_DATA>();

            if (filter != null)
            {
                if (filter.FromDate.HasValue)
                {
                    queryFilter = queryFilter.And(c => c.EFFECTIVE_DATE >= filter.FromDate);
                }
                if (filter.ToDate.HasValue)
                {
                    queryFilter = queryFilter.And(c => c.EFFECTIVE_DATE<= filter.ToDate);
                }
                if (!string.IsNullOrEmpty(filter.FormType))
                {
                    queryFilter = queryFilter.And(c => (c.FORM_TYPE == null ? "" : c.FORM_TYPE.ToUpper() ) == filter.FormType.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.VehicleUsage))
                {
                    queryFilter = queryFilter.And(c => (c.VEHICLE_USAGE == null ? "" : c.VEHICLE_USAGE.ToUpper()) == filter.VehicleUsage.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.Location))
                {
                    queryFilter = queryFilter.And(c => (c.ADDRESS == null ? "" : c.ADDRESS.ToUpper()) == filter.Location.ToUpper());
                }
            }
            return _kpiMonitoringRepository.Get(queryFilter, null, "").ToList();
        }
    }
}
