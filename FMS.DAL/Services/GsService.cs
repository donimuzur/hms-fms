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
    public class GsService : IGsService
    {
        private IGenericRepository<MST_GS> _gsRepository;
        private IUnitOfWork _uow;

        public GsService(IUnitOfWork uow)
        {
            _uow = uow;
            _gsRepository = _uow.GetGenericRepository<MST_GS>();
        }

        public List<MST_GS> GetGs()
        {
            return _gsRepository.Get().ToList();
        }

        public MST_GS GetGsById(int MstGsId)
        {
            return _gsRepository.GetByID(MstGsId);
        }
        
    }
}
