using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;

namespace FMS.Contract.Service
{
    public interface IRoleService
    {
        List<MST_SYSACCESS> GetRoles();

        string getRoleAliasByRoleName(string RoleName);
        MST_SYSACCESS GetRoleById(int id);
    }
}
