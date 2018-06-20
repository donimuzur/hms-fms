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
    public class ArchTraCsfService : IArchTraCsfService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_TRA_CSF> _archTraCsfRepository;

        public ArchTraCsfService(IUnitOfWork uow)
        {
            _uow = uow;
            _archTraCsfRepository = _uow.GetGenericRepository<ARCH_TRA_CSF>();
        }

        public void Save(ARCH_TRA_CSF db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_TRA_CSF>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }
    }
}
