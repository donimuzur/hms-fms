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
    public class DelegationService : IDelegationService
    {
        private IUnitOfWork _uow;

        private IGenericRepository<MST_DELEGATION> _delegationRepository;

        public DelegationService(IUnitOfWork uow)
        {
            _uow = uow;
            _delegationRepository = _uow.GetGenericRepository<MST_DELEGATION>();
        }

        public List<MST_DELEGATION> GetDelegation()
        {
            return _delegationRepository.Get().ToList();
        }

        public MST_DELEGATION GetDelegationById(int MstDelegationId)
        {
            return _delegationRepository.GetByID(MstDelegationId);
        }

        public void save(MST_DELEGATION dbDelegation)
        {
            _uow.GetGenericRepository<MST_DELEGATION>().InsertOrUpdate(dbDelegation);
            _uow.SaveChanges();
        }

        public void save(MST_DELEGATION dbDelegation, Login userLogin)
        {
            _uow.GetGenericRepository<MST_DELEGATION>().InsertOrUpdate(dbDelegation, userLogin, Enums.MenuList.MasterDelegation);
            _uow.SaveChanges();
        }
    }
}
