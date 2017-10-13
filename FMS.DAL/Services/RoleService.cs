using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;
using NLog;

namespace FMS.DAL.Services
{
    public class RoleService : IRoleService
    {
        private IUnitOfWork _uow;
        
        private IGenericRepository<MST_SYSACCESS> _roleRepository;
        //private string IncludeTables = "";

        public RoleService(IUnitOfWork uow)
        {
            _uow = uow;

            _roleRepository = _uow.GetGenericRepository<MST_SYSACCESS>();
        }

        public List<MST_SYSACCESS> GetRoles()
        {
            return _roleRepository.Get().ToList();
        }

        public MST_SYSACCESS GetRoleById(int id)
        {
            return _roleRepository.GetByID(id);
        }
    }
}
