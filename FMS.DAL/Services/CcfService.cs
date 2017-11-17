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
    public class CcfService : ICcfService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<TRA_CCF> _traCcfRepository;

        public CcfService(IUnitOfWork uow)
        {
            _uow = uow;
            _traCcfRepository = _uow.GetGenericRepository<TRA_CCF>();
        }

        public List<TRA_CCF> GetCcf()
        {
            return _traCcfRepository.Get().ToList();
        }

        public TRA_CCF GetCcfById(long TraCcfId)
        {
            return _traCcfRepository.GetByID(TraCcfId);
        }

        public void Save(TRA_CCF dbCcf, Login userlogin)
        {
            _traCcfRepository.InsertOrUpdate(dbCcf, userlogin, Enums.MenuList.TraCcf);
            _uow.SaveChanges();
        }

        public void CancelCcf(long id, int Remark, string user)
        {
            var data = _traCcfRepository.GetByID(id);

            if (data != null)
            {
                data.DOCUMENT_STATUS = Enums.DocumentStatus.Cancelled;
                data.MODIFIED_DATE = DateTime.Now;
                data.MODIFIED_BY = user;
                data.IS_ACTIVE = false;
                //data.REMARK = Remark;
                _uow.SaveChanges();
            }
        }
    }
}
