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
    public class ArchTraCtfService : IArchTraCtfService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_TRA_CTF> _archTraCtfRepository;
        public ArchTraCtfService(IUnitOfWork uow)
        {
            _uow = uow;
            _archTraCtfRepository = _uow.GetGenericRepository<ARCH_TRA_CTF>();
        }
        public void Save(ARCH_TRA_CTF db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_TRA_CTF>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }

    }
}
