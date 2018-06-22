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
    public class ArchLocationMappingService : IArchLocationMappingService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_MST_LOCATION_MAPPING> _archLocationMappingRepository;
        public ArchLocationMappingService(IUnitOfWork uow)
        {
            _uow = uow;
            _archLocationMappingRepository = uow.GetGenericRepository<ARCH_MST_LOCATION_MAPPING>();
        }
        public void Save(ARCH_MST_LOCATION_MAPPING db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_MST_LOCATION_MAPPING>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }
        public List<ARCH_MST_LOCATION_MAPPING> GetLocationMapping(LocationMappingParamInput filter)
        {
            Expression<Func<ARCH_MST_LOCATION_MAPPING, bool>> queryFilter = PredicateHelper.True<ARCH_MST_LOCATION_MAPPING>();

            if (filter != null)
            {

                if (!string.IsNullOrEmpty(filter.Address))
                {
                    queryFilter = queryFilter.And(c => (c.ADDRESS == null ? "".Contains(filter.Address.ToUpper()) : c.ADDRESS.ToUpper().Contains(filter.Address.ToUpper())));
                }
                if (!string.IsNullOrEmpty(filter.Basetown))
                {
                    var listFunction = filter.Basetown.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.BASETOWN == null ? "" : c.BASETOWN.ToUpper())));
                }
                if (!string.IsNullOrEmpty(filter.Location))
                {
                    var listFunction = filter.Location.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.LOCATION == null ? "" : c.LOCATION.ToUpper())));
                }
                if (!string.IsNullOrEmpty(filter.ZonePriceList))
                {
                    var listFunction = filter.ZonePriceList.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.ZONE_PRICE_LIST == null ? "" : c.ZONE_PRICE_LIST.ToUpper())));
                }
                if (!string.IsNullOrEmpty(filter.Region))
                {
                    var listFunction = filter.Region.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.REGION == null ? "" : c.REGION.ToUpper())));
                }
                if (!string.IsNullOrEmpty(filter.ZoneSales))
                {
                    var listFunction = filter.ZoneSales.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.ZONE_SALES == null ? "" : c.ZONE_SALES.ToUpper())));
                }
                if (filter.DateFrom.HasValue)
                {
                    queryFilter = queryFilter.And(c => c.VALIDITY_FROM >= filter.DateFrom);
                }

                if (filter.DateTo.HasValue)
                {
                    queryFilter = queryFilter.And(c => (c.VALIDITY_FROM <= filter.DateTo));
                }

            }

            return _archLocationMappingRepository.Get(queryFilter, null, "").ToList();
        }
        public ARCH_MST_LOCATION_MAPPING GetLocationMappingById(int mstLocationMappingId)
        {
            return _archLocationMappingRepository.GetByID(mstLocationMappingId);
        }
    }
}
