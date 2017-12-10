using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Utils;

namespace FMS.DAL.Services
{
    public class ExecutiveSummaryService : IExecutiveSummaryService
    {
        private IUnitOfWork _uow;

        private IGenericRepository<NO_OF_VEHICLE_REPORT_DATA> _noVehRepository;

        public ExecutiveSummaryService(IUnitOfWork uow)
        {
            _uow = uow;
            _noVehRepository = _uow.GetGenericRepository<NO_OF_VEHICLE_REPORT_DATA>();
        }

        public List<NO_OF_VEHICLE_REPORT_DATA> GetAllNoVehicle(VehicleGetByParamInput filter)
        {
            Expression<Func<NO_OF_VEHICLE_REPORT_DATA, bool>> queryFilter = PredicateHelper.True<NO_OF_VEHICLE_REPORT_DATA>();

            if (filter != null) {
                if (filter.MonthFrom > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_MONTH >= filter.MonthFrom);
                }
                if (filter.MonthTo > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_MONTH <= filter.MonthTo);
                }
                if (filter.YearFrom > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_YEAR >= filter.YearFrom);
                }
                if (filter.YearTo > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_YEAR <= filter.YearTo);
                }
                if (!string.IsNullOrEmpty(filter.VehicleType))
                {
                    queryFilter = queryFilter.And(c => c.VEHICLE_TYPE.ToUpper() == filter.VehicleType.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.SupplyMethod))
                {
                    queryFilter = queryFilter.And(c => c.SUPPLY_METHOD.ToUpper() == filter.SupplyMethod.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.Function))
                {
                    queryFilter = queryFilter.And(c => c.FUNCTION.ToUpper() == filter.Function.ToUpper());
                }
            }

            return _noVehRepository.Get(queryFilter, null, "").ToList();
        }
    }
}
