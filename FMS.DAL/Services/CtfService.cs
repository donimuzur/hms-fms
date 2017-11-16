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
        public void Save(TRA_CTF dbCtf, Login userlogin)
        {
            _traCtfRepository.InsertOrUpdate(dbCtf, userlogin , Enums.MenuList.TraCsf);
            _uow.SaveChanges();
        }
        public TRA_CTF GetCtfById(long TraCtfId)
        {
            return _traCtfRepository.GetByID(TraCtfId);
        }

        public void CancelCtf(long id, int Remark, string user)
        {
            var data = _traCtfRepository.GetByID(id);

            if (data != null)
            {
                data.DOCUMENT_STATUS = Enums.DocumentStatus.Cancelled;
                data.MODIFIED_DATE = DateTime.Now;
                data.MODIFIED_BY = user;
                data.IS_ACTIVE = false;
                data.REMARK = Remark;
                _uow.SaveChanges();
            }
        }

    }
}
