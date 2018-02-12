using System.Linq.Expressions;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Utils;
using System.Data.Entity;

namespace FMS.DAL.Services
{
    public class FleetService : IFleetService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<MST_FLEET> _fleetRepository;
        private IGenericRepository<FLEET_CHANGE> _fleetChangeRepository;

        public FleetService(IUnitOfWork uow)
        {
            _uow = uow;
            _fleetRepository = _uow.GetGenericRepository<MST_FLEET>();
            _fleetChangeRepository = _uow.GetGenericRepository<FLEET_CHANGE>();
        }

        public List<MST_FLEET> GetFleet()
        {
            return _fleetRepository.Get().ToList();
        }

        public List<MST_FLEET> GetFleet(int pageNumber, int dataPerPage)
        {

            int skip = dataPerPage * pageNumber;
            return _fleetRepository.Get().OrderBy(x => x.MST_FLEET_ID).Skip(skip).Take(Math.Min(_fleetRepository.Count() - skip, dataPerPage)).ToList();
        }

        public MST_FLEET GetFleetById(int MstFleetId)
        {
            return _fleetRepository.GetByID(MstFleetId);
        }
        public List<MST_FLEET> GetFleetForEndContractLessThan(int days)
        {
            var adddays = DateTime.Now.AddDays(days);
            var data = _fleetRepository.Get().Where(x => x.END_CONTRACT < adddays).ToList();
            return data;
        }
        public void save(MST_FLEET dbFleet)
        {
            _uow.GetGenericRepository<MST_FLEET>().InsertOrUpdate(dbFleet);
            _uow.SaveChanges();
        }

        public void save(MST_FLEET dbFleet, Login userLogin)
        {
            _uow.GetGenericRepository<MST_FLEET>().InsertOrUpdate(dbFleet, userLogin, Enums.MenuList.MasterFleet);
            _uow.SaveChanges();
        }

        public List<MST_FLEET> GetFleetByParam(FleetParamInput input)
        {
            Expression<Func<MST_FLEET, bool>> queryFilterFleet = null;
            queryFilterFleet = c => c.MST_FLEET_ID > 0;
            if (input != null)
            {

                if (!string.IsNullOrEmpty(input.Status))
                {
                    if (input.Status == "True")
                    {
                        queryFilterFleet = c => c.IS_ACTIVE == true;
                    }
                    else if (input.Status == "False")
                    {
                        queryFilterFleet = c => c.IS_ACTIVE == false;
                    }
                }

                if (!string.IsNullOrEmpty(input.EmployeeId))
                {
                    queryFilterFleet = queryFilterFleet.And(c => c.EMPLOYEE_ID == input.EmployeeId);
                }

                if (!string.IsNullOrEmpty(input.FormalName))
                {
                    queryFilterFleet = queryFilterFleet.And(c => c.EMPLOYEE_NAME == input.FormalName);
                }

                if (!string.IsNullOrEmpty(input.PoliceNumber))
                {
                    queryFilterFleet = queryFilterFleet.And(c => c.POLICE_NUMBER == input.PoliceNumber);
                }

                if (!string.IsNullOrEmpty(input.EngineNumber))
                {
                    queryFilterFleet = queryFilterFleet.And(c => c.ENGINE_NUMBER == input.EngineNumber);
                }

                if (!string.IsNullOrEmpty(input.ChasisNumber))
                {
                    queryFilterFleet = queryFilterFleet.And(c => c.CHASIS_NUMBER == input.ChasisNumber);
                }

                if (!string.IsNullOrEmpty(input.VehicleCity))
                {
                    queryFilterFleet = queryFilterFleet.And(c => c.CITY == input.VehicleCity);
                }

                if (!string.IsNullOrEmpty(input.VehicleType))
                {
                    queryFilterFleet = queryFilterFleet.And(c => c.VEHICLE_TYPE == input.VehicleType);
                }

                if (!string.IsNullOrEmpty(input.VehicleUsage))
                {
                    var listFunction = input.VehicleUsage.ToUpper().Split(',').ToList();
                    queryFilterFleet = queryFilterFleet.And(c => listFunction.Contains(c.VEHICLE_USAGE.ToUpper()));
                    //queryFilterFleet = queryFilterFleet.And(c => c.VEHICLE_USAGE == input.VehicleUsage);
                }


                if (!string.IsNullOrEmpty(input.BodyType))
                {
                    var listFunction = input.BodyType.ToUpper().Split(',').ToList();
                    queryFilterFleet = queryFilterFleet.And(c => listFunction.Contains(c.BODY_TYPE.ToUpper()));
                    //queryFilterFleet = queryFilterFleet.And(c => c.BODY_TYPE == input.BodyType);
                }

                if (!string.IsNullOrEmpty(input.SupplyMethod))
                {
                    var listFunction = input.SupplyMethod.ToUpper().Split(',').ToList();
                    queryFilterFleet = queryFilterFleet.And(c => listFunction.Contains(c.SUPPLY_METHOD.ToUpper()));
                    //queryFilterFleet = queryFilterFleet.And(c => c.SUPPLY_METHOD == input.SupplyMethod);
                }

                if (!string.IsNullOrEmpty(input.City))
                {
                    var listFunction = input.City.ToUpper().Split(',').ToList();
                    queryFilterFleet = queryFilterFleet.And(c => listFunction.Contains(c.CITY.ToUpper()));
                    //queryFilterFleet = queryFilterFleet.And(c => c.CITY == input.City);
                }

                if (!string.IsNullOrEmpty(input.StartRent))
                {
                    var StartRent = Convert.ToDateTime(input.StartRent);
                    queryFilterFleet = queryFilterFleet.And(c => c.START_CONTRACT >= StartRent);
                }
                if (!string.IsNullOrEmpty(input.StartRentTo))
                {
                    var StartRentTo = Convert.ToDateTime(input.StartRentTo);
                    queryFilterFleet = queryFilterFleet.And(c => c.START_CONTRACT <= StartRentTo);
                }
                if (!string.IsNullOrEmpty(input.Vendor))
                {
                    var listFunction = input.Vendor.ToUpper().Split(',').ToList();
                    queryFilterFleet = queryFilterFleet.And(c => listFunction.Contains(c.VENDOR_NAME.ToUpper()));
                    //queryFilterFleet = queryFilterFleet.And(c => (c.VENDOR_NAME == null ? "" : c.VENDOR_NAME.ToUpper()) ==input.Vendor.ToUpper());
                }
                if (!string.IsNullOrEmpty(input.Function))
                {
                    var listFunction = input.Function.ToUpper().Split(',').ToList();
                    if (listFunction.Any(x=>x.ToLower() == "others"))
                    {
                        queryFilterFleet = queryFilterFleet.And(c => listFunction.Contains(c.VEHICLE_FUNCTION.ToUpper()) ||
                                                            (c.VEHICLE_FUNCTION.ToUpper() != "SALES" && c.VEHICLE_FUNCTION.ToUpper() != "MARKETING"));
                    }
                    else
                    {
                        queryFilterFleet = queryFilterFleet.And(c => listFunction.Contains(c.VEHICLE_FUNCTION.ToUpper()));
                    }

                    //queryFilterFleet = queryFilterFleet
                    //    .And(c => (c.VEHICLE_FUNCTION == null ? "" : c.VEHICLE_FUNCTION.ToUpper()) == input.Function.ToUpper());
                }
                if (!string.IsNullOrEmpty(input.EndRent))
                {
                    var EndRent = Convert.ToDateTime(input.EndRent);
                    queryFilterFleet = queryFilterFleet.And(c => c.END_CONTRACT >= EndRent);
                }
                if (!string.IsNullOrEmpty(input.EndRentTo))
                {
                    var EndRentTo = Convert.ToDateTime(input.EndRentTo);
                    queryFilterFleet = queryFilterFleet.And(c => c.END_CONTRACT <= EndRentTo);
                }
                if (!string.IsNullOrEmpty(input.EndDate))
                {
                    var EndDate = Convert.ToDateTime(input.EndDate);
                    queryFilterFleet = queryFilterFleet.And(c => c.END_DATE >= EndDate);
                }
                if (!string.IsNullOrEmpty(input.EndDateTo))
                {
                    var EndDateTo = Convert.ToDateTime(input.EndDateTo);
                    queryFilterFleet = queryFilterFleet.And(c => c.END_DATE <= EndDateTo);
                }
                if (!string.IsNullOrEmpty(input.Regional))
                {
                    var listFunction = input.Regional.ToUpper().Split(',').ToList();
                    queryFilterFleet = queryFilterFleet.And(c => listFunction.Contains(c.REGIONAL.ToUpper()));
                    //queryFilterFleet = queryFilterFleet.And(c => c.REGIONAL == input.Regional);
                }
            }
            return _fleetRepository.Get(queryFilterFleet, null, "").ToList();
        }

        public List<FLEET_CHANGE> GetFleetChange()
        {
            return GetFleetChangeByParam(null);
        }

        public List<FLEET_CHANGE> GetFleetChangeByParam(FleetChangeParamInput input)
        {
            Expression<Func<FLEET_CHANGE, bool>> queryFilterFleet = null;

            var fleetDashboard = Convert.ToInt32(ConfigurationManager.AppSettings["FleetDashboard"]);
            var dateFilter = DateTime.Now.AddDays(-fleetDashboard);

            queryFilterFleet = c => c.FLEET_CHANGE_ID > 0 && c.CHANGE_DATE >= dateFilter;
            if (input != null)
            {

                if (!string.IsNullOrEmpty(input.EmployeeId))
                {
                    queryFilterFleet = queryFilterFleet.And(c => c.EMPLOYEE_ID == input.EmployeeId);
                }

                if (!string.IsNullOrEmpty(input.FormalName))
                {
                    queryFilterFleet = queryFilterFleet.And(c => c.EMPLOYEE_NAME == input.FormalName);
                }

                if (!string.IsNullOrEmpty(input.PoliceNumber))
                {
                    queryFilterFleet = queryFilterFleet.And(c => c.POLICE_NUMBER == input.PoliceNumber);
                }

                if (!string.IsNullOrEmpty(input.ChasisNumber))
                {
                    queryFilterFleet = queryFilterFleet.And(c => c.CHASIS_NUMBER == input.ChasisNumber);
                }

                if (!string.IsNullOrEmpty(input.PoliceNumber))
                {
                    queryFilterFleet = queryFilterFleet.And(c => c.POLICE_NUMBER == input.PoliceNumber);
                }
            }
            return _fleetChangeRepository.Get(queryFilterFleet, null, "").ToList();
        }

        public void saveFleetChange(FLEET_CHANGE dbFleetChange, Login userLogin)
        {
            _uow.GetGenericRepository<FLEET_CHANGE>().InsertOrUpdate(dbFleetChange, userLogin, Enums.MenuList.DashboardFleet);
            _uow.SaveChanges();
        }
    }
}
