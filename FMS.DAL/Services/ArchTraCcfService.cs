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
    public class ArchTraCcfService : IArchTraCcfService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_TRA_CCF> _archTraCcfRepository;

        public ArchTraCcfService(IUnitOfWork uow)
        {
            _uow = uow;
            _archTraCcfRepository = _uow.GetGenericRepository<ARCH_TRA_CCF>();
        }
        public void Save(ARCH_TRA_CCF db, Login Login)
        {
            _uow.GetGenericRepository<ARCH_TRA_CCF>().InsertOrUpdate(db, Login, Enums.MenuList.MasterData);
        }
    }
}
