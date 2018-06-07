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
    public class ArchEpafService : IArchEpafService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_MST_EPAF> _archEpafRepository;
        public ArchEpafService (IUnitOfWork uow)
        {
            _uow = uow;
            _archEpafRepository = uow.GetGenericRepository<ARCH_MST_EPAF>();
        }
        public void Save(ARCH_MST_EPAF db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_MST_EPAF>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }
    }
}
