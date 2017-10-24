using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.DAL.Services
{
    public class EpafService : IEpafService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<MST_EPAF> _epafRepository;
        public EpafService(IUnitOfWork uow)
        {
            _uow = uow;
            _epafRepository = _uow.GetGenericRepository<MST_EPAF>();
        }

        public List<MST_EPAF> GetEpaf()
        {
            return _epafRepository.Get().ToList();
        }
    }
}
