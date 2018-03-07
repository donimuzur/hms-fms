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
    public class PriceListService : IPriceListService
    {
        private IUnitOfWork _uow;

        private IGenericRepository<MST_PRICELIST> _priceListRepository;

        public PriceListService(IUnitOfWork uow)
        {
            _uow = uow;
            _priceListRepository = _uow.GetGenericRepository<MST_PRICELIST>();
        }
        
        public List<MST_PRICELIST> GetPriceList()
        {
            return _priceListRepository.Get().ToList();
        }

        public MST_PRICELIST GetPriceListById(int MstPriceListId)
        {
            return _priceListRepository.GetByID(MstPriceListId);
        }

        public MST_PRICELIST GetExist(string Model)
        {
            return _priceListRepository.Get(x => x.MODEL == Model).FirstOrDefault(); ;
        }

        public void save(MST_PRICELIST dbPriceList)
        {
            _uow.GetGenericRepository<MST_PRICELIST>().InsertOrUpdate(dbPriceList);
        }

        public void save(MST_PRICELIST dbPriceList, Login userLogin)
        {
            _uow.GetGenericRepository<MST_PRICELIST>().InsertOrUpdate(dbPriceList, userLogin, Enums.MenuList.MasterPriceList);
        }
        public List<MST_PRICELIST> GetPriceList(PricelistParamInput input)
        {
            Expression<Func<MST_PRICELIST, bool>> queryFilter = PredicateHelper.True<MST_PRICELIST>(); 

            if (input != null)
            {

                if (!string.IsNullOrEmpty(input.VehicleUsage))
                {
                    var listFunction = input.VehicleUsage.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.VEHICLE_USAGE == null ? "" : c.VEHICLE_USAGE.ToUpper())));
                }
                if (!string.IsNullOrEmpty(input.VehicleType))
                {
                    var listFunction = input.VehicleType.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.VEHICLE_TYPE == null ? "" : c.VEHICLE_TYPE.ToUpper())));
                }
                if (!string.IsNullOrEmpty(input.vendor))
                {
                    var listFunction = input.vendor.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.VENDOR == null ? "" : c.VENDOR.ToString())));
                }
                if (!string.IsNullOrEmpty(input.ZonePricelist))
                {
                    var listFunction = input.ZonePricelist.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.ZONE_PRICE_LIST == null ? "" : c.ZONE_PRICE_LIST.ToUpper())));
                }
            }

            return _priceListRepository.Get(queryFilter, null, "").ToList();
        }
    }
}
