using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;
﻿using System.Linq.Expressions;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;
using FMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.DAL.Services
{
    public class FleetChangeService : IFleetChangeService
    {
        private IGenericRepository<FLEET_CHANGE> _fleetChangeRepo;
        private IUnitOfWork _uow;

        public FleetChangeService(IUnitOfWork uow)
        {
            _uow = uow;
            _fleetChangeRepo = uow.GetGenericRepository<FLEET_CHANGE>();
        }

        public List<FLEET_CHANGE> GetListFleetChange()
        {
            var data =  _fleetChangeRepo.Get().Where(x => x.DATE_SEND == null).ToList();
            return data;
        }

        public void Save(FLEET_CHANGE dbFleetChange)
        {
            _fleetChangeRepo.InsertOrUpdate(dbFleetChange);
            _uow.SaveChanges();

        }
    }
}
