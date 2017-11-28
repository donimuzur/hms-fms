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
    public class CtfExtendService : ICtfExtendService
    {
        private IGenericRepository<TRA_CTF_EXTEND> _ctfExtendRepository;
        private IUnitOfWork _uow;
        public CtfExtendService(IUnitOfWork uow)
        {
            _uow = uow;
            _ctfExtendRepository = _uow.GetGenericRepository<TRA_CTF_EXTEND>();
        }
        public void Save(TRA_CTF_EXTEND dbCtfExtend)
        {
            _ctfExtendRepository.InsertOrUpdate(dbCtfExtend);
            _uow.SaveChanges();
        }
        public List<TRA_CTF_EXTEND> GetCtfExtend()
        {
            return _ctfExtendRepository.Get().ToList();
        }
        public void Save(TRA_CTF_EXTEND dbCtf, Login userlogin)
        {
            _ctfExtendRepository.InsertOrUpdate(dbCtf);
            _uow.SaveChanges();
        }
    }
}
