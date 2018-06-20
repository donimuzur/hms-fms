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
    public class ArchTraCafService : IArchTraCafService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_TRA_CAF> _archTraCafRepository;

        public ArchTraCafService(IUnitOfWork uow)
        {
            _uow = uow;
            _archTraCafRepository = _uow.GetGenericRepository<ARCH_TRA_CAF>();
        }
        public void Save(ARCH_TRA_CAF db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_TRA_CAF>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }
    }
}
