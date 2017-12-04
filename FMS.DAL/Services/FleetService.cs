using System.Linq.Expressions;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Core;
using System;
using System.Collections.Generic;
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

        public FleetService(IUnitOfWork uow)
        {
            _uow = uow;
            _fleetRepository = _uow.GetGenericRepository<MST_FLEET>();
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
            queryFilterFleet = c => c.IS_ACTIVE == true;
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

                if (!string.IsNullOrEmpty(input.VehicleCity))
                {
                    queryFilterFleet = queryFilterFleet.And(c => c.CITY == input.VehicleCity);


                }

                if (!string.IsNullOrEmpty(input.PoliceNumber))
                {


                    queryFilterFleet = queryFilterFleet.And(c => c.POLICE_NUMBER == input.PoliceNumber);


                }

                if (!string.IsNullOrEmpty(input.VehicleType))
                {

                    queryFilterFleet = queryFilterFleet.And(c => c.VEHICLE_TYPE == input.VehicleType);


                }

                if (!string.IsNullOrEmpty(input.VehicleUsage))
                {

                    queryFilterFleet = queryFilterFleet.And(c => c.VEHICLE_USAGE == input.VehicleUsage);


                }


                if (!string.IsNullOrEmpty(input.BodyType))
                {

                    queryFilterFleet = queryFilterFleet.And(c => c.BODY_TYPE == input.BodyType);


                }

                if (!string.IsNullOrEmpty(input.SupplyMethod))
                {

                    queryFilterFleet = queryFilterFleet.And(c => c.SUPPLY_METHOD == input.SupplyMethod);


                }

                if (!string.IsNullOrEmpty(input.City))
                {

                    queryFilterFleet = queryFilterFleet.And(c => c.CITY == input.City);


                }

                if (!string.IsNullOrEmpty(input.StartRent))
                {

                    queryFilterFleet = queryFilterFleet.And(c => c.START_CONTRACT == Convert.ToDateTime(input.StartRent));

                }

                if (!string.IsNullOrEmpty(input.EndRent))
                {

                    queryFilterFleet = queryFilterFleet.And(c => c.END_CONTRACT == Convert.ToDateTime(input.EndRent));


                }

                if (!string.IsNullOrEmpty(input.Regional))
                {

                    queryFilterFleet = queryFilterFleet.And(c => c.REGIONAL == input.Regional);


                }
            }
            return _fleetRepository.Get(queryFilterFleet, null, "").ToList();
        }
    }
}
