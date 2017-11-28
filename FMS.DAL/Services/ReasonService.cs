using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
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
    public class ReasonService : IReasonService
    {
        public IGenericRepository<MST_REASON> _ReasonRepository;
        public IUnitOfWork _uow;

        public ReasonService (IUnitOfWork uow)
        {
            _uow = uow;
            _ReasonRepository = uow.GetGenericRepository<MST_REASON>();
        }

        public List<MST_REASON> GetReason()
        {
            return _ReasonRepository.Get().ToList();
        }
        public void save(MST_REASON dbReason)
        {
            _uow.GetGenericRepository<MST_REASON>().InsertOrUpdate(dbReason);
            _uow.SaveChanges();
        }

        public MST_REASON GetReasonById(int MstReasonId)
        {
            return _uow.GetGenericRepository<MST_REASON>().GetByID(MstReasonId);
        }
        public void save(MST_REASON dbReason, Login userLogin)
        {
            _uow.GetGenericRepository<MST_REASON>().InsertOrUpdate(dbReason, userLogin, Enums.MenuList.MasterReason);
            _uow.SaveChanges();
        }
    }
}
