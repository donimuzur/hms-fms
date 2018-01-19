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
    public class VehicleOverallReportService : IVehicleOverallReportService
    {
        private IGenericRepository<VEHICLE_REPORT_DATA> _vehicleReportRepository;
        private IUnitOfWork _uow;

        public VehicleOverallReportService(IUnitOfWork uow)
        {
            _uow = uow;
            _vehicleReportRepository = _uow.GetGenericRepository<VEHICLE_REPORT_DATA>();
        }

        public List<VEHICLE_REPORT_DATA> GetVehicle(VehicleOverallReportGetByParamInput filter)
        {
            Expression<Func<VEHICLE_REPORT_DATA, bool>> queryFilter = PredicateHelper.True<VEHICLE_REPORT_DATA>();

            if (filter != null)
            {
                if (filter.VehicleStatus.HasValue)
                {
                    queryFilter = queryFilter.And(c => c.VEHICLE_STATUS == filter.VehicleStatus);
                }
                if (filter.FromDate.HasValue)
                {
                    queryFilter = queryFilter.And(c => c.START_RENT >= filter.FromDate);
                }
                if (filter.ToDate.HasValue)
                {
                    queryFilter = queryFilter.And(c => c.START_RENT <= filter.ToDate);
                }
                if (!string.IsNullOrEmpty(filter.SupplyMethod))
                {
                    queryFilter = queryFilter.And(c => (string.IsNullOrEmpty(c.SUPPLY_METHOD) == true ? "" : c.SUPPLY_METHOD.ToUpper()) == filter.SupplyMethod.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.BodyType))
                {
                    queryFilter = queryFilter.And(c => (string.IsNullOrEmpty(c.BODY_TYPE) == true ? "" : c.BODY_TYPE.ToUpper()) == filter.BodyType.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.VehicleType))
                {
                    queryFilter = queryFilter.And(c => (string.IsNullOrEmpty(c.VEHICLE_TYPE ) == true ? "" : c.VEHICLE_TYPE.ToUpper())== filter.VehicleType.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.Vendor))
                {
                    queryFilter = queryFilter.And(c => (string.IsNullOrEmpty(c.VENDOR) == true ? "" :c.VENDOR.ToUpper())== filter.Vendor.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.Function))
                {
                    queryFilter = queryFilter.And(c => (string.IsNullOrEmpty(c.FUNCTION )== true ? "" :c.FUNCTION.ToUpper()  )== filter.Function.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.Regional))
                {
                    queryFilter = queryFilter.And(c => (string.IsNullOrEmpty(c.REGIONAL) == true ? "" : c.REGIONAL.ToUpper()) == filter.Regional.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.City))
                {
                    queryFilter = queryFilter.And(c => (string.IsNullOrEmpty(c.CITY) == true ? "" :c.CITY.ToUpper()) == filter.City.ToUpper());
                }
            }
            return _vehicleReportRepository.Get(queryFilter, null, "").ToList();
        }
    }
}
