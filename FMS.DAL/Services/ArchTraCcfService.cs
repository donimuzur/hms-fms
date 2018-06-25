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
        private IGenericRepository<ARCH_TRA_CCF_DETAIL> _archTraCcfDetailRepository;

        public ArchTraCcfService(IUnitOfWork uow)
        {
            _uow = uow;
            _archTraCcfRepository = _uow.GetGenericRepository<ARCH_TRA_CCF>();
            _archTraCcfDetailRepository = _uow.GetGenericRepository<ARCH_TRA_CCF_DETAIL>();
        }
        public void Save(ARCH_TRA_CCF db, Login Login)
        {
            _uow.GetGenericRepository<ARCH_TRA_CCF>().InsertOrUpdate(db, Login, Enums.MenuList.MasterData);
        }
        public List<ARCH_TRA_CCF> GetCcf()
        {
            return _archTraCcfRepository.Get().ToList();
        }

        public ARCH_TRA_CCF GetCcfById(long id)
        {
            return _archTraCcfRepository.GetByID(id);
        }

        public List<ARCH_TRA_CCF_DETAIL> GetCcfD1()
        {
            return _archTraCcfDetailRepository.Get().ToList();
        }
    }
}
