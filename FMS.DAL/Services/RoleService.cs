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
        private ILogger _logger;
        private IGenericRepository<role> _roleRepository;
        private string IncludeTables = "";

        public RoleService(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _roleRepository = _uow.GetGenericRepository<role>();
        }

        public List<role> GetRoles()
        {
            return _roleRepository.Get().ToList();
        }

        public role GetRoleById(int id)
        {
            return _roleRepository.GetByID(id);
        }
    }
}
