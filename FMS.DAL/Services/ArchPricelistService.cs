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
    }
}
