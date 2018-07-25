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
    public class ArchTraCrfService :IArchTraCrfService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_TRA_CRF> _archTraCrfRepository;
        public ArchTraCrfService(IUnitOfWork uow)
        {
            _uow = uow;
            _archTraCrfRepository = _uow.GetGenericRepository<ARCH_TRA_CRF>();
        }

        public void Save(ARCH_TRA_CRF db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_TRA_CRF>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }
        
        public List<ARCH_TRA_CRF> GetList()
        {
            return _archTraCrfRepository.Get().ToList();
        }

        public ARCH_TRA_CRF GetById(int id)
        {
            return _archTraCrfRepository.GetByID(id);
        }
    }
}
