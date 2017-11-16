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
    public class RemarkService : IRemarkService
    {
        private IUnitOfWork _uow;

        private IGenericRepository<MST_REMARK> _remarkRepository;
        public RemarkService (IUnitOfWork uow)
        {
            _uow = uow;
            _remarkRepository = uow.GetGenericRepository<MST_REMARK>();
        }
        public List<MST_REMARK> GetRemark()
        {
            return _remarkRepository.Get().ToList();
        }

        public MST_REMARK GetRemarkById(int MstRemarkId)
        {
            return _remarkRepository.GetByID(MstRemarkId);
        }
        public List<MST_REMARK> GetRemarkByDoc(int MstDocId)
        {
            return _remarkRepository.Get(x => x.DOCUMENT_TYPE == MstDocId).ToList();
        }

        public void save(MST_REMARK dbRemark)
        {
            _uow.GetGenericRepository<MST_REMARK>().InsertOrUpdate(dbRemark);
            _uow.SaveChanges();
        }

        public void save(MST_REMARK dbRemark, Login userLogin)
        {
            _uow.GetGenericRepository<MST_REMARK>().InsertOrUpdate(dbRemark, userLogin, Enums.MenuList.MasterRemark);
            _uow.SaveChanges();
        }
    }
}
