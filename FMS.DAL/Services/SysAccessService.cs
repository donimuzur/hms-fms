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
    public class SysAccessService : ISysAccessService
    {
        private IGenericRepository<MST_SYSACCESS> _sysAccessRepository;
        private IUnitOfWork _uow;
        public SysAccessService(IUnitOfWork uow)
        {
            _uow = uow;
            _sysAccessRepository = _uow.GetGenericRepository<MST_SYSACCESS>();
        }

        public List<MST_SYSACCESS> GetSysAccess()
        {
            return _sysAccessRepository.Get().ToList();
        }

        public MST_SYSACCESS GetSysAccessById(int MstSysAccessId)
        {
            return _sysAccessRepository.GetByID(MstSysAccessId);
        }

        public void save(MST_SYSACCESS dbSysAccess)
        {
            _sysAccessRepository.InsertOrUpdate(dbSysAccess);
            _uow.SaveChanges();
        }

        public void save(MST_SYSACCESS dbSysAccess, Login userLogin)
        {
            _sysAccessRepository.InsertOrUpdate(dbSysAccess, userLogin, Enums.MenuList.MasterSysAccess);
            _uow.SaveChanges();
        }
    }
}
