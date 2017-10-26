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
    public class SysAccessService : ISysAccessService
    {
        private IGenericRepository<MST_SYSACCESS> _sysAccessRepository;\
        private IUnitOfWork _uow;

        public SysAccessService(IUnitOfWork uow)
        {
            _uow = uow;
            _sysAccessRepository = uow.GetGenericRepository<MST_SYSACCESS>();
        }
        
        public List<MST_SYSACCESS> GetSysAccess()
        {
            return _sysAccessRepository.Get().ToList();
        }

        public MST_SYSACCESS GetSysAccessById(int MstSysAccessId)
        {
            return _sysAccessRepository.GetByID(MstSysAccessId);
        }

        public void Save(MST_SYSACCESS dbSysAccess)
        {
            _sysAccessRepository.InsertOrUpdate(dbSysAccess);
            _uow.SaveChanges();
        }
    }
}
