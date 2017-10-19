using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;
using NLog;

namespace FMS.DAL.Services
{
    public class PenaltyService : IPenaltyService
    {
        private IUnitOfWork _uow;

        private IGenericRepository<MST_PENALTY> _penaltyRepository;

        public PenaltyService(IUnitOfWork uow)
        {
            _uow = uow;

            _penaltyRepository = _uow.GetGenericRepository<MST_PENALTY>();
        }
        public List<MST_PENALTY> GetPenalty()
        {
            return _penaltyRepository.Get().ToList();
        }

        public MST_PENALTY GetPenaltyById(int MstPenaltyID)
        {
            return _penaltyRepository.GetByID(MstPenaltyID);
        }
    }
}
