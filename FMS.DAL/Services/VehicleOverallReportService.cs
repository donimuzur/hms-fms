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
                    queryFilter = queryFilter.And(c => c.END_RENT >= filter.FromDate);
                }
                if (filter.ToDate.HasValue)
                {
                    queryFilter = queryFilter.And(c => c.END_RENT <= filter.ToDate);
                }
                if (!string.IsNullOrEmpty(filter.SupplyMethod))
                {
                    queryFilter = queryFilter.And(c => c.SUPPLY_METHOD == filter.SupplyMethod);
                }
                if (!string.IsNullOrEmpty(filter.BodyType))
                {
                    queryFilter = queryFilter.And(c => c.BODY_TYPE == filter.BodyType);
                }
                if (!string.IsNullOrEmpty(filter.VehicleType))
                {
                    queryFilter = queryFilter.And(c => c.VEHICLE_TYPE == filter.VehicleType);
                }
                if (!string.IsNullOrEmpty(filter.Vendor))
                {
                    queryFilter = queryFilter.And(c => c.VENDOR == filter.Vendor);
                }
                if (!string.IsNullOrEmpty(filter.Function))
                {
                    queryFilter = queryFilter.And(c => c.FUNCTION == filter.Function);
                }
                if (!string.IsNullOrEmpty(filter.Regional))
                {
                    queryFilter = queryFilter.And(c => c.REGIONAL == filter.Regional);
                }
                if (!string.IsNullOrEmpty(filter.City))
                {
                    queryFilter = queryFilter.And(c => c.CITY.ToUpper() == filter.City.ToUpper());
                }
            }
            return _vehicleReportRepository.Get(queryFilter, null, "").ToList();
        }
    }
}
