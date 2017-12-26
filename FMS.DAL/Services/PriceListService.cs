using FMS.BusinessObject;
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

    }
}
