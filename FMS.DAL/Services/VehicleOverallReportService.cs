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
                if (filter.EndContract_FromDate.HasValue)
                {
                    queryFilter = queryFilter.And(c => c.END_RENT >= filter.EndContract_FromDate);
                }
                if (filter.EndContract_ToDate.HasValue)
                {
                    queryFilter = queryFilter.And(c => c.END_RENT <= filter.EndContract_ToDate);
                }
                if (filter.EndDate_FromDate.HasValue)
                {
                    queryFilter = queryFilter.And(c => c.END_DATE >= filter.EndDate_FromDate);
                }
                if (filter.EndDate_ToDate.HasValue)
                {
                    queryFilter = queryFilter.And(c => c.END_DATE <= filter.EndDate_ToDate);
                }
                if (!string.IsNullOrEmpty(filter.PoliceNumber))
                {
                    queryFilter = queryFilter.And(c => (string.IsNullOrEmpty(c.POLICE_NUMBER) == true ? "" : c.POLICE_NUMBER.ToUpper()) == filter.PoliceNumber.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.EmployeeName))
                {
                    queryFilter = queryFilter.And(c => (string.IsNullOrEmpty(c.EMPLOYEE_NAME) == true ? "" : c.EMPLOYEE_NAME.ToUpper()) == filter.EmployeeName.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.EngineNumber))
                {
                    queryFilter = queryFilter.And(c => (string.IsNullOrEmpty(c.ENGINE_NUMBER) == true ? "" : c.ENGINE_NUMBER.ToUpper()) == filter.EngineNumber.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.EmployeeID))
                {
                    queryFilter = queryFilter.And(c => (string.IsNullOrEmpty(c.EMPLOYEE_ID) == true ? "" : c.EMPLOYEE_ID.ToUpper()) == filter.EmployeeID.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.ChasisNumber))
                {
                    queryFilter = queryFilter.And(c => (string.IsNullOrEmpty(c.CHASIS_NUMBER) == true ? "" : c.CHASIS_NUMBER.ToUpper()) == filter.ChasisNumber.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.SupplyMethod))
                {
                    var listFunction = filter.SupplyMethod.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains(c.SUPPLY_METHOD.ToUpper()));

                }
                if (!string.IsNullOrEmpty(filter.BodyType))
                {
                    var listFunction = filter.BodyType.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains(c.BODY_TYPE.ToUpper()));

                }
                if (!string.IsNullOrEmpty(filter.VehicleType))
                {
                    queryFilter = queryFilter.And(c => (string.IsNullOrEmpty(c.VEHICLE_TYPE ) == true ? "" : c.VEHICLE_TYPE.ToUpper())== filter.VehicleType.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.VehicleUsage))
                {
                    var listFunction = filter.VehicleUsage.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains(c.VEHICLE_USAGE.ToUpper()));

                }
                if (!string.IsNullOrEmpty(filter.Vendor))
                {
                    var listFunction = filter.Vendor.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains(c.VENDOR.ToUpper()));

                }
                if (!string.IsNullOrEmpty(filter.Function))
                {
                    var listFunction = filter.Function.ToUpper().Split(',').ToList();
                    if (listFunction.Any(x => x.ToLower() == "others"))
                    {
                        queryFilter = queryFilter.And(c => listFunction.Contains(c.FUNCTION.ToUpper()) ||
                                                            (c.FUNCTION.ToUpper() != "SALES" && c.FUNCTION.ToUpper() != "MARKETING"));
                    }
                    else
                    {
                        queryFilter = queryFilter.And(c => listFunction.Contains(c.FUNCTION.ToUpper()));
                    }
                }
                if (!string.IsNullOrEmpty(filter.Regional))
                {
                    var listFunction = filter.Regional.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains(c.REGIONAL.ToUpper()));

                }
                if (!string.IsNullOrEmpty(filter.City))
                {
                    var listFunction = filter.City.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains(c.CITY.ToUpper()));

                }
            }
            return _vehicleReportRepository.Get(queryFilter, null, "").ToList();
        }
    }
}
