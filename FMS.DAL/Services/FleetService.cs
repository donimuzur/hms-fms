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

        public MST_FLEET GetFleetById(int MstFleetId)
        {
            return _fleetRepository.GetByID(MstFleetId);
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
            Expression<Func<MST_FLEET, bool>> queryFilterFleet = c => c.IS_ACTIVE;
            if (input != null)
            {
                if (!string.IsNullOrEmpty(input.EmployeeId))
                {
                    queryFilterFleet = queryFilterFleet.And( c => c.EMPLOYEE_ID == input.EmployeeId);

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
                    queryFilterFleet = queryFilterFleet.And(c => c.VEHICLE_USAGE == input.VehicleUsage);

                }

                
            }
            return _fleetRepository.Get(queryFilterFleet, null, "").ToList();
        }
    }
}
