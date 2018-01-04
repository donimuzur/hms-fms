using FMS.BusinessObject.Dto;
using FMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.BLL
{
    public interface IRoleBLL
    {
        Enums.UserRole GetUserRole(string RoleName);
        List<RoleDto> GetRoles();
    }
}
