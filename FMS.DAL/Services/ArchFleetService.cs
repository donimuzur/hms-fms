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
    public class ArchFleetService : IArchFleetService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_MST_FLEET> _archFleetRepository;

        public ArchFleetService(IUnitOfWork uow)
        {
            _uow = uow;
            _archFleetRepository = uow.GetGenericRepository<ARCH_MST_FLEET>();
        }
        public void Save(ARCH_MST_FLEET db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_MST_FLEET>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }
    }
}
