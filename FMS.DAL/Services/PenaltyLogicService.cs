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
    public class PenaltyLogicService : IPenaltyLogicService
    {
        private IGenericRepository<MST_PENALTY_LOGIC> _penaltyLogicRepository;
        private IUnitOfWork _uow;
        public PenaltyLogicService(IUnitOfWork uow)
        {
            _uow = uow;
            _penaltyLogicRepository = uow.GetGenericRepository<MST_PENALTY_LOGIC>();
        }

        public MST_PENALTY_LOGIC GetPenaltyLogicByID(int MstPenaltyLogicId)
        {
            return _penaltyLogicRepository.GetByID(MstPenaltyLogicId);
        }

        public List<MST_PENALTY_LOGIC> GetPenaltyLogic()
        {
            return _penaltyLogicRepository.Get().ToList();
        }

        public void Save(MST_PENALTY_LOGIC dbPenaltyLogic)
        {
            _penaltyLogicRepository.InsertOrUpdate(dbPenaltyLogic);
            _uow.SaveChanges();
        }

        public void Save(MST_PENALTY_LOGIC dbPenaltyLogic, Login userLogin)
        {
            _penaltyLogicRepository.InsertOrUpdate(dbPenaltyLogic, userLogin, Enums.MenuList.MasterPenaltyLogic);
            _uow.SaveChanges();
        }

    }
}
