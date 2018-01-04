﻿using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.DAL.Services
{
    public class LocationMappingService : ILocationMappingService
    {

        private IUnitOfWork _uow;

        private IGenericRepository<MST_LOCATION_MAPPING> _locationmappingRepository;

        public LocationMappingService(IUnitOfWork uow)
        {
            _uow = uow;
            _locationmappingRepository = _uow.GetGenericRepository<MST_LOCATION_MAPPING>();
        }

        public List<MST_LOCATION_MAPPING> GetLocationMapping()
        {
            return _locationmappingRepository.Get().ToList();
        }

        public MST_LOCATION_MAPPING GetLocationMappingById(int MstLocationMappingId)
        {
            return _locationmappingRepository.GetByID(MstLocationMappingId);

        }

        public void Save (MST_LOCATION_MAPPING dbLocationMapping)
        {
            _locationmappingRepository.InsertOrUpdate(dbLocationMapping);
        }
      
        public void Save(MST_LOCATION_MAPPING dbLocationMapping, Login userLogin)
        {
            _locationmappingRepository.InsertOrUpdate(dbLocationMapping, userLogin, Enums.MenuList.MasterLocationMapping);
        }


    }
}
