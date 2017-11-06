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
    public class CsfService : ICsfService
    {
        private IUnitOfWork _uow;

        private IGenericRepository<TRA_CSF> _csfRepository;

        public CsfService(IUnitOfWork uow)
        {
            _uow = uow;
            _csfRepository = _uow.GetGenericRepository<TRA_CSF>();
        }

        public List<TRA_CSF> GetCsf()
        {
            return _csfRepository.Get().ToList();
        }
    }
}
