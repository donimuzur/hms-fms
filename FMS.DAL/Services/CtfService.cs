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
    public class CtfService : ICtfService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<TRA_CTF> _traCtfRepository;

        public CtfService(IUnitOfWork uow)
        {
            _uow = uow;
            _traCtfRepository = _uow.GetGenericRepository<TRA_CTF>();
        }
        public List<TRA_CTF> GetCtf()
        {
            return _traCtfRepository.Get().ToList();
        }
        public void Save(TRA_CTF dbCtf)
        {
            _traCtfRepository.InsertOrUpdate(dbCtf);
            _uow.SaveChanges();
        }
    }
}
