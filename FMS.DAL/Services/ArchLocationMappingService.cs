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
    }
}
