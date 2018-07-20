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
    public class ArchPricelistService : IArchPricelistService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_MST_PRICELIST> _archPricelistRepository;

        public ArchPricelistService(IUnitOfWork uow)
        {
            _uow = uow;
            _archPricelistRepository = uow.GetGenericRepository<ARCH_MST_PRICELIST>();
        }
        public void Save(ARCH_MST_PRICELIST db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_MST_PRICELIST>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }
        public List<ARCH_MST_PRICELIST> GetPriceList(PricelistParamInput filter)
        {
            Expression<Func<ARCH_MST_PRICELIST, bool>> queryFilter = PredicateHelper.True<ARCH_MST_PRICELIST>();

            if (filter != null)
            {

                if (!string.IsNullOrEmpty(filter.VehicleUsage))
                {
                    var listFunction = filter.VehicleUsage.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.VEHICLE_USAGE == null ? "" : c.VEHICLE_USAGE.ToUpper())));
                }
                if (!string.IsNullOrEmpty(filter.VehicleType))
                {
                    var listFunction = filter.VehicleType.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.VEHICLE_TYPE == null ? "" : c.VEHICLE_TYPE.ToUpper())));
                }
                if (!string.IsNullOrEmpty(filter.vendor))
                {
                    var listFunction = filter.vendor.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.VENDOR == null ? "" : c.VENDOR.ToString())));
                }
                if (!string.IsNullOrEmpty(filter.ZonePricelist))
                {
                    var listFunction = filter.ZonePricelist.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.ZONE_PRICE_LIST == null ? "" : c.ZONE_PRICE_LIST.ToUpper())));
                }
            }

            return _archPricelistRepository.Get(queryFilter, null, "").ToList();
        }

        public ARCH_MST_PRICELIST GetPriceListById(int Id)
        {
            return _archPricelistRepository.GetByID(Id);
        }
    }
}
