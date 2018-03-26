using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Core;
using FMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public List<MST_LOCATION_MAPPING> GetLocationMapping(LocationMappingParamInput input)
        {
            Expression<Func<MST_LOCATION_MAPPING, bool>> queryFilter = PredicateHelper.True<MST_LOCATION_MAPPING>();

            if (input != null)
            {

                if (!string.IsNullOrEmpty(input.Address))
                {
                    queryFilter = queryFilter.And(c => (c.ADDRESS == null  ? "".Contains(input.Address.ToUpper()) : c.ADDRESS.ToUpper().Contains(input.Address.ToUpper())) );
                }
                if (!string.IsNullOrEmpty(input.Basetown))
                {
                    var listFunction = input.Basetown.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.BASETOWN == null ? "" : c.BASETOWN.ToUpper())));
                }
                if (!string.IsNullOrEmpty(input.Location))
                {
                    var listFunction = input.Location.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.LOCATION == null ? "" : c.LOCATION.ToUpper())));
                }
                if (!string.IsNullOrEmpty(input.ZonePriceList))
                {
                    var listFunction = input.ZonePriceList.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.ZONE_PRICE_LIST == null ? "" : c.ZONE_PRICE_LIST.ToUpper())));
                }
                if (!string.IsNullOrEmpty(input.Region))
                {
                    var listFunction = input.Region.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.REGION == null ? "" : c.REGION.ToUpper())));
                }
                if (!string.IsNullOrEmpty(input.ZoneSales))
                {
                    var listFunction = input.ZoneSales.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.ZONE_SALES == null ? "" : c.ZONE_SALES.ToUpper())));
                }
                if (input.DateFrom.HasValue)
                {
                    queryFilter = queryFilter.And(c => c.VALIDITY_FROM >= input.DateFrom);
                }

                if (input.DateTo.HasValue)
                {
                    queryFilter = queryFilter.And(c => (c.VALIDITY_FROM  <= input.DateTo));
                }

            }

            return _locationmappingRepository.Get(queryFilter, null, "").ToList();
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
