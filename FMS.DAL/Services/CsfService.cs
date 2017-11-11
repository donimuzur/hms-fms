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


        public void saveCsf(TRA_CSF dbTraCsf, Login userlogin)
        {
            _uow.GetGenericRepository<TRA_CSF>().InsertOrUpdate(dbTraCsf, userlogin, Enums.MenuList.TraCsf);
            _uow.SaveChanges();
        }

        public void CancelCsf(long id, int Remark, string user)
        {
            var data = _csfRepository.GetByID(id);

            if (data != null)
            {
                data.DOCUMENT_STATUS = (int)Enums.DocumentStatus.Cancelled;
                data.MODIFIED_DATE = DateTime.Now;
                data.MODIFIED_BY = user;
                data.IS_ACTIVE = false;
                data.REMARK = Remark;
                _uow.SaveChanges();
            }
        }
    }
}
