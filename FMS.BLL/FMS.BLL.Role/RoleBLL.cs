using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.Core;
using FMS.DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BLL.Role
{
    public class RoleBLL : IRoleBLL
    {
        private IRoleService _roleService;
        private IUnitOfWork _uow;
        public RoleBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _roleService = new RoleService(_uow);
        }
        public Enums.UserRole GetUserRole(string RoleName)
        {
            var role = _roleService.getRoleAliasByRoleName(RoleName);
            Enums.UserRole roles;
            var a = Enum.TryParse(role, out roles);
            return roles;
        }
        public List<RoleDto> GetRoles()
        {
            var data = _roleService.GetRoles();
            var redata = AutoMapper.Mapper.Map<List<RoleDto>>(data);
            return redata;
        }
    }
}
