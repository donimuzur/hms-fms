using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Core;
using FMS.DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.DAL.Services
{
   
    public class ArchTraTemporaryService : IArchTraTemporaryService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_TRA_TEMPORARY> _archTraTemporaryRepository;

        public ArchTraTemporaryService(IUnitOfWork uow)
        {
            _uow = uow;
            _archTraTemporaryRepository = _uow.GetGenericRepository<ARCH_TRA_TEMPORARY>();
        }
        public void Save(ARCH_TRA_TEMPORARY db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_TRA_TEMPORARY>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }

    }
}
